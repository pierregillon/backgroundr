using System;
using System.Threading;
using backgroundr.application;
using backgroundr.domain;
using ddd_cqrs;
using StructureMap;

namespace backgroundr.daemon
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                Console.WriteLine("* Exiting daemon ...");
                e.Cancel = true;
                exitEvent.Set();
            };

            Initialize(Bootstrap.BuildContainer());

            exitEvent.WaitOne();
        }

        private static void Initialize(IContainer container)
        {
            var logger = container.GetInstance<ILogger>();

            logger.Log("Starting daemon ...");

            var service = container.GetInstance<FlickrParametersService>();
            if (!service.Exists()) {
                logger.Log("No configuration file found. Exiting.");
                return;
            }

            logger.Log("Configuration file found");
            var parameters = service.Read();
            container.Inject(parameters);

            var dispatcher = container.GetInstance<ICommandDispatcher>();
            dispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }
    }
}