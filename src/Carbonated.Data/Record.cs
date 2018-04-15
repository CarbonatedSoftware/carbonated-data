using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Carbonated.Data
{
    public class Record : IDataRecord
    {
        private static readonly Regex DisallowedChars = new Regex(@"[\W_]", RegexOptions.Compiled);

        private readonly IDataReader dataReader;
        private readonly IDictionary<string, int> fieldMappings;

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

        public bool HasField(string name) => fieldMappings.ContainsKey(name);

        internal int GetIndex(string name) => fieldMappings.ContainsKey(name) ? fieldMappings[name] : -1;

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

        public static string GetNormalizedName(string name) => DisallowedChars.Replace(name, "").ToLowerInvariant();


        public object this[string name] => dataReader[name];

        public object this[int i] => dataReader[i];

        public bool IsDBNull(string name) => dataReader.IsDBNull(GetIndex(name));

        #region Name-based getters

        /* ----------------------------------------------------------------
         * Name-based getters for the data types supported by IDataRecord are provided so
         * that TypeMapper and other custom data access consumers can easily get values by
         * name. There are also ..OrDefault() variants provided for all supported getters to
         * simplify that common pattern.
         */

        public bool GetBoolean(string name) => dataReader.GetBoolean(GetIndex(name));

        public byte GetByte(string name) => dataReader.GetByte(GetIndex(name));

        public char GetChar(string name) => dataReader.GetChar(GetIndex(name));

        public DateTime GetDateTime(string name) => dataReader.GetDateTime(GetIndex(name));

        public decimal GetDecimal(string name) => dataReader.GetDecimal(GetIndex(name));

        public double GetDouble(string name) => dataReader.GetDouble(GetIndex(name));

        public float GetFloat(string name) => dataReader.GetFloat(GetIndex(name));

        public Guid GetGuid(string name) => dataReader.GetGuid(GetIndex(name));

        public short GetInt16(string name) => dataReader.GetInt16(GetIndex(name));

        public int GetInt32(string name) => dataReader.GetInt32(GetIndex(name));

        public long GetInt64(string name) => dataReader.GetInt64(GetIndex(name));

        public string GetString(string name) => dataReader.GetString(GetIndex(name));


        public bool GetBooleanOrDefault(string name) => GetValueOrDefault(dataReader.GetBoolean, name);

        public byte GetByteOrDefault(string name) => GetValueOrDefault(dataReader.GetByte, name);

        public char GetCharOrDefault(string name) => GetValueOrDefault(dataReader.GetChar, name);

        public DateTime GetDateTimeOrDefault(string name) => GetValueOrDefault(dataReader.GetDateTime, name);

        public decimal GetDecimalOrDefault(string name) => GetValueOrDefault(dataReader.GetDecimal, name);

        public double GetDoubleOrDefault(string name) => GetValueOrDefault(dataReader.GetDouble, name);

        public float GetFloatOrDefault(string name) => GetValueOrDefault(dataReader.GetFloat, name);

        public Guid GetGuidOrDefault(string name) => GetValueOrDefault(dataReader.GetGuid, name);

        public short GetInt16OrDefault(string name) => GetValueOrDefault(dataReader.GetInt16, name);

        public int GetInt32OrDefault(string name) => GetValueOrDefault(dataReader.GetInt32, name);

        public long GetInt64OrDefault(string name) => GetValueOrDefault(dataReader.GetInt64, name);

        public string GetStringOrDefault(string name) => GetValueOrDefault(dataReader.GetString, name);


        private T GetValueOrDefault<T>(Func<int, T> getterFunction, string name)
        {
            return HasField(name) && !IsDBNull(name) ? getterFunction(GetIndex(name)) : default(T);
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
}
