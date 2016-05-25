using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using System.Linq;

namespace TT.Domain.Commands.RPClassifiedAds
{
    public class CreateRPClassifiedAd : DomainCommand<int>
    {
        public string UserId { get; set; }

        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);

                if (user == null)
                    throw new DomainException(string.Format("User with ID {0} could not be found", UserId));

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
                throw new DomainException("The ad title is too short.");

            // assert the title field is not too long
            if (Title.Length > 35)
                throw new DomainException("The ad title is too long.");

            // assert the text fields are not too short
            if (Text == null || Text.Length < 50)
                throw new DomainException("The ad description is too short.");

            // assert the text fields are not too long
            if (Text.Length > 300)
                throw new DomainException("The ad description is too long.");

            // assert the yes field is not too long
            if (YesThemes.Length > 200)
                throw new DomainException("The ad description is too long.");

            // assert the yes field is not too long
            if (YesThemes.Length > 200)
                throw new DomainException("The ad description is too long.");

            // assert the no field is not too long
            if (NoThemes.Length > 200)
                throw new DomainException("The ad description is too long.");

            // assert the timezone fields is not too long
            if (PreferredTimezones.Length > 70)
                throw new DomainException("The ad title is too long.");
        }
    }
}
