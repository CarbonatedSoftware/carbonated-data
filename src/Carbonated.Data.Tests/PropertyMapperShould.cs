﻿using System;
using System.Linq;
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
            var mapper = new PropertyMapper<Entity>();

            CollectionAssert.AreEqual(Strings("Id", "Name", "Title"), mapper.Mappings.Select(m => m.Field));
        }

        [Test]
        public void OverrideDefaultMappingsWithExplicitMappings()
        {
            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Name, "nom");

            var fields = mapper.Mappings.Select(m => m.Field);
            CollectionAssert.DoesNotContain(fields, "Name");
            CollectionAssert.Contains(fields, "nom");
        }

        [Test]
        public void ThrowExceptionWhenFieldIsMappedToMoreThanOneProperty()
        {
            var mapper = new PropertyMapper<Entity>();

            var ex = Assert.Throws<Exception>(() => mapper.Map(x => x.Name, "Title"));
            StringAssert.StartsWith("Field cannot be mapped to more than one property", ex.Message);
        }

        [Test]
        public void DefaultGeneratedMappingsToOptional()
        {
            var mapper = new PropertyMapper<Entity>();

            Assert.IsTrue(mapper.Mappings.All(m => m.Condition == PopulationCondition.Optional));
        }

        [Test]
        public void OverrideDefaultConditionsWithTypeCondition()
        {
            var mapper = new PropertyMapper<Entity>(PopulationCondition.Required);

            Assert.IsTrue(mapper.Mappings.All(m => m.Condition == PopulationCondition.Required));
        }

        [Test]
        public void MarkPropertyAsIgnoredWhenIgnored()
        {
            var mapper = new PropertyMapper<Entity>()
                .Ignore(x => x.Title);

            Assert.IsTrue(mapper.Mappings.Single(m => m.Property.Name == "Title").IsIgnored);
        }

        [Test]
        public void OverrideTypeConditionWithPropertyConditions()
        {
            var mapper = new PropertyMapper<Entity>(PopulationCondition.Required)
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
            var mapper = new PropertyMapper<Entity>(PopulationCondition.Required)
                .Map(x => x.Name, "nom");

            Assert.AreEqual(PopulationCondition.Required, mapper.Mappings.Single(m => m.Property.Name == "Name").Condition);
        }

        [Test]
        public void SetCustomValueConverterWhenSpecified()
        {
            Func<object, object> customConverter = value => int.Parse(value.ToString());

            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Id, "id", customConverter);

            Assert.AreSame(customConverter, mapper.Mappings.Single(m => m.Property.Name == "Id").ValueConverter);
        }

        [Test]
        public void SetAfterBindingActionForEntityWhenSpecified()
        {
            Action<Record, Entity> action = (record, entity) => entity.Name = "name";

            var mapper = new PropertyMapper<Entity>()
                .AfterBinding(action);

            Assert.AreSame(action, mapper.AfterBindAction);
        }

        #region Instance activation and population

        [Test]
        public void PopulateFromAutoGeneratedMapping()
        {
            var record = Record(("id", 10), ("name", "John Q"), ("title", "Tester"));

            var mapper = new PropertyMapper<Entity>();
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void PopulateFromCustomMapping()
        {
            var record = Record(("id", 10), ("nom", "John Q"), ("role", "Tester"));

            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Name, "nom")
                .Map(x => x.Title, "role");
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        [Test]
        public void PopulateFromCustomMappingWithValueConverter()
        {
            var record = Record(("id", "10"), ("name", "John Q"), ("title", "Tester"));

            var mapper = new PropertyMapper<Entity>()
                .Map(x => x.Id, "id", v => int.Parse(v.ToString()));
            var inst = mapper.CreateInstance(record);

            Assert.AreEqual(new Entity(10, "John Q", "Tester"), inst);
        }

        // call after bind action when set

        // ignore data when property is marked ignored

        // populate when record has non-normal name

        // populate when property has non-normal name

        // convert nulls to default for value types

        // set nulls on nullable value types

        // convert enums

        // throw when undefined non-numeric passed for an enum

        // convert guids

        // convert nullable guids

        // throw when invalid guid value is passed

        // convert empty char columns as char default

        // convert value types stored as strings

        // deserialize json for complex properties when field is string

        // deserialize json for complex properties for arrays

        // deserialize json for complex properties for arrays when empty

        // deserialize json for complex properties for arrays when value is null

        // throw when required property has missing field

        // throw when not null property is null


        #endregion
    }
}
