using System.Reflection;

namespace Carbonated.Data
{
    public class PropertyMapInfo
    {
        public PropertyMapInfo(string field, PropertyInfo property)
        {
            Field = field;
            Property = property;
        }

        public string Field { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
