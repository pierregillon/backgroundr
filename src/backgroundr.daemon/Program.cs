using System;
using System.Threading;
using System.Threading.Tasks;
using ddd_cqrs;

namespace backgroundr.daemon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                exitEvent.Set();
            };

            var container = Bootstrap.BuildContainer();
            var logger = container.GetInstance<ILogger>();
            logger.Log("Starting daemon ...");
            var daemon = container.GetInstance<Daemon>();
            container.Inject(await daemon.ReadFileConfiguration());
            daemon.Start();
            exitEvent.WaitOne();
            logger.Log("Exiting daemon ...");
            daemon.Stop();
        }
    }
}