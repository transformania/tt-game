using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetRestockItem : DomainQuerySingle<RestockItemDetail>
    {
        public int RestockItemId { get; set; }

        public override RestockItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<RestockItem>()
                            .Where(cr => cr.Id == RestockItemId)
                            .ProjectToFirstOrDefault<RestockItemDetail>();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RestockItemId <= 0)
                throw new DomainException("RestockItem Id must be greater than 0");
        }
    }
}
