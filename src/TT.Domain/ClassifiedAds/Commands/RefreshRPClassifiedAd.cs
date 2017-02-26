using System.Linq;
using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.ClassifiedAds.Commands
{
    public class RefreshRPClassifiedAd : DomainCommand
    {
        public string UserId { get; set; }

        public int RPClassifiedAdId { get; set; }

        public bool CheckUserId { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="RPClassifiedAdNotFoundException"></exception>
        /// <exception cref="RPClassifiedAdNotOwnerException"></exception>
        /// <exception cref="RPClassifiedAdException"></exception>
        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<RPClassifiedAd>()
                            where q.Id == RPClassifiedAdId
                            select q;

                var ad = query.FirstOrDefault();
                
                if (ad == null)
                {
                    throw new RPClassifiedAdNotFoundException("RPClassifiedAdId with ID {0} could not be found", RPClassifiedAdId)
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist."
                    };
                }

                if (CheckUserId && ad.OwnerMembershipId != UserId)
                {
                    throw new RPClassifiedAdNotOwnerException("User {0} does not own RPClassifiedAdId {1}", UserId, RPClassifiedAdId)
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad."
                    };
                }

                ad.Refresh(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId <= 0)
                throw new RPClassifiedAdInvalidInputException("RPClassifiedAd Id must be greater than 0");
        }
    }
}
