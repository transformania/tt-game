using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class UpdateDonatorTests : TestBase
    {
        [Test]
        public void can_update_donator_entry()
        {
            var donator = new DonatorBuilder()
                .With(d => d.Id, 123)
                .With(d => d.PatreonName, "Jimmybob")
                .With(d => d.Tier, 1)
                .With(d => d.ActualDonationAmount, 1)
                .BuildAndSave();

            var user = new UserBuilder()
                .With(d => d.Donator, donator)
                .BuildAndSave();

            var cmd = new UpdateDonator()
            {
                UserId = user.Id,
                ActualDonationAmount = 9,
                PatreonName = "Jimmybob the second",
                Tier = 3,
                SpecialNotes = "good boy!"
            };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Donator>().Where(d =>
                d.PatreonName == "Jimmybob the second" &&
                d.ActualDonationAmount == 9 &&
                d.Tier == 3 &&
                d.SpecialNotes == "good boy!"), Has.Exactly(1).Items);
        }

        [Test]
        public void should_require_user()
        {
            var cmd = new UpdateDonator
            {
                UserId = null,
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("userId is required"));
        }

        [Test]
        public void should_throw_error_if_user_not_found()
        {
            var cmd = new UpdateDonator
            {
                UserId = "fakeuser",
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User 'fakeuser' could not be found"));
        }
    }
}
