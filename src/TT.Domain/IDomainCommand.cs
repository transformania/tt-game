using Highway.Data;

namespace TT.Domain
{
    public interface IDomainCommand
    {
        void Execute(IDataContext context);
    }

    public interface IDomainCommand<out T>
    {
        T Execute(IDataContext context);
    }
}