using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Carbonated.Data.Internals;

namespace Carbonated.Data;

/// <summary>
/// Iterates over an enumerable collection of an entity, exposing its properties as data reader fields.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to read.</typeparam>
public sealed class EntityDataReader<TEntity> : IDataRecord, IDataReader
{
    private readonly IList<TEntity> entities;
    private readonly PropertyMapper<TEntity> propertyMapper;
    private readonly IList<PropertyMapInfo> propertyMappings;
    private readonly IDictionary<string, int> fieldMappings;

    private readonly int recordCount;
    private int recordIndex = -1;
    private bool isClosed = false;

    /// <summary>
    /// Creates an Entity Data Reader that will allow a collection of entities to be read using
    /// a data reader.
    /// </summary>
    /// <param name="entities">The collection of entities to read.</param>
    /// <param name="propertyMapper">The mapper between propertys and data fields.</param>
    public EntityDataReader(IEnumerable<TEntity> entities, PropertyMapper<TEntity> propertyMapper)
    {
        this.entities = entities.ToList();
        this.propertyMapper = propertyMapper;

        recordCount = this.entities.Count;

        propertyMappings = this.propertyMapper.Mappings.Where(m => !m.IsIgnored || m.IgnoreBehavior == IgnoreBehavior.OnLoad).ToList();
        fieldMappings = new Dictionary<string, int>(propertyMappings.Count);
        for (int i = 0; i < propertyMappings.Count; i++)
        {
            fieldMappings[propertyMappings[i].Field] = i;
        }
    }

    #region IDataReader methods

    /// <inheritdoc/>
    public int Depth => 0;

    /// <inheritdoc/>
    public bool IsClosed => isClosed;

    /// <inheritdoc/>
    public int RecordsAffected => -1;

    /// <inheritdoc/>
    public void Close() => isClosed = true;

    /// <inheritdoc/>
    public void Dispose() => Close();

    /// <inheritdoc/>
    public DataTable GetSchemaTable() => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool NextResult() => false;

    /// <inheritdoc/>
    public bool Read() => ++recordIndex < recordCount;

    #endregion

    #region IDataRecord methods

    /// <inheritdoc/>
    public object GetValue(int i)
    {
        var propMap = propertyMappings[i];
        object value = propMap.Property.GetValue(entities[recordIndex]);
        return propMap.ToDbConverter != null ? propMap.ToDbConverter(value) : value;
    }

    /// <inheritdoc/>
    public object this[int i] => GetValue(i);

    /// <inheritdoc/>
    public object this[string name] => GetValue(GetOrdinal(name));

    /// <inheritdoc/>
    /// <inheritdoc/>
    public int FieldCount => fieldMappings.Count;

    /// <inheritdoc/>
    public bool GetBoolean(int i) => (bool)GetValue(i);

    /// <inheritdoc/>
    public byte GetByte(int i) => (byte)GetValue(i);

    /// <inheritdoc/>
    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotImplementedException();

    /// <inheritdoc/>
    public char GetChar(int i) => (char)GetValue(i);

    /// <inheritdoc/>
    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IDataReader GetData(int i) => throw new NotImplementedException();

    /// <inheritdoc/>
    public string GetDataTypeName(int i) => GetFieldType(i).Name;

    /// <inheritdoc/>
    public DateTime GetDateTime(int i) => (DateTime)GetValue(i);

    /// <inheritdoc/>
    public decimal GetDecimal(int i) => (decimal)GetValue(i);

    /// <inheritdoc/>
    public double GetDouble(int i) => (double)GetValue(i);

    /// <inheritdoc/>
    public Type GetFieldType(int i) => propertyMappings[i].Property.PropertyType;

    /// <inheritdoc/>
    public float GetFloat(int i) => (float)GetValue(i);

    /// <inheritdoc/>
    public Guid GetGuid(int i) => (Guid)GetValue(i);

    /// <inheritdoc/>
    public short GetInt16(int i) => (short)GetValue(i);

    /// <inheritdoc/>
    public int GetInt32(int i) => (int)GetValue(i);

    /// <inheritdoc/>
    public long GetInt64(int i) => (long)GetValue(i);

    /// <inheritdoc/>
    public string GetName(int i) => propertyMappings[i].Field;

    /// <inheritdoc/>
    public int GetOrdinal(string name) => fieldMappings[name];

    /// <inheritdoc/>
    public string GetString(int i) => GetValue(i)?.ToString();

    /// <inheritdoc/>
    public int GetValues(object[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = GetValue(i);
        }
        return values.Length;
    }

    /// <inheritdoc/>
    public bool IsDBNull(int i) => GetValue(i) == null;

    #endregion
}
