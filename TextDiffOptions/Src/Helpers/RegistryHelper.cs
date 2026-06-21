
using Microsoft.Win32;

namespace RegistryUtility
{
    public static class RegistryHelper
    {
        // ─── CurrentUser ──────────────────────────────────────────────────────────

        public static bool CurrentUserKeyExists(string key,
            ref string subKeyValue,
            string subKey = "")
        {
            subKeyValue = "";
            try
            {
                using (var rkKey = Registry.CurrentUser.OpenSubKey(key))
                {
                    if (rkKey == null) return false;
                    object val = rkKey.GetValue(subKey);
                    if (val == null) return false;
                    string valStr = val.ToString();
                    if (valStr == null) return false;
                    subKeyValue = valStr;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CurrentUserKeyExists(string key, string subKey = "")
        {
            string unused = "";
            return CurrentUserKeyExists(key, ref unused, subKey);
        }

        // ─── LocalMachine ────────────────────────────────────────────────────────

        public static bool LocalMachineKeyExists(string key,
            ref string subKeyValue,
            string subKey = "",
            string newSubKeyValue = "")
        {
            subKeyValue = "";
            try
            {
                bool write = !string.IsNullOrEmpty(newSubKeyValue);
                using (var rkKey = Registry.LocalMachine.OpenSubKey(key, writable: write))
                {
                    if (rkKey == null) return false;
                    object val = rkKey.GetValue(subKey);
                    if (val == null) return false;
                    string valStr = val.ToString();
                    if (valStr == null) return false;
                    subKeyValue = valStr;
                    if (write)
                    {
                        int intVal = int.Parse(newSubKeyValue);
                        rkKey.SetValue(subKey, intVal, RegistryValueKind.DWord);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ─── ClassesRoot ──────────────────────────────────────────────────────────

        public static bool ClassesRootKeyExists(string key, string subKey = "")
        {
            try
            {
                using (var rkKey = Registry.ClassesRoot.OpenSubKey(key + "\\" + subKey))
                    return rkKey != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool ClassesRootKeyExists(string key, string subKey,
            ref string subKeyValue)
        {
            subKeyValue = "";
            try
            {
                using (var rkKey = Registry.ClassesRoot.OpenSubKey(key))
                {
                    if (rkKey == null) return false;
                    object val = rkKey.GetValue(subKey);
                    if (val == null) return false;
                    string valStr = val.ToString();
                    if (valStr == null) return false;
                    subKeyValue = valStr;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] ListCurrentUserSubKeys(string key)
        {
            try
            {
                using (var rkKey = Registry.CurrentUser.OpenSubKey(key))
                {
                    if (rkKey == null) return null;
                    return rkKey.GetSubKeyNames();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}