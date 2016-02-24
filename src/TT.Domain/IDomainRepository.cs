using Highway.Data;

namespace TT.Domain
{
    public interface IDomainRepository : IRepository
    {
        T Execute<T>(IDomainCommand<T> command);
    }
}