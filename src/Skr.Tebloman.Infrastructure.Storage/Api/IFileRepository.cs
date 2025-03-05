
namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Represents a file based repository.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Reads the stored instances from disk.
        /// </summary>
        void Load();

        /// <summary>
        /// Writes the currently managed instances in memory to disk.
        /// </summary>
        void Save();

        /// <summary>
        /// Removes the storage file from disk.
        /// </summary>
        void Delete();
    }
}
