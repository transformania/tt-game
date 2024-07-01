using System.Collections.Generic;
using System.Data.Entity;
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
            ContextQuery = ctx =>
            {
                var entries = ctx.AsQueryable<XpLeaderboardEntry>()
                    .Include(r => r.FormSource)
                    .Where(r => r.RoundNumber == Round)
                    .OrderBy(r => r.Rank)
                    .ToList();

                return entries.Select(r => r.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
