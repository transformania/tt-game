using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.World.DTOs;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Queries
{
    public class GetOldItemLeaderboardEntries : DomainQuery<ItemLeaderboardEntryDetail>
    {
        public int Round { get; set; }

        public override IEnumerable<ItemLeaderboardEntryDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<ItemLeaderboardEntry>()
            .Where(r => r.RoundNumber == Round)
            .OrderBy(r => r.Rank)
            .ProjectToQueryable<ItemLeaderboardEntryDetail>();
            return ExecuteInternal(context);
        }
    }
}