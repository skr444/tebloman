using System;

namespace Skr.Tebloman.Common.Data.Model
{
    /// <summary>
    /// Represents a source for replacement data.
    /// </summary>
    public sealed class ReplacementSource : Entity
    {
        /// <summary>
        /// Gets or sets the name of this replacement source.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text for this source.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets the date for this source.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ReplacementSource"/>.
        /// </summary>
        public ReplacementSource()
        {
            Name = String.Empty;
            Text = null;
            Date = null;
        }

        /// <summary>
        /// Gets a value indicating whether this source is a date source.
        /// </summary>
        public bool IsDateSource => Date.HasValue;

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
