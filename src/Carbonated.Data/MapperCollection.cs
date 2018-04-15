using System.Collections.Generic;
using System.Linq;

namespace Carbonated.Data
{
    /// <summary>
    /// Manages the collection of entity mappers. The collection is pre-loaded with
    /// mappers for common framework value types, decimal, Guid, and string.
    /// </summary>
    public class MapperCollection
    {
        private readonly List<Mapper> mappers = new List<Mapper>();

        /*TODO: Replace Add with a Configure/Register/Annotate method that returns the
         * mapper being added so that it can be configured directly. Overloads for property
         * mappers w/ & w/o default condition, and type mappers.
         */

        /// <summary>
        /// Adds an entity mapper to the collection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that the mapper is for.</typeparam>
        /// <param name="mapper">The mapper to add.</param>
        public void Add<TEntity>(Mapper<TEntity> mapper)
        {
            if (HasMapper<TEntity>())
            {
                throw new MappingException($"A mapper is already registered for type {typeof(TEntity).Name}");
            }
            mappers.Add(mapper);
        }

        /// <summary>
        /// Gets the mapper for the specified entity type. If no mapper has been registered
        /// for that entity type, a new property mapper will be created and stored
        /// automatically.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to get the mapper for.</typeparam>
        /// <returns>The entity type's mapper.</returns>
        public Mapper<TEntity> Get<TEntity>()
        {
            var mapper = mappers.SingleOrDefault(m => m.EntityType == typeof(TEntity));
            if (mapper == null)
            {
                mappers.Add(mapper = new PropertyMapper<TEntity>());
            }
            return (Mapper<TEntity>)mapper;
        }

        /// <summary>
        /// Indicates if a mapper for the specified entity type has been registered.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to check for.</typeparam>
        /// <returns>true if a mapper is registered; otherwise, false</returns>
        public bool HasMapper<TEntity>()
        {
            return mappers.Any(m => m.EntityType == typeof(TEntity));
        }
    }
}
