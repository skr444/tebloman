using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Provides means to manage instances of <see cref="ReplacementSource"/>.
    /// </summary>
    public interface IReplacementSourceRepository : IRepository<ReplacementSource>, IFileRepository
    {
    }
}
