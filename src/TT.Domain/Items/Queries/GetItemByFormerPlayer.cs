using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemByFormerPlayer : DomainQuerySingle<ItemDetail>
    {
        public int PlayerId { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Item>()
                           .Where(p => p.FormerPlayer.Id == PlayerId)
                           .ProjectToFirstOrDefault<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}