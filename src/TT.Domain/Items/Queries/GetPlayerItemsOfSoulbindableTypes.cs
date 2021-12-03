using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetPlayerItemsOfSoulbindableTypes : DomainQuery<ItemDetail>
    {
        public int OwnerId { get; set; }

        public override IEnumerable<ItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Item>()
                    .ProjectToQueryable<ItemDetail>()
                    .Where(i => i.Owner != null &&
                                i.Owner.Id == OwnerId &&
                                i.ItemSource.ItemType != PvPStatics.ItemType_Consumable &&
                                i.ItemSource.ItemType != PvPStatics.ItemType_Rune);
            };

            return ExecuteInternal(context);
        }
    }
}
