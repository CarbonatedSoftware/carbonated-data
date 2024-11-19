using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Carbonated.Data;

/// <summary>
/// Wraps a data record with get-by-name methods and fuzzy-name matching.
/// </summary>
public class Record : IDataRecord
{
    private static readonly Regex DisallowedChars = new(@"[\W_]", RegexOptions.Compiled);

    private readonly IDataReader dataReader;
    private readonly Dictionary<string, int> fieldMappings;

    /// <summary>
    /// Constructs a record that wraps the specified data reader.
    /// </summary>
    /// <param name="dataReader">The source data reader that will be read from.</param>
    public Record(IDataReader dataReader)
    {
        this.dataReader = dataReader;
        fieldMappings = new Dictionary<string, int>(dataReader.FieldCount, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            string name = dataReader.GetName(i).ToLowerInvariant();
            fieldMappings[name] = i;
        }

        var normalized = fieldMappings
            .Select(fm => (field: fm.Key, normal: GetNormalizedName(fm.Key), index: fm.Value))
            .Where(m => m.field != m.normal)
            .GroupBy(m => m.normal)
            .Where(group => group.Count() == 1 && !fieldMappings.ContainsKey(group.Key));

        foreach (var group in normalized)
        {
            fieldMappings.Add(group.Key, group.First().index);
        }
    }

    /// <summary>
    /// Returns a value that indicates if the record contains a field.
    /// </summary>
    /// <param name="name">The field name to check for.</param>
    /// <returns>true if the field is found; otherwise, false.</returns>
    public bool HasField(string name) => fieldMappings.ContainsKey(name);

    internal int GetIndex(string name) => fieldMappings.TryGetValue(name, out int index) ? index : -1;

    /// <summary>
    /// Gets the value of the specified field, if it exists. The name will be matched exactly if possible,
    /// falling back on fuzzy matching if needed. If the name cannot be found, null will be returned.
    /// </summary>
    /// <param name="name">The field name to get.</param>
    /// <returns>The value of the field if it can be found; otherwise, null.</returns>
    public object GetValue(string name)
    {
        if (HasField(name))
        {
            return this[GetIndex(name)];
        }

        string normal = GetNormalizedName(name);
        if (HasField(normal))
        {
            return this[GetIndex(normal)];
        }

        return null;
    }

    /// <summary>
    /// Returns the normalized form of a name, with all non-alphanumeric characters removed.
    /// </summary>
    /// <param name="name">The name to normalize.</param>
    /// <returns>The normalized name.</returns>
    public static string GetNormalizedName(string name) => DisallowedChars.Replace(name, "").ToLowerInvariant();

    /// <summary>
    /// Gets the column with the specified name.
    /// </summary>
    /// <param name="name">The name of the column to find.</param>
    /// <returns>The column with the specified name as an System.Object.</returns>
    public object this[string name] => dataReader[name];

    /// <summary>
    /// Gets the column located at the specified index.
    /// </summary>
    /// <param name="i">The zero-based index of the column to get.</param>
    /// <returns>The column located at the specified index as an System.Object.</returns>
    public object this[int i] => dataReader[i];

    /// <summary>
    /// Return whether the specified field is set to null.
    /// </summary>
    /// <param name="name">The name of the field to find.</param>
    /// <returns>true if the specified field is set to null; otherwise, false.</returns>
    public bool IsDBNull(string name) => dataReader.IsDBNull(GetIndex(name));

    #region Name-based getters

    /* ----------------------------------------------------------------
     * Name-based getters for the data types supported by IDataRecord are provided so
     * that TypeMapper and other custom data access consumers can easily get values by
     * name. There are also ..OrDefault() variants provided for all supported getters to
     * simplify that common pattern.
     */

    /// <summary>
    /// Gets the value of the specified column as a Boolean.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public bool GetBoolean(string name) => dataReader.GetBoolean(GetIndex(name));

    /// <summary>
    /// Gets the 8-bit unsigned integer value of the specified column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public byte GetByte(string name) => dataReader.GetByte(GetIndex(name));

    /// <summary>
    /// Gets the character value of the specified column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public char GetChar(string name) => dataReader.GetChar(GetIndex(name));

