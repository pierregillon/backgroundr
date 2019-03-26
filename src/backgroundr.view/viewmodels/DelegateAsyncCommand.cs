using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace backgroundr.view.viewmodels
{
    public class DelegateAsyncCommand : ICommand
    {
        public Func<Task> CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public async void Execute(object parameter)
        {
            await CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}