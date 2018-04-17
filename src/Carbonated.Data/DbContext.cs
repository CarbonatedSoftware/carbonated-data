using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Carbonated.Data
{
    /// <summary>
    /// Data access context with query methods.
    /// </summary>
    public interface DbContext : IDisposable
    {
        /// <summary>
        /// The engine-specific object factory to create Db objects.
        /// </summary>
        DbObjectFactory ObjectFactory { get; }

        /// <summary>
        /// Executes SQL without returning a result.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The number of rows affected.</returns>
        int NonQuery(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes SQL without returning a result.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The number of rows affected.</returns>
        int NonQuery(string sql, IEnumerable<DbParameter> parameters = null);


        /// <summary>
        /// Execute a SQL query and returns the results as a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the results entity.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A list of entities.</returns>
        IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Execute a SQL query and returns the results as a list of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the results entity.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A list of entities.</returns>
        IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);


        /// <summary>
        /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the results entity.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A typed reader.</returns>
        EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the results entity.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A typed reader.</returns>
        EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns the results in a DbDataReader.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A DbDataReader</returns>
        DbDataReader QueryReader(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes a SQL query and returns the results in a DbDataReader.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>A DbDataReader</returns>
        DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null);


        /// <summary>
        /// Executes a SQL query and returns the first row, first column of the result as the desired type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The scalar result.</returns>
        TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes a SQL query and returns the first row, first column of the result as the desired type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The scalar result.</returns>
        TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns the first row, first column of the result as an object.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The scalar result.</returns>
        object QueryScalar(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes a SQL query and returns the first row, first column of the result as an object.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The scalar result.</returns>
        object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null);


        /// <summary>
        /// Executes a SQL query and returns the result in a DataTable.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The data table result.</returns>
        DataTable QueryTable(string sql, params (string name, object value)[] parameters);

        /// <summary>
        /// Executes a SQL query and returns the result in a DataTable.
        /// </summary>
        /// <param name="sql">An ad hoc script or the name of a stored procedure to execute.</param>
        /// <param name="parameters">Parameters for the script, if any.</param>
        /// <returns>The data table result.</returns>
        DataTable QueryTable(string sql, IEnumerable<DbParameter> parameters);

        /// <summary>
        /// Saves the contents of a data table to the database. The table must have a primary key defined so
        /// that Insert, Update, and Delete commands can be generated.
        /// </summary>
        /// <param name="table">The table to save.</param>
        /// <returns>The number of records affected.</returns>
        /// <exception cref="Exception">Thrown if no primary key is defined.</exception>
        int SaveTable(DataTable table);
    }
}