    /// <summary>
    /// Gets the date and time data value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public DateTime GetDateTime(string name) => dataReader.GetDateTime(GetIndex(name));

    /// <summary>
    /// Gets the fixed-position numeric value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public decimal GetDecimal(string name) => dataReader.GetDecimal(GetIndex(name));

    /// <summary>
    /// Gets the double-precision floating point number of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public double GetDouble(string name) => dataReader.GetDouble(GetIndex(name));

    /// <summary>
    /// Gets the single-precision floating point number of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public float GetFloat(string name) => dataReader.GetFloat(GetIndex(name));

    /// <summary>
    /// Returns the GUID value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public Guid GetGuid(string name) => dataReader.GetGuid(GetIndex(name));

    /// <summary>
    /// Gets the 16-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public short GetInt16(string name) => dataReader.GetInt16(GetIndex(name));

    /// <summary>
    /// Gets the 32-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public int GetInt32(string name) => dataReader.GetInt32(GetIndex(name));

    /// <summary>
    /// Gets the 64-bit signed integer value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public long GetInt64(string name) => dataReader.GetInt64(GetIndex(name));

    /// <summary>
    /// Gets the string value of the specified field.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public string GetString(string name) => dataReader.GetString(GetIndex(name));


    /// <summary>
    /// Gets the value of the specified column as a Boolean. If the column is null, the type default will
    /// be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public bool GetBooleanOrDefault(string name) => GetValueOrDefault(dataReader.GetBoolean, name);

    /// <summary>
    /// Gets the value of the specified column as a Boolean. If the column is null, the type default will
    /// be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public bool GetBooleanOrDefault(string name, bool fallback) => GetValueOrDefault(dataReader.GetBoolean, name, fallback);

    /// <summary>
    /// Gets the 8-bit unsigned integer value of the specified column. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public byte GetByteOrDefault(string name) => GetValueOrDefault(dataReader.GetByte, name);

    /// <summary>
    /// Gets the 8-bit unsigned integer value of the specified column. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public byte GetByteOrDefault(string name, byte fallback) => GetValueOrDefault(dataReader.GetByte, name, fallback);

    /// <summary>
    /// Gets the character value of the specified column. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public char GetCharOrDefault(string name) => GetValueOrDefault(dataReader.GetChar, name);

    /// <summary>
    /// Gets the character value of the specified column. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public char GetCharOrDefault(string name, char fallback) => GetValueOrDefault(dataReader.GetChar, name, fallback);

    /// <summary>
    /// Gets the date and time data value of the specified field. If the column is null, the type default
    /// will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public DateTime GetDateTimeOrDefault(string name) => GetValueOrDefault(dataReader.GetDateTime, name);

    /// <summary>
    /// Gets the date and time data value of the specified field. If the column is null, the type default
    /// will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public DateTime GetDateTimeOrDefault(string name, DateTime fallback) => GetValueOrDefault(dataReader.GetDateTime, name, fallback);

    /// <summary>
    /// Gets the fixed-position numeric value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public decimal GetDecimalOrDefault(string name) => GetValueOrDefault(dataReader.GetDecimal, name);

    /// <summary>
    /// Gets the fixed-position numeric value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public decimal GetDecimalOrDefault(string name, decimal fallback) => GetValueOrDefault(dataReader.GetDecimal, name, fallback);

    /// <summary>
    /// Gets the double-precision floating point number of the specified field. If the column is null, the
    /// type default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public double GetDoubleOrDefault(string name) => GetValueOrDefault(dataReader.GetDouble, name);

    /// <summary>
    /// Gets the double-precision floating point number of the specified field. If the column is null, the
    /// type default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public double GetDoubleOrDefault(string name, double fallback) => GetValueOrDefault(dataReader.GetDouble, name, fallback);

    /// <summary>
    /// Gets the single-precision floating point number of the specified field. If the column is null,
    /// the type default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public float GetFloatOrDefault(string name) => GetValueOrDefault(dataReader.GetFloat, name);

    /// <summary>
    /// Gets the single-precision floating point number of the specified field. If the column is null,
    /// the type default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public float GetFloatOrDefault(string name, float fallback) => GetValueOrDefault(dataReader.GetFloat, name, fallback);

