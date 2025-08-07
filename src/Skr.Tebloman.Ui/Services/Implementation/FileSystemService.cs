using System.IO;
using System.Diagnostics;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Ui.Services.Implementation
{
    /// <inheritdoc/>
    internal sealed class FileSystemService : IFileSystemService
    {
        private const string ExplorerExe = "explorer.exe";

        private readonly IFileStorage storage;

        /// <summary>
        /// Creates a new instance of <see cref="FileSystemService"/>.
        /// </summary>
        /// <param name="fileStorage">File storage service.</param>
        public FileSystemService(IFileStorage fileStorage)
        {
            storage = fileStorage;
        }

        /// <inheritdoc/>
        public void OpenApplicationDataFolderInExplorer()
        {
            OpenFolderInExplorer(storage.StorageDirectory);
        }

        /// <inheritdoc/>
        public void OpenFolderInExplorer(string path)
        {
            if (Directory.Exists(path))
            {
                Process.Start(ExplorerExe, path);
            }
        }
    }
}
