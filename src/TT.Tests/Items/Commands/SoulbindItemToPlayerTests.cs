using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Game;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    public class SoulbindItemToPlayerTests : TestBase
    {
        private Player formerItemPlayer;
        private Player ownerPlayer;
        private Item item;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            formerItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.FirstName, "Bob")
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .With(p =>  p.Level, 5)
                .BuildAndSave();

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 68)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 33)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

        }

        [Test]
        public void can_soulbind_item()
        {

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Items, new List<Item>())
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(Repository.Execute(cmd),
                Is.EqualTo("You soulbound <b>Bob Doe</b> the <b>Test Item Source</b> for <b>0</b> Arpeyjis."));

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == item.Id);

            Assert.That(editedItem, Is.Not.Null);
            Assert.That(editedItem.SoulboundToPlayer.FirstName, Is.EqualTo("Sam"));
            Assert.That(editedItem.SoulboundToPlayer.BotId, Is.EqualTo(AIStatics.ActivePlayerBotId));
            Assert.That(editedItem.SoulboundToPlayer.Id, Is.EqualTo(ownerPlayer.Id));
            Assert.That(editedItem.FormerPlayer.PlayerLogs, Has.Exactly(1).Items);
            Assert.That(editedItem.FormerPlayer.PlayerLogs.ElementAt(0).Message,
                Is.EqualTo("Sam Doe has soulbound you!  No other players will be able to claim you as theirs."));
        }

        [Test]
        public void should_throw_exception_if_item_null()
        {
            var cmd = new SoulbindItemToPlayer
            {
                ItemId = 12345,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with ID '12345' not found."));
        }

        [Test]
        public void should_throw_exception_if_player_null()
        {
            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = 12345
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID '12345' not found."));
        }

        [Test]
        public void should_throw_exception_if_item_not_locked()
        {

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, false)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Only permanent items or pets may be souldbound."));
        }

        [Test]
        public void should_throw_exception_if_player_doesnt_own_item()
        {
            var someoneElse = new PlayerBuilder()
                .With(p => p.Id, 1831)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, someoneElse)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, false)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't own that item."));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void should_throw_exception_if_player_too_low_level(int level)
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 1831)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Level, level)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = player.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must be at least level 4 in order to soulbind any items or pets to you."));
        }

        [Test]
        public void should_throw_exception_if_item_nonconsenting()
        {
            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 734)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, false)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This item is not currently consenting to soulbinding."));
        }

        [Test]
        public void should_throw_exception_if_insufficient_arpeyjis()
        {

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 150)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Items, new List<Item>())
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 187)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var previousSouledItem = new ItemBuilder()
                .With(i => i.Id, 183)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.IsPermanent, true)
                .With(i => i.SoulboundToPlayer, ownerPlayer)
                .BuildAndSave();

            Assert.That(() => ownerPlayer.GiveItem(previousSouledItem), Throws.Nothing);

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You cannot afford this.  You need <b>60</b> Arpeyjis and only have <b>0</b>."));
        }

        [Test]
        [TestCase(AIStatics.MaleRatBotId)]
        [TestCase(AIStatics.FemaleRatBotId)]
        public void can_soulbind_rat_boss_when_event_over(int botId)
        {
            formerItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 7471)
                .With(p => p.FirstName, "This")
                .With(p => p.LastName, "Rat")
                .With(p => p.BotId, botId)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            new WorldBuilder()
                .With(i => i.Id, 77)
                .With(w => w.Boss_Thief, "completed")
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(Repository.Execute(cmd),
                Is.EqualTo("You soulbound <b>This Rat</b> the <b>Test Item Source</b> for <b>0</b> Arpeyjis."));

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == item.Id);

            Assert.That(editedItem, Is.Not.Null);
            Assert.That(editedItem.SoulboundToPlayer.Id, Is.EqualTo(ownerPlayer.Id));
            Assert.That(editedItem.SoulboundToPlayer.FirstName, Is.EqualTo("Sam"));
            Assert.That(editedItem.SoulboundToPlayer.BotId, Is.EqualTo(AIStatics.ActivePlayerBotId));
        }

        [Test]
        [TestCase(AIStatics.MaleRatBotId)]
        [TestCase(AIStatics.FemaleRatBotId)]
        public void should_throw_exception_if_soulbinding_rat_boss_during_event(int botId)
        {
            formerItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 7472)
                .With(p => p.FirstName, "This")
                .With(p => p.LastName, "Rat")
                .With(p => p.BotId, botId)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            new WorldBuilder()
                .With(i => i.Id, 77)
                .With(w => w.Boss_Thief, AIStatics.ACTIVE)
                .BuildAndSave();

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You cannot soulbind This Rat until both rats have been defeated."));
        }

    }
}
