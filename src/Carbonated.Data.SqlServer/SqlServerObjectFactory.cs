using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Carbonated.Data.SqlServer;

/// <summary>
/// Factory that creates data access objects for SQL Server.
/// </summary>
public class SqlServerObjectFactory : DbObjectFactory
{
    /// <summary>
    /// Creates a SqlCommand.
    /// </summary>
    /// <returns>The command.</returns>
    public override DbCommand CreateCommand() => new SqlCommand();

    /// <summary>
    /// Creates a SqlConnection for the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The connection with the connection string set.</returns>
    public override DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

    /// <summary>
    /// Creates a SqlDataAdapter.
    /// </summary>
    /// <returns>The data adapter.</returns>
    public override DbDataAdapter CreateDataAdapter() => new SqlDataAdapter();

    /// <summary>
    /// Creates a SqlParameter.
    /// </summary>
    /// <returns>The parameter.</returns>
    public override DbParameter CreateParameter() => new SqlParameter();

    /// <summary>
    /// Delimits an identifier with brackets, if it is not already delimited.
    /// Delimiting an identifier allows SQL keywords to be used as column or other
    /// object names in queries.
    /// </summary>
    /// <param name="identifier">The identifier name to delimit.</param>
    /// <returns>The identifier in delimited form.</returns>
    public override string DelimitIdentifier(string identifier)
    {
        if (identifier.StartsWith("[") && identifier.EndsWith("]"))
            return identifier;
        return $"[{identifier}]";
    }
}
