using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Provides means to manage instances of <see cref="Profile"/>.
    /// </summary>
    public interface IProfileRepository : IRepository<Profile>, IFileRepository
    {
    }
}
