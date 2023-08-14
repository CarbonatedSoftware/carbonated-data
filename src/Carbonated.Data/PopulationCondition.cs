namespace Carbonated.Data;

/// <summary>
/// The conditions that must be met when populating fields of an object from a data record.
/// </summary>
public enum PopulationCondition
{
    /// <summary> The field does not need to be present in the data record. </summary>
    Optional,
    /// <summary> The field must be present. </summary>
    Required,
    /// <summary> The field must be present and have a non-null value. </summary>
    NotNull
}
