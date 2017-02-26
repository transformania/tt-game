using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Skills.Queries;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Domain.Queries.Skills
{
    [TestFixture]
    public class GetSkillsOwnedByPlayerTests : TestBase
    {
        [Test]
        public void get_all_player_skills()
        {
            var owner = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();

            new SkillBuilder()
                .With(s => s.Id, 1)
                .With(s => s.Owner, owner)
                .With(s => s.SkillSource, new SkillSourceBuilder().With(ss => ss.Id, 5).BuildAndSave())
                .BuildAndSave();

            new SkillBuilder()
                .With(s => s.Id, 2)
                .With(s => s.Owner, owner)
                .With(s => s.SkillSource, new SkillSourceBuilder().With(ss => ss.Id, 6).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetSkillsOwnedByPlayer { playerId = 50 };

            var skills = DomainRegistry.Repository.Find(cmd);
            var skillIds = skills.Select(s => s.Id);

            skillIds.Should().Contain(1);
            skillIds.Should().Contain(2);
        }
    }
}
