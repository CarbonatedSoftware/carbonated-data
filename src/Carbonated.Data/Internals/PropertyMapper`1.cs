using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbonated.Data.Internals
{
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
        public PropertyMapper<TEntity> Map<TProperty>(Expression<Func<TEntity, TProperty>> property, string field)
            => MapProp(property, field, defaultCondition, null);

        /// <summary>
        /// Maps a property to the specified field name, with a function that will convert the value as it is
        /// loaded from the data record. The population condition will be the default for the mapper.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <param name="valueConverter">Function that will convert values as they are loaded.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> Map<TProperty>(Expression<Func<TEntity, TProperty>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, defaultCondition, valueConverter);

        /// <summary>
        /// Maps an Optional property to the specified field name
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapOptional<TProperty>(Expression<Func<TEntity, TProperty>> property, string field) 
            => MapProp(property, field, PopulationCondition.Optional, null);

        /// <summary>
        /// Maps an Optional property to the specified field name, with a function that will convert the value
        /// as it is loaded from the data record.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <param name="valueConverter">Function that will convert values as they are loaded.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapOptional<TProperty>(Expression<Func<TEntity, TProperty>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.Optional, valueConverter);

        /// <summary>
        /// Maps a Required property to the specified field name
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapRequired<TProperty>(Expression<Func<TEntity, TProperty>> property, string field) 
            => MapProp(property, field, PopulationCondition.Required, null);

        /// <summary>
        /// Maps a Required property to the specified field name, with a function that will convert the value
        /// as it is loaded from the data record.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <param name="valueConverter">Function that will convert values as they are loaded.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapRequired<TProperty>(Expression<Func<TEntity, TProperty>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.Required, valueConverter);

        /// <summary>
        /// Maps a NotNull property to the specified field name
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapNotNull<TProperty>(Expression<Func<TEntity, TProperty>> property, string field) 
            => MapProp(property, field, PopulationCondition.NotNull, null);

        /// <summary>
        /// Maps a NotNull property to the specified field name, with a function that will convert the value
        /// as it is loaded from the data record.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <param name="field">The field name to load data from.</param>
        /// <param name="valueConverter">Function that will convert values as they are loaded.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> MapNotNull<TProperty>(Expression<Func<TEntity, TProperty>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.NotNull, valueConverter);

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
        /// Marks a property to be ignored. Ignored properties will not have any data loaded for them, even if
        /// there is a matching field available in the data source.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property being mapped.</typeparam>
        /// <param name="property">Expression that specifies which property of the entity is being mapped.</param>
        /// <returns>The property mapper.</returns>
        public PropertyMapper<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
            var existing = mappings.SingleOrDefault(m => m.Property.Name == prop.Name);
            if (existing == null)
            {
                mappings.Add(new PropertyMapInfo(prop.Name, prop) { IsIgnored = true });
            }
            else
            {
                existing.IsIgnored = true;
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

        private PropertyMapper<TEntity> MapProp<TProperty>(Expression<Func<TEntity, TProperty>> property, string field, PopulationCondition condition, Func<object, object> valueConverter)
        {
            var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
            if (PropertyIsMappedToDifferentField(field, prop))
            {
                throw new MappingException($"Field cannot be mapped to more than one property: {field}");
            }

            //TODO: Consider updating rather than removing and re-adding.
            RemoveGeneratedMapping(prop);
            mappings.Add(new PropertyMapInfo(field, prop) { Condition = condition, ValueConverter = valueConverter });

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
            var instance = Activator.CreateInstance<TEntity>();
            foreach (var mapping in Mappings.Where(m => !m.IsIgnored))
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

                if (mapping.ValueConverter != null)
                {
                    prop.SetValue(instance, mapping.ValueConverter(value), null);
                }
                else
                {
                    if (valueConverters.TryGetValue(prop.PropertyType, out ValueConverter conv))
                    {
                        prop.SetValue(instance, conv.Convert(value), null);
                    }
                    else
                    {
                        prop.SetValue(instance, Converter.ToType(value, prop.PropertyType), null);
                    }
                }
            }
            AfterBindAction?.Invoke(record, instance);

            return instance;
        }
    }
}
