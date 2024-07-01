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
    public class GetItemsAtLocationVisibleToGameMode : DomainQuery<PlayPageItemDetail>
    {

        public string dbLocationName { get; set; }
        public int gameMode { get; set; }

        public override IEnumerable<PlayPageItemDetail> Execute(IDataContext context)
        {

            // treat SuperProtection same as Protection
            if (gameMode == (int)GameModeStatics.GameModes.Superprotection)
                gameMode = (int)GameModeStatics.GameModes.Protection;

            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Runes.Select(r => r.ItemSource))
                    .Include(i => i.SoulboundToPlayer)
                    .Where(i => i.dbLocationName == dbLocationName &&
                                (i.PvPEnabled == (int)GameModeStatics.GameModes.Any || i.PvPEnabled == gameMode))
                    .ToList();

                return items.Select(i => i.MapPlayPageItemDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
