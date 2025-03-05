using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Manages instances of <see cref="Profile"/>.
    /// </summary>
    internal sealed class ProfileRepository : FileRepository<Profile>, IProfileRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProfileRepository"/>.
        /// </summary>
        /// <param name="storagePath">Filesystem path pointing to the storage file.</param>
        public ProfileRepository(string storagePath) : base(storagePath)
        {
        }
    }
}
