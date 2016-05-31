using System;
using System.Collections.Generic;
using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetUserRPClassifiedAdsCount : DomainQuerySingle<int>
    {
        public string UserId { get; set; }

        public override int Execute(IDataContext context)
        {
            ContextQuery = ctx =>
                (from q in ctx.AsQueryable<RPClassifiedAd>()
                where q.OwnerMembershipId == UserId
                select q).Count();

            return ExecuteInternal(context);
        }
    }
}
