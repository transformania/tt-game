using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using System.Linq;

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

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var ad = ctx.AsQueryable<RPClassifiedAd>().SingleOrDefault(a => a.Id == RPClassifiedAdId);

                if (ad == null)
                    throw new DomainException(string.Format("RPClassifiedAdId with ID {0} could not be found", RPClassifiedAdId));

                if (ad.User.Id == UserId)
                    throw new DomainException(string.Format("User {0} does not own RPClassifiedAdId {1}", UserId, RPClassifiedAdId));

                ad.Update(this);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RPClassifiedAdId <= 0)
                throw new DomainException("RPClassifiedAd Id must be greater than 0");

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
