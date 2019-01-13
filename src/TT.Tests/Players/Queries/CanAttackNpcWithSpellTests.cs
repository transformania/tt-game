using System;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Models;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class CanAttackNpcWithSpellTests : TestBase
    {
        private Player targetDonna = new Player();
        private Player targeBimboBoss = new Player();
        private Player maleRatThief = new Player();
        private Player femaleRatThief = new Player();
        private Player roadQueen = new Player();
        private Player narcissa = new Player();
        private Player bimboMouse = new Player();
        private Player nerdMouse = new Player();
        private Player miniboss = new Player();
        private Player dungeonDemon = new Player();

        private DbStaticForm futureFormInanimate = new DbStaticForm();
        private DbStaticForm futureFormPet = new DbStaticForm();
        private DbStaticForm futureFormAnimate = new DbStaticForm();

        [SetUp]
        public void Init()
        {
            targetDonna.BotId = AIStatics.DonnaBotId;
            targetDonna.FirstName = "Donna";
            targetDonna.LastName = "Milton";

            targeBimboBoss.BotId = AIStatics.BimboBossBotId;
            targeBimboBoss.FirstName = "Lady";
            targeBimboBoss.LastName = "Lovebringer";

            maleRatThief.BotId = AIStatics.MaleRatBotId;
            maleRatThief.FirstName = "male";
            maleRatThief.LastName = "thief";

            femaleRatThief.BotId = AIStatics.FemaleRatBotId;
            femaleRatThief.FirstName = "female";
            femaleRatThief.LastName = "thief";

            roadQueen.BotId = AIStatics.MotorcycleGangLeaderBotId;
            roadQueen.FirstName = "road";
            roadQueen.LastName = "queen";

            narcissa.BotId = AIStatics.FaebossBotId;
            narcissa.FirstName = "Narcissa";
            narcissa.LastName = "Fae";

            bimboMouse.BotId = AIStatics.MouseBimboBotId;
            bimboMouse.FirstName = "Adrianna";
            bimboMouse.LastName = "mouse";
            bimboMouse.FormSourceId = BossProcedures_Sisters.BimboBossFormSourceId;

            nerdMouse.BotId = AIStatics.MouseNerdBotId;
            nerdMouse.FirstName = "Candice";
            nerdMouse.LastName = "mouse";
            nerdMouse.FormSourceId = BossProcedures_Sisters.NerdBossFormSourceId;

            dungeonDemon.BotId = AIStatics.DemonBotId;

            futureFormInanimate.MobilityType = PvPStatics.MobilityInanimate;
            futureFormPet.MobilityType = PvPStatics.MobilityPet;
            futureFormAnimate.MobilityType = PvPStatics.MobilityFull;
        }

        #region donna
        [Test]
        public void should_return_emptystring_for_inanimate_spell_on_donna()
        {
            var cmd = new CanAttackNpcWithSpell { target = targetDonna, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_emptystring_for_pet_spell_on_donna()
        {
            var cmd = new CanAttackNpcWithSpell { target = targetDonna, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_no_cast_message_for_animate_spell_on_donna()
        {
            var cmd = new CanAttackNpcWithSpell { target = targetDonna, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against Donna Milton.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_weaken_spell_on_donna()
        {
            var cmd = new CanAttackNpcWithSpell { target = targetDonna, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against Donna Milton.  Maybe a different one would do...");
        }
        #endregion

        #region bimbo boss
        [Test]
        public void should_return_emptystring_for_inanimate_spell_on_bimbo_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = targeBimboBoss, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_emptystring_for_pet_spell_on_bimbo_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = targeBimboBoss, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_no_cast_message_for_animate_spell_on_bimbo_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = targeBimboBoss, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against Lady Lovebringer.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_weaken_spell_on_bimbo_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = targeBimboBoss, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against Lady Lovebringer.  Maybe a different one would do...");
        }
        #endregion

        #region ratthieves
        [Test]
        public void should_return_emptystring_for_inanimate_spell_on_male_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = maleRatThief, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_no_cast_message_for_pet_spell_on_male_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = maleRatThief, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against male thief.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_animate_spell_on_male_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = maleRatThief, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against male thief.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_weaken_spell_on_male_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = maleRatThief, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against male thief.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_emptystring_for_inanimate_spell_on_female_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = femaleRatThief, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_no_cast_message_for_pet_spell_on_female_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = femaleRatThief, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against female thief.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_animate_spell_on_female_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = femaleRatThief, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against female thief.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_weaken_spell_on_female_rat_thief_boss()
        {
            var cmd = new CanAttackNpcWithSpell { target = femaleRatThief, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against female thief.  Maybe a different one would do...");
        }

        #endregion

        #region road queen
        [Test]
        public void should_return_emptystring_for_inanimate_spell_on_road_queen()
        {
            var cmd = new CanAttackNpcWithSpell { target = roadQueen, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_emptystring_for_pet_spell_on_road_queen()
        {
            var cmd = new CanAttackNpcWithSpell { target = roadQueen, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_no_cast_message_for_animate_spell_on_road_queen()
        {
            var cmd = new CanAttackNpcWithSpell { target = roadQueen, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against road queen.  Maybe a different one would do...");
        }

        [Test]
        public void should_return_no_cast_message_for_weaken_spell_on_road_queen()
        {
            var cmd = new CanAttackNpcWithSpell { target = roadQueen, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You get the feeling this type of spell won't work against road queen.  Maybe a different one would do...");
        }
        #endregion

        #region narcissa
        [Test]
        public void should_return_emptystring_for_correct_spell_on_narcissa_with_valid_form()
        {
            var player = new Player();
            var cmd = new CanAttackNpcWithSpell { target = narcissa, spellSourceId = BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId, attacker = player};
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        [TestCase(BossProcedures_FaeBoss.GreatFaeFormSourceId)]
        [TestCase(BossProcedures_FaeBoss.DarkFaeFormSourceId)]
        [TestCase(BossProcedures_FaeBoss.EnchantedTreeFormSourceId)]
        public void should_return_emptystring_for_correct_spell_on_narcissa_with_invalid_form(int formSourceId)
        {
            var player = new Player();
            player.FormSourceId = formSourceId;
            var cmd = new CanAttackNpcWithSpell { target = narcissa, spellSourceId = BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId, attacker = player };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You try to cast upon Narcissa, but the fae's mastery over your current form is overwhelming and you find that you cannot!");
        }

        [Test]
        public void should_return_correct_message_for_correct_spell_on_narcissa_with_weaken()
        {
            var player = new Player();
            var cmd = new CanAttackNpcWithSpell { target = narcissa, spellSourceId = PvPStatics.Spell_WeakenId, attacker = player };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("This spell has no effect on Narcissa!  Maybe you should talk to Rusty at the bar and get some advice...");
        }

        #endregion

        #region mouse sisters
        [Test]
        public void should_return_emptystring_for_correct_spell_and_form_on_bimbo_mouse()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.NerdSpellFormSourceId;
            var cmd = new CanAttackNpcWithSpell { target = bimboMouse, attacker = player, spellSourceId = BossProcedures_Sisters.NerdSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_emptystring_for_correct_spell_and_form_on_nerd_mouse()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.BimboSpellFormSourceId;
            var cmd = new CanAttackNpcWithSpell { target = nerdMouse, attacker = player, spellSourceId = BossProcedures_Sisters.BimboSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_message_for_corect_spell_and_incorrect_form_on_bimbo_mouse()
        {
            var player = new Player();
            var cmd = new CanAttackNpcWithSpell { target = bimboMouse, attacker = player, spellSourceId = BossProcedures_Sisters.NerdSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You can't seem to find the right peeved-off mindset to cast this spell against Candice.  Maybe you'd have better luck if you were casting magic against her as a Nerdy Mousegirl...");
        }

        [Test]
        public void should_return_message_for_corect_spell_and_incorrect_form_on_nerd_mouse()
        {
            var player = new Player();
            var cmd = new CanAttackNpcWithSpell { target = nerdMouse, attacker = player, spellSourceId = BossProcedures_Sisters.BimboSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("You can't seem to find the right peeved-off mindset to cast this spell against Adrianna.  Maybe you'd have better luck if you were casting magic against her as a Bimbo Mousegirl...");
        }

        [Test]
        public void should_return_message_for_incorrect_spell_and_correct_form_on_bimbo_mouse()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.NerdSpellFormSourceId;
            var cmd = new CanAttackNpcWithSpell { target = bimboMouse, attacker = player, spellSourceId = PvPStatics.Spell_WeakenId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("This spell won't work against Candice.");
        }

        [Test]
        public void should_return_message_for_incorrect_spell_and_correct_form_on_nerd_mouse()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.BimboSpellFormSourceId;
            var cmd = new CanAttackNpcWithSpell { target = nerdMouse, attacker = player, spellSourceId = PvPStatics.Spell_WeakenId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("This spell won't work against Adrianna.");
        }

        [Test]
        public void should_return_message_when_bimbo_mouse_transformed()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.NerdSpellFormSourceId;
            bimboMouse.FormSourceId = -123;
            var cmd = new CanAttackNpcWithSpell { target = bimboMouse, attacker = player, spellSourceId = BossProcedures_Sisters.NerdSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("One of the Brisby sisters have already been transformed; there's no need to attack them any further.");
        }

        [Test]
        public void should_return_message_when_nerd_mouse_transformed()
        {
            var player = new Player();
            player.FormSourceId = BossProcedures_Sisters.BimboSpellFormSourceId;
            nerdMouse.FormSourceId = -123;
            var cmd = new CanAttackNpcWithSpell { target = nerdMouse, attacker = player, spellSourceId = BossProcedures_Sisters.BimboSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("One of the Brisby sisters have already been transformed; there's no need to attack them any further.");
        }

        #endregion

        #region minibosses

        [Test]
        [TestCase(AIStatics.MinibossSororityMotherId)]
        [TestCase(AIStatics.MinibossPopGoddessId)]
        public void should_return_emptystring_for_inanimate_spell(int botId)
        {
            miniboss.BotId = botId;
            var cmd = new CanAttackNpcWithSpell { target = miniboss, futureForm = futureFormInanimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        [TestCase(AIStatics.MinibossSororityMotherId)]
        [TestCase(AIStatics.MinibossPopGoddessId)]
        public void should_return_emptystring_for_pet_spell(int botId)
        {
            miniboss.BotId = botId;
            var cmd = new CanAttackNpcWithSpell { target = miniboss, futureForm = futureFormPet };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        [TestCase(AIStatics.MinibossSororityMotherId)]
        [TestCase(AIStatics.MinibossPopGoddessId)]
        public void should_return_message_for_animate_spell(int botId)
        {
            miniboss.BotId = botId;
            var cmd = new CanAttackNpcWithSpell { target = miniboss, futureForm = futureFormAnimate };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("Your target seems immune from this kind of spell.  Maybe a different one would do...");
        }

        [Test]
        [TestCase(AIStatics.MinibossSororityMotherId)]
        [TestCase(AIStatics.MinibossPopGoddessId)]
        public void should_return_message_for_weaken(int botId)
        {
            miniboss.BotId = botId;
            var cmd = new CanAttackNpcWithSpell { target = miniboss, futureForm = null };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("Your target seems immune from this kind of spell.  Maybe a different one would do...");
        }

        #endregion

        #region dungeon demon

        [Test]
        public void should_return_emptystring_for_vanqish_spell()
        {
            var cmd = new CanAttackNpcWithSpell { target = dungeonDemon, spellSourceId = PvPStatics.Dungeon_VanquishSpellSourceId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_emptystring_for_weaken_spell()
        {
            var cmd = new CanAttackNpcWithSpell { target = dungeonDemon, spellSourceId = PvPStatics.Spell_WeakenId };
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be(String.Empty);
        }

        [Test]
        public void should_return_message_for_animate_spell()
        {
            var cmd = new CanAttackNpcWithSpell { target = dungeonDemon, spellSourceId = -123};
            var result = DomainRegistry.Repository.FindSingle(cmd);
            result.Should().Be("Only the 'Vanquish' spell and Weaken have any effect on the Dark Demonic Guardians.");
        }

        #endregion
    }
}
