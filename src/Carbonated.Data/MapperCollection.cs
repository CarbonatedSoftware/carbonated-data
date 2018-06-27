﻿using System;
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

        /// <summary>
        /// Constructs a new mapper collection.
        /// </summary>
        public MapperCollection()
        {
            AddFrameworkTypes();
        }

        /// <summary>
        /// Adds a property mapper for an entity and returns it for configuration. The default population
        /// condition for all properties will be Optional.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that the mapper is for.</typeparam>
        /// <returns>The property mapper being configured.</returns>
        /// <exception cref="MappingException">Throws if a mapper is already configured for the entity.</exception>
        public PropertyMapper<TEntity> Configure<TEntity>()
        {
            if (HasMapper<TEntity>())
            {
                throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
            }
            var mapper = new PropertyMapper<TEntity>();
            mappers.Add(mapper);
            return mapper;
        }

        /// <summary>
        /// Adds a property mapper for an entity with the default condition specified and returns the mapper
        /// for configuration.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that the mapper is for.</typeparam>
        /// <param name="condition">The default population condition to set for all properties.</param>
        /// <returns>The property mapper being configured.</returns>
        /// <exception cref="MappingException">Throws if a mapper is already configured for the entity.</exception>
        public PropertyMapper<TEntity> Configure<TEntity>(PopulationCondition condition)
        {
            if (HasMapper<TEntity>())
            {
                throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
            }
            var mapper = new PropertyMapper<TEntity>(condition);
            mappers.Add(mapper);
            return mapper;
        }

        /// <summary>
        /// Adds a Type Mapper for an entity using the specified creator method.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that the mapper is for.</typeparam>
        /// <param name="creator">The creator method used to populate the entity from a record.</param>
        /// <exception cref="MappingException">Throws if a mapper is already configured for the entity.</exception>
        public void Configure<TEntity>(Func<Record, TEntity> creator)
        {
            if (HasMapper<TEntity>())
            {
                throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
            }
            mappers.Add(new TypeMapper<TEntity>(creator));
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

        private void AddFrameworkTypes()
        {
            mappers.Add(new ValueTypeMapper<bool>());
            mappers.Add(new ValueTypeMapper<byte>());
            mappers.Add(new ValueTypeMapper<short>());
            mappers.Add(new ValueTypeMapper<int>());
            mappers.Add(new ValueTypeMapper<long>());
            mappers.Add(new ValueTypeMapper<float>());
            mappers.Add(new ValueTypeMapper<double>());
            mappers.Add(new ValueTypeMapper<decimal>());
            mappers.Add(new ValueTypeMapper<DateTime>());
            mappers.Add(new ValueTypeMapper<Guid>());
            mappers.Add(new ValueTypeMapper<char>());
            mappers.Add(new ValueTypeMapper<string>());
            mappers.Add(new ValueTypeMapper<byte[]>());

            mappers.Add(new ValueTypeMapper<bool?>());
            mappers.Add(new ValueTypeMapper<byte?>());
            mappers.Add(new ValueTypeMapper<short?>());
            mappers.Add(new ValueTypeMapper<int?>());
            mappers.Add(new ValueTypeMapper<long?>());
            mappers.Add(new ValueTypeMapper<float?>());
            mappers.Add(new ValueTypeMapper<double?>());
            mappers.Add(new ValueTypeMapper<decimal?>());
            mappers.Add(new ValueTypeMapper<DateTime?>());
            mappers.Add(new ValueTypeMapper<Guid?>());
            mappers.Add(new ValueTypeMapper<char?>());
        }
    }
}
