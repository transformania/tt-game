using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetUserDonatorsTests : TestBase
    {
        [Test]
        public void should_get_users_with_donations()
        {
            var donator = new DonatorBuilder()
                .With(d => d.Id, 123)
                .With(d => d.PatreonName, "Jimmybob")
                .With(d => d.Tier, 3)
                .BuildAndSave();

            var donatorTierZero = new DonatorBuilder()
                .With(d => d.Id, 321)
                .With(d => d.PatreonName, "sourface")
                .With(d => d.Tier, 0)
                .BuildAndSave();

            // has donator; should be listed
            new UserBuilder()
                .With(u => u.Donator, donator)
                .BuildAndSave();

            // has donator but of insufficient tier; should not be listed
            new UserBuilder()
                .With(u => u.Donator, donatorTierZero)
                .BuildAndSave();

            // no donator; should not be listed
            new UserBuilder()
                .With(u => u.Donator, null)
                .BuildAndSave();

            var users = DomainRegistry.Repository.Find(new GetUserDonators {MinimumTier = 1}).ToArray(); ;

            Assert.That(users, Has.Exactly(1).Items);
            Assert.That(users[0].Donator.Id, Is.EqualTo(123));
            Assert.That(users[0].Donator.PatreonName, Is.EqualTo("Jimmybob"));
        }
    }
}
