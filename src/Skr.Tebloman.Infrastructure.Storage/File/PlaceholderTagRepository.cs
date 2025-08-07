using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Runtime.Api;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Manages instances of <see cref="PlaceholderTag"/>.
    /// </summary>
    internal sealed class PlaceholderTagRepository : FileRepository<PlaceholderTag>, IPlaceholderTagRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlaceholderTagRepository"/>.
        /// </summary>
        /// <param name="storageFilePath">Filesystem path pointing to the storage file.</param>
        /// <param name="lifecycleManagement">Central lifecycle management.</param>
        public PlaceholderTagRepository(string storageFilePath, ILifecycleManager lifecycleManagement)
            : base(storageFilePath, lifecycleManagement)
        {
        }
    }
}
