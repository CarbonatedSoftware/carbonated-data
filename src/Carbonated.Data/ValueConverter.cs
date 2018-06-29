using System;

namespace Carbonated.Data
{
    public abstract class ValueConverter
    {
        private readonly Func<object, object> conversion;

        public ValueConverter(Type type, Func<object, object> conversion)
        {
            ValueType = type;
            this.conversion = conversion;
        }

        public Type ValueType { get; }

        public object Convert(object input) => conversion(input);
    }

    public class ValueConverter<T> : ValueConverter
    {
        public ValueConverter(Func<object, T> conversion) : base(typeof(T), Adapt(conversion))
        {
        }

        private static Func<object, object> Adapt(Func<object, T> conversion) => val => conversion(val);
    }
}
