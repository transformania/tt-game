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
                ctx.AsQueryable<RPClassifiedAd>().Where(ad => ad.User.Id == UserId).ProjectToQueryable<RPClassifiedAdDetail>();

            return ExecuteInternal(context);
        }
    }
}
