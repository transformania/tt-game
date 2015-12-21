using System.Collections.Generic;

namespace tfgame.dbModels
{
    /// <summary>
    /// Base class for executing queries which return multiple items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Query<T>
    {
        internal abstract IList<T> Find();

        public IList<T> List()
        {
            return DomainRegistry.Root.Find(this);
        }
    }

    /// <summary>
    /// Base class for executing queries which return a single item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QuerySingle<T>
    {
        internal abstract T FindSingle();

        public T Find()
        {
            return DomainRegistry.Root.Find(this);
        }
    }
}