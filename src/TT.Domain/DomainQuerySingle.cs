using Highway.Data;

namespace TT.Domain
{
    public abstract class DomainQuerySingle<T> : Scalar<T>, IDomainQuerySingle<T>
    {
        public abstract T Execute(IDataContext context);

        protected virtual void Validate() { }

        protected T ExecuteInternal(IDataContext context)
        {
            Validate();
            return base.Execute(context);
        }
    }
}