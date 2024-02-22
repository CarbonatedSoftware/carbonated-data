using System;
using System.Collections.Generic;
using System.Linq;
using Carbonated.Data.SqlServer.Tests.Models;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

[TestFixture]
public class ConnectorTests
{
    private const string TestConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarbonatedTest;Integrated Security=True;Trust Server Certificate=True";
    private DbConnector connector;

    [SetUp]
    public void SetUp()
    {
        connector = new SqlServerDbConnector(TestConnectionString);
    }

    [TearDown]
    public void TearDown()
    {
        connector.Dispose();
    }

    [Test]
    public void AdHocEntityQuery()
    {
        var cities = connector.Query<City>("select * from cities");

        Assert.Multiple(() =>
        {
            Assert.That(cities.Count(), Is.EqualTo(12));
            Assert.That(cities.First().Name, Is.EqualTo("New York"));
        });
    }

    [Test]
    public void ParameterizedAdHocQuery()
    {
        var city = connector.Query<City>("select * from cities where id = @id", ("@id", 6)).SingleOrDefault();

        Assert.Multiple(() =>
        {
            Assert.That(city.Id, Is.EqualTo(6));
            Assert.That(city.Name, Is.EqualTo("Phoenix"));
        });
    }

    [Test]
    public void StoredProcQuery()
    {
        var cities = connector.Query<City>("GetCitiesByState", ("@state", "TX"));

        Assert.Multiple(() =>
        {
            Assert.That(cities.Count(), Is.EqualTo(4));
            Assert.That(cities.First().Name, Is.EqualTo("Houston"));
        });
    }

    [Test]
    public void ScalarQuery()
    {
        int count = connector.QueryScalar<int>("select count(*) from cities");

        Assert.That(count, Is.EqualTo(12));
    }

    [Test]
    public void UpdateWithNonQuery()
    {
        // Confirm initial state
        City city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
        Assert.That(city.Population, Is.EqualTo(885400));

        // Update then very new state
        connector.NonQuery("update cities set population = 50 where name = @name", ("@name", "Austin"));

        city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
        Assert.That(city.Population, Is.EqualTo(50));

        // Reset to initial state and verify
        connector.NonQuery("update cities set population = 885400 where id = @id", ("@id", 11));

        city = connector.Query<City>("select * from cities where id = @id", ("@id", 11)).Single();
        Assert.That(city.Population, Is.EqualTo(885400));
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

        Assert.Multiple(() =>
        {
            Assert.That(reader.IsClosed, Is.True);
            Assert.That(cities, Has.Count.EqualTo(12));
        });
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

        Assert.Multiple(() =>
        {
            Assert.That(reader.IsClosed, Is.True);
            Assert.That(cities.Count(), Is.EqualTo(4));
        });
    }

    [Test]
    public void QueryWithCustomMappings()
    {
        connector.Mappers.Configure<City>()
            .Map(x => x.Name, "nom");

        var cities = connector.Query<City>("select id, name as nom from cities where state = 'TX'");

        Assert.That(cities.First().Name, Is.EqualTo("Houston"));
    }

    [Test]
    public void NullParameterValuesAreConvertedToDbNull()
    {
        var p = connector.ObjectFactory.CreateParameter("@test", null);
        Assert.That(p.Value, Is.EqualTo(DBNull.Value));
    }

    [Test]
    public void QueryAndSaveTable()
    {
        // Check initial state
        int count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
        Assert.That(count, Is.EqualTo(0));

        // Load the table and add a row
        var cities = connector.QueryTable("select * from cities");
        cities.TableName = "cities";

        var row = cities.NewRow();
        row["id"] = 13;
        row["name"] = "Jacksonville";
        row["state"] = "FL";
        row["population"] = 842583;
        cities.Rows.Add(row);

        connector.SaveTable(cities);

        // Verify that the row was added
        var flCities = connector.QueryTable("select * from cities where state = 'FL'");
        flCities.TableName = "cities";
        Assert.That(flCities.Rows, Has.Count.EqualTo(1));

        // Restore test data state to starting point
        flCities.Rows[0].Delete();
        connector.SaveTable(flCities);

        // Verify that we're back to our starting point
        count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
        Assert.That(count, Is.EqualTo(0));
    }
}
