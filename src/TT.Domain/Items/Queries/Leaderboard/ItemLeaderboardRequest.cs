using MediatR;
using System.Collections.Generic;
using TT.Domain.Items.Queries.Leaderboard.DTOs;

namespace TT.Domain.Items.Queries.Leaderboard
{
    public class ItemLeaderboardRequest : IRequest<IEnumerable<ItemLeaderboardDetail>>
    {
        public int Limit { get; set; }
    }
}
