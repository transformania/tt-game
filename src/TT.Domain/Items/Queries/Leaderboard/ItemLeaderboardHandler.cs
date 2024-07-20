using Highway.Data;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;
using TT.Domain.Items.Queries.Leaderboard.DTOs;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries.Leaderboard
{
    public class ItemLeaderboardHandler : RequestHandler<ItemLeaderboardRequest, IEnumerable<ItemLeaderboardDetail>>
    {
        private readonly IDataContext context;

        public ItemLeaderboardHandler(IDataContext context)
        {
            this.context = context;
        }

        protected override IEnumerable<ItemLeaderboardDetail> Handle(ItemLeaderboardRequest message)
        {
            var item = context.AsQueryable<Item>()
                .Include(i => i.ItemSource)
                .Include(i => i.FormerPlayer.ItemXP)
                .Where(i => i.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                .OrderByDescending(i => i.Level)
                .ThenByDescending(i => i.FormerPlayer.ItemXP.Amount)
                .Take(message.Limit)
                .ToList();

            return item.Select(i => new ItemLeaderboardDetail
                {
                    Item = new ItemLeaderboardItemDetail
                    {
                        Id = i.Id,
                        FormerPlayer = i.FormerPlayer.MapToLeaderboardPlayerDto(),
                        ItemSource = i.ItemSource.MapToLeaderboardItemSourceDetail(),
                        IsPermanent = i.IsPermanent,
                        Level = i.Level
                    },
                    FormerPlayer = i.FormerPlayer.MapToLeaderboardPlayerDto(),
                    ItemSource = i.ItemSource.MapToLeaderboardItemSourceDetail(),
                    ItemXP = i.FormerPlayer.ItemXP == null ? null : new ItemLeaderboardInanimateXPDetail
                    {
                        Id = i.FormerPlayer.ItemXP.Id,
                        Amount = i.FormerPlayer.ItemXP.Amount
                    }
                })
                .AsQueryable()
                .Memoize();
        }
    }
}
