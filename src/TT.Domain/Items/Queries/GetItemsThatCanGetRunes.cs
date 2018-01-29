using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetItemsThatCanGetRunes : DomainQuery<ItemRuneDetail>
    {
        public int OwnerId { get; set; }

        public override IEnumerable<ItemRuneDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>()
            .ProjectToQueryable<ItemRuneDetail>()
            .Where(i => i.Owner.Id == OwnerId && (
                            i.ItemSource.ItemType == PvPStatics.ItemType_Pet ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Shoes ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Hat ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Pants ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Underpants ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Shirt ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Undershirt ||
                            i.ItemSource.ItemType == PvPStatics.ItemType_Accessory 
            )
            
            );

            return ExecuteInternal(context);
        }
    }
}
