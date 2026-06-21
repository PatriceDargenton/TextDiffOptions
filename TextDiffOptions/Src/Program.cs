
using System;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using FileUtility;
using App.Helper;

namespace TextDiffOptions
{
    internal static class Program
    {
        // ─── Application metadata ─────────────────────────────────────────────────
        
        public const string AppVersionDate = "21/06/2026";

        public static readonly string AppName =
            Assembly.GetEntryAssembly()?.GetName().Name ?? "TextDiffOptions";

        public static readonly string AppVersion = GetVersion();

        public const string AppTitleDescription = ": Options interface for TextDiffToHtml";

        private static string _appTitle;
        public static string AppTitle => _appTitle ?? AppName;

        private static string GetVersion()
        {
            var v = Assembly.GetEntryAssembly()?.GetName().Version;
            if (v == null) return "1.0";
            return v.Major + "." + v.Minor + v.Build;
        }

        // ─── Entry point ──────────────────────────────────────────────────────────

        [STAThread]
        static void Main(string[] args)
        {
            // Add support for legacy code pages (e.g., Windows-1252)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (Const.IsDebug)
            {
                Start(args);
                return;
            }

#pragma warning disable CS0162 // Inaccessible code
            try
            {
                Start(args);
            }
            catch (Exception ex)
            {
                AppHelper.ShowError(ex, "Start " + AppTitle);
            }
#pragma warning restore CS0162

        }

        private static void Start(string[] args)
        {
            AppHelper.SetAppTitle(AppTitle);

            string path1 = "";
            string path2 = "";
            bool syntaxOk = false;
            int argCount = 0;

            if (args != null && args.Length > 0)
            {
                argCount = args.Length;

                if (argCount == 2)
                {
                    syntaxOk = true;
                    path1 = args[0];
                    if (!FileHelper.FileExists(path1, prompt: true))
                    { syntaxOk = false; goto Launch; }
                    path2 = args[1];
                    if (!FileHelper.FileExists(path2, prompt: true))
                        syntaxOk = false;
                }
            }

Launch:
            if (Const.IsRelease && !syntaxOk)
            {
                MessageBox.Show(
                    "Syntax: paths of the two text files to compare" + Const.newlineCRLF +
                    "Otherwise, add a shortcut via the dedicated menu below" + Const.newlineCRLF +
                    " and send two files to compare to TextDiffOptions" + Const.newlineCRLF +
                    " via the Windows File Explorer.",
                    AppTitle + AppTitleDescription,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (argCount > 0) return;
            }

            ApplicationConfiguration.Initialize();
            var form = new frmTextDiffOptions();
            form.FilePath1 = path1;
            form.FilePath2 = path2;
            Application.Run(form);
        }
    }
}