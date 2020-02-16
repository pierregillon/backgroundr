using System;
using backgroundr.application;
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
        private readonly IEventEmitter _eventEmitter;

        public Daemon(
            FlickrParametersService flickrParametersService,
            FlickrAuthenticationService authenticationService,
            ILogger logger,
            ICommandDispatcher commandDispatcher,
            IEventEmitter eventEmitter)
        {
            _flickrParametersService = flickrParametersService;
            _authenticationService = authenticationService;
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _eventEmitter = eventEmitter;
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
            _flickrParametersService.FlickrConfigurationFileChanged += OnFlickrConfigurationFileChanged;
            _flickrParametersService.SubscribeToChange();
            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
        }

        public void Stop()
        {
            _flickrParametersService.FlickrConfigurationFileChanged -= OnFlickrConfigurationFileChanged;
            _flickrParametersService.UnsubscribeToChange();
        }

        private void OnFlickrConfigurationFileChanged()
        {
            _eventEmitter.Emit(new FlickrConfigurationFileChanged());
        }

        private FlickrParameters ReadExistingConfiguration()
        {
            _logger.Log("Configuration file found");
            return _flickrParametersService.Read();
        }

        private FlickrParameters InitializeNewConfiguration()
        {
            _logger.Log("No configuration file found");
            _authenticationService.AuthenticateUserInBrowser();
            Console.WriteLine("Flickr code : ");
            var flickrCode = Console.ReadLine();
            var token = _authenticationService.FinalizeAuthentication(flickrCode);
            var flickrParameters = new FlickrParameters();
            flickrParameters.PrivateAccess = token;
            flickrParameters.RefreshPeriod = TimeSpan.FromHours(1);
            flickrParameters.UserId = token.UserId;
            _flickrParametersService.Save(flickrParameters);
            return flickrParameters;
        }
    }
}