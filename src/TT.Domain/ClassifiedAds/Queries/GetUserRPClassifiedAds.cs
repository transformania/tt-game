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
                var ads = ctx.AsQueryable<RPClassifiedAd>()
                    .Where(cr => cr.OwnerMembershipId == UserId)
                    .OrderByDescending(cr => cr.RefreshTimestamp)
                    .ToList();

                return ads.Select(cr => cr.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
