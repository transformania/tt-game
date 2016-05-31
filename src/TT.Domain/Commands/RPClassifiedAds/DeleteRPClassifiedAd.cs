using Highway.Data;
using System.Linq;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class DeleteRPClassifiedAd : DomainCommand
    {
        public int RPClassifiedAdId { get; set; }

        /// <summary>
        /// If null, this <see cref="DomainCommand"/> does not check if user with id <see cref="UserId"/> owns this ad.
        /// </summary>
        public string UserId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<RPClassifiedAd>()
                            where q.Id == RPClassifiedAdId
                            select q;

                var ad = query.FirstOrDefault();

                if (ad == null)
                    throw new RPClassifiedAdException(string.Format("RPClassifiedAd with ID {0} was not found", RPClassifiedAdId))
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist.",
                        ExType = RPClassifiedAdException.ExceptionType.NoAdfound
                    };

                if (UserId != null && ad.OwnerMembershipId != UserId)
                    throw new RPClassifiedAdException(string.Format("User {0} does not own RP Classified Ad {1}", UserId, RPClassifiedAdId))
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad.",
                        ExType = RPClassifiedAdException.ExceptionType.NotOwner
                    };

                ctx.Remove(ad);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId <= 0)
                throw new RPClassifiedAdException("RPClassifiedAd Id must be greater than 0")
                {
                    ExType = RPClassifiedAdException.ExceptionType.InvalidInput
                };
        }
    }
}
