using Highway.Data;

namespace TT.Domain
{
    public abstract class DomainQuerySingle<T> : Scalar<T>, IDomainQuerySingle<T>
    {
        public new abstract T Execute(IDataContext context);

        protected virtual void Validate() { }

        protected T ExecuteInternal(IDataContext context)
        {
            Validate();
            var query = base.Execute(context);
            DomainRegistry.Repository.Dispose();
            return query;
        }
    }
}