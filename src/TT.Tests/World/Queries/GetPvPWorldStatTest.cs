using NUnit.Framework;
using TT.Domain;
using TT.Domain.World.Queries;
using TT.Tests.Builders.Game;

namespace TT.Tests.World.Queries
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
            Assert.That(stat.Id, Is.EqualTo(77));
            Assert.That(stat.ChaosMode, Is.False);
        }
    }
}
