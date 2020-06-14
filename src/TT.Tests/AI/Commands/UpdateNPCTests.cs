using NUnit.Framework;
using System.Linq;
using TT.Domain.AI.Commands;
using TT.Domain.AI.Entities;
using TT.Tests.Builders.AI;

namespace TT.Tests.AI.Commands
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

            Assert.That(() => Repository.Execute(cmdEdit), Throws.Nothing);

            var editedNPC = DataContext.AsQueryable<NPC>().FirstOrDefault(n => n.Id == 3);

            Assert.That(editedNPC.Id, Is.EqualTo(3));
            Assert.That(editedNPC.SpawnText, Is.EqualTo("updated spawn"));
        }

    }
}
