using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Statics;

namespace TT.Domain.Queries.Item
{
    public class GetHighestLevelPlayerItems : DomainQuery<ItemFormerPlayerDetail>
    {
        public int Limit { get; set; }

        public override IEnumerable<ItemFormerPlayerDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Entities.Items.Item>().ProjectToQueryable<ItemFormerPlayerDetail>()
                .Where(i => i.FormerPlayer != null && i.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                .OrderByDescending(i => i.Level)
                .ThenByDescending(i => i.FormerPlayer.ItemXP.Amount)
                .Take(Limit);
            return ExecuteInternal(context);
        }
    }
}
