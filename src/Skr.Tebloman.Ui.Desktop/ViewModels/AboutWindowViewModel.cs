using System;
using System.Collections.Generic;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Skr.Tebloman.Ui.Desktop.Views;
using Skr.Tebloman.Ui.Services;

namespace Skr.Tebloman.Ui.Desktop.ViewModels
{
    /// <summary>
    /// Provides interaction logic for the <see cref="AboutWindow"/> view.
    /// </summary>
    internal sealed class AboutWindowViewModel : ObservableObject
    {
        private readonly IAppInfoService appInfoService;
        private readonly IDictionary<string, string> appInfo;

        /// <summary>
        /// Creates a new instance of <see cref="AboutWindowViewModel"/>.
        /// </summary>
        /// <param name="infoService">App info provider.</param>
        public AboutWindowViewModel(IAppInfoService infoService)
        {
            appInfoService = infoService;
            appInfo = new Dictionary<string, string>
            {
                { "Application name", appInfoService.AppTitle },
                { "Application short name", appInfoService.AppName },
                { "Copyright", appInfoService.Copyright },
                { "Year", DateTime.UtcNow.Year.ToString() },
                { "Version", appInfoService.Version },
            };
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle => $"{appInfoService.AppName} - About";

        /// <summary>
        /// Gets the application information table.
        /// </summary>
        public IDictionary<string, string> AppInfo => appInfo;

        public ICommand CloseCommand => new RelayCommand<IClosable>(CloseWindow);

        /// <summary>
        /// Closes this window.
        /// </summary>
        /// <param name="window">The reference to the window instance.</param>
        private void CloseWindow(IClosable? window)
        {
            window?.Close();
        }
    }
}
