using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using Skr.Tebloman.Ui.Desktop.ViewModels;

namespace Skr.Tebloman.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window, IClosable
    {
        public AboutWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<AboutWindowViewModel>();
        }
    }
}
