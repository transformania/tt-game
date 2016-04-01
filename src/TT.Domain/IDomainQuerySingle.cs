using Highway.Data;

namespace TT.Domain
{
    public interface IDomainQuerySingle<out T>
    {
        T Execute(IDataContext context);
    }
}