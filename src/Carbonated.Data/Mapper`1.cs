using System;

namespace Carbonated.Data
{
    /// <summary>
    /// Base class that generic-typed record-to-entity mappers derive from, allowing them
    /// to be included in less specifically typed collections together.
    /// </summary>
    public abstract class Mapper
    {
        protected internal Type EntityType { get; set; }
    }

    /// <summary>
    /// Base interface that mappers implement.
    /// </summary>
    /// <typeparam name="TEntity">The entity being mapped.</typeparam>
    public abstract class Mapper<TEntity> : Mapper
    {
        public Mapper()
        {
            EntityType = typeof(TEntity);
        }

        /// <summary>
        /// Creates and populates instance of an entity from a record.
        /// </summary>
        /// <param name="record">The record to create an instance from.</param>
        /// <returns>The newly created and populate instance.</returns>
        protected internal abstract TEntity CreateInstance(Record record);
    }
}
