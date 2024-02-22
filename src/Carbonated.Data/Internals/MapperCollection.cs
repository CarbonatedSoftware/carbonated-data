using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Carbonated.Data.Internals;

/// <summary>
/// Manages the collection of entity mappers. The collection is pre-loaded with
/// mappers for common framework value types, decimal, Guid, and string.
/// </summary>
public class MapperCollection
{
    private readonly ConcurrentDictionary<Type, Mapper> mappers = new();
    private readonly Dictionary<Type, ValueConverter> valueConverters = new();

    /// <summary>
    /// Constructs a new mapper collection.
    /// </summary>
    public MapperCollection()
    {
        AddFrameworkTypes();
    }

    /// <summary>
    /// Adds a value converter to the list of converters that the mapper collection tracks. The converters
    /// will be used by Property Mappers when populating properties of types in the list.
    /// </summary>
    /// <typeparam name="T">The type to add a converter for.</typeparam>
    /// <param name="conversion">The conversion method, from object to the desired type.</param>
    public void AddValueConverter<T>(Func<object, T> conversion)
    {
        valueConverters.Add(typeof(T), new ValueConverter<T>(conversion));
    }

    /// <summary>
    /// Indicates if a value converter for the specified type has been registered.
    /// </summary>
    /// <typeparam name="T">The type to check for.</typeparam>
    /// <returns>true if a converter is registered; otherwise, false.</returns>
    public bool HasValueConverter<T>()
    {
        return valueConverters.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Gets a custom value converter if any can be found for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <returns>The custom value converter, or null if none is found.</returns>
    internal ValueConverter GetValueConverter<T>()
    {
        valueConverters.TryGetValue(typeof(T), out ValueConverter conv);
        return conv;
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
        var mapper = new PropertyMapper<TEntity>(valueConverters);
        if (!mappers.TryAdd(typeof(TEntity), mapper))
        {
            throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
        }
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
        var mapper = new PropertyMapper<TEntity>(valueConverters, condition);
        if (!mappers.TryAdd(typeof(TEntity), mapper))
        {
            throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
        }
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
        if (!mappers.TryAdd(typeof(TEntity), new TypeMapper<TEntity>(creator)))
        {
            throw new MappingException($"A mapper is already configured for type {typeof(TEntity).Name}");
        }
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
        var mapper = mappers.GetOrAdd(typeof(TEntity), _ => new PropertyMapper<TEntity>(valueConverters));
        return (Mapper<TEntity>)mapper;
    }

    /// <summary>
    /// Indicates if a mapper for the specified entity type has been registered.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to check for.</typeparam>
    /// <returns>true if a mapper is registered; otherwise, false</returns>
    public bool HasMapper<TEntity>()
    {
        return mappers.ContainsKey(typeof(TEntity));
    }

    private void AddFrameworkTypes()
    {
        mappers.TryAdd(typeof(bool), new ValueTypeMapper<bool>());
        mappers.TryAdd(typeof(byte), new ValueTypeMapper<byte>());
        mappers.TryAdd(typeof(short), new ValueTypeMapper<short>());
        mappers.TryAdd(typeof(int), new ValueTypeMapper<int>());
        mappers.TryAdd(typeof(long), new ValueTypeMapper<long>());
        mappers.TryAdd(typeof(float), new ValueTypeMapper<float>());
        mappers.TryAdd(typeof(double), new ValueTypeMapper<double>());
        mappers.TryAdd(typeof(decimal), new ValueTypeMapper<decimal>());
        mappers.TryAdd(typeof(DateTime), new ValueTypeMapper<DateTime>());
        mappers.TryAdd(typeof(DateOnly), new ValueTypeMapper<DateOnly>());
        mappers.TryAdd(typeof(TimeOnly), new ValueTypeMapper<TimeOnly>());
        mappers.TryAdd(typeof(Guid), new ValueTypeMapper<Guid>());
        mappers.TryAdd(typeof(char), new ValueTypeMapper<char>());
        mappers.TryAdd(typeof(string), new ValueTypeMapper<string>());
        mappers.TryAdd(typeof(byte[]), new ValueTypeMapper<byte[]>());

        mappers.TryAdd(typeof(bool?), new ValueTypeMapper<bool?>());
        mappers.TryAdd(typeof(byte?), new ValueTypeMapper<byte?>());
        mappers.TryAdd(typeof(short?), new ValueTypeMapper<short?>());
        mappers.TryAdd(typeof(int?), new ValueTypeMapper<int?>());
        mappers.TryAdd(typeof(long?), new ValueTypeMapper<long?>());
        mappers.TryAdd(typeof(float?), new ValueTypeMapper<float?>());
        mappers.TryAdd(typeof(double?), new ValueTypeMapper<double?>());
        mappers.TryAdd(typeof(decimal?), new ValueTypeMapper<decimal?>());
        mappers.TryAdd(typeof(DateTime?), new ValueTypeMapper<DateTime?>());
        mappers.TryAdd(typeof(DateOnly?), new ValueTypeMapper<DateOnly?>());
        mappers.TryAdd(typeof(TimeOnly?), new ValueTypeMapper<TimeOnly?>());
        mappers.TryAdd(typeof(Guid?), new ValueTypeMapper<Guid?>());
        mappers.TryAdd(typeof(char?), new ValueTypeMapper<char?>());
    }
}
