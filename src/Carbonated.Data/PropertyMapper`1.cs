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

        public PropertyMapper()
        {
            mappings = GenerateDefaultMappings();
        }

        internal IEnumerable<PropertyMapInfo> Mappings => mappings;

        IList<PropertyMapInfo> GenerateDefaultMappings()
        {
            return typeof(TEntity)
                .GetProperties().Where(p => p.CanWrite)
                .Select(prop => new PropertyMapInfo(prop.Name, prop))
                .ToList();
        }

        public PropertyMapper<TEntity> Map<P>(Expression<Func<TEntity, P>> property, string field)
        {
            var prop = (PropertyInfo)((MemberExpression)property.Body).Member;
            if (PropertyIsMappedToDifferentField(field, prop))
            {
                throw new Exception($"Field cannot be mapped to more than one property: {field}");
            }

            RemoveGeneratedMapping(prop);
            mappings.Add(new PropertyMapInfo(field, prop));

            return this;
        }

        private bool PropertyIsMappedToDifferentField(string field, PropertyInfo prop) 
            => mappings.Any(m => m.Field == field && m.Property.Name != prop.Name);

        private void RemoveGeneratedMapping(PropertyInfo prop)
        {
            var existing = mappings.FirstOrDefault(m => m.Property.Name == prop.Name);
            if (existing != null)
            {
                mappings.Remove(existing);
            }
        }
    }
}
