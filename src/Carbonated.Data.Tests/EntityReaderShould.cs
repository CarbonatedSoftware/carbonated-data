using System.Linq;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

[TestFixture]
public class EntityReaderShould
{
    [Test]
    public void PopulateInstancesWithTypeMapper()
    {
        var dataRecord = new MockDataRecord();
        var dataReader = new MockDataReader(dataRecord);

        var mapper = new TypeMapper<Entity>(r => new Entity() { Id = 3 });
        var reader = new EntityReader<Entity>(dataReader, mapper);

        var inst = reader.First();

        Assert.That(inst.Id, Is.EqualTo(3));
    }

    [Test]
    public void PopulateInstancesWithPropertyMapper()
    {
        var record1 = new MockDataRecord(("id", 1), ("n", "John Q"), ("t", "Tester"));
        var record2 = new MockDataRecord(("id", 2), ("n", "Jane R"), ("t", "Supervisor"));
        var dataReader = new MockDataReader(record1, record2);

        var mapper = PropMapper<Entity>()
            .Map(x => x.Name, "n")
            .Map(x => x.Title, "t");

        var reader = new EntityReader<Entity>(dataReader, mapper);
        var items = reader.Take(2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(items[0].Name, Is.EqualTo("John Q"));
            Assert.That(items[1].Name, Is.EqualTo("Jane R"));
        });
    }

    [Test]
    public void PopulateUsingValueConvertersWhenProvided()
    {
        var record = new MockDataRecord(("id", 42), ("agentnumber", 86));
        var dataReader = new MockDataReader(record);

        var mapperColl = new MapperCollection();
        mapperColl.AddValueConverter(x => new SemanticInt((int)x));

        var mapper = mapperColl.Get<EntityWithSemanticProperty>();

        var reader = new EntityReader<EntityWithSemanticProperty>(dataReader, mapper);
        var items = reader.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(items[0].Id, Is.EqualTo(42));
            Assert.That(items[0].AgentNumber.Value, Is.EqualTo(86));
        });
    }

    [Test]
    public void CloseUnderlyingDataReaderUponCompletionOfEnumeration()
    {
        var record1 = new MockDataRecord(("id", 1), ("n", "John Q"), ("t", "Tester"));
        var record2 = new MockDataRecord(("id", 2), ("n", "Jane R"), ("t", "Supervisor"));
        var dataReader = new MockDataReader(record1, record2);

        var mapper = PropMapper<Entity>()
            .Map(x => x.Name, "n")
            .Map(x => x.Title, "t");

        var reader = new EntityReader<Entity>(dataReader, mapper);

        Assert.That(dataReader.IsClosed, Is.False);

        var items = reader.ToList();

        Assert.That(dataReader.IsClosed, Is.True);
    }
}
