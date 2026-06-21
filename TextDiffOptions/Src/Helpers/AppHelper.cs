
using System;
using System.Windows.Forms;

namespace App.Helper
{
    public static class AppHelper
    {
        public static string AppTitle { get; private set; }
        public static void SetAppTitle(string title) => AppTitle = title;

        public static void ShowError(Exception ex,
            ref string finalErrorMessage,
            string functionTitle = "",
            string info = "",
            string errorDetail = "",
            bool copyToClipboard = true)
        {
            if (!Cursors.Default.Equals(Cursor.Current))
                Cursor.Current = Cursors.Default;

            string msg = "";
            if (!string.IsNullOrEmpty(functionTitle))
                msg = "Function: " + functionTitle;
            if (!string.IsNullOrEmpty(info))
                msg += "\r\n" + info;
            if (!string.IsNullOrEmpty(errorDetail))
                msg += "\r\n" + errorDetail;
            if (!string.IsNullOrEmpty(ex.Message))
            {
                msg += "\r\n" + ex.Message.Trim();
                if (ex.InnerException != null)
                    msg += "\r\n" + ex.InnerException.Message;
            }

            if (copyToClipboard)
                CopyToClipboard(msg);

            finalErrorMessage = msg;
            MessageBox.Show(msg, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowError(Exception ex,
            string functionTitle = "",
            string info = "",
            string errorDetail = "",
            bool copyToClipboard = true)
        {
            string unused = "";
            ShowError(ex, ref unused, functionTitle, info, errorDetail, copyToClipboard);
        }

        public static void SetWaitCursor(bool deactivate = false)
        {
            Cursor.Current = deactivate ? Cursors.Default : Cursors.WaitCursor;
        }

        public static void CopyToClipboard(string info)
        {
            try
            {
                var dataObj = new DataObject();
                dataObj.SetData(DataFormats.Text, info);
                Clipboard.SetDataObject(dataObj);
            }
            catch (Exception ex)
            {
                ShowError(ex, "CopyToClipboard", copyToClipboard: false);
            }
        }

        public static void ProcessWindowsMessages()
        {
            try
            {
                Application.DoEvents();
            }
            catch
            {
            }
        }

        public static void ReleaseResources()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ProcessWindowsMessages();
        }
    }
}