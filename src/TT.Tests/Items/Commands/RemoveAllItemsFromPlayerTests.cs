using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    public class RemoveAllItemsFromPlayersTest : TestBase
    {

        private Player lindella;
        private Player wuffie;
        private Player karin;
        private Player player;
        private Player soulboundItemPlayer;
        private Player unlockedItemPlayer;
        private Player lockedItemPlayer;
        private Player lockedPetPlayer;
        private Item soulboundItem;
        private Item unlockedItem;
        private Item lockedItem;
        private Item lockedPet;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();


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

            karin = new PlayerBuilder()
                .With(p => p.Id, 115)
                .With(p => p.BotId, AIStatics.SoulbinderBotId)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();


            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Items, new List<Item>())
                .With(p => p.Location, "player_tile")
                .BuildAndSave();


            soulboundItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 1001)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            unlockedItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 1002)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Location, "old_tile")
                .BuildAndSave();

            lockedItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 1003)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            lockedPetPlayer = new PlayerBuilder()
                .With(p => p.Id, 1004)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();


            soulboundItem = new ItemBuilder()
                .With(i => i.Id, 2001)
                .With(i => i.Owner, player)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, soulboundItemPlayer)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            unlockedItem = new ItemBuilder()
                .With(i => i.Id, 2002)
                .With(i => i.Owner, player)
                .With(i => i.SoulboundToPlayer, null)
                .With(i => i.IsPermanent, false)
                .With(i => i.FormerPlayer, unlockedItemPlayer)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            lockedItem = new ItemBuilder()
                .With(i => i.Id, 2003)
                .With(i => i.Owner, player)
                .With(i => i.SoulboundToPlayer, null)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, lockedItemPlayer)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            lockedPet= new ItemBuilder()
                .With(i => i.Id, 2004)
                .With(i => i.Owner, player)
                .With(i => i.SoulboundToPlayer, null)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, lockedPetPlayer)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            player.GiveItem(soulboundItem);
            player.GiveItem(unlockedItem);
            player.GiveItem(lockedItem);
            player.GiveItem(lockedPet);
        }

        [Test]
        public void should_move_all_items_to_floor_or_npcs()
        {
            var cmd = new RemoveAllItemsFromPlayer {PlayerId = player.Id};
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var lindellaLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == lindella.Id);
            Assert.That(lindellaLoaded, Is.Not.Null);
            Assert.That(lindellaLoaded.Items, Has.Exactly(1).Items);
            Assert.That(lindellaLoaded.Items.ElementAt(0).Id, Is.EqualTo(lockedItem.Id));

            var lockedItemLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == lockedItem.Id);
            Assert.That(lockedItemLoaded, Is.Not.Null);
            Assert.That(lockedItemLoaded.Owner, Is.EqualTo(lindella));
            Assert.That(lockedItemLoaded.IsEquipped, Is.EqualTo(false));

            var lockedItemPlayerLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == lockedItemPlayer.Id);
            Assert.That(lockedItemPlayerLoaded, Is.Not.Null);


            var wuffieLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == wuffie.Id);
            Assert.That(wuffieLoaded, Is.Not.Null);
            Assert.That(wuffieLoaded.Items, Has.Exactly(1).Items);
            Assert.That(wuffieLoaded.Items.ElementAt(0).Id, Is.EqualTo(lockedPet.Id));

            var lockedPetLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == lockedPet.Id);
            Assert.That(lockedPetLoaded, Is.Not.Null);
            Assert.That(lockedPetLoaded.Owner, Is.EqualTo(wuffie));
            Assert.That(lockedPetLoaded.IsEquipped, Is.EqualTo(true));

            var lockedPetPlayerLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == lockedPetPlayer.Id);
            Assert.That(lockedPetPlayerLoaded, Is.Not.Null);


            var karinLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == karin.Id);
            Assert.That(karinLoaded, Is.Not.Null);
            Assert.That(karinLoaded.Items, Has.Exactly(1).Items);
            Assert.That(karinLoaded.Items.ElementAt(0).Id, Is.EqualTo(soulboundItem.Id));

            var soulboundItemLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundItem.Id);
            Assert.That(soulboundItemLoaded, Is.Not.Null);
            Assert.That(soulboundItemLoaded.Owner, Is.EqualTo(karin));
            Assert.That(soulboundItemLoaded.SoulboundToPlayer, Is.EqualTo(player));
            Assert.That(soulboundItemLoaded.IsEquipped, Is.EqualTo(false));

            var soulboundPlayerLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == soulboundItemPlayer.Id);
            Assert.That(soulboundPlayerLoaded, Is.Not.Null);


            var unlockedItemLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == unlockedItem.Id);
            Assert.That(unlockedItemLoaded, Is.Not.Null);
            Assert.That(unlockedItemLoaded.Owner, Is.EqualTo(null));
            Assert.That(unlockedItemLoaded.IsEquipped, Is.EqualTo(false));
            Assert.That(unlockedItemLoaded.dbLocationName, Is.EqualTo("player_tile"));

            var unlockedItemPlayerLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == unlockedItemPlayer.Id);
            Assert.That(unlockedItemPlayerLoaded, Is.Not.Null);
        }
    }
}
