using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    internal sealed class ReplacementSourceRepository : FileRepository<ReplacementSource>, IReplacementSourceRepository
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReplacementSourceRepository"/>.
        /// </summary>
        /// <param name="storageFilePath">Filesystem path pointing to the storage file.</param>
        public ReplacementSourceRepository(string storageFilePath) : base(storageFilePath)
        {
        }
    }
}
