using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetUserDonatorTests : TestBase
    {
        [Test]
        public void should_get_user_with_donations()
        {
            var donator = new DonatorBuilder()
                .With(d => d.Id, 123)
                .With(d => d.PatreonName, "Jimmybob")
                .BuildAndSave();

            // has donator; should be listed
            new UserBuilder()
                .With(u => u.Donator, donator)
                .BuildAndSave();

            var user = DomainRegistry.Repository.FindSingle(new GetUserDonator{Id = 123});

            user.Donator.Id.Should().Be(123);
            user.Donator.PatreonName.Should().Be("Jimmybob");
        }
    }
}
