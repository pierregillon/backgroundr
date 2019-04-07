using System.Windows;

namespace backgroundr.view.windows.parameters
{
    /// <summary>
    /// Interaction logic for ParametersWindow.xaml
    /// </summary>
    public partial class ParametersWindow : Window
    {
        public ParametersWindow(ParametersViewModel viewModel)
        {
            InitializeComponent();

            viewModel.Close += Close;

            DataContext = viewModel;
        }
    }
}