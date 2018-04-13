using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        #region IDataRecord explicit implementation - we want to hide this away

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
