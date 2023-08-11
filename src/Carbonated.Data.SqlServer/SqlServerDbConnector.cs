namespace Carbonated.Data.SqlServer;

/// <summary>
/// SQL Server data access connector.
/// </summary>
public class SqlServerDbConnector : DbConnector
{
    /// <summary>
    /// Creates a new SQL Server connector with a <see cref="SqlServerObjectFactory"/>.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="commandTimeout">The command timeout for queries.</param>
    public SqlServerDbConnector(string connectionString, int commandTimeout = 300)
        : base(new SqlServerObjectFactory(), connectionString, commandTimeout) { }

    /// <summary>
    /// Creates a new SQL Server connector with a <see cref="SqlServerObjectFactory"/>.
    /// </summary>
    /// <param name="objectFactory">Instance of a SQL Server or derivative object factory.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="commandTimeout">The command timeout for queries.</param>
    public SqlServerDbConnector(SqlServerObjectFactory objectFactory, string connectionString, int commandTimeout = 300)
        : base(objectFactory, connectionString, commandTimeout) { }
}
