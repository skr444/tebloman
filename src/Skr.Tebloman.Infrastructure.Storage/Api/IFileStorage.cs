
namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Provides means to interact with the data storage.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Gets the repository for the requested data type.
        /// </summary>
        /// <typeparam name="T">Interface type of the repository.</typeparam>
        /// <returns>The requested repository.</returns>
        T GetRepository<T>();

        /// <summary>
        /// Reads all available repositories from disk.
        /// </summary>
        void Load();

        /// <summary>
        /// Stores all available repositories to disk.
        /// </summary>
        void Save();

        /// <summary>
        /// Removes the storage files of all available repositories from disk.
        /// </summary>
        void Delete();
    }
}
