using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Skr.Tebloman.Common.Data;

namespace Skr.Tebloman.Infrastructure.Storage.Test
{
    /// <summary>
    /// Test cases for <see cref="Repository{TData}"/>.
    /// </summary>
    [TestFixture]
    internal class RepositoryTest
    {
        #region Types

        /// <summary>
        /// Concrete <see cref="Entity"/> for testing purposes.
        /// </summary>
        private sealed class TestData : Entity
        {
            public string Data { get; set; }

            public TestData()
            {
                Data = String.Empty;
            }
        }

        /// <summary>
        /// Test fixture variant of <see cref="Repository{TData}"/>.
        /// </summary>
        private sealed class ConcreteRepository : Repository<TestData>
        {
            public IDictionary<Guid, TestData> Entities { get; set; }

            public ConcreteRepository() : base()
            {
                Entities = new Dictionary<Guid, TestData>();
                Acquire();
            }

            public void SetEntities(IEnumerable<TestData> entities)
            {
                Entities = new Dictionary<Guid, TestData>(entities.ToDictionary(k => k.Id, v => v));
                store = new Dictionary<Guid, TestData>(Entities);
            }

            protected override void Acquire()
            {
                store = new Dictionary<Guid, TestData>(Entities);
            }

            protected override void Persist()
            {
                Entities = new Dictionary<Guid, TestData>(store);
            }
        }

        #endregion Types

        private static readonly Guid Id1 = Guid.NewGuid();
        private static readonly Guid Id2 = Guid.NewGuid();
        private static readonly Guid Id3 = Guid.NewGuid();

        private ConcreteRepository unitUnderTest;

        [SetUp]
        public void Setup()
        {
            unitUnderTest = new ConcreteRepository();
        }

        [TearDown]
        public void Cleanup()
        {
            unitUnderTest = null;
        }

        [Test]
        public void AddOrUpdateAddTest()
        {
            var t1 = new TestData
            {
                Data = "hello"
            };
            unitUnderTest.AddOrUpdate(t1);

            Assert.That(unitUnderTest.Entities.Count, Is.EqualTo(1));
            Assert.That(unitUnderTest.Entities.FirstOrDefault().Value, Is.SameAs(t1));
            Assert.That(unitUnderTest.Entities[t1.Id], Is.SameAs(t1));
        }

        [Test]
        public void AddOrUpdateUpdateTest()
        {
            var t1 = new TestData
            {
                Data = "hello"
            };
            unitUnderTest.SetEntities([t1]);
            Assert.That(unitUnderTest.Entities.Count, Is.EqualTo(1));

            t1.Data = "miau";
            unitUnderTest.AddOrUpdate(t1);

            Assert.That(unitUnderTest.Entities.Count, Is.EqualTo(1));
            Assert.That(unitUnderTest.Entities.FirstOrDefault().Value, Is.SameAs(t1));
            Assert.That(unitUnderTest.Entities[t1.Id], Is.SameAs(t1));
            Assert.That(unitUnderTest.Entities[t1.Id].Data, Is.SameAs("miau"));
        }

        /// <summary>
        /// Checks whether added instances are returned.
        /// </summary>
        [Test]
        public void AddRetrieveSameInstanceTest()
        {
            var t1 = new TestData
            {
                Data = "hello"
            };
            unitUnderTest.AddOrUpdate(t1);

            TestData t2 = null;
            var success = unitUnderTest.TryGet(t1.Id, out t2);

            Assert.That(success, Is.True);
            Assert.That(t2, Is.Not.Null);
            Assert.That(t1, Is.SameAs(t2));
            Assert.That(t1.Data, Is.EqualTo(t2.Data));
        }

        [Test]
        public void AddMultipleRetrieveTest()
        {
            const int numberOfItems = 10;

            var col1 = new List<TestData>();
            for (var i = 0; i < numberOfItems; i++)
            {
                col1.Add(new TestData { Data = $"hello_#{i}" });
                unitUnderTest.AddOrUpdate(col1.LastOrDefault()!);
            }

            var col2 = unitUnderTest.All();
             
            Assert.That(col2.Count, Is.EqualTo(col1.Count));
            for (var i = 0; i < numberOfItems; ++i)
            {
                Assert.That(col1.Count(x => x.Id == col2.ElementAt(i).Id), Is.EqualTo(1));
                Assert.That(col1[i], Is.SameAs(col2.ElementAt(i)));
            }
        }

        [Test]
        public void AllTest()
        {
            var t1 = new TestData { Id = Id1, Data = "one" };
            var t2 = new TestData { Id = Id2, Data = "two" };
            var t3 = new TestData { Id = Id3, Data = "three" };
            unitUnderTest.SetEntities([t1, t2, t3]);

            var all = unitUnderTest.All();

            Assert.That(all.Count, Is.EqualTo(3));
            Assert.That(all.Contains(t1), Is.True);
            Assert.That(all.Contains(t2), Is.True);
            Assert.That(all.Contains(t3), Is.True);
        }

        [Test]
        public void DeleteTest()
        {
            var t1 = new TestData { Id = Id1, Data = "one" };
            var t2 = new TestData { Id = Id2, Data = "two" };
            var t3 = new TestData { Id = Id3, Data = "three" };
            unitUnderTest.SetEntities([t1, t2, t3]);

            unitUnderTest.Delete(Id2);

            Assert.That(unitUnderTest.All().Count, Is.EqualTo(2));
            Assert.That(unitUnderTest.TryGet(Id1, out _), Is.True);
            Assert.That(unitUnderTest.TryGet(Id2, out _), Is.False);
            Assert.That(unitUnderTest.TryGet(Id3, out _), Is.True);
        }
    }
}
