using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class PlayerIsAtBusStopTests : TestBase
    {
        [Test]
        [TestCase(LocationsStatics.STREET_270_WEST_9TH_AVE)]
        [TestCase(LocationsStatics.STREET_160_SUNNYGLADE_DRIVE)]
        public void Should_return_true_if_player_at_bus_stop(string location)
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.Location, location)
                .BuildAndSave();

            var cmd = new PlayerIsAtBusStop { playerLocation = player.Location};
            Assert.That(DomainRegistry.Repository.FindSingle(cmd), Is.True);
        }

        [Test]
        public void Should_return_true_if_player_not_at_bus_stop()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.Location, "Fakelocationn")
                .BuildAndSave();

            var cmd = new PlayerIsAtBusStop { playerLocation = player.Location };
            Assert.That(DomainRegistry.Repository.FindSingle(cmd), Is.False);
        }
    }
}
