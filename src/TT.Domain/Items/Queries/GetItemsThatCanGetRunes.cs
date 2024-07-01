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
    public class GetItemsThatCanGetRunes : DomainQuery<ItemRuneDetail>
    {
        public int OwnerId { get; set; }

        public override IEnumerable<ItemRuneDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var runes = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.Owner)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Runes.Select(r => r.ItemSource))
                    .Include(i => i.Runes.Select(r => r.Owner))
                    .Include(i => i.Runes.Select(r => r.FormerPlayer))
                    .Where(i => i.Owner.Id == OwnerId && (
                        i.ItemSource.ItemType == PvPStatics.ItemType_Pet ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Shoes ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Hat ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Pants ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Underpants ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Shirt ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Undershirt ||
                        i.ItemSource.ItemType == PvPStatics.ItemType_Accessory
                    ))
                    .ToList();

                return runes.Select(i => i.MapToItemRuneDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
