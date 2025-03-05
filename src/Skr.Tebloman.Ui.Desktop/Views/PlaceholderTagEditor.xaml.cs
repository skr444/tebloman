using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using Skr.Tebloman.Ui.Desktop.ViewModels;

namespace Skr.Tebloman.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PlaceholderTagEditor.xaml
    /// </summary>
    public partial class PlaceholderTagEditor : Window, IClosable
    {
        public PlaceholderTagEditor()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<PlaceholderTagEditorViewModel>();
        }
    }
}
