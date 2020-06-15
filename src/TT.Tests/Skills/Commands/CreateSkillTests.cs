using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Skills.Commands;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Skills.Commands
{
    [TestFixture]
    public class CreateSkillTests : TestBase
    {
        [Test]
        public void can_create_skill()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            new SkillSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            var cmd = new CreateSkill {ownerId = 100, skillSourceId = 55 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Skill>().Where(p =>
                p.Owner.Id == 100 &&
                p.SkillSource.Id == 55), Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_error_if_skill_source_not_found()
        {
            var cmd = new CreateSkill { ownerId = 100, skillSourceId = 55 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("StaticSkill Source with Id 55 could not be found"));
        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            new SkillSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            var cmd = new CreateSkill { ownerId = 100, skillSourceId = 55 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with Id 100 could not be found"));
        }
    }
}
