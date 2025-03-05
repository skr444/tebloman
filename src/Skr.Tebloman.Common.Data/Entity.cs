using System;

namespace Skr.Tebloman.Common.Data
{
    /// <summary>
    /// Represents a trackable data entitiy.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets a unique identifier for this entity.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets the timestamp when this item was created.
        /// </summary>
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets the timestamp when this item was last changed.
        /// </summary>
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Entity"/>.
        /// </summary>
        protected Entity()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }
    }
}
