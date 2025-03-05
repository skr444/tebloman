using Skr.Tebloman.Common.Data.Model;
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
        public PlaceholderTagRepository(string storageFilePath) : base(storageFilePath)
        {
        }
    }
}
