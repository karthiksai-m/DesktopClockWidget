using Microsoft.Win32;

namespace DesktopClockWidget
{
    internal static class StartupManager
    {
        public static void Enable(string appName, string exePath)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue(appName, exePath);
        }

        public static void Disable(string appName)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.DeleteValue(appName, false);
        }
    }
}
