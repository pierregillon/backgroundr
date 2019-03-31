using System;
using System.Threading.Tasks;
using System.Windows;
using backgroundr.application;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.view.services;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.viewmodels
{
    public class TaskBarViewModel : ViewModelBase
    {
        private readonly StructureMap.IContainer _container;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IFileService _fileService;
        private readonly MessageBoxService _messageBoxService;

        public ICommand RandomlyChangeBackgroundImageCommand => new DelegateAsyncCommand {
            CommandAction = RandomlyChangeBackgroundImage
        };
        public ICommand OpenParametersWindowCommand => new DelegateCommand {
            CanExecuteFunc = () => Application.Current.MainWindow == null,
            CommandAction = () => {
                Application.Current.MainWindow = _container.GetInstance<ParametersWindow>();
                Application.Current.MainWindow.Show();
            }
        };
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() }; }
        }
        public bool ChangingBackground
        {
            get { return GetNotifiableProperty<bool>(); }
            set { SetNotifiableProperty<bool>(value); }
        }

        // ----- Constructor
        public TaskBarViewModel(
            StructureMap.IContainer container,
            ICommandDispatcher commandDispatcher,
            IFileService fileService,
            MessageBoxService messageBoxService)
        {
            _container = container;
            _commandDispatcher = commandDispatcher;
            _fileService = fileService;
            _messageBoxService = messageBoxService;
        }

        // ----- Public methods
        private async Task RandomlyChangeBackgroundImage()
        {
            try {
                ChangingBackground = true;
                await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
            }
            catch (Exception ex) {
                _fileService.Append("logs.txt", $"{DateTime.Now} - ERROR : " + ex + Environment.NewLine);
                await _messageBoxService.ShowError("An error occured when changing the image background with next Flickr photo. For more information, see the logs.txt file." + Environment.NewLine + "=> " + ex.Message);
            }
            finally {
                ChangingBackground = false;
            }
        }
    }
}