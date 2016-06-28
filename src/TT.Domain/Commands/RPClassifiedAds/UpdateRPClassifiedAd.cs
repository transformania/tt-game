using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class UpdateRPClassifiedAd : DomainCommand
    {
        public string UserId { get; set; }
        public int RPClassifiedAdId { get; set; }

        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }

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

                var ad = query.SingleOrDefault();

                if (ad == null)
                {
                    throw new RPClassifiedAdNotFoundException("RPClassifiedAd with ID {0} could not be found", RPClassifiedAdId)
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist.",
                    };
                }

                if (CheckUserId && ad.OwnerMembershipId != UserId)
                {
                    throw new RPClassifiedAdNotOwnerException("User {0} does not own RPClassifiedAdId {1}", UserId, RPClassifiedAdId)
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad.",
                    };
                }

                ad.Update(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            // assert the title field is not too short
            if ((Title?.Length ?? 0) < 5)
                throw new RPClassifiedAdInvalidInputException("The title is too short.");

            // assert the title field is not too long
            if (Title.Length > 35)
                throw new RPClassifiedAdInvalidInputException("The title is too long.");

            // assert the text fields are not too short
            if ((Text?.Length ?? 0) < 50)
                throw new RPClassifiedAdInvalidInputException("The description is too short.");

            // assert the text fields are not too long
            if (Text.Length > 300)
                throw new RPClassifiedAdInvalidInputException("The description is too long.");

            // assert the yes field is not too long
            if ((YesThemes?.Length ?? 0) > 200)
                throw new RPClassifiedAdInvalidInputException("The desired themes field is too long.");

            // assert the no field is not too long
            if ((NoThemes?.Length ?? 0) > 200)
                throw new RPClassifiedAdInvalidInputException("The undesired themes field is too long.");

            // assert the timezone fields is not too long
            if ((PreferredTimezones?.Length ?? 0) > 70)
                throw new RPClassifiedAdInvalidInputException("The preferred timezones field is too long.");
        }
    }
}
