using System.Collections.Generic;
using System.Linq;
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
    public class RemoveSoulbindingOnItemTests : TestBase
    {

        private Player player;
        private Player formerPlayerInanimate;
        private Item soulboundItem;

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

            soulboundItem = new ItemBuilder()
                .With(i => i.Id, 123)
                .With(i => i.Owner, player)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, formerPlayerInanimate)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            player.GiveItem(soulboundItem);

        }

        [Test]
        public void should_remove_soulbinding()
        {

            var cmd = new RemoveSoulbindingOnItem { ItemId = soulboundItem.Id };
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var itemLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundItem.Id);
            Assert.That(itemLoaded, Is.Not.Null);
            Assert.That(itemLoaded.SoulboundToPlayer, Is.Null);
            Assert.That(itemLoaded.ConsentsToSoulbinding, Is.False);
        }

        [Test]
        public void throw_exception_if_item_not_found()
        {
            var cmd = new RemoveSoulbindingOnItem { ItemId = 12345 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with Id '12345' could not be found."));
        }

    }
}
