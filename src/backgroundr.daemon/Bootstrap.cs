using System;
using System.Runtime.InteropServices;
using backgroundr.application;
using backgroundr.daemon.windows;
using backgroundr.domain;
using backgroundr.infrastructure;
using ddd_cqrs;
using StructureMap;

namespace backgroundr.daemon
{
    public class Bootstrap
    {
        public static Container BuildContainer()
        {
            return new Container(configuration => {
                configuration.For<IFileService>().Use<FileService>();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    configuration.For<IDesktopBackgroundImageUpdater>().Use<DirectRegistryDesktopBackgroundImageUpdater>();
                }
                else {
                    throw new Exception("This os version is not supported.");
                }

#if DEBUG
                configuration.For<IPhotoProvider>().Use<LocalComputerImageProvider>();
#else
                configuration.For<IPhotoProvider>().Use<FlickrPhotoProvider>();
#endif
                configuration.For<FlickrApiCredentials>().Use<FlickrApiCredentials>()
                    .Ctor<string>("apiToken").Is("NjRmYzIyY2E5MjI0NzY3NDE1MzBhNDEyNWI0MDA5ZDY=")
                    .Ctor<string>("apiSecret").Is("ZDFmOTlkOWFhNzQ1Yzg1ZQ==")
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
                configuration.For<ILogger>().Use<ConsoleLogger>();
                configuration.For(typeof(ICommandHandler<>)).DecorateAllWith(typeof(LoggerCommandHandlerDecorator<>));
            });
        }
    }
}