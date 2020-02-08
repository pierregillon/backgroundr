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

        public Daemon(FlickrParametersService flickrParametersService, FlickrAuthenticationService authenticationService, ILogger logger, ICommandDispatcher commandDispatcher)
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

        public void WatchFileChanges() { }

        public void ScheduleNextBackgroundImageChange()
        {
            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());
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