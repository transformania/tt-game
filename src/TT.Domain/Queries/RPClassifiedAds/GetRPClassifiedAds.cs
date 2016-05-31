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
                var adl = (from adq in ctx.AsQueryable<RPClassifiedAd>()
                          join player in ctx.AsQueryable<Player>() on adq.OwnerMembershipId equals player.MembershipId into tempPlayers
                          from players in tempPlayers.DefaultIfEmpty() // LEFT JOIN
                          where adq.RefreshTimestamp >= CutOff
                          select new { RPClassifiedAd = adq, Player = players });

                var ads =  adl.ProjectToQueryable<RPClassifiedAdAndPlayerDetail>();

                return ads;
            };

            return ExecuteInternal(context).ToList(); // execute SQL to prevent changes with LINQ later
        }


    }
}
