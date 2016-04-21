using NUnit.Framework;
using FluentAssertions;
using TT.Domain.Commands.AI;

namespace TT.Tests.Domain.Commands.AI
{
    [TestFixture]
    public class CreateNPCTests : TestBase
    {
        [Test]
        public void Should_create_new_npc()
        {

            var cmd = new CreateNPC { SpawnText = "spawning!" };

            var npc = Repository.Execute(cmd);

            npc.Should().BeGreaterThan(0);
        }

    }
}
