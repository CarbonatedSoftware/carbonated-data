using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Carbonated.Data.SqlServer;

/// <summary>
/// Provides a set of extension methods for creating and configuring <see cref="DbParameter"/> instances for use with
/// SQL commands. These methods simplify the creation of parameters for various SQL data types, including strings,
/// dates, and table-valued parameters.
/// </summary>
/// <remarks>This class is meant to be used in conjunction with the DbContextParametersExtensions, which will provide
/// the paramater names based on the anonymous type property names. If you do not use them in that context, you will
/// need to set the <see cref="SqlParameter.ParameterName"/> property value on the returned parameter.</remarks>
public static class ParametersExtensions
{
    #region String Parameter Helpers

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Char"/> type and the specified size, using the
    /// provided string value as the parameter value.
    /// </summary>
    /// <remarks>This method is a convenience helper for creating SQL parameters with a fixed-length character
    /// type. The <paramref name="size"/> parameter determines the maximum length of the string that can be stored in
    /// the database.</remarks>
    /// <param name="value">The string value to assign to the parameter. Can be <see langword="null"/>.</param>
    /// <param name="size">The size of the parameter in characters. Must be a positive integer.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified value and size.</returns>
    public static DbParameter AsCharParam(this string value, int size)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Char, size)
        {
            Value = (object)value ?? DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.NChar"/> type and the specified size, using the
    /// provided string value as the parameter value.
    /// </summary>
    /// <remarks>This method is a convenience helper for creating SQL parameters with a fixed-length Unicode character
    /// type. The <paramref name="size"/> parameter determines the maximum length of the string that can be stored in
    /// the database.</remarks>
    /// <param name="value">The string value to assign to the parameter. Can be <see langword="null"/>.</param>
    /// <param name="size">The size of the parameter in characters. Must be a positive integer.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified value and size.</returns>
    public static DbParameter AsNCharParam(this string value, int size)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.NChar, size)
        {
            Value = (object)value ?? DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.VarChar"/> type and the specified size, using
    /// the provided string value as the parameter value.
    /// </summary>
    /// <remarks>This method is a convenience helper for creating SQL parameters with a variable-length character
    /// type. The <paramref name="size"/> parameter determines the maximum length of the string that can be stored in
    /// the database. The default size is 8000.</remarks>
    /// <param name="value">The string value to assign to the parameter. Can be <see langword="null"/>.</param>
    /// <param name="size">The maximum size of the parameter in characters. Optional; defaults to 8000.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified value and size.</returns>
    public static DbParameter AsVarCharParam(this string value, int size = 8000)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.VarChar, size)
        {
            Value = (object)value ?? DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.NVarChar"/> type and the specified size, using
    /// the provided string value as the parameter value.
    /// </summary>
    /// <remarks>This method is a convenience helper for creating SQL parameters with a variable-length Unicode
    /// character type. The <paramref name="size"/> parameter determines the maximum length of the string that can be
    /// stored in the database. The default size is 4000.</remarks>
    /// <param name="value">The string value to assign to the parameter. Can be <see langword="null"/>.</param>
    /// <param name="size">The maximum size of the parameter in characters. Optional; defaults to 4000.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified value and size.</returns>
    public static DbParameter AsNVarCharParam(this string value, int size = 4000)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.NVarChar, size)
        {
            Value = (object)value ?? DBNull.Value
        };
    }

    #endregion

    #region Date and Time Parameter Helpers

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Date"/> type using the provided
    /// <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> value to assign to the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified date value.</returns>
    public static DbParameter AsDateParam(this DateTime date)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Date)
        {
            Value = date
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Date"/> type using the provided
    /// <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> value to assign to the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified date value.</returns>
    public static DbParameter AsDateParam(this DateTime? date)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Date)
        {
            Value = date.HasValue ? date.Value : DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.DateTime2"/> type using the provided
    /// <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> value to assign to the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified DateTime2 value.</returns>
    public static DbParameter AsDateTime2Param(this DateTime date)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.DateTime2)
        {
            Value = date
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.DateTime2"/> type using the provided
    /// <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> value to assign to the parameter.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured with the specified DateTime2 value.</returns>
    public static DbParameter AsDateTime2Param(this DateTime? date)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.DateTime2)
        {
            Value = date.HasValue ? date.Value : DBNull.Value
        };
    }

    #endregion

    #region Table-Valued Parameter Helpers

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Structured"/> type for a table-valued
    /// parameter, using the provided value and table type name.
    /// </summary>
    /// <param name="value">The value to assign to the parameter. Typically a <see cref="DataTable"/> or an
    /// <see cref="IDataReader"/>.</param>
    /// <param name="tableValueTypeName">The name of the user-defined table type in SQL Server.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured as a table-valued parameter.</returns>
    public static DbParameter AsTableValueParam(this object value, string tableValueTypeName)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Structured)
        {
            TypeName = tableValueTypeName,
            Value = value ?? DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Structured"/> type for a table-valued parameter,
    /// using the provided <see cref="DataTable"/> and table type name.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> to assign to the parameter.</param>
    /// <param name="tableValueTypeName">The name of the user-defined table type in SQL Server.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured as a table-valued parameter.</returns>
    public static DbParameter AsTableValueParam(this DataTable table, string tableValueTypeName)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Structured)
        {
            TypeName = tableValueTypeName,
            Value = table != null ? table : DBNull.Value
        };
    }

    /// <summary>
    /// Creates a <see cref="SqlParameter"/> with a <see cref="SqlDbType.Structured"/> type for a table-valued
    /// parameter, using the provided <see cref="IDataReader"/> and table type name.
    /// </summary>
    /// <param name="reader">The <see cref="IDataReader"/> to assign to the parameter.</param>
    /// <param name="tableValueTypeName">The name of the user-defined table type in SQL Server.</param>
    /// <returns>A <see cref="DbParameter"/> instance configured as a table-valued parameter.</returns>
    public static DbParameter AsTableValueParam(this IDataReader reader, string tableValueTypeName)
    {
        return new SqlParameter(DbContextParametersExtensions.ParamName_PropertyName, SqlDbType.Structured)
        {
            TypeName = tableValueTypeName,
            Value = reader != null ? reader : DBNull.Value
        };
    }

    #endregion
}
