using Microsoft.Win32;

namespace backgroundr.view.utils
{
    public class StartupService
    {
        private readonly RegistryKey _startupRegistry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public bool IsApplicationAutomaticallyStart(string applicationName)
        {
            return _startupRegistry.GetValue(applicationName) != null;
        }

        public void EnableAutomaticStartup(string applicationName, string exeFilePath)
        {
            _startupRegistry.SetValue(applicationName, exeFilePath);
        }

        public void DisableAutomaticStartup(string applicationName)
        {
            _startupRegistry.DeleteValue(applicationName);
        }
    }
}