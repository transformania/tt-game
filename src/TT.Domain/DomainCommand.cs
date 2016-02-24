using Highway.Data;

namespace TT.Domain
{
    public abstract class DomainCommand<T> : Highway.Data.Command, IDomainCommand<T>
    {
        public new abstract T Execute(IDataContext context);

        protected void ExecuteInternal(IDataContext context)
        {
            base.Execute(context);
        }
    }
}