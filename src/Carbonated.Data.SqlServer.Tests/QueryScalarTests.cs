using System;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

[TestFixture]
public class QueryScalarTests
{
    private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True;Trust Server Certificate=True";
    private DbConnector db;

    [SetUp]
    public void SetUp()
    {
        db = new SqlServerDbConnector(TestConnectionString);
    }

    private string Select(string field, int id) => $"select [{field}] from type_test where id = {id}";

    [Test]
    public void QueryScalarBool()
    {
        Assert.IsFalse(db.QueryScalar<bool>(Select("bool", 1)));
        Assert.IsFalse(db.QueryScalar<bool>(Select("bool", 2)));
        Assert.IsTrue(db.QueryScalar<bool>(Select("bool", 3)));
    }

    [Test]
    public void QueryScalarByte()
    {
        Assert.AreEqual(0, db.QueryScalar<byte>(Select("byte", 1)));
        Assert.AreEqual(0, db.QueryScalar<byte>(Select("byte", 2)));
        Assert.AreEqual(1, db.QueryScalar<byte>(Select("byte", 3)));
    }

    [Test]
    public void QueryScalarShort()
    {
        Assert.AreEqual(0, db.QueryScalar<short>(Select("short", 1)));
        Assert.AreEqual(0, db.QueryScalar<short>(Select("short", 2)));
        Assert.AreEqual(2, db.QueryScalar<short>(Select("short", 3)));
    }

    [Test]
    public void QueryScalarInt()
    {
        Assert.AreEqual(0, db.QueryScalar<int>(Select("int", 1)));
        Assert.AreEqual(0, db.QueryScalar<int>(Select("int", 2)));
        Assert.AreEqual(3, db.QueryScalar<int>(Select("int", 3)));
    }

    [Test]
    public void QueryScalarLong()
    {
        Assert.AreEqual(0, db.QueryScalar<long>(Select("long", 1)));
        Assert.AreEqual(0, db.QueryScalar<long>(Select("long", 2)));
        Assert.AreEqual(5, db.QueryScalar<long>(Select("long", 3)));
    }

    [Test]
    public void QueryScalarFloat()
    {
        Assert.AreEqual(0, db.QueryScalar<float>(Select("float", 1)));
        Assert.AreEqual(0, db.QueryScalar<float>(Select("float", 2)));
        Assert.AreEqual(8.13f, db.QueryScalar<float>(Select("float", 3)));
    }

    [Test]
    public void QueryScalarDouble()
    {
        Assert.AreEqual(0, db.QueryScalar<double>(Select("double", 1)));
        Assert.AreEqual(0, db.QueryScalar<double>(Select("double", 2)));
        Assert.AreEqual(21.34, db.QueryScalar<double>(Select("double", 3)));
    }

    [Test]
    public void QueryScalarDecimal()
    {
        Assert.AreEqual(0, db.QueryScalar<decimal>(Select("decimal", 1)));
        Assert.AreEqual(0, db.QueryScalar<decimal>(Select("decimal", 2)));
        Assert.AreEqual(55.89m, db.QueryScalar<decimal>(Select("decimal", 3)));
    }

    [Test]
    public void QueryScalarDateTime()
    {
        Assert.AreEqual(DateTime.MinValue, db.QueryScalar<DateTime>(Select("DateTime", 1)));
        Assert.AreEqual(new DateTime(1753, 1, 1, 0, 0, 0), db.QueryScalar<DateTime>(Select("DateTime", 2)));
        Assert.AreEqual(new DateTime(2018, 4, 2, 13, 14, 15), db.QueryScalar<DateTime>(Select("DateTime", 3)));
    }

    [Test]
    public void QueryScalarDateTimeStoredAsDateTime2()
    {
        Assert.AreEqual(DateTime.MinValue, db.QueryScalar<DateTime>(Select("DateTime2", 1)));
        Assert.AreEqual(DateTime.MinValue, db.QueryScalar<DateTime>(Select("DateTime2", 2)));
        Assert.AreEqual(new DateTime(2023, 8, 11, 14, 9, 15), db.QueryScalar<DateTime>(Select("DateTime2", 3)));
    }

    [Test]
    public void QueryScalarDateOnly()
    {
        Assert.AreEqual(DateOnly.MinValue, db.QueryScalar<DateOnly>(Select("Date", 1)));
        Assert.AreEqual(DateOnly.MinValue, db.QueryScalar<DateOnly>(Select("Date", 2)));
        Assert.AreEqual(new DateOnly(2023, 8, 11), db.QueryScalar<DateOnly>(Select("Date", 3)));
    }

    [Test]
    public void QueryScalarTimeOnly()
    {
        Assert.AreEqual(TimeOnly.MinValue, db.QueryScalar<TimeOnly>(Select("Time", 1)));
        Assert.AreEqual(TimeOnly.MinValue, db.QueryScalar<TimeOnly>(Select("Time", 2)));
        Assert.AreEqual(new TimeOnly(14, 9, 15), db.QueryScalar<TimeOnly>(Select("Time", 3)));
    }

