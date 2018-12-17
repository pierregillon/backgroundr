using System;
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
        private TaskbarIcon _taskbar;

        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new Container(configuration => {
                configuration.For<IFileService>().Use<FileService>();
                configuration.For<IDesktopBackgroundImageUpdater>().Use<WindowDesktopBackgroundImageUpdater>();
                configuration.For<IImageProvider>().Use<FlickrImageProvider>();
                configuration.For<IFileDownloader>().Use<HttpFileDownloader>();
                configuration.For<IRandom>().Use<PseudoRandom>();
                configuration.For<IClock>().Use<DefaultClock>();
                configuration.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                configuration.For<ICommandHandler<ChangeDesktopBackgroundImageRandomly>>().Use<ChangeDesktopBackgroundImageRandomlyHandler>().Singleton();
                configuration.For<ICommandHandler<StartDesktopBackgroundImageTimer>>().Use<StartDesktopBackgroundImageTimerHandler>();
                configuration.For<IEventEmitter>().Use<StructureMapEventEmitter>();
                configuration.For<IEventListener<DesktopBackgroundChanged>>().Use<RearmTimerListener>();
                configuration.For<BackgroundrParameters>().Singleton();
                configuration.For<BackgroundrTimer>().Singleton();
            });

            base.OnStartup(e);

            _taskbar = (TaskbarIcon) FindResource("Taskbar");
            _taskbar.DataContext = container.GetInstance<TaskBarViewModel>();

            if (File.Exists(".flickr")) {
                var fileService = container.GetInstance<IFileService>();
                var parameters = fileService.Deserialize<BackgroundrParameters>(".flickr");
                container.Inject(parameters);
            }

            var dispatcher = container.GetInstance<ICommandDispatcher>();
            dispatcher.Dispatch(new StartDesktopBackgroundImageTimer()).Wait();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbar.Dispose();
            base.OnExit(e);
        }
    }
}