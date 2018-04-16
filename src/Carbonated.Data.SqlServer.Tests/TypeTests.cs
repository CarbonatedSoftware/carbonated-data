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
        public void QueryBool()
        {
            var values = connector.Query<bool>("select [bool] from type_test order by id");

            CollectionAssert.AreEqual(new bool[] { false, false, true }, values);
        }

        [Test]
        public void QueryByte()
        {
            var values = connector.Query<byte>("select [byte] from type_test order by id");

            CollectionAssert.AreEqual(new byte[] { 0, 0, 1 }, values);
        }

        [Test]
        public void QueryShort()
        {
            var values = connector.Query<short>("select [short] from type_test order by id");

            CollectionAssert.AreEqual(new short[] { 0, 0, 2 }, values);
        }

        [Test]
        public void QueryInt()
        {
            var values = connector.Query<int>("select [int] from type_test order by id");

            CollectionAssert.AreEqual(new int[] { 0, 0, 3 }, values);
        }

        [Test]
        public void QueryLong()
        {
            var values = connector.Query<long>("select [long] from type_test order by id");

            CollectionAssert.AreEqual(new long[] { 0, 0, 5 }, values);
        }

        [Test]
        public void QueryFloat()
        {
            var values = connector.Query<float>("select [float] from type_test order by id");

            CollectionAssert.AreEqual(new float[] { 0.0f, 0.0f, 8.13f }, values);
        }

        [Test]
        public void QueryDouble()
        {
            var values = connector.Query<double>("select [double] from type_test order by id");

            CollectionAssert.AreEqual(new double[] { 0.0, 0.0, 21.34 }, values);
        }

        [Test]
        public void QueryDecimal()
        {
            var values = connector.Query<decimal>("select [decimal] from type_test order by id");

            CollectionAssert.AreEqual(new decimal[] { 0.0m, 0.0m, 55.89m }, values);
        }

        [Test]
        public void QueryDateTime()
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
        public void QueryGuidStoredAsString()
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
        public void QueryGuidStoredAsUniqueIdentifier()
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
        public void QueryChar()
        {
            var values = connector.Query<char>("select [char] from type_test order by id");

            CollectionAssert.AreEqual(new char[] { '\0', '\0', 'c' }, values);
        }

        [Test]
        public void QueryString()
        {
            var values = connector.Query<string>("select [string] from type_test order by id");

            CollectionAssert.AreEqual(new string[] { null, string.Empty, "str" }, values);
        }

        [Test]
        public void QueryByteArray()
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

        #region Nullable types

        [Test]
        public void QueryNullableBool()
        {
            var values = connector.Query<bool?>("select [bool] from type_test order by id");

            CollectionAssert.AreEqual(new bool?[] { null, false, true }, values);
        }

        [Test]
        public void QueryNullableByte()
        {
            var values = connector.Query<byte?>("select [byte] from type_test order by id");

            CollectionAssert.AreEqual(new byte?[] { null, 0, 1 }, values);
        }

        [Test]
        public void QueryNullableShort()
        {
            var values = connector.Query<short?>("select [short] from type_test order by id");

            CollectionAssert.AreEqual(new short?[] { null, 0, 2 }, values);
        }

        [Test]
        public void QueryNullableInt()
        {
            var values = connector.Query<int>("select [int] from type_test order by id");

            CollectionAssert.AreEqual(new int[] { 0, 0, 3 }, values);
        }

        [Test]
        public void QueryNullableLong()
        {
            var values = connector.Query<long?>("select [long] from type_test order by id");

            CollectionAssert.AreEqual(new long?[] { null, 0, 5 }, values);
        }

        [Test]
        public void QueryNullableFloat()
        {
            var values = connector.Query<float?>("select [float] from type_test order by id");

            CollectionAssert.AreEqual(new float?[] { null, 0.0f, 8.13f }, values);
        }

        [Test]
        public void QueryNullableDouble()
        {
            var values = connector.Query<double?>("select [double] from type_test order by id");

            CollectionAssert.AreEqual(new double?[] { null, 0.0, 21.34 }, values);
        }

        [Test]
        public void QueryNullableDecimal()
        {
            var values = connector.Query<decimal?>("select [decimal] from type_test order by id");

            CollectionAssert.AreEqual(new decimal?[] { null, 0.0m, 55.89m }, values);
        }

        [Test]
        public void QueryNullableDateTime()
        {
            var values = connector.Query<DateTime?>("select [datetime] from type_test order by id");

            var expected = new DateTime?[]
            {
                null,
                new DateTime(2018, 4, 2, 13, 14, 15),
                new DateTime(2018, 4, 2, 13, 14, 15)
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void QueryNullableGuidStoredAsString()
        {
            var values = connector.Query<Guid?>("select [guid_as_string] from type_test order by id");

            var expected = new Guid?[]
            {
                null,
                null,
                new Guid("7ca43d156e8749dfbaffdb241d0d494c")
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void QueryNullableGuidStoredAsUniqueIdentifier()
        {
            var values = connector.Query<Guid?>("select [guid_as_uniqueid] from type_test order by id");

            var expected = new Guid?[]
            {
                null,
                null,
                new Guid("7ca43d156e8749dfbaffdb241d0d494c")
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void QueryNullableChar()
        {
            var values = connector.Query<char?>("select [char] from type_test order by id");

            CollectionAssert.AreEqual(new char?[] { null, null, 'c' }, values);
        }

        #endregion
    }
}
