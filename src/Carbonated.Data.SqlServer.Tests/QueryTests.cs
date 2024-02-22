using System;
using System.Linq;
using Carbonated.Data.SqlServer.Tests.Models;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

[TestFixture]
public class QueryTests
{
    private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True;Trust Server Certificate=True";
    private DbConnector connector;

    [SetUp]
    public void SetUp()
    {
        connector = new SqlServerDbConnector(TestConnectionString);
    }

    [TearDown]
    public void TearDown()
    {
        connector.Dispose();
    }

    [Test]
    public void QueryBool()
    {
        var values = connector.Query<bool>("select [bool] from type_test order by id");

        Assert.That(values, Is.EqualTo(new bool[] { false, false, true }).AsCollection);
    }

    [Test]
    public void QueryByte()
    {
        var values = connector.Query<byte>("select [byte] from type_test order by id");

        Assert.That(values, Is.EqualTo(new byte[] { 0, 0, 1 }).AsCollection);
    }

    [Test]
    public void QueryShort()
    {
        var values = connector.Query<short>("select [short] from type_test order by id");

        Assert.That(values, Is.EqualTo(new short[] { 0, 0, 2 }).AsCollection);
    }

    [Test]
    public void QueryInt()
    {
        var values = connector.Query<int>("select [int] from type_test order by id");

        Assert.That(values, Is.EqualTo(new int[] { 0, 0, 3 }).AsCollection);
    }

    [Test]
    public void QueryLong()
    {
        var values = connector.Query<long>("select [long] from type_test order by id");

        Assert.That(values, Is.EqualTo(new long[] { 0, 0, 5 }).AsCollection);
    }

    [Test]
    public void QueryFloat()
    {
        var values = connector.Query<float>("select [float] from type_test order by id");

        Assert.That(values, Is.EqualTo(new float[] { 0.0f, 0.0f, 8.13f }).AsCollection);
    }

    [Test]
    public void QueryDouble()
    {
        var values = connector.Query<double>("select [double] from type_test order by id");

        Assert.That(values, Is.EqualTo(new double[] { 0.0, 0.0, 21.34 }).AsCollection);
    }

    [Test]
    public void QueryDecimal()
    {
        var values = connector.Query<decimal>("select [decimal] from type_test order by id");

        Assert.That(values, Is.EqualTo(new decimal[] { 0.0m, 0.0m, 55.89m }).AsCollection);
    }

