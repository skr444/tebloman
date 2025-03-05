using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Manages instances of <see cref="Fragment"/>.
    /// </summary>
    internal sealed class FragmentRepository : FileRepository<Fragment>, IFragmentRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="FragmentRepository"/>.
        /// </summary>
        /// <param name="storagePath">Filesystem path pointing to the storage file.</param>
        public FragmentRepository(string storagePath) : base(storagePath)
        {
        }
    }
}
