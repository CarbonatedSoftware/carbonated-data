using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Carbonated.Data.SqlServer;

/// <summary>
/// Provides helper methods for creating strongly-typed SQL Server parameters.
/// Meant for use with Tuple parameters and DbParameter lists.
/// </summary>
public static class ParametersHelper
{
    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with the specified name and value.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>A <see cref="DbParameter"/> instance.</returns>
    public static DbParameter Param(string name, object value)
        => new SqlParameter(name, value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> from a tuple containing the parameter name and value.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <returns>A <see cref="DbParameter"/> instance.</returns>
    public static DbParameter AsParam(this (string name, object value) param)
        => Param(param.name, param.value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with the specified name, value, and SQL data type.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <param name="type">The SQL data type of the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance.</returns>
    public static DbParameter Param(string name, object value, SqlDbType type)
        => new SqlParameter(name, value) { SqlDbType = type };

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> from a tuple and SQL data type.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <param name="type">The SQL data type of the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance.</returns>
    public static DbParameter AsParam(this (string name, object value) param, SqlDbType type)
        => Param(param.name, param.value, type);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>Date</c> type.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Date"/>.</returns>
    public static DbParameter DateParam(string name, object value)
        => Param(name, value, SqlDbType.Date);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>Date</c> type from a tuple.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Date"/>.</returns>
    public static DbParameter AsDateParam(this (string name, object value) param)
        => DateParam(param.name, param.value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>Time</c> type.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Time"/>.</returns>
    public static DbParameter TimeParam(string name, object value)
        => Param(name, value, SqlDbType.Time);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>Time</c> type from a tuple.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Time"/>.</returns>
    public static DbParameter AsTimeParam(this (string name, object value) param)
        => TimeParam(param.name, param.value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>DateTime2</c> type.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.DateTime2"/>.</returns>
    public static DbParameter DateTime2Param(string name, object value)
        => Param(name, value, SqlDbType.DateTime2);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>DateTime2</c> type from a tuple.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.DateTime2"/>.</returns>
    public static DbParameter AsDateTime2Param(this (string name, object value) param)
        => DateTime2Param(param.name, param.value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>DateTimeOffset</c> type.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.DateTimeOffset"/>.</returns>
    public static DbParameter DateTimeOffsetParam(string name, object value)
        => Param(name, value, SqlDbType.DateTimeOffset);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a SQL <c>DateTimeOffset</c> type from a tuple.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.DateTimeOffset"/>.</returns>
    public static DbParameter AsDateTimeOffsetParam(this (string name, object value) param)
        => DateTimeOffsetParam(param.name, param.value);

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a table-valued parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value (typically a DataTable or IEnumerable).</param>
    /// <param name="tableValueTypeName">The user-defined table type name in SQL Server.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Structured"/>.</returns>
    public static DbParameter TableValueParam(string name, object value, string tableValueTypeName)
        => new SqlParameter(name, value) { SqlDbType = SqlDbType.Structured, TypeName = tableValueTypeName };

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> for a table-valued parameter from a tuple.
    /// </summary>
    /// <param name="param">A tuple with the parameter name and value.</param>
    /// <param name="tableValueTypeName">The user-defined table type name in SQL Server.</param>
    /// <returns>A <see cref="DbParameter"/> instance with <see cref="SqlDbType.Structured"/>.</returns>
    public static DbParameter AsTableValueParam(this (string name, object value) param, string tableValueTypeName)
        => TableValueParam(param.name, param.value, tableValueTypeName);
}
