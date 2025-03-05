using System;

using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Ui.Services.Implementation
{
    /// <inheritdoc/>
    internal sealed class PlaceholderTagService : IPlaceholderTagService
    {
        /// <inheritdoc/>
        public PlaceholderTag? Placeholder { get; set; }

        /// <inheritdoc/>
        public PlaceholderTag GetPlaceholder(IPlaceholderTagRepository repository)
        {
            if (repository.TryGet(Placeholder?.Id ?? Guid.Empty, out PlaceholderTag? placeholder))
            {
                return placeholder!;
            }

            return Placeholder ?? new PlaceholderTag();
        }
    }
}
