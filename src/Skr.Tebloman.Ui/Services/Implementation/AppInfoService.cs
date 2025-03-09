using System.Diagnostics;
using System.Reflection;

namespace Skr.Tebloman.Ui.Services.Implementation
{
    /// <inheritdoc/>
    internal sealed class AppInfoService : IAppInfoService
    {
        private const string AppNameValue = "Tebloman";
        private const string AppTitleValue = "Text Block Manager";

        private readonly AssemblyName assemblyName;
        private readonly Assembly assembly;
        private readonly FileVersionInfo fileVersionInfo;

        /// <inheritdoc/>
        public string AppName => fileVersionInfo.CompanyName ?? AppNameValue;

        /// <inheritdoc/>
        public string AppTitle => fileVersionInfo.LegalTrademarks ?? AppTitleValue;

        /// <inheritdoc/>
        public string Version => fileVersionInfo.FileVersion ?? assemblyName.Version?.ToString() ?? "n/a";

        /// <inheritdoc/>
        public string Copyright => fileVersionInfo.LegalCopyright ?? "skr444";

        /// <summary>
        /// Creates a new instance of <see cref="AppInfoService"/>.
        /// </summary>
        public AppInfoService()
        {
            assembly = Assembly.GetExecutingAssembly();
            assemblyName = assembly.GetName();
            fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        }

        public override string ToString()
        {
            return $"AppName='{AppName}', AppTitle='{AppTitle}', Version='{Version}', ProductName='{fileVersionInfo.ProductName}', LegalTrademarks='{fileVersionInfo.LegalTrademarks}', FileVersion='{fileVersionInfo.FileVersion}', FileName='{fileVersionInfo.FileName}'";
        }
    }
}
