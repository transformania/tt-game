using Highway.Data;
using TT.Domain.DTOs.RPClassifiedAds;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;

namespace TT.Domain.Queries.RPClassifiedAds
{
    public class GetRPClassifiedAd : DomainQuerySingle<RPClassifiedAdDetail>
    {
        public int RPClassifiedAdId { get; set; }

        public override RPClassifiedAdDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<RPClassifiedAd>()
                            .Where(ad => ad.Id == RPClassifiedAdId)
                            .ProjectToFirstOrDefault<RPClassifiedAdDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
