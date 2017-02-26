using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.AI.Queries;
using TT.Tests.Builders.AI;

namespace TT.Tests.Domain.Queries.AI
{
    [TestFixture]
    public class GetNPCTests : TestBase
    {
        [Test]
        public void Should_fetch_npcs_by_id()
        {
            new NPCBuilder().With(cr => cr.Id, 5)
                .With(cr => cr.SpawnText, "spawn text uno")
                .BuildAndSave();

            var cmd = new GetNPC { NPCId = 5 };

            var npc = DomainRegistry.Repository.FindSingle(cmd);

            npc.Id.Should().Be(5);
            npc.SpawnText.Should().BeEquivalentTo("spawn text uno");
        }
    }
}