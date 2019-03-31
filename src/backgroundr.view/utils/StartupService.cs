using System.Reflection;
using Microsoft.Win32;

namespace backgroundr.view.utils
{
    public class StartupService
    {
        private readonly string _applicationName;
        private readonly string _executableFilePath;
        private readonly RegistryKey _startupRegistry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public StartupService()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _applicationName = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            _executableFilePath = assembly.Location;
        }

        public bool IsApplicationStartingOnSystemStartup()
        {
            return _startupRegistry.GetValue(_applicationName) != null;
        }

        public void EnableAutomaticStartup()
        {
            _startupRegistry.SetValue(_applicationName, _executableFilePath);
        }

        public void DisableAutomaticStartup()
        {
            if (_startupRegistry.GetValue(_applicationName) != null) {
                _startupRegistry.DeleteValue(_applicationName);
            }
        }
    }
}