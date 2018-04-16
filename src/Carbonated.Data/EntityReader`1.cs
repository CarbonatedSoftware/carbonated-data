using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Carbonated.Data
{
    /// <summary>
    /// Iterates over a data reader, converting each record to an entity as the rows are
    /// enumerated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to read.</typeparam>
    public class EntityReader<TEntity> : IEnumerable<TEntity>, IDisposable
    {
        private readonly IDataReader dataReader;
        private readonly Mapper<TEntity> mapper;

        /// <summary>
        /// Creates an entity reader, wrapping the provided data reader. The specified
        /// mapper is used to instantiate and populate the entities as they are read.
        /// </summary>
        /// <param name="dataReader">The data reader to wrap.</param>
        /// <param name="mapper">The mapper to user during record conversion.</param>
        public EntityReader(IDataReader dataReader, Mapper<TEntity> mapper)
        {
            this.dataReader = dataReader;
            this.mapper = mapper;
        }

        /// <summary>
        /// Disposes the entity reader and its underlying data reader.
        /// </summary>
        public void Dispose() => dataReader.Dispose();

        /// <summary>
        /// Gets a value indicating whether the reader is closed.
        /// </summary>
        public bool IsClosed => dataReader.IsClosed;

        /// <summary>
        /// Returns the enumerator over the data reader. The reader will close when the
        /// source data reader has no more records.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            try
            {
                var record = new Record(dataReader);
                while (dataReader.Read())
                {
                    yield return mapper.CreateInstance(record);
                }
            }
            finally
            {
                dataReader.Close();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
