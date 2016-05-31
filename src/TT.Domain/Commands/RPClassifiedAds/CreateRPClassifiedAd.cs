using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using System.Linq;
using TT.Domain.Queries.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class CreateRPClassifiedAd : DomainCommand<int>
    {
        public string UserId { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public string PreferredTimezones { get; set; }

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
                var query = from q in ctx.AsQueryable<User>()
                            where q.Id == UserId
                            select q;

                var user = query.SingleOrDefault();

                if (user == null)
                    throw new RPClassifiedAdException(string.Format("User with ID {0} could not be found.", UserId))
                    {
                        UserFriendlyError = "You don't exist.",
                        ExType = RPClassifiedAdException.ExceptionType.NotAUser
                    };

                if (new GetUserRPClassifiedAdsCount() { UserId = UserId }.Execute(context) >= 3)
                    throw new RPClassifiedAdException(string.Format("User with ID {0} can not create any more ads.", UserId))
                    {
                        UserFriendlyError = "You already have the maximum number of RP Classified Ads posted per player.",
                        UserFriendlySubError = "Wait a while for old postings to get automatically deleted or delete some of your own yourself.",
                        ExType = RPClassifiedAdException.ExceptionType.AdLimit
                    };

                var ad = RPClassifiedAd.Create(user, this);

                ctx.Add(ad);
                ctx.Commit();

                result = ad.Id;
            };

            ExecuteInternal(context);

            return result;
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
