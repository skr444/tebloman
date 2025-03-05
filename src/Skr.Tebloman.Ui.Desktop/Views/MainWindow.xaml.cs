using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;
using Skr.Tebloman.Ui.Desktop.ViewModels;

namespace Skr.Tebloman.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IClosable
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();
        }
    }
}