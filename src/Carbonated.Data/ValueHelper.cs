using System;

namespace Carbonated.Data
{
    internal class ValueHelper
    {
        internal static T GetValue<T>(object value)
        {
            return (T)(GetValue(value, typeof(T)) ?? default(T));
        }

        private static object GetValue(object value, Type type)
        {
            type = GetUnderlyingType(type);

            if (value == null || value is DBNull)
            {
                return null;
            }
            else  if (type == typeof(Guid))
            {
                return ConvertGuid(value);
            }
            else if (type == typeof(char))
            {
                return ConvertChar(value);
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        private static Type GetUnderlyingType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(type)
                : type;
        }

        private static object ConvertGuid(object value)
        {
            if (value is Guid)
            {
                return value;
            }
            string str = value.ToString();
            return !string.IsNullOrEmpty(str) ? Guid.Parse(str) : (object)null;
        }

        private static object ConvertChar(object value)
        {
            if (value is char)
            {
                return value;
            }
            string str = value.ToString();
            return !string.IsNullOrEmpty(str) ? str[0] : (object)null;
        }
    }
}
