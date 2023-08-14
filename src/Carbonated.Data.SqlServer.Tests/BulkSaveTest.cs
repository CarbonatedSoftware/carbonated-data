using System.Collections.Generic;
using System.Data;
using Carbonated.Data.SqlServer.Tests.Models;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Carbonated.Data.SqlServer.Tests;

/// <summary>
/// Demonstrates that the EntityDataReader can be used by SqlBulkCopy for a bulk save process.
/// </summary>
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

        var reader = connector.CreateEntityReader(cities);

        BulkSave(reader, "cities", "insert into cities (name, state, population) select name, state, population from #temp_cities");

        // Verify that the row was added
        count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
        Assert.That(count, Is.EqualTo(1));

        // Restore test data state to starting point
        connector.NonQuery("delete from cities where state = 'FL'");
        count = connector.QueryScalar<int>("select count(*) from cities where state = 'FL'");
        Assert.That(count, Is.Zero);
    }

    private void BulkSave(IDataReader reader, string table, string mergeScript)
    {
        string tempTable = $"#temp_{table}";

        using var ctx = connector.OpenContext();

        ctx.NonQuery($"select top 0 * into {tempTable} from {table}");

        SqlBulkCopy sbc = new((SqlConnection)ctx.Connection);
        sbc.DestinationTableName = tempTable;
        sbc.WriteToServer(reader);

        ctx.NonQuery(mergeScript);
    }
}
