using System;
using System.Reflection;

namespace Carbonated.Data
{
    /// <summary>
    /// Holds information about a property map.
    /// </summary>
    internal class PropertyMapInfo
    {
        /// <summary>
        /// Constructs the property mapper.
        /// </summary>
        /// <param name="field">The field being mapped.</param>
        /// <param name="property">The property being mapped.</param>
        internal PropertyMapInfo(string field, PropertyInfo property)
        {
            Field = field;
            Property = property;
        }

        /// <summary>
        /// The field name that is being mapped.
        /// </summary>
        internal string Field { get; set; }

        /// <summary>
        /// The property that is being mapped.
        /// </summary>
        internal PropertyInfo Property { get; set; }

        /// <summary>
        /// Condition that must be fulfilled when populating a property.
        /// </summary>
        internal PopulationCondition Condition { get; set; } = PopulationCondition.Optional;

        /// <summary>
        /// When set, value converter that will be used in place of standard behavior when
        /// populating entities.
        /// </summary>
        internal Func<object, object> ValueConverter { get; set; }

        /// <summary>
        /// When set, indicates that the field should be ignored during data binding.
        /// </summary>
        internal bool IsIgnored { get; set; }
    }
}
