using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class PlayerBecomesItemTests : TestBase
    {
        private Player victim;
        private Player attacker;
        private FormSource formSource;
        private ItemSource itemSource;

        private BuffBox attackerBuffBox;

        private List<Item> victimItems;
        private List<Item> emptyItemList;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            attackerBuffBox = new BuffBox();

            victimItems = new List<Item>();
            emptyItemList = new List<Item>();

            var victimItem = new ItemBuilder()
                .With(i => i.Id, 82624)
                .With(i => i.dbLocationName, String.Empty)
                .With(i => i.FormerPlayer, null)
                .With(i => i.Owner, null)
                .BuildAndSave();

            victimItems.Add(victimItem);

            victim = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.FirstName, "Victim")
                .With(p => p.LastName, "MgGee")
                .With(p => p.Level, 13)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Level, 13)
                .With(p => p.Location, "someplace")
                .With(p => p.Items, victimItems)
                .BuildAndSave();

            attacker = new PlayerBuilder()
                .With(p => p.Id, 2)
                .With(p => p.FirstName, "Attacker")
                .With(p => p.LastName, "Smacker")
                .With(p => p.Items, emptyItemList)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.Protection)
                .BuildAndSave();

            itemSource = new ItemSourceBuilder()
                .With(i => i.Id, 100)
                .With(i => i.FriendlyName, "A new Item!")
                .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                .BuildAndSave();

            formSource = new FormSourceBuilder()
                .With(i => i.Id, 87)
                .With(i => i.MobilityType, PvPStatics.MobilityInanimate)
                .With(i => i.ItemSource, itemSource)
                .BuildAndSave();
        }

        [Test]
        public void player_should_become_item_and_go_to_attacker()
        {

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox};

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a A new Item!!</b>!");
            logs.LocationLog.Should().Be("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here.");

            var victimPostTF = DataContext.AsQueryable<Player>().First(p => p.Id == victim.Id);
            victimPostTF.Mobility.Should().Be(PvPStatics.MobilityInanimate);
            victimPostTF.FormSource.Id.Should().Be(formSource.Id);
            victimPostTF.Item.ItemSource.Id.Should().Be(itemSource.Id);

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            newItem.Owner.Id.Should().Be(attacker.Id);
            newItem.IsPermanent.Should().Be(false);
            newItem.PvPEnabled.Should().Be((int) GameModeStatics.GameModes.Protection); // match attacker's game mode
            newItem.Level.Should().Be(victim.Level);
            newItem.dbLocationName.Should().Be(String.Empty);
            newItem.ItemSource.FriendlyName.Should().Be(itemSource.FriendlyName);

            var attackerPostTF = DataContext.AsQueryable<Player>().First(p => p.Id == attacker.Id);
            attackerPostTF.Items.Count.Should().Be(1);
            attackerPostTF.Items.ElementAt(0).FormerPlayer.Id.Should().Be(victimPostTF.Id);

            var droppedItem = DataContext.AsQueryable<Item>().First(i => i.Id == 82624);
            droppedItem.Owner.Should().Be(null);
            droppedItem.dbLocationName.Should().Be("someplace");
        }

        [Test]
        public void bots_should_lock_permanently_immediately_and_be_any_game_mode()
        {
            var botVictim = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.FirstName, "Psychopath")
                .With(p => p.LastName, "Panties")
                .With(p => p.Level, 13)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.Protection)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .With(p => p.Level, 7)
                .BuildAndSave();

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = botVictim.Id, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox };

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Psychopath Panties into a A new Item!</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a A new Item!!</b>!");
            logs.LocationLog.Should().Be("<br><b>Psychopath Panties was completely transformed into a A new Item!</b> here.");

            var victimPostTF = DataContext.AsQueryable<Player>().First(p => p.Id == botVictim.Id);
            victimPostTF.Mobility.Should().Be(PvPStatics.MobilityInanimate);
            victimPostTF.FormSource.Id.Should().Be(formSource.Id);
            victimPostTF.Item.ItemSource.Id.Should().Be(itemSource.Id);

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == botVictim.Id);
            newItem.Owner.Id.Should().Be(attacker.Id);
            newItem.PvPEnabled.Should().Be((int)GameModeStatics.GameModes.Any);
            newItem.IsPermanent.Should().Be(true);
            newItem.Level.Should().Be(botVictim.Level);
            newItem.dbLocationName.Should().Be(String.Empty);
            newItem.ItemSource.FriendlyName.Should().Be(itemSource.FriendlyName);
        }

        [Test]
        public void fall_on_ground_if_no_inventory_space_for_attacker()
        {
            attackerBuffBox.FromEffects_Fortitude = -9999;
            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox };

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a A new Item!!</b>!");
            logs.LocationLog.Should().Be("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here.");

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            newItem.Owner.Should().Be(null);
            newItem.dbLocationName.Should().Be(victim.Location);
        }

        [Test]
        public void pets_automatically_equipped_to_new_owner_when_owner_has_no_existing_pet()
        {

            var petItemSource = new ItemSourceBuilder()
                .With(i => i.Id, 1000)
                .With(i => i.FriendlyName, "Squeaky Pet")
                .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                .BuildAndSave();

            var petFormSource = new FormSourceBuilder()
                .With(i => i.Id, 870)
                .With(i => i.MobilityType, PvPStatics.MobilityPet)
                .With(i => i.ItemSource, petItemSource)
                .BuildAndSave();


            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = petFormSource.Id, AttackerBuffs = attackerBuffBox };

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Victim MgGee into a Squeaky Pet</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a Squeaky Pet!</b>!");
            logs.LocationLog.Should().Be("<br><b>Victim MgGee was completely transformed into a Squeaky Pet</b> here.");

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            newItem.Owner.Should().Be(attacker);
            newItem.IsEquipped.Should().Be(true);
        }

        [Test]
        public void pets_automatically_dropped_when_owner_has_existing_pet()
        {

            var pet = new ItemBuilder()
                .With(i => i.Id, 878)
                .With(i => i.IsEquipped, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                    .With(i => i.Id, 1525).BuildAndSave();

            var petList = new List<Item>();
            petList.Add(pet);

            var petItemSource = new ItemSourceBuilder()
                .With(i => i.Id, 1000)
                .With(i => i.FriendlyName, "Squeaky Pet")
                .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                .BuildAndSave();

            var petFormSource = new FormSourceBuilder()
                .With(i => i.Id, 870)
                .With(i => i.MobilityType, PvPStatics.MobilityPet)
                .With(i => i.ItemSource, petItemSource)
                .BuildAndSave();

            attacker = new PlayerBuilder()
                .With(p => p.Id, 296)
                .With(p => p.FirstName, "Attacker")
                .With(p => p.LastName, "Smacker")
                .With(p => p.Items, petList)
                .With(p => p.Location, "rainbowland")
                .BuildAndSave();

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = petFormSource.Id, AttackerBuffs = attackerBuffBox };

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Victim MgGee into a Squeaky Pet</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a Squeaky Pet!</b>!");
            logs.LocationLog.Should().Be("<br><b>Victim MgGee was completely transformed into a Squeaky Pet</b> here.");

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            newItem.Owner.Should().Be(null);
            newItem.IsEquipped.Should().Be(false);
            newItem.dbLocationName.Should().Be(victim.Location);
        }

        [Test]
        public void should_create_item_when_attacker_null()
        {
            var cmd = new PlayerBecomesItem { AttackerId = null, VictimId = victim.Id, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox };

            var logs = DomainRegistry.Repository.Execute(cmd);
            logs.AttackerLog.Should().Be("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!");
            logs.VictimLog.Should().Be("<br><b>You have been fully transformed into a A new Item!!</b>!");
            logs.LocationLog.Should().Be("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here.");

            var victimPostTF = DataContext.AsQueryable<Player>().First(p => p.Id == victim.Id);
            victimPostTF.Mobility.Should().Be(PvPStatics.MobilityInanimate);
            victimPostTF.FormSource.Id.Should().Be(formSource.Id);
            victimPostTF.Item.ItemSource.Id.Should().Be(itemSource.Id);

            var newItem = DataContext.AsQueryable<Item>().First(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            newItem.Owner.Should().Be(null);
            newItem.dbLocationName.Should().Be(victim.Location);
            newItem.PvPEnabled.Should().Be((int) GameModeStatics.GameModes.Any); // chaos poof turns into any game mode
        }

        [Test]
        public void Should_throw_exception_if_victim_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = -1, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>()
                .WithMessage("Player (victim) with ID '-1' could not be found");

        }

        [Test]
        public void Should_throw_exception_if_attacker_specified_but_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = 123, VictimId = victim.Id, NewFormId = formSource.Id, AttackerBuffs = attackerBuffBox };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>()
                .WithMessage("Player (attacker) with ID '123' could not be found");

        }

        [Test]
        public void Should_throw_exception_if_new_form_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = -1, AttackerBuffs = attackerBuffBox };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Form with ID '-1' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_item_source_not_found()
        {

            var formSource2 = new FormSourceBuilder()
                .With(i => i.Id, 99)
                .With(i => i.ItemSource, null)
                .BuildAndSave();

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource2.Id, AttackerBuffs = attackerBuffBox };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Form is not inanimate or pet");
        }
    }
}
