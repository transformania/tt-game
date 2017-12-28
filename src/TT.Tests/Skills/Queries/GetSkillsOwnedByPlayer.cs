using System.Linq;
using FluentAssertions;
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

            var skills = DomainRegistry.Repository.Find(cmd);
            var skillIds = skills.Select(s => s.Id);

            skillIds.Should().Contain(1);
            skillIds.Should().Contain(2);

            var animateSkill = skills.First(s => s.Id == 1);
            animateSkill.SkillSource.FriendlyName.Should().Be("Mad Cowmaker");
            animateSkill.SkillSource.FormSource.PortraitUrl.Should().Be("madCow.png");
            animateSkill.SkillSource.FormSource.FriendlyName.Should().Be("Mad Cow");

            var inanimateSkill = skills.First(s => s.Id == 2);
            inanimateSkill.SkillSource.FriendlyName.Should().Be("Statuemaker");
            inanimateSkill.SkillSource.FormSource.PortraitUrl.Should().Be(null);
            inanimateSkill.SkillSource.FormSource.FriendlyName.Should().Be("Statue");
            inanimateSkill.SkillSource.FormSource.ItemSource.FriendlyName.Should().Be("Statue");
            inanimateSkill.SkillSource.FormSource.ItemSource.PortraitUrl.Should().Be("statueItem.png");
        }
    }
}
