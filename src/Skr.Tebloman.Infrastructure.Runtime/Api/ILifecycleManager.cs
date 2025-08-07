using System;
using System.Threading;

namespace Skr.Tebloman.Infrastructure.Runtime.Api
{
    /// <summary>
    /// Provides tools for application lifecycle management.
    /// </summary>
    public interface ILifecycleManager
    {
        /// <summary>
        /// Gets a cancellation token that signals application shutdown.
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// Requests all cancellation tokens to signal application shutdown.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Exits the application gracefully.
        /// </summary>
        void Shutdown();
    }
}
