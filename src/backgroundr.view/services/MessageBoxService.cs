using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace backgroundr.view.services
{
    public class MessageBoxService
    {
        public async Task ShowError(string error)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                    MessageBox.Show(
                        error,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error)
                ),
                DispatcherPriority.ApplicationIdle
            );
        }

        public bool ShowQuestion(string question)
        {
            return MessageBox.Show(
                       question,
                       "Question",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}