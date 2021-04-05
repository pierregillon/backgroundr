using System;
using System.Threading.Tasks;
using backgroundr.application.scheduleNextDesktopBackgroundImageChange;
using backgroundr.domain;
using backgroundr.infrastructure;
using ddd_cqrs;

namespace backgroundr.daemon
{
    public class Daemon
    {
        private readonly FlickrParametersService _flickrParametersService;
        private readonly FlickrAuthenticationService _authenticationService;
        private readonly ICommandDispatcher _commandDispatcher;

        public Daemon(
            FlickrParametersService flickrParametersService,
            FlickrAuthenticationService authenticationService,
            ICommandDispatcher commandDispatcher)
        {
            _flickrParametersService = flickrParametersService;
            _authenticationService = authenticationService;
            _commandDispatcher = commandDispatcher;
        }

        public async Task Install()
        {
            await _authenticationService.AuthenticateUserInBrowser();
            Console.Write("Flickr code : ");
            var flickrCode = Console.ReadLine();
            var token = await _authenticationService.FinalizeAuthentication(flickrCode);
            var flickrParameters = new FlickrParameters {
                PrivateAccess = token,
                RefreshPeriod = TimeSpan.FromHours(1),
                UserId = token.UserId
            };
            await _flickrParametersService.Save(flickrParameters);
        }

        public void Start()
        {
            if (!_flickrParametersService.ConfigurationExists()) {
                throw new InvalidOperationException("Unable to start daemon: no configuration file found.");
            }

            _flickrParametersService.SubscribeToChange();
            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChangeCommand());
        }

        public void Stop()
        {
            _flickrParametersService.UnsubscribeToChange();
        }
    }
}