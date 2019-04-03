using System.Windows;

namespace backgroundr.view
{
    public partial class FlickrAuthenticationDialog : Window
    {
        public FlickrAuthenticationDialog(FlickrAuthenticationViewModel viewModel)
        {
            InitializeComponent();

            viewModel.Close += Close;
            viewModel.Initialize();

            DataContext = viewModel;
        }
    }
}
