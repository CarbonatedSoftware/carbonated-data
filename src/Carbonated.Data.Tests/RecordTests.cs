using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

public class RecordTests
{
    [Test]
    public void ReadOptionalRecords()
    {
        var record = Record(("name", "Moe"), ("age", 31));
        var mapper = PropMapper<OptionalRecord>();

        var inst = mapper.CreateInstance2(record);

        Assert.That(inst.Name, Is.EqualTo("Moe"));
        Assert.That(inst.Age, Is.EqualTo(31));
    }

    [Test]
    public void ReadMixedRecords()
    {
        var record = Record(("name", "Larry"), ("age", 34));
        var mapper = PropMapper<RequiredAndOptionalRecord>();

        var inst = mapper.CreateInstance2(record);

        Assert.That(inst.Name, Is.EqualTo("Larry"));
        Assert.That(inst.Age, Is.EqualTo(34));
    }

    [Test]
    public void ReadRequiredRecords()
    {
        var record = Record(("name", "Curly"), ("age", 28));
        var mapper = PropMapper<RequiredRecord>();

        var inst = mapper.CreateInstance2(record);

        Assert.That(inst.Name, Is.EqualTo("Curly"));
        Assert.That(inst.Age, Is.EqualTo(28));
    }

}
