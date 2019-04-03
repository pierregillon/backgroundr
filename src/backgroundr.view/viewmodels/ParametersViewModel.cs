using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.view.utils;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.viewmodels
{
    public class ParametersViewModel : ViewModelBase
    {
        private readonly FlickrParameters _flickrParameters;
        private readonly IFileService _fileService;
        private readonly IEncryptor _encryptor;
        private readonly StartupService _startupService;
        private readonly ICommandDispatcher _commandDispatcher;

        public string UserId
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public string Tags
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public string OAuthAccessToken
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public SecureString OAuthAccessTokenSecret
        {
            get { return GetNotifiableProperty<SecureString>(); }
            set { SetNotifiableProperty<SecureString>(value); }
        }
        public bool AutomaticallyStart
        {
            get { return GetNotifiableProperty<bool>(); }
            set { SetNotifiableProperty<bool>(value); }
        }
        public IList<RefreshPeriod> Periods { get; set; } = new List<RefreshPeriod> {
            new RefreshPeriod(TimeSpan.FromSeconds(5)),
            new RefreshPeriod(TimeSpan.FromMinutes(1)),
            new RefreshPeriod(TimeSpan.FromMinutes(5)),
            new RefreshPeriod(TimeSpan.FromMinutes(15)),
            new RefreshPeriod(TimeSpan.FromMinutes(30)),
            new RefreshPeriod(TimeSpan.FromHours(1)),
            new RefreshPeriod(TimeSpan.FromHours(2)),
            new RefreshPeriod(TimeSpan.FromHours(4)),
            new RefreshPeriod(TimeSpan.FromHours(6)),
            new RefreshPeriod(TimeSpan.FromDays(1)),
            new RefreshPeriod(TimeSpan.FromDays(2)),
            new RefreshPeriod(TimeSpan.FromDays(7)),
            new RefreshPeriod(TimeSpan.FromDays(30))
        };
        public RefreshPeriod SelectedPeriod
        {
            get { return GetNotifiableProperty<RefreshPeriod>(); }
            set { SetNotifiableProperty<RefreshPeriod>(value); }
        }

        public ICommand ValidateCommand => new DelegateCommand {
            CommandAction = Validate
        };
        public ICommand CancelCommand => new DelegateCommand {
            CommandAction = () => {
                Application.Current?.MainWindow?.Close();
            }
        };

        public ParametersViewModel(
            FlickrParameters flickrParameters,
            IFileService fileService,
            IEncryptor encryptor,
            StartupService startupService,
            ICommandDispatcher commandDispatcher)
        {
            _flickrParameters = flickrParameters;
            _fileService = fileService;
            _encryptor = encryptor;
            _startupService = startupService;
            _commandDispatcher = commandDispatcher;

            UserId = _flickrParameters.UserId;
            Tags = _flickrParameters.Tags;
            OAuthAccessToken = _flickrParameters.OAuthAccessToken;
            OAuthAccessTokenSecret = _flickrParameters.OAuthAccessTokenSecret?.ToSecureString();
            AutomaticallyStart = _startupService.IsApplicationStartingOnSystemStartup();
            SelectedPeriod = Periods.FirstOrDefault(x => x.Value == flickrParameters.RefreshPeriod);
        }

        private void Validate()
        {
            UpdateParameters();

            if (AutomaticallyStart) {
                _startupService.EnableAutomaticStartup();
            }
            else {
                _startupService.DisableAutomaticStartup();
            }

            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());

            Application.Current?.MainWindow?.Close();
        }
        private void UpdateParameters()
        {
            _flickrParameters.UserId = UserId;
            _flickrParameters.Tags = Tags;
            _flickrParameters.OAuthAccessToken = OAuthAccessToken;

            var oAuthAccessTokenSecret = OAuthAccessTokenSecret.ToInsecureString();
            if (_flickrParameters.OAuthAccessTokenSecret != oAuthAccessTokenSecret) {
                _flickrParameters.OAuthAccessTokenSecret = _encryptor.Encrypt(oAuthAccessTokenSecret);
            }

            _flickrParameters.RefreshPeriod = SelectedPeriod.Value;
            _fileService.Serialize(_flickrParameters, ".flickr");
        }
    }
}