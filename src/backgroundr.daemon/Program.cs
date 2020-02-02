using System;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Threading;
using backgroundr.application;
using backgroundr.domain;
using backgroundr.infrastructure;
using ddd_cqrs;
using StructureMap;

namespace backgroundr.daemon
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            var container = BuildContainer();
            var service = container.GetInstance<FlickrParametersService>();

            if (service.Exists()) {
                var parameters = service.Read();
                container.Inject(parameters);
                var dispatcher = container.GetInstance<ICommandDispatcher>();
                dispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());

                Console.WriteLine("* Starting ...");

                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                    Console.WriteLine("* Exiting ...");
                    e.Cancel = true;
                    exitEvent.Set();
                };

                exitEvent.WaitOne();

            }
            else {
                Console.WriteLine("* No configuration file found.");
            }
        }

        private static Container BuildContainer()
        {
            return new Container(configuration => {
                configuration.For<IFileService>().Use<FileService>();
                configuration.For<IDesktopBackgroundImageUpdater>().Use<FakeDesktopBackgroundImageUpdater>();
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
            });
        }
    }

    internal class FakeDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public void ChangeBackgroundImage(string backgroundPath, PicturePosition style)
        {
            Console.WriteLine($"* Changing background to {backgroundPath} position {style}");
        }
    }
}