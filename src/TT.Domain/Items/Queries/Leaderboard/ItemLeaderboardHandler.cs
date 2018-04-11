using AutoMapper;
using AutoMapper.QueryableExtensions;
using Highway.Data;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Queries.Leaderboard.DTOs;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries.Leaderboard
{
    public class ItemLeaderboardHandler : IRequestHandler<ItemLeaderboardRequest, IEnumerable<ItemLeaderboardDetail>>
    {
        private readonly IDataContext context;
        private readonly IMapper mapper;

        public ItemLeaderboardHandler(IDataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<ItemLeaderboardDetail> Handle(ItemLeaderboardRequest message)
        {
            var query = context.AsQueryable<Item>()
                .ProjectTo<ItemLeaderboardDetail>(mapper.ConfigurationProvider)
                .Where(i => i.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                .OrderByDescending(i => i.Item.Level)
                .ThenByDescending(i => i.ItemXP.Amount)
                .Take(message.Limit)
                .Memoize();

            return mapper.Map<IEnumerable<ItemLeaderboardDetail>>(query);
        }
    }
}
