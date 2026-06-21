
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FileEncoding.Helper; // TextEncodingDetect
using App.Helper;

namespace FileUtility
{
    public static class FileHelper
    {
        const string newlineCRLF = "\r\n"; // Carriage Return + Line Feed: Environment.NewLine
        public const int CodePageWindowsLatin1252 = 1252;
        public const int EncodingUnicodeUtf8 = 65001;
        public const string EncodingISO_8859_1 = "ISO-8859-1";
        public const int NullStringIndex = -1;

        // ─── File existence ───────────────────────────────────────────────────────

        public static bool FileExists(string filePath, bool prompt = false)
        {
            bool exists = File.Exists(filePath);
            if (!exists && prompt)
                MessageBox.Show(
                    "Cannot find the file:" + newlineCRLF + filePath,
                    AppHelper.AppTitle + " - File not found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            return exists;
        }

        public static bool DeleteFile(string filePath, bool promptError = false)
        {
            if (!FileExists(filePath)) return true;
            if (!IsFileAccessible(filePath, promptClose: promptError, promptRetry: promptError))
                return false;
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                if (promptError)
                    AppHelper.ShowError(ex, "Cannot delete the file:" + newlineCRLF + filePath);
                return false;
            }
        }

        public static bool DeleteFilteredFiles(string folderPath, string filter,
            bool promptError = false)
        {
            if (!Directory.Exists(folderPath)) return true;
            string[] files = Directory.GetFileSystemEntries(folderPath, filter);
            foreach (string file in files)
                if (!DeleteFile(file, promptError)) return false;
            return true;
        }

        public static bool IsFileAccessible(string filePath,
            bool prompt = false,
            bool promptClose = false,
            bool missingOk = false,
            bool promptRetry = false,
            bool readOnly = false,
            bool write = true)
        {
Retry:
            if (missingOk)
            {
                if (!FileExists(filePath)) return true;
            }
            else
            {
                if (!FileExists(filePath, prompt)) return false;
            }

            DialogResult response = DialogResult.Cancel;
            FileStream fs = null;
            try
            {
                var mode = FileMode.Open;
                var access = write ? FileAccess.ReadWrite : FileAccess.Read;
                fs = new FileStream(filePath, mode, access, FileShare.ReadWrite);
                fs.Close();
                fs = null;
                return true;
            }
            catch (Exception ex)
            {
                if (prompt)
                {
                    AppHelper.ShowError(ex, "IsFileAccessible",
                        "Cannot access the file:" + newlineCRLF + filePath);
                }
                else if (promptClose)
                {
                    var buttons = promptRetry
                        ? MessageBoxButtons.RetryCancel
                        : MessageBoxButtons.OK;
                    string question = promptRetry ? "\r\nDo you want to retry?" : "";
                    response = MessageBox.Show(
                        "The file is not accessible for writing:" + newlineCRLF +
                        filePath + newlineCRLF +
                        "Please close it or change its protection attributes." +
                        question,
                        AppHelper.AppTitle, buttons, MessageBoxIcon.Exclamation);
                }
            }
            finally
            {
                fs?.Dispose();
            }

            if (response == DialogResult.Retry) goto Retry;
            return false;
        }

        // ─── Read / Write ─────────────────────────────────────────────────────────

