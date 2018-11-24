using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace backgroundr.view
{
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); 
            base.OnExit(e);
        }
    }
}
