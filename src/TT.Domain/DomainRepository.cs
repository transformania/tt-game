using System.Collections.Generic;
using Highway.Data;

namespace TT.Domain
{
    public class DomainRepository : Repository, IDomainRepository
    {
        public new IDataContext Context { get; private set; }

        public DomainRepository(IDataContext context)
            :base(context)
        {
            Context = context;
        }

        public void Execute(IDomainCommand command)
        {
            command.Execute(Context);
        }

        public T Execute<T>(IDomainCommand<T> command)
        {
            return command.Execute(Context);
        }

        public IEnumerable<T> Find<T>(IDomainQuery<T> query)
        {
            return query.Execute(Context);
        }

        public T FindSingle<T>(IDomainQuerySingle<T> query)
        {
            return query.Execute(Context);
        }

        #region IDisposable Support
        public bool Disposed { get { return disposedValue; } }
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes <see cref="Context"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}