using System.Collections.Generic;
using Highway.Data;
using System.Linq;
using TT.Domain.DTOs.RPClassifiedAds;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetUserRPClassifiedAds : DomainQuery<RPClassifiedAdDetail>
    {
        public string UserId { get; set; }

        public override IEnumerable<RPClassifiedAdDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = (from q in ctx.AsQueryable<RPClassifiedAd>()
                          where q.OwnerMembershipId == UserId
                          select q).ProjectToQueryable<RPClassifiedAdDetail>();

                return query;
            };

            return ExecuteInternal(context).ToList(); // execute SQL to prevent changes with LINQ later
        }
    }
}
