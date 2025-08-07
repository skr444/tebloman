using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using Skr.Tebloman.Ui.Desktop.ViewModels;
using Skr.Tebloman.Ui.Helper;

namespace Skr.Tebloman.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();

            if (DataContext is BaseViewModel vm)
            {
                vm.CloseRequested += (_, _) => Close();
            }

            Closing += (object? _, CancelEventArgs e) =>
            {
                if (DataContext is BaseViewModel vm)
                {
                    e.Cancel = !vm.ProcessCloseRequest();
                }
            };
        }
    }
}
