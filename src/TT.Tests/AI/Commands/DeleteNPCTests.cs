using NUnit.Framework;
using TT.Domain.AI.Commands;
using TT.Domain.AI.Entities;
using TT.Domain.Exceptions;
using TT.Tests.Builders.AI;

namespace TT.Tests.AI.Commands
{
    [TestFixture]
    public class DeleteNPCTests : TestBase
    {
        [Test]
        public void Should_delete_NPC()
        {
            new NPCBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.SpawnText, "goose")
                .BuildAndSave();

            var cmd = new DeleteNPC { NPCId = 7 };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<NPC>(), Is.Empty);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_NPC_id_is_invalid(int id)
        {
            var cmd = new DeleteNPC { NPCId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("NPC Id must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_NPC_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteNPC { NPCId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"NPC with ID {id} was not found"));
        }
    }
}
