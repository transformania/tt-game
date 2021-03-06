﻿using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.World.DTOs;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Queries
{
    public class GetOldPvPLeaderboardEntries : DomainQuery<PvPLeaderboardEntryDetail>
    {
        public int Round { get; set; }

        public override IEnumerable<PvPLeaderboardEntryDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<PvPLeaderboardEntry>()
            .Where(r => r.RoundNumber == Round)
            .OrderBy(r => r.Rank)
            .ProjectToQueryable<PvPLeaderboardEntryDetail>();
            return ExecuteInternal(context);
        }
    }
}
