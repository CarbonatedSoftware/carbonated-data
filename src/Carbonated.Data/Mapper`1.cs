namespace Carbonated.Data
{
    /// <summary>
    /// Base interface that mappers implement.
    /// </summary>
    /// <typeparam name="TEntity">The entity being mapped.</typeparam>
    public interface Mapper<TEntity>
    {
        /// <summary>
        /// Creates and populates instance of an entity from a record.
        /// </summary>
        /// <param name="record">The record to create an instance from.</param>
        /// <returns>The newly created and populate instance.</returns>
        TEntity CreateInstance(Record record);
    }
}
