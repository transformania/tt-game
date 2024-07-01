using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetItemRune : DomainQuerySingle<ItemRuneDetail>
    {
        public int ItemId { get; set; }

        public override ItemRuneDetail Execute(IDataContext context)
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
                    .FirstOrDefault(p => p.Id == ItemId);

                return item.MapToItemRuneDto();
            };

            return ExecuteInternal(context);
        }
    }
}