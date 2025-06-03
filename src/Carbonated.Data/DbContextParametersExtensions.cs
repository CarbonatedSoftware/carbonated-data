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
    /// Executes a SQL query and returns the results as a list of entities, using an anonymous object for parameters.
    /// </summary>
    public static IEnumerable<TEntity> Query<TEntity>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.Query<TEntity>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the results in a reader that returns each row as an entity, using an anonymous
    /// object for parameters.
    /// </summary>
    public static EntityReader<TEntity> QueryReader<TEntity>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryReader<TEntity>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the results in a DbDataReader, using an anonymous object for parameters.
    /// </summary>
    public static DbDataReader QueryReader(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryReader(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as the desired type, using an
    /// anonymous object for parameters.
    /// </summary>
    public static TResult QueryScalar<TResult>(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryScalar<TResult>(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the first row, first column of the result as an object, using an anonymous
    /// object for parameters.
    /// </summary>
    public static object QueryScalar(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryScalar(sql, ps);
    }

    /// <summary>
    /// Executes a SQL query and returns the result in a DataTable, using an anonymous object for parameters.
    /// </summary>
    public static DataTable QueryTable(this DbContext context, string sql, object parameters)
    {
        var ps = ConvertPropertiesToParameters(context, parameters);
        return context.QueryTable(sql, ps);
    }

    private static List<DbParameter> ConvertPropertiesToParameters(DbContext context, object parameters)
    {
        List<DbParameter> ps = [];
        if (parameters != null)
        {
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (propertyType == typeof(DbParameter) || propertyType.IsSubclassOf(typeof(DbParameter)))
                {
                    var param = (DbParameter)property.GetValue(parameters);
                    if (param.ParameterName == ParamName_PropertyName)
                    {
                        param.ParameterName = "@" + property.Name;
                    }
                    ps.Add(param);
                }
                else
                {
                    ps.Add(context.ObjectFactory.CreateParameter("@" + property.Name, property.GetValue(parameters)));
                }
            }
        }

        return ps;
    }
}