        public static StringBuilder ReadFileToStringBuilder(string filePath,
            bool readOnly = false, bool utf8 = false)
        {
            var sb = new StringBuilder();
            if (!FileExists(filePath, prompt: true)) return sb;

            FileStream fs = null;
            try
            {
                var share = readOnly ? FileShare.ReadWrite : FileShare.Read;
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, share);
                var enc = utf8
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(CodePageWindowsLatin1252);
                using (var sr = new StreamReader(fs, enc))
                {
                    fs = null;
                    bool first = true;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!first) sb.Append(newlineCRLF);
                        first = false;
                        sb.Append(line);
                    }
                }
                return sb;
            }
            catch (Exception ex)
            {
                AppHelper.ShowError(ex, "ReadFileToStringBuilder");
                return null;
            }
            finally
            {
                fs?.Dispose();
            }
        }

        public static StringBuilder ReadFileToStringBuilder(string filePath, Encoding encoding,
            bool readOnly = false)
        {
            var sb = new StringBuilder();
            if (!FileExists(filePath, prompt: true)) return sb;

            FileStream fs = null;
            try
            {
                var share = readOnly ? FileShare.ReadWrite : FileShare.Read;
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, share);
                using (var sr = new StreamReader(fs, encoding))
                {
                    fs = null;
                    bool first = true;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!first) sb.Append(newlineCRLF);
                        first = false;
                        sb.Append(line);
                    }
                }
                return sb;
            }
            catch (Exception ex)
            {
                AppHelper.ShowError(ex, "ReadFileToStringBuilder");
                return null;
            }
            finally
            {
                fs?.Dispose();
            }
        }

        public static bool WriteFile(string filePath, StringBuilder content,
            ref string errorMessage,
            bool defaultEncoding = false,
            bool iso88591 = false,
            bool utf8 = false,
            bool utf16 = false,
            int encodingCodePage = 0,
            string encodingName = "",
            bool prompt = true)
        {
            if (!DeleteFile(filePath, promptError: prompt)) return false;

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            try
            {
                Encoding enc = Encoding.Default;
                if (!defaultEncoding)
                {
                    if (iso88591)
                        enc = Encoding.GetEncoding(EncodingISO_8859_1);
                    else if (utf8)
                        enc = Encoding.UTF8;
                    else if (utf16)
                        enc = Encoding.Unicode;
                    else if (encodingCodePage > 0)
                        enc = Encoding.GetEncoding(encodingCodePage);
                    else if (!string.IsNullOrEmpty(encodingName))
                        enc = Encoding.GetEncoding(encodingName);
                    else
                        enc = Encoding.GetEncoding(CodePageWindowsLatin1252);
                }

                using (var sw = new StreamWriter(filePath, append: false, encoding: enc))
                    sw.Write(content.ToString());

                return true;
            }
            catch (Exception ex)
            {
                string msg = "Cannot write data to file:" + newlineCRLF + filePath;
                errorMessage = msg + newlineCRLF + ex.Message;
                if (prompt) AppHelper.ShowError(ex, "WriteFile", msg);
                return false;
            }
        }

        public static bool WriteFile(string filePath, StringBuilder content,
            bool defaultEncoding = false,
            bool iso88591 = false,
            bool utf8 = false,
            bool utf16 = false,
            int encodingCodePage = 0,
            string encodingName = "",
            bool prompt = true)
        {
            string unused = "";
            return WriteFile(filePath, content, ref unused, defaultEncoding, iso88591, utf8, utf16,
                encodingCodePage, encodingName, prompt);
        }

        // ─── Encoding detection ───────────────────────────────────────────────────

        public static Encoding ReadEncodingTed(string filePath, ref string encodingName,
            bool useDefaultEncoding = false)
        {
            encodingName = "Detection failed";
            try
            {
                const int maxBytes = 4096;
                byte[] buffer;
                using (var fs = new FileStream(filePath, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                {
                    int toRead = (int)Math.Min(fs.Length, maxBytes);
                    buffer = new byte[toRead];
                    // fs.Read(buffer, 0, toRead); // CA2022
                    fs.ReadExactly(buffer, 0, toRead);
                }

                var detector = new TextEncodingDetect();
                var detected = detector.DetectEncoding(buffer, buffer.Length);

                switch (detected)
                {
                    case TextEncodingDetect.Encoding.Utf8Bom:
                        encodingName = "UTF-8 with BOM";
                        return Encoding.UTF8;
                    case TextEncodingDetect.Encoding.Utf8Nobom:
                        encodingName = "UTF-8 without BOM";
                        return Encoding.UTF8;
                    case TextEncodingDetect.Encoding.Utf16LeBom:
                        encodingName = "UTF-16 LE with BOM";
                        return Encoding.Unicode;
                    case TextEncodingDetect.Encoding.Utf16LeNoBom:
                        encodingName = "UTF-16 LE without BOM";
                        return Encoding.Unicode;
                    case TextEncodingDetect.Encoding.Utf16BeBom:
                        encodingName = "UTF-16 BE with BOM";
                        return Encoding.BigEndianUnicode;
                    case TextEncodingDetect.Encoding.Utf16BeNoBom:
                        encodingName = "UTF-16 BE without BOM";
                        return Encoding.BigEndianUnicode;
                    case TextEncodingDetect.Encoding.Ascii:
                        encodingName = "ASCII";
                        return useDefaultEncoding
                            ? Encoding.Default
                            : Encoding.GetEncoding(CodePageWindowsLatin1252);
                    case TextEncodingDetect.Encoding.Ansi:
                        encodingName = "ANSI (Windows-1252)";
                        return Encoding.GetEncoding(CodePageWindowsLatin1252);
                    default:
                        encodingName = useDefaultEncoding ? "Default" : "Windows-1252";
                        return useDefaultEncoding
                            ? Encoding.Default
                            : Encoding.GetEncoding(CodePageWindowsLatin1252);
                }
            }
            catch (Exception ex)
            {
                encodingName = "Error: " + ex.Message;
                return Encoding.GetEncoding(CodePageWindowsLatin1252);
            }
        }

        // ─── Formatting ───────────────────────────────────────────────────────────

        public static string FormatFileSize(long sizeBytes,
            bool detail = false, bool suppressZeroDecimal = false)
        {
            float kb = (float)Math.Round(sizeBytes / 1024.0, 1);
            float mb = (float)Math.Round(sizeBytes / (1024.0 * 1024), 1);
            float gb = (float)Math.Round(sizeBytes / (1024.0 * 1024 * 1024), 1);
            string unit = sizeBytes < 2 ? " byte" : " bytes";

            if (detail)
            {
                string s = FormatNumber(sizeBytes) + unit;
                if (kb >= 1) s += " (" + FormatNumber(kb) + " KB";
                if (mb >= 1) s += " = " + FormatNumber(mb) + " MB";
                if (gb >= 1) s += " = " + FormatNumber(gb) + " GB";
                if (kb >= 1 || mb >= 1 || gb >= 1) s += ")";
                return s;
            }
            else
            {
                if (gb >= 1) return FormatNumber(gb, suppressZeroDecimal) + " GB";
                if (mb >= 1) return FormatNumber(mb, suppressZeroDecimal) + " MB";
                if (kb >= 1) return FormatNumber(kb, suppressZeroDecimal) + " KB";
                return FormatNumber(sizeBytes, suppressDecimal: true) + unit;
            }
        }

        public static string FormatNumber(float value, bool suppressZeroDecimal = true,
            int decimals = 1)
        {
            var nfi = new NumberFormatInfo
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ".",
                NumberGroupSizes = new[] { 3, 3, 3 },
                NumberDecimalDigits = decimals
            };
            string s = value.ToString("n", nfi);
            if (suppressZeroDecimal)
            {
                if (decimals == 1) s = s.Replace(".0", "");
            }
            return s;
        }

        public static string FormatNumber(long value, bool suppressDecimal = true)
        {
            var nfi = new NumberFormatInfo
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ".",
                NumberGroupSizes = new[] { 3, 3, 3 },
                NumberDecimalDigits = 0
            };
            return value.ToString("n0", nfi);
        }

        // ─── Accent removal ───────────────────────────────────────────────────────

        public static StringBuilder RemoveAccents(StringBuilder source, bool lowercase = true)
        {
            if (source.Length == 0) return new StringBuilder();
            string text = source.ToString();
            if (lowercase) text = text.ToLower();
            return RemoveDiacritics(text);
        }

        private static StringBuilder RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            const char charAe = 'æ';
            const char charOe = 'œ';
            const char charAE = 'Æ';
            const char charOE = 'Œ';
            const char char3Dots = '…';

            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    if (c == charAE) { sb.Append('A'); sb.Append('E'); }
                    else if (c == charOE) { sb.Append('O'); sb.Append('E'); }
                    else if (c == charAe) { sb.Append('a'); sb.Append('e'); }
                    else if (c == charOe) { sb.Append('o'); sb.Append('e'); }
                    else if (c == char3Dots) sb.Append("...");
                    else sb.Append(c);
                }
            }
            return sb;
        }
    }
}