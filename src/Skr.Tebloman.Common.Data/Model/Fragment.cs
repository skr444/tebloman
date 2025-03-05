using System;
using System.Collections.Generic;

namespace Skr.Tebloman.Common.Data.Model
{
    /// <summary>
    /// Represents a frequently used text block.
    /// </summary>
    public sealed class Fragment : Entity
    {
        /// <summary>
        /// Gets or sets the name of this fragment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the textual contents of this block.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the profile ids this fragment is assigned to.
        /// </summary>
        public ICollection<Guid> Profiles { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Fragment"/>.
        /// </summary>
        public Fragment()
        {
            Name = String.Empty;
            Text = String.Empty;
            Profiles = [];
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
