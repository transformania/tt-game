using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class UpdateRPClassifiedAd : DomainCommand
    {
        /// <summary>
        /// If null, this <see cref="DomainCommand"/> does not check if user with id <see cref="UserId"/> owns this ad.
        /// </summary>
        public string UserId { get; set; }
        public int RPClassifiedAdId { get; set; }

        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<RPClassifiedAd>()
                            where q.Id == RPClassifiedAdId
                            select q;

                var ad = query.SingleOrDefault();

                if (ad == null)
                    throw new RPClassifiedAdException(string.Format("RPClassifiedAd with ID {0} could not be found", RPClassifiedAdId))
                    {
                        UserFriendlyError = "This RP Classified Ad doesn't exist.",
                        ExType = RPClassifiedAdException.ExceptionType.NoAdfound
                    };

                if (UserId != null && ad.OwnerMembershipId != UserId)
                    throw new RPClassifiedAdException(string.Format("User {0} does not own RPClassifiedAdId {1}", UserId, RPClassifiedAdId))
                    {
                        UserFriendlyError = "You do not own this RP Classified Ad.",
                        ExType = RPClassifiedAdException.ExceptionType.NotOwner
                    };

                ad.Update(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            // assert the title field is not too short
            if (Title == null || Title.Length < 5)
                throw new RPClassifiedAdException("The title is too short.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the title field is not too long
            if (Title.Length > 35)
                throw new RPClassifiedAdException("The title is too long.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the text fields are not too short
            if (Text == null || Text.Length < 50)
                throw new RPClassifiedAdException("The description is too short.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the text fields are not too long
            if (Text.Length > 300)
                throw new RPClassifiedAdException("The description is too long.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the yes field is not too long
            if (YesThemes.Length > 200)
                throw new RPClassifiedAdException("The desired themes field is too long.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the no field is not too long
            if (NoThemes.Length > 200)
                throw new RPClassifiedAdException("The undesired themes field is too long.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };

            // assert the timezone fields is not too long
            if (PreferredTimezones.Length > 70)
                throw new RPClassifiedAdException("The preferred timezones field is too long.") { ExType = RPClassifiedAdException.ExceptionType.InvalidInput };
        }
    }
}
