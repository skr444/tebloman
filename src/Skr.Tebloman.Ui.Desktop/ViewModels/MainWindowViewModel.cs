using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using Skr.Tebloman.Infrastructure.Storage.Api;
using Skr.Tebloman.Ui.ViewModels;
using Skr.Tebloman.Common.Data.Model;
using CommunityToolkit.Mvvm.Input;
using Skr.Tebloman.Ui.Desktop.Views;
using Skr.Tebloman.Ui.Services;
using System.Windows.Media;
using Skr.Tebloman.Common.Data;
using System.Threading.Tasks;
using System.Threading;

namespace Skr.Tebloman.Ui.Desktop.ViewModels
{
    /// <summary>
    /// Provides interaction logic for the <see cref="MainWindow"/> view.
    /// </summary>
    internal sealed class MainWindowViewModel : ObservableObject
    {
        #region Types

        /// <summary>
        /// Enumerates different sources of fragments.
        /// </summary>
        private enum FragmentSource
        {
            /// <summary>
            /// Indicates that the currently displayed fragment originates from the global fragment store.
            /// </summary>
            Global = 0,

            /// <summary>
            /// Indicates that the currently displayed fragment is assigned to the current profile.
            /// </summary>
            Profile
        }

        #endregion Types

        private const int StatusMessageDurationSeconds = 3;

        private readonly IProfileRepository profileRepository;
        private readonly IPlaceholderTagRepository placeholderTagRepository;
        private readonly IFragmentRepository fragmentRepository;
        private readonly IReplacementSourceRepository replacementSourceRepository;
        private readonly IAppInfoService appInfoService;

        private ProfileListItemViewModel profile;
        private PlaceholderTagListItemViewModel placeholder;
        private FragmentListItemViewModel fragment;
        private ReplacementSourceListItemViewModel replacementSource;
        private FragmentSource fragmentSource;
        private string? statusText;
        private readonly CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle => $"{appInfoService.AppName} - {appInfoService.AppTitle}";

        #region Menu

        /// <summary>
        /// Terminates this application.
        /// </summary>
        public ICommand ExitCommand => new RelayCommand(() =>
        {
            Application.Current.Shutdown();
        });

        /// <summary>
        /// Loads the data repositories from disk.
        /// </summary>
        public ICommand LoadFromDiskCommand => new RelayCommand(() =>
        {
            OnPropertyChanged(nameof(Placeholders));
            OnPropertyChanged(nameof(GlobalFragments));
            OnPropertyChanged(nameof(Profiles));
            OnPropertyChanged(nameof(ReplacementSources));
        });

        /// <summary>
        /// Persists the current application data to disk.
        /// </summary>
        public ICommand SaveToDiskCommand => new RelayCommand(() =>
        {
            profileRepository.Save();
            placeholderTagRepository.Save();
            fragmentRepository.Save();
            replacementSourceRepository.Save();
        });

        /// <summary>
        /// Opens the about window.
        /// </summary>
        public ICommand AboutCommand => new RelayCommand(() =>
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        });

        #endregion Menu

        #region Profiles

        /// <summary>
        /// Gets a list of available profiles.
        /// </summary>
        public IEnumerable<ProfileListItemViewModel> Profiles =>
            profileRepository.All().Select(x => new ProfileListItemViewModel(x));

