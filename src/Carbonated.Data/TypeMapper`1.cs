using System;

namespace Carbonated.Data
{
    /// <summary>
    /// Maps records to entities using a custom creator function.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being mapped.</typeparam>
    public class TypeMapper<TEntity> : Mapper<TEntity>
    {
        /// <summary>
        /// Custom function that takes in a record and returns an instance of the entity.
        /// </summary>
        public Func<Record, TEntity> Creator { get; set; }

        public TEntity CreateInstance(Record record) => Creator(record);
    }
}
