using System.Linq;
using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.ClassifiedAds.Commands
{
    public class DeleteRPClassifiedAd : DomainCommand
    {
        public int RPClassifiedAdId { get; set; }

        public string UserId { get; set; }

        public bool CheckUserId { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="RPClassifiedAdNotFoundException"></exception>
        /// <exception cref="RPClassifiedAdNotOwnerException"></exception>
        /// <exception cref="RPClassifiedAdInvalidInputException"></exception>
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
                    throw new RPClassifiedAdNotFoundException("RPClassifiedAd with ID {0} was not found", RPClassifiedAdId)
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist."
                    };
                }

                if (CheckUserId && ad.OwnerMembershipId != UserId)
                {
                    throw new RPClassifiedAdNotOwnerException(
                        "User {0} does not own RP Classified Ad {1}", UserId, RPClassifiedAdId)
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad."
                    };
                }

                ctx.Remove(ad);

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
