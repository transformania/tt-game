using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Identity;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Queries.Identity 
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
                .BuildAndSave();

            // has donator; should be listed
            new UserBuilder()
                .With(u => u.Donator, donator)
                .BuildAndSave();

            // no donator; should not be listed
            new UserBuilder()
                .With(u => u.Donator, null)
                .BuildAndSave();

            var users = DomainRegistry.Repository.Find(new GetUserDonators()).ToArray(); ;

            users.Length.Should().Be(1);
            users[0].Donator.Id.Should().Be(123);
            users[0].Donator.PatreonName.Should().Be("Jimmybob");
        }
    }
}
