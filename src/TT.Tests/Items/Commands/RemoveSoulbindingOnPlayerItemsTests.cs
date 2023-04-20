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
    public class RemoveSoulbindingOnPlayerItemsTests : TestBase
    {

        private Player player;
        private Player formerPlayerInanimate;
        private Player formerPlayerPet;
        private Player formerPlayerPsycho;
        private Player lindella;
        private Player wuffie;
        private Player soulbinder;
        private Item soulboundItem;
        private Item soulboundPet;
        private Item soulboundPsychoItem;
        private Item soulboundRecycledItem;

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

            formerPlayerPsycho = new PlayerBuilder()
                .With(p => p.Id, 22345)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
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

            soulboundPsychoItem = new ItemBuilder()
                .With(i => i.Id, 125)
                .With(i => i.Owner, soulbinder)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, formerPlayerPsycho)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            soulboundRecycledItem = new ItemBuilder()
                .With(i => i.Id, 126)
                .With(i => i.Owner, soulbinder)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.FormerPlayer, null)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            soulbinder.GiveItem(soulboundItem);
            soulbinder.GiveItem(soulboundPet);
            soulbinder.GiveItem(soulboundPsychoItem);
            soulbinder.GiveItem(soulboundRecycledItem);

        }

        [Test]
        public void should_remove_soulbinding_and_transfer_items_to_merchants()
        {

            var cmd = new RemoveSoulbindingOnPlayerItems {PlayerId = player.Id};
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var lindellaLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == lindella.Id);
            Assert.That(lindellaLoaded, Is.Not.Null);
            Assert.That(lindellaLoaded.Items, Has.Exactly(3).Items);
            Assert.That(lindellaLoaded.Items.ElementAt(0).Id, Is.EqualTo(soulboundItem.Id));
            Assert.That(lindellaLoaded.Items.ElementAt(1).Id, Is.EqualTo(soulboundPsychoItem.Id));
            Assert.That(lindellaLoaded.Items.ElementAt(2).Id, Is.EqualTo(soulboundRecycledItem.Id));

            var wuffieLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == wuffie.Id);
            Assert.That(wuffieLoaded, Is.Not.Null);
            Assert.That(wuffieLoaded.Items, Has.Exactly(1).Items);
            Assert.That(wuffieLoaded.Items.ElementAt(0).Id, Is.EqualTo(soulboundPet.Id));

            var soulboundItemLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundItem.Id);
            Assert.That(soulboundItemLoaded, Is.Not.Null);
            Assert.That(soulboundItemLoaded.SoulboundToPlayer, Is.Null);
            Assert.That(soulboundItemLoaded.Owner, Is.EqualTo(lindella));
            Assert.That(soulboundItemLoaded.ConsentsToSoulbinding, Is.False);

            var soulboundPetLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundPet.Id);
            Assert.That(soulboundPetLoaded, Is.Not.Null);
            Assert.That(soulboundPetLoaded.SoulboundToPlayer, Is.Null);
            Assert.That(soulboundPetLoaded.Owner, Is.EqualTo(wuffie));
            Assert.That(soulboundPetLoaded.ConsentsToSoulbinding, Is.False);

            var soulboundPsychoLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundPsychoItem.Id);
            Assert.That(soulboundPsychoLoaded, Is.Not.Null);
            Assert.That(soulboundPsychoLoaded.SoulboundToPlayer, Is.Null);
            Assert.That(soulboundPsychoLoaded.Owner, Is.EqualTo(lindella));
            Assert.That(soulboundPsychoLoaded.ConsentsToSoulbinding, Is.True);

            var soulboundRecycledLoaded = DataContext.AsQueryable<Item>().FirstOrDefault(p => p.Id == soulboundRecycledItem.Id);
            Assert.That(soulboundRecycledLoaded, Is.Not.Null);
            Assert.That(soulboundRecycledLoaded.SoulboundToPlayer, Is.Null);
            Assert.That(soulboundRecycledLoaded.Owner, Is.EqualTo(lindella));
            Assert.That(soulboundRecycledLoaded.ConsentsToSoulbinding, Is.True);

            var formerPlayerInanimateLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == this.formerPlayerInanimate.Id);
            Assert.That(formerPlayerInanimateLoaded, Is.Not.Null);
            Assert.That(formerPlayerInanimateLoaded.PlayerLogs, Has.Exactly(1).Items);
            Assert.That(formerPlayerInanimateLoaded.PlayerLogs.ElementAt(0).Message,
                Is.EqualTo(
                    "Your soulbinding has been shattered between you and your owner!"));
            Assert.That(formerPlayerInanimateLoaded.PlayerLogs.ElementAt(0).IsImportant, Is.True);

            var formerPlayerPetLoaded = DataContext.AsQueryable<Player>().FirstOrDefault(p => p.Id == this.formerPlayerPet.Id);
            Assert.That(formerPlayerPetLoaded, Is.Not.Null);
            Assert.That(formerPlayerPetLoaded.PlayerLogs, Has.Exactly(1).Items);
            Assert.That(formerPlayerPetLoaded.PlayerLogs.ElementAt(0).Message,
                Is.EqualTo(
                    "Your soulbinding has been shattered between you and your owner!"));
            Assert.That(formerPlayerPetLoaded.PlayerLogs.ElementAt(0).IsImportant, Is.True);
        }
    }
}
