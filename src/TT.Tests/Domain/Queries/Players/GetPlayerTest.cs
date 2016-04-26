using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Players;
using TT.Tests.Builders.AI;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Players
{
    public class GetPlayerTest : TestBase
    {

        [Test]
        public void Should_find_player_by_id()
        {
            var user = new UserBuilder().With(u => u.Id, "guid").BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var player = new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.User, user)
                .With(p => p.NPC, npc)
                .BuildAndSave();

            var cmd = new GetPlayer { PlayerId = 23 };

            var foundPlayer = DomainRegistry.Repository.FindSingle(cmd);

            foundPlayer.Id.Should().Equals(3);
            foundPlayer.NPC.Id.Should().Equals(7);
           // foundPlayer.User.Id.Should().BeEquivalentTo("guid");
            //item.DbName.Should().BeEquivalentTo("dbName");
            //item.FriendlyName.Should().BeEquivalentTo("Hello!");
            //item.IsUnique.Should().Be(true);
        }

    }
}
