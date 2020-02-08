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

            var service = container.GetInstance<FlickrParametersService>();
            if (!service.Exists()) {
                logger.Log("No configuration file found");
                var authenticationService = container.GetInstance<FlickrAuthenticationService>();
                authenticationService.AuthenticateUserInBrowser();
                Console.WriteLine("Flickr code : ");
                var flickrCode = Console.ReadLine();
                var token = authenticationService.FinalizeAuthentication(flickrCode);
                var flickrParameters = new FlickrParameters();
                flickrParameters.PrivateAccess = token;
                flickrParameters.RefreshPeriod = TimeSpan.FromHours(1);
                flickrParameters.UserId = token.UserId;
                service.Save(flickrParameters);
                container.Inject(flickrParameters);
            }
            else {
                logger.Log("Configuration file found");
                var parameters = service.Read();
                container.Inject(parameters);
            }

            var dispatcher = container.GetInstance<ICommandDispatcher>();
            dispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());

            return true;
        }
    }
}