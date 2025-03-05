using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Skr.Tebloman.Common.Data.Model;
using Skr.Tebloman.Infrastructure.Storage.Api;
using Skr.Tebloman.Ui.Desktop.Views;
using Skr.Tebloman.Ui.Services;
using Skr.Tebloman.Ui.ViewModels;
using System.Threading;

namespace Skr.Tebloman.Ui.Desktop.ViewModels
{
    /// <summary>
    /// Provides interaction logic for the <see cref="PlaceholderTagEditor"/> view.
    /// </summary>
    internal sealed class PlaceholderTagEditorViewModel : ObservableObject
    {
        private readonly IPlaceholderTagRepository placeholderTagRepository;
        private readonly IReplacementSourceRepository replacementSourceRepository;

        private PlaceholderTagListItemViewModel? placeholder;
        private ReplacementSource? replacementSource;
        private string? statusText;
        private readonly CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Gets a list of all available replacement sources.
        /// </summary>
        public IEnumerable<string> ReplacementSources =>
            replacementSourceRepository.All().Select(x => x.Name);

        /// <summary>
        /// Gets or sets the currently selected replacement source.
        /// </summary>
        public string SelectedReplacementSource
        {
            get => replacementSource?.Name ?? String.Empty;
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var source = replacementSourceRepository.All().FirstOrDefault(x => x.Name == value);
                    if (source != null)
                    {
                        replacementSource = source;
                        if (placeholder?.Item != null)
                        {
                            placeholder.Item.SourceId = replacementSource.Id;
                        }
                        OnPropertyChanged(nameof(ReplacementSourceName));
                        OnPropertyChanged(nameof(HasReplacementSource));
                        OnPropertyChanged(nameof(ReplacementSourceComboBoxEnabled));
                        OnPropertyChanged(nameof(ReplacementTextBoxEnabled));
                    }
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the name of the currently selected replacement source.
        /// </summary>
        public string ReplacementSourceName => replacementSource?.Name ?? String.Empty;

        /// <summary>
        /// Gets or sets the pattern of the currently edited placeholder.
        /// </summary>
        public string Pattern
        {
            get => placeholder?.Item.Pattern ?? String.Empty;
            set
            {
                if (placeholder?.Item != null)
                {
                    placeholder.Item.Pattern = value;
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently edited placeholder has a replacement source.
        /// </summary>
        public bool HasReplacementSource
        {
            get => placeholder?.Item?.SourceId != null;
            set
            {
                if (placeholder?.Item != null)
                {
                    if (value)
                    {
                        placeholder.Item.SourceId = replacementSource?.Id ?? Guid.NewGuid();
                    }
                    else
                    {
                        placeholder.Item.SourceId = null;
                    }

                    OnPropertyChanged(nameof(ReplacementSourceComboBoxEnabled));
                    OnPropertyChanged(nameof(ReplacementTextBoxEnabled));
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the replacement source combobox is enabled.
        /// </summary>
        public bool ReplacementSourceComboBoxEnabled => HasReplacementSource;

        /// <summary>
        /// Gets or sets a value indicating whether the replacement value textbox is enabled.
        /// </summary>
        public bool ReplacementTextBoxEnabled => !HasReplacementSource;

        /// <summary>
        /// Gets or sets a value that should be replaced for this placeholder.
        /// </summary>
        public string Replacement
        {
            get => placeholder?.Item.Replacement ?? String.Empty;
            set
            {
                if (placeholder?.Item != null)
                {
                    placeholder.Item.Replacement = value;
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the character sequence that marks the start of the placeholder.
        /// </summary>
        public string StartMarker
        {
            get => placeholder?.Item.StartMarker ?? String.Empty;
            set
            {
                if (placeholder?.Item != null)
                {
                    placeholder.Item.StartMarker = value;
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the character sequence that marks the end of the placeholder.
        /// </summary>
        public string EndMarker
        {
            get => placeholder?.Item.EndMarker ?? String.Empty;
            set
            {
                if (placeholder?.Item != null)
                {
                    placeholder.Item.EndMarker = value;
                }

                saveCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the status bar text.
        /// </summary>
        public string StatusText => statusText ?? String.Empty;

        private RelayCommand newCommand;

        /// <summary>
        /// Creates a new placeholder.
        /// </summary>
        public ICommand NewCommand => newCommand;

        private RelayCommand saveCommand;

        /// <summary>
        /// Saves the currently placeholder to the file storage.
        /// </summary>
        public ICommand SaveCommand => saveCommand;

        /// <summary>
        /// Closes the window.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PlaceholderTagEditorViewModel"/>.
        /// </summary>
        /// <param name="fileStorage">File storage service.</param>
        /// <param name="placeholderTagService">Placeholder tag service.</param>
        public PlaceholderTagEditorViewModel(IFileStorage fileStorage, IPlaceholderTagService placeholderTagService)
        {
            placeholderTagRepository = fileStorage.GetRepository<IPlaceholderTagRepository>();
            replacementSourceRepository = fileStorage.GetRepository<IReplacementSourceRepository>();

            placeholder = new PlaceholderTagListItemViewModel(placeholderTagService.GetPlaceholder(placeholderTagRepository));

            cancellationTokenSource = new CancellationTokenSource();

            newCommand = new RelayCommand(() =>
            {
                placeholder = new PlaceholderTagListItemViewModel(new PlaceholderTag());
                OnPropertyChanged(nameof(Pattern));
                OnPropertyChanged(nameof(HasReplacementSource));
                OnPropertyChanged(nameof(ReplacementSourceComboBoxEnabled));
                OnPropertyChanged(nameof(ReplacementTextBoxEnabled));
                OnPropertyChanged(nameof(Replacement));
                OnPropertyChanged(nameof(StartMarker));
                OnPropertyChanged(nameof(EndMarker));
                saveCommand?.NotifyCanExecuteChanged();
            });

            saveCommand = new RelayCommand(() =>
            {
                if (placeholder?.Item != null)
                {
                    placeholderTagRepository.AddOrUpdate(placeholder.Item);
                    placeholderTagService.Placeholder = placeholder.Item;
                    Task.Run(async () =>
                    {
                        await NotifySaved(cancellationTokenSource.Token);
                    }, cancellationTokenSource.Token);
                }
            }, () =>
            {
                return    !String.IsNullOrWhiteSpace(placeholder?.Item?.Pattern)
                       && (   !String.IsNullOrWhiteSpace(placeholder?.Item?.Replacement)
                           || (placeholder?.Item?.SourceId != null))
                       && !String.IsNullOrWhiteSpace(placeholder?.Item?.StartMarker)
                       && !String.IsNullOrWhiteSpace(placeholder?.Item?.EndMarker);
            });

            CloseCommand = new RelayCommand<IClosable>(CloseWindow);
        }

        /// <summary>
        /// Terminates asynchronous operations and closes this window.
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(IClosable? window)
        {
            cancellationTokenSource.Cancel();
            window?.Close();
        }

        /// <summary>
        /// Displays a disappearing status notification.
        /// </summary>
        /// <param name="token">Token for cancelling this operation.</param>
        /// <returns>An instance of <see cref="Task"/> to smoothe out the async/await usage.</returns>
        private async Task<Task> NotifySaved(CancellationToken token)
        {
            try
            {
                statusText = "Saved";
                OnPropertyChanged(nameof(StatusText));
                await Task.Delay(TimeSpan.FromSeconds(2), token);
                statusText = String.Empty;
                OnPropertyChanged(nameof(StatusText));
            }
            catch (TaskCanceledException)
            {
            }

            return Task.CompletedTask;
        }
    }
}
