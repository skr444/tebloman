using System;
using System.Threading;

using Skr.Tebloman.Infrastructure.Runtime.Api;

namespace Skr.Tebloman.Infrastructure.Runtime.Implementation
{
    /// <inheritdoc />
    internal sealed class LifecycleManager : ILifecycleManager
    {
        private readonly CancellationTokenSource cts;
        private readonly Action shutdownSequence;

        /// <inheritdoc />
        public CancellationToken Token => cts.Token;

        /// <summary>
        /// Creates a new instance of <see cref="LifecycleManager"/>.
        /// </summary>
        /// <param name="shutdown">The shutdown sequence.</param>
        public LifecycleManager(Action shutdown)
        {
            cts = new CancellationTokenSource();
            shutdownSequence = shutdown;
        }

        /// <inheritdoc />
        public void Cancel()
        {
            cts.Cancel();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            cts.Cancel();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            cts.Dispose();
            shutdownSequence?.Invoke();
        }
    }
}
