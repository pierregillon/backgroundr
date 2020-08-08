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

        public async Task<FlickrParameters> ReadFileConfiguration()
        {
            if (!_flickrParametersService.ConfigurationExists()) {
                return await InitializeNewConfiguration();
            }
            else {
                return await ReadExistingConfiguration();
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

        private async Task<FlickrParameters> ReadExistingConfiguration()
        {
            _logger.Log("Configuration file found");
            return await _flickrParametersService.Read();
        }

        private async Task<FlickrParameters> InitializeNewConfiguration()
        {
            _logger.Log("No configuration file found");
            
            var token = await GetFlickrToken();
            var flickrParameters = new FlickrParameters {
                PrivateAccess = token,
                RefreshPeriod = TimeSpan.FromHours(1),
                UserId = token.UserId
            };
            await _flickrParametersService.Save(flickrParameters);
            return flickrParameters;
        }

        private async Task<FlickrPrivateAccess> GetFlickrToken()
        {
            await _authenticationService.AuthenticateUserInBrowser();
            Console.Write("Flickr code : ");
            var flickrCode = Console.ReadLine();
            return await _authenticationService.FinalizeAuthentication(flickrCode);
        }
    }
}