using System.Data;

namespace Carbonated.Data
{
    /// <summary>
    /// Maps and converts the first column of a record to a framework value type using 
    /// <see cref="Converter.ToType{T}(object)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value that will be returned. This is expected to be one of the
    /// framework value types, string, or byte[].</typeparam>
    public class ValueTypeMapper<T> : TypeMapper<T>
    {
        /// <summary>
        /// Constructs a Type Mapper and sets the creator function.
        /// </summary>
        public ValueTypeMapper() : base(ConvertValue) { }

        private static T ConvertValue(IDataRecord record) => Converter.ToType<T>(record.GetValue(0));
    }
}
