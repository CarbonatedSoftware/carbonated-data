using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbonated.Data
{
    public class PropertyMapper<TEntity>
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

        public PropertyMapper<TEntity> Map<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, defaultCondition);

        public PropertyMapper<TEntity> MapOptional<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.Optional);

        public PropertyMapper<TEntity> MapRequired<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.Required);

        public PropertyMapper<TEntity> MapNotNull<P>(Expression<Func<TEntity, P>> property, string field) 
            => MapProp(property, field, PopulationCondition.NotNull);

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

        private PropertyMapper<TEntity> MapProp<P>(Expression<Func<TEntity, P>> property, string field, PopulationCondition condition)
        {
            var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
            if (PropertyIsMappedToDifferentField(field, prop))
            {
                throw new Exception($"Field cannot be mapped to more than one property: {field}");
            }

            RemoveGeneratedMapping(prop);
            mappings.Add(new PropertyMapInfo(field, prop) { Condition = condition });

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
    }
}
