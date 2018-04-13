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
        public void WillThrowExceptionWhenFieldIsMappedToMoreThanOneProperty()
        {
            var mapper = new PropertyMapper<Entity>();

            var ex = Assert.Throws<Exception>(() => mapper.Map(x => x.Name, "Title"));
            StringAssert.StartsWith("Field cannot be mapped to more than one property", ex.Message);
        }


        private string[] Strings(params string[] strings) => strings;
    }
}
