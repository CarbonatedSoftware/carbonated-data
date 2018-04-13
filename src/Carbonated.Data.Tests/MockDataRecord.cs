using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carbonated.Data.Tests
{
    public class MockDataRecord : IDataRecord
    {
        // We need a list of KVPs because we need to able to access it by index.
        private readonly List<KeyValuePair<string, object>> fields = new List<KeyValuePair<string, object>>();

        public MockDataRecord()
        {
        }

        public MockDataRecord(params (string field, object value)[] fields)
        {
            this.fields.AddRange(fields.Select(((string field, object value) c) => new KeyValuePair<string, object>(c.field, c.value)));
        }

        public object this[int i] => fields[i].Value;
        public object this[string name] => fields.Single(f => f.Key == name).Value;
        public int FieldCount => fields.Count;
        public bool GetBoolean(int i) => (bool)fields[i].Value; //TODO: Why?
        public string GetName(int i) => fields[i].Key;
        public int GetOrdinal(string name) => fields.FindIndex(f => f.Key == name);
        public string GetString(int i) => fields[i].Value.ToString();
        public object GetValue(int i) => fields[i].Value;
        public bool IsDBNull(int i) => fields[i].Value == null || fields[i].Value == DBNull.Value;

        #region IDataRecord members that we don't need for testing
        public byte GetByte(int i) => throw new NotImplementedException();
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public char GetChar(int i) => throw new NotImplementedException();
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public IDataReader GetData(int i) => throw new NotImplementedException();
        public string GetDataTypeName(int i) => throw new NotImplementedException();
        public DateTime GetDateTime(int i) => throw new NotImplementedException();
        public decimal GetDecimal(int i) => throw new NotImplementedException();
        public double GetDouble(int i) => throw new NotImplementedException();
        public Type GetFieldType(int i) => throw new NotImplementedException();
        public float GetFloat(int i) => throw new NotImplementedException();
        public Guid GetGuid(int i) => throw new NotImplementedException();
        public short GetInt16(int i) => throw new NotImplementedException();
        public int GetInt32(int i) => throw new NotImplementedException();
        public long GetInt64(int i) => throw new NotImplementedException();
        public int GetValues(object[] values) => throw new NotImplementedException();
        #endregion
    }
}
