using System;

using CommunityToolkit.Mvvm.ComponentModel;

using Skr.Tebloman.Infrastructure.Runtime.Api;

namespace Skr.Tebloman.Ui.Helper
{
    /// <summary>
    /// Extends the <see cref="ObservableObject"/> with centralized closing mechanism.
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        protected readonly ILifecycleManager lifecycleManager;

        /// <summary>
        /// Notifies requests to close the window.
        /// </summary>
        public EventHandler<EventArgs>? CloseRequested;

        /// <summary>
        /// Creates a new instance of <see cref="BaseViewModel"/>.
        /// </summary>
        /// <param name="lifecycleManagement">The application lifecycle management service.</param>
        protected BaseViewModel(ILifecycleManager lifecycleManagement)
        {
            lifecycleManager = lifecycleManagement;
        }

        /// <summary>
        /// Requests this window to close.
        /// </summary>
        protected void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Processes a close request.
        /// </summary>
        /// <returns><see langword="true"/> if closing can proceed, otherwise <see langword="false"/>.</returns>
        public virtual bool ProcessCloseRequest()
        {
            return true;
        }
    }
}
