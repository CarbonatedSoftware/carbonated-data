using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbonated.Data.Internals;
using Carbonated.Data.SqlServer.Tests.Models;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests
{
    internal class BulkSaveTest
    {
        private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True;Trust Server Certificate=True";
        private DbConnector connector;

        [SetUp]
        public void SetUp()
        {
            connector = new SqlServerDbConnector(TestConnectionString);
        }

        [Test]
        public void SaveUsingEntityDataReaderInPlaceOfTable()
        {
            // Initial state
            int count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
            Assert.That(count, Is.Zero);

            // Load as an entity
            var cities = new List<City>() { new() { Name = "Jacksonville", State = "FL", Population = 842583 } };

            BulkSave(cities, "cities", "insert into cities (name, state, population) select name, state, population from #temp_cities");

            // Verify that the row was added
            count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
            Assert.That(count, Is.EqualTo(1));

            // Restore test data state to starting point
            connector.NonQuery("delete from cities where state = 'FL'");
            count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
            Assert.That(count, Is.Zero);
        }

        public void BulkSave<TEntity>(IEnumerable<TEntity> entities, string table, string mergeScript)
        {
            //TODO: make sure table name is valid
            //TODO: make sure that there are entities present
            //TODO: get property mapper for entities

            if (connector.Mappers.Get<TEntity>() is not PropertyMapper<TEntity> mapper)
            {
                throw new Exception("We need a property mapper...");
            }
            var reader = new EntityDataReader<TEntity>(entities, mapper);

            string tempTable = $"#temp_{table}";

            var ctx = connector.OpenContext();

            ctx.NonQuery($"select top 0 * into {tempTable} from {table}");

            SqlBulkCopy sbc = new((SqlConnection)ctx.Connection);
            sbc.DestinationTableName = tempTable;
            // set mappings
            // set Bulk options
            sbc.WriteToServer(reader);

            ctx.NonQuery(mergeScript);
            //TODO: merge parmeters?
        }
    }
}
