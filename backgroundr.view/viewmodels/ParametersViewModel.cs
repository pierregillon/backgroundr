using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.domain;
using backgroundr.view.utils;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.viewmodels
{
    public class ParametersViewModel : ViewModelBase
    {
        private const string APPLICATION_NAME = "Backgroundr";

        private readonly BackgroundrParameters _parameters;
        private readonly IFileService _fileService;
        private readonly StartupService _startupService;

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

        public ICommand ValidateCommand
        {
            get
            {
                return new DelegateCommand {
                    CommandAction = () => {
                        _parameters.UserId = UserId;
                        _parameters.Tags = Tags;
                        _parameters.ApiToken = Token;
                        _parameters.ApiSecret = TokenSecret;
                        _parameters.OAuthAccessToken = OAuthAccessToken;
                        _parameters.OAuthAccessTokenSecret = OAuthAccessTokenSecret;
                        _fileService.Serialize(_parameters, ".flickr");

                        if (AutomaticallyStart) {
                            _startupService.EnableAutomaticStartup(APPLICATION_NAME, Assembly.GetExecutingAssembly().Location);
                        }
                        else {
                            _startupService.DisableAutomaticStartup(APPLICATION_NAME);
                        }

                        Application.Current?.MainWindow?.Close();
                    }
                };
            }
        }
        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand {
                    CommandAction = () => {
                        Application.Current?.MainWindow?.Close();
                    }
                };
            }
        }

        public ParametersViewModel(BackgroundrParameters parameters, IFileService fileService, StartupService startupService)
        {
            _parameters = parameters;
            _fileService = fileService;
            _startupService = startupService;

            UserId = _parameters.UserId;
            Tags = _parameters.Tags;
            Token = _parameters.ApiToken;
            TokenSecret = _parameters.ApiSecret;
            OAuthAccessToken = _parameters.OAuthAccessToken;
            OAuthAccessTokenSecret = _parameters.OAuthAccessTokenSecret;

            AutomaticallyStart = _startupService.IsApplicationAutomaticallyStart(APPLICATION_NAME);
        }
    }
}