    /// <summary>
    /// Returns the GUID value of the specified field. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public Guid GetGuidOrDefault(string name) => GetValueOrDefault(dataReader.GetGuid, name);

    /// <summary>
    /// Returns the GUID value of the specified field. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public Guid GetGuidOrDefault(string name, Guid fallback) => GetValueOrDefault(dataReader.GetGuid, name, fallback);

    /// <summary>
    /// Gets the 16-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public short GetInt16OrDefault(string name) => GetValueOrDefault(dataReader.GetInt16, name);

    /// <summary>
    /// Gets the 16-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public short GetInt16OrDefault(string name, short fallback) => GetValueOrDefault(dataReader.GetInt16, name, fallback);

    /// <summary>
    /// Gets the 32-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public int GetInt32OrDefault(string name) => GetValueOrDefault(dataReader.GetInt32, name);

    /// <summary>
    /// Gets the 32-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public int GetInt32OrDefault(string name, int fallback) => GetValueOrDefault(dataReader.GetInt32, name, fallback);

    /// <summary>
    /// Gets the 64-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public long GetInt64OrDefault(string name) => GetValueOrDefault(dataReader.GetInt64, name);

    /// <summary>
    /// Gets the 64-bit signed integer value of the specified field. If the column is null, the type
    /// default will be returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public long GetInt64OrDefault(string name, long fallback) => GetValueOrDefault(dataReader.GetInt64, name, fallback);

    /// <summary>
    /// Gets the string value of the specified field. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the column.</returns>
    public string GetStringOrDefault(string name) => GetValueOrDefault(dataReader.GetString, name);

    /// <summary>
    /// Gets the string value of the specified field. If the column is null, the type default will be
    /// returned instead.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="fallback">The fallback value to use by default if the field is missing or null.</param>
    /// <returns>The value of the column.</returns>
    public string GetStringOrDefault(string name, string fallback) => GetValueOrDefault(dataReader.GetString, name, fallback);


    private T GetValueOrDefault<T>(Func<int, T> getterFunction, string name, T fallback = default)
    {
        return HasField(name) && !IsDBNull(name) ? getterFunction(GetIndex(name)) : fallback;
    }

    #endregion

    #region IDataRecord explicit implementation

    /*-----------------------------------------------------------------
     * IDataRecord is explicitly implemented so that it is available to consuming code
     * if needed by casting the Record to IDataRecord.
     */

    object IDataRecord.this[int i] => dataReader[i];

    object IDataRecord.this[string name] => dataReader[name];

    int IDataRecord.FieldCount => dataReader.FieldCount;

    bool IDataRecord.GetBoolean(int i) => dataReader.GetBoolean(i);

    byte IDataRecord.GetByte(int i) => dataReader.GetByte(i);

    long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    char IDataRecord.GetChar(int i) => dataReader.GetChar(i);

    long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    IDataReader IDataRecord.GetData(int i) => dataReader.GetData(i);

    string IDataRecord.GetDataTypeName(int i) => dataReader.GetDataTypeName(i);

    DateTime IDataRecord.GetDateTime(int i) => dataReader.GetDateTime(i);

    decimal IDataRecord.GetDecimal(int i) => dataReader.GetDecimal(i);

    double IDataRecord.GetDouble(int i) => dataReader.GetDouble(i);

    Type IDataRecord.GetFieldType(int i) => dataReader.GetFieldType(i);

    float IDataRecord.GetFloat(int i) => dataReader.GetFloat(i);

    Guid IDataRecord.GetGuid(int i) => dataReader.GetGuid(i);

    short IDataRecord.GetInt16(int i) => dataReader.GetInt16(i);

    int IDataRecord.GetInt32(int i) => dataReader.GetInt32(i);

    long IDataRecord.GetInt64(int i) => dataReader.GetInt64(i);

    string IDataRecord.GetName(int i) => dataReader.GetName(i);

    int IDataRecord.GetOrdinal(string name) => dataReader.GetOrdinal(name);

    string IDataRecord.GetString(int i) => dataReader.GetString(i);

    object IDataRecord.GetValue(int i) => dataReader.GetValue(i);

    int IDataRecord.GetValues(object[] values) => dataReader.GetValues(values);

    bool IDataRecord.IsDBNull(int i) => dataReader.IsDBNull(i);

    #endregion
}
