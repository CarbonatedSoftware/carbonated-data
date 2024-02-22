using System;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

[TestFixture]
public class RecordShould
{
    [Test]
    public void KnowWhichFieldsItContains()
    {
        var record = Record(("Foo", 1));

        Assert.Multiple(() =>
        {
            Assert.That(record.HasField("Foo"), Is.True);
            Assert.That(record.HasField("Oof"), Is.False);
        });
    }

    [Test]
    public void HaveCaseInsensitiveFieldNames()
    {
        var record = Record(("Foo", 1));

        Assert.That(record.HasField("foo"), Is.True);
    }

    [Test]
    public void MapFieldNamesInBothExplicitAndNormalizedForm()
    {
        var record = Record(("Fo_o", 1));

        Assert.Multiple(() =>
        {
            Assert.That(record.HasField("Fo_o"), Is.True);
            Assert.That(record.HasField("foo"), Is.True);
        });
    }

    [Test]
    public void NotNormalizeFieldNamesWhenExplicitNamesWouldCollide()
    {
        var record = Record(("foo", 1), ("f_oo", 2));

        Assert.Multiple(() =>
        {
            Assert.That(record.GetValue("foo"), Is.EqualTo(1));
            Assert.That(record.GetValue("f_oo"), Is.EqualTo(2));
        });
    }

    [Test]
    public void NotNormalizeFieldNamesWhenNormalizedNamesWouldCollide()
    {
        var record = Record(("f_oo", 1), ("fo_o", 2));

        Assert.Multiple(() =>
        {
            Assert.That(record.HasField("foo"), Is.False);
            Assert.That(record.GetValue("f_oo"), Is.EqualTo(1));
            Assert.That(record.GetValue("fo_o"), Is.EqualTo(2));
        });
    }

    [Test]
    public void GetValueWhenNonNormalizedNameIsPassed()
    {
        var record = Record(("foo", 1));

        Assert.That(record.GetValue("f_oo"), Is.EqualTo(1));
    }

    [Test]
    public void ReturnNullFromGetValueWhenFieldIsNotFound()
    {
        var record = Record(("Foo", 1));

        Assert.That(record.GetValue("bar"), Is.Null);
    }

    [Test]
    public void ReturnIndexOfFieldFromGetIndex()
    {
        var record = Record(("Foo", 1));

        Assert.Multiple(() =>
        {
            Assert.That(record.GetIndex("foo"), Is.EqualTo(0));
            Assert.That(record.GetIndex("bar"), Is.EqualTo(-1));
        });
    }

    [Test]
    public void GetTypesByName()
    {
        var record = Record(("bool", true), ("byte", (byte)1), ("char", 'a'));

        Assert.Multiple(() =>
        {
            Assert.That(record.GetBoolean("bool"), Is.EqualTo(true));
            Assert.That(record.GetByte("byte"), Is.EqualTo(1));
            Assert.That(record.GetChar("char"), Is.EqualTo('a'));
        });
    }

    [Test]
    public void GetTypeOrDefaultByName()
    {
        var record = Record(("bool1", true), ("bool2", DBNull.Value), ("dec1", 42m), ("dec2", DBNull.Value));

        Assert.Multiple(() =>
        {
            Assert.That(record.GetBooleanOrDefault("bool1"), Is.True);
            Assert.That(record.GetBooleanOrDefault("bool2"), Is.False);
            Assert.That(record.GetBooleanOrDefault("missing"), Is.False);
        });

        Assert.Multiple(() =>
        {
            Assert.That(record.GetDecimalOrDefault("dec1"), Is.EqualTo(42));
            Assert.That(record.GetDecimalOrDefault("dec2"), Is.EqualTo(0));
            Assert.That(record.GetDecimalOrDefault("missing"), Is.EqualTo(0));
        });
    }

    [Test]
    public void GetTypeOrFallbackByName()
    {
        var record = Record(("int1", 42), ("int2", DBNull.Value));

        Assert.Multiple(() =>
        {
            Assert.That(record.GetInt32OrDefault("int1"), Is.EqualTo(42));

            Assert.That(record.GetInt32OrDefault("int2"), Is.EqualTo(0));
            Assert.That(record.GetInt32OrDefault("int2", 76), Is.EqualTo(76));

            Assert.That(record.GetInt32OrDefault("missing"), Is.EqualTo(0));
            Assert.That(record.GetInt32OrDefault("missing", 99), Is.EqualTo(99));
        });
    }
}
