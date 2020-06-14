using NUnit.Framework;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.ClassifiedAds.Commands
{
    [Category("RPClassifiedAd Tests")]
    public class DeleteRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private RPClassifiedAd Ad;
        private DeleteRPClassifiedAd cmd;

        public override void SetUp()
        {
            base.SetUp();

            JohnSmith = new UserBuilder()
                .With(u => u.Email, "JohnSmith@example.com")
                .With(u => u.UserName, "JohnSmith")
                .With(u => u.Id, "guid")
                .BuildAndSave();

            Ad = new RPClassifiedAdBuilder()
                .With(ad => ad.Title, "This Is a Title")
                .With(ad => ad.Text, "This is some text. This is some text. This is some text.")
                .With(ad => ad.YesThemes, "Everything")
                .With(ad => ad.NoThemes, "rp with pictures")
                .With(ad => ad.PreferredTimezones, "Anytime")
                .With(ad => ad.User, JohnSmith)
                .With(ad => ad.OwnerMembershipId, JohnSmith.Id)
                .BuildAndSave();

            cmd = new DeleteRPClassifiedAd() { UserId = JohnSmith.Id, RPClassifiedAdId = Ad.Id };
        }

        [Test]
        public void Should_delete_ad()
        {
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<RPClassifiedAd>(), Is.Empty);
        }

        [Test]
        public void Should_delete_if_CheckUserId_is_false()
        {
            cmd.UserId = null;
            cmd.CheckUserId = false;

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<RPClassifiedAd>(), Is.Empty);
        }

        [Test]
        public void Should_throw_if_not_owner()
        {
            cmd.UserId += "-";

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotOwnerException>().With.Message
                    .EqualTo($"User {cmd.UserId} does not own RP Classified Ad {cmd.RPClassifiedAdId}").And
                    .Property("UserFriendlyError").EqualTo("You do not own this RP Classified Ad."));
        }

        [Test]
        public void Should_throw_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotFoundException>().With.Message
                    .EqualTo($"RPClassifiedAd with ID {cmd.RPClassifiedAdId} was not found").And
                    .Property("UserFriendlyError").EqualTo("This RP Classified Ad doesn't exist."));
        }
    }
}
