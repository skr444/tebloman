using System;
using System.Collections.Generic;

namespace Skr.Tebloman.Infrastructure.Storage.Api
{
    /// <summary>
    /// Provides CRUD operations for the specified data type.
    /// </summary>
    /// <typeparam name="TData">Type of data to operate.</typeparam>
    public interface IRepository<TData>
    {
        /// <summary>
        /// Attempts to retrieve a single instance of <typeparamref name="TData"/>.
        /// </summary>
        /// <param name="id">Identifier of the requested instance.</param>
        /// <param name="instance">The retrieved instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the requested instance was retrieved, otherwise <see langword="false"/>.
        /// </returns>
        bool TryGet(Guid id, out TData? instance);

        /// <summary>
        /// Retrieves all existing instances from this repository.
        /// </summary>
        /// <returns>A collection of available instances.</returns>
        ICollection<TData> All();

        /// <summary>
        /// Adds the provided instance to the repository if it is not present,
        /// otherwise updates the existing instance.
        /// </summary>
        /// <param name="data">The instance to add or update.</param>
        void AddOrUpdate(TData data);

        /// <summary>
        /// Removes the instance with the provided identifier from this repository.
        /// </summary>
        /// <param name="id">Identifier for the instance that should be removed.</param>
        void Delete(Guid id);
    }
}
