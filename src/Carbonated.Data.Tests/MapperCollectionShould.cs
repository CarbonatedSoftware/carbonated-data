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

        //TODO: system value types should be pre-registered
    }
}
