using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.domain;

namespace backgroundr.view.viewmodels
{
    public class ParametersViewModel : INotifyPropertyChanged
    {
        private readonly BackgroundrParameters _parameters;
        private readonly IFileService _fileService;

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
        public ICommand ValidateCommand
        {
            get
            {
                return new DelegateCommand {
                    CommandAction = () => {
                        _parameters.UserId = UserId;
                        _parameters.Tags = Tags;
                        _parameters.Token = Token;
                        _parameters.TokenSecret = TokenSecret;
                        _parameters.OAuthAccessToken = OAuthAccessToken;
                        _parameters.OAuthAccessTokenSecret = OAuthAccessTokenSecret;
                        _fileService.Serialize(_parameters, ".flickr");
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

        public ParametersViewModel(BackgroundrParameters parameters, IFileService fileService)
        {
            _parameters = parameters;
            _fileService = fileService;

            UserId = _parameters.UserId;
            Tags = _parameters.Tags;
            Token = _parameters.Token;
            TokenSecret = _parameters.TokenSecret;
            OAuthAccessToken = _parameters.OAuthAccessToken;
            OAuthAccessTokenSecret = _parameters.OAuthAccessTokenSecret;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();
        protected virtual T GetNotifiableProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (_properties.ContainsKey(propertyName)) {
                return (T) _properties[propertyName];
            }
            return default(T);
        }
        protected virtual void SetNotifiableProperty<T>(object value, [CallerMemberName] string propertyName = null)
        {
            if (_properties.ContainsKey(propertyName) == false) {
                _properties.Add(propertyName, value);
            }
            else {
                _properties[propertyName] = value;
            }
            OnPropertyChanged(propertyName);
        }
    }
}