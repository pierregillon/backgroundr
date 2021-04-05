using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backgroundr.domain;
using ddd_cqrs;

namespace backgroundr.daemon
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length == 0) {
                await Run();
            }
            else if (args.First() == "--install") {
                await Install();
            }
            else {
                Console.WriteLine($"Unknown arguments: {string.Join(" | ", args)}");
            }
        }

        private static async Task Install()
        {
            var container = Bootstrap.BuildContainer();
            container.GetInstance<ILogger>().Log("Starting daemon ...");
            await container.GetInstance<Daemon>().Install();
        }

        private static async Task Run()
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                exitEvent.Set();
            };

            var container = Bootstrap.BuildContainer();

            var flickrParametersService = container.GetInstance<FlickrParametersService>();
            container.Inject(await flickrParametersService.Read());

            var logger = container.GetInstance<ILogger>();
            logger.Log("Starting daemon ...");
            
            var daemon = container.GetInstance<Daemon>();
            daemon.Start();
            
            exitEvent.WaitOne();
            
            logger.Log("Exiting daemon ...");
            daemon.Stop();
        }
    }
}