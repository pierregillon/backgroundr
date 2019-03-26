using System.Windows;
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
        }
    }
}
