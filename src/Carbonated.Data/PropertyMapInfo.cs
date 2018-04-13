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

        /// <summary>
        /// The field name that is being mapped.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// The property that is being mapped.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Condition that must be fulfilled when populating a property.
        /// </summary>
        public PopulationCondition Condition { get; set; } = PopulationCondition.Optional;

        /// <summary>
        /// When set, indicates that the field should be ignored during data binding.
        /// </summary>
        public bool IsIgnored { get; set; }
    }
}
