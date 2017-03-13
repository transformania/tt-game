using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Tests.Builders.AI;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Queries
{
    public class GetPlayerTest : TestBase
    {

        [Test]
        public void Should_find_player_by_id()
        {
            var user = new UserBuilder().With(u => u.Id, "guid").BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();
            var form = new FormSourceBuilder().With(n => n.Id, 101).BuildAndSave();

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.User, user)
                .With(p => p.NPC, npc)
                .With(p => p.FormSource, form)
                .BuildAndSave();

            var cmd = new GetPlayer { PlayerId = 23 };

            var foundPlayer = DomainRegistry.Repository.FindSingle(cmd);

            foundPlayer.Id.Should().Be(23);
            foundPlayer.NPC.Id.Should().Be(7);
            foundPlayer.FormSource.Id.Should().Be(101);
        }

        [Test]
        public void Should_find_player_by_bot_id()
        {
            var user = new UserBuilder().With(u => u.Id, "guid").BuildAndSave();

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.User, user)
                .With(p => p.BotId, 5)
                .BuildAndSave();

            var cmd = new GetPlayerByBotId{ BotId = 5 };

            var foundPlayer = DomainRegistry.Repository.FindSingle(cmd);

            foundPlayer.Id.Should().Be(23);
            foundPlayer.BotId.Should().Be(5);
        }

    }
}
