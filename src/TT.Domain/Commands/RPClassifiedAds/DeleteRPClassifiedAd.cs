using Highway.Data;
using System.Linq;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class DeleteRPClassifiedAd : DomainCommand
    {
        public string UserId { get; set; }
        public int RPClassifiedAdId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<RPClassifiedAd>().FirstOrDefault(ad => ad.Id == RPClassifiedAdId);

                if (deleteMe == null)
                    throw new DomainException(string.Format("RPClassifiedAd with ID {0} was not found", RPClassifiedAdId));

                if (deleteMe.User.Id != UserId)
                    throw new DomainException(string.Format("User {0} does not own RPClassifiedAd {1}", UserId, RPClassifiedAdId));

                ctx.Remove(deleteMe);

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
