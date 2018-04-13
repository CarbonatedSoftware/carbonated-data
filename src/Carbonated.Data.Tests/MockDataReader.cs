using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carbonated.Data.Tests
{
    public class MockDataReader : IDataReader
    {
        private readonly IList<IDataRecord> records;
        private int recordIndex;

        public MockDataReader(IEnumerable<IDataRecord> records)
        {
            this.records = records.ToList();
            recordIndex = -1;
            CurrentRecord = this.records.Count > 0 ? this.records[0] : null; // Init current record so that Reader can work properly.
        }

        public MockDataReader(params IDataRecord[] records) : this(records.AsEnumerable()) { }

        public IDataRecord CurrentRecord { get; private set; }

        public bool Read()
        {
            if (++recordIndex < records.Count)
            {
                CurrentRecord = records[recordIndex];
                return true;
            }
            else
            {
                CurrentRecord = null;
                return false;
            }
        }

        #region IDataRecord members
        public object this[string name] => CurrentRecord[name];
        public object this[int i] => CurrentRecord[i];
        public int FieldCount => CurrentRecord.FieldCount;
        public string GetName(int i) => CurrentRecord.GetName(i);
        public string GetDataTypeName(int i) => CurrentRecord.GetDataTypeName(i);
        public Type GetFieldType(int i) => CurrentRecord.GetFieldType(i);
        public object GetValue(int i) => CurrentRecord.GetValue(i);
        public int GetValues(object[] values) => CurrentRecord.GetValues(values);
        public int GetOrdinal(string name) => CurrentRecord.GetOrdinal(name);
        public bool GetBoolean(int i) => CurrentRecord.GetBoolean(i);
        public byte GetByte(int i) => CurrentRecord.GetByte(i);
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => CurrentRecord.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        public char GetChar(int i) => CurrentRecord.GetChar(i);
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => CurrentRecord.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        public Guid GetGuid(int i) => CurrentRecord.GetGuid(i);
        public short GetInt16(int i) => CurrentRecord.GetInt16(i);
        public int GetInt32(int i) => CurrentRecord.GetInt32(i);
        public long GetInt64(int i) => CurrentRecord.GetInt64(i);
        public float GetFloat(int i) => CurrentRecord.GetFloat(i);
        public double GetDouble(int i) => CurrentRecord.GetDouble(i);
        public string GetString(int i) => CurrentRecord.GetString(i);
        public decimal GetDecimal(int i) => CurrentRecord.GetDecimal(i);
        public DateTime GetDateTime(int i) => CurrentRecord.GetDateTime(i);
        public IDataReader GetData(int i) => CurrentRecord.GetData(i);
        public bool IsDBNull(int i) => CurrentRecord.IsDBNull(i);
        #endregion

        #region IDataReader members that we don't care about for testing
        public int Depth { get { throw new NotImplementedException(); } }
        public bool IsClosed { get { throw new NotImplementedException(); } }
        public int RecordsAffected { get { throw new NotImplementedException(); } }
        public void Close() { throw new NotImplementedException(); }
        public DataTable GetSchemaTable() { throw new NotImplementedException(); }
        public bool NextResult() { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        #endregion

    }
}
