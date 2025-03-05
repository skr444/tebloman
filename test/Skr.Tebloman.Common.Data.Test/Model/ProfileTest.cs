using System;
using System.Collections.Generic;

using NUnit.Framework;

using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Common.Data.Test.Model
{
    /// <summary>
    /// Unit tests for <see cref="Profile"/>.
    /// </summary>
    [TestFixture]
    public class ProfileTest
    {
        private static Guid Id1 = Guid.NewGuid();
        private static Guid Id2 = Guid.NewGuid();
        private static Guid Id3 = Guid.NewGuid();
        private static Guid Id4 = Guid.NewGuid();
        private static Guid Id5 = Guid.NewGuid();

        private Profile unitUnderTest;

        [SetUp]
        public void Setup()
        {
            unitUnderTest = new Profile();
            unitUnderTest.Fragments.Add(Id1, 0);
            unitUnderTest.Fragments.Add(Id2, 1);
            unitUnderTest.Fragments.Add(Id3, 2);
            unitUnderTest.Fragments.Add(Id4, 3);
            unitUnderTest.Fragments.Add(Id5, 4);
        }

        [TearDown]
        public void Cleanup()
        {
            unitUnderTest = null!;
        }

        /// <summary>
        /// Checks whether the first added fragment has positiion '0'.
        /// </summary>
        [Test]
        public void AddFragmentFirstItemTest()
        {
            unitUnderTest.Fragments = new Dictionary<Guid, uint>();

            unitUnderTest.AddFragment(Id1);

            Assert.That(unitUnderTest.Fragments.Keys, Has.Exactly(1).EqualTo(Id1));
            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
        }

        /// <summary>
        /// Checks whether the added fragment has the highest position index.
        /// </summary>
        [Test]
        public void AddFragmentTest()
        {
            var id = Guid.NewGuid();
            unitUnderTest.AddFragment(id);

            Assert.That(unitUnderTest.Fragments.Keys, Has.Exactly(1).EqualTo(id));
            Assert.That(unitUnderTest.Fragments[id], Is.EqualTo(5));
        }

        /// <summary>
        /// Checks whether the position indices of the remaining fragments are correctly updated after a fragment has been removed from the middle.
        /// </summary>
        [Test]
        public void RemoveFragmentTest()
        {
            unitUnderTest.RemoveFragment(Id3);

            Assert.That(unitUnderTest.Fragments.Keys, Has.None.EqualTo(Id3));
            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
            Assert.That(unitUnderTest.Fragments[Id2], Is.EqualTo(1));
            Assert.That(unitUnderTest.Fragments[Id4], Is.EqualTo(2));
            Assert.That(unitUnderTest.Fragments[Id5], Is.EqualTo(3));
        }

        /// <summary>
        /// Checks whether no position indices are updated when the last fragment is removed.
        /// </summary>
        [Test]
        public void RemoveFragmentLastItemTest()
        {
            var id = Guid.NewGuid();
            unitUnderTest.Fragments = new Dictionary<Guid, uint>();
            unitUnderTest.AddFragment(id);

            unitUnderTest.RemoveFragment(id);

            Assert.That(unitUnderTest.Fragments.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Checks whether the position indices are correctly updated when a fragment is moved up.
        /// </summary>
        [Test]
        public void MoveFragmentUpTest()
        {
            unitUnderTest.MoveFragmentUp(Id4);

            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
            Assert.That(unitUnderTest.Fragments[Id2], Is.EqualTo(1));
            Assert.That(unitUnderTest.Fragments[Id3], Is.EqualTo(3));
            Assert.That(unitUnderTest.Fragments[Id4], Is.EqualTo(2));
            Assert.That(unitUnderTest.Fragments[Id5], Is.EqualTo(4));
        }

        /// <summary>
        /// Checks whether no position indices are updated if the first fragment is moved up.
        /// </summary>
        [Test]
        public void MoveFragmentUpFirstItemTest()
        {
            unitUnderTest.MoveFragmentUp(Id1);

            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
            Assert.That(unitUnderTest.Fragments[Id2], Is.EqualTo(1));
            Assert.That(unitUnderTest.Fragments[Id3], Is.EqualTo(2));
            Assert.That(unitUnderTest.Fragments[Id4], Is.EqualTo(3));
            Assert.That(unitUnderTest.Fragments[Id5], Is.EqualTo(4));
        }

        /// <summary>
        /// Checks whether the position indices are correctly updated when a fragment is moved down.
        /// </summary>
        [Test]
        public void MoveFragmentDownTest()
        {
            unitUnderTest.MoveFragmentDown(Id3);

            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
            Assert.That(unitUnderTest.Fragments[Id2], Is.EqualTo(1));
            Assert.That(unitUnderTest.Fragments[Id3], Is.EqualTo(3));
            Assert.That(unitUnderTest.Fragments[Id4], Is.EqualTo(2));
            Assert.That(unitUnderTest.Fragments[Id5], Is.EqualTo(4));
        }

        /// <summary>
        /// Checks whether no position indices are changed if the last fragment is moved down.
        /// </summary>
        [Test]
        public void MoveFragmentDownLastItemTest()
        {
            unitUnderTest.MoveFragmentDown(Id5);

            Assert.That(unitUnderTest.Fragments[Id1], Is.EqualTo(0));
            Assert.That(unitUnderTest.Fragments[Id2], Is.EqualTo(1));
            Assert.That(unitUnderTest.Fragments[Id3], Is.EqualTo(2));
            Assert.That(unitUnderTest.Fragments[Id4], Is.EqualTo(3));
            Assert.That(unitUnderTest.Fragments[Id5], Is.EqualTo(4));
        }
    }
}
