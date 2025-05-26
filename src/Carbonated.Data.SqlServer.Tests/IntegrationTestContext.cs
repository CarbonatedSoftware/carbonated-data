using Microsoft.Extensions.Configuration;

namespace Carbonated.Data.SqlServer.Tests;

internal class IntegrationTestContext
{
    static IntegrationTestContext()
    {
        TestConnectionString = new ConfigurationManager()
            .AddUserSecrets<IntegrationTestContext>()
            .Build()
            .GetConnectionString("IntegrationTestDb");
    }

    public static string TestConnectionString { get; private set; }
}
