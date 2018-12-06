using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using backgroundr.application;
using backgroundr.cqrs;

namespace backgroundr.view.viewmodels
{
    public class NotifyIconViewModel : ViewModelBase
    {
        private readonly StructureMap.IContainer _container;
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
                        Application.Current.MainWindow = _container.GetInstance<ParametersWindow>();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() }; }
        }
        public bool ChangingBackground
        {
            get { return GetNotifiableProperty<bool>(); }
            set { SetNotifiableProperty<bool>(value); }
        }

        public NotifyIconViewModel(StructureMap.IContainer container, ICommandDispatcher commandDispatcher)
        {
            _container = container;
            _commandDispatcher = commandDispatcher;
        }

        public async Task RandomlyChangeBackgroundImage()
        {
            try {
                ChangingBackground = true;
                await _commandDispatcher.Dispatch(new RandomlyChangeOsBackgroundImage());
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