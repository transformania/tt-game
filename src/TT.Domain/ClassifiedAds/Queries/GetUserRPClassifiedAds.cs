using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.ClassifiedAds.DTOs;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.ClassifiedAds.Queries
{
    public class GetUserRPClassifiedAds : DomainQuery<RPClassifiedAdDetail>
    {
        public string UserId { get; set; }

        public override IEnumerable<RPClassifiedAdDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = (from ad in ctx.AsQueryable<RPClassifiedAd>()
                             where ad.OwnerMembershipId == UserId
                             orderby ad.RefreshTimestamp descending
                             select ad)
                    .ProjectToQueryable<RPClassifiedAdDetail>();

                return query;
            };

            return ExecuteInternal(context);
        }
    }
}
