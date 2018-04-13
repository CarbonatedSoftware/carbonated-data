using System;
using System.Linq;
using NUnit.Framework;

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

        private string[] Strings(params string[] strings) => strings;
    }
}
