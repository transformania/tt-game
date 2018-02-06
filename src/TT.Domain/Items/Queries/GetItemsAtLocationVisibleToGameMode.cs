using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetItemsAtLocationVisibleToGameMode : DomainQuery<ItemDetail>
    {

        public string dbLocationName { get; set; }
        public int gameMode { get; set; }

        public override IEnumerable<ItemDetail> Execute(IDataContext context)
        {

            // treat SuperProtection same as Protection
            if (gameMode == GameModeStatics.SuperProtection)
                gameMode = GameModeStatics.Protection;

            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemDetail>()
                .Where(i => i.dbLocationName == dbLocationName &&
                (i.PvPEnabled == GameModeStatics.Any || i.PvPEnabled == gameMode));

            return ExecuteInternal(context);
        }
    }
}
