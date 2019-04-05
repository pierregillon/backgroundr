using System;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.infrastructure;
using backgroundr.view.mvvm;
using backgroundr.view.services;

namespace backgroundr.view.windows.authentication
{
    public class FlickrAuthenticationViewModel : ViewModelBase
    {
        private readonly FlickrAuthenticationService _service;
        private readonly MessageBoxService _messageBoxService;

        public event Action<FlickrPrivateAccess> Close;

        public string FlickrCode
        {
            get { return GetNotifiableProperty<string>(); }
            set { SetNotifiableProperty<string>(value); }
        }

        public ICommand CancelCommand => new DelegateCommand {
            CommandAction = () => Close?.Invoke(null)
        };

        public ICommand ValidateCommand => new DelegateCommand {
            CommandAction = Validate,
            CanExecuteFunc = () => !string.IsNullOrEmpty(FlickrCode) && !string.IsNullOrWhiteSpace(FlickrCode)
        };

        public FlickrAuthenticationViewModel(FlickrAuthenticationService service, MessageBoxService messageBoxService)
        {
            _service = service;
            _messageBoxService = messageBoxService;
        }

        public void Initialize()
        {
            _service.AuthenticateUserInBrowser();
        }

        private async void Validate()
        {
            try {
                var privateAccess = _service.FinalizeAuthentication(FlickrCode);
                Close?.Invoke(privateAccess);
            }
            catch (Exception ex) {
                await _messageBoxService.ShowError("An error occured during authentication. " + ex.Message);
            }
        }

    }
}