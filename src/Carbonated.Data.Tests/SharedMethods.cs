namespace Carbonated.Data.Tests
{
    public static class SharedMethods
    {
        public static Record Record(params (string field, object value)[] fields)
        {
            var record = new MockDataRecord(fields);
            var reader = new MockDataReader(record);
            return new Data.Record(reader);
        }

        public static string[] Strings(params string[] strings) => strings;
    }
}
