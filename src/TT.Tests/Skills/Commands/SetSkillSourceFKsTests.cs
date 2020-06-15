using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Skills.Commands;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Skills.Commands
{
    [TestFixture]
    public class SetSkillSourceFKsTests : TestBase
    {

        private SkillSource skill;
        private FormSource form;
        private FormSource exclusiveToForm;
        private ItemSource exlusiveToItem;
        private EffectSource givesEffect;

        [SetUp]
        public void Init()
        {

            skill = new SkillSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            form = new FormSourceBuilder()
                .With(f => f.Id, 3)
                .BuildAndSave();

            exclusiveToForm = new FormSourceBuilder()
                .With(f => f.Id, 7)
                .BuildAndSave();

            exlusiveToItem = new ItemSourceBuilder()
                .With(i => i.Id, 100)
                .BuildAndSave();

            givesEffect = new EffectSourceBuilder()
                .With(i => i.Id, 78)
                .BuildAndSave();

        }

        [Test]
        public void should_save_skill_with_new_fks()
        {
            DomainRegistry.Repository.Execute(new SetSkillSourceFKs
            {
                SkillSourceId = 55,
                FormSourceId = form.Id, 
                ExclusiveToFormSourceId = exclusiveToForm.Id,
                ExclusiveToItemSourceId = exlusiveToItem.Id,
                GivesEffectSourceId = givesEffect.Id
            });

            var skillSource = DataContext.AsQueryable<SkillSource>().First(f => f.Id == 55);

            Assert.That(skillSource.FormSource.Id, Is.EqualTo(3));

            Assert.That(skillSource.ExclusiveToFormSource.Id, Is.EqualTo(7));

            Assert.That(skillSource.ExclusiveToItemSource.Id, Is.EqualTo(100));

            Assert.That(skillSource.GivesEffectSource.Id, Is.EqualTo(78));
        }

        [Test]
        public void should_save_skill_with_new_fks_all_nulls()
        {
            DomainRegistry.Repository.Execute(new SetSkillSourceFKs
            {
                SkillSourceId = 55,
                FormSourceId = null,
                ExclusiveToFormSourceId = null,
                ExclusiveToItemSourceId = null,
                GivesEffectSourceId = null
            });

            var skillSource = DataContext.AsQueryable<SkillSource>().First(f => f.Id == 55);

            Assert.That(skillSource.FormSource, Is.Null);
            Assert.That(skillSource.ExclusiveToFormSource, Is.Null);
            Assert.That(skillSource.GivesEffect, Is.Null);
            Assert.That(skillSource.ExclusiveToItemSource, Is.Null);
        }

        [Test]
        public void should_throw_error_if_form_source_submitted_but_not_found()
        {
            var cmd = new SetSkillSourceFKs { SkillSourceId = 55, FormSourceId = 99999 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "FormSource Source with id '99999' could not be found.  Does it need to be published first?"));
        }

        [Test]
        public void should_throw_error_if_exclusive_to_form_source_submitted_but_not_found()
        {
            var cmd = new SetSkillSourceFKs { SkillSourceId = 55, ExclusiveToFormSourceId = 99999};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("ExclusiveToFormSourceId with id '99999' could not be found"));
        }

        [Test]
        public void should_throw_error_if_gives_effect_source_submitted_but_not_found()
        {

            var cmd = new SetSkillSourceFKs { SkillSourceId = 55, GivesEffectSourceId = -999};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("EffectSource with Id '-999' could not be found"));
        }

        [Test]
        public void should_throw_error_if_exclusive_to_item_source_submitted_but_not_found()
        {

            var cmd = new SetSkillSourceFKs { SkillSourceId = 55, ExclusiveToItemSourceId = -999 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("ExclusiveToItemSource with id '-999' could not be found"));
        }

        [Test]
        public void should_throw_error_if_skill_source_not_found()
        {
            var cmd = new SetSkillSourceFKs { SkillSourceId = 3457};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("StaticSkill Source with Id 3457 could not be found"));
        }

    }
}
