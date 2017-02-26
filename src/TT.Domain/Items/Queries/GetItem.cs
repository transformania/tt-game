using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItem : DomainQuerySingle<ItemDetail>
    {
        public int ItemId { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Item>()
                           .Where(p => p.Id == ItemId)
                           .ProjectToFirstOrDefault<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}