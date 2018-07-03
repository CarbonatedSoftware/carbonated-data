using System;
using Carbonated.Data.Internals;
using NUnit.Framework;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class MapperCollectionShould
    {
        private MapperCollection mc;

        [SetUp]
        public void SetUp()
        {
            mc = new MapperCollection();
        }

        [Test]
        public void AddTypeMapperWhenCreateFuncIsConfigured()
        {
            mc.Configure<Entity>(record => new Entity());

            Assert.IsInstanceOf<TypeMapper<Entity>>(mc.Get<Entity>());
        }

        [Test]
        public void AddPropertyMapperAndReturnItWhenConfigured()
        {
            var mapper = mc.Configure<Entity>();

            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void AddPropertyMapperWithDefaultConditionAndReturnItWhenConfigured()
        {
            var mapper = mc.Configure<Entity>(PopulationCondition.Required);

            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void KnowWhatMappersHaveBeenConfigured()
        {
            mc.Configure<Entity>();

            Assert.IsTrue(mc.HasMapper<Entity>());
        }

        [Test]
        public void ThrowOnConfigureIfTypeIsAlreadyRegistered()
        {
            mc.Configure<Entity>();

            Assert.Throws<MappingException>(() => mc.Configure<Entity>());
        }

        [Test]
        public void AutoGenerateAndAddPropertyMapperDuringGetIfNotYetRegistered()
        {
            Assert.IsFalse(mc.HasMapper<Entity>());

            var mapper = mc.Get<Entity>();

            Assert.IsTrue(mc.HasMapper<Entity>());
            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void HaveFrameworkTypeMappersBuiltIn()
        {
            Assert.IsTrue(mc.HasMapper<bool>());
            Assert.IsTrue(mc.HasMapper<byte>());
            Assert.IsTrue(mc.HasMapper<short>());
            Assert.IsTrue(mc.HasMapper<int>());
            Assert.IsTrue(mc.HasMapper<long>());
            Assert.IsTrue(mc.HasMapper<float>());
            Assert.IsTrue(mc.HasMapper<double>());
            Assert.IsTrue(mc.HasMapper<decimal>());
            Assert.IsTrue(mc.HasMapper<DateTime>());
            Assert.IsTrue(mc.HasMapper<Guid>());
            Assert.IsTrue(mc.HasMapper<char>());
            Assert.IsTrue(mc.HasMapper<string>());
            Assert.IsTrue(mc.HasMapper<byte[]>());

            Assert.IsTrue(mc.HasMapper<bool?>());
            Assert.IsTrue(mc.HasMapper<byte?>());
            Assert.IsTrue(mc.HasMapper<short?>());
            Assert.IsTrue(mc.HasMapper<int?>());
            Assert.IsTrue(mc.HasMapper<long?>());
            Assert.IsTrue(mc.HasMapper<float?>());
            Assert.IsTrue(mc.HasMapper<double?>());
            Assert.IsTrue(mc.HasMapper<decimal?>());
            Assert.IsTrue(mc.HasMapper<DateTime?>());
            Assert.IsTrue(mc.HasMapper<Guid?>());
            Assert.IsTrue(mc.HasMapper<char?>());
        }
    }
}
