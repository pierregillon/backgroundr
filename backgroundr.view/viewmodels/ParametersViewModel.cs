using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.view.design;
using backgroundr.view.utils;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.viewmodels
{
    public class ParametersViewModel : ViewModelBase
    {
        private const string APPLICATION_NAME = "Backgroundr";

        private readonly Parameters _parameters;
        private readonly IFileService _fileService;
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
        public string Token
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public string TokenSecret
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public string OAuthAccessToken
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }
        public string OAuthAccessTokenSecret
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
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
            Parameters parameters,
            IFileService fileService,
            StartupService startupService,
            ICommandDispatcher commandDispatcher)
        {
            _parameters = parameters;
            _fileService = fileService;
            _startupService = startupService;
            _commandDispatcher = commandDispatcher;

            UserId = _parameters.UserId;
            Tags = _parameters.Tags;
            Token = _parameters.ApiToken;
            TokenSecret = _parameters.ApiSecret;
            OAuthAccessToken = _parameters.OAuthAccessToken;
            OAuthAccessTokenSecret = _parameters.OAuthAccessTokenSecret;
            AutomaticallyStart = _startupService.IsApplicationAutomaticallyStart(APPLICATION_NAME);
            SelectedPeriod = Periods.FirstOrDefault(x => x.Value == parameters.RefreshPeriod);
        }

        private void Validate()
        {
            _parameters.UserId = UserId;
            _parameters.Tags = Tags;
            _parameters.ApiToken = Token;
            _parameters.ApiSecret = TokenSecret;
            _parameters.OAuthAccessToken = OAuthAccessToken;
            _parameters.OAuthAccessTokenSecret = OAuthAccessTokenSecret;
            _parameters.RefreshPeriod = SelectedPeriod.Value;
            _fileService.Serialize(_parameters, ".flickr");

            if (AutomaticallyStart) {
                _startupService.EnableAutomaticStartup(APPLICATION_NAME, Assembly.GetExecutingAssembly().Location);
            }
            else {
                _startupService.DisableAutomaticStartup(APPLICATION_NAME);
            }

            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());

            Application.Current?.MainWindow?.Close();
        }
    }
}