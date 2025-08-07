using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Skr.Tebloman.Common.Data;
using Skr.Tebloman.Infrastructure.Runtime.Api;
using Skr.Tebloman.Infrastructure.Storage.Api;

using IoFile = System.IO.File;

namespace Skr.Tebloman.Infrastructure.Storage.File
{
    /// <summary>
    /// Reads and writes instances of <typeparamref name="TData"/> from and to disk.
    /// </summary>
    /// <typeparam name="TData">Data type of the instance to persist.</typeparam>
    internal abstract class FileRepository<TData> : Repository<TData>, IFileRepository where TData : Entity
    {
        private readonly string path;
        private readonly JsonSerializerOptions serializeReadOptions;
        private readonly JsonSerializerOptions serializeWriteOptions;
        private readonly ILifecycleManager lifecycleManager;

        /// <summary>
        /// Creates a new instance of <see cref="FileRepository{TData}"/>.
        /// </summary>
        /// <param name="storageFilePath">Filesystem path pointing to the storage file.</param>
        /// <param name="lifecycleManagement">Central lifecycle management.</param>
        protected FileRepository(string storageFilePath, ILifecycleManager lifecycleManagement) : base()
        {
            ArgumentException.ThrowIfNullOrEmpty(storageFilePath, nameof(storageFilePath));
            path = storageFilePath;

            ArgumentNullException.ThrowIfNull(lifecycleManagement, nameof(lifecycleManagement));
            lifecycleManager = lifecycleManagement;

            string? folder = Path.GetDirectoryName(path);
            if (!String.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

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

            Acquire();
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

        /// <inheritdoc />
        protected override void Acquire()
        {
            Load();
        }

        /// <inheritdoc />
        protected override void Persist()
        {
            Save();
        }
    }
}
