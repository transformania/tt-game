using Highway.Data;
using System.Linq;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class RefreshRPClassifiedAd : DomainCommand
    {
        public string UserId { get; set; }

        public int RPClassifiedAdId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var ad = ctx.AsQueryable<RPClassifiedAd>().SingleOrDefault(a => a.Id == RPClassifiedAdId);

                if (ad == null)
                    throw new DomainException(string.Format("RPClassifiedAdId with ID {0} could not be found", RPClassifiedAdId));

                if (ad.User.Id == UserId)
                    throw new DomainException(string.Format("User {0} does not own RPClassifiedAdId {1}", UserId, RPClassifiedAdId));

                ad.Refresh(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId <= 0)
                throw new DomainException("RPClassifiedAd Id must be greater than 0");
        }
    }
}
