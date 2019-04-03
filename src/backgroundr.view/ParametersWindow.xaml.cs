using System.Windows;
using System.Windows.Controls;
using backgroundr.view.utils;
using backgroundr.view.viewmodels;

namespace backgroundr.view
{
    /// <summary>
    /// Interaction logic for ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow : Window
    {
        public ParametersWindow(ParametersViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            TokenSecret.Password = viewModel.TokenSecret?.ToInsecureString();
            OAuthAccessTokenSecret.Password = viewModel.OAuthAccessTokenSecret?.ToInsecureString();
        }

        private void TokenSecret_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null) {
                ((ParametersViewModel) DataContext).TokenSecret = ((PasswordBox) sender).SecurePassword;
            }
        }

        private void OAuthAccessTokenSecret_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null) {
                ((ParametersViewModel) DataContext).OAuthAccessTokenSecret = ((PasswordBox) sender).SecurePassword;
            }
        }
    }
}