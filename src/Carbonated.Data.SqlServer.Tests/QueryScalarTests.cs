using System;
using Carbonated.Data.SqlServer.Tests.Models;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

[TestFixture]
public class QueryScalarTests
{
    private DbConnector db;

    [SetUp]
    public void SetUp()
    {
        db = new SqlServerDbConnector(IntegrationTestContext.TestConnectionString);
    }

    [TearDown]
    public void TearDown()
    {
        db.Dispose();
    }


    private string Select(string field, int id) => $"select [{field}] from type_test where id = {id}";

    [Test]
    public void QueryScalarBool()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<bool>(Select("bool", 1)), Is.False);
            Assert.That(db.QueryScalar<bool>(Select("bool", 2)), Is.False);
            Assert.That(db.QueryScalar<bool>(Select("bool", 3)), Is.True);
        });
    }

    [Test]
    public void QueryScalarByte()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<byte>(Select("byte", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<byte>(Select("byte", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<byte>(Select("byte", 3)), Is.EqualTo(1));
        });
    }

    [Test]
    public void QueryScalarShort()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<short>(Select("short", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<short>(Select("short", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<short>(Select("short", 3)), Is.EqualTo(2));
        });
    }

    [Test]
    public void QueryScalarInt()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<int>(Select("int", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<int>(Select("int", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<int>(Select("int", 3)), Is.EqualTo(3));
        });
    }

    [Test]
    public void QueryScalarLong()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<long>(Select("long", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<long>(Select("long", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<long>(Select("long", 3)), Is.EqualTo(5));
        });
    }

    [Test]
    public void QueryScalarFloat()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<float>(Select("float", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<float>(Select("float", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<float>(Select("float", 3)), Is.EqualTo(8.13f));
        });
    }

    [Test]
    public void QueryScalarDouble()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<double>(Select("double", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<double>(Select("double", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<double>(Select("double", 3)), Is.EqualTo(21.34));
        });
    }

    [Test]
    public void QueryScalarDecimal()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<decimal>(Select("decimal", 1)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<decimal>(Select("decimal", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<decimal>(Select("decimal", 3)), Is.EqualTo(55.89m));
        });
    }

    [Test]
    public void QueryScalarDateTime()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime", 1)), Is.EqualTo(DateTime.MinValue));
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime", 2)), Is.EqualTo(new DateTime(1753, 1, 1, 0, 0, 0)));
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime", 3)), Is.EqualTo(new DateTime(2018, 4, 2, 13, 14, 15)));
        });
    }

    [Test]
    public void QueryScalarDateTimeStoredAsDateTime2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime2", 1)), Is.EqualTo(DateTime.MinValue));
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime2", 2)), Is.EqualTo(DateTime.MinValue));
            Assert.That(db.QueryScalar<DateTime>(Select("DateTime2", 3)), Is.EqualTo(new DateTime(2023, 8, 11, 14, 9, 15)));
        });
    }

    [Test]
    public void QueryScalarDateOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateOnly>(Select("Date", 1)), Is.EqualTo(DateOnly.MinValue));
            Assert.That(db.QueryScalar<DateOnly>(Select("Date", 2)), Is.EqualTo(DateOnly.MinValue));
            Assert.That(db.QueryScalar<DateOnly>(Select("Date", 3)), Is.EqualTo(new DateOnly(2023, 8, 11)));
        });
    }

    [Test]
    public void QueryScalarTimeOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<TimeOnly>(Select("Time", 1)), Is.EqualTo(TimeOnly.MinValue));
            Assert.That(db.QueryScalar<TimeOnly>(Select("Time", 2)), Is.EqualTo(TimeOnly.MinValue));
            Assert.That(db.QueryScalar<TimeOnly>(Select("Time", 3)), Is.EqualTo(new TimeOnly(14, 9, 15)));
        });
    }

    [Test]
    public void QueryScalarGuidStoredAsString()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_string", 1)), Is.EqualTo(Guid.Empty));
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_string", 2)), Is.EqualTo(Guid.Empty));
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_string", 3)), Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
        });
    }

    [Test]
    public void QueryScalarGuidStoredAsUniqueIdentifier()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_uniqueid", 1)), Is.EqualTo(Guid.Empty));
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_uniqueid", 2)), Is.EqualTo(Guid.Empty));
            Assert.That(db.QueryScalar<Guid>(Select("guid_as_uniqueid", 3)), Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
        });
    }

    [Test]
    public void QueryScalarChar()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<char>(Select("char", 1)), Is.EqualTo('\0'));
            Assert.That(db.QueryScalar<char>(Select("char", 2)), Is.EqualTo('\0'));
            Assert.That(db.QueryScalar<char>(Select("char", 3)), Is.EqualTo('c'));
        });
    }

    [Test]
    public void QueryScalarString()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<string>(Select("string", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<string>(Select("string", 2)), Is.EqualTo(""));
            Assert.That(db.QueryScalar<string>(Select("string", 3)), Is.EqualTo("str"));
        });
    }

    [Test]
    public void QueryScalarByteArray()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<byte[]>(Select("byte_array", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<byte[]>(Select("byte_array", 2)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<byte[]>(Select("byte_array", 3)), Is.EqualTo(new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }));
        });
    }

    [Test]
    public void QueryScalarIntEnum()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Numbers>(Select("int_enum", 1)), Is.EqualTo(Numbers.Zero));
            Assert.That(db.QueryScalar<Numbers>(Select("int_enum", 2)), Is.EqualTo(Numbers.Zero));
            Assert.That(db.QueryScalar<Numbers>(Select("int_enum", 3)), Is.EqualTo(Numbers.Three));
        });
    }

    [Test]
    public void QueryScalarStringEnum()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Colors>(Select("string_enum", 1)), Is.EqualTo(Colors.Red));
            Assert.That(db.QueryScalar<Colors>(Select("string_enum", 2)), Is.EqualTo(Colors.Red));
            Assert.That(db.QueryScalar<Colors>(Select("string_enum", 3)), Is.EqualTo(Colors.Green));
        });
    }

    [Test]
    public void QueryScalarNullableBool()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<bool?>(Select("bool", 1)), Is.Null);
            Assert.That(db.QueryScalar<bool?>(Select("bool", 2)), Is.False);
            Assert.That(db.QueryScalar<bool?>(Select("bool", 3)), Is.True);
        });
    }

    [Test]
    public void QueryScalarNullableByte()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<byte?>(Select("byte", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<byte?>(Select("byte", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<byte?>(Select("byte", 3)), Is.EqualTo(1));
        });
    }

    [Test]
    public void QueryScalarNullableShort()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<short?>(Select("short", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<short?>(Select("short", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<short?>(Select("short", 3)), Is.EqualTo(2));
        });
    }

    [Test]
    public void QueryScalarNullableInt()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<int?>(Select("int", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<int?>(Select("int", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<int?>(Select("int", 3)), Is.EqualTo(3));
        });
    }

    [Test]
    public void QueryScalarNullableLong()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<long?>(Select("long", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<long?>(Select("long", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<long?>(Select("long", 3)), Is.EqualTo(5));
        });
    }

    [Test]
    public void QueryScalarNullableFloat()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<float?>(Select("float", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<float?>(Select("float", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<float?>(Select("float", 3)), Is.EqualTo(8.13f));
        });
    }

    [Test]
    public void QueryScalarNullableDouble()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<double?>(Select("double", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<double?>(Select("double", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<double?>(Select("double", 3)), Is.EqualTo(21.34));
        });
    }

    [Test]
    public void QueryScalarNullableDecimal()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<decimal?>(Select("decimal", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<decimal?>(Select("decimal", 2)), Is.EqualTo(0));
            Assert.That(db.QueryScalar<decimal?>(Select("decimal", 3)), Is.EqualTo(55.89m));
        });
    }

    [Test]
    public void QueryScalarNullableDateTime()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime", 2)), Is.EqualTo(new DateTime(1753, 1, 1, 0, 0, 0)));
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime", 3)), Is.EqualTo(new DateTime(2018, 4, 2, 13, 14, 15)));
        });
    }

    [Test]
    public void QueryScalarNullableDateTimeStoredAsDateTime2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime2", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime2", 2)), Is.EqualTo(DateTime.MinValue));
            Assert.That(db.QueryScalar<DateTime?>(Select("DateTime2", 3)), Is.EqualTo(new DateTime(2023, 8, 11, 14, 9, 15)));
        });
    }

    [Test]
    public void QueryScalarNullableDateOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<DateOnly?>(Select("Date", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<DateOnly?>(Select("Date", 2)), Is.EqualTo(DateOnly.MinValue));
            Assert.That(db.QueryScalar<DateOnly?>(Select("Date", 3)), Is.EqualTo(new DateOnly(2023, 8, 11)));
        });
    }

    [Test]
    public void QueryScalarNullableTimeOnly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<TimeOnly?>(Select("Time", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<TimeOnly?>(Select("Time", 2)), Is.EqualTo(TimeOnly.MinValue));
            Assert.That(db.QueryScalar<TimeOnly?>(Select("Time", 3)), Is.EqualTo(new TimeOnly(14, 9, 15)));
        });
    }

    [Test]
    public void QueryScalarNullableGuidStoredAsString()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_string", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_string", 2)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_string", 3)), Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
        });
    }

    [Test]
    public void QueryScalarNullableGuidStoredAsUniqueIdentifier()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 2)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 3)), Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
        });
    }

    [Test]
    public void QueryScalarNullableChar()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<char?>(Select("char", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<char?>(Select("char", 2)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<char?>(Select("char", 3)), Is.EqualTo('c'));
        });
    }

    [Test]
    public void QueryScalarNullableIntEnum()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Numbers?>(Select("int_enum", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Numbers?>(Select("int_enum", 2)), Is.EqualTo(Numbers.Zero));
            Assert.That(db.QueryScalar<Numbers?>(Select("int_enum", 3)), Is.EqualTo(Numbers.Three));
        });
    }

    [Test]
    public void QueryScalarNullableStringEnum()
    {
        Assert.Multiple(() =>
        {
            Assert.That(db.QueryScalar<Colors?>(Select("string_enum", 1)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Colors?>(Select("string_enum", 2)), Is.EqualTo(null));
            Assert.That(db.QueryScalar<Colors?>(Select("string_enum", 3)), Is.EqualTo(Colors.Green));
        });
    }

    [Test]
    public void QueryScalarUsingCustomValueConverter()
    {
        db.Mappers.AddValueConverter(x => new SemanticInt((int)x));

        var result = db.QueryScalar<SemanticInt>(Select("int", 3));

        Assert.That(result.Value, Is.EqualTo(3));
    }

    class SemanticInt
    {
        public SemanticInt(int value) { Value = value; }
        public int Value { get; }
    }
}
