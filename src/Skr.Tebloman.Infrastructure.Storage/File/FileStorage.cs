using System;
using System.IO;

using Skr.Tebloman.Infrastructure.Runtime.Api;
using Skr.Tebloman.Infrastructure.Storage.Api;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Manages file based repositories.
    /// </summary>
    internal sealed class FileStorage : IFileStorage
    {
        private readonly string storageDirectory;
        private readonly IProfileRepository profileRepository;
        private readonly IPlaceholderTagRepository placeholderTagRepository;
        private readonly IFragmentRepository fragmentRepository;
        private readonly IReplacementSourceRepository replacementSourceRepository;

        /// <inheritdoc/>
        public string StorageDirectory => storageDirectory;

        /// <summary>
        /// Creates a new instance of <see cref="FileStorage"/>.
        /// </summary>
        /// <param name="storageDir">Application data storage directory.</param>
        /// <param name="lifecycleManagement">Central lifecycle management.</param>
        public FileStorage(string storageDir, ILifecycleManager lifecycleManagement)
        {
            storageDirectory = storageDir;

            profileRepository = new ProfileRepository(Path.Combine(storageDirectory, "profiles.json"), lifecycleManagement);
            placeholderTagRepository = new PlaceholderTagRepository(Path.Combine(storageDirectory, "placeholders.json"),
                lifecycleManagement);
            fragmentRepository = new FragmentRepository(Path.Combine(storageDirectory, "fragments.json"), lifecycleManagement);
            replacementSourceRepository = new ReplacementSourceRepository(
                Path.Combine(storageDirectory, "replacementSources.json"),
                lifecycleManagement);
        }

        /// <inheritdoc/>
        public T GetRepository<T>()
        {
            var t = typeof(T);
            switch (true)
            {
                case var _ when t.IsAssignableFrom(typeof(IProfileRepository)):
                    return (T)profileRepository;

                case var _ when t.IsAssignableFrom(typeof(IPlaceholderTagRepository)):
                    return (T)placeholderTagRepository;

                case var _ when t.IsAssignableFrom(typeof(IFragmentRepository)):
                    return (T)fragmentRepository;

                case var _ when t.IsAssignableFrom(typeof(IReplacementSourceRepository)):
                    return (T)replacementSourceRepository;

                default:
                    throw new ArgumentException($"The requested repository type '{typeof(T).Name}' is not supported.",
                        nameof(T));
            }
        }

        /// <inheritdoc/>
        public void Load()
        {
            profileRepository.Load();
            placeholderTagRepository.Load();
            fragmentRepository.Load();
            replacementSourceRepository.Load();
        }

        /// <inheritdoc/>
        public void Save()
        {
            profileRepository.Save();
            placeholderTagRepository.Save();
            fragmentRepository.Save();
            replacementSourceRepository.Save();
        }

        /// <inheritdoc/>
        public void Delete()
        {
            profileRepository.Delete();
            placeholderTagRepository.Delete();
            fragmentRepository.Delete();
            replacementSourceRepository.Delete();
        }
    }
}
