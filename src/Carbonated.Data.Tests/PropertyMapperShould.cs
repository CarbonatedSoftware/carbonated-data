﻿using System;
using System.Collections.Generic;
using System.Linq;
using Carbonated.Data.Internals;
using Carbonated.Data.Tests.Types;
using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class PropertyMapperShould
    {
        [Test]
        public void GenerateDefaultPropertyMappingsForWritableProperties()
        {
            var mapper = PropMapper<Entity>();

            CollectionAssert.AreEqual(Strings("Id", "Name", "Title"), mapper.Mappings.Select(m => m.Field));
        }

        [Test]
        public void OverrideDefaultMappingsWithExplicitMappings()
        {
            var mapper = PropMapper<Entity>()
                .Map(x => x.Name, "nom");

            var fields = mapper.Mappings.Select(m => m.Field);
            CollectionAssert.DoesNotContain(fields, "Name");
            CollectionAssert.Contains(fields, "nom");
        }

        [Test]
        public void ThrowExceptionWhenFieldIsMappedToMoreThanOneProperty()
        {
            var mapper = PropMapper<Entity>();

            var ex = Assert.Throws<MappingException>(() => mapper.Map(x => x.Name, "Title"));
            StringAssert.StartsWith("Field cannot be mapped to more than one property", ex.Message);
        }

        [Test]
        public void DefaultGeneratedMappingsToOptional()
        {
            var mapper = PropMapper<Entity>();

            Assert.IsTrue(mapper.Mappings.All(m => m.Condition == PopulationCondition.Optional));
        }

        [Test]
        public void OverrideDefaultConditionsWithTypeCondition()
        {
            var mapper = PropMapper<Entity>(PopulationCondition.Required);

            Assert.IsTrue(mapper.Mappings.All(m => m.Condition == PopulationCondition.Required));
        }

        [Test]
        public void MarkPropertyAsIgnoredWhenIgnored()
        {
            var mapper = PropMapper<Entity>()
                .Ignore(x => x.Title);

            Assert.IsTrue(mapper.Mappings.Single(m => m.Property.Name == "Title").IsIgnored);
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

            Assert.AreEqual(PopulationCondition.NotNull, idMap.Condition);
            Assert.AreEqual(PopulationCondition.Optional, nameMap.Condition);
            Assert.AreEqual(PopulationCondition.Optional, titleMap.Condition);
        }

        [Test]
        public void KeepTypeConditionWhenNonConditionalMapIsCalled()
        {
            var mapper = PropMapper<Entity>(PopulationCondition.Required)
                .Map(x => x.Name, "nom");

            Assert.AreEqual(PopulationCondition.Required, mapper.Mappings.Single(m => m.Property.Name == "Name").Condition);
        }

        [Test]
        public void SetCustomValueConverterWhenSpecified()
        {
            Func<object, object> customConverter = value => int.Parse(value.ToString());

            var mapper = PropMapper<Entity>()
                .Map(x => x.Id, "id", customConverter);

            Assert.AreSame(customConverter, mapper.Mappings.Single(m => m.Property.Name == "Id").ValueConverter);
        }

        [Test]
        public void SetAfterBindingActionForEntityWhenSpecified()
        {
            Action<Record, Entity> action = (record, entity) => entity.Name = "name";

            var mapper = PropMapper<Entity>()
                .AfterBinding(action);

            Assert.AreSame(action, mapper.AfterBindAction);
        }

        #region Instance activation and population

        [Test]
        public void PopulateFromAutoGeneratedMapping()
        {
            var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

            var mapper = PropMapper<Entity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void PopulateFromCustomMapping()
        {
            var record = Record(("id", 10), ("nom", "John Q"), ("role", "Tester"));

            var mapper = PropMapper<Entity>()
                .Map(x => x.Name, "nom")
                .Map(x => x.Title, "role");
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void PopulateFromCustomMappingWithValueConverter()
        {
            var record = Record(("id", "ten"), ("name", "John Q"), ("title", "Tester"));

            var mapper = PropMapper<Entity>()
                .Map(x => x.Id, "id", v => v.ToString() == "ten" ? 10 : 0);
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void CallAfterBindActionWhenSet()
        {
            var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

            var mapper = PropMapper<Entity>()
                .AfterBinding((r, e) => e.Title = "override");
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "override"), inst);
        }

        [Test]
        public void NotLoadFromRecordWhenPropertyIsMarkedIgnore()
        {
            var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

            var mapper = PropMapper<Entity>()
                .Ignore(x => x.Title);
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", null), inst);
        }

        [Test]
        public void PopulateWhenRecordHasNonNormalName()
        {
            var record = Record(("id", 10), ("name", "John Q"), ("ti_tle", "Tester"));

            var mapper = PropMapper<Entity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void PopulateWhenPropertyHasNonNormalName()
        {
            var record = Record(("EntityId", 50));

            var mapper = PropMapper<NonNormalEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(50, inst.Entity_Id);
        }

        [Test]
        public void ConvertNullsToDefaultForValueTypes()
        {
            var record = Record(("intprop", null), ("dateprop", DBNull.Value));

            var mapper = PropMapper<IntDateEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(0, inst.IntProp);
            Assert.AreEqual(DateTime.MinValue, inst.DateProp);
        }

        [Test]
        public void SetNullForNullableValueTypes()
        {
            var record = Record(("intprop", null), ("dateprop", DBNull.Value));

            var mapper = PropMapper<NullableIntDateEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.IsNull(inst.IntProp);
            Assert.IsNull(inst.DateProp);
        }

        [Test]
        public void ConvertEnums()
        {
            var record = Record(("color", "Blue"), ("shape", 2), ("othercolor", DBNull.Value), ("othershape", ""));

            var mapper = PropMapper<EnumEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(EnumEntity.Colors.Blue, inst.Color);
            Assert.AreEqual(EnumEntity.Shapes.Square, inst.Shape);
            Assert.AreEqual(EnumEntity.Colors.Red, inst.OtherColor);
            Assert.AreEqual(EnumEntity.Shapes.Circle, inst.OtherShape);
        }

        [Test]
        public void ThrowWhenRecordHasUndefinedEnumValue()
        {
            var record1 = Record(("color", "Yellow"));
            var record2 = Record(("shape", 5));

            var mapper = PropMapper<EnumEntity>();

            Assert.Throws<BindingException>(() => mapper.CreateInstance(record1));
            Assert.Throws<BindingException>(() => mapper.CreateInstance(record2));
        }

        [Test]
        public void ConvertGuids()
        {
            var record = Record(("foo", null), ("bar", DBNull.Value), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));

            var mapper = PropMapper<GuidEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(Guid.Empty, inst.Foo);
            Assert.AreEqual(Guid.Empty, inst.Bar);
            Assert.AreEqual(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"), inst.Baz);
        }

        [Test]
        public void ConvertNullableGuids()
        {
            var record = Record(("foo", null), ("bar", string.Empty), ("baz", "10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"));

            var mapper = PropMapper<NullableGuidEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.IsNull(inst.Foo);
            Assert.IsNull(inst.Bar);
            Assert.AreEqual(new Guid("10DB5BD9-A8CC-46E2-A5EB-791460B0B1CC"), inst.Baz);
        }

        [Test]
        public void ThrowWhenRecordHasInvalidGuidValue()
        {
            var record = Record(("foo", "bogusguid"));

            var mapper = PropMapper<GuidEntity>();

            Assert.Throws<BindingException>(() => mapper.CreateInstance(record));
        }

        [Test]
        public void ConvertEmptyCharColumnsIntoDefaultForCharType()
        {
            var record = Record(("CharProp", string.Empty));

            var mapper = PropMapper<CharEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(default(char), inst.CharProp);
        }

        [Test]
        public void ConvertValueTypesStoredAsStringsInTheRecord()
        {
            var record = Record(("intprop", "42"), ("dateprop", "2018-04-02 08:30:01"), ("decimalprop", "3.14"));

            var mapper = PropMapper<ValueEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(42, inst.IntProp);
            Assert.AreEqual(new DateTime(2018, 4, 2, 8, 30, 1), inst.DateProp);
            Assert.AreEqual(3.14m, inst.DecimalProp);
        }

        [Test]
        public void DeserializeJsonWhenPropertyIsComplexAndFieldIsJsonString()
        {
            var record = Record(("IntDateProp", "{ \"IntProp\" : 5, \"DateProp\" : \"2016-01-01 16:15:00 \" }"));

            var mapper = PropMapper<JsonEntity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(inst.IntDateProp.IntProp, 5);
            Assert.AreEqual(inst.IntDateProp.DateProp, new DateTime(2016, 1, 1, 16, 15, 0));
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

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, inst1.Numbers);
            CollectionAssert.AreEqual(Strings("foo", "bar"), inst1.Strings);

            Assert.AreEqual(0, inst2.Numbers.Length);
            Assert.AreEqual(0, inst2.Strings.Count());

            Assert.IsNull(inst3.Numbers);
            Assert.IsNull(inst3.Strings);
        }

        [Test]
        public void ThrowWhenRecordIsMissingRequiredField()
        {
            var record = Record(("Id", 10), ("Title", "Tester"));

            var mapper = PropMapper<Entity>()
                .Required(x => x.Name);

            Assert.Throws<BindingException>(() => mapper.CreateInstance(record));
        }

        [Test]
        public void ThrowWhenRecordHasNullInNotNullField()
        {
            var record = Record(("id", 10), ("name", null), ("title", "Tester"));

            var mapper = PropMapper<Entity>()
                .NotNull(x => x.Name);

            Assert.Throws<BindingException>(() => mapper.CreateInstance(record));
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

            Assert.AreEqual(86, inst.AgentNumber.Value);
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
}
