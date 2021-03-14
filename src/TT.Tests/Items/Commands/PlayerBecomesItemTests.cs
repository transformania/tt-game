using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
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

        private List<Item> victimItems;
        private List<Item> emptyItemList;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            JokeShopProcedures.PSYCHOTIC_EFFECT = 123;

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
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.Superprotection)
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

        [TearDown]
        public void TearDown()
        {
            JokeShopProcedures.PSYCHOTIC_EFFECT = null;
        }

        [Test]
        public void player_should_become_item_and_go_to_attacker()
        {

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog").EqualTo("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a A new Item!!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here."));

            var victimPostTF = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == victim.Id);
            Assert.That(victimPostTF, Is.Not.Null);
            Assert.That(victimPostTF.Mobility, Is.EqualTo(PvPStatics.MobilityInanimate));
            Assert.That(victimPostTF.FormSource.Id, Is.EqualTo(formSource.Id));
            Assert.That(victimPostTF.Item.ItemSource.Id, Is.EqualTo(itemSource.Id));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner.Id, Is.EqualTo(attacker.Id));
            Assert.That(newItem.IsPermanent, Is.False);
            Assert.That(newItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Protection)); // Superprotection players always get protection items
            Assert.That(newItem.Level, Is.EqualTo(victim.Level));
            Assert.That(newItem.dbLocationName, Is.Empty);
            Assert.That(newItem.ItemSource.FriendlyName, Is.EqualTo(itemSource.FriendlyName));
            Assert.That(newItem.ConsentsToSoulbinding, Is.False);

            var attackerPostTF = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == attacker.Id);
            Assert.That(attackerPostTF, Is.Not.Null);
            Assert.That(attackerPostTF.Items, Has.Exactly(1).Items);
            Assert.That(attackerPostTF.Items.ElementAt(0).FormerPlayer.Id, Is.EqualTo(victimPostTF.Id));

            var droppedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 82624);
            Assert.That(droppedItem, Is.Not.Null);
            Assert.That(droppedItem.Owner, Is.Null);
            Assert.That(droppedItem.dbLocationName, Is.EqualTo("someplace"));
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

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = botVictim.Id, NewFormId = formSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog")
                    .EqualTo("<br><b>You fully transformed Psychopath Panties into a A new Item!</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a A new Item!!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Psychopath Panties was completely transformed into a A new Item!</b> here."));

            var victimPostTF = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == botVictim.Id);
            Assert.That(victimPostTF, Is.Not.Null);
            Assert.That(victimPostTF.Mobility, Is.EqualTo(PvPStatics.MobilityInanimate));
            Assert.That(victimPostTF.FormSource.Id, Is.EqualTo(formSource.Id));
            Assert.That(victimPostTF.Item.ItemSource.Id, Is.EqualTo(itemSource.Id));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == botVictim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner.Id, Is.EqualTo(attacker.Id));
            Assert.That(newItem.PvPEnabled, Is.EqualTo((int) GameModeStatics.GameModes.Any));
            Assert.That(newItem.IsPermanent, Is.True);
            Assert.That(newItem.Level, Is.EqualTo(botVictim.Level));
            Assert.That(newItem.dbLocationName, Is.Empty);
            Assert.That(newItem.ItemSource.FriendlyName, Is.EqualTo(itemSource.FriendlyName));
            Assert.That(newItem.ConsentsToSoulbinding, Is.True);
        }

        [Test]
        public void fall_on_ground_if_no_inventory_space_for_attacker()
        {
            attacker = new PlayerBuilder()
                .With(p => p.Id, 296)
                .With(p => p.FirstName, "Attacker")
                .With(p => p.LastName, "Smacker")
                .With(p => p.Items, emptyItemList)
                .With(p => p.ExtraInventory, -9999)
                .BuildAndSave();

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog").EqualTo("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a A new Item!!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here."));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner, Is.Null);
            Assert.That(newItem.dbLocationName, Is.EqualTo(victim.Location));
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


            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = petFormSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog").EqualTo("<br><b>You fully transformed Victim MgGee into a Squeaky Pet</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a Squeaky Pet!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Victim MgGee was completely transformed into a Squeaky Pet</b> here."));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner, Is.EqualTo(attacker));
            Assert.That(newItem.IsEquipped, Is.True);
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

            var petList = new List<Item>
            {
                pet
            };

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

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = petFormSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog").EqualTo("<br><b>You fully transformed Victim MgGee into a Squeaky Pet</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a Squeaky Pet!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Victim MgGee was completely transformed into a Squeaky Pet</b> here."));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner, Is.Null);
            Assert.That(newItem.IsEquipped, Is.False);
            Assert.That(newItem.dbLocationName, Is.EqualTo(victim.Location));
        }

        [Test]
        public void should_create_item_when_attacker_null()
        {
            var cmd = new PlayerBecomesItem { AttackerId = null, VictimId = victim.Id, NewFormId = formSource.Id };

            Assert.That(DomainRegistry.Repository.Execute(cmd),
                Has.Property("AttackerLog").EqualTo("<br><b>You fully transformed Victim MgGee into a A new Item!</b>!")
                    .And.Property("VictimLog")
                    .EqualTo("<br><b>You have been fully transformed into a A new Item!!</b>!")
                    .And.Property("LocationLog")
                    .EqualTo("<br><b>Victim MgGee was completely transformed into a A new Item!</b> here."));

            var victimPostTF = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == victim.Id);
            Assert.That(victimPostTF, Is.Not.Null);
            Assert.That(victimPostTF.Mobility, Is.EqualTo(PvPStatics.MobilityInanimate));
            Assert.That(victimPostTF.FormSource.Id, Is.EqualTo(formSource.Id));
            Assert.That(victimPostTF.Item.ItemSource.Id, Is.EqualTo(itemSource.Id));

            var newItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.FormerPlayer != null && i.FormerPlayer.Id == victim.Id);
            Assert.That(newItem, Is.Not.Null);
            Assert.That(newItem.Owner, Is.Null);
            Assert.That(newItem.dbLocationName, Is.EqualTo(victim.Location));
            Assert.That(newItem.PvPEnabled, Is.EqualTo((int) GameModeStatics.GameModes.Any)); // chaos poof turns into any game mode
        }

        [Test]
        public void Should_throw_exception_if_victim_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = -1, NewFormId = formSource.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player (victim) with ID '-1' could not be found"));

        }

        [Test]
        public void Should_throw_exception_if_attacker_specified_but_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = 123, VictimId = victim.Id, NewFormId = formSource.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player (attacker) with ID '123' could not be found"));

        }

        [Test]
        public void Should_throw_exception_if_new_form_not_found()
        {
            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = -1 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Form with ID '-1' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_item_source_not_found()
        {

            var formSource2 = new FormSourceBuilder()
                .With(i => i.Id, 99)
                .With(i => i.ItemSource, null)
                .BuildAndSave();

            var cmd = new PlayerBecomesItem { AttackerId = attacker.Id, VictimId = victim.Id, NewFormId = formSource2.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Form is not inanimate or pet"));
        }
    }
}
