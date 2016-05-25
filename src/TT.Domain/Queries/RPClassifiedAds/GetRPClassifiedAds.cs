using System.Collections.Generic;
using Highway.Data;
using TT.Domain.DTOs.RPClassifiedAds;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetRPClassifiedAds : DomainQuery<RPClassifiedAdDetail>
    {
        public override IEnumerable<RPClassifiedAdDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => 
                ctx.AsQueryable<RPClassifiedAd>().ProjectToQueryable<RPClassifiedAdDetail>();

            return ExecuteInternal(context);
        }
    }
}
