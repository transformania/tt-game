using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Queries
{
    public class GetTome : DomainQuerySingle<TomeDetail>
    {
        public int TomeId { get; set; }

        public override TomeDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>()
                            .Where(cr => cr.Id == TomeId)
                            .ProjectToFirstOrDefault<TomeDetail>();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (TomeId <= 0)
                throw new DomainException("Tome Id must be greater than 0");
        }
    }
}
