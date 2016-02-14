using Highway.Data;

namespace TT.Domain
{
    public interface IDomainCommand<out T> : ICommand
    {
        new T Execute(IDataContext context);
    }
}