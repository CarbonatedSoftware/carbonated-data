using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Carbonated.Data
{
    /// <summary>
    /// Data access connector that automatically manages commands and connections for queries.
    /// </summary>
    public class DbConnector : DbContext
    {
        private readonly string connectionString;
        private readonly int commandTimeout;
        private readonly bool isContext;
        private DbObjectFactory dbFactory;
        private DbConnection contextConnection;

        public DbConnector(DbObjectFactory objectFactory, string connectionString, int commandTimeout = 300)
        {
            dbFactory = objectFactory;
            this.connectionString = connectionString;
            this.commandTimeout = commandTimeout;
        }

        /// <summary>
        /// Internal constructor used by the OpenContext call.
        /// </summary>
        internal DbConnector(DbObjectFactory objectFactory, DbConnection connection, int commandTimeout)
        {
            dbFactory = objectFactory;
            contextConnection = connection;
            this.commandTimeout = commandTimeout;
            isContext = true;
        }

        /// <summary>
        /// Collection of entity mappers which will be used by the entity Query methods.
        /// </summary>
        public MapperCollection Mappers { get; private set; } = new MapperCollection();

        /// <summary>
        /// The DbObjectFactory being used by the connector.
        /// </summary>
        public DbObjectFactory ObjectFactory => dbFactory;

        #region DbContext

        /// <summary>
        /// Opens a connection and returns a data context that will operate against that
        /// single connection until disposed. The context will use the same connection
        /// string and timeout settings from the parent connector.
        /// </summary>
        /// <returns>A disposable data context with an open connection.</returns>
        public DbContext OpenContext()
        {
            var cn = dbFactory.OpenConnection(connectionString);
            return new DbConnector(dbFactory, cn, commandTimeout);
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

        public int NonQuery(string sql, params (string name, object value)[] parameters) 
            => NonQuery(sql, dbFactory.CreateParameters(parameters));

        public int NonQuery(string sql, IEnumerable<DbParameter> parameters = null)
        {
            var cn = isContext ? contextConnection : dbFactory.OpenConnection(connectionString);
            try
            {
                using (var cmd = MakeCommand(sql, cn, parameters))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (!isContext)
                {
                    cn.Close();
                }
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters) 
            => Query<TEntity>(sql, dbFactory.CreateParameters(parameters));

        public IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
            => QueryReader<TEntity>(sql, parameters).ToList();

        public EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters) 
            => QueryReader<TEntity>(sql, dbFactory.CreateParameters(parameters));

        public EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
            => new EntityReader<TEntity>(QueryReader(sql, parameters), Mappers.Get<TEntity>());

        public DbDataReader QueryReader(string sql, params (string name, object value)[] parameters) 
            => QueryReader(sql, dbFactory.CreateParameters(parameters));

        public DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null)
        {
            var cn = isContext ? contextConnection : dbFactory.OpenConnection(connectionString);
            using (var cmd = MakeCommand(sql, cn, parameters))
            {
                // Set the behavior to close the connection when the reader is disposed if
                // we're not in a context.
                var behavior = isContext ? CommandBehavior.Default : CommandBehavior.CloseConnection;
                return cmd.ExecuteReader(behavior);
            }
        }

        public TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters) 
            => QueryScalar<TResult>(sql, dbFactory.CreateParameters(parameters));

        //TODO: We need to make sure that this converts framework value types in the same
        // way that the built-ins do for the entity mappers.
        public TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null) 
            => (TResult)Convert.ChangeType(QueryScalar(sql, parameters), typeof(TResult));

        public object QueryScalar(string sql, params (string name, object value)[] parameters) 
            => QueryScalar(sql, dbFactory.CreateParameters(parameters));

        public object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null)
        {
            var cn = isContext ? contextConnection : dbFactory.OpenConnection(connectionString);
            try
            {
                using (var cmd = MakeCommand(sql, cn, parameters))
                {
                    return cmd.ExecuteScalar();
                }
            }
            finally
            {
                if (!isContext)
                {
                    cn.Close();
                }
            }
        }

        public DataTable QueryTable(string sql, params (string name, object value)[] parameters) 
            => QueryTable(sql, dbFactory.CreateParameters(parameters));

        public DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters)
        {
            throw new NotImplementedException("Coming soon.");
        }

        public int SaveTable(DataTable table)
        {
            throw new NotImplementedException("Coming soon.");
        }

        /// <summary>
        /// Builds a simple command with the connection and parameters set.
        /// </summary>
        private DbCommand MakeCommand(string sql, DbConnection connection, IEnumerable<DbParameter> parameters)
        {
            var cmd = dbFactory.CreateCommand(sql, connection, commandTimeout);
            if (parameters?.Count() > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }
            return cmd;
        }
    }
}
