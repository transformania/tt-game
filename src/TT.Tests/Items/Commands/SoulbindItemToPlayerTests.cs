using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
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
                .BuildAndSave();

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 68)
                .With(p => p.FirstName, "Sam")
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

            var result = Repository.Execute(cmd);
            result.Should().Be("You soulbound <b>Bob Doe</b> the <b>Test Item Source</b> for <b>0</b> Arpeyjis.");

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == item.Id);

            editedItem.SoulboundToPlayer.FirstName.Should().Be("Sam");
            editedItem.SoulboundToPlayer.BotId.Should().Be(AIStatics.ActivePlayerBotId);
            editedItem.SoulboundToPlayer.Id.Should().Be(ownerPlayer.Id);
            editedItem.FormerPlayer.PlayerLogs.Count.Should().Be(1);
            editedItem.FormerPlayer.PlayerLogs.ElementAt(0).Message.Should().Be("Sam Doe has soulbound you!  No other players will be able to claim you as theirs.");

        }

        [Test]
        public void should_throw_exception_if_item_null()
        {
            var cmd = new SoulbindItemToPlayer
            {
                ItemId = 12345,
                OwnerId = ownerPlayer.Id,
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Item with id '12345' not found.");

        }

        [Test]
        public void should_throw_exception_if_player_null()
        {
            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = 12345
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with id '12345' not found.");

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

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Only permanent items or pets may be souldbound.");
        }

        [Test]
        public void should_throw_exception_if_player_doesnt_own_item()
        {
            var someoneElse = new PlayerBuilder()
                .With(p => p.Id, 1831)
                .With(p => p.FirstName, "Sam")
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

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You don't own that item.");
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

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("This item is not currently consenting to soulbinding.");
        }

        [Test]
        public void should_throw_exception_if_insufficient_arpeyjis()
        {

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 150)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Items, new List<Item>())
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

            ownerPlayer.GiveItem(previousSouledItem);

            var cmd = new SoulbindItemToPlayer
            {
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You cannot afford this.  You need <b>100</b> Arpeyjis and only have <b>0</b>.");

        }
    }
}
