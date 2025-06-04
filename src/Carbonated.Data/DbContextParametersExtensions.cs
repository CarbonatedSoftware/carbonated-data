using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Carbonated.Data;

/// <summary>
/// Provides extension methods for executing SQL queries on a <see cref="DbContext"/> using anonymous objects for
/// parameters, where the property names of the anonymous object are used as parameter names.
/// </summary>
public static class DbContextParametersExtensions
{
    /// <summary>
    /// Special parameter name used to indicate that the property name should be used as the parameter name.
    /// </summary>
    public const string ParamName_PropertyName = "__property_name";

    /// <summary>
    /// Executes a non-query SQL command against the database using the specified SQL string and parameters.
    /// </summary>
    /// <remarks>This method is an extension method for <see cref="DbContext"/> and is used to execute SQL commands that
    /// do not return results, such as INSERT, UPDATE, or DELETE statements.</remarks>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the command.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL command. The properties of
    /// the object will be converted into SQL parameters.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    public static int NonQuery(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.NonQuery(sql, ps);
    }

    /// <summary>
    /// Execute a SQL query and returns an enumerable collection of entities of the specified type.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the entities returned by the query.</returns>
    public static IEnumerable<TEntity> Query<TEntity>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.Query<TEntity>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity.
    /// </summary>
    /// <remarks>The SQL query must return a result set that matches the structure of the specified
    /// <typeparamref name="TEntity"/>. Property mappings and custom conversions can be configured on the
    /// <see cref="DbConnector.Mappers"/> collection.</remarks>
    /// <typeparam name="TEntity">The type of the entities to be returned.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>A reader returning entities of <typeparamref name="TEntity"/>.</returns>
    public static EntityReader<TEntity> QueryReader<TEntity>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryReader<TEntity>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>A <see cref="DbDataReader"/> with the query results.</returns>
    public static DbDataReader QueryReader(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryReader(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>The scalar <typeparamref name="TResult"/> result.</returns>
    public static TResult QueryScalar<TResult>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryScalar<TResult>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>The scalar result.</returns>
    public static object QueryScalar(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryScalar(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the result in a <see cref="DataTable"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance used to execute the query.</param>
    /// <param name="sql">An ad hoc query or the name of a stored procedure to execute.</param>
    /// <param name="parameters">An object containing the parameters to be passed to the SQL query. The properties of
    /// this object will be converted into query parameters.</param>
    /// <returns>The data table result.</returns>
    public static DataTable QueryTable(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryTable(sql, ps);
    }

    private static List<DbParameter> ConvertPropertiesToParameters(DbContext context, object parameters)
    {
        if (parameters == null)
        {
            return [];
        }

        List<DbParameter> dbParams = [];
        var properties = parameters.GetType().GetProperties();
        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            if (propertyType == typeof(DbParameter) || propertyType.IsSubclassOf(typeof(DbParameter)))
            {
                var param = (DbParameter)property.GetValue(parameters);
                if (string.IsNullOrEmpty(param.ParameterName) || param.ParameterName == ParamName_PropertyName)
                {
                    param.ParameterName = "@" + property.Name;
                }
                dbParams.Add(param);
            }
            else
            {
                dbParams.Add(context.ObjectFactory.CreateParameter("@" + property.Name, property.GetValue(parameters)));
            }
        }
        return dbParams;
    }
}