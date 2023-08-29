using System;
using System.Reflection;

namespace Carbonated.Data.Internals;

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
    /// populating entity properties from database fields.
    /// </summary>
    internal Func<object, object> FromDbConverter { get; set; }

    /// <summary>
    /// When set, value converter that will be used in place of standard behavior when
    /// saving property values to database fields.
    /// </summary>
    internal Func<object, object> ToDbConverter { get; set; }

    /// <summary>
    /// When set, indicates that the field should be ignored during data binding.
    /// </summary>
    internal bool IsIgnored { get; set; }

    /// <summary>
    /// When is IsIgnored is set, the ignore behavior that will be used.
    /// </summary>
    internal IgnoreBehavior IgnoreBehavior { get; set; } = IgnoreBehavior.Both;
}
