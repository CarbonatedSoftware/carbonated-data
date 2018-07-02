using System;

namespace Carbonated.Data
{
    /// <summary>
    /// Base for value converters.
    /// </summary>
    public abstract class ValueConverter
    {
        private readonly Func<object, object> conversion;

        /// <summary>
        /// Creates a value converter for a type using the provided conversion method.
        /// </summary>
        /// <param name="type">The type being converted to.</param>
        /// <param name="conversion">The conversion to use.</param>
        public ValueConverter(Type type, Func<object, object> conversion)
        {
            ValueType = type;
            this.conversion = conversion;
        }

        /// <summary>
        /// The Type that this converter will produce.
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// Executes the converter for a value.
        /// </summary>
        /// <param name="input">The value to convert.</param>
        /// <returns>The value converted into the target type.</returns>
        public object Convert(object input) => conversion(input);
    }

    /// <summary>
    /// Converts a value from its base storage type to the desired type.
    /// </summary>
    /// <typeparam name="T">The type being converted to.</typeparam>
    public class ValueConverter<T> : ValueConverter
    {
        /// <summary>
        /// Creates a value converter and sets the convert method.
        /// </summary>
        /// <param name="conversion">The conversion method for the value.</param>
        public ValueConverter(Func<object, T> conversion) : base(typeof(T), Adapt(conversion)) { }

        /// <summary>
        /// Adapts the typed conversion func to the underlying (object, object) func.
        /// </summary>
        private static Func<object, object> Adapt(Func<object, T> conversion) => val => conversion(val);
    }
}
