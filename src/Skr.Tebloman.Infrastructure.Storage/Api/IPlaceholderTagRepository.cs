using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Provides means to manage instances of <see cref="PlaceholderTag"/>.
    /// </summary>
    public interface IPlaceholderTagRepository : IRepository<PlaceholderTag>, IFileRepository
    {
    }
}
