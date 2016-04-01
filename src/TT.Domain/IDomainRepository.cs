using System.Collections.Generic;

namespace TT.Domain
{
    public interface IDomainRepository
    {
        void Execute(IDomainCommand command);
        T Execute<T>(IDomainCommand<T> command);
        IEnumerable<T> Find<T>(IDomainQuery<T> query);
        T FindSingle<T>(IDomainQuerySingle<T> query);
    }
}