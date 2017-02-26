using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.World.Queries;
using TT.Tests.Builders.Game;

namespace TT.Tests.Domain.Queries.Game
{
    [TestFixture]
    public class GetPvPWorldStatTest : TestBase
    {

        [Test]
        public void should_get_world_stats()
        {
            new WorldBuilder().With(i => i.Id, 77)
                .With(p => p.ChaosMode, false)
                .BuildAndSave();

            var stat = DomainRegistry.Repository.FindSingle(new GetWorld());
            stat.Id.Should().Be(77);
            stat.ChaosMode.Should().Be(false);

        }
    }
}
