using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    public class RemoveSoulbindingOnPlayerItemsTests : TestBase
    {

        private Player player;
        private Player formerPlayerInanimate;
        private Player formerPlayerPet;
        private Player lindella;
        private Player wuffie;
        private Player soulbinder;
        private Item soulboundItem;
        private Item soulboundPet;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();


            formerPlayerInanimate = new PlayerBuilder()
                .With(p => p.Id, 2345)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .BuildAndSave();

            formerPlayerPet = new PlayerBuilder()
                .With(p => p.Id, 21235)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .BuildAndSave();

            lindella = new PlayerBuilder()
                .With(p => p.Id, 105)
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            wuffie = new PlayerBuilder()
                .With(p => p.Id, 110)
                .With(p => p.BotId, AIStatics.WuffieBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            soulbinder = new PlayerBuilder()
                .With(p => p.Id, 114)
                .With(p => p.BotId, AIStatics.SoulbinderBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            soulboundItem = new ItemBuilder()
                .With(i => i.Id, 123)
                .With(i => i.Owner, soulbinder)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, formerPlayerInanimate)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            soulboundPet= new ItemBuilder()
                .With(i => i.Id, 124)
                .With(i => i.Owner, soulbinder)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, formerPlayerPet)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            soulbinder.GiveItem(soulboundItem);
            soulbinder.GiveItem(soulboundPet);

        }

        [Test]
        public void should_remove_soulbinding_and_transfer_items_to_merchants()
        {

            var cmd = new RemoveSoulbindingOnPlayerItems {PlayerId = player.Id};
            Repository.Execute(cmd);

            var lindellaLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == lindella.Id);
            lindellaLoaded.Items.Count.Should().Be(1);
            lindellaLoaded.Items.ElementAt(0).Id.Should().Be(soulboundItem.Id);

            var wuffieLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == wuffie.Id);
            wuffieLoaded.Items.Count.Should().Be(1);
            wuffieLoaded.Items.ElementAt(0).Id.Should().Be(soulboundPet.Id);

            var soulboundItemLoaded = DataContext.AsQueryable<Item>().First(p => p.Id == soulboundItem.Id);
            soulboundItemLoaded.SoulboundToPlayer.Should().Be(null);
            soulboundItemLoaded.Owner.Should().Be(lindella);
            soulboundItemLoaded.ConsentsToSoulbinding.Should().Be(false);

            var soulboundPetLoaded = DataContext.AsQueryable<Item>().First(p => p.Id == soulboundPet.Id);
            soulboundPetLoaded.SoulboundToPlayer.Should().Be(null);
            soulboundPetLoaded.Owner.Should().Be(wuffie);
            soulboundPetLoaded.ConsentsToSoulbinding.Should().Be(false);

            var formerPlayerInanimateLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == this.formerPlayerInanimate.Id);
            formerPlayerInanimateLoaded.PlayerLogs.Count.Should().Be(1);
            formerPlayerInanimateLoaded.PlayerLogs.ElementAt(0).Message.Should().Be("Your past owner has lost the last of their own humanity, shattering the soulbinding between you.");
            formerPlayerInanimateLoaded.PlayerLogs.ElementAt(0).IsImportant.Should().Be(true);

            var formerPlayerPetLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == this.formerPlayerPet.Id);
            formerPlayerPetLoaded.PlayerLogs.Count.Should().Be(1);
            formerPlayerPetLoaded.PlayerLogs.ElementAt(0).Message.Should().Be("Your past owner has lost the last of their own humanity, shattering the soulbinding between you.");
            formerPlayerPetLoaded.PlayerLogs.ElementAt(0).IsImportant.Should().Be(true);

        }

    }
}
