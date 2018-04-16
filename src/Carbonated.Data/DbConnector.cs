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
    public class DbConnector
    {
        private readonly DbObjectFactory dbObjectFactory;
        private readonly string connectionString;
        private readonly int commandTimeout;

        public DbConnector(DbObjectFactory objectFactory, string connectionString, int commandTimeout = 300)
        {
            dbObjectFactory = objectFactory;
            this.connectionString = connectionString;
            this.commandTimeout = commandTimeout;
        }

        public MapperCollection Mappers { get; private set; } = new MapperCollection();

        public int NonQuery(string sql, params (string name, object value)[] parameters) 
            => NonQuery(sql, dbObjectFactory.CreateParameters(parameters));

        public int NonQuery(string sql, IEnumerable<DbParameter> parameters = null)
        {
            using (var cn = dbObjectFactory.OpenConnection(connectionString))
            using (var cmd = MakeCommand(sql, cn, parameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters) 
            => Query<TEntity>(sql, dbObjectFactory.CreateParameters(parameters));

        public IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
            => QueryReader<TEntity>(sql, parameters).ToList();


        public EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters) 
            => QueryReader<TEntity>(sql, dbObjectFactory.CreateParameters(parameters));

        public EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null) 
            => new EntityReader<TEntity>(QueryReader(sql, parameters), Mappers.Get<TEntity>());

        public DbDataReader QueryReader(string sql, params (string name, object value)[] parameters) 
            => QueryReader(sql, dbObjectFactory.CreateParameters(parameters));

        public DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null)
        {
            using (var cn = dbObjectFactory.OpenConnection(connectionString))
            using (var cmd = MakeCommand(sql, cn, parameters))
            {
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters) 
            => QueryScalar<TResult>(sql, dbObjectFactory.CreateParameters(parameters));

        //TODO: We need to make sure that this converts framework value types in the same
        // way that the built-ins do for the entity mappers.
        public TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null) 
            => (TResult)Convert.ChangeType(QueryScalar(sql, parameters), typeof(TResult));

        public object QueryScalar(string sql, params (string name, object value)[] parameters) 
            => QueryScalar(sql, dbObjectFactory.CreateParameters(parameters));

        public object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null)
        {
            using (var cn = dbObjectFactory.OpenConnection(connectionString))
            using (var cmd = MakeCommand(sql, cn, parameters))
            {
                return cmd.ExecuteScalar();
            }
        }

        public DataTable QueryTable(string sql, params (string name, object value)[] parameters) 
            => QueryTable(sql, dbObjectFactory.CreateParameters(parameters));

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
            var cmd = dbObjectFactory.CreateCommand(sql, connection, commandTimeout);
            if (parameters?.Count() > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }
            return cmd;
        }
    }
}
