using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Statics;

namespace TT.Domain.Queries.Item
{
    public class GetItemsAtLocationVisibleToGameMode : DomainQuery<ItemListingDetail>
    {

        public string dbLocationName { get; set; }
        public int gameMode { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {

            // treat SuperProtection same as Protection
            if (gameMode == GameModeStatics.SuperProtection)
                gameMode = GameModeStatics.Protection;

            ContextQuery = ctx => ctx.AsQueryable<Entities.Items.Item>().ProjectToQueryable<ItemListingDetail>().Where(i => i.dbLocationName == dbLocationName &&
            (i.PvPEnabled == GameModeStatics.Any || i.PvPEnabled == gameMode));
            return ExecuteInternal(context);
        }
    }
}
