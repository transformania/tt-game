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
        private DateTime time = DateTime.MinValue;
        private TimeSpan cutOff;
        public TimeSpan CutOff
        {
            get { return cutOff; }
            set { cutOff = value; time = DateTime.UtcNow.Add(value.Negate()); }
        }

        public override IEnumerable<RPClassifiedAdAndPlayerDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = (from ad in ctx.AsQueryable<RPClassifiedAd>()
                             join player in ctx.AsQueryable<Player>() // GROUP JOIN
                                 on ad.OwnerMembershipId equals player.MembershipId into players
                             where ad.RefreshTimestamp >= time
                             orderby ad.RefreshTimestamp descending
                             select new { RPClassifiedAd = ad, Players = players })
                    .ProjectToQueryable<RPClassifiedAdAndPlayerDetail>();

                return query;
            };

            return ExecuteInternal(context).ToList(); // execute SQL to prevent changes with LINQ later
        }
    }
}
