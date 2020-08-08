using System;
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
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public Daemon(
            FlickrParametersService flickrParametersService,
            FlickrAuthenticationService authenticationService,
            ILogger logger,
            ICommandDispatcher commandDispatcher)
        {
            _flickrParametersService = flickrParametersService;
            _authenticationService = authenticationService;
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        public FlickrParameters ReadFileConfiguration()
        {
            if (!_flickrParametersService.ConfigurationExists()) {
                return InitializeNewConfiguration();
            }
            else {
                return ReadExistingConfiguration();
            }
        }

        public void Start()
        {
            _flickrParametersService.SubscribeToChange();
            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChangeCommand());
        }

        public void Stop()
        {
            _flickrParametersService.UnsubscribeToChange();
        }

        private FlickrParameters ReadExistingConfiguration()
        {
            _logger.Log("Configuration file found");
            return _flickrParametersService.Read();
        }

        private FlickrParameters InitializeNewConfiguration()
        {
            _logger.Log("No configuration file found");
            
            var token = GetFlickrToken();
            var flickrParameters = new FlickrParameters {
                PrivateAccess = token,
                RefreshPeriod = TimeSpan.FromHours(1),
                UserId = token.UserId
            };
            _flickrParametersService.Save(flickrParameters);
            return flickrParameters;
        }

        private FlickrPrivateAccess GetFlickrToken()
        {
            _authenticationService.AuthenticateUserInBrowser();
            Console.Write("Flickr code : ");
            var flickrCode = Console.ReadLine();
            return _authenticationService.FinalizeAuthentication(flickrCode);
        }
    }
}