﻿using System;
using System.Collections.Generic;
using System.Linq;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests;

[TestFixture]
public class PropertyMapperShould
{
    [Test]
    public void GenerateDefaultPropertyMappingsForWritableProperties()
    {
        var mapper = PropMapper<Entity>();

        Assert.That(mapper.Mappings.Select(m => m.Field), Is.EqualTo(Strings("Id", "Name", "Title")).AsCollection);
    }

    [Test]
    public void OverrideDefaultMappingsWithExplicitMappings()
    {
        var mapper = PropMapper<Entity>()
            .Map(x => x.Name, "nom");

        var fields = mapper.Mappings.Select(m => m.Field);
        Assert.That(fields, Has.No.Member("Name"));
        Assert.That(fields, Has.Member("nom"));
    }

    [Test]
    public void ThrowExceptionWhenFieldIsMappedToMoreThanOneProperty()
    {
        var mapper = PropMapper<Entity>();

        Assert.That(() => mapper.Map(x => x.Name, "Title"), Throws.InstanceOf<MappingException>()
            .With.Message.StartsWith("Field cannot be mapped to more than one property"));
    }

    [Test]
    public void DefaultGeneratedMappingsToOptional()
    {
        var mapper = PropMapper<Entity>();

        Assert.That(mapper.Mappings.All(m => m.Condition == PopulationCondition.Optional), Is.True);
    }

    [Test]
    public void OverrideDefaultConditionsWithTypeCondition()
    {
        var mapper = PropMapper<Entity>(PopulationCondition.Required);

        Assert.That(mapper.Mappings.All(m => m.Condition == PopulationCondition.Required), Is.True);
    }

    [Test]
    public void MarkPropertyAsIgnoredWhenIgnored()
    {
        var mapper = PropMapper<Entity>()
            .Ignore(x => x.Title);

        Assert.That(mapper.Mappings.Single(m => m.Property.Name == "Title").IsIgnored, Is.True);
    }

    [Test]
    public void OverrideTypeConditionWithPropertyConditions()
    {
        var mapper = PropMapper<Entity>(PopulationCondition.Required)
            .MapNotNull(x => x.Id, "id")
            .MapOptional(x => x.Name, "name")
            .Optional(x => x.Title);

        var idMap = mapper.Mappings.Single(m => m.Property.Name == "Id");
        var nameMap = mapper.Mappings.Single(m => m.Property.Name == "Name");
        var titleMap = mapper.Mappings.Single(m => m.Property.Name == "Title");

        Assert.Multiple(() =>
        {
            Assert.That(idMap.Condition, Is.EqualTo(PopulationCondition.NotNull));
            Assert.That(nameMap.Condition, Is.EqualTo(PopulationCondition.Optional));
            Assert.That(titleMap.Condition, Is.EqualTo(PopulationCondition.Optional));
        });
    }

    [Test]
    public void KeepTypeConditionWhenNonConditionalMapIsCalled()
    {
        var mapper = PropMapper<Entity>(PopulationCondition.Required)
            .Map(x => x.Name, "nom");

        Assert.That(mapper.Mappings.Single(m => m.Property.Name == "Name").Condition, Is.EqualTo(PopulationCondition.Required));
    }

    [Test]
    public void SetCustomValueConverterWhenSpecified()
    {
        Func<object, object> customConverter = value => int.Parse(value.ToString());

        var mapper = PropMapper<Entity>()
            .Map(x => x.Id, "id", customConverter);

        Assert.That(mapper.Mappings.Single(m => m.Property.Name == "Id").FromDbConverter, Is.SameAs(customConverter));
    }

    [Test]
    public void SetAfterBindingActionForEntityWhenSpecified()
    {
        Action<Record, Entity> action = (record, entity) => entity.Name = "name";

        var mapper = PropMapper<Entity>()
            .AfterBinding(action);

        Assert.That(mapper.AfterBindAction, Is.SameAs(action));
    }

    #region Instance activation and population

