using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Skills.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Skills.Queries
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
                .With(s => s.SkillSource, new SkillSourceBuilder()
                    .With(ss => ss.Id, 5)
                    .With(ss => ss.MobilityType, PvPStatics.MobilityFull)
                    .With(ss => ss.FriendlyName, "Mad Cowmaker")
                    .With(ss => ss.FormSource, new FormSourceBuilder()
                        .With(fs => fs.FriendlyName, "Mad Cow")
                        .With(fs => fs.PortraitUrl, "madCow.png").BuildAndSave()
                    ).BuildAndSave())
                .BuildAndSave();

            new SkillBuilder()
                .With(s => s.Id, 2)
                .With(s => s.Owner, owner)
                .With(s => s.SkillSource, new SkillSourceBuilder()
                    .With(ss => ss.Id, 6)
                    .With(ss => ss.MobilityType, PvPStatics.MobilityInanimate)
                    .With(ss => ss.FriendlyName, "Statuemaker")
                    .With(ss => ss.FormSource, new FormSourceBuilder()
                        .With(fs => fs.FriendlyName, "Statue")
                        .With(fs => fs.ItemSource, new ItemSourceBuilder()
                            .With(i => i.ItemType, PvPStatics.ItemType_Accessory)
                            .With(i => i.FriendlyName, "Statue")
                            .With(i => i.PortraitUrl, "statueItem.png")
                            .BuildAndSave()
                        )
                        .BuildAndSave())
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetSkillsOwnedByPlayer { playerId = 50 };

            var skills = DomainRegistry.Repository.Find(cmd).ToList();
            var skillIds = skills.Select(s => s.Id).ToList();

            Assert.That(skillIds, Has.Member(1));
            Assert.That(skillIds, Has.Member(2));

            var animateSkill = skills.First(s => s.Id == 1);
            Assert.That(animateSkill.SkillSource.FriendlyName, Is.EqualTo("Mad Cowmaker"));
            Assert.That(animateSkill.SkillSource.FormSource.PortraitUrl, Is.EqualTo("madCow.png"));
            Assert.That(animateSkill.SkillSource.FormSource.FriendlyName, Is.EqualTo("Mad Cow"));

            var inanimateSkill = skills.First(s => s.Id == 2);
            Assert.That(inanimateSkill.SkillSource.FriendlyName, Is.EqualTo("Statuemaker"));
            Assert.That(inanimateSkill.SkillSource.FormSource.PortraitUrl, Is.Null);
            Assert.That(inanimateSkill.SkillSource.FormSource.FriendlyName, Is.EqualTo("Statue"));
            Assert.That(inanimateSkill.SkillSource.FormSource.ItemSource.FriendlyName, Is.EqualTo("Statue"));
            Assert.That(inanimateSkill.SkillSource.FormSource.ItemSource.PortraitUrl, Is.EqualTo("statueItem.png"));
        }
    }
}
