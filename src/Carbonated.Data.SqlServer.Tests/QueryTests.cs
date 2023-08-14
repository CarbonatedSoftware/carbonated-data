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

    [Test]
    public void QueryBool()
    {
        var values = connector.Query<bool>("select [bool] from type_test order by id");

        CollectionAssert.AreEqual(new bool[] { false, false, true }, values);
    }

    [Test]
    public void QueryByte()
    {
        var values = connector.Query<byte>("select [byte] from type_test order by id");

        CollectionAssert.AreEqual(new byte[] { 0, 0, 1 }, values);
    }

    [Test]
    public void QueryShort()
    {
        var values = connector.Query<short>("select [short] from type_test order by id");

        CollectionAssert.AreEqual(new short[] { 0, 0, 2 }, values);
    }

    [Test]
    public void QueryInt()
    {
        var values = connector.Query<int>("select [int] from type_test order by id");

        CollectionAssert.AreEqual(new int[] { 0, 0, 3 }, values);
    }

    [Test]
    public void QueryLong()
    {
        var values = connector.Query<long>("select [long] from type_test order by id");

        CollectionAssert.AreEqual(new long[] { 0, 0, 5 }, values);
    }

    [Test]
    public void QueryFloat()
    {
        var values = connector.Query<float>("select [float] from type_test order by id");

        CollectionAssert.AreEqual(new float[] { 0.0f, 0.0f, 8.13f }, values);
    }

    [Test]
    public void QueryDouble()
    {
        var values = connector.Query<double>("select [double] from type_test order by id");

        CollectionAssert.AreEqual(new double[] { 0.0, 0.0, 21.34 }, values);
    }

    [Test]
    public void QueryDecimal()
    {
        var values = connector.Query<decimal>("select [decimal] from type_test order by id");

        CollectionAssert.AreEqual(new decimal[] { 0.0m, 0.0m, 55.89m }, values);
    }

    [Test]
    public void QueryDateTime()
    {
        var values = connector.Query<DateTime>("select [datetime] from type_test order by id");

        var expected = new DateTime[]
        {
            DateTime.MinValue,
            new DateTime(1753, 1, 1, 0, 0, 0),
            new DateTime(2018, 4, 2, 13, 14, 15)
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryDateTimeStoredAsDateTime2()
    {
        var values = connector.Query<DateTime>("select [datetime2] from type_test order by id");

        var expected = new DateTime[]
        {
            DateTime.MinValue,
            DateTime.MinValue,
            new DateTime(2023, 8, 11, 14, 9, 15)
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryDateOnly()
    {
        var values = connector.Query<DateOnly>("select [date] from type_test order by id");

        var expected = new DateOnly[]
        {
            DateOnly.MinValue,
            DateOnly.MinValue,
            new DateOnly(2023, 8, 11)
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryTimeOnly()
    {
        var values = connector.Query<TimeOnly>("select [time] from type_test order by id");

        var expected = new TimeOnly[]
        {
            TimeOnly.MinValue,
            TimeOnly.MinValue,
            new TimeOnly(14, 9, 15)
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryGuidStoredAsString()
    {
        var values = connector.Query<Guid>("select [guid_as_string] from type_test order by id");

        var expected = new Guid[]
        {
            Guid.Empty,
            Guid.Empty,
            new Guid("7ca43d156e8749dfbaffdb241d0d494c")
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryGuidStoredAsUniqueIdentifier()
    {
        var values = connector.Query<Guid>("select [guid_as_uniqueid] from type_test order by id");

        var expected = new Guid[]
        {
            Guid.Empty,
            Guid.Empty,
            new Guid("7ca43d156e8749dfbaffdb241d0d494c")
        };
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryChar()
    {
        var values = connector.Query<char>("select [char] from type_test order by id");

        CollectionAssert.AreEqual(new char[] { '\0', '\0', 'c' }, values);
    }

    [Test]
    public void QueryString()
    {
        var values = connector.Query<string>("select [string] from type_test order by id");

        CollectionAssert.AreEqual(new string[] { null, string.Empty, "str" }, values);
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
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryEntities()
    {
        var entities = connector.Query<TypeTest>("select * from type_test order by id").ToArray();

        var nulls = entities[0];
        var empties = entities[1];
        var values = entities[2];

        Assert.IsFalse(nulls.Bool);
        Assert.AreEqual(0, nulls.Byte);
        Assert.AreEqual(0, nulls.Short);
        Assert.AreEqual(0, nulls.Int);
        Assert.AreEqual(0, nulls.Long);
        Assert.AreEqual(0, nulls.Float);
        Assert.AreEqual(0, nulls.Double);
        Assert.AreEqual(0, nulls.Decimal);
        Assert.AreEqual(DateTime.MinValue, nulls.DateTime);
        Assert.AreEqual(DateTime.MinValue, nulls.DateTime2);
        Assert.AreEqual(DateOnly.MinValue, nulls.Date);
        Assert.AreEqual(TimeOnly.MinValue, nulls.Time);
        Assert.AreEqual(Guid.Empty, nulls.GuidAsString);
        Assert.AreEqual(Guid.Empty, nulls.GuidAsUniqueId);
        Assert.AreEqual('\0', nulls.Char);
        Assert.IsNull(nulls.String);
        Assert.IsNull(nulls.ByteArray);
        Assert.AreEqual(Numbers.Zero, nulls.IntEnum);
        Assert.AreEqual(Colors.Red, nulls.StringEnum);

        Assert.IsFalse(empties.Bool);
        Assert.AreEqual(0, empties.Byte);
        Assert.AreEqual(0, empties.Short);
        Assert.AreEqual(0, empties.Int);
        Assert.AreEqual(0, empties.Long);
        Assert.AreEqual(0, empties.Float);
        Assert.AreEqual(0, empties.Double);
        Assert.AreEqual(0, empties.Decimal);
        Assert.AreEqual(new DateTime(1753, 1, 1, 0, 0, 0), empties.DateTime);
        Assert.AreEqual(DateTime.MinValue, empties.DateTime2);
        Assert.AreEqual(DateOnly.MinValue, empties.Date);
        Assert.AreEqual(TimeOnly.MinValue, empties.Time);
        Assert.AreEqual(Guid.Empty, empties.GuidAsString);
        Assert.AreEqual(Guid.Empty, empties.GuidAsUniqueId);
        Assert.AreEqual('\0', empties.Char);
        Assert.AreEqual(string.Empty, empties.String);
        CollectionAssert.AreEqual(null, empties.ByteArray);
        Assert.AreEqual(Numbers.Zero, empties.IntEnum);
        Assert.AreEqual(Colors.Red, empties.StringEnum);

        Assert.IsTrue(values.Bool);
        Assert.AreEqual(1, values.Byte);
        Assert.AreEqual(2, values.Short);
        Assert.AreEqual(3, values.Int);
        Assert.AreEqual(5, values.Long);
        Assert.AreEqual(8.13f, values.Float);
        Assert.AreEqual(21.34, values.Double);
        Assert.AreEqual(55.89m, values.Decimal);
        Assert.AreEqual(new DateTime(2018, 4, 2, 13, 14, 15), values.DateTime);
        Assert.AreEqual(new DateTime(2023, 8, 11, 14, 09, 15), values.DateTime2);
        Assert.AreEqual(new DateOnly(2023, 8, 11), values.Date);
        Assert.AreEqual(new TimeOnly(14, 9, 15), values.Time);
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), values.GuidAsString);
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), values.GuidAsUniqueId);
        Assert.AreEqual('c', values.Char);
        Assert.AreEqual("str", values.String);
        CollectionAssert.AreEqual(new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }, values.ByteArray);
        Assert.AreEqual(Numbers.Three, values.IntEnum);
        Assert.AreEqual(Colors.Green, values.StringEnum);
    }


    [Test]
    public void QueryNullableBool()
    {
        var values = connector.Query<bool?>("select [bool] from type_test order by id");

        CollectionAssert.AreEqual(new bool?[] { null, false, true }, values);
    }

    [Test]
    public void QueryNullableByte()
    {
        var values = connector.Query<byte?>("select [byte] from type_test order by id");

        CollectionAssert.AreEqual(new byte?[] { null, 0, 1 }, values);
    }

    [Test]
    public void QueryNullableShort()
    {
        var values = connector.Query<short?>("select [short] from type_test order by id");

        CollectionAssert.AreEqual(new short?[] { null, 0, 2 }, values);
    }

    [Test]
    public void QueryNullableInt()
    {
        var values = connector.Query<int>("select [int] from type_test order by id");

        CollectionAssert.AreEqual(new int[] { 0, 0, 3 }, values);
    }

    [Test]
    public void QueryNullableLong()
    {
        var values = connector.Query<long?>("select [long] from type_test order by id");

        CollectionAssert.AreEqual(new long?[] { null, 0, 5 }, values);
    }

    [Test]
    public void QueryNullableFloat()
    {
        var values = connector.Query<float?>("select [float] from type_test order by id");

        CollectionAssert.AreEqual(new float?[] { null, 0.0f, 8.13f }, values);
    }

    [Test]
    public void QueryNullableDouble()
    {
        var values = connector.Query<double?>("select [double] from type_test order by id");

        CollectionAssert.AreEqual(new double?[] { null, 0.0, 21.34 }, values);
    }

    [Test]
    public void QueryNullableDecimal()
    {
        var values = connector.Query<decimal?>("select [decimal] from type_test order by id");

        CollectionAssert.AreEqual(new decimal?[] { null, 0.0m, 55.89m }, values);
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
        CollectionAssert.AreEqual(expected, values);
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
        CollectionAssert.AreEqual(expected, values);
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
        CollectionAssert.AreEqual(expected, values);
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
        CollectionAssert.AreEqual(expected, values);
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
        CollectionAssert.AreEqual(expected, values);
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
        CollectionAssert.AreEqual(expected, values);
    }

    [Test]
    public void QueryNullableChar()
    {
        var values = connector.Query<char?>("select [char] from type_test order by id");

        CollectionAssert.AreEqual(new char?[] { null, null, 'c' }, values);
    }

    [Test]
    public void QueryNullableEntities()
    {
        var entities = connector.Query<NullableTypeTest>("select * from type_test order by id").ToArray();

        var nulls = entities[0];
        var empties = entities[1];
        var values = entities[2];

        Assert.IsNull(nulls.Bool);
        Assert.IsNull(nulls.Byte);
        Assert.IsNull(nulls.Short);
        Assert.IsNull(nulls.Int);
        Assert.IsNull(nulls.Long);
        Assert.IsNull(nulls.Float);
        Assert.IsNull(nulls.Double);
        Assert.IsNull(nulls.Decimal);
        Assert.IsNull(nulls.DateTime);
        Assert.IsNull(nulls.DateTime2);
        Assert.IsNull(nulls.Date);
        Assert.IsNull(nulls.Time);
        Assert.IsNull(nulls.GuidAsString);
        Assert.IsNull(nulls.GuidAsUniqueId);
        Assert.IsNull(nulls.Char);
        Assert.IsNull(nulls.IntEnum);
        Assert.IsNull(nulls.StringEnum);

        Assert.IsFalse(empties.Bool);
        Assert.AreEqual(0, empties.Byte);
        Assert.AreEqual(0, empties.Short);
        Assert.AreEqual(0, empties.Int);
        Assert.AreEqual(0, empties.Long);
        Assert.AreEqual(0, empties.Float);
        Assert.AreEqual(0, empties.Double);
        Assert.AreEqual(0, empties.Decimal);
        Assert.AreEqual(new DateTime(1753, 1, 1, 0, 0, 0), empties.DateTime);
        Assert.AreEqual(DateTime.MinValue, empties.DateTime2);
        Assert.AreEqual(DateOnly.MinValue, empties.Date);
        Assert.AreEqual(TimeOnly.MinValue, empties.Time);
        Assert.IsNull(empties.GuidAsString);
        Assert.IsNull(empties.GuidAsUniqueId);
        Assert.IsNull(empties.Char);
        Assert.AreEqual(Numbers.Zero, empties.IntEnum);
        Assert.IsNull(empties.StringEnum);

        Assert.IsTrue(values.Bool);
        Assert.AreEqual(1, values.Byte);
        Assert.AreEqual(2, values.Short);
        Assert.AreEqual(3, values.Int);
        Assert.AreEqual(5, values.Long);
        Assert.AreEqual(8.13f, values.Float);
        Assert.AreEqual(21.34, values.Double);
        Assert.AreEqual(55.89m, values.Decimal);
        Assert.AreEqual(new DateTime(2018, 4, 2, 13, 14, 15), values.DateTime);
        Assert.AreEqual(new DateTime(2023, 8, 11, 14, 9, 15), values.DateTime2);
        Assert.AreEqual(new DateOnly(2023, 8, 11), values.Date);
        Assert.AreEqual(new TimeOnly(14, 9, 15), values.Time);
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), values.GuidAsString);
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), values.GuidAsUniqueId);
        Assert.AreEqual('c', values.Char);
        Assert.AreEqual(Numbers.Three, values.IntEnum);
        Assert.AreEqual(Colors.Green, values.StringEnum);
    }
}
