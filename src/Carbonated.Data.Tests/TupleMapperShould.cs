using System;
using System.Collections.Generic;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

[TestFixture]
public class TupleMapperShould
{
    private enum Colors { Red, Blue, Green }
    private enum Shapes { Circle, Triangle, Square }


    [Test]
    public void ConvertNullsToDefaultForValueTypes()
    {
        var record = Record(("intprop", null), ("dateprop", DBNull.Value));
        var mapper = new TupleMapper<(int, DateTime)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(0));
            Assert.That(inst.Item2, Is.EqualTo(DateTime.MinValue));
        });
    }

    [Test]
    public void SetNullForNullableValueTypes()
    {
        var record = Record(("intprop", null), ("dateprop", DBNull.Value));
        var mapper = new TupleMapper<(int?, DateTime?)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.Null);
            Assert.That(inst.Item2, Is.Null);
        });
    }

    [Test]
    public void ConvertEnums()
    {
        var record = Record(("color", "Blue"), ("shape", 2), ("othercolor", DBNull.Value), ("othershape", ""));
        var mapper = new TupleMapper<(Colors, Shapes, Colors, Shapes)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(Colors.Blue));
            Assert.That(inst.Item2, Is.EqualTo(Shapes.Square));
            Assert.That(inst.Item3, Is.EqualTo(Colors.Red));
            Assert.That(inst.Item4, Is.EqualTo(Shapes.Circle));
        });
    }

    [Test]
    public void ThrowWhenRecordHasUndefinedEnumValue()
    {
        var record1 = Record(("color", "Yellow"), ("shape", 0));
        var record2 = Record(("color", 0), ("shape", 5));
        var mapper = new TupleMapper<(Colors, Shapes)>(null);

        Assert.That(() => mapper.CreateInstance(record1), Throws.TypeOf<BindingException>());
        Assert.That(() => mapper.CreateInstance(record2), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void ConvertGuids()
    {
        var record = Record(("foo", null), ("bar", DBNull.Value), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));
        var mapper = new TupleMapper<(Guid, Guid, Guid)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(Guid.Empty));
            Assert.That(inst.Item2, Is.EqualTo(Guid.Empty));
            Assert.That(inst.Item3, Is.EqualTo(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC")));
        });
    }

    [Test]
    public void ConvertNullableGuids()
    {
        var record = Record(("foo", null), ("bar", string.Empty), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));
        var mapper = new TupleMapper<(Guid?, Guid?, Guid?)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.Null);
            Assert.That(inst.Item2, Is.Null);
            Assert.That(inst.Item3, Is.EqualTo(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC")));
        });
    }

    [Test]
    public void ThrowWhenRecordHasInvalidGuidValue()
    {
        var record = Record(("foo", "bogusguid"), ("bar", "test"));

        var mapper = new TupleMapper<(Guid, string)>(null);

        Assert.That(() => mapper.CreateInstance(record), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void ConvertEmptyCharColumnsIntoDefaultForCharType()
    {
        var record = Record(("CharProp", string.Empty), ("string","test"));
        var mapper = new TupleMapper<(char, string)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.That(inst.Item1, Is.EqualTo(default(char)));
    }

    [Test]
    public void ConvertValueTypesStoredAsStringsInTheRecord()
    {
        var record = Record(("intprop", "42"), ("dateprop", "2018-04-02 08:30:01"), ("decimalprop", "3.14"));
        var mapper = new TupleMapper<(int, DateTime, decimal)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(42));
            Assert.That(inst.Item2, Is.EqualTo(new DateTime(2018, 4, 2, 8, 30, 1)));
            Assert.That(inst.Item3, Is.EqualTo(3.14m));
        });
    }

    [Test]
    public void UseCustomValueConverterIfAvailableForType()
    {
        var cvs = new Dictionary<Type, ValueConverter>
        {
            [typeof(SemanticInt)] = new ValueConverter<SemanticInt>(x => new SemanticInt((int)x))
        };
        var record = Record(("id", 42), ("agentnumber", 86));
        var mapper = new TupleMapper<(int, SemanticInt)>(cvs);

        var inst = mapper.CreateInstance(record);

        Assert.That(inst.Item2.Value, Is.EqualTo(86));
    }

    [Test]
    public void CreateRecordWhenMoreFieldsThanNeededArePresent()
    {
        var record = Record(("foo", 1), ("bar", 2), ("baz", 3));
        var mapper = new TupleMapper<(int, int)>(null);

        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Item1, Is.EqualTo(1));
            Assert.That(inst.Item2, Is.EqualTo(2));
        });
    }

    [Test]
    public void ThrowWhenRecordDoesNotHaveEnoughFieldsForTuple()
    {
        var record = Record(("foo", 1), ("bar", 2), ("baz", 3));
        var mapper = new TupleMapper<(int, int, int, int)>(null);

        var act = () => mapper.CreateInstance(record);

        Assert.That(act, Throws.ArgumentException.With.Message.Contains("record has fewer fields"));
    }
}
