﻿using System;
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

        private static Container GetContainer()
        {
            var container = new Container(configuration => {
                configuration.For<IFileService>().Use<FileService>();
                configuration.For<IDesktopBackgroundImageUpdater>().Use<WindowDesktopBackgroundImageUpdater>();
#if DEBUG
                configuration.For<IPhotoProvider>().Use<LocalComputerImageProvider>();
#else
                configuration.For<IPhotoProvider>().Use<FlickrPhotoProvider>();
#endif
                configuration.For<FlickrApiCredentials>().Use<FlickrApiCredentials>()
                    .Ctor<string>("apiToken").Is(Settings.Default.FlickrApiToken)
                    .Ctor<string>("apiSecret").Is(Settings.Default.FlickrApiSecret)
                    .Singleton();

                configuration.For<IFileDownloader>().Use<HttpFileDownloader>();
                configuration.For<IRandom>().Use<PseudoRandom>();
                configuration.For<IClock>().Use<DefaultClock>();
                configuration.For<IEncryptor>().Use<AesEncryptor>().Ctor<string>("encryptionKey").Is(Environment.MachineName);
                configuration.For<ICommandDispatcher>().Use<StructureMapCommandDispatcher>();
                configuration.For<ICommandHandler<ChangeDesktopBackgroundImageRandomly>>().Use<ChangeDesktopBackgroundImageRandomlyHandler>().Singleton();
                configuration.For<ICommandHandler<ScheduleNextDesktopBackgroundImageChange>>().Use<ScheduleNextDesktopBackgroundImageChangeHandler>();
                configuration.For<ICommandDispatchScheduler>().Use<CommandDispatchScheduler>().Singleton();
                configuration.For<IEventEmitter>().Use<StructureMapEventEmitter>();
                configuration.For<IEventListener<DesktopBackgroundImageUpdated>>().Use<Scheduler>();
                configuration.For<FlickrParameters>().Singleton();
            });
            return container;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbar.Dispose();
            base.OnExit(e);
        }
    }
}