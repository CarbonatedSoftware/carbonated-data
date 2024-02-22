using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

public class PropertyMapperShould_WhenCreatingRecords
{
    [Test]
    public void CreateRecordsWithAllOptionalProperties()
    {
        var record = Record(("name", "Moe"), ("age", 31));
        var mapper = PropMapper<OptionalRecord>();

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Name, Is.EqualTo("Moe"));
            Assert.That(inst.Age, Is.EqualTo(31));
        });
    }

    [Test]
    public void CreateRecordsWithMixtureOfOptionalAndRequiredProperties()
    {
        var record = Record(("name", "Larry"), ("age", 34));
        var mapper = PropMapper<RequiredAndOptionalRecord>();

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Name, Is.EqualTo("Larry"));
            Assert.That(inst.Age, Is.EqualTo(34));
        });
    }

    [Test]
    public void CreateRecordWithAllRequiredProperties()
    {
        var record = Record(("name", "Curly"), ("age", 28));
        var mapper = PropMapper<RequiredRecord>();

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Name, Is.EqualTo("Curly"));
            Assert.That(inst.Age, Is.EqualTo(28));
        });
    }

    record RequiredRecord(string Name, int Age);

    record RequiredAndOptionalRecord(string Name)
    {
        public int Age { get; init; }
    }

    record OptionalRecord
    {
        public string Name { get; init; }
        public int Age { get; init; }
    }
}
