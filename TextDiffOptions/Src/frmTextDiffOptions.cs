
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using FileUtility;
using RegistryUtility;
using Shortcut.Helper;
using App.Helper;

namespace TextDiffOptions
{
    public partial class frmTextDiffOptions : Form
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string ExeTextDiffOptions = "TextDiffOptions.exe";
        private const string ShortcutTextDiffOptions = ExeTextDiffOptions + ".lnk";

        private const int PageSize = 50000;

        // ─── Constructor ──────────────────────────────────────────────────────────

        public frmTextDiffOptions()
        {
            InitializeComponent();
            Load += frmTextDiffOptions_Load;
            Shown += frmTextDiffOptions_Shown;

            string icoPath = Path.Combine(Application.StartupPath, "TextDiffOptions.ico");
            if (File.Exists(icoPath))
                Icon = new System.Drawing.Icon(icoPath);
        }

        // ─── Interface ────────────────────────────────────────────────────────────

        public string FilePath1 = "";
        public string FilePath2 = "";

        // ─── State ────────────────────────────────────────────────────────────────

        private int _pageCount = 1;
        private int _pageNumber = 1;
        private string _winDiffPath = "";
        private string _winMergePath = "";
        private string _textDiffToHtmlPath = "";

        private string _shortcutPath =
            Environment.GetFolderPath(Environment.SpecialFolder.SendTo) +
            "\\" + ShortcutTextDiffOptions;

        // ─── Load / Shown ─────────────────────────────────────────────────────────

        private void frmTextDiffOptions_Load(object sender, EventArgs e)
        {
            //Program.SetAppTitle(Program.AppTitle);

            if (ReadWinMergeRegistryKey())
                lbAlgorithm.Items.Add("WinMerge");

            string tdthPath = Application.StartupPath + "\\TextDiffToHtml\\TextDiffToHtml.exe";
            if (FileHelper.FileExists(tdthPath, prompt: true))
            {
                lbAlgorithm.Items.Add("TextDiffToHtml");
                _textDiffToHtmlPath = tdthPath;
            }
        }

        private void frmTextDiffOptions_Shown(object sender, EventArgs e)
        {
            string title = "TextDiffOptions " + Program.AppVersion + 
                " (" + Const.dateVersion + ") - Options interface for TextDiffToHtml";
            if (Program.IsDebug) title += " - Debug";
            Text = title;

            bool configMode = false;
            if (FilePath1.Length == 0)
            {
                configMode = true;
            }
            else
            {
                if (!FileHelper.FileExists(FilePath1, prompt: true)) configMode = true;
                if (!FileHelper.FileExists(FilePath2, prompt: true)) configMode = true;
            }

            if (configMode && Program.IsRelease)
            {
                cmdAddShortcut.Visible = true;
                cmdRemoveShortcut.Visible = true;
                cmdCompare.Visible = false;
                cmdCancel.Visible = false;
                lblPath1.Text = "";
                lblPath2.Text = "";
                CheckShortcut();
                return;
            }
            else
            {
                cmdAddShortcut.Visible = false;
                cmdRemoveShortcut.Visible = false;
            }

            string path1 = FilePath1;
            string path2 = FilePath2;

            if (lbAlgorithm.Items.Count > 0)
                lbAlgorithm.SelectedItem = lbAlgorithm.Items.Contains("WinMerge")
                    ? "WinMerge"
                    : lbAlgorithm.Items[0];

            if (!string.IsNullOrEmpty(path1) && !string.IsNullOrEmpty(path2) &&
                FileHelper.FileExists(path1) && FileHelper.FileExists(path2))
            {
                long len1 = new FileInfo(path1).Length;
                long len2 = new FileInfo(path2).Length;

                if (len2 < len1)
                {
                    string tmp = path1; path1 = path2; path2 = tmp;
                    FilePath1 = path1; FilePath2 = path2;
                }

                if (lbAlgorithm.SelectedItem?.ToString() == "WinDiff" &&
                    (len1 > PageSize || len2 > PageSize))
                    chkPaginate.Checked = true;
            }

            if (Program.IsDebug)
            {
                chkAll.Checked = false;
                chkAccents.Checked = false;
                chkPunctuation.Checked = false;
                chkCase.Checked = false;
                chkNonBreakingSpaces.Checked = false;
                chkSpaces.Checked = false;
                chkQuotes.Checked = false;
                chkInfo.Checked = true;
                chkSentences.Checked = false;
                chkPaginate.Checked = false;
                chkRatio.Checked = false;
                chkParagraphs.Checked = false;
                chkNumbers.Checked = false;
            }

            lblPath1.Text = path1;
            lblPath2.Text = path2;
        }

