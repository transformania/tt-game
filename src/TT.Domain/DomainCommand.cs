using Highway.Data;

namespace TT.Domain
{
    public abstract class DomainCommand : Highway.Data.Command, IDomainCommand
    {
        protected virtual void Validate() { }

        protected void ExecuteInternal(IDataContext context)
        {
            Validate();
            // ReSharper disable once RedundantBaseQualifier
            base.Execute(context);
        }
    }

    public abstract class DomainCommand<T> : Highway.Data.Command, IDomainCommand<T>
    {
        public new abstract T Execute(IDataContext context);

        protected virtual void Validate() { }

        protected void ExecuteInternal(IDataContext context)
        {
            Validate();
            base.Execute(context);
        }
    }
}