        /// <summary>
        /// Gets or sets the currently selected profile.
        /// </summary>
        public ProfileListItemViewModel SelectedProfile
        {
            get => profile ?? new ProfileListItemViewModel(new Profile());
            set
            {
                if (value != null)
                {
                    profile = value;
                    OnPropertyChanged(nameof(SelectedProfileName));
                    OnPropertyChanged(nameof(ProfileFragments));
                    OnPropertyChanged(nameof(ResultText));
                }
                saveProfileCommand.NotifyCanExecuteChanged();
                removeProfileCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the currently selected profile.
        /// </summary>
        public string SelectedProfileName
        {
            get => profile?.Item.Name ?? String.Empty;
            set
            {
                if (value != null)
                {
                    profile.Item.Name = value;
                }
                saveProfileCommand.NotifyCanExecuteChanged();
                removeProfileCommand.NotifyCanExecuteChanged();
            }
        }

        private readonly RelayCommand newProfileCommand;

        /// <summary>
        /// Creates a new profile instance.
        /// </summary>
        public ICommand NewProfileCommand => newProfileCommand;

        private readonly RelayCommand saveProfileCommand;

        /// <summary>
        /// Saves the currently selected profile in the storage.
        /// </summary>
        public ICommand SaveProfileCommand => saveProfileCommand;

        private readonly RelayCommand removeProfileCommand;

        /// <summary>
        /// Removes the currently selected profile from the storage.
        /// </summary>
        public ICommand RemoveProfileCommand => removeProfileCommand;

        #endregion Profiles

        #region Placeholders

        /// <summary>
        /// Gets a list of the currently available placeholder tags.
        /// </summary>
        public IEnumerable<PlaceholderTagListItemViewModel> Placeholders =>
            placeholderTagRepository.All().Select(x => new PlaceholderTagListItemViewModel(x));

        /// <summary>
        /// Gets or sets the currently selected placeholder tag.
        /// </summary>
        public PlaceholderTagListItemViewModel SelectedPlaceholder
        {
            get => placeholder ?? new PlaceholderTagListItemViewModel(new PlaceholderTag());
            set
            {
                if (value != null)
                {
                    placeholder = value;
                    OnPropertyChanged(nameof(PlaceholderPattern));
                    OnPropertyChanged(nameof(PlaceholderReplacement));
                    OnPropertyChanged(nameof(ResultText));
                }
            }
        }

        /// <summary>
        /// Gets the pattern of the currently selected placeholder.
        /// </summary>
        public string PlaceholderPattern => placeholder?.Item?.Pattern ?? String.Empty;

        /// <summary>
        /// Gets the replacement value of the currently selected placeholder.
        /// </summary>
        public string PlaceholderReplacement => placeholder?.Item?.Replacement ?? String.Empty;

        private readonly RelayCommand removePlaceholderCommand;

        /// <summary>
        /// Removes the currently selected placeholder.
        /// </summary>
        public ICommand RemovePlaceholderCommand => removePlaceholderCommand;

        private readonly RelayCommand editPlaceholderCommand;

        /// <summary>
        /// Opens the editor view for the currently selected placeholder.
        /// </summary>
        public ICommand EditPlaceholderCommand => editPlaceholderCommand;

        #endregion Placeholders

        #region Fragments

        /// <summary>
        /// Gets a list of all fragments assigned to the currently selected profile.
        /// </summary>
        public IEnumerable<FragmentListItemViewModel> ProfileFragments => SelectedProfile?.Item.Fragments.OrderBy(x => x.Value).Select(x =>
        {
            if (fragmentRepository.TryGet(x.Key, out Fragment? fragmentCandidate))
            {
                return new FragmentListItemViewModel(fragmentCandidate!);
            }
            return new FragmentListItemViewModel(new Fragment());
        }) ?? Array.Empty<FragmentListItemViewModel>();

        /// <summary>
        /// Gets a list of all available fragments.
        /// </summary>
        public IEnumerable<FragmentListItemViewModel> GlobalFragments =>
            fragmentRepository.All().Select(x => new FragmentListItemViewModel(x));

        /// <summary>
        /// Gets or sets the currently selected fragment associated with the currently selected profile.
        /// </summary>
        public FragmentListItemViewModel SelectedProfileFragment
        {
            get => SelectedFragment;
            set
            {
                fragmentSource = FragmentSource.Profile;
                SelectedFragment = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected fragment from the fragment pool.
        /// </summary>
        public FragmentListItemViewModel SelectedGlobalFragment
        {
            get => SelectedFragment;
            set
            {
                fragmentSource = FragmentSource.Global;
                SelectedFragment = value;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected fragment.
        /// </summary>
        private FragmentListItemViewModel SelectedFragment
        {
            get => fragment ?? new FragmentListItemViewModel(new Fragment());
            set
            {
                fragment = value ?? new FragmentListItemViewModel(new Fragment());
                addFragmentToProfileCommand.NotifyCanExecuteChanged();
                removeFragmentFromProfileCommand.NotifyCanExecuteChanged();
                saveFragmentCommand.NotifyCanExecuteChanged();
                removeFragmentCommand.NotifyCanExecuteChanged();
                moveFragmentDownCommand.NotifyCanExecuteChanged();
                moveFragmentUpCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(FragmentName));
                OnPropertyChanged(nameof(FragmentText));
                OnPropertyChanged(nameof(ProfileFragmentsBackground));
                OnPropertyChanged(nameof(GlobalFragmentsBackground));
            }
        }

        /// <summary>
        /// Gets or sets the name of the currently selected fragment.
        /// </summary>
        public string FragmentName
        {
            get => SelectedFragment.Item.Name ?? String.Empty;
            set
            {
                if (value != null)
                {
                    SelectedFragment.Item.Name = value;
                }
                saveFragmentCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text of the currently selected fragment.
        /// </summary>
        public string FragmentText
        {
            get => SelectedFragment.Item.Text ?? String.Empty;
            set
            {
                if (value != null)
                {
                    SelectedFragment.Item.Text = value;
                }
                saveFragmentCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the background color of the profile fragment list.
        /// </summary>
        public Brush ProfileFragmentsBackground => (fragmentSource == FragmentSource.Profile)
            ? new SolidColorBrush(Color.FromRgb(210, 255, 220))
            : new SolidColorBrush(Colors.White);

        /// <summary>
        /// Gets or sets the background color of the fragment pool list.
        /// </summary>
        public Brush GlobalFragmentsBackground => (fragmentSource == FragmentSource.Global)
            ? new SolidColorBrush(Color.FromRgb(210, 255, 220))
            : new SolidColorBrush(Colors.White);

        private readonly RelayCommand addFragmentToProfileCommand;

        /// <summary>
        /// Add the currently selected fragment to the currently selected profile.
        /// </summary>
        public ICommand AddFragmentToProfileCommand => addFragmentToProfileCommand;

        private readonly RelayCommand removeFragmentFromProfileCommand;

        /// <summary>
        /// Removes the currently selected fragment from the currently selected profile.
        /// </summary>
        public ICommand RemoveFragmentFromProfileCommand => removeFragmentFromProfileCommand;

        private readonly RelayCommand moveFragmentUpCommand;

        /// <summary>
        /// Moves the currently selected fragment up by one position in the currently selected profile.
        /// </summary>
        public ICommand MoveFragmentUpCommand => moveFragmentUpCommand;

        private readonly RelayCommand moveFragmentDownCommand;

        /// <summary>
        /// Moves the currently selected fragment down by one position in the currently selected profile.
        /// </summary>
        public ICommand MoveFragmentDownCommand => moveFragmentDownCommand;

        private readonly RelayCommand newFragmentCommand;

        /// <summary>
        /// Creates a new black fragment.
        /// </summary>
        public ICommand NewFragmentCommand => newFragmentCommand;

        private readonly RelayCommand saveFragmentCommand;

        /// <summary>
        /// Saves the currently selected fragment to the file storage.
        /// </summary>
        public ICommand SaveFragmentCommand => saveFragmentCommand;

        private readonly RelayCommand removeFragmentCommand;

        /// <summary>
        /// Removes the currently selected fragment from the file storage.
        /// </summary>
        public ICommand RemoveFragmentCommand => removeFragmentCommand;

        #endregion Fragments

        #region Replacement sources

        /// <summary>
        /// Gets a list of the currently available replacement sources.
        /// </summary>
        public IEnumerable<ReplacementSourceListItemViewModel> ReplacementSources =>
            replacementSourceRepository.All().Select(x => new ReplacementSourceListItemViewModel(x));

        /// <summary>
        /// Gets or sets the currently selected replacement source.
        /// </summary>
        public ReplacementSourceListItemViewModel SelectedReplacementSource
        {
            get => replacementSource ?? new ReplacementSourceListItemViewModel(new ReplacementSource());
            set
            {
                if (value != null)
                {
                    replacementSource = value;
                    OnPropertyChanged(nameof(ReplacementSourceName));
                    OnPropertyChanged(nameof(ReplacementSourceIsDate));
                    OnPropertyChanged(nameof(ReplacementSourceText));
                    OnPropertyChanged(nameof(ReplacementSourceDate));
                    OnPropertyChanged(nameof(ReplacementSourceDataTextBoxEnabled));
                    OnPropertyChanged(nameof(ReplacementSourceDatePickerEnabled));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the currently selected replacement source.
        /// </summary>
        public string ReplacementSourceName
        {
            get => replacementSource?.Item.Name ?? String.Empty;
            set
            {
                if (replacementSource?.Item != null)
                {
                    replacementSource.Item.Name = value;
                    saveReplacementSourceCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently selected replacement source is a date source.
        /// </summary>
        public bool ReplacementSourceIsDate
        {
            get => replacementSource?.Item?.IsDateSource ?? false;
            set
            {
                if ((replacementSource != null) && (replacementSource.Item != null))
                {
                    if (value)
                    {
                        if (DateTime.TryParse(replacementSource.Item.Text, out DateTime candidate))
                        {
                            replacementSource.Item.Date = candidate;
                        }
                        else
                        {
                            replacementSource.Item.Date = DateTime.Now;
                        }

                        replacementSource.Item.Text = null;
                    }
                    else
                    {
                        replacementSource.Item.Text = replacementSource.Item.Date.ToString();
                        replacementSource.Item.Date = null;
                    }
                }

                OnPropertyChanged(nameof(ReplacementSourceText));
                OnPropertyChanged(nameof(ReplacementSourceDate));
                OnPropertyChanged(nameof(ReplacementSourceDataTextBoxEnabled));
                OnPropertyChanged(nameof(ReplacementSourceDatePickerEnabled));
            }
        }

        /// <summary>
        /// Gets or sets the text of the currently selected replacement source.
        /// </summary>
        public string ReplacementSourceText
        {
            get => replacementSource?.Item?.Text ?? String.Empty;
            set
            {
                if ((replacementSource?.Item != null) && !replacementSource.Item.IsDateSource)
                {
                    replacementSource.Item.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the date of the currently selected replacement source.
        /// </summary>
        public DateTime ReplacementSourceDate
        {
            get => replacementSource?.Item?.Date ?? DateTime.Now;
            set
            {
                if ((replacementSource?.Item != null) && replacementSource.Item.IsDateSource)
                {
                    replacementSource.Item.Date = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the text box for the currently selected replacement source is enabled.
        /// </summary>
        public bool ReplacementSourceDataTextBoxEnabled => !ReplacementSourceIsDate;

        /// <summary>
        /// Gets a value indicating whether the date picker for the currently selected replacement source is enabled.
        /// </summary>
        public bool ReplacementSourceDatePickerEnabled => ReplacementSourceIsDate;

        private readonly RelayCommand newReplacementSourceCommand;

        /// <summary>
        /// Creates a new replacement source.
        /// </summary>
        public ICommand NewReplacementSourceCommand => newReplacementSourceCommand;

        private readonly RelayCommand saveReplacementSourceCommand;

        /// <summary>
        /// Saves the currently selected replacement source in the file storage.
        /// </summary>
        public ICommand SaveReplacementSourceCommand => saveReplacementSourceCommand;

        private readonly RelayCommand removeReplacementSourceCommand;

        /// <summary>
        /// Removes the currently selected replacement source from the file storage.
        /// </summary>
        public ICommand RemoveReplacementSourceCommand => removeReplacementSourceCommand;

        #endregion Replacement sources

        /// <summary>
        /// Gets the assembled text from the fragments of the currently selected profile.
        /// </summary>
        public string ResultText
        {
            get
            {
                var text = new StringBuilder();
                bool first = true;
                foreach (FragmentListItemViewModel fragment in ProfileFragments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        text.AppendLine();
                    }

                    text.AppendLine(ReplacePlaceholders(fragment.Item, placeholderTagRepository.All()));
                }
                return text.ToString();
            }
        }

        /// <summary>
        /// Copies the current result text to the clip board.
        /// </summary>
        public ICommand CopyToClipBoardCommand => new RelayCommand(() =>
        {
            string text = ResultText;
            if (!String.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
                NotifyStatus("Text copied to clip board");
            }
        });

        /// <summary>
        /// Gets the status bar text.
        /// </summary>
        public string StatusText => statusText ?? String.Empty;

        /// <summary>
        /// Terminates asyncronous operations and closes the main window.
        /// </summary>
        public ICommand WindowClosingCommand { get; }

        /// <summary>
        /// Creates a new instance of <see cref="MainWindowViewModel"/>.
        /// </summary>
        /// <param name="fileStorage">The file storage service.</param>
        /// <param name="placeholderTagService">The placeholder tag service.</param>
        public MainWindowViewModel(IFileStorage fileStorage, IPlaceholderTagService placeholderTagService,
            IAppInfoService infoService)
        {
            profileRepository = fileStorage.GetRepository<IProfileRepository>();
            placeholderTagRepository = fileStorage.GetRepository<IPlaceholderTagRepository>();
            fragmentRepository = fileStorage.GetRepository<IFragmentRepository>();
            replacementSourceRepository = fileStorage.GetRepository<IReplacementSourceRepository>();
            appInfoService = infoService;

            var repaired = RepairRepositories();

            profile = new ProfileListItemViewModel(profileRepository.All().FirstOrDefault() ?? new Profile());
            placeholder = new PlaceholderTagListItemViewModel(placeholderTagRepository.All().FirstOrDefault() ?? new PlaceholderTag());
            fragment = new FragmentListItemViewModel(fragmentRepository.All().FirstOrDefault() ?? new Fragment());
            replacementSource = new ReplacementSourceListItemViewModel(replacementSourceRepository.All().FirstOrDefault() ?? new ReplacementSource());
            fragmentSource = FragmentSource.Global;

            cancellationTokenSource = new CancellationTokenSource();

            // profile
            newProfileCommand = new RelayCommand(() =>
            {
                SelectedProfile = new ProfileListItemViewModel(new Profile());
                OnPropertyChanged(nameof(FragmentName));
                OnPropertyChanged(nameof(FragmentText));
            });
            saveProfileCommand = new RelayCommand(() =>
            {
                profileRepository.AddOrUpdate(profile.Item);
                OnPropertyChanged(nameof(Profiles));
                NotifySaved(profile.Item);
            }, () => !String.IsNullOrEmpty(profile?.Item.Name));
            removeProfileCommand = new RelayCommand(() =>
            {
                if (PromptRemove(profile.Item) == MessageBoxResult.Yes)
                {
                    profileRepository.Delete(profile.Item.Id);
                    SelectedProfile = new ProfileListItemViewModel(profileRepository.All().FirstOrDefault() ?? new Profile());
                    OnPropertyChanged(nameof(Profiles));
                    OnPropertyChanged(nameof(FragmentName));
                    OnPropertyChanged(nameof(FragmentText));
                }
            }, () =>
            {
                return (profile != null);
            });

            // placeholder
            removePlaceholderCommand = new RelayCommand(() =>
            {
                if (PromptRemove(placeholder.Item) == MessageBoxResult.Yes)
                {
                    placeholderTagRepository.Delete(placeholder.Item.Id);
                    SelectedPlaceholder = new PlaceholderTagListItemViewModel(
                        placeholderTagRepository.All().FirstOrDefault() ?? new PlaceholderTag());
                    OnPropertyChanged(nameof(Placeholders));
                    OnPropertyChanged(nameof(ResultText));
                }
            }, () =>
            {
                return (placeholder != null);
            });
            editPlaceholderCommand = new RelayCommand(() =>
            {
                placeholderTagService.Placeholder = placeholder.Item;
                PlaceholderTagEditor editor = new PlaceholderTagEditor();
                editor.Closed += (sender, e) =>
                {
                    placeholder = new PlaceholderTagListItemViewModel(placeholderTagService.GetPlaceholder(placeholderTagRepository));
                    OnPropertyChanged(nameof(Placeholders));
                    OnPropertyChanged(nameof(PlaceholderPattern));
                    OnPropertyChanged(nameof(PlaceholderReplacement));
                    OnPropertyChanged(nameof(ResultText));
                };
                editor.ShowDialog();
            }, () =>
            {
                return (placeholder?.Item != null);
            });

            // fragment
            addFragmentToProfileCommand = new RelayCommand(() =>
            {
                if (   !profile.Item.Fragments.Keys.Any(x => fragment.Item.Id == x)
                    && !fragment.Item.Profiles.Any(x => profile.Item.Id == x))
                {
                    fragment.Item.Profiles.Add(profile.Item.Id);
                    profile.Item.AddFragment(fragment.Item.Id);
                    fragmentRepository.AddOrUpdate(fragment.Item);
                    profileRepository.AddOrUpdate(profile.Item);
                    OnPropertyChanged(nameof(ProfileFragments));
                    OnPropertyChanged(nameof(ResultText));
                }
            }, () =>
            {
                return    (fragmentSource == FragmentSource.Global)
                       && (!fragment?.Item.Profiles.Any(x => profile?.Item.Id == x) ?? false);
            });
            removeFragmentFromProfileCommand = new RelayCommand(() =>
            {
                if (profile.Item.RemoveFragment(fragment.Item.Id))
                {
                    fragment.Item.Profiles.Remove(profile.Item.Id);
                    fragmentRepository.AddOrUpdate(fragment.Item);
                    profileRepository.AddOrUpdate(profile.Item);
                    OnPropertyChanged(nameof(ProfileFragments));
                    OnPropertyChanged(nameof(ResultText));
                }
            }, () =>
            {
                return    (fragmentSource == FragmentSource.Profile)
                       && (fragment?.Item.Profiles.Any(x => profile?.Item.Id == x) ?? false);
            });
            moveFragmentUpCommand = new RelayCommand(() =>
            {
                profile.Item.MoveFragmentUp(fragment.Item.Id);
                profileRepository.AddOrUpdate(profile.Item);
                OnPropertyChanged(nameof(ProfileFragments));
                OnPropertyChanged(nameof(ResultText));
            }, () =>
            {
                return    (fragmentSource == FragmentSource.Profile)
                       && (fragment?.Item != null)
                       && (profile?.Item != null);
            });
            moveFragmentDownCommand = new RelayCommand(() =>
            {
                profile.Item.MoveFragmentDown(fragment.Item.Id);
                profileRepository.AddOrUpdate(profile.Item);
                OnPropertyChanged(nameof(ProfileFragments));
                OnPropertyChanged(nameof(ResultText));
            }, () =>
            {
                return    (fragmentSource == FragmentSource.Profile)
                       && (fragment?.Item != null)
                       && (profile?.Item != null);
            });
            saveFragmentCommand = new RelayCommand(() =>
            {
                fragmentRepository.AddOrUpdate(fragment.Item);
                OnPropertyChanged(nameof(GlobalFragments));
                OnPropertyChanged(nameof(ProfileFragments));
                OnPropertyChanged(nameof(ResultText));
                NotifySaved(fragment.Item);
            }, () =>
            {
                return !String.IsNullOrEmpty(fragment?.Item.Name)
                    && !String.IsNullOrEmpty(fragment?.Item.Text);
            });
            newFragmentCommand = new RelayCommand(() =>
            {
                SelectedFragment = new FragmentListItemViewModel(new Fragment());
            });
            removeFragmentCommand = new RelayCommand(() =>
            {
                if (PromptRemove(fragment.Item) == MessageBoxResult.Yes)
                {
                    Guid fragmentId = fragment.Item.Id;
                    if (profile.Item.RemoveFragment(fragmentId))
                    {
                        fragment.Item.Profiles.Remove(profile.Item.Id);
                        fragmentRepository.AddOrUpdate(fragment.Item);
                        profileRepository.AddOrUpdate(profile.Item);
                        OnPropertyChanged(nameof(ProfileFragments));
                        OnPropertyChanged(nameof(ResultText));
                    }

                    fragmentRepository.Delete(fragmentId);
                    OnPropertyChanged(nameof(GlobalFragments));
                    SelectedFragment = new FragmentListItemViewModel(
                        fragmentRepository.All().FirstOrDefault() ?? new Fragment());
                }
            }, () =>
            {
                return (fragmentSource == FragmentSource.Global) && (fragment != null);
            });

            // replacement source
            newReplacementSourceCommand = new RelayCommand(() =>
            {
                SelectedReplacementSource = new ReplacementSourceListItemViewModel(new ReplacementSource
                {
                    Name = "source"
                });
            });
            saveReplacementSourceCommand = new RelayCommand(() =>
            {
                replacementSourceRepository.AddOrUpdate(replacementSource.Item);
                OnPropertyChanged(nameof(ReplacementSources));
                OnPropertyChanged(nameof(ResultText));
                NotifySaved(replacementSource.Item);
            }, () =>
            {
                return !String.IsNullOrEmpty(replacementSource?.Item.Name);
            });
            removeReplacementSourceCommand = new RelayCommand(() =>
            {
                if (PromptRemove(replacementSource.Item) == MessageBoxResult.Yes)
                {
                    replacementSourceRepository.Delete(replacementSource.Item.Id);
                    SelectedReplacementSource = new ReplacementSourceListItemViewModel(
                        replacementSourceRepository.All().FirstOrDefault() ?? new ReplacementSource());
                    OnPropertyChanged(nameof(ReplacementSources));
                    OnPropertyChanged(nameof(ResultText));
                }
            }, () =>
            {
                return (replacementSource != null);
            });

            WindowClosingCommand = new RelayCommand<IClosable>(OnWindowClosing);
        }

        #region Helpers

        /// <summary>
        /// Proxy method for <see cref="Window.Close"/>.
        /// Here it's used as a handler for the closed event.
        /// </summary>
        /// <param name="window">The instance of the window that was closed.</param>
        private void OnWindowClosing(IClosable? window)
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Replaces all placeholders in a fragment with their corresponding values.
        /// </summary>
        /// <param name="fragment">The fragment containing zero or more placeholders.</param>
        /// <param name="placeholderTags">A set of placeholder instances with their replacement values.</param>
        /// <returns>The fragment string with all matched placeholders replaced.</returns>
        private string ReplacePlaceholders(Fragment fragment, IEnumerable<PlaceholderTag> placeholderTags)
        {
            var fragmentText = fragment.Text;
            foreach (var placeholder in placeholderTags)
            {
                placeholder.Build();

                ReplacementSource? replacementSourceCandidate = null;
                if (   placeholder.SourceId.HasValue
                    && replacementSourceRepository.TryGet(placeholder.SourceId.Value, out ReplacementSource? source))
                {
                    replacementSourceCandidate = source;
                }
                fragmentText = placeholder.Replace(fragmentText, replacementSourceCandidate);
            }

            return fragmentText;
        }

        /// <summary>
        /// Performs maintenance on the repositories by removing orphaned references between profiles and fragments,
        /// as well as between replacement sources and placeholders.
        /// </summary>
        /// <returns>The number of repaired instances.</returns>
        private int RepairRepositories()
        {
            int repaired = 0;

            var inexistingFragmentIds = new List<Guid>();
            foreach (var profile in profileRepository.All())
            {
                foreach (var fragmentId in profile.Fragments.Keys)
                {
                    if (!fragmentRepository.TryGet(fragmentId, out Fragment? _))
                    {
                        inexistingFragmentIds.Add(fragmentId);
                    }
                }
            }

            var fixedProfiles = new List<Profile>();
            foreach (var fragmentId in inexistingFragmentIds)
            {
                foreach (var profile in profileRepository.All())
                {
                    if (profile.RemoveFragment(fragmentId))
                    {
                        fixedProfiles.Add(profile);
                        repaired++;
                    }
                }
            }

            foreach (var profile in fixedProfiles)
            {
                profileRepository.AddOrUpdate(profile);
            }

            var inexistingReplacementSourceIds = new List<Guid>();
            foreach (var placeholder in placeholderTagRepository.All())
            {
                if (   placeholder.SourceId.HasValue
                    && !replacementSourceRepository.TryGet(placeholder.SourceId.Value,
                            out ReplacementSource? _))
                {
                    inexistingReplacementSourceIds.Add(placeholder.SourceId.Value);
                }
            }

            var fixedPlaceholders = new List<PlaceholderTag>();
            foreach (var replacementSourceId in inexistingReplacementSourceIds)
            {
                foreach (var placeholder in placeholderTagRepository.All())
                {
                    if (placeholder.SourceId == replacementSourceId)
                    {
                        placeholder.SourceId = null;
                        fixedPlaceholders.Add(placeholder);
                        repaired++;
                    }
                }
            }

            foreach (var placeholder in fixedPlaceholders)
            {
                placeholderTagRepository.AddOrUpdate(placeholder);
            }

            return repaired;
        }

        /// <summary>
        /// Helper method to prompt the user for removal of an instance from the storage.
        /// </summary>
        /// <param name="entity">The instance about to be deleted.</param>
        /// <returns>An instance of <see cref="MessageBoxResult"/> indicating which option the user chose.</returns>
        private static MessageBoxResult PromptRemove(Entity entity)
        {
            string itemName = GetTypeLabel(entity.GetType());
            string message =
                $"You are about to delete a {itemName.ToLowerInvariant()}. This operation cannot be undone. Are you sure?";
            return MessageBox.Show(message, $"{itemName} removal!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Helper method to display a disappearing 'saved' status message.
        /// </summary>
        /// <param name="entity">The instance that was saved.</param>
        private void NotifySaved(Entity entity)
        {
            NotifyStatus($"{GetTypeLabel(entity.GetType())} saved");
        }

        /// <summary>
        /// Displays a disappearing message in the status bar.
        /// </summary>
        /// <param name="message">The message text.</param>
        private void NotifyStatus(string message)
        {
            Task.Run(async () =>
            {
                await DisplayStatusMessage(cancellationTokenSource.Token, message);
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Helper method to change the status text that is invoked asyncronously to achieve the disappearing effect.
        /// </summary>
        /// <param name="token">Cancellation token to abort the asynchronous operation.</param>
        /// <param name="message">The message text.</param>
        /// <returns>An instance of <see cref="Task"/> to smooth out async/await usage.</returns>
        private async Task<Task> DisplayStatusMessage(CancellationToken token, string message)
        {
            try
            {
                statusText = message;
                OnPropertyChanged(nameof(StatusText));
                await Task.Delay(TimeSpan.FromSeconds(StatusMessageDurationSeconds), token);
                statusText = String.Empty;
                OnPropertyChanged(nameof(StatusText));
            }
            catch (TaskCanceledException)
            {
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Transforms a type name into a multi word string.
        /// Basically a CamelCase to separate words converter.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>The converted type label.</returns>
        private static string GetTypeLabel(Type type)
        {
            var labelBuilder = new StringBuilder();
            string name = type.Name;

            foreach (var letter in name)
            {
                if (IsAsciiUpper(letter))
                {
                    labelBuilder.Append(' ');
                }
                labelBuilder.Append(letter);
            }

            string label = labelBuilder.ToString().ToLowerInvariant().Trim();
            label = String.Join(String.Empty, label.Select((x, i) => (i == 0) ? $"{x}".ToUpperInvariant() : $"{x}"));
            return label;
        }

        /// <summary>
        /// Checks whether an ASCII character is upper case.
        /// </summary>
        /// <param name="character">The ASCII character to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="character"/> is upper case, otherwise <see langword="false"/>.</returns>
        private static bool IsAsciiUpper(char character)
        {
            var numericalValue = (int)character;
            
            if ((numericalValue >= 65) && (numericalValue <= 90))
            {
                return true;
            }

            return false;
        }

        #endregion Helpers
    }
}
