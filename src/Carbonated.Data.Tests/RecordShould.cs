using System;
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
        public void GetValueWhenNonNormalizedNameIsPassed()
        {
            var record = Record(("foo", 1));

            Assert.AreEqual(1, record.GetValue("f_oo"));
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

        [Test]
        public void GetTypesByName()
        {
            var record = Record(("bool", true), ("byte", (byte)1), ("char", 'a'));

            Assert.AreEqual(true, record.GetBoolean("bool"));
            Assert.AreEqual(1, record.GetByte("byte"));
            Assert.AreEqual('a', record.GetChar("char"));
        }

        [Test]
        public void GetTypeOrDefaultByName()
        {
            var record = Record(("bool1", true), ("bool2", DBNull.Value), ("dec1", 42m), ("dec2", DBNull.Value));

            Assert.IsTrue(record.GetBooleanOrDefault("bool1"));
            Assert.IsFalse(record.GetBooleanOrDefault("bool2"));
            Assert.IsFalse(record.GetBooleanOrDefault("missing"));

            Assert.AreEqual(42, record.GetDecimalOrDefault("dec1"));
            Assert.AreEqual(0, record.GetDecimalOrDefault("dec2"));
            Assert.AreEqual(0, record.GetDecimalOrDefault("missing"));
        }

        [Test]
        public void GetTypeOrFallbackByName()
        {
            var record = Record(("int1", 42), ("int2", DBNull.Value));

            Assert.AreEqual(42, record.GetInt32OrDefault("int1"));

            Assert.AreEqual(0, record.GetInt32OrDefault("int2"));
            Assert.AreEqual(76, record.GetInt32OrDefault("int2", 76));

            Assert.AreEqual(0, record.GetInt32OrDefault("missing"));
            Assert.AreEqual(99, record.GetInt32OrDefault("missing", 99));
        }
    }
}
