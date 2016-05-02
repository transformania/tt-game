using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItem : DomainQuerySingle<ItemDetail>
    {
        public int ItemId { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Items.Item>()
                           .Where(p => p.Id == ItemId)
                           .ProjectToFirstOrDefault<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}