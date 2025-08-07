using System.Windows;

using CommunityToolkit.Mvvm.DependencyInjection;

using Skr.Tebloman.Ui.Desktop.ViewModels;
using Skr.Tebloman.Ui.Helper;

namespace Skr.Tebloman.Ui.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PlaceholderTagEditor.xaml
    /// </summary>
    public partial class PlaceholderTagEditor : Window
    {
        public PlaceholderTagEditor()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<PlaceholderTagEditorViewModel>();

            if (DataContext is BaseViewModel vm)
            {
                vm.CloseRequested += (_, _) => Close();
            }

            Closing += (_, e) =>
            {
                if (DataContext is BaseViewModel vm)
                {
                    e.Cancel = !vm.ProcessCloseRequest();
                }
            };
        }
    }
}
