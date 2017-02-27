using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.AI;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Players;
using TT.Domain.Statics;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.AI
{
    [TestFixture]
    public class RestockNPCTests : TestBase
    {
        [Test]
        public void should_restock_npc()
        {
            var player = new PlayerBuilder()
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            // add 1 (from 0)
            var socks = new RestockItemBuilder()
                .With(r => r.Id, 100)
                .With(r => r.BaseItem, new ItemSourceBuilder()
                    .With(i => i.Id, 50)
                    .With(i => i.FriendlyName, "socks")
                    .BuildAndSave())
                .With(r => r.AmountToRestockTo, 1)
                .With(r => r.AmountBeforeRestock, 0)
                .With(r => r.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            // add 3 (from 0)
            var hat = new RestockItemBuilder()
                .With(r => r.Id, 105)
                .With(r => r.BaseItem, new ItemSourceBuilder()
                    .With(i => i.Id, 55)
                    .With(i => i.FriendlyName, "hats")
                    .BuildAndSave())
                .With(r => r.AmountToRestockTo, 3)
                .With(r => r.AmountBeforeRestock, 1)
                .With(r => r.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            // don't add any, wrong bot
            var bonnet = new RestockItemBuilder()
                .With(r => r.Id, 110)
                .With(r => r.BaseItem, new ItemSourceBuilder()
                    .With(i => i.Id, 60)
                    .With(i => i.FriendlyName, "bonnet")
                    .BuildAndSave())
                .With(r => r.AmountToRestockTo, 3)
                .With(r => r.AmountBeforeRestock, 1)
                .With(r => r.BotId, AIStatics.LoremasterBotId)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new RestockNPC {BotId = player.BotId});

            var playerLoaded = DataContext.AsQueryable<Player>().First();
            playerLoaded.Items.Count.Should().Be(4);

            var itemDates = playerLoaded.Items.Select(i => i.LastSold).ToList();
            itemDates.ElementAt(0).Should().BeCloseTo(DateTime.UtcNow, 60000);
            itemDates.ElementAt(1).Should().BeCloseTo(DateTime.UtcNow, 60000);
            itemDates.ElementAt(2).Should().BeCloseTo(DateTime.UtcNow, 60000);
            itemDates.ElementAt(3).Should().BeCloseTo(DateTime.UtcNow, 60000);

            var itemNames = playerLoaded.Items.Select(i => i.ItemSource.FriendlyName).ToList();

            itemNames.ElementAt(0).Should().Be("socks");
            itemNames.ElementAt(1).Should().Be("hats");
            itemNames.ElementAt(2).Should().Be("hats");
            itemNames.ElementAt(3).Should().Be("hats");

            itemNames.Contains(socks.BaseItem.FriendlyName).Should().Be(true);
            itemNames.Contains(hat.BaseItem.FriendlyName).Should().Be(true);
            itemNames.Contains(bonnet.BaseItem.FriendlyName).Should().Be(false);
        }

        [Test]
        public void should_throw_error_if_npc_not_found()
        {
            var cmd = new RestockNPC { BotId = 12345 };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.ShouldThrowExactly<DomainException>().WithMessage("Player with BotId '12345' could not be found");
        }
    }
}
