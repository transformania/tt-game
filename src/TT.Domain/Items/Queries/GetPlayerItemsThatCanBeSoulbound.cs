using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetPlayerItemsThatCanBeSoulbound : DomainQuery<ItemDetail>
    {
        public int OwnerId { get; set; }

        public override IEnumerable<ItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Item>()
                    .Where(i => i.Owner != null && 
                        i.Owner.Id == OwnerId &&
                        i.FormerPlayer != null &&
                        i.IsPermanent &&
                        i.SoulboundToPlayer == null &&
                        i.ConsentsToSoulbinding
                    )
                    .Include(i => i.Owner)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.SoulboundToPlayer)
                    .ProjectToQueryable<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
