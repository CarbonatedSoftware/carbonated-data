using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbonated.Data
{
    public class PropertyMapper<TEntity> : Mapper<TEntity>
    {
        private readonly IList<PropertyMapInfo> mappings;
        private readonly PopulationCondition defaultCondition;

        public PropertyMapper() : this(PopulationCondition.Optional) { }

        public PropertyMapper(PopulationCondition condition)
        {
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

        public PropertyMapper<TEntity> Map<P>(Expression<Func<TEntity, P>> property, string field)
            => MapProp(property, field, defaultCondition, null);

        public PropertyMapper<TEntity> Map<P>(Expression<Func<TEntity, P>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, defaultCondition, valueConverter);

        public PropertyMapper<TEntity> MapOptional<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.Optional, null);

        public PropertyMapper<TEntity> MapOptional<P>(Expression<Func<TEntity, P>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.Optional, valueConverter);

        public PropertyMapper<TEntity> MapRequired<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.Required, null);

        public PropertyMapper<TEntity> MapRequired<P>(Expression<Func<TEntity, P>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.Required, valueConverter);

        public PropertyMapper<TEntity> MapNotNull<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.NotNull, null);

        public PropertyMapper<TEntity> MapNotNull<P>(Expression<Func<TEntity, P>> property, string field, Func<object, object> valueConverter)
            => MapProp(property, field, PopulationCondition.NotNull, valueConverter);

        public PropertyMapper<TEntity> Optional<P>(Expression<Func<TEntity, P>> property) 
            => SetCondition(property, PopulationCondition.Optional);

        public PropertyMapper<TEntity> Required<P>(Expression<Func<TEntity, P>> property) 
            => SetCondition(property, PopulationCondition.Required);

        public PropertyMapper<TEntity> NotNull<P>(Expression<Func<TEntity, P>> property) 
            => SetCondition(property, PopulationCondition.NotNull);

        public PropertyMapper<TEntity> Ignore<P>(Expression<Func<TEntity, P>> property)
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

        public PropertyMapper<TEntity> AfterBinding(Action<Record, TEntity> action)
        {
            AfterBindAction = action;
            return this;
        }

        private PropertyMapper<TEntity> MapProp<P>(Expression<Func<TEntity, P>> property, string field, PopulationCondition condition, Func<object, object> valueConverter)
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

        public TEntity CreateInstance(Record record)
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
                    prop.SetValue(instance, mapping.ValueConverter(value));
                }
                else
                {
                    Type propertyType = prop.PropertyType;
                    bool isNullable = IsNullable(propertyType);
                    if (isNullable)
                    {
                        // If we have a nullable type, extract it so that our type comparisons below work.
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                    }

                    if (value == null || value is DBNull)
                    {
                        prop.SetValue(instance, null);
                    }
                    else if (propertyType.IsEnum)
                    {
                        prop.SetValue(instance, ConvertEnum(value, propertyType));
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        prop.SetValue(instance, ConvertGuid(value));
                    }
                    else if (propertyType == typeof(char) && value.ToString() == string.Empty)
                    {
                        // Empty char columns are possible in a database, but converting them
                        // to the char type will fail, so we need to check for them and set
                        // the value to null so that default will be set.
                        prop.SetValue(instance, null);
                    }
                    else if (!isNullable && IsComplex(propertyType) && IsPossiblyJson(value))
                    {
                        prop.SetValue(instance, DeserializeJson(value, prop.PropertyType));
                    }
                    else
                    {
                        prop.SetValue(instance, Convert.ChangeType(value, propertyType));
                    }
                }
            }
            AfterBindAction?.Invoke(record, instance);

            return instance;
        }

        private bool IsNullable(Type type) 
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        private bool IsComplex(Type type)
        {
            return !(type.IsPrimitive 
                || type == typeof(DateTime) 
                || type == typeof(decimal) 
                || type == typeof(Guid) 
                || type == typeof(string));
        }

        private object ConvertEnum(object value, Type propertyType)
        {
            if (Enum.IsDefined(propertyType, value))
            {
                return Enum.Parse(propertyType, value.ToString(), true);
            }
            if (value is string str && str == string.Empty)
            {
                return null;
            }
            throw new BindingException($"Value could not be parsed as {propertyType.Name}: {value}");
        }

        private object ConvertGuid(object value)
        {
            // Empty string should be treated as nulls.
            if (value.ToString() == string.Empty)
            {
                return null;
            }

            if (Guid.TryParse(value.ToString(), out Guid guid))
            {
                return guid;
            }

            throw new BindingException($"Value could not be parsed as {typeof(Guid).Name}: {value}");
        }

        private bool IsPossiblyJson(object value)
        {
            if (!(value is string))
            {
                return false;
            }
            string str = value.ToString().Trim();
            return string.IsNullOrWhiteSpace(str)
                || (str.StartsWith("{") && str.EndsWith("}"))
                || (str.StartsWith("[") && str.EndsWith("]"));
        }

        private object DeserializeJson(object value, Type propertyType) 
            => Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString(), propertyType);
    }
}
