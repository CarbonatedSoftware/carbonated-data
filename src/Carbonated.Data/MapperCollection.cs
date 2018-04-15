using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbonated.Data
{
    public class MapperCollection
    {
        private List<Mapper> mappers = new List<Mapper>();

        public void Add<TEntity>(Mapper<TEntity> mapper)
        {
            mappers.Add(mapper);
        }

        public bool HasMapper<TEntity>()
        {
            return mappers.Any(m => m.EntityType == typeof(TEntity));
        }
    }
}