    [Test]
    public void PopulateFromAutoGeneratedMapping()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>();
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "Tester")));
    }

    [Test]
    public void PopulateFromCustomMapping()
    {
        var record = Record(("id", 10), ("nom", "John Q"), ("role", "Tester"));

        var mapper = PropMapper<Entity>()
            .Map(x => x.Name, "nom")
            .Map(x => x.Title, "role");
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "Tester")));
    }

    [Test]
    public void PopulateFromCustomMappingWithValueConverter()
    {
        var record = Record(("id", "ten"), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .Map(x => x.Id, "id", v => v.ToString() == "ten" ? 10 : 0);
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "Tester")));
    }

    [Test]
    public void CallAfterBindActionWhenSet()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .AfterBinding((r, e) => e.Title = "override");
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "override")));
    }

    [Test]
    public void NotLoadFromRecordWhenPropertyIsMarkedIgnoreBoth()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .Ignore(x => x.Title);
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", null)));
    }

    [Test]
    public void NotLoadFromRecordWhenPropertyIsMarkedIgnoreOnLoad()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .IgnoreOnLoad(x => x.Title);
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", null)));
    }

    [Test]
    public void LoadFromRecordWhenPropertyIsMarkedIgnoreOnSave()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .IgnoreOnSave(x => x.Title);
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "Tester")));
    }

    [Test]
    public void PopulateWhenRecordHasNonNormalName()
    {
        var record = Record(("id", 10), ("name", "John Q"), ("ti_tle", "Tester"));

        var mapper = PropMapper<Entity>();
        var inst = mapper.CreateInstance(record);

        Assert.That(inst, Is.EqualTo(new Entity(10, "John Q", "Tester")));
    }

    [Test]
    public void PopulateWhenPropertyHasNonNormalName()
    {
        var record = Record(("EntityId", 50));

        var mapper = PropMapper<NonNormalEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.That(inst.Entity_Id, Is.EqualTo(50));
    }

    [Test]
    public void ConvertNullsToDefaultForValueTypes()
    {
        var record = Record(("intprop", null), ("dateprop", DBNull.Value));

        var mapper = PropMapper<IntDateEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.IntProp, Is.EqualTo(0));
            Assert.That(inst.DateProp, Is.EqualTo(DateTime.MinValue));
        });
    }

    [Test]
    public void SetNullForNullableValueTypes()
    {
        var record = Record(("intprop", null), ("dateprop", DBNull.Value));

        var mapper = PropMapper<NullableIntDateEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.IntProp, Is.Null);
            Assert.That(inst.DateProp, Is.Null);
        });
    }

    [Test]
    public void ConvertEnums()
    {
        var record = Record(("color", "Blue"), ("shape", 2), ("othercolor", DBNull.Value), ("othershape", ""));

        var mapper = PropMapper<EnumEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Color, Is.EqualTo(EnumEntity.Colors.Blue));
            Assert.That(inst.Shape, Is.EqualTo(EnumEntity.Shapes.Square));
            Assert.That(inst.OtherColor, Is.EqualTo(EnumEntity.Colors.Red));
            Assert.That(inst.OtherShape, Is.EqualTo(EnumEntity.Shapes.Circle));
        });
    }

    [Test]
    public void ThrowWhenRecordHasUndefinedEnumValue()
    {
        var record1 = Record(("color", "Yellow"));
        var record2 = Record(("shape", 5));

        var mapper = PropMapper<EnumEntity>();

        Assert.That(() => mapper.CreateInstance(record1), Throws.TypeOf<BindingException>());
        Assert.That(() => mapper.CreateInstance(record2), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void ConvertGuids()
    {
        var record = Record(("foo", null), ("bar", DBNull.Value), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));

        var mapper = PropMapper<GuidEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Foo, Is.EqualTo(Guid.Empty));
            Assert.That(inst.Bar, Is.EqualTo(Guid.Empty));
            Assert.That(inst.Baz, Is.EqualTo(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC")));
        });
    }

    [Test]
    public void ConvertNullableGuids()
    {
        var record = Record(("foo", null), ("bar", string.Empty), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));

        var mapper = PropMapper<NullableGuidEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.Foo, Is.Null);
            Assert.That(inst.Bar, Is.Null);
            Assert.That(inst.Baz, Is.EqualTo(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC")));
        });
    }

    [Test]
    public void ThrowWhenRecordHasInvalidGuidValue()
    {
        var record = Record(("foo", "bogusguid"));

        var mapper = PropMapper<GuidEntity>();

        Assert.That(() => mapper.CreateInstance(record), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void ConvertEmptyCharColumnsIntoDefaultForCharType()
    {
        var record = Record(("CharProp", string.Empty));

        var mapper = PropMapper<CharEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.That(inst.CharProp, Is.EqualTo(default(char)));
    }

    [Test]
    public void ConvertValueTypesStoredAsStringsInTheRecord()
    {
        var record = Record(("intprop", "42"), ("dateprop", "2018-04-02 08:30:01"), ("decimalprop", "3.14"));

        var mapper = PropMapper<ValueEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.IntProp, Is.EqualTo(42));
            Assert.That(inst.DateProp, Is.EqualTo(new DateTime(2018, 4, 2, 8, 30, 1)));
            Assert.That(inst.DecimalProp, Is.EqualTo(3.14m));
        });
    }

    [Test]
    public void DeserializeJsonWhenPropertyIsComplexAndFieldIsJsonString()
    {
        var record = Record(("IntDateProp", "{ \"IntProp\" : 5, \"DateProp\" : \"2016-01-01 16:15:00 \" }"));

        var mapper = PropMapper<JsonEntity>();
        var inst = mapper.CreateInstance(record);

        Assert.Multiple(() =>
        {
            Assert.That(inst.IntDateProp.IntProp, Is.EqualTo(5));
            Assert.That(inst.IntDateProp.DateProp, Is.EqualTo(new DateTime(2016, 1, 1, 16, 15, 0)));
        });
    }

    [Test]
    public void DeserializeJsonWhenPropertyIsArrayAndFieldIsJsonArray()
    {
        var record1 = Record(("numbers", "[1, 2, 3, 4]"), ("strings", "[\"foo\", \"bar\"]"));
        var record2 = Record(("numbers", "[]"), ("strings", "[]"));
        var record3 = Record(("numbers", null), ("strings", ""));

        var mapper = PropMapper<JsonArrayEntity>();
        var inst1 = mapper.CreateInstance(record1);
        var inst2 = mapper.CreateInstance(record2);
        var inst3 = mapper.CreateInstance(record3);

        Assert.Multiple(() =>
        {
            Assert.That(inst1.Numbers, Is.EqualTo(new int[] { 1, 2, 3, 4 }).AsCollection);
            Assert.That(inst1.Strings, Is.EqualTo(Strings("foo", "bar")).AsCollection);

            Assert.That(inst2.Numbers, Is.Empty);
            Assert.That(inst2.Strings.Count(), Is.EqualTo(0));

            Assert.That(inst3.Numbers, Is.Null);
            Assert.That(inst3.Strings, Is.Null);
        });
    }

    [Test]
    public void ThrowWhenRecordIsMissingRequiredField()
    {
        var record = Record(("Id", 10), ("Title", "Tester"));

        var mapper = PropMapper<Entity>()
            .Required(x => x.Name);

        Assert.That(() => mapper.CreateInstance(record), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void ThrowWhenRecordHasNullInNotNullField()
    {
        var record = Record(("id", 10), ("name", null), ("title", "Tester"));

        var mapper = PropMapper<Entity>()
            .NotNull(x => x.Name);

        Assert.That(() => mapper.CreateInstance(record), Throws.TypeOf<BindingException>());
    }

    [Test]
    public void UseCustomValueConverterIfAvailableForType()
    {
        var cvs = new Dictionary<Type, ValueConverter>
        {
            [typeof(SemanticInt)] = new ValueConverter<SemanticInt>(x => new SemanticInt((int)x))
        };
        var record = Record(("id", 42), ("agentnumber", 86));
        var mapper = PropMapper<EntityWithSemanticProperty>(cvs);

        var inst = mapper.CreateInstance(record);

        Assert.That(inst.AgentNumber.Value, Is.EqualTo(86));
    }

    #endregion

    #region Test types

    class NonNormalEntity
    {
        public int Entity_Id { get; set; }
    }

    class IntDateEntity
    {
        public int IntProp { get; set; }
        public DateTime DateProp { get; set; }
    }

    class NullableIntDateEntity
    {
        public int? IntProp { get; set; }
        public DateTime? DateProp { get; set; }
    }

    class EnumEntity
    {
        public enum Colors { Red, Blue, Green }
        public enum Shapes { Circle, Triangle, Square }

        public Colors Color { get; set; }
        public Shapes Shape { get; set; }
        public Colors OtherColor { get; set; }
        public Shapes OtherShape { get; set; }
    }

    class GuidEntity
    {
        public Guid Foo { get; set; }
        public Guid Bar { get; set; }
        public Guid Baz { get; set; }
    }

    class NullableGuidEntity
    {
        public Guid? Foo { get; set; }
        public Guid? Bar { get; set; }
        public Guid? Baz { get; set; }
    }

    class CharEntity
    {
        public char CharProp { get; set; }
    }

    class ValueEntity
    {
        public int IntProp { get; set; }
        public DateTime DateProp { get; set; }
        public decimal DecimalProp { get; set; }
    }

    class JsonEntity
    {
        public IntDateEntity IntDateProp { get; set; }
    }

    class JsonArrayEntity
    {
        public int[] Numbers { get; set; }
        public IEnumerable<string> Strings { get; set; }
    }

    #endregion
}