        private void ShowStatusMessage(string msg)
        {
            toolStripStatusLabel.Text = msg;
            Application.DoEvents();
        }

        // ─── Compare ──────────────────────────────────────────────────────────────

        private bool ConfirmFileSizeForWinDiff(string filePath)
        {
            long size = new FileInfo(filePath).Length;
            if (size <= PageSize * 5) return true;
            return MessageBox.Show(
                "The file size (" + FileHelper.FormatFileSize(size) +
                ") exceeds the recommended limit (" +
                FileHelper.FormatFileSize((long)(PageSize * 1.024), suppressZeroDecimal: true) +
                ") for WinDiff:" + Const.newlineCRLF + filePath + Const.newlineCRLF +
                "Are you sure you want to compare without pagination?",
                Program.AppTitle,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.Cancel;
        }

        private void Compare()
        {
            AppHelper.SetWaitCursor();
            cmdCancel.Enabled = true;
            cmdCompare.Enabled = false;

            string path1 = lblPath1.Text;
            string path2 = lblPath2.Text;
            string origPath1 = path1;
            string origPath2 = path2;

            string startupPath = Application.StartupPath;
            if (!FileHelper.DeleteFilteredFiles(startupPath, Const.TmpFileFilter)) goto Fin;
            if (!FileHelper.DeleteFilteredFiles(startupPath, Const.FusionFileFilter)) goto Fin;

            if (!FileHelper.FileExists(path1, prompt: true)) goto Fin;
            if (!FileHelper.FileExists(path2, prompt: true)) goto Fin;

            StringBuilder sb1, sb2;
            int srcIdx1, srcIdx2;

            string enc1Name = "", enc2Name = "";
            Encoding enc1 = FileHelper.ReadEncodingTed(path1, ref enc1Name, useDefaultEncoding: true);
            Encoding enc2 = FileHelper.ReadEncodingTed(path2, ref enc2Name, useDefaultEncoding: true);

            if (chkPaginate.Checked)
            {
                int pages = _pageCount;
                System.Collections.Generic.Dictionary<int, TextDiffProcessor.PageInfo> d1 = null, d2 = null;
                if (!TextDiffProcessor.PaginateFiles(path1, path2, PageSize, ref pages,
                    ref d1, ref d2, chkRatio.Checked, enc1, enc2)) goto Fin;

                _pageCount = pages;
                sb1 = d1[_pageNumber - 1].PageContent;
                sb2 = d2[_pageNumber - 1].PageContent;
                srcIdx1 = d1[_pageNumber - 1].SourceIndex;
                srcIdx2 = d2[_pageNumber - 1].SourceIndex;
            }
            else
            {
                if (lbAlgorithm.SelectedItem?.ToString() == "WinDiff")
                {
                    if (!ConfirmFileSizeForWinDiff(path1)) goto Fin;
                    if (!ConfirmFileSizeForWinDiff(path2)) goto Fin;
                }
                sb1 = FileHelper.ReadFileToStringBuilder(path1, enc1);
                sb2 = FileHelper.ReadFileToStringBuilder(path2, enc2);
                srcIdx1 = 0; srcIdx2 = 0;
            }

            int writesLeft = 0;
            if (!chkNonBreakingSpaces.Checked) writesLeft++;
            if (!chkSpaces.Checked) writesLeft++;
            if (!chkAccents.Checked) writesLeft++;
            if (!chkCase.Checked) writesLeft++;
            if (!chkParagraphs.Checked) writesLeft++;
            if (!chkPunctuation.Checked) writesLeft++;
            if (!chkQuotes.Checked) writesLeft++;
            int totalWrites = writesLeft;

            bool needsWrite = chkInfo.Checked || writesLeft > 0;

            StringBuilder dest1 = null, dest2 = null;

            var sbOrig1 = new StringBuilder(); sbOrig1.Append(sb1);
            var sbOrig2 = new StringBuilder(); sbOrig2.Append(sb2);

            if (!chkQuotes.Checked)
            {
                TextDiffProcessor.NormalizeQuotes(sb1, ref dest1); sb1 = dest1;
                TextDiffProcessor.NormalizeQuotes(sb2, ref dest2); sb2 = dest2;
                writesLeft--;
            }

            if (!chkAccents.Checked)
            {
                bool lowercase = !chkCase.Checked;
                if (!TextDiffProcessor.RemoveAccents(path1, ref sb1, ref dest1, lowercase)) goto Fin;
                if (!TextDiffProcessor.RemoveAccents(path2, ref sb2, ref dest2, lowercase)) goto Fin;
                sb1 = dest1; sb2 = dest2;
                writesLeft--;
            }

            if (!chkNonBreakingSpaces.Checked)
            {
                TextDiffProcessor.RemoveNonBreakingSpaces(path1, ref sb1, ref dest1); sb1 = dest1;
                TextDiffProcessor.RemoveNonBreakingSpaces(path2, ref sb2, ref dest2); sb2 = dest2;
                writesLeft--;
            }

            if (!chkSpaces.Checked)
            {
                TextDiffProcessor.RemoveSpaces(path1, ref sb1, ref dest1); sb1 = dest1;
                TextDiffProcessor.RemoveSpaces(path2, ref sb2, ref dest2); sb2 = dest2;
                writesLeft--;
            }

            if (!chkCase.Checked)
            {
                if (!TextDiffProcessor.RemoveUppercase(path1, ref sb1, ref dest1)) goto Fin;
                if (!TextDiffProcessor.RemoveUppercase(path2, ref sb2, ref dest2)) goto Fin;
                sb1 = dest1; sb2 = dest2;
                writesLeft--;
            }

            if (!chkParagraphs.Checked && chkPunctuation.Checked)
            {
                if (!TextDiffProcessor.SplitParagraphsIntoSentences(path1, ref sb1, ref dest1)) goto Fin;
                if (!TextDiffProcessor.SplitParagraphsIntoSentences(path2, ref sb2, ref dest2)) goto Fin;
                sb1 = dest1; sb2 = dest2;
                writesLeft--;
            }

            if (!chkPunctuation.Checked)
            {
                bool compareWords = !chkSentences.Checked;
                bool compareParagraphs = chkParagraphs.Checked;
                bool compareNumbers = chkNumbers.Checked;
                if (!TextDiffProcessor.RemovePunctuation(path1, ref sb1, ref dest1,
                    compareWords, compareParagraphs, compareNumbers)) goto Fin;
                if (!TextDiffProcessor.RemovePunctuation(path2, ref sb2, ref dest2,
                    compareWords, compareParagraphs, compareNumbers)) goto Fin;
                sb1 = dest1; sb2 = dest2;
                writesLeft--;
            }

            if (needsWrite)
            {
                path1 = startupPath + "\\" + Const.FilePrefix + "1" + Const.ExtTxt;
                path2 = startupPath + "\\" + Const.FilePrefix + "2" + Const.ExtTxt;

                if (totalWrites == 0)
                {
                    dest1 = FileHelper.ReadFileToStringBuilder(origPath1, enc1);
                    dest2 = FileHelper.ReadFileToStringBuilder(origPath2, enc2);
                }

                if (!WriteFiles(dest1, dest2, path1, origPath1, path2, origPath2,
                    srcIdx1, srcIdx2, sbOrig1, sbOrig2, enc1Name, enc2Name)) goto Fin;
            }

            string quotedPath1 = "\"" + path1 + "\"";
            string quotedPath2 = "\"" + path2 + "\"";
            string cmdArgs = quotedPath1 + " " + quotedPath2;

            var p = new Process();
            string selectedAlgo = lbAlgorithm.SelectedItem?.ToString() ?? "";
            switch (selectedAlgo)
            {
                case "WinDiff":
                    p.StartInfo = new ProcessStartInfo(_winDiffPath);
                    break;
                case "WinMerge":
                    p.StartInfo = new ProcessStartInfo(_winMergePath);
                    break;
                case "TextDiffToHtml":
                    p.StartInfo = new ProcessStartInfo(_textDiffToHtmlPath);
                    break;
                default:
                    goto Fin;
            }
            p.StartInfo.Arguments = cmdArgs;
            p.Start();

Fin:
            UpdatePageButtons();
            cmdCompare.Enabled = true;
            cmdCancel.Enabled = false;
            AppHelper.SetWaitCursor(deactivate: true);
        }

        private bool WriteFiles(StringBuilder page1, StringBuilder page2,
            string destPath1, string origPath1,
            string destPath2, string origPath2,
            int srcIdx1, int srcIdx2,
            StringBuilder orig1, StringBuilder orig2,
            string encName1, string encName2)
        {
            for (int fileNum = 1; fileNum <= 2; fileNum++)
            {
                string destPath = Application.StartupPath + "\\" +
                    Const.FilePrefix + fileNum + Const.ExtTxt;
                StringBuilder page = fileNum == 1 ? page1 : page2;
                StringBuilder srcOrig = fileNum == 1 ? orig1 : orig2;
                string origPath = fileNum == 1 ? origPath1 : origPath2;
                int srcIdx = fileNum == 1 ? srcIdx1 : srcIdx2;
                string encName = fileNum == 1 ? encName1 : encName2;
                string srcPath = fileNum == 1 ? destPath1 : destPath2;

                if (chkInfo.Checked)
                {
                    bool showFinalSize = !chkParagraphs.Checked;
                    int pageNum = _pageNumber;
                    int pageCount = _pageCount;
                    if (!chkPaginate.Checked) { pageNum = 1; pageCount = 1; }
                    StringBuilder outBuf = null;
                    if (!TextDiffProcessor.AddInfo(fileNum, srcPath, origPath, encName,
                        ref page, ref srcOrig, ref outBuf,
                        pageNum, pageCount, srcIdx, true, showFinalSize)) return false;
                    page = outBuf;
                }

                bool mergeHyphenated = !chkParagraphs.Checked;
                if (mergeHyphenated)
                {
                    StringBuilder merged = null;
                    TextDiffProcessor.MergeHyphenatedWords(page, ref merged, fileNum, origPath);
                    page = merged;
                }

                if (!FileHelper.WriteFile(destPath, page)) return false;
            }
            return true;
        }

        // ─── Events ───────────────────────────────────────────────────────────────

        private void cmdCompare_Click(object sender, EventArgs e) => Compare();

        private void chkAll_Click(object sender, EventArgs e)
        {
            bool val = chkAll.Checked;
            chkNonBreakingSpaces.Checked = val;
            chkSpaces.Checked = val;
            chkCase.Checked = val;
            chkAccents.Checked = val;
            chkPunctuation.Checked = val;
            chkQuotes.Checked = val;
            chkNumbers.Checked = val;
            chkSentences.Checked = val;
            chkParagraphs.Checked = val;
            UpdateSentencesAndParagraphsState();
        }

        private void UpdateChkAll()
        {
            chkAll.Checked =
                chkNonBreakingSpaces.Checked && chkSpaces.Checked && chkCase.Checked &&
                chkAccents.Checked && chkPunctuation.Checked && chkQuotes.Checked &&
                chkNumbers.Checked;
        }

        private void chkNonBreakingSpaces_Click(object sender, EventArgs e)
        {
            if (chkNonBreakingSpaces.Checked) chkPunctuation.Checked = true;
            UpdateChkAll();
        }

        private void chkSpaces_Click(object sender, EventArgs e)
        {
            if (chkSpaces.Checked) chkPunctuation.Checked = true;
            UpdateChkAll();
        }

        private void chkCase_Click(object sender, EventArgs e) => UpdateChkAll();

        private void chkAccents_Click(object sender, EventArgs e) => UpdateChkAll();

        private void chkQuotes_Click(object sender, EventArgs e) => UpdateChkAll();

        private void chkNumbers_Click(object sender, EventArgs e)
        {
            if (!chkNumbers.Checked)
            {
                chkPunctuation.Checked = false;
                chkSentences.Checked = false;
            }
            UpdateChkAll();
        }

        private void chkPunctuation_Click(object sender, EventArgs e) => UpdateChkAll();

        private void chkPunctuation_CheckedChanged(object sender, EventArgs e)
            => UpdateSentencesAndParagraphsState();

        private void UpdateSentencesAndParagraphsState()
        {
            if (chkPunctuation.Checked && !chkSentences.Checked)
                chkSentences.Checked = true;
            if (chkPunctuation.Checked && !chkNumbers.Checked)
                chkNumbers.Checked = true;
        }

        private void chkSentences_Click(object sender, EventArgs e)
        {
            if (!chkSentences.Checked && chkPunctuation.Checked)
                chkPunctuation.Checked = false;
            if (chkSentences.Checked) chkNumbers.Checked = true;
        }

        private void chkSentences_CheckedChanged(object sender, EventArgs e)
            => UpdatePageButtons();

        private void chkParagraphs_Click(object sender, EventArgs e) { }

        private void chkPaginate_CheckedChanged(object sender, EventArgs e)
            => UpdatePageButtons();

        private void cmdPrevPage_Click(object sender, EventArgs e)
        {
            _pageNumber--;
            UpdatePageButtons();
            if (_pageNumber == 1) cmdNextPage.Select();
        }

        private void cmdNextPage_Click(object sender, EventArgs e)
        {
            _pageNumber++;
            UpdatePageButtons();
            if (_pageNumber == _pageCount) cmdPrevPage.Select();
        }

        private void UpdatePageButtons()
        {
            if (chkPaginate.Checked)
            {
                cmdPrevPage.Enabled = _pageNumber > 1;
                cmdNextPage.Enabled = _pageNumber < _pageCount;
                lblPageNum.Enabled = true;
                chkRatio.Enabled = true;
            }
            else
            {
                lblPageNum.Enabled = false;
                cmdPrevPage.Enabled = false;
                cmdNextPage.Enabled = false;
                chkRatio.Enabled = false;
            }
            lblPageNum.Text = _pageNumber + "/" + _pageCount;
        }

        // ─── WinMerge registry ────────────────────────────────────────────────────

        private bool ReadWinMergeRegistryKey()
        {
            string winMergePath = "";
            if (!RegistryHelper.CurrentUserKeyExists(
                "SOFTWARE\\Thingamahoochie\\WinMerge", ref winMergePath, "Executable"))
                return false;
            _winMergePath = winMergePath;
            if (_winMergePath.Length == 0) return false;
            if (!FileHelper.FileExists(_winMergePath, prompt: true)) return false;
            return true;
        }

        // ─── Shortcut management ──────────────────────────────────────────────────

        private void CheckShortcut()
        {
            bool exists = FileHelper.FileExists(_shortcutPath);
            cmdAddShortcut.Enabled = !exists;
            cmdRemoveShortcut.Enabled = exists;
        }

        private void cmdAddShortcut_Click(object sender, EventArgs e)
        {
            string link = _shortcutPath;
            string target = Application.StartupPath + "\\" + ExeTextDiffOptions;
            ShortcutHelper.CreateShortcut(ref link, ref target);
            CheckShortcut();
        }

        private void cmdRemoveShortcut_Click(object sender, EventArgs e)
        {
            if (!FileHelper.FileExists(_shortcutPath)) return;
            if (!FileHelper.DeleteFile(_shortcutPath)) return;
            CheckShortcut();
        }
    }
}
