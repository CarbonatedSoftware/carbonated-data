using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class MapperCollectionShould
    {
        [Test]
        public void TrackMappersThatHaveBeenAdded()
        {
            var mc = new MapperCollection();
            mc.Add(new PropertyMapper<Entity>());

            Assert.IsTrue(mc.HasMapper<Entity>());
        }
    }
}
