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
    public class CreateDonatorTests : TestBase
    {
        [Test]
        public void can_create_donator_entry()
        {

            var user = new UserBuilder()
                .BuildAndSave();

            var cmd = new CreateDonator
            {
                UserId = user.Id,
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3,
                SpecialNotes = "good boy!"
            };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Donator>().Where(d =>
                d.PatreonName == "Bob" &&
                d.ActualDonationAmount == 9 &&
                d.Tier == 3 &&
                d.SpecialNotes == "good boy!"), Has.Exactly(1).Items);

        }

        [Test]
        public void should_require_user()
        {
            var cmd = new CreateDonator
            {
                UserId = null,
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3
            };
          
            Assert.That(() =>  Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("userId is required"));
        }

        [Test]
        public void should_throw_error_if_user_not_found()
        {
            var cmd = new CreateDonator
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
