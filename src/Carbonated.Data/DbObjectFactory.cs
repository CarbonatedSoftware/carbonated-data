using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Carbonated.Data
{
    /// <summary>
    /// Factory that creates data access objects, with overloads for common scenarios.
    /// Implementations will override the base methods to create engine-specific objects.
    /// </summary>
    public abstract class DbObjectFactory
    {
        /// <summary>
        /// Creates an empty DbCommand derivative.
        /// </summary>
        /// <returns>The command.</returns>
        public abstract DbCommand CreateCommand();

        /// <summary>
        /// Creates a DbConnection derivative for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The connection with connection string set.</returns>
        public abstract DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Creates a DbDataAdapter derivative.
        /// </summary>
        /// <returns>The data adapter.</returns>
        public abstract DbDataAdapter CreateDataAdapter();

        /// <summary>
        /// Creates a DbParameter derivative.
        /// </summary>
        /// <returns>The parameter.</returns>
        public abstract DbParameter CreateParameter();

        /// <summary>
        /// Creates a DbCommand, determining the type automatically from the SQL text.
        /// </summary>
        /// <param name="sql">The SQL command text.</param>
        /// <param name="connection">The connection that the command will run on.</param>
        /// <param name="timeout">The command timeout in seconds.</param>
        /// <returns>The constructed command.</returns>
        public DbCommand CreateCommand(string sql, DbConnection connection, int timeout)
        {
            CommandType type = IsAdHoc(sql) ? CommandType.Text : CommandType.StoredProcedure;
            return CreateCommand(sql, type, connection, timeout);
        }

        /// <summary>
        /// Creates a DbCommand.
        /// </summary>
        /// <param name="sql">The SQL command text.</param>
        /// <param name="type">The command type.</param>
        /// <param name="connection">The connection that the command will run on.</param>
        /// <param name="timeout">The command timeout in seconds.</param>
        /// <returns>The constructed command</returns>
        public DbCommand CreateCommand(string sql, CommandType type, DbConnection connection, int timeout)
        {
            var c = CreateCommand();
            c.CommandText = sql;
            c.CommandType = type;
            c.CommandTimeout = timeout;
            c.Connection = connection;
            return c;
        }
        
        /// <summary>
        /// Creates a DbDataAdapter with the select command set.
        /// </summary>
        /// <param name="selectCommand">The select command to run to fill the adapter.</param>
        /// <returns>The adapter.</returns>
        public DbDataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            var adapter = CreateDataAdapter();
            adapter.SelectCommand = selectCommand;
            return adapter;
        }

        /// <summary>
        /// Creates a DbParameter with the name and value specified.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The parameter with name and value set.</returns>
        public DbParameter CreateParameter(string name, object value)
        {
            var p = CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            return p;
        }

        /// <summary>
        /// Creates a list of DbParameters from the array of name/value tuples provided.
        /// </summary>
        /// <param name="parameters">The parameter names and values.</param>
        /// <returns>Enumerable list of parameters.</returns>
        public IEnumerable<DbParameter> CreateParameters((string name, object value)[] parameters) 
            => parameters?.Select(p => CreateParameter(p.name, p.value));

        /// <summary>
        /// Creates and opens a DbConnection for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An open connection.</returns>
        public DbConnection OpenConnection(string connectionString)
        {
            var cn = CreateConnection(connectionString);
            cn.Open();
            return cn;            
        }

        /// <summary>
        /// Returns true if there are interstitial spaces in the SQL text, indicating that it
        /// is an ad-hoc query, rather than a stored procedure name.
        /// </summary>
        private bool IsAdHoc(string sql) => sql.Trim().Contains(" ");
    }
}
