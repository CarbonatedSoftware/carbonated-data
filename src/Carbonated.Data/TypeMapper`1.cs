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
        /// Constructs a Type Mapper and sets the creator function.
        /// </summary>
        /// <param name="creator"></param>
        public TypeMapper(Func<Record, TEntity> creator)
        {
            Creator = creator;
        }

        /// <summary>
        /// Custom function that takes in a record and returns an instance of the entity.
        /// </summary>
        public Func<Record, TEntity> Creator { get; set; }

        /// <summary>
        /// Creates and populates instance of an entity from a record.
        /// </summary>
        /// <param name="record">The record to create an instance from.</param>
        /// <returns>The newly created and populate instance.</returns>
        protected internal override TEntity CreateInstance(Record record) => Creator(record);
    }
}
