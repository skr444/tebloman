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
using Skr.Tebloman.Infrastructure.Runtime.Api;
using Skr.Tebloman.Infrastructure.Runtime.Implementation;

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
            var lifecycleManager = new LifecycleManager(Current.Shutdown);
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton<ILifecycleManager, LifecycleManager>(
                    _ => lifecycleManager)
                .AddSingleton<IFileStorage, FileStorage>(
                    _ => new FileStorage(storageDirectory, lifecycleManager))
                .AddSingleton<IPlaceholderTagService, PlaceholderTagService>()
                .AddSingleton<IAppInfoService, AppInfoService>()
                .AddSingleton<IFileSystemService, FileSystemService>()
                .AddTransient<MainWindowViewModel>()
                .AddTransient<PlaceholderTagEditorViewModel>()
                .AddTransient<AboutWindowViewModel>()
                .BuildServiceProvider());

            base.OnStartup(e);
        }
    }
}
