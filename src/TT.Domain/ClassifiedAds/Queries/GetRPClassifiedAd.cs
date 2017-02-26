using System.Linq;
using Highway.Data;
using TT.Domain.ClassifiedAds.DTOs;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.ClassifiedAds.Queries
{
    public class GetRPClassifiedAd : DomainQuerySingle<RPClassifiedAdDetail>
    {
        public int RPClassifiedAdId { get; set; }

        public string UserId { get; set; }

        public bool CheckUserId { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RPClassifiedAdNotFoundException"></exception>
        /// <exception cref="RPClassifiedAdNotOwnerException"></exception>
        /// <exception cref="RPClassifiedAdInvalidInputException"></exception>
        public override RPClassifiedAdDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<RPClassifiedAd>()
                            where q.Id == RPClassifiedAdId
                            select q;

                var ad = query.ProjectToFirstOrDefault<RPClassifiedAdDetail>();

                if (ad == null)
                {
                    throw new RPClassifiedAdNotFoundException("RPClassifiedAd with ID {0} could not be found", RPClassifiedAdId)
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist.",
                    };
                }

                if (CheckUserId && ad.OwnerMembershipId != UserId)
                {
                    throw new RPClassifiedAdNotOwnerException("User {0} does not own RP Classified Ad Id {1}", UserId, RPClassifiedAdId)
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad.",
                    };
                }

                return ad;
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId < 0)
                throw new RPClassifiedAdInvalidInputException("The ID {0} can not be negative", RPClassifiedAdId);
        }
    }
}
