using System;
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

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                Console.WriteLine("Exiting daemon ...");
                e.Cancel = true;
                exitEvent.Set();
            };

            if (Initialize(Bootstrap.BuildContainer())) {
                exitEvent.WaitOne();
            }
        }

        private static bool Initialize(IContainer container)
        {
            var logger = container.GetInstance<ILogger>();
            logger.Log("Starting daemon ...");
            var daemon = container.GetInstance<Daemon>();
            container.Inject(daemon.ReadFileConfiguration());
            daemon.WatchFileChanges();
            daemon.ScheduleNextBackgroundImageChange();
            return true;
        }
    }
}