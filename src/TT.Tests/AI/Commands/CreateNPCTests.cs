using NUnit.Framework;
using TT.Domain.AI.Commands;

namespace TT.Tests.AI.Commands
{
    [TestFixture]
    public class CreateNPCTests : TestBase
    {
        [Test]
        public void Should_create_new_npc()
        {

            var cmd = new CreateNPC { SpawnText = "spawning!" };

            Assert.That(Repository.Execute(cmd), Is.GreaterThan(0));
        }

    }
}
