using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetHighestLevelPlayerItems : DomainQuery<ItemFormerPlayerDetail>
    {
        public int Limit { get; set; }

        public override IEnumerable<ItemFormerPlayerDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemFormerPlayerDetail>()
                .Where(i => i.FormerPlayer != null && i.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                .OrderByDescending(i => i.Level)
                .ThenByDescending(i => i.FormerPlayer.ItemXP.Amount)
                .Take(Limit);
            return ExecuteInternal(context);
        }
    }
}
