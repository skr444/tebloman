using System;
using System.IO;
using System.Threading;
using IoFile = System.IO.File;

using NSubstitute;

using NUnit.Framework;

using Skr.Tebloman.Common.Data;
using Skr.Tebloman.Infrastructure.Runtime.Api;
using Skr.Tebloman.Infrastructure.Storage.File;

namespace Skr.Tebloman.Infrastructure.Storage.Test.File
{
    /// <summary>
    /// Test cases for <see cref="FileRepository"/>.
    /// </summary>
    [TestFixture]
    internal class FileRepositoryTest
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
        private sealed class ConcreteFileRepository : FileRepository<TestData>
        {
            public ConcreteFileRepository(string storageFilePath, ILifecycleManager lifecycleManagement)
                : base(storageFilePath, lifecycleManagement)
            {
            }

            protected override void Acquire()
            {
                Load();
            }

            protected override void Persist()
            {
                Save();
            }
        }

        #endregion Types

        private static readonly string FilePath;

        static FileRepositoryTest()
        {
            FilePath = Path.Combine(Path.GetTempPath(), $"{nameof(FileRepositoryTest)}.json");
        }

        private static readonly Guid Id1 = Guid.NewGuid();
        private static readonly Guid Id2 = Guid.NewGuid();
        private static readonly Guid Id3 = Guid.NewGuid();

        private ConcreteFileRepository unitUnderTest;
        private ILifecycleManager lifecycleManagement;

        [SetUp]
        public void Setup()
        {
            lifecycleManagement = Substitute.For<ILifecycleManager>();
            lifecycleManagement.Token.Returns(new CancellationToken(false));
            if (IoFile.Exists(FilePath))
            {
                IoFile.Delete(FilePath);
            }
            unitUnderTest = new ConcreteFileRepository(FilePath, lifecycleManagement);
        }

        [TearDown]
        public void Cleanup()
        {
            lifecycleManagement = null!;
            unitUnderTest = null!;
        }

        [Test]
        public void WriteReadRoundtripTest()
        {
            var t1 = new TestData
            {
                Data = "Hello"
            };

            unitUnderTest.AddOrUpdate(t1);

            TestData? result;
            bool success = unitUnderTest.TryGet(t1.Id, out result);

            Assert.That(success, Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(t1.Data, Is.EqualTo(result.Data));
        }

        [Test]
        public void DeleteTest()
        {
            var t1 = new TestData { Id = Id1, Data = "one" };
            var t2 = new TestData { Id = Id2, Data = "two" };
            var t3 = new TestData { Id = Id3, Data = "three" };

            unitUnderTest.AddOrUpdate(t1);
            unitUnderTest.AddOrUpdate(t2);
            unitUnderTest.AddOrUpdate(t3);
            Assert.That(unitUnderTest.All().Count, Is.EqualTo(3));

            unitUnderTest.Delete(Id2);

            Assert.That(unitUnderTest.All().Count, Is.EqualTo(2));
            Assert.That(unitUnderTest.TryGet(Id1, out _), Is.True);
            Assert.That(unitUnderTest.TryGet(Id2, out _), Is.False);
            Assert.That(unitUnderTest.TryGet(Id3, out _), Is.True);
        }
    }
}
