using System;
using System.Reflection;

namespace Carbonated.Data
{
    /// <summary>
    /// Holds information about a property map.
    /// </summary>
    public class PropertyMapInfo
    {
        /// <summary>
        /// Constructs the property mapper.
        /// </summary>
        /// <param name="field">The field being mapped.</param>
        /// <param name="property">The property being mapped.</param>
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
        /// When set, value converter that will be used in place of standard behavior when
        /// populating entities.
        /// </summary>
        public Func<object, object> ValueConverter { get; set; }

        /// <summary>
        /// When set, indicates that the field should be ignored during data binding.
        /// </summary>
        public bool IsIgnored { get; set; }
    }
}
