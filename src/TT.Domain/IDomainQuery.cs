using System.Collections.Generic;
using Highway.Data;

namespace TT.Domain
{
    public interface IDomainQuery<out T>
    {
        IEnumerable<T> Execute(IDataContext context);
    }
}