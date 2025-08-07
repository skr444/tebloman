using System;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace Skr.Tebloman.Ui.Helper
{
    /// <summary>
    /// WPF friendly implementation of <see cref="RelayCommand"/>.
    /// </summary>
    /// <remarks>
    ///     Uses the <see cref="CommandManager"/> to propagate and update can execute states.
    /// </remarks>
    public sealed class WpfRelayCommand : IRelayCommand
    {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="WpfRelayCommand"/>.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public WpfRelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            ArgumentNullException.ThrowIfNull(execute);
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            execute();
        }

        /// <summary>
        /// Does nothing since can execute changed handling is done through <see cref="CommandManager"/>.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            // do nothing
        }
    }
}
