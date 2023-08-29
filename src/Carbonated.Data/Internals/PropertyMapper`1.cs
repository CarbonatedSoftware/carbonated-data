using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace Carbonated.Data.Internals;

/// <summary>
/// Maps records to entities on a field-to-property basis. Property mappings can be configured using the
/// various Map and condition methods.
/// </summary>
/// <typeparam name="TEntity">The type of entity being mapped.</typeparam>
public class PropertyMapper<TEntity> : Mapper<TEntity>
{
    private readonly IList<PropertyMapInfo> mappings;
    private readonly PopulationCondition defaultCondition;
    private readonly IDictionary<Type, ValueConverter> valueConverters;

    /// <summary>
    /// Constructs a Property Mapper with a default condition of Optional.
    /// </summary>
    /// <param name="converters">Dictionary of value converters for custom types.</param>
    public PropertyMapper(IDictionary<Type, ValueConverter> converters) : this(converters, PopulationCondition.Optional) { }

    /// <summary>
    /// Constructs a Property Mapper with the default condition specified.
    /// </summary>
    /// <param name="converters">Dictionary of value converters for custom types.</param>
    /// <param name="condition">The default population condition to set for properties.</param>
    public PropertyMapper(IDictionary<Type, ValueConverter> converters, PopulationCondition condition)
    {
        valueConverters = converters ?? new Dictionary<Type, ValueConverter>();
        defaultCondition = condition;
        mappings = GenerateDefaultMappings();

        IList<PropertyMapInfo> GenerateDefaultMappings()
        {
            return typeof(TEntity)
                .GetProperties().Where(p => p.CanWrite)
                .Select(prop => new PropertyMapInfo(prop.Name, prop) { Condition = defaultCondition })
                .ToList();
        }
    }

    internal IEnumerable<PropertyMapInfo> Mappings => mappings;

    internal Action<Record, TEntity> AfterBindAction { get; private set; }

    /// <summary>
    /// Maps a property to the specified field name. The population condition will be the default for the
    /// mapper.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Map<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        string field)
        => MapProp(property, field, defaultCondition, null, null);

    /// <summary>
    /// Maps a property to the specified field name, with a function that will convert the value as it is
    /// loaded from the data record. The population condition will be the default for the mapper.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Map<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        Func<object, object> fromDbConverter)
        => MapProp(property, field, defaultCondition, fromDbConverter, null);

    /// <summary>
    /// Maps a property to the specified field name, with a function that will convert the value as it is
    /// loaded from the data record. The population condition will be the default for the mapper.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <param name="toDbConverter">Function that will convert values as they are read using EntityDataReader.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Map<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        Func<object, object> fromDbConverter, 
        Func<object, object> toDbConverter)
    {
        return MapProp(property, field, defaultCondition, fromDbConverter, toDbConverter);
    }

    /// <summary>
    /// Maps an Optional property to the specified field name
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapOptional<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field) 
        => MapProp(property, field, PopulationCondition.Optional, null, null);

    /// <summary>
    /// Maps an Optional property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapOptional<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        Func<object, object> fromDbConverter)
        => MapProp(property, field, PopulationCondition.Optional, fromDbConverter, null);

    /// <summary>
    /// Maps an Optional property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <param name="toDbConverter">Function that will convert values as they are read using EntityDataReader.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapOptional<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        string field,
        Func<object, object> fromDbConverter,
        Func<object, object> toDbConverter)
        => MapProp(property, field, PopulationCondition.Optional, fromDbConverter, toDbConverter);

    /// <summary>
    /// Maps a Required property to the specified field name
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapRequired<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field) 
        => MapProp(property, field, PopulationCondition.Required, null, null);

    /// <summary>
    /// Maps a Required property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapRequired<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        Func<object, object> fromDbConverter)
        => MapProp(property, field, PopulationCondition.Required, fromDbConverter, null);

    /// <summary>
    /// Maps a Required property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <param name="toDbConverter">Function that will convert values as they are read using EntityDataReader.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapRequired<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        string field,
        Func<object, object> fromDbConverter,
        Func<object, object> toDbConverter)
        => MapProp(property, field, PopulationCondition.Required, fromDbConverter, toDbConverter);

    /// <summary>
    /// Maps a NotNull property to the specified field name
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapNotNull<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field) 
        => MapProp(property, field, PopulationCondition.NotNull, null, null);

    /// <summary>
    /// Maps a NotNull property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapNotNull<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        Func<object, object> fromDbConverter)
        => MapProp(property, field, PopulationCondition.NotNull, fromDbConverter, null);

    /// <summary>
    /// Maps a NotNull property to the specified field name, with a function that will convert the value
    /// as it is loaded from the data record.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <param name="field">The field name to load data from.</param>
    /// <param name="fromDbConverter">Function that will convert values as they are loaded.</param>
    /// <param name="toDbConverter">Function that will convert values as they are read using EntityDataReader.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> MapNotNull<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        string field,
        Func<object, object> fromDbConverter,
        Func<object, object> toDbConverter)
        => MapProp(property, field, PopulationCondition.NotNull, fromDbConverter, toDbConverter);

    /// <summary>
    /// Marks a property's population condition as Optional. This will override the default condition set
    /// for the mapper in the constructor.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Optional<TProperty>(Expression<Func<TEntity, TProperty>> property) 
        => SetCondition(property, PopulationCondition.Optional);

    /// <summary>
    /// Marks a property's population condition as Required. This will override the default condition set
    /// for the mapper in the constructor.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Required<TProperty>(Expression<Func<TEntity, TProperty>> property) 
        => SetCondition(property, PopulationCondition.Required);

    /// <summary>
    /// Marks a property's population condition as NotNull. This will override the default condition set
    /// for the mapper in the constructor.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> NotNull<TProperty>(Expression<Func<TEntity, TProperty>> property) 
        => SetCondition(property, PopulationCondition.NotNull);

    /// <summary>
    /// Marks a property to be ignored. The property will not have any data loaded for it, even if there is
    /// a matching field available in the data source, and will not be read as a field by the
    /// <see cref="EntityDataReader{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> property)
    {
        return Ignore(property, IgnoreBehavior.Both);
    }

    /// <summary>
    /// Marks a property to be ignored during load. The property will not have any data loaded for it, even
    /// if there is a matching field available in the data source.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> IgnoreOnLoad<TProperty>(Expression<Func<TEntity,TProperty>> property)
    {
        return Ignore(property, IgnoreBehavior.OnLoad);
    }

    /// <summary>
    /// Marks a property to be ignored during save. The property will not be read as a field by the
    /// <see cref="EntityDataReader{TEntity}"/>, even if a value is set.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
    /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> IgnoreOnSave<TProperty>(Expression<Func<TEntity, TProperty>> property)
    {
        return Ignore(property, IgnoreBehavior.OnSave);
    }

    private PropertyMapper<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> property, IgnoreBehavior ignoreBehavior = IgnoreBehavior.Both)
    {
        var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
        var existing = mappings.SingleOrDefault(m => m.Property.Name == prop.Name);
        if (existing == null)
        {
            mappings.Add(new PropertyMapInfo(prop.Name, prop) { IsIgnored = true, IgnoreBehavior = ignoreBehavior });
        }
        else
        {
            existing.IsIgnored = true;
            existing.IgnoreBehavior = ignoreBehavior;
        }
        return this;
    }

    /// <summary>
    /// Sets an action that will execute after an entity has had its properties populated. The action is
    /// passed both the data record and the newly loaded entity, so that it can perform whatever
    /// additional logic is needed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The property mapper.</returns>
    public PropertyMapper<TEntity> AfterBinding(Action<Record, TEntity> action)
    {
        AfterBindAction = action;
        return this;
    }

    private PropertyMapper<TEntity> MapProp<TProperty>(
        Expression<Func<TEntity, TProperty>> property, 
        string field, 
        PopulationCondition condition, 
        Func<object, object> fromDbConverter, 
        Func<object, object> toDbConverter)
    {
        var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
        if (PropertyIsMappedToDifferentField(field, prop))
        {
            throw new MappingException($"Field cannot be mapped to more than one property: {field}");
        }

        RemoveGeneratedMapping(prop);
        mappings.Add(new PropertyMapInfo(field, prop) { Condition = condition, FromDbConverter = fromDbConverter, ToDbConverter = toDbConverter });

        return this;
    }

    private PropertyMapper<TEntity> SetCondition<P>(Expression<Func<TEntity, P>> property, PopulationCondition condition)
    {
        var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
        var existing = mappings.SingleOrDefault(m => m.Property.Name == prop.Name);
        if (existing == null)
        {
            throw new Exception($"No mapping was found for property {prop.Name}");
        }
        existing.Condition = condition;
        existing.IsIgnored = false;

        return this;
    }

    private bool PropertyIsMappedToDifferentField(string field, PropertyInfo prop) 
        => mappings.Any(m => m.Field == field && m.Property.Name != prop.Name);

    private void RemoveGeneratedMapping(PropertyInfo prop)
    {
        var existing = mappings.SingleOrDefault(m => m.Property.Name == prop.Name);
        if (existing != null)
        {
            mappings.Remove(existing);
        }
    }

    /// <summary>
    /// Creates and populates instance of an entity from a record.
    /// </summary>
    /// <param name="record">The record to create an instance from.</param>
    /// <returns>The newly created and populate instance.</returns>
    protected internal override TEntity CreateInstance(Record record)
    {
        creationInfo ??= new InstanceCreationInfo(Mappings);

        TEntity instance = creationInfo.UseDefaultCtor
            ? Activator.CreateInstance<TEntity>()
            : (TEntity)creationInfo.Ctor.Invoke(GetCtorArgs(record));
        foreach (var mapping in creationInfo.NonCtorMappings)
        {
            mapping.Property.SetValue(instance, GetValue(record, mapping));
        }
        AfterBindAction?.Invoke(record, instance);
        return instance;

        object[] GetCtorArgs(Record record)
        {
            return creationInfo.CtorMappings.Select(mapping => GetValue(record, mapping)).ToArray();
        }
    }

    private object GetValue(Record record, PropertyMapInfo mapping)
    {
        if (mapping.Condition == PopulationCondition.Required && !record.HasField(mapping.Field))
        {
            throw new BindingException($"A required field was not found in the data record: {mapping.Field}");
        }

        var value = record.GetValue(mapping.Field);
        var prop = mapping.Property;

        if (mapping.Condition == PopulationCondition.NotNull && (value == null || value is DBNull))
        {
            throw new BindingException($"The value of {mapping.Field} may not be null.");
        }

        if (mapping.FromDbConverter != null)
        {
            return mapping.FromDbConverter(value);
        }
        if (valueConverters.TryGetValue(prop.PropertyType, out ValueConverter conv))
        {
            return conv.Convert(value);
        }
        return Converter.ToType(value, prop.PropertyType);
    }

    private InstanceCreationInfo creationInfo = null;

    private class InstanceCreationInfo
    {
        internal bool UseDefaultCtor { get; }
        internal ConstructorInfo Ctor { get; }
        internal List<PropertyMapInfo> CtorMappings { get; }
        internal List<PropertyMapInfo> NonCtorMappings { get; }

        internal InstanceCreationInfo(IEnumerable<PropertyMapInfo> mappings)
        {
            var filteredMappings = mappings.Where(m => !m.IsIgnored || m.IgnoreBehavior == IgnoreBehavior.OnSave).ToList();

            var defaultCtor = typeof(TEntity).GetConstructor(Type.EmptyTypes);
            if (defaultCtor != null)
            {
                UseDefaultCtor = true;
                Ctor = defaultCtor;
                CtorMappings = new List<PropertyMapInfo>();
                NonCtorMappings = filteredMappings;
                return;
            }

            var ctors = typeof(TEntity)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Select(c => (ctor: c, parameters: c.GetParameters()))
                .OrderBy(c => c.parameters.Length);

            foreach (var ctor in ctors)
            {
                var ctorMappings = filteredMappings.Where(m => ctor.parameters.Any(p => p.Name.Equals(m.Property.Name, StringComparison.OrdinalIgnoreCase))).ToList();
                if (ctor.parameters.Length == ctorMappings.Count)
                {
                    Ctor = ctor.ctor;
                    CtorMappings = ctor.parameters.Select(p => ctorMappings.Single(m => p.Name.Equals(m.Property.Name, StringComparison.OrdinalIgnoreCase))).ToList();
                    NonCtorMappings = filteredMappings.Except(ctorMappings).ToList();
                    return;
                }
            }

            throw new Exception("Could not find suitable constructor for type: " + typeof(TEntity).FullName);
        }
    }
}
