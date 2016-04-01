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
    }
}