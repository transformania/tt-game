using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Skills;
using TT.Domain.Entities.Skills;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Domain.Commands.Skills
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
            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<Skill>().Count(p =>
                p.Owner.Id == 100 &&
                p.SkillSource.Id == 55)
            .Should().Be(1);
        }

        [Test]
        public void should_throw_error_if_skill_source_not_found()
        {
            var cmd = new CreateSkill { ownerId = 100, skillSourceId = 55 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Skill Source with Id 55 could not be found");
        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            new SkillSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            var cmd = new CreateSkill { ownerId = 100, skillSourceId = 55 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with Id 100 could not be found");
        }



    }
}
