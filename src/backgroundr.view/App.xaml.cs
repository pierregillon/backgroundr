using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;
using backgroundr.view.Properties;
using backgroundr.view.services;
using backgroundr.view.windows.taskbar;
using Hardcodet.Wpf.TaskbarNotification;
using StructureMap;

namespace backgroundr.view
{
    public partial class App : Application
    {
        private TaskbarIcon _taskbar;

        protected override void OnStartup(StartupEventArgs e)
        {
            var container = GetContainer();

            base.OnStartup(e);

            _taskbar = (TaskbarIcon) FindResource("Taskbar");
            _taskbar.DataContext = container.GetInstance<TaskBarViewModel>();

            var service = container.GetInstance<FlickrParametersService>();

            if (service.Exists()) {
                var parameters = service.Read();
                container.Inject(parameters);
                var dispatcher = container.GetInstance<ICommandDispatcher>();
                dispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
            }
            else {
                Current.MainWindow = container.GetInstance<windows.parameters.ParametersWindow>();
                Current.MainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbar.Dispose();
            base.OnExit(e);
        }
    }
}