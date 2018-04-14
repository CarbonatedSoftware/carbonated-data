using System.Linq;
using NUnit.Framework;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class EntityReaderShould
    {
        [Test]
        public void PopulateInstancesWithTypeMapper()
        {
            var dataRecord = new MockDataRecord();
            var dataReader = new MockDataReader(dataRecord);

            var mapper = new TypeMapper<Entity>() { Creator = r => new Entity() { Id = 3 } };
            var reader = new EntityReader<Entity>(dataReader, mapper);

            var inst = reader.First();

            Assert.AreEqual(3, inst.Id);
        }

        [Test]
        public void PopulateInstancesWithPropertyMapper()
        {
            var record1 = new MockDataRecord(("id", 1), ("n", "John Q"), ("t", "Tester"));
            var record2 = new MockDataRecord(("id", 2), ("n", "Jane R"), ("t", "Supervisor"));
            var dataReader = new MockDataReader(record1, record2);

            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Name, "n")
                .Map(x => x.Title, "t");

            var reader = new EntityReader<Entity>(dataReader, mapper);
            var items = reader.Take(2).ToList();

            Assert.AreEqual("John Q", items[0].Name);
            Assert.AreEqual("Jane R", items[1].Name);
        }

        [Test]
        public void CloseUnderlyingDataReaderUponCompletionOfEnumeration()
        {
            var record1 = new MockDataRecord(("id", 1), ("n", "John Q"), ("t", "Tester"));
            var record2 = new MockDataRecord(("id", 2), ("n", "Jane R"), ("t", "Supervisor"));
            var dataReader = new MockDataReader(record1, record2);

            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Name, "n")
                .Map(x => x.Title, "t");

            var reader = new EntityReader<Entity>(dataReader, mapper);

            Assert.IsFalse(dataReader.IsClosed);

            var items = reader.ToList();

            Assert.IsTrue(dataReader.IsClosed);
        }
    }
}
