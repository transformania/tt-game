using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetItem : DomainQuerySingle<ItemDetail>
    {
        public int ItemId { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var item = ctx
                    .AsQueryable<Item>()
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
                    .FirstOrDefault(p => p.Id == ItemId);

                return item.MapToItemDto();
            };

            return ExecuteInternal(context);
        }
    }
}