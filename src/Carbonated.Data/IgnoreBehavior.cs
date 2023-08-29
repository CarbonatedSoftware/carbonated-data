namespace Carbonated.Data;

/// <summary>
/// Controls the ignore behavior for property in a <see cref="Internals.PropertyMapper{TEntity}"/>.
/// </summary>
public enum IgnoreBehavior
{
    /// <summary>
    /// Ignore the property both on read and write operations.
    /// The property will not be populated during instantiation by a <see cref="Internals.PropertyMapper{TEntity}"/>.
    /// The property will not be included as a field by an <see cref="EntityDataReader{TEntity}"/>.
    /// </summary>
    Both,

    /// <summary>
    /// The property will not be populated during instantiation by a <see cref="Internals.PropertyMapper{TEntity}"/>.
    /// The property will be included as a field by an <see cref="EntityDataReader{TEntity}"/>.
    /// </summary>
    OnLoad,

    /// <summary>
    /// The property will not be included as a field by an <see cref="EntityDataReader{TEntity}"/>.
    /// The property will be populated during instantiation by a <see cref="Internals.PropertyMapper{TEntity}"/>.
    /// </summary>
    OnSave
}
