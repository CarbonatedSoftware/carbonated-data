using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Carbonated.Data.Internals;

namespace Carbonated.Data;

/// <summary>
/// Data access connector that automatically manages commands and connections for queries.
/// </summary>
public class DbConnector : DbContext
{
    private readonly DbConnectorOptions options;
    private readonly bool isContext;
    private DbObjectFactory dbFactory;
    private DbConnection contextConnection;

    /// <summary>
    /// Constructs a DbConnector.
    /// </summary>
    /// <param name="objectFactory">The engine-specific object factory to use.</param>
    /// <param name="connectionString">The connection string to use for queries.</param>
    /// <param name="commandTimeout">The default command timeout for commands.</param>
    public DbConnector(DbObjectFactory objectFactory, string connectionString, int commandTimeout = 300)
        : this(objectFactory, new DbConnectorOptions() { ConnectionString = connectionString, CommandTimeout = commandTimeout })
    {
    }

    /// <summary>
    /// Constructs a DbConnector.
    /// </summary>
    /// <param name="objectFactory">The engine-specific object factory to use.</param>
    /// <param name="options">The options to use for queries and connector behavior.</param>
    public DbConnector(DbObjectFactory objectFactory, DbConnectorOptions options)
    {
        dbFactory = objectFactory;
        this.options = options;
    }

    /// <summary>
    /// Internal constructor used by the OpenContext call.
    /// </summary>
    internal DbConnector(DbObjectFactory objectFactory, DbConnection connection, DbConnectorOptions options)
    {
        dbFactory = objectFactory;
        contextConnection = connection;
        this.options = options;
        isContext = true;
    }

    /// <summary>
    /// Collection of entity mappers which will be used by the entity Query methods.
    /// </summary>
    public MapperCollection Mappers { get; private set; } = new MapperCollection();

    /// <summary>
    /// The connection string in use by the connector.
    /// </summary>
    public string ConnectionString => options.ConnectionString;

    /// <summary>
    /// The DbObjectFactory being used by the connector.
    /// </summary>
    public DbObjectFactory ObjectFactory => dbFactory;


    #region DbContext

    DbConnection DbContext.Connection => contextConnection;

    /// <summary>
    /// Opens a connection and returns a data context that will operate against that
    /// single connection until disposed. The context will use the same connection
    /// string and timeout settings from the parent connector.
    /// </summary>
    /// <returns>A disposable data context with an open connection.</returns>
    public DbContext OpenContext()
    {
        var cn = dbFactory.OpenConnection(options.ConnectionString);
        return new DbConnector(dbFactory, cn, options);
    }

    /// <summary>
    /// Disposes and invalidates the context connection, if there is one. This will have
    /// no effect if the connector was not created with an OpenContext call.
    /// </summary>
    public void Dispose()
    {
        if (isContext)
        {
            contextConnection?.Close();
            contextConnection = null;
            dbFactory = null; // Null the factory so that using a disposed context will throw.
        }
    }

    #endregion


