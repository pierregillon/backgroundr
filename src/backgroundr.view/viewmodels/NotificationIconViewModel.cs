using System;
using System.Threading.Tasks;
using System.Windows;
using backgroundr.application;
using backgroundr.cqrs;
using ICommand = System.Windows.Input.ICommand;

namespace backgroundr.view.viewmodels
{
    public class TaskBarViewModel : ViewModelBase
    {
        private readonly StructureMap.IContainer _container;
        private readonly ICommandDispatcher _commandDispatcher;

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
        public TaskBarViewModel(StructureMap.IContainer container, ICommandDispatcher commandDispatcher)
        {
            _container = container;
            _commandDispatcher = commandDispatcher;
        }

        // ----- Public methods
        private async Task RandomlyChangeBackgroundImage()
        {
            try {
                ChangingBackground = true;
                await _commandDispatcher.Dispatch(new ChangeDesktopBackgroundImageRandomly());
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            finally {
                ChangingBackground = false;
            }
        }
    }
}