using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.AI.Queries;
using TT.Tests.Builders.AI;

namespace TT.Tests.AI.Queries
{
    [TestFixture]
    public class GetNPCsTests : TestBase
    {
        [Test]
        public void Should_fetch_all_available_npcs()
        {
            new NPCBuilder().With(cr => cr.Id, 5)
                .With(cr => cr.SpawnText, "spawn text uno")
                .BuildAndSave();

            new NPCBuilder().With(cr => cr.Id, 6)
                 .With(cr => cr.SpawnText, "spawn text dos")
                 .BuildAndSave();

            var cmd = new GetNPCs();

            var npcs = DomainRegistry.Repository.Find(cmd);

            npcs.Should().HaveCount(2);
        }

        [Test]
        public void Should_return_empty_list_if_no_npcs_found()
        {
            var cmd = new GetNPCs();

            var npcs = DomainRegistry.Repository.Find(cmd);

            npcs.Should().BeEmpty();
        }

    }
}