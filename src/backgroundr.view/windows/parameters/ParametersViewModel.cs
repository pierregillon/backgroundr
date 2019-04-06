using System;
using System.Collections.Generic;
using System.Linq;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;
using backgroundr.view.mvvm;
using backgroundr.view.services;
using backgroundr.view.utils;
using StructureMap;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.windows.parameters
{
    public class ParametersViewModel : ViewModelBase
    {
        private readonly FlickrParameters _flickrParameters;
        private readonly FlickrParametersService _flickrParametersService;
        private readonly StartupService _startupService;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IContainer _container;
        private readonly MessageBoxService _messageBoxService;

        public event Action Close;

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
        public FlickrPrivateAccess PrivateAccess
        {
            get { return GetNotifiableProperty<FlickrPrivateAccess>(); }
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
            CommandAction = () => { Close?.Invoke(); }
        };

        public ICommand ConnectToFlickrAccountCommand => new DelegateCommand {
            CommandAction = ConnectToFlickrAccount
        };

        public ICommand DisconnectFlickrAccountCommand => new DelegateCommand {
            CommandAction = DisconnectFlickrAccount
        };

        // ----- Constructor

        public ParametersViewModel(
            FlickrParameters flickrParameters,
            FlickrParametersService flickrParametersService,
            StartupService startupService,
            ICommandDispatcher commandDispatcher,
            IContainer container,
            MessageBoxService messageBoxService)
        {
            _flickrParameters = flickrParameters;
            _flickrParametersService = flickrParametersService;
            _startupService = startupService;
            _commandDispatcher = commandDispatcher;
            _container = container;
            _messageBoxService = messageBoxService;

            UserId = _flickrParameters.UserId;
            Tags = _flickrParameters.Tags;
            AutomaticallyStart = _startupService.IsApplicationStartingOnSystemStartup();
            SelectedPeriod = Periods.FirstOrDefault(x => x.Value == flickrParameters.RefreshPeriod);
            PrivateAccess = _flickrParameters.PrivateAccess;
        }

        // ----- Internal logics

        private void Validate()
        {
            SaveParameters();

            if (AutomaticallyStart) {
                _startupService.EnableAutomaticStartup();
            }
            else {
                _startupService.DisableAutomaticStartup();
            }

            _commandDispatcher.Dispatch(new ScheduleNextDesktopBackgroundImageChange());

            Close?.Invoke();
        }

        private void SaveParameters()
        {
            _flickrParameters.UserId = UserId;
            _flickrParameters.Tags = Tags;
            _flickrParameters.RefreshPeriod = SelectedPeriod.Value;
            _flickrParameters.PrivateAccess = PrivateAccess;
            _flickrParametersService.Save(_flickrParameters);
        }

        private async void ConnectToFlickrAccount()
        {
            try {
                PrivateAccess = _container.GetInstance<authentication.FlickrAuthenticationDialog>().ShowDialog();
            }
            catch (Exception ex) {
                await _messageBoxService.ShowError("An error occurred during authentication. " + ex.Message);
            }
        }

        private void DisconnectFlickrAccount()
        {
            if (_messageBoxService.ShowQuestion("Are you sure to disconnect your Flickr account ? You won't be able to access your private photos anymore.")) {
                PrivateAccess = null;
            }
        }
    }
}