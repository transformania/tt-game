using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
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
