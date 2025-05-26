using System;
using System.Collections.Generic;

namespace Carbonated.Data.Internals;

/// <summary>
/// Provides functionality to map database records to tuple types. The tuple type must be a valid tuple type, which is
/// either <see cref="ValueTuple"/> or a type that derives from <see cref="ValueTuple"/>.
/// </summary>
/// <typeparam name="TEntity">The tuple type to which records will be mapped.</typeparam>
public class TupleMapper<TEntity> : Mapper<TEntity>
{
    private readonly IDictionary<Type, ValueConverter> valueConverters;

    /// <summary>
    /// Constructs a Tuple Mapper for the specif tuple type.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the TEntity is not a valid tuple type.</exception>
    public TupleMapper(IDictionary<Type, ValueConverter> converters)
    {
        if (!IsTupleType(EntityType))
        {
            throw new ArgumentException($"The type {EntityType} is not a valid tuple type.");
        }

        valueConverters = converters ?? new Dictionary<Type, ValueConverter>();
    }

    /// <inheritdoc />
    protected internal override TEntity CreateInstance(Record record)
    {
        var args = typeof(TEntity).GetGenericArguments();
        if (record.FieldCount < args.Length)
        {
            throw new ArgumentException($"The record has fewer fields ({record.FieldCount}) than the tuple type ({args.Length}).");
        }

        var values = new object[args.Length];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = valueConverters.TryGetValue(args[i], out ValueConverter conv)
                ? conv.Convert(record[i])
                : Converter.ToType(record[i], args[i]);
        }

        return (TEntity)Activator.CreateInstance(EntityType, values);
    }

    internal static bool IsTupleType(Type type)
    {
        return type.IsGenericType 
            && type.GetGenericTypeDefinition().FullName.StartsWith("System.ValueTuple")
            && type.GetGenericArguments().Length > 0;
    }
}
