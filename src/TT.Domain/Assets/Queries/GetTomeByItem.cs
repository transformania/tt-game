using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Queries
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