    /// <summary>
    /// Executes SQL without returning a result.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The number of rows affected.</returns>
    public int NonQuery(string sql, params (string name, object value)[] parameters) 
        => NonQuery(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes SQL without returning a result.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The number of rows affected.</returns>
    public int NonQuery(string sql, IEnumerable<DbParameter> parameters = null)
    {
        var cn = isContext ? contextConnection : dbFactory.OpenConnection(options.ConnectionString);
        try
        {
            using var cmd = MakeCommand(sql, cn, parameters);
            return cmd.ExecuteNonQuery();
        }
        finally
        {
            if (!isContext)
            {
                cn.Close();
            }
        }
    }


    /// <summary>
    /// Execute a SQL query and returns the results as a list of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the results entity.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A list of entities.</returns>
    public IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters) 
        => Query<TEntity>(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Execute a SQL query and returns the results as a list of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the results entity.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A list of entities.</returns>
    public IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
        => QueryReader<TEntity>(sql, parameters).ToList();


    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the results entity.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A typed reader.</returns>
    public EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters) 
        => QueryReader<TEntity>(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the results entity.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A typed reader.</returns>
    public EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
        => new EntityReader<TEntity>(QueryReader(sql, parameters), Mappers.Get<TEntity>());

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A DbDataReader</returns>
    public DbDataReader QueryReader(string sql, params (string name, object value)[] parameters) 
        => QueryReader(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>A DbDataReader</returns>
    public DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null)
    {
        var cn = isContext ? contextConnection : dbFactory.OpenConnection(options.ConnectionString);
        using var cmd = MakeCommand(sql, cn, parameters);
        // Set the behavior to close the connection when the reader is disposed if we're not in a context.
        var behavior = isContext ? CommandBehavior.Default : CommandBehavior.CloseConnection;
        return cmd.ExecuteReader(behavior);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the desired type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The scalar result.</returns>
    public TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters) 
        => QueryScalar<TResult>(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the desired type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The scalar result.</returns>
    public TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null)
    {
        object result = QueryScalar(sql, parameters);
        var conv = Mappers.GetValueConverter<TResult>();
        return conv != null 
            ? (TResult)conv.Convert(result) 
            : Converter.ToType<TResult>(result);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The scalar result.</returns>
    public object QueryScalar(string sql, params (string name, object value)[] parameters) 
        => QueryScalar(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The scalar result.</returns>
    public object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null)
    {
        var cn = isContext ? contextConnection : dbFactory.OpenConnection(options.ConnectionString);
        try
        {
            using var cmd = MakeCommand(sql, cn, parameters);
            return cmd.ExecuteScalar();
        }
        finally
        {
            if (!isContext)
            {
                cn.Close();
            }
        }
    }


    /// <summary>
    /// Executes a SQL query and returns the result in a DataTable.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The data table result.</returns>
    public DataTable QueryTable(string sql, params (string name, object value)[] parameters) 
        => QueryTable(sql, dbFactory.CreateParameters(parameters));

    /// <summary>
    /// Executes a SQL query and returns the result in a DataTable.
    /// </summary>
    /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the script, if any.</param>
    /// <returns>The data table result.</returns>
    public DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters)
    {
        var cn = isContext ? contextConnection : dbFactory.OpenConnection(options.ConnectionString);
        try
        {
            using var cmd = MakeCommand(sql, cn, parameters);
            using var adapter = dbFactory.CreateDataAdapter(cmd);
            var set = new DataSet() { EnforceConstraints = options.EnforceConstraintsOnQueryTable };
            adapter.FillSchema(set, SchemaType.Source);
            adapter.Fill(set);

            return set.Tables[0];
        }
        finally
        {
            if (!isContext)
            {
                cn.Close();
            }
        }
    }

    /// <summary>
    /// Saves the contents of a data table to the database. The table must have a primary key defined so
    /// that Insert, Update, and Delete commands can be generated.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <returns>The number of records affected.</returns>
    /// <exception cref="Exception">Thrown if no primary key is defined.</exception>
    public int SaveTable(DataTable table)
    {
        if (table.PrimaryKey == null || table.PrimaryKey.Length ==0)
        {
            throw new Exception($"Table cannot be saved without primary key: {table.TableName}");
        }

        var cn = isContext ? contextConnection : dbFactory.OpenConnection(options.ConnectionString);
        try
        {
            var sam = new SaveAdapterMaker(dbFactory, options.CommandTimeout);
            using var adapter = sam.MakeAdapter(table, cn);
            return adapter.Update(table);
        }
        finally
        {
            if (!isContext)
            {
                cn.Close();
            }
        }
    }

    /// <summary>
    /// Creates an Entity Data Reader that will allow the collection of entities to be read as
    /// if they were a data record. The entity type must configured with a Property Mapper in
    /// order for this to work.
    /// </summary>
    /// <typeparam name="TEntity">The type of the results entity.</typeparam>
    /// <param name="entities">The collection of entities to read.</param>
    /// <returns>An EntityDataReader for the entity collection.</returns>
    /// <exception cref="Exception">Thrown if the entity mapper is not a ProperyMapper.</exception>
    public EntityDataReader<TEntity> CreateEntityReader<TEntity>(IEnumerable<TEntity> entities)
    {
        if (Mappers.Get<TEntity>() is not PropertyMapper<TEntity> mapper)
        {
            throw new Exception($"Entity Data Reader requires a Property Mapper for the entity: {typeof(TEntity).Name}");
        }
        return new EntityDataReader<TEntity>(entities, mapper);
    }

    /// <summary>
    /// Builds a simple command with the connection and parameters set.
    /// </summary>
    private DbCommand MakeCommand(string sql, DbConnection connection, IEnumerable<DbParameter> parameters)
    {
        var cmd = dbFactory.CreateCommand(sql, connection, options.CommandTimeout);
        if (parameters?.Count() > 0)
        {
            cmd.Parameters.AddRange(parameters.ToArray());
        }
        return cmd;
    }
}
