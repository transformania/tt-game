using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.World.DTOs;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Queries
{
    public class GetOldXpLeaderboardEntries : DomainQuery<XpLeaderboardEntryDetail>
    {
        public int Round { get; set; }

        public override IEnumerable<XpLeaderboardEntryDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<XpLeaderboardEntry>()
            .Where(r => r.RoundNumber == Round)
            .OrderBy(r => r.Rank)
            .ProjectToQueryable<XpLeaderboardEntryDetail>();
            return ExecuteInternal(context);
        }
    }
}
