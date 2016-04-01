using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTomeByItem : DomainQuerySingle<TomeDetail>
    {
        public int ItemSourceId { get; set; }

        public override TomeDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>()
                            .Where(cr => cr.BaseItem.Id == ItemSourceId)
                            .ProjectToFirstOrDefault<TomeDetail>();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ItemSourceId <= 0)
                throw new DomainException("ItemSourceID must be a number greater than 0");
        }
    }
}
