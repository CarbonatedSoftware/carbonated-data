using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests
{
    [TestFixture]
    public class ConnectorTests
    {
        private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True";
        private DbConnector connector;

        [SetUp]
        public void SetUp()
        {
            connector = new SqlServerDbConnector(TestConnectionString);
        }

        [Test]
        public void AdHocEntityQuery()
        {
            var cities = connector.Query<City>("select * from cities");

            Assert.AreEqual(12, cities.Count());
            Assert.AreEqual("New York", cities.First().Name);
        }

        [Test]
        public void ParameterizedAdHocQuery()
        {
            var city = connector.Query<City>("select * from cities where id = @id", ("@id", 6)).SingleOrDefault();

            Assert.AreEqual(6, city.Id);
            Assert.AreEqual("Phoenix", city.Name);
        }

        [Test]
        public void StoredProcQuery()
        {
            var cities = connector.Query<City>("GetCitiesByState", ("@state", "TX"));

            Assert.AreEqual(4, cities.Count());
            Assert.AreEqual("Houston", cities.First().Name);
        }

        [Test]
        public void ScalarQuery()
        {
            int count = connector.QueryScalar<int>("select count(*) from cities");

            Assert.AreEqual(12, count);
        }

        [Test]
        public void UpdateWithNonQuery()
        {
            // Confirm initial state
            City city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
            Assert.AreEqual(885400, city.Population);

            // Update then very new state
            connector.NonQuery("update cities set population = 50 where name = @name", ("@name", "Austin"));

            city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
            Assert.AreEqual(50, city.Population);

            // Reset to initial state and verify
            connector.NonQuery("update cities set population = 885400 where id = @id", ("@id", 11));

            city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
            Assert.AreEqual(885400, city.Population);
        }

        [Test]
        public void QueryReaderUntilExhausted()
        {
            var cities = new List<City>();

            EntityReader<City> reader;
            using (reader = connector.QueryReader<City>("select * from cities"))
            {
                foreach (var city in reader)
                {
                    cities.Add(city);
                }
            }

            Assert.IsTrue(reader.IsClosed);
            Assert.AreEqual(12, cities.Count);
        }

        [Test]
        public void QueryReaderPartially()
        {
            IEnumerable<City> cities;

            EntityReader<City> reader;
            using (reader = connector.QueryReader<City>("select * from cities order by id"))
            {
                cities = reader.TakeWhile(city => city.Population > 2_000_000).ToList();
            }

            Assert.IsTrue(reader.IsClosed);
            Assert.AreEqual(4, cities.Count());
        }

        [Test]
        public void QueryWithCustomMappings()
        {
            connector.Mappers.Add(new PropertyMapper<City>()
                .Map(x => x.Name, "nom"));

            var cities = connector.Query<City>("select id, name as nom from cities where state = 'TX'");

            Assert.AreEqual("Houston", cities.First().Name);
        }

        class City
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string State { get; set; }
            public int Population { get; set; }
        }
    }
}
