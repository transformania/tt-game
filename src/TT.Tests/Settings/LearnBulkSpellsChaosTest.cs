using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Skills.Commands;
using TT.Domain.Skills.DTOs;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Skills.Commands
{
    [TestFixture]
    public class LearnBulkSpellsChaosTest : TestBase
    {

        private Player playerBaseStart;
        private SkillSource skillSource1;
        private SkillSource skillSource2;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            playerBaseStart = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();
            
            skillSource1 = new SkillSourceBuilder()
                .With(ss => ss.Id, 100)
                .BuildAndSave();

            skillSource2 = new SkillSourceBuilder()
                .With(ss => ss.Id, 200)
                .BuildAndSave();
            
        }

        public IEnumerable<LearnableSkillsDetail> CreateTestSkillList() {
            var newSkill1 = new LearnableSkillsDetail { Id = skillSource1.Id };
            var newSkill2 = new LearnableSkillsDetail { Id = skillSource2.Id };

            var list = new List<LearnableSkillsDetail>();
            list.Add(newSkill1);
            list.Add(newSkill2);

            return list;
        }

        [Test]
        public void should_learn_multiple_spells()
        {
            var newSkillList = CreateTestSkillList();
            var cmd = new CreateAllSkills { ownerId = playerBaseStart.Id, skillList = newSkillList };

            //Assert the function had no errors and the player's skill list matches
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);
            Assert.That(DataContext.AsQueryable<Skill>().Where(p =>
                p.Owner.Id == playerBaseStart.Id), Has.Exactly(newSkillList.Count()).Items);
        }

        [Test]
        public void should_throw_error_if_skill_source_not_found()
        {
            var newSkill = new LearnableSkillsDetail { Id = 300 };
            List<LearnableSkillsDetail> errSkillList = new List<LearnableSkillsDetail>();
            errSkillList.Add(newSkill);

            //Should throw error cause skill with ID 300 should not exist
            var cmd = new CreateAllSkills { ownerId = playerBaseStart.Id, skillList = errSkillList };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("StaticSkill Source with Id 300 could not be found"));
        }
    }
}
