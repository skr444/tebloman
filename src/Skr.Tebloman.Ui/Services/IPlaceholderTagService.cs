using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Ui.Services
{
    /// <summary>
    /// Provides means to convey instances of <see cref="PlaceholderTag"/> from one viewmodel to another.
    /// </summary>
    public interface IPlaceholderTagService
    {
        /// <summary>
        /// Gets or sets the instance of <see cref="PlaceholderTag"/> to convey.
        /// </summary>
        PlaceholderTag? Placeholder { get; set; }

        /// <summary>
        /// Fetches this placeholder instance from the storage.
        /// </summary>
        /// <param name="repository">The placeholder repository.</param>
        /// <returns>The retrieved instance of <see cref="PlaceholderTag"/> or a new blank instance.</returns>
        PlaceholderTag GetPlaceholder(IPlaceholderTagRepository repository);
    }
}
