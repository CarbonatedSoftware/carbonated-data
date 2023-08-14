namespace Carbonated.Data;

/// <summary>
/// Options for the DbConnector.
/// </summary>
public class DbConnectorOptions
{
    /// <summary>
    /// The connection string to use for queries.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// The timeout for command execution in seconds.
    /// The default is 300.
    /// </summary>
    public int CommandTimeout { get; set; } = 300;

    /// <summary>
    /// Controls if table contraints are enforced during calls to QueryTable.
    /// The default is true.
    /// </summary>
    public bool EnforceConstraintsOnQueryTable { get; set; } = true;
}
