using NUnit.Framework;
using static Carbonated.Data.Tests.SharedMethods;

namespace Carbonated.Data.Tests
{
    [TestFixture]
    public class RecordShould
    {
        [Test]
        public void KnowWhichFieldsItContains()
        {
            var record = Record(("Foo", 1));

            Assert.IsTrue(record.HasField("Foo"));
            Assert.IsFalse(record.HasField("Oof"));
        }

        [Test]
        public void HaveCaseInsensitiveFieldNames()
        {
            var record = Record(("Foo", 1));

            Assert.IsTrue(record.HasField("foo"));
        }

        [Test]
        public void MapFieldNamesInBothExplicitAndNormalizedForm()
        {
            var record = Record(("Fo_o", 1));

            Assert.IsTrue(record.HasField("Fo_o"));
            Assert.IsTrue(record.HasField("foo"));
        }

        [Test]
        public void NotNormalizeFieldNamesWhenExplicitNamesWouldCollide()
        {
            var record = Record(("foo", 1), ("f_oo", 2));

            Assert.AreEqual(1, record.GetValue("foo"));
            Assert.AreEqual(2, record.GetValue("f_oo"));
        }

        [Test]
        public void NotNormalizeFieldNamesWhenNormalizedNamesWouldCollide()
        {
            var record = Record(("f_oo", 1), ("fo_o", 2));

            Assert.IsFalse(record.HasField("foo"));
            Assert.AreEqual(1, record.GetValue("f_oo"));
            Assert.AreEqual(2, record.GetValue("fo_o"));
        }

        [Test]
        public void ReturnNullFromGetValueWhenFieldIsNotFound()
        {
            var record = Record(("Foo", 1));

            Assert.IsNull(record.GetValue("bar"));
        }

        [Test]
        public void ReturnIndexOfFieldFromGetIndex()
        {
            var record = Record(("Foo", 1));

            Assert.AreEqual(0, record.GetIndex("foo"));
            Assert.AreEqual(-1, record.GetIndex("bar"));
        }
    }
}
