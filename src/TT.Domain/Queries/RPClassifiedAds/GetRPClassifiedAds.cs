using System.Collections.Generic;
using Highway.Data;
using TT.Domain.DTOs.RPClassifiedAds;
using TT.Domain.Entities.RPClassifiedAds;
using System;
using System.Linq;
using TT.Domain.Entities.Players;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetRPClassifiedAds : DomainQuery<RPClassifiedAdAndPlayerDetail>
    {
        public DateTime CutOff { get; set; }

        public override IEnumerable<RPClassifiedAdAndPlayerDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = (from adq in ctx.AsQueryable<RPClassifiedAd>()
                    join player in ctx.AsQueryable<Player>() // GROUP JOIN
                        on adq.OwnerMembershipId equals player.MembershipId into tempPlayers
                    where adq.RefreshTimestamp >= CutOff
                    select new { RPClassifiedAd = adq, Players = tempPlayers })
                    .ProjectToQueryable<RPClassifiedAdAndPlayerDetail>();

                return query;
            };

            return ExecuteInternal(context).ToList(); // execute SQL to prevent changes with LINQ later
        }
    }
}
