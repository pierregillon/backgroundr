using System.Windows;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;
using backgroundr.view.viewmodels;
using Hardcodet.Wpf.TaskbarNotification;
using StructureMap;

namespace backgroundr.view
{
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new Container(configuration => {
                configuration.For<IFileService>().Use<FileService>();
                configuration.For<ICommandDispatcher>().Use<CommandDispatcher>().Singleton();
                configuration.For<ICommandHandler<RandomlyChangeOsBackgroundImage>>().Use<RandomlyChangeOsBackgroundImageHander>();
            });

            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            _notifyIcon.DataContext = container.GetInstance<NotifyIconViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); 
            base.OnExit(e);
        }
    }
}
