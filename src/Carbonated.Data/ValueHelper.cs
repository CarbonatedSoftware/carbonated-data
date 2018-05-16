using System;
using System.Collections.Generic;
using System.Text;

namespace Carbonated.Data
{
    internal class ValueHelper
    {
        internal static T GetValue<T>(object value)
        {
            if (typeof(T) == typeof(Guid))
            {
                return GetGuid<T>(value);
            }

            if (typeof(T)==typeof(char))
            {
                if (value is char c)
                    return (T)value;

                if (value is string str && !string.IsNullOrEmpty(str))
                    return (T)(object)str[0];

                return default(T);
            }

            return value == DBNull.Value || value == null 
                ? default(T) 
                : (T)Convert.ChangeType(value, typeof(T));
        }

        private static T GetGuid<T>(object value)
        {
            if (value is Guid guid)
                return (T)value;

            if (value is string str && !string.IsNullOrEmpty(str))
                return (T)(object)Guid.Parse(str);

            return default(T);
        }
    }
}
