using System.Windows;
using backgroundr.view.windows.taskbar;
using Hardcodet.Wpf.TaskbarNotification;
using StructureMap;

namespace backgroundr.view
{
    public partial class App : Application
    {
        private TaskbarIcon _taskBar;

        protected override void OnStartup(StartupEventArgs e)
        {
            var container = GetContainer();

            base.OnStartup(e);

            _taskBar = (TaskbarIcon) FindResource("Taskbar");
            if (_taskBar != null) {
                _taskBar.DataContext = container.GetInstance<TaskBarViewModel>();
            }
        }

        private static IContainer GetContainer()
        {
            return new Container();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskBar.Dispose();
            base.OnExit(e);
        }
    }
}