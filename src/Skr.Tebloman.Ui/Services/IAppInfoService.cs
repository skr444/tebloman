namespace Skr.Tebloman.Ui.Services
{
    /// <summary>
    /// Provides information about the application.
    /// </summary>
    public interface IAppInfoService
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// Gets the application title.
        /// </summary>
        string AppTitle { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the copyright information.
        /// </summary>
        string Copyright { get; }
    }
}
