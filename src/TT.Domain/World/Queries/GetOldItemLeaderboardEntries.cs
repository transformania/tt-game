using System.Collections.Generic;
using System.Data.Entity;
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
            ContextQuery = ctx =>
            {
                var entries = ctx.AsQueryable<ItemLeaderboardEntry>()
                    .Include(r => r.ItemSource)
                    .Where(r => r.RoundNumber == Round)
                    .OrderBy(r => r.Rank)
                    .ToList();

                return entries.Select(r => r.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}