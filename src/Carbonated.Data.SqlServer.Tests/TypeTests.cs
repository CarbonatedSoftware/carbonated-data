using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests
{
    [TestFixture]
    public class TypeTests
    {
        private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True";
        private DbConnector connector;

        [SetUp]
        public void SetUp()
        {
            connector = new SqlServerDbConnector(TestConnectionString);
        }

        [Test]
        public void BoolTest()
        {
            var values = connector.Query<bool>("select [bool] from type_test order by id");

            CollectionAssert.AreEqual(new bool[] { false, false, true }, values);
        }

        [Test]
        public void ByteTest()
        {
            var values = connector.Query<byte>("select [byte] from type_test order by id");

            CollectionAssert.AreEqual(new byte[] { 0, 0, 1 }, values);
        }

        [Test]
        public void ShortTest()
        {
            var values = connector.Query<short>("select [short] from type_test order by id");

            CollectionAssert.AreEqual(new short[] { 0, 0, 2 }, values);
        }

        [Test]
        public void IntTest()
        {
            var values = connector.Query<int>("select [int] from type_test order by id");

            CollectionAssert.AreEqual(new int[] { 0, 0, 3 }, values);
        }

        [Test]
        public void LongTest()
        {
            var values = connector.Query<long>("select [long] from type_test order by id");

            CollectionAssert.AreEqual(new long[] { 0, 0, 5 }, values);
        }

        [Test]
        public void FloatTest()
        {
            var values = connector.Query<float>("select [float] from type_test order by id");

            CollectionAssert.AreEqual(new float[] { 0.0f, 0.0f, 8.13f }, values);
        }

        [Test]
        public void DoubleTest()
        {
            var values = connector.Query<double>("select [double] from type_test order by id");

            CollectionAssert.AreEqual(new double[] { 0.0, 0.0, 21.34 }, values);
        }

        [Test]
        public void DecimalTest()
        {
            var values = connector.Query<decimal>("select [decimal] from type_test order by id");

            CollectionAssert.AreEqual(new decimal[] { 0.0m, 0.0m, 55.89m }, values);
        }

        [Test]
        public void DateTimeTest()
        {
            var values = connector.Query<DateTime>("select [datetime] from type_test order by id");

            var expected = new DateTime[]
            {
                DateTime.MinValue,
                new DateTime(2018, 4, 2, 13, 14, 15),
                new DateTime(2018, 4, 2, 13, 14, 15)
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void GuidAsStringTest()
        {
            var values = connector.Query<Guid>("select [guid_as_string] from type_test order by id");

            var expected = new Guid[]
            {
                Guid.Empty,
                Guid.Empty,
                new Guid("7ca43d156e8749dfbaffdb241d0d494c")
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void GuidAsUniqueIdTest()
        {
            var values = connector.Query<Guid>("select [guid_as_uniqueid] from type_test order by id");

            var expected = new Guid[]
            {
                Guid.Empty,
                Guid.Empty,
                new Guid("7ca43d156e8749dfbaffdb241d0d494c")
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void CharTest()
        {
            var values = connector.Query<char>("select [char] from type_test order by id");

            CollectionAssert.AreEqual(new char[] { '\0', '\0', 'c' }, values);
        }

        [Test]
        public void StringTest()
        {
            var values = connector.Query<string>("select [string] from type_test order by id");

            CollectionAssert.AreEqual(new string[] { null, string.Empty, "str" }, values);
        }

        [Test]
        public void ByteArrayTest()
        {
            var values = connector.Query<byte[]>("select [byte_array] from type_test order by id");

            var expected = new byte[][]
            {
                null,
                null,
                new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }
            };
            CollectionAssert.AreEqual(expected, values);
        }
    }
}
