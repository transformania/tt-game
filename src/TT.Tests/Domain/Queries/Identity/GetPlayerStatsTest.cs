
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Identity;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Queries.Identity
{
    [TestFixture]
    public class GetPlayerStatsTest : TestBase
    {

        [Test]
        public void should_get_player_stats()
        {

           var owner = new UserBuilder()
                .With(u => u.Id, "abcde")
                .BuildAndSave();

            new StatBuilder()
                .With(i => i.Id, 245)
                .With(i => i.Owner, owner)
                .With(i => i.AchievementType, "surfing")
                .BuildAndSave();

            var stats = DomainRegistry.Repository.Find(new GetPlayerStats { OwnerId = owner.Id });

            stats.Count().Should().Be(1);
            stats.First().Id.Should().Be(245);
            stats.First().AchievementType.Should().Be("surfing");
        }
    }
}
