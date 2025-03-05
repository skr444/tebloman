using System;
using System.Collections.Generic;
using System.Linq;

namespace Skr.Tebloman.Common.Data.Model
{
    /// <summary>
    /// Represents a named group of text blocks.
    /// </summary>
    public sealed class Profile : Entity
    {
        /// <summary>
        /// Gets or sets the name of this profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a collection of fragments associated to this profile.
        /// </summary>
        public IDictionary<Guid, uint> Fragments { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Profile"/>.
        /// </summary>
        public Profile()
        {
            Name = String.Empty;
            Fragments = new Dictionary<Guid, uint>();
        }

        /// <summary>
        /// Adds a fragment to this profile.
        /// </summary>
        /// <param name="id">The id of the fragment to add.</param>
        public void AddFragment(Guid id)
        {
            if (!Fragments.Any())
            {
                Fragments.Add(id, 0);
            }
            else
            {
                Fragments.Add(id, Fragments.Values.Max() + 1);
            }
        }

        /// <summary>
        /// Removes a fragment from this profile.
        /// </summary>
        /// <param name="id">The id of the fragment to remove.</param>
        public bool RemoveFragment(Guid id)
        {
            if (!Fragments.ContainsKey(id))
            {
                return false;
            }

            uint removedPosition = Fragments[id];

            var needPositionUpdate = new List<Guid>();
            foreach (var fragment in Fragments)
            {
                if (removedPosition <= fragment.Value)
                {
                    needPositionUpdate.Add(fragment.Key);
                }
            }

            foreach (var fragment in needPositionUpdate)
            {
                Fragments[fragment]--;
            }

            return Fragments.Remove(id);
        }

        /// <summary>
        /// Moves the fragment up by one position.
        /// </summary>
        /// <param name="id">The id of the fragment to move up.</param>
        public void MoveFragmentUp(Guid id)
        {
            if (!Fragments.ContainsKey(id))
            {
                return;
            }

            uint movedPosition = Fragments[id];

            if (movedPosition == 0)
            {
                return;
            }

            movedPosition--;

            foreach (var fragment in Fragments)
            {
                if (movedPosition == fragment.Value)
                {
                    Fragments[fragment.Key]++;
                }
            }

            Fragments[id]--;
        }

        /// <summary>
        /// Moves the fragment down by one position.
        /// </summary>
        /// <param name="id">The id of the fragment to move down.</param>
        public void MoveFragmentDown(Guid id)
        {
            if (!Fragments.ContainsKey(id))
            {
                return;
            }

            uint movedPosition = Fragments[id];

            if (movedPosition == Fragments.Values.Max())
            {
                return;
            }

            movedPosition++;

            foreach (var fragment in Fragments)
            {
                if (movedPosition == fragment.Value)
                {
                    Fragments[fragment.Key]--;
                }
            }

            Fragments[id]++;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
