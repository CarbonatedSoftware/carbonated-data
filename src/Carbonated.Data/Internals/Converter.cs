using System;

namespace Carbonated.Data.Internals
{
    internal class Converter
    {
        internal static T ToType<T>(object value)
        {
            return (T)(ToType(value, typeof(T)) ?? default(T));
        }

        internal static object ToType(object value, Type type)
        {
            bool isNullable = IsNullable(type);
            type = isNullable ? Nullable.GetUnderlyingType(type) : type;

            if (value == null || value is DBNull)
            {
                return null;
            }
            else if (type.IsEnum)
            {
                return ConvertEnum(value, type);
            }
            else if (type == typeof(Guid))
            {
                return ConvertGuid(value);
            }
            else if (type == typeof(char) && value.ToString() == string.Empty)
            {
                // Empty char columns are possible in a database, but converting them
                // to the char type will fail, so we need to check for them and set
                // the value to null so that default will be set.
                return null;
            }
            else if (!isNullable && IsComplex(type) && IsPossiblyJson(value))
            {
                return DeserializeJson(value, type);
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        private static bool IsNullable(Type type) 
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        private static object ConvertEnum(object value, Type type)
        {
            if (Enum.IsDefined(type, value))
            {
                return Enum.Parse(type, value.ToString(), true);
            }
            if (value is string str && str == string.Empty)
            {
                return null;
            }
            throw new BindingException($"Value could not be parsed as {type.Name}: {value}");
        }

        private static object ConvertGuid(object value)
        {
            if (value is Guid)
            {
                return value;
            }

            string str = value.ToString();
            if (str == string.Empty)
            {
                // Empty string for a GUID should be treated as a null.
                return null;
            }

            if (Guid.TryParse(str, out Guid guid))
            {
                return guid;
            }

            throw new BindingException($"Value could not be parsed as {typeof(Guid).Name}: {value}");
        }

        private static bool IsComplex(Type type)
        {
            return !(type.IsPrimitive
                || type == typeof(DateTime)
                || type == typeof(decimal)
                || type == typeof(Guid)
                || type == typeof(string));
        }

        private static bool IsPossiblyJson(object value)
        {
            if (!(value is string))
            {
                return false;
            }
            string str = value.ToString().Trim();
            return string.IsNullOrWhiteSpace(str)
                || (str.StartsWith("{") && str.EndsWith("}"))
                || (str.StartsWith("[") && str.EndsWith("]"));
        }

        private static object DeserializeJson(object value, Type propertyType)
            => Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString(), propertyType);
    }
}
