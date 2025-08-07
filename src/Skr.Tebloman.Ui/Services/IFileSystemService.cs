namespace Skr.Tebloman.Ui.Services
{
    /// <summary>
    /// Provides access to the file system.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Opens the specified folder in Windows explorer.
        /// </summary>
        /// <param name="path">The directory to open in explorer. </param>
        void OpenFolderInExplorer(string path);

        /// <summary>
        /// Opens the application folder in Windows explorer.
        /// </summary>
        void OpenApplicationDataFolderInExplorer();
    }
}
