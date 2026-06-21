
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FileUtility;
using Dictionary.Helper; // SortableDictionary

namespace TextDiffOptions
{
    internal static class TextDiffProcessor
    {
        // ─── Inner types ──────────────────────────────────────────────────────────

        public class PageInfo
        {
            public int SourceIndex { get; }
            public StringBuilder PageContent { get; }

            public PageInfo(int sourceIndex, StringBuilder pageContent)
            {
                SourceIndex = sourceIndex;
                PageContent = pageContent;
            }
        }

        public class WordInfo
        {
            public string ConcatWord;
            public string Word1;
            public string Word2;
            public int ConcatCount;
            public int Count1;
            public int Count2;
        }

        // ─── Accent removal ───────────────────────────────────────────────────────

        public static bool RemoveAccents(string filePath,
            ref StringBuilder source,
            ref StringBuilder dest,
            bool lowercase)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);
            dest = FileHelper.RemoveAccents(source, lowercase);
            return true;
        }

        // ─── File info ────────────────────────────────────────────────────────────

        private static void ReadFileInfo(string filePath,
            ref string sizeText, ref string dateText, ref long fileSize)
        {
            var fi = new FileInfo(filePath);
            fileSize = fi.Length;
            sizeText = FileHelper.FormatFileSize(fileSize, detail: true);
            dateText = fi.LastWriteTime.ToString();
        }

        // ─── Add info header ──────────────────────────────────────────────────────

        public static bool AddInfo(int fileNumber, string filePath, string originalFilePath,
            string encodingName,
            ref StringBuilder source,
            ref StringBuilder originalSource,
            ref StringBuilder dest,
            int pageNumber = 0, int totalPages = 0,
            int sourceOrigIndex = -1,
            bool showFileName = true,
            bool showFinalSize = false)
        {
            if (source == null) return false;
            if (originalSource == null) return false;
            if (dest == null) dest = new StringBuilder();

            string sizeText = "", dateText = "";
            long fileSize = 0;
            ReadFileInfo(originalFilePath, ref sizeText, ref dateText, ref fileSize);

            var sbTmp = new StringBuilder();
            if (showFileName)
            {
                sbTmp.AppendLine("File #" + fileNumber + " : " +
                    Path.GetFileName(originalFilePath) + " : " +
                    Path.GetDirectoryName(originalFilePath));
                sbTmp.AppendLine("Size = " + sizeText + ", Date = " + dateText);
            }

            if (totalPages > 1)
            {
                sbTmp.AppendLine("Page " + pageNumber + "/" + totalPages);
                long end = sourceOrigIndex + originalSource.Length;
                sbTmp.Append("Section = [" + sourceOrigIndex + " - " + end + "[");
                int pageSize = (int)(end - sourceOrigIndex);
                sbTmp.Append(" : " + FileHelper.FormatFileSize(pageSize, detail: true));
                sbTmp.Append(" : " + FileHelper.FormatFileSize(end, detail: true));
                float pct = (float)end / fileSize;
                sbTmp.Append(" : " + pct.ToString("0.00%"));
                sbTmp.Append(Const.newlineCRLF);
            }

            if (showFinalSize)
            {
                long finalSize = source.Length;
                sbTmp.AppendLine("Final size = " + 
                    FileHelper.FormatFileSize(finalSize, detail: true));
            }

            sbTmp.AppendLine("Encoding = " + encodingName);
            dest = sbTmp.Append(source);
            return true;
        }

        // ─── Pagination ───────────────────────────────────────────────────────────

        public static bool PaginateFiles(string filePath1, string filePath2,
            int pageSize, ref int pageCount,
            ref Dictionary<int, PageInfo> pages1,
            ref Dictionary<int, PageInfo> pages2,
            bool applyRatio,
            Encoding encoding1, Encoding encoding2)
        {
            var sb1 = FileHelper.ReadFileToStringBuilder(filePath1, encoding1);
            var sb2 = FileHelper.ReadFileToStringBuilder(filePath2, encoding2);
            int len1 = sb1.Length;
            int len2 = sb2.Length;

            int maxLen = Math.Max(len1, len2);

            pageCount = maxLen / pageSize;
            long remainder = maxLen % pageSize;
            if (remainder > 0) pageCount++;

            float ratio = 1.0f;
            if (applyRatio && len1 > 0)
                ratio = (float)len2 / len1;

            pages1 = new Dictionary<int, PageInfo>();
            pages2 = new Dictionary<int, PageInfo>();
            int cumul1 = 0, cumul2 = 0;

            for (int pageNum = 0; pageNum < pageCount; pageNum++)
            {
                int len1Page = pageSize;
                int len2Page = pageSize;

                if (applyRatio)
                {
                    len1Page = (int)(len1Page / ratio);
                    if (pageNum == pageCount - 1 && cumul1 + len1Page < len1)
                        len1Page = len1 - cumul1;
                }

                int idx1 = cumul1, idx2 = cumul2;

                Paginate(idx1, len1Page, sb1, out var page1);
                pages1.Add(pageNum, new PageInfo(idx1, page1));

                Paginate(idx2, len2Page, sb2, out var page2);
                pages2.Add(pageNum, new PageInfo(idx2, page2));

                cumul1 += page1.Length;
                cumul2 += page2.Length;
            }

            return true;
        }

        private static void Paginate(int startIndex, int chunkSize,
            StringBuilder source, out StringBuilder destPage)
        {
            int srcLen = source.Length;
            if (startIndex + chunkSize > srcLen)
            {
                chunkSize = srcLen - startIndex;
                if (chunkSize < 0)
                {
                    chunkSize = 0;
                    if (startIndex > srcLen) startIndex = srcLen;
                }
            }

            char[] chars = new char[chunkSize];
            source.CopyTo(startIndex, chars, 0, chunkSize);
            destPage = new StringBuilder();
            foreach (char c in chars)
                destPage.Append(c);
        }

        // ─── Non-breaking space removal ───────────────────────────────────────────

        public static void RemoveNonBreakingSpaces(string filePath,
            ref StringBuilder source,
            ref StringBuilder dest)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);

            dest = source
                .Replace((char)Const.AsciiNonBreakingSpaceCode, ' ')
                .Replace((char)Const.Utf16NarrowNonBreakingSpaceCode, ' ');

            // Replace en-dashes surrounded by spaces with hyphens
            dest = source.Replace(" " + (char)Const.AsciiEnDashCode + " ", " - ");
            dest = source.Replace((char)Const.AsciiEnDashCode + " ", "- ");
            dest = source.Replace(" " + (char)Const.AsciiEnDashCode, " -");

            dest = source.Replace(Const.String3Dots, "...");
        }

        // ─── Whitespace removal ───────────────────────────────────────────────────

        public static void RemoveSpaces(string filePath,
            ref StringBuilder source, ref StringBuilder dest)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);
            dest = new StringBuilder();

            string[] paragraphs = source.ToString().Split(new[] { Const.newlineCRLF }, StringSplitOptions.None);
            foreach (string para in paragraphs)
            {
                string trimmed = para.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                dest.AppendLine(trimmed);
            }
        }

        // ─── Lowercase ────────────────────────────────────────────────────────────

        public static bool RemoveUppercase(string filePath,
            ref StringBuilder source, ref StringBuilder dest)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);
            dest = new StringBuilder();
            dest.Append(source.ToString().ToLower());
            return true;
        }

        // ─── Split paragraphs into sentences ─────────────────────────────────────

        public static bool SplitParagraphsIntoSentences(string filePath,
            ref StringBuilder source, ref StringBuilder dest)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);
            dest = new StringBuilder();

            string quote = ((char)Const.AsciiDoubleQuoteCode).ToString();
            string pattern = @"(?<=[\.!\?;:—]|\." + quote + @")\s+";
            string[] sentences = Regex.Split(source.ToString(), pattern);

            foreach (string sentence in sentences)
                dest.AppendLine(sentence);

            return true;
        }

        // ─── Punctuation removal ──────────────────────────────────────────────────

        public static bool RemovePunctuation(string filePath,
            ref StringBuilder source, ref StringBuilder dest,
            bool compareWords, bool compareParagraphs, bool compareNumbers)
        {
            if (source == null) source = FileHelper.ReadFileToStringBuilder(filePath);
            dest = new StringBuilder();

            const string wordPattern = @"\w+";
            bool suppressDoubleCrLf = !compareParagraphs;
            char[] sentenceSeps = Const.SentenceSeparators.ToCharArray();

            string[] paragraphs = source.ToString().Split(new[] { Const.newlineCRLF }, StringSplitOptions.None);

            bool lastWasNewline = false;
            foreach (string para in paragraphs)
            {
                if (para.Length == 0) continue;

                string[] sentences = para.Split(sentenceSeps);
                foreach (string sentence in sentences)
                {
                    if (sentence.Length == 0) continue;
                    if (sentence.Length == 1 &&
                        (byte)sentence[0] == Const.AsciiDoubleQuoteCode) continue;

                    var matches = Regex.Matches(sentence, wordPattern);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        lastWasNewline = false;
                        string word = matches[i].Value;

                        if (!compareNumbers && compareWords && IsNumeric(word))
                        {
                            lastWasNewline = true;
                            continue;
                        }

                        dest.Append(word);
                        if (compareWords)
                        {
                            dest.Append(Const.newlineCRLF);
                            lastWasNewline = true;
                        }
                        else
                        {
                            dest.Append(" ");
                        }
                    }

                    if (!suppressDoubleCrLf || !lastWasNewline)
                        dest.Append(Const.newlineCRLF);
                }

                if (!suppressDoubleCrLf || !lastWasNewline)
                    dest.Append(Const.newlineCRLF);
            }

            return true;
        }

        private static bool IsNumeric(string s)
        {
            return double.TryParse(s, out _);
        }

        // ─── Quote normalization ──────────────────────────────────────────────────

        public static void NormalizeQuotes(StringBuilder source, ref StringBuilder dest)
        {
            string quote = ((char)Const.AsciiSingleQuoteCode).ToString();
            string quote2 = ((char)Const.AsciiSingleQuote2Code).ToString();
            string openSingle2 = ((char)Const.AsciiLeftSingleQuotationMarkCode).ToString();
            string closeSingle2 = ((char)Const.AsciiRightSingleQuotationMarkCode).ToString();
            string closeAccent = ((char)Const.AsciiAcuteAccentCode).ToString();

            string doubleQuote = ((char)Const.AsciiDoubleQuoteCode).ToString();
            string openDouble3 = ((char)Const.AsciiLeftDoubleQuotationMarkCode).ToString();
            string closeDouble3 = ((char)Const.AsciiRightDoubleQuotationMarkCode).ToString();

            string openFrench = ((char)Const.AsciiLeftGuillemetCode).ToString();
            string closeFrench = ((char)Const.AsciiRightGuillemetCode).ToString();
            string nbSpace = ((char)Const.AsciiNonBreakingSpaceCode).ToString();
            string thinNbSpace = char.ConvertFromUtf32(Const.Utf16NarrowNonBreakingSpaceCode);

            dest = source
                .Replace(quote2, quote)
                .Replace(openSingle2, quote)
                .Replace(closeSingle2, quote)
                .Replace(openDouble3, doubleQuote)
                .Replace(closeDouble3, doubleQuote)
                .Replace(closeAccent, quote)
                .Replace(openFrench + thinNbSpace, doubleQuote)
                .Replace(thinNbSpace + closeFrench, doubleQuote)
                .Replace(openFrench + nbSpace, doubleQuote)
                .Replace(nbSpace + closeFrench, doubleQuote)
                .Replace(openFrench + " ", doubleQuote)
                .Replace(" " + closeFrench, doubleQuote)
                .Replace(openFrench, doubleQuote)
                .Replace(closeFrench, doubleQuote);
        }

        // ─── Hyphenated word merging ──────────────────────────────────────────────

        public static void MergeHyphenatedWords(StringBuilder wordsSrc,
            ref StringBuilder wordsDest, int fileNumber, string originalFilePath)
        {
            var sb = new StringBuilder();
            wordsDest = new StringBuilder();

            string[] lines = wordsSrc.ToString()
                .Split(new[] { Const.newlineCRLF }, StringSplitOptions.None);
            var freq = new Dictionary<string, int>();

            foreach (string w in lines)
            {
                string w2 = w.Trim();
                if (freq.ContainsKey(w2)) freq[w2]++;
                else freq[w2] = 1;
            }

            var report = new SortableDictionary<string, WordInfo>();
            int total = lines.Length - 1;
            int idx = 0;
            bool merged = false;

            while (idx < total)
            {
                merged = false;
                string word = lines[idx].Trim();
                string nextWord = lines[idx + 1].Trim();

                if (word.Length <= 1 || nextWord.Length <= 1)
                {
                    wordsDest.AppendLine(word);
                    idx++;
                    continue;
                }

                string concat = word + nextWord;
                if (!freq.ContainsKey(word) || !freq.ContainsKey(nextWord) ||
                    !freq.ContainsKey(concat))
                {
                    wordsDest.AppendLine(word);
                    idx++;
                    continue;
                }

                int countWord = freq[word];
                int countNext = freq[nextWord];
                int countConcat = freq[concat];

                if (countWord < countConcat && countNext < countConcat)
                {
                    wordsDest.AppendLine(concat);
                    var wi = new WordInfo
                    {
                        ConcatWord = concat, ConcatCount = countConcat,
                        Word1 = word, Count1 = countWord,
                        Word2 = nextWord, Count2 = countNext
                    };
                    if (report.ContainsKey(concat))
                    {
                        if (countConcat > report[concat].ConcatCount)
                            report[concat].ConcatCount = countConcat;
                    }
                    else
                    {
                        report.Add(concat, wi);
                    }
                    idx++;
                    merged = true;
                }
                else
                {
                    wordsDest.AppendLine(word);
                }
                idx++;
            }

            if (!merged && idx <= total)
            {
                string lastWord = lines[idx].Trim();
                if (lastWord.Length > 0) wordsDest.AppendLine(lastWord);
            }

            string reportPath = Application.StartupPath + "\\" +
                Const.Fusion + fileNumber + Const.ExtTxt;
            string origPath = Application.StartupPath + "\\" +
                Const.FilePrefix + fileNumber + "_" + 
                Const.OrigSuffix + Const.ExtTxt;

            if (sb.Length == 0)
            {
                FileHelper.DeleteFile(reportPath);
                FileHelper.DeleteFile(origPath);
                return;
            }

            FileHelper.WriteFile(reportPath, sb);
            FileHelper.WriteFile(origPath, wordsSrc);
        }
    }
}