using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.cqrs;

namespace backgroundr.view.viewmodels
{
    public class NotifyIconViewModel
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ICommand RandomlyChangeBackgroundImageCommand
        {
            get
            {
                return new DelegateAsyncCommand {
                    CanExecuteFunc = () => true,
                    CommandAction = RandomlyChangeBackgroundImage
                };
            }
        }
        public ICommand OpenParametersWindowCommand
        {
            get
            {
                return new DelegateCommand {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () => {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() }; }
        }

        public NotifyIconViewModel(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task RandomlyChangeBackgroundImage()
        {
            await _commandDispatcher.Dispatch(new RandomlyChangeOsBackgroundImage());
        }
    }
}