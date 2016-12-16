using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.Skills;
using TT.Domain.Queries.Skills;
using TT.Domain.Statics;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Domain.Queries.Skills

{

    [TestFixture]
    public class GetSkillsPurchaseableByPlayerTests : TestBase
    {
        [Test]
        public void get_all_purchaseable_skills()
        {

            var owner = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.Skills, new List<Skill>())
                .BuildAndSave();

            new SkillBuilder()
                .With(s => s.Id, 1)
                .With(s => s.Owner, owner)
                .With(s => s.SkillSource, new SkillSourceBuilder()
                    .With(ss => ss.Id, 5)
                    .With(s => s.MobilityType, PvPStatics.MobilityFull)
                    .BuildAndSave())
                .BuildAndSave();

            var notLiveSkill = new SkillSourceBuilder()
                .With(s => s.Id, 1)
                .With(s => s.IsLive, null)
                .With(s => s.MobilityType, PvPStatics.MobilityFull)
                .With(s => s.LearnedAtLocation, "somewhere")
                .BuildAndSave();

            var notRightMobilitySkill = new SkillSourceBuilder()
                .With(s => s.Id, 2)
                .With(s => s.IsLive, "live")
                .With(s => s.MobilityType, PvPStatics.MobilityInanimate)
                .With(s => s.LearnedAtLocation, "somewhere")
                .BuildAndSave();

            var noLocationOrRegionSkill = new SkillSourceBuilder()
                .With(s => s.Id, 3)
                .With(s => s.IsLive, "live")
                .With(s => s.MobilityType, PvPStatics.MobilityInanimate)
                .With(s => s.LearnedAtLocation, null)
                .With(s => s.LearnedAtRegion, null)
                .BuildAndSave();

            var learnableSkill = new SkillSourceBuilder()
                .With(s => s.Id, 4)
                .With(s => s.IsLive, "live")
                .With(s => s.MobilityType, PvPStatics.MobilityFull)
                .With(s => s.LearnedAtLocation, "somewhere")
                .With(s => s.FormSource, new FormSourceBuilder()
                    .With(f => f.Id, 100).BuildAndSave())
                .With(s => s.IsPlayerLearnable, true)
                .BuildAndSave();

            var alreadyLearnedSkill = new SkillSourceBuilder()
                .With(s => s.Id, 5)
                .With(s => s.IsLive, "live")
                .With(s => s.MobilityType, PvPStatics.MobilityFull)
                .With(s => s.LearnedAtLocation, "somewhere")
                .With(s => s.FormSource, new FormSourceBuilder().With(f => f.Id, 100).BuildAndSave())
                .With(s => s.IsPlayerLearnable, true)
                .With(s => s.Id, 5)
                .BuildAndSave();

            var cmd = new GetSkillsPurchaseableByPlayer { playerId = owner.Id, MobilityType = PvPStatics.MobilityFull };

            var skills = DomainRegistry.Repository.Find(cmd);
            var skillIds = skills.Select(s => s.Id);

            skillIds.Should().NotContain(notLiveSkill.Id);
            skillIds.Should().NotContain(notRightMobilitySkill.Id);
            skillIds.Should().NotContain(noLocationOrRegionSkill.Id);
            skillIds.Should().NotContain(alreadyLearnedSkill.Id);

            skillIds.Should().Contain(learnableSkill.Id);
        }
    }
}
