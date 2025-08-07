using System;
using System.Collections.Generic;

using Skr.Tebloman.Common.Data;

namespace Skr.Tebloman.Infrastructure.Storage
{
    /// <summary>
    /// Stores instances of <typeparamref name="TData"/> in memory.
    /// </summary>
    /// <typeparam name="TData">Data type of the instance to persist.</typeparam>
    internal abstract class Repository<TData> : IRepository<TData> where TData : Entity
    {
        protected IDictionary<Guid, TData> store;

        protected Repository()
        {
            store = new Dictionary<Guid, TData>();
        }

        /// <inheritdoc />
        public void AddOrUpdate(TData data)
        {
            Validate(data);

            if (store.ContainsKey(data.Id))
            {
                store.Remove(data.Id);
            }

            store.Add(data.Id, data);

            data.Modified = DateTime.UtcNow;
            Persist();
        }

        /// <inheritdoc />
        public ICollection<TData> All()
        {
            Acquire();

            return store.Values;
        }

        /// <inheritdoc />
        public void Delete(Guid id)
        {
            if (store.Remove(id))
            {
                Persist();
            }
        }

        /// <inheritdoc />
        public bool TryGet(Guid id, out TData? instance)
        {
            instance = null;
            Acquire();

            return store.TryGetValue(id, out instance);
        }

        /// <summary>
        /// Performs the necessary actions to persist the current store data.
        /// </summary>
        protected virtual void Persist()
        {
        }

        /// <summary>
        /// Performs the necessary actions to update the store with the latest persisted data version.
        /// </summary>
        protected virtual void Acquire()
        {
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="data">The instance to check.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="data"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="data"/> has an invalid identifier.</exception>
        protected static void Validate(TData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (data.Id == Guid.Empty)
            {
                throw new ArgumentException("Invalid id.", nameof(data));
            }
        }
    }
}
