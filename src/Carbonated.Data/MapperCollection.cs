using System;
using System.Collections.Generic;
using System.Data;
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

        public MapperCollection()
        {
            AddFrameworkTypes();
        }

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

        private void AddFrameworkTypes()
        {
            mappers.Add(new TypeMapper<bool>(GetValue<bool>));
            mappers.Add(new TypeMapper<byte>(GetValue<byte>));
            mappers.Add(new TypeMapper<short>(GetValue<short>));
            mappers.Add(new TypeMapper<int>(GetValue<int>));
            mappers.Add(new TypeMapper<long>(GetValue<long>));
            mappers.Add(new TypeMapper<float>(GetValue<float>));
            mappers.Add(new TypeMapper<double>(GetValue<double>));
            mappers.Add(new TypeMapper<decimal>(GetValue<decimal>));
            mappers.Add(new TypeMapper<DateTime>(GetValue<DateTime>));
            mappers.Add(new TypeMapper<Guid>(GetGuid));
            mappers.Add(new TypeMapper<char>(GetChar));
            mappers.Add(new TypeMapper<string>(GetValue<string>));
            mappers.Add(new TypeMapper<byte[]>(GetValue<byte[]>));

            mappers.Add(new TypeMapper<bool?>(GetNullableValue<bool?>));
            mappers.Add(new TypeMapper<byte?>(GetNullableValue<byte?>));
            mappers.Add(new TypeMapper<short?>(GetNullableValue<short?>));
            mappers.Add(new TypeMapper<int?>(GetNullableValue<int?>));
            mappers.Add(new TypeMapper<long?>(GetNullableValue<long?>));
            mappers.Add(new TypeMapper<float?>(GetNullableValue<float?>));
            mappers.Add(new TypeMapper<double?>(GetNullableValue<double?>));
            mappers.Add(new TypeMapper<decimal?>(GetNullableValue<decimal?>));
            mappers.Add(new TypeMapper<DateTime?>(GetNullableValue<DateTime?>));
            mappers.Add(new TypeMapper<Guid?>(GetNullableGuid));
            mappers.Add(new TypeMapper<char?>(GetNullableChar));
        }

        private T GetValue<T>(IDataRecord record) 
            => record.IsDBNull(0) ? default(T) : (T)Convert.ChangeType(record.GetValue(0), typeof(T));

        private T GetNullableValue<T>(IDataRecord record) 
            => record.IsDBNull(0) ? default(T) : (T)Convert.ChangeType(record.GetValue(0), Nullable.GetUnderlyingType(typeof(T)));

        private Guid GetGuid(IDataRecord record)
        {
            // If we're using SQL Server and the Guid is backed by a UniqueIdentifier
            // column, we can use GetValue. If it's backed by a string, we need to parse it
            // directly because ChangeType doesn't have a string => Guid conversion.
            if (record.GetFieldType(0) == typeof(Guid))
            {
                return GetValue<Guid>(record);
            }
            return record.IsDBNull(0) || string.IsNullOrEmpty(record.GetString(0)) ? Guid.Empty : Guid.Parse(record.GetString(0));
        }

        private Guid? GetNullableGuid(IDataRecord record)
        {
            // See note in GetGuid for details about parsing string values. We also need to
            // ensure that DbNull gets converted to null.
            if (record.GetFieldType(0) == typeof(Guid))
            {
                return record.IsDBNull(0) ? null : (Guid?)record.GetGuid(0);
            }
            return record.IsDBNull(0) || string.IsNullOrEmpty(record.GetString(0)) ? null : (Guid?)Guid.Parse(record.GetString(0));
        }

        private char GetChar(IDataRecord record)
        {
            // The SqlServer record treats all CHAR columns as strings, so we need to get
            // char values as strings.
            return record.IsDBNull(0) || string.IsNullOrEmpty(record.GetString(0)) ? default(char) : record.GetString(0)[0];
        }

        private char? GetNullableChar(IDataRecord record)
        {
            // Same basic logic as GetChar, but returning null instead of default for nulls.
            return record.IsDBNull(0) || string.IsNullOrEmpty(record.GetString(0)) ? null : (char?)record.GetString(0)[0];
        }
    }
}
