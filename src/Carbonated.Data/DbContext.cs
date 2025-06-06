﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Carbonated.Data;

/// <summary>
/// Data access context with query methods.
/// </summary>
public interface DbContext : IDisposable
{
    /// <summary>
    /// The connection string in use by the context.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// The underlying connection in use by the context.
    /// </summary>
    DbConnection Connection { get; }

    /// <summary>
    /// The transaction in use by the context, if there is one.
    /// </summary>
    DbTransaction Transaction { get; }

    /// <summary>
    /// The engine-specific object factory to create Db objects.
    /// </summary>
    DbObjectFactory ObjectFactory { get; }

    /// <summary>
    /// Rolls back the active transaction, if there is one.
    /// </summary>
    void Rollback();

    /// <summary>
    /// Executes a non-query SQL command against the database using the specified SQL string and parameters.
    /// </summary>
    /// <remarks>This method is used to execute SQL commands that do not return results, such as INSERT, UPDATE, or
    /// DELETE statements.</remarks>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the SQL command, if any.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    int NonQuery(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a non-query SQL command against the database using the specified SQL string and parameters.
    /// </summary>
    /// <remarks>This method is used to execute SQL commands that do not return results, such as INSERT, UPDATE, or
    /// DELETE statements.</remarks>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the SQL command, if any.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    int NonQuery(string sql, IEnumerable<DbParameter> parameters = null);


    /// <summary>
    /// Execute a SQL query and returns an enumerable collection of entities of the specified type.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the entities returned by the query.</returns>
    IEnumerable<TEntity> Query<TEntity>(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Execute a SQL query and returns an enumerable collection of entities of the specified type.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the entities returned by the query.</returns>
    IEnumerable<TEntity> Query<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);


    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>A reader returning entities of <typeparamref name="TEntity"/>.</returns>
    EntityReader<TEntity> QueryReader<TEntity>(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>A reader returning entities of <typeparamref name="TEntity"/>.</returns>
    EntityReader<TEntity> QueryReader<TEntity>(string sql, IEnumerable<DbParameter> parameters = null);

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>A <see cref="DbDataReader"/> with the query results.</returns>
    DbDataReader QueryReader(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>A <see cref="DbDataReader"/> with the query results.</returns>
    DbDataReader QueryReader(string sql, IEnumerable<DbParameter> parameters = null);


    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>The scalar <typeparamref name="TResult"/> result.</returns>
    TResult QueryScalar<TResult>(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>The scalar <typeparamref name="TResult"/> result.</returns>
    TResult QueryScalar<TResult>(string sql, IEnumerable<DbParameter> parameters = null);

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>The scalar result.</returns>
    object QueryScalar(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>The scalar result.</returns>
    object QueryScalar(string sql, IEnumerable<DbParameter> parameters = null);


    /// <summary>
    /// Executes a SQL query and returns the result in a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
    /// <returns>The data table result.</returns>
    DataTable QueryTable(string sql, params (string name, object value)[] parameters);

    /// <summary>
    /// Executes a SQL query and returns the result in a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">Parameters for the query, if any.</param>
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