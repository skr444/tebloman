using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Skr.Tebloman.Common.Data;
using Skr.Tebloman.Infrastructure.Storage.Api;

using IoFile = System.IO.File;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Reads and writes instances of <typeparamref name="TData"/> from and to disk.
    /// </summary>
    /// <typeparam name="TData">Data type of the instance to persist.</typeparam>
    internal abstract class FileRepository<TData> : IFileRepository, IRepository<TData> where TData : Entity
    {
        private readonly string path;
        private readonly JsonSerializerOptions serializeReadOptions;
        private readonly JsonSerializerOptions serializeWriteOptions;
        protected IDictionary<Guid, TData> store;

        /// <summary>
        /// Creates a new instance of <see cref="FileRepository{TData}"/>.
        /// </summary>
        /// <param name="storageFilePath">Filesystem path pointing to the storage file.</param>
        protected FileRepository(string storageFilePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(storageFilePath, nameof(storageFilePath));

            path = storageFilePath;

            serializeReadOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            serializeWriteOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            store = new Dictionary<Guid, TData>();
            Load();
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

            Save();
        }

        /// <inheritdoc />
        public ICollection<TData> All()
        {
            Load();

            return store.Values;
        }

        /// <inheritdoc />
        public void Delete(Guid id)
        {
            if (store.ContainsKey(id))
            {
                store.Remove(id);
                Save();
            }
        }

        /// <inheritdoc />
        public bool TryGet(Guid id, out TData? instance)
        {
            instance = null;
            Load();

            return store.TryGetValue(id, out instance);
        }

        /// <inheritdoc />
        public void Save()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(store, serializeWriteOptions);
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <inheritdoc />
        public void Load()
        {
            if (IoFile.Exists(path))
            {
                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    store = JsonSerializer.Deserialize<IDictionary<Guid, TData>>(stream, serializeReadOptions) ?? new Dictionary<Guid, TData>();
                }
            }
            else
            {
                Save();
            }
        }

        /// <inheritdoc />
        public void Delete()
        {
            if (IoFile.Exists(path))
            {
                IoFile.Delete(path);
            }
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
