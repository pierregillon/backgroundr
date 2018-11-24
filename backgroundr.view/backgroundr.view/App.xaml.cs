using System.IO;
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
                configuration.For<BackgroundrParameters>().Singleton();
            });

            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            _notifyIcon.DataContext = container.GetInstance<NotifyIconViewModel>();

            if (File.Exists(".flickr")) {
                var fileService = container.GetInstance<IFileService>();
                var parameters = fileService.Deserialize<BackgroundrParameters>(".flickr");
                container.Inject(parameters);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); 
            base.OnExit(e);
        }
    }
}
