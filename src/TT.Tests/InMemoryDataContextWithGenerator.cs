using System.Linq;
using Highway.Data.Contexts;
using TT.Domain.Entities;
using TT.Tests.Utilities;

namespace TT.Tests
{
    public class InMemoryDataContextWithGenerator : InMemoryDataContext
    {
        private int nextId;

        public InMemoryDataContextWithGenerator()
        {
            nextId = 1;
        }

        public override T Add<T>(T item)
        {
            if (item is Entity<int>)
            {
                var entity = GetEntity<int, T>(item);
                var prop = entity.GetType().GetProperties().Single(p => p.Name == "Id");
                var currVal = (int)prop.GetValue(entity);
                
                if (currVal == 0)
                    ReflectionHelper.SetProtectedProperty(prop, entity, nextId);
            }
            else if (item is Entity<string>)
            {
                var entity = GetEntity<string, T>(item);
                var prop = entity.GetType().GetProperties().Single(p => p.Name == "Id");
                var currVal = (string)prop.GetValue(entity);

                if (currVal == null)
                    ReflectionHelper.SetProtectedProperty(prop, entity, nextId.ToString());
            }

            nextId++;

            return base.Add(item);
        }

        private Entity<TKey> GetEntity<TKey, T>(T item)
        {
            return item as Entity<TKey>;
        }
    }
}