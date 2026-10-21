using System.Windows;
namespace Industrial.UI.Wpf.Demo
{
    public partial class MainWindow : Window
    {
        private readonly DemoViewModel _viewModel = new DemoViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Closed += (sender, args) => _viewModel.Dispose();
        }
    }
}
