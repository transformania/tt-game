using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItemSource : DomainQuerySingle<ItemSourceDetail>
    {
        public int ItemSourceId { get; set; }

        public override ItemSourceDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ItemSource>()
                           .Where(p => p.Id == ItemSourceId)
                           .ProjectToFirstOrDefault<ItemSourceDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