    [Test]
    public void QueryDateTime()
    {
        var values = connector.Query<DateTime>("select [datetime] from type_test order by id");

        var expected = new DateTime[]
        {
            DateTime.MinValue,
            new(1753, 1, 1, 0, 0, 0),
            new(2018, 4, 2, 13, 14, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryDateTimeStoredAsDateTime2()
    {
        var values = connector.Query<DateTime>("select [datetime2] from type_test order by id");

        var expected = new DateTime[]
        {
            DateTime.MinValue,
            DateTime.MinValue,
            new(2023, 8, 11, 14, 9, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryDateOnly()
    {
        var values = connector.Query<DateOnly>("select [date] from type_test order by id");

        var expected = new DateOnly[]
        {
            DateOnly.MinValue,
            DateOnly.MinValue,
            new(2023, 8, 11)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryTimeOnly()
    {
        var values = connector.Query<TimeOnly>("select [time] from type_test order by id");

        var expected = new TimeOnly[]
        {
            TimeOnly.MinValue,
            TimeOnly.MinValue,
            new(14, 9, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryGuidStoredAsString()
    {
        var values = connector.Query<Guid>("select [guid_as_string] from type_test order by id");

        var expected = new Guid[]
        {
            Guid.Empty,
            Guid.Empty,
            new("7ca43d156e8749dfbaffdb241d0d494c")
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryGuidStoredAsUniqueIdentifier()
    {
        var values = connector.Query<Guid>("select [guid_as_uniqueid] from type_test order by id");

        var expected = new Guid[]
        {
            Guid.Empty,
            Guid.Empty,
            new("7ca43d156e8749dfbaffdb241d0d494c")
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryChar()
    {
        var values = connector.Query<char>("select [char] from type_test order by id");

        Assert.That(values, Is.EqualTo(new char[] { '\0', '\0', 'c' }).AsCollection);
    }

    [Test]
    public void QueryString()
    {
        var values = connector.Query<string>("select [string] from type_test order by id");

        Assert.That(values, Is.EqualTo(new string[] { null, string.Empty, "str" }).AsCollection);
    }

    [Test]
    public void QueryByteArray()
    {
        var values = connector.Query<byte[]>("select [byte_array] from type_test order by id");

        var expected = new byte[][]
        {
            null,
            null,
            new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryEntities()
    {
        var entities = connector.Query<TypeTest>("select * from type_test order by id").ToArray();

        var nulls = entities[0];
        var empties = entities[1];
        var values = entities[2];

        Assert.Multiple(() =>
        {
            Assert.That(nulls.Bool, Is.False);
            Assert.That(nulls.Byte, Is.EqualTo(0));
            Assert.That(nulls.Short, Is.EqualTo(0));
            Assert.That(nulls.Int, Is.EqualTo(0));
            Assert.That(nulls.Long, Is.EqualTo(0));
            Assert.That(nulls.Float, Is.EqualTo(0));
            Assert.That(nulls.Double, Is.EqualTo(0));
            Assert.That(nulls.Decimal, Is.EqualTo(0));
            Assert.That(nulls.DateTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(nulls.DateTime2, Is.EqualTo(DateTime.MinValue));
            Assert.That(nulls.Date, Is.EqualTo(DateOnly.MinValue));
            Assert.That(nulls.Time, Is.EqualTo(TimeOnly.MinValue));
            Assert.That(nulls.GuidAsString, Is.EqualTo(Guid.Empty));
            Assert.That(nulls.GuidAsUniqueId, Is.EqualTo(Guid.Empty));
            Assert.That(nulls.Char, Is.EqualTo('\0'));
            Assert.That(nulls.String, Is.Null);
            Assert.That(nulls.ByteArray, Is.Null);
            Assert.That(nulls.IntEnum, Is.EqualTo(Numbers.Zero));
            Assert.That(nulls.StringEnum, Is.EqualTo(Colors.Red));
        });

        Assert.Multiple(() =>
        {
            Assert.That(empties.Bool, Is.False);
            Assert.That(empties.Byte, Is.EqualTo(0));
            Assert.That(empties.Short, Is.EqualTo(0));
            Assert.That(empties.Int, Is.EqualTo(0));
            Assert.That(empties.Long, Is.EqualTo(0));
            Assert.That(empties.Float, Is.EqualTo(0));
            Assert.That(empties.Double, Is.EqualTo(0));
            Assert.That(empties.Decimal, Is.EqualTo(0));
            Assert.That(empties.DateTime, Is.EqualTo(new DateTime(1753, 1, 1, 0, 0, 0)));
            Assert.That(empties.DateTime2, Is.EqualTo(DateTime.MinValue));
            Assert.That(empties.Date, Is.EqualTo(DateOnly.MinValue));
            Assert.That(empties.Time, Is.EqualTo(TimeOnly.MinValue));
            Assert.That(empties.GuidAsString, Is.EqualTo(Guid.Empty));
            Assert.That(empties.GuidAsUniqueId, Is.EqualTo(Guid.Empty));
            Assert.That(empties.Char, Is.EqualTo('\0'));
            Assert.That(empties.String, Is.EqualTo(string.Empty));
            Assert.That(empties.ByteArray, Is.EqualTo(null).AsCollection);
            Assert.That(empties.IntEnum, Is.EqualTo(Numbers.Zero));
            Assert.That(empties.StringEnum, Is.EqualTo(Colors.Red));
        });

        Assert.Multiple(() =>
        {
            Assert.That(values.Bool, Is.True);
            Assert.That(values.Byte, Is.EqualTo(1));
            Assert.That(values.Short, Is.EqualTo(2));
            Assert.That(values.Int, Is.EqualTo(3));
            Assert.That(values.Long, Is.EqualTo(5));
            Assert.That(values.Float, Is.EqualTo(8.13f));
            Assert.That(values.Double, Is.EqualTo(21.34));
            Assert.That(values.Decimal, Is.EqualTo(55.89m));
            Assert.That(values.DateTime, Is.EqualTo(new DateTime(2018, 4, 2, 13, 14, 15)));
            Assert.That(values.DateTime2, Is.EqualTo(new DateTime(2023, 8, 11, 14, 09, 15)));
            Assert.That(values.Date, Is.EqualTo(new DateOnly(2023, 8, 11)));
            Assert.That(values.Time, Is.EqualTo(new TimeOnly(14, 9, 15)));
            Assert.That(values.GuidAsString, Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
            Assert.That(values.GuidAsUniqueId, Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
            Assert.That(values.Char, Is.EqualTo('c'));
            Assert.That(values.String, Is.EqualTo("str"));
            Assert.That(values.ByteArray, Is.EqualTo(new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }).AsCollection);
            Assert.That(values.IntEnum, Is.EqualTo(Numbers.Three));
            Assert.That(values.StringEnum, Is.EqualTo(Colors.Green));
        });
    }


    [Test]
    public void QueryNullableBool()
    {
        var values = connector.Query<bool?>("select [bool] from type_test order by id");

        Assert.That(values, Is.EqualTo(new bool?[] { null, false, true }).AsCollection);
    }

    [Test]
    public void QueryNullableByte()
    {
        var values = connector.Query<byte?>("select [byte] from type_test order by id");

        Assert.That(values, Is.EqualTo(new byte?[] { null, 0, 1 }).AsCollection);
    }

    [Test]
    public void QueryNullableShort()
    {
        var values = connector.Query<short?>("select [short] from type_test order by id");

        Assert.That(values, Is.EqualTo(new short?[] { null, 0, 2 }).AsCollection);
    }

    [Test]
    public void QueryNullableInt()
    {
        var values = connector.Query<int>("select [int] from type_test order by id");

        Assert.That(values, Is.EqualTo(new int[] { 0, 0, 3 }).AsCollection);
    }

    [Test]
    public void QueryNullableLong()
    {
        var values = connector.Query<long?>("select [long] from type_test order by id");

        Assert.That(values, Is.EqualTo(new long?[] { null, 0, 5 }).AsCollection);
    }

    [Test]
    public void QueryNullableFloat()
    {
        var values = connector.Query<float?>("select [float] from type_test order by id");

        Assert.That(values, Is.EqualTo(new float?[] { null, 0.0f, 8.13f }).AsCollection);
    }

    [Test]
    public void QueryNullableDouble()
    {
        var values = connector.Query<double?>("select [double] from type_test order by id");

        Assert.That(values, Is.EqualTo(new double?[] { null, 0.0, 21.34 }).AsCollection);
    }

    [Test]
    public void QueryNullableDecimal()
    {
        var values = connector.Query<decimal?>("select [decimal] from type_test order by id");

        Assert.That(values, Is.EqualTo(new decimal?[] { null, 0.0m, 55.89m }).AsCollection);
    }

    [Test]
    public void QueryNullableDateTime()
    {
        var values = connector.Query<DateTime?>("select [datetime] from type_test order by id");

        var expected = new DateTime?[]
        {
            null,
            new DateTime(1753, 1, 1, 0, 0, 0),
            new DateTime(2018, 4, 2, 13, 14, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableDateTimeStoredAsDateTime2()
    {
        var values = connector.Query<DateTime?>("select [datetime2] from type_test order by id");

        var expected = new DateTime?[]
        {
            null,
            DateTime.MinValue,
            new DateTime(2023, 8, 11, 14, 9, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableDateOnly()
    {
        var values = connector.Query<DateOnly?>("select [date] from type_test order by id");

        var expected = new DateOnly?[]
        {
            null,
            DateOnly.MinValue,
            new DateOnly(2023, 8, 11)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableTimeOnly()
    {
        var values = connector.Query<TimeOnly?>("select [time] from type_test order by id");

        var expected = new TimeOnly?[]
        {
            null,
            TimeOnly.MinValue,
            new TimeOnly(14, 9, 15)
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableGuidStoredAsString()
    {
        var values = connector.Query<Guid?>("select [guid_as_string] from type_test order by id");

        var expected = new Guid?[]
        {
            null,
            null,
            new Guid("7ca43d156e8749dfbaffdb241d0d494c")
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableGuidStoredAsUniqueIdentifier()
    {
        var values = connector.Query<Guid?>("select [guid_as_uniqueid] from type_test order by id");

        var expected = new Guid?[]
        {
            null,
            null,
            new Guid("7ca43d156e8749dfbaffdb241d0d494c")
        };
        Assert.That(values, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void QueryNullableChar()
    {
        var values = connector.Query<char?>("select [char] from type_test order by id");

        Assert.That(values, Is.EqualTo(new char?[] { null, null, 'c' }).AsCollection);
    }

    [Test]
    public void QueryNullableEntities()
    {
        var entities = connector.Query<NullableTypeTest>("select * from type_test order by id").ToArray();

        var nulls = entities[0];
        var empties = entities[1];
        var values = entities[2];

        Assert.Multiple(() =>
        {
            Assert.That(nulls.Bool, Is.Null);
            Assert.That(nulls.Byte, Is.Null);
            Assert.That(nulls.Short, Is.Null);
            Assert.That(nulls.Int, Is.Null);
            Assert.That(nulls.Long, Is.Null);
            Assert.That(nulls.Float, Is.Null);
            Assert.That(nulls.Double, Is.Null);
            Assert.That(nulls.Decimal, Is.Null);
            Assert.That(nulls.DateTime, Is.Null);
            Assert.That(nulls.DateTime2, Is.Null);
            Assert.That(nulls.Date, Is.Null);
            Assert.That(nulls.Time, Is.Null);
            Assert.That(nulls.GuidAsString, Is.Null);
            Assert.That(nulls.GuidAsUniqueId, Is.Null);
            Assert.That(nulls.Char, Is.Null);
            Assert.That(nulls.IntEnum, Is.Null);
            Assert.That(nulls.StringEnum, Is.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(empties.Bool, Is.False);
            Assert.That(empties.Byte, Is.EqualTo(0));
            Assert.That(empties.Short, Is.EqualTo(0));
            Assert.That(empties.Int, Is.EqualTo(0));
            Assert.That(empties.Long, Is.EqualTo(0));
            Assert.That(empties.Float, Is.EqualTo(0));
            Assert.That(empties.Double, Is.EqualTo(0));
            Assert.That(empties.Decimal, Is.EqualTo(0));
            Assert.That(empties.DateTime, Is.EqualTo(new DateTime(1753, 1, 1, 0, 0, 0)));
            Assert.That(empties.DateTime2, Is.EqualTo(DateTime.MinValue));
            Assert.That(empties.Date, Is.EqualTo(DateOnly.MinValue));
            Assert.That(empties.Time, Is.EqualTo(TimeOnly.MinValue));
            Assert.That(empties.GuidAsString, Is.Null);
            Assert.That(empties.GuidAsUniqueId, Is.Null);
            Assert.That(empties.Char, Is.Null);
            Assert.That(empties.IntEnum, Is.EqualTo(Numbers.Zero));
            Assert.That(empties.StringEnum, Is.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(values.Bool, Is.True);
            Assert.That(values.Byte, Is.EqualTo(1));
            Assert.That(values.Short, Is.EqualTo(2));
            Assert.That(values.Int, Is.EqualTo(3));
            Assert.That(values.Long, Is.EqualTo(5));
            Assert.That(values.Float, Is.EqualTo(8.13f));
            Assert.That(values.Double, Is.EqualTo(21.34));
            Assert.That(values.Decimal, Is.EqualTo(55.89m));
            Assert.That(values.DateTime, Is.EqualTo(new DateTime(2018, 4, 2, 13, 14, 15)));
            Assert.That(values.DateTime2, Is.EqualTo(new DateTime(2023, 8, 11, 14, 9, 15)));
            Assert.That(values.Date, Is.EqualTo(new DateOnly(2023, 8, 11)));
            Assert.That(values.Time, Is.EqualTo(new TimeOnly(14, 9, 15)));
            Assert.That(values.GuidAsString, Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
            Assert.That(values.GuidAsUniqueId, Is.EqualTo(new Guid("7ca43d156e8749dfbaffdb241d0d494c")));
            Assert.That(values.Char, Is.EqualTo('c'));
            Assert.That(values.IntEnum, Is.EqualTo(Numbers.Three));
            Assert.That(values.StringEnum, Is.EqualTo(Colors.Green));
        });
    }
}
