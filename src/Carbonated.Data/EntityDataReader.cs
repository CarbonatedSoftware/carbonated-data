using Carbonated.Data.Internals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carbonated.Data
{
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

        //TODO: continue implementation

        public EntityDataReader(IEnumerable<TEntity> entities, PropertyMapper<TEntity> propertyMapper)
        {
            this.entities = entities.ToList();
            this.propertyMapper = propertyMapper;

            recordCount = this.entities.Count;

            propertyMappings = this.propertyMapper.Mappings.ToList();
            fieldMappings = new Dictionary<string, int>(propertyMappings.Count);
            for (int i = 0; i < propertyMappings.Count; i++)
            {
                fieldMappings[propertyMappings[i].Field] = i;
            }

            //TODO: we might need a way to set the Ordinal of mapped fields.
        }

        #region IDataReader methods

        /// <inheritdoc/>
        public int Depth => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsClosed => isClosed;

        /// <inheritdoc/>
        public int RecordsAffected => -1;

        /// <inheritdoc/>
        public void Close() => isClosed = true;

        /// <inheritdoc/>
        public void Dispose() => Close();

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool NextResult() => false;

        /// <inheritdoc/>
        public bool Read() => ++recordIndex < recordCount;

        #endregion

        #region IDataRecord methods

        /// <inheritdoc/>
        public object this[int i] => GetValue(i);

        /// <inheritdoc/>
        public object this[string name] => this[GetOrdinal(name)];

        /// <inheritdoc/>
        public int GetOrdinal(string name) => fieldMappings[name];

        /// <inheritdoc/>
        public object GetValue(int i)
        {
            var propMap = propertyMappings[i];
            object value = propMap.Property.GetValue(entities[recordIndex]);
            return propMap.ToStorageConverter != null ? propMap.ToStorageConverter(value) : value;
        }

        /// <inheritdoc/>
        public int FieldCount => fieldMappings.Count;

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
