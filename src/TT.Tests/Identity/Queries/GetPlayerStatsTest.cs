using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
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

            var stats = DomainRegistry.Repository.Find(new GetPlayerStats { OwnerId = owner.Id }).ToList();

            Assert.That(stats, Has.Exactly(1).Items);
            Assert.That(stats.First().Id, Is.EqualTo(245));
            Assert.That(stats.First().AchievementType, Is.EqualTo("surfing"));
        }
    }
}
