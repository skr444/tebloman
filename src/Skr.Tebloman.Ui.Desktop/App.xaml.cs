using System;
using System.IO;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using CommunityToolkit.Mvvm.DependencyInjection;

using Skr.Tebloman.Infrastructure.Storage.Api;
using Skr.Tebloman.Infrastructure.Storage.File;
using Skr.Tebloman.Ui.Services;
using Skr.Tebloman.Ui.Services.Implementation;
using Skr.Tebloman.Ui.Desktop.ViewModels;

namespace Skr.Tebloman.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AppName = "Tebloman";
        private readonly string storageDirectory;

        public App()
        {
            InitializeComponent();
            storageDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName);
            if (!Directory.Exists(storageDirectory))
            {
                Directory.CreateDirectory(storageDirectory);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton<IFileStorage, FileStorage>(
                    _ => new FileStorage(storageDirectory))
                .AddSingleton<IPlaceholderTagService, PlaceholderTagService>()
                .AddSingleton<IAppInfoService, AppInfoService>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<PlaceholderTagEditorViewModel>()
                .AddTransient<AboutWindowViewModel>()
                .BuildServiceProvider());

            base.OnStartup(e);
        }
    }
}
