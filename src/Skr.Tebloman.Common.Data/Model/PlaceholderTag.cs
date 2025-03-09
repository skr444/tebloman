using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Skr.Tebloman.Common.Data.Model
{
    /// <summary>
    /// Represents a placeholder tag.
    /// </summary>
    public sealed class PlaceholderTag : Entity
    {
        private Regex? instance;

        /// <summary>
        /// Gets or sets the regex pattern identifying the sequence that should be replaced.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the replacement sequence.
        /// </summary>
        public string Replacement { get; set; }

        /// <summary>
        /// Gets or sets the id of a replacement source for this placeholder.
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the character sequence denoting the beginning of this placeholder tag.
        /// </summary>
        public string StartMarker { get; set; }

        /// <summary>
        /// Gets or sets the character sequence denoting the end of this placeholder tag.
        /// </summary>
        public string EndMarker { get; set; }

        /// <summary>
        /// Gets the regex instance of this placeholder tag.
        /// </summary>
        public Regex RegexInstance => instance!;

        /// <summary>
        /// Creates a new instance of <see cref="PlaceholderTag"/>.
        /// </summary>
        public PlaceholderTag()
        {
            Pattern = ".";
            Replacement = String.Empty;
            SourceId = null;
            StartMarker = "{";
            EndMarker = "}";
            Build();
        }

        /// <summary>
        /// Compiles the regex instance.
        /// </summary>
        public void Build()
        {
            instance = new Regex(
                $"\\{StartMarker}{Pattern}(\\:(?<Selector>Date|Weekday))*?(\\:(?<Format>[YyMmDdIi]))*?(\\:(?<Culture>[a-z]{{2}}\\-[A-Z]{{2}}))*?\\{EndMarker}",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Replaces all matches of this placeholder tag in a given text.
        /// </summary>
        /// <param name="input">The text containing placeholders to replace with this tag.</param>
        /// <param name="source">The source from which to get the replacement string.</param>
        /// <returns>The <paramref name="input"/> string with all occurrences of this placeholder tag replaced.</returns>
        public string Replace(string input, ReplacementSource? source = null)
        {
            return instance!.Replace(input, x =>
            {
                if (source == null)
                {
                    return Replacement;
                }

                if (source.IsDateSource)
                {
                    DateTime dateCandidate = source.Date ?? DateTime.Now;
                    string candidate = dateCandidate.ToString();

                    string? format = x.Groups["Format"]?.Value;
                    if (!String.IsNullOrEmpty(format))
                    {
                        if ((format == "i") || (format == "I"))
                        {
                            return dateCandidate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }

                        string? culture = x.Groups["Culture"]?.Value;
                        if (String.IsNullOrEmpty(culture))
                        {
                            return dateCandidate.ToString(format);
                        }
                        else
                        {
                            return dateCandidate.ToString(format, CultureInfo.CreateSpecificCulture(culture));
                        }
                    }

                    return (x.Groups["Selector"]?.Value) switch
                    {
                        "Date" => dateCandidate.ToShortDateString(),
                        "Weekday" => dateCandidate.DayOfWeek.ToString(),
                        _ => dateCandidate.ToString("D"),
                    };
                }

                return source.Text ?? Replacement;
            });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Pattern;
        }
    }
}
