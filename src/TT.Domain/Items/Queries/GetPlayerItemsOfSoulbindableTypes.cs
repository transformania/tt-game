using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;
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
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.Owner)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Runes.Select(r => r.ItemSource))
                    .Include(i => i.Runes.Select(r => r.Owner))
                    .Include(i => i.Runes.Select(r => r.FormerPlayer))
                    .Include(i => i.EmbeddedOnItem.ItemSource)
                    .Include(i => i.EmbeddedOnItem.Owner)
                    .Include(i => i.EmbeddedOnItem.FormerPlayer)
                    .Include(i => i.SoulboundToPlayer)
                    .Where(i => i.Owner != null &&
                                i.Owner.Id == OwnerId &&
                                i.ItemSource.ItemType != PvPStatics.ItemType_Consumable &&
                                i.ItemSource.ItemType != PvPStatics.ItemType_Rune)
                    .ToList();

                return items.Select(i => i.MapToItemDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
