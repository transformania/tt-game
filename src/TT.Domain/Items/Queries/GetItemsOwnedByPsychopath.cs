using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetItemsOwnedByPsychopath : DomainQuery<ItemDetailForPsycho>
    {

        public int OwnerId { private get; set; }

        public override IEnumerable<ItemDetailForPsycho> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.Owner)
                    .Include(i => i.FormerPlayer)
                    .Where(i => i.Owner != null && i.Owner.Id == OwnerId)
                    .ToList();

                return items.Select(i => new ItemDetailForPsycho
                {
                    Id = i.Id,
                    Owner = i.Owner.MapToPlayerForPsycho(),
                    FormerPlayer = i.FormerPlayer.MapToPlayerForPsycho(),
                    ItemSource = new ItemSourceForPsycho
                    {
                        FriendlyName = i.ItemSource.FriendlyName,
                        ItemType = i.ItemSource.ItemType
                    }
                }).AsQueryable();
            };
            return ExecuteInternal(context);
        }
    }
}