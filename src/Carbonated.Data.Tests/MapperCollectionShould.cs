using System;
using NUnit.Framework;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class MapperCollectionShould
    {
        private MapperCollection mapperCollection;

        [SetUp]
        public void SetUp()
        {
            mapperCollection = new MapperCollection();
        }

        [Test]
        public void AddTypeMapperWhenCreateFuncIsConfigured()
        {
            mapperCollection.Configure<Entity>(record => new Entity());

            Assert.IsInstanceOf<TypeMapper<Entity>>(mapperCollection.Get<Entity>());
        }

        [Test]
        public void AddPropertyMapperAndReturnItWhenConfigured()
        {
            var mapper = mapperCollection.Configure<Entity>();

            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void AddPropertyMapperWithDefaultConditionAndReturnItWhenConfigured()
        {
            var mapper = mapperCollection.Configure<Entity>(PopulationCondition.Required);

            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void KnowWhatMappersHaveBeenConfigured()
        {
            mapperCollection.Configure<Entity>();

            Assert.IsTrue(mapperCollection.HasMapper<Entity>());
        }

        [Test]
        public void ThrowOnConfigureIfTypeIsAlreadyRegistered()
        {
            mapperCollection.Configure<Entity>();

            Assert.Throws<MappingException>(() => mapperCollection.Configure<Entity>());
        }

        [Test]
        public void AutoGenerateAndAddPropertyMapperDuringGetIfNotYetRegistered()
        {
            Assert.IsFalse(mapperCollection.HasMapper<Entity>());

            var mapper = mapperCollection.Get<Entity>();

            Assert.IsTrue(mapperCollection.HasMapper<Entity>());
            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void HaveFrameworkTypeMappersBuiltIn()
        {
            Assert.IsTrue(mapperCollection.HasMapper<bool>());
            Assert.IsTrue(mapperCollection.HasMapper<byte>());
            Assert.IsTrue(mapperCollection.HasMapper<short>());
            Assert.IsTrue(mapperCollection.HasMapper<int>());
            Assert.IsTrue(mapperCollection.HasMapper<long>());
            Assert.IsTrue(mapperCollection.HasMapper<float>());
            Assert.IsTrue(mapperCollection.HasMapper<double>());
            Assert.IsTrue(mapperCollection.HasMapper<decimal>());
            Assert.IsTrue(mapperCollection.HasMapper<DateTime>());
            Assert.IsTrue(mapperCollection.HasMapper<Guid>());
            Assert.IsTrue(mapperCollection.HasMapper<char>());
            Assert.IsTrue(mapperCollection.HasMapper<string>());
            Assert.IsTrue(mapperCollection.HasMapper<byte[]>());

            Assert.IsTrue(mapperCollection.HasMapper<bool?>());
            Assert.IsTrue(mapperCollection.HasMapper<byte?>());
            Assert.IsTrue(mapperCollection.HasMapper<short?>());
            Assert.IsTrue(mapperCollection.HasMapper<int?>());
            Assert.IsTrue(mapperCollection.HasMapper<long?>());
            Assert.IsTrue(mapperCollection.HasMapper<float?>());
            Assert.IsTrue(mapperCollection.HasMapper<double?>());
            Assert.IsTrue(mapperCollection.HasMapper<decimal?>());
            Assert.IsTrue(mapperCollection.HasMapper<DateTime?>());
            Assert.IsTrue(mapperCollection.HasMapper<Guid?>());
            Assert.IsTrue(mapperCollection.HasMapper<char?>());
        }
    }
}