    [Test]
    public void QueryScalarGuidStoredAsString()
    {
        Assert.AreEqual(Guid.Empty, db.QueryScalar<Guid>(Select("guid_as_string", 1)));
        Assert.AreEqual(Guid.Empty, db.QueryScalar<Guid>(Select("guid_as_string", 2)));
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), db.QueryScalar<Guid>(Select("guid_as_string", 3)));
    }

    [Test]
    public void QueryScalarGuidStoredAsUniqueIdentifier()
    {
        Assert.AreEqual(Guid.Empty, db.QueryScalar<Guid>(Select("guid_as_uniqueid", 1)));
        Assert.AreEqual(Guid.Empty, db.QueryScalar<Guid>(Select("guid_as_uniqueid", 2)));
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), db.QueryScalar<Guid>(Select("guid_as_uniqueid", 3)));
    }

    [Test]
    public void QueryScalarChar()
    {
        Assert.AreEqual('\0', db.QueryScalar<char>(Select("char", 1)));
        Assert.AreEqual('\0', db.QueryScalar<char>(Select("char", 2)));
        Assert.AreEqual('c', db.QueryScalar<char>(Select("char", 3)));
    }

    [Test]
    public void QueryScalarString()
    {
        Assert.AreEqual(null, db.QueryScalar<string>(Select("string", 1)));
        Assert.AreEqual("", db.QueryScalar<string>(Select("string", 2)));
        Assert.AreEqual("str", db.QueryScalar<string>(Select("string", 3)));
    }

    [Test]
    public void QueryScalarByteArray()
    {
        Assert.AreEqual(null, db.QueryScalar<byte[]>(Select("byte_array", 1)));
        Assert.AreEqual(null, db.QueryScalar<byte[]>(Select("byte_array", 2)));
        Assert.AreEqual(new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }, db.QueryScalar<byte[]>(Select("byte_array", 3)));
    }

    [Test]
    public void QueryScalarIntEnum()
    {
        Assert.AreEqual(Numbers.Zero, db.QueryScalar<Numbers>(Select("int_enum", 1)));
        Assert.AreEqual(Numbers.Zero, db.QueryScalar<Numbers>(Select("int_enum", 2)));
        Assert.AreEqual(Numbers.Three, db.QueryScalar<Numbers>(Select("int_enum", 3)));
    }

    [Test]
    public void QueryScalarStringEnum()
    {
        Assert.AreEqual(Colors.Red, db.QueryScalar<Colors>(Select("string_enum", 1)));
        Assert.AreEqual(Colors.Red, db.QueryScalar<Colors>(Select("string_enum", 2)));
        Assert.AreEqual(Colors.Green, db.QueryScalar<Colors>(Select("string_enum", 3)));
    }

    [Test]
    public void QueryScalarNullableBool()
    {
        Assert.IsNull(db.QueryScalar<bool?>(Select("bool", 1)));
        Assert.IsFalse(db.QueryScalar<bool?>(Select("bool", 2)));
        Assert.IsTrue(db.QueryScalar<bool?>(Select("bool", 3)));
    }

    [Test]
    public void QueryScalarNullableByte()
    {
        Assert.AreEqual(null, db.QueryScalar<byte?>(Select("byte", 1)));
        Assert.AreEqual(0, db.QueryScalar<byte?>(Select("byte", 2)));
        Assert.AreEqual(1, db.QueryScalar<byte?>(Select("byte", 3)));
    }

    [Test]
    public void QueryScalarNullableShort()
    {
        Assert.AreEqual(null, db.QueryScalar<short?>(Select("short", 1)));
        Assert.AreEqual(0, db.QueryScalar<short?>(Select("short", 2)));
        Assert.AreEqual(2, db.QueryScalar<short?>(Select("short", 3)));
    }

    [Test]
    public void QueryScalarNullableInt()
    {
        Assert.AreEqual(null, db.QueryScalar<int?>(Select("int", 1)));
        Assert.AreEqual(0, db.QueryScalar<int?>(Select("int", 2)));
        Assert.AreEqual(3, db.QueryScalar<int?>(Select("int", 3)));
    }

    [Test]
    public void QueryScalarNullableLong()
    {
        Assert.AreEqual(null, db.QueryScalar<long?>(Select("long", 1)));
        Assert.AreEqual(0, db.QueryScalar<long?>(Select("long", 2)));
        Assert.AreEqual(5, db.QueryScalar<long?>(Select("long", 3)));
    }

    [Test]
    public void QueryScalarNullableFloat()
    {
        Assert.AreEqual(null, db.QueryScalar<float?>(Select("float", 1)));
        Assert.AreEqual(0, db.QueryScalar<float?>(Select("float", 2)));
        Assert.AreEqual(8.13f, db.QueryScalar<float?>(Select("float", 3)));
    }

    [Test]
    public void QueryScalarNullableDouble()
    {
        Assert.AreEqual(null, db.QueryScalar<double?>(Select("double", 1)));
        Assert.AreEqual(0, db.QueryScalar<double?>(Select("double", 2)));
        Assert.AreEqual(21.34, db.QueryScalar<double?>(Select("double", 3)));
    }

    [Test]
    public void QueryScalarNullableDecimal()
    {
        Assert.AreEqual(null, db.QueryScalar<decimal?>(Select("decimal", 1)));
        Assert.AreEqual(0, db.QueryScalar<decimal?>(Select("decimal", 2)));
        Assert.AreEqual(55.89m, db.QueryScalar<decimal?>(Select("decimal", 3)));
    }

    [Test]
    public void QueryScalarNullableDateTime()
    {
        Assert.AreEqual(null, db.QueryScalar<DateTime?>(Select("DateTime", 1)));
        Assert.AreEqual(new DateTime(1753, 1, 1, 0, 0, 0), db.QueryScalar<DateTime?>(Select("DateTime", 2)));
        Assert.AreEqual(new DateTime(2018, 4, 2, 13, 14, 15), db.QueryScalar<DateTime?>(Select("DateTime", 3)));
    }

    [Test]
    public void QueryScalarNullableDateTimeStoredAsDateTime2()
    {
        Assert.AreEqual(null, db.QueryScalar<DateTime?>(Select("DateTime2", 1)));
        Assert.AreEqual(DateTime.MinValue, db.QueryScalar<DateTime?>(Select("DateTime2", 2)));
        Assert.AreEqual(new DateTime(2023, 8, 11, 14, 9, 15), db.QueryScalar<DateTime?>(Select("DateTime2", 3)));
    }

    [Test]
    public void QueryScalarNullableDateOnly()
    {
        Assert.AreEqual(null, db.QueryScalar<DateOnly?>(Select("Date", 1)));
        Assert.AreEqual(DateOnly.MinValue, db.QueryScalar<DateOnly?>(Select("Date", 2)));
        Assert.AreEqual(new DateOnly(2023, 8, 11), db.QueryScalar<DateOnly?>(Select("Date", 3)));
    }

    [Test]
    public void QueryScalarNullableTimeOnly()
    {
        Assert.AreEqual(null, db.QueryScalar<TimeOnly?>(Select("Time", 1)));
        Assert.AreEqual(TimeOnly.MinValue, db.QueryScalar<TimeOnly?>(Select("Time", 2)));
        Assert.AreEqual(new TimeOnly(14, 9, 15), db.QueryScalar<TimeOnly?>(Select("Time", 3)));
    }

    [Test]
    public void QueryScalarNullableGuidStoredAsString()
    {
        Assert.AreEqual(null, db.QueryScalar<Guid?>(Select("guid_as_string", 1)));
        Assert.AreEqual(null, db.QueryScalar<Guid?>(Select("guid_as_string", 2)));
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), db.QueryScalar<Guid?>(Select("guid_as_string", 3)));
    }

    [Test]
    public void QueryScalarNullableGuidStoredAsUniqueIdentifier()
    {
        Assert.AreEqual(null, db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 1)));
        Assert.AreEqual(null, db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 2)));
        Assert.AreEqual(new Guid("7ca43d156e8749dfbaffdb241d0d494c"), db.QueryScalar<Guid?>(Select("guid_as_uniqueid", 3)));
    }

    [Test]
    public void QueryScalarNullableChar()
    {
        Assert.AreEqual(null, db.QueryScalar<char?>(Select("char", 1)));
        Assert.AreEqual(null, db.QueryScalar<char?>(Select("char", 2)));
        Assert.AreEqual('c', db.QueryScalar<char?>(Select("char", 3)));
    }

    [Test]
    public void QueryScalarNullableIntEnum()
    {
        Assert.AreEqual(null, db.QueryScalar<Numbers?>(Select("int_enum", 1)));
        Assert.AreEqual(Numbers.Zero, db.QueryScalar<Numbers?>(Select("int_enum", 2)));
        Assert.AreEqual(Numbers.Three, db.QueryScalar<Numbers?>(Select("int_enum", 3)));
    }

    [Test]
    public void QueryScalarNullableStringEnum()
    {
        Assert.AreEqual(null, db.QueryScalar<Colors?>(Select("string_enum", 1)));
        Assert.AreEqual(null, db.QueryScalar<Colors?>(Select("string_enum", 2)));
        Assert.AreEqual(Colors.Green, db.QueryScalar<Colors?>(Select("string_enum", 3)));
    }

    [Test]
    public void QueryScalarUsingCustomValueConverter()
    {
        db.Mappers.AddValueConverter(x => new SemanticInt((int)x));

        var result = db.QueryScalar<SemanticInt>(Select("int", 3));

        Assert.AreEqual(3, result.Value);
    }

    class SemanticInt
    {
        public SemanticInt(int value) { Value = value; }
        public int Value { get; }
    }
}
