using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain;
using TT.Tests.Builders.AI;
using TT.Domain.Entities.NPCs;
using TT.Domain.Commands.AI;

namespace TT.Tests.Domain.Commands.AI
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

            Repository.Execute(cmd);

            DataContext.AsQueryable<NPC>().Count().Should().Be(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_NPC_id_is_invalid(int id)
        {
            var cmd = new DeleteNPC { NPCId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("NPC Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_NPC_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteNPC { NPCId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage($"NPC with ID {id} was not found");
        }
    }
}
