using System;
using NUnit.Framework;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class MapperCollectionShould
    {
        [Test]
        public void TrackMappersThatHaveBeenAdded()
        {
            var mcol = new MapperCollection();
            mcol.Add(new PropertyMapper<Entity>());

            Assert.IsTrue(mcol.HasMapper<Entity>());
        }

        [Test]
        public void AutoGenerateAndAddPropertyMapperDuringGetIfNotYetRegistered()
        {
            var mcol = new MapperCollection();

            Assert.IsFalse(mcol.HasMapper<Entity>());

            var mapper = mcol.Get<Entity>();

            Assert.IsTrue(mcol.HasMapper<Entity>());
            Assert.IsInstanceOf<PropertyMapper<Entity>>(mapper);
        }

        [Test]
        public void ThrowOnAddIfTypeIsAlreadyRegistered()
        {
            var mcol = new MapperCollection();
            mcol.Add(new PropertyMapper<Entity>());

            Assert.Throws<MappingException>(() => mcol.Add(new PropertyMapper<Entity>()));
        }

        [Test]
        public void HaveFrameworkTypeMappersBuiltIn()
        {
            var mcol = new MapperCollection();

            Assert.IsTrue(mcol.HasMapper<bool>());
            Assert.IsTrue(mcol.HasMapper<byte>());
            Assert.IsTrue(mcol.HasMapper<short>());
            Assert.IsTrue(mcol.HasMapper<int>());
            Assert.IsTrue(mcol.HasMapper<long>());
            Assert.IsTrue(mcol.HasMapper<float>());
            Assert.IsTrue(mcol.HasMapper<double>());
            Assert.IsTrue(mcol.HasMapper<decimal>());
            Assert.IsTrue(mcol.HasMapper<DateTime>());
            Assert.IsTrue(mcol.HasMapper<Guid>());
            Assert.IsTrue(mcol.HasMapper<char>());
            Assert.IsTrue(mcol.HasMapper<string>());
            Assert.IsTrue(mcol.HasMapper<byte[]>());

            Assert.IsTrue(mcol.HasMapper<bool?>());
            Assert.IsTrue(mcol.HasMapper<byte?>());
            Assert.IsTrue(mcol.HasMapper<short?>());
            Assert.IsTrue(mcol.HasMapper<int?>());
            Assert.IsTrue(mcol.HasMapper<long?>());
            Assert.IsTrue(mcol.HasMapper<float?>());
            Assert.IsTrue(mcol.HasMapper<double?>());
            Assert.IsTrue(mcol.HasMapper<decimal?>());
            Assert.IsTrue(mcol.HasMapper<DateTime?>());
            Assert.IsTrue(mcol.HasMapper<Guid?>());
            Assert.IsTrue(mcol.HasMapper<char?>());
        }
    }
}
