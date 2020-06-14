using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.AI.Commands;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.AI.Commands
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
            Assert.That(playerLoaded.Items, Has.Exactly(4).Items);

            var itemDates = playerLoaded.Items.Select(i => i.LastSold).ToList();
            Assert.That(itemDates, Is.All.EqualTo(DateTime.UtcNow).Within(1).Minutes);

            var itemNames = playerLoaded.Items.Select(i => i.ItemSource.FriendlyName).ToList();

            Assert.That(itemNames.ElementAt(0), Is.EqualTo("socks"));
            Assert.That(itemNames.ElementAt(1), Is.EqualTo("hats"));
            Assert.That(itemNames.ElementAt(2), Is.EqualTo("hats"));
            Assert.That(itemNames.ElementAt(3), Is.EqualTo("hats"));

            Assert.That(itemNames, Has.Member(socks.BaseItem.FriendlyName));
            Assert.That(itemNames, Has.Member(hat.BaseItem.FriendlyName));
            Assert.That(itemNames, Has.No.Member(bonnet.BaseItem.FriendlyName));
        }

        [Test]
        public void should_throw_error_if_npc_not_found()
        {
            var cmd = new RestockNPC { BotId = 12345 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with BotId '12345' could not be found"));
        }
    }
}
