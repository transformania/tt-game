using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Tests.Builders.AI;
using TT.Domain.Commands.AI;
using TT.Domain.Entities.NPCs;

namespace TT.Tests.Domain.Commands.AI
{
    [TestFixture]
    public class UpdateNPCTests : TestBase
    {
        [Test]
        public void Should_update_existing_NPC()
        {
            new NPCBuilder().With(n => n.Id, 3)
                .With(n => n.SpawnText, "spawn 1")
                .BuildAndSave();

            var cmdEdit = new UpdateNPC { NPCId = 3, SpawnText = "updated spawn"};

            Repository.Execute(cmdEdit);

            var editedNPC = DataContext.AsQueryable<NPC>().FirstOrDefault(n => n.Id == 3);

            editedNPC.Id.Should().Be(3);
            editedNPC.SpawnText.Should().Be("updated spawn");
        }

    }
}
