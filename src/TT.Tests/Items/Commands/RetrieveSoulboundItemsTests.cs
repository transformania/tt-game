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

    [TestFixture]
    public class RetrieveSoulboundItemsTests : TestBase
    {

        private Player player;
        private Player inanimatePlayer;
        private Player inanimatePlayer2;
        private Player soulbindingNpc;
        private Item souldboundItemToTransfer1;
        private Item souldboundItemToTransfer2;
        private Item soulboundPetToTransfer;
        private Item soulboundItemOnGround;
        private Item soulboundItemForSomeoneElse;

        private Item rune;

        //private Item rune1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            soulbindingNpc = new PlayerBuilder()
                .With(p => p.Id, 800)
                .With(p => p.BotId, AIStatics.SoulbinderBotId)
                .With(p => p.Location, "underworld")
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Location, "underworld")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .BuildAndSave();

            inanimatePlayer = new PlayerBuilder()
                .With(p => p.Id, 101)
                .With(p => p.Location, "underworld")
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .BuildAndSave();

            inanimatePlayer2 = new PlayerBuilder()
                .With(p => p.Id, 102)
                .With(p => p.Location, "underworld")
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .BuildAndSave();

            souldboundItemToTransfer1 = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.FormerPlayer, inanimatePlayer)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.Owner, soulbindingNpc)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.FriendlyName, "High Heels")
                    .With(i => i.ItemType, PvPStatics.ItemType_Shoes)
                    .BuildAndSave())
                .With(i => i.Runes, new List<Item>())
                .BuildAndSave();

            rune = new ItemBuilder()
                .With(i => i.Id, 87773)
                .With(i => i.Owner, soulbindingNpc)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.FriendlyName, "Rune of Thingie")
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            souldboundItemToTransfer1.AttachRune(rune);

            souldboundItemToTransfer2 = new ItemBuilder()
                .With(i => i.Id, 501)
                .With(i => i.FormerPlayer, inanimatePlayer)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.Owner, soulbindingNpc)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.FriendlyName, "Bottle of Perfume")
                    .With(i => i.ItemType, PvPStatics.ItemType_Accessory)
                    .BuildAndSave())
                .BuildAndSave();

            soulboundPetToTransfer = new ItemBuilder()
                .With(i => i.Id, 15126)
                .With(i => i.FormerPlayer, inanimatePlayer)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.Owner, soulbindingNpc)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.FriendlyName, "Puppy Doggy")
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            soulboundItemOnGround = new ItemBuilder()
                .With(i => i.Id, 93254)
                .With(i => i.FormerPlayer, inanimatePlayer)
                .With(i => i.SoulboundToPlayer, player)
                .With(i => i.Owner, null)
                .BuildAndSave();

            soulboundItemForSomeoneElse = new ItemBuilder()
                .With(i => i.Id, 45784)
                .With(i => i.FormerPlayer, inanimatePlayer)
                .With(i => i.SoulboundToPlayer, inanimatePlayer2)
                .With(i => i.Owner, soulbindingNpc)
                .BuildAndSave();

            soulbindingNpc.GiveItem(souldboundItemToTransfer1);
            soulbindingNpc.GiveItem(souldboundItemToTransfer2);
            soulbindingNpc.GiveItem(soulboundItemForSomeoneElse);
            soulbindingNpc.GiveItem(soulboundPetToTransfer);
        }

        [Test]
        public void can_transfer_all_soulbound_items_with_runes()
        {
            Assert.That(Repository.Execute(new RetrieveSoulboundItems {PlayerId = player.Id}),
                Is.EqualTo("John Doe returns your soulbound High Heels, Bottle of Perfume, and Puppy Doggy."));

            var playerLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            Assert.That(playerLoaded.Items, Has.Exactly(3).Items);
            Assert.That(playerLoaded.Items.ElementAt(0).Id, Is.EqualTo(souldboundItemToTransfer1.Id));
            Assert.That(playerLoaded.Items.ElementAt(0).Owner.Id, Is.EqualTo(player.Id));
            Assert.That(playerLoaded.Items.ElementAt(1).Id, Is.EqualTo(souldboundItemToTransfer2.Id));
            Assert.That(playerLoaded.Items.ElementAt(1).Owner.Id, Is.EqualTo(player.Id));
            Assert.That(playerLoaded.Items.ElementAt(2).Id, Is.EqualTo(soulboundPetToTransfer.Id));
            Assert.That(playerLoaded.Items.ElementAt(2).Owner.Id, Is.EqualTo(player.Id));

            Assert.That(playerLoaded.Items.Select(i => i.Id),
                Has.No.Member(soulboundItemOnGround.Id).And.No.Member(soulboundItemForSomeoneElse.Id));
        }

        [Test]
        public void dont_transfer_pet_if_player_already_has_one()
        {
            var pet = new ItemBuilder()
                .With(i => i.Id, 83247)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.FriendlyName, "Mole")
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            player.GiveItem(pet);

            Assert.That(Repository.Execute(new RetrieveSoulboundItems {PlayerId = player.Id}),
                Is.EqualTo("John Doe returns your soulbound High Heels and Bottle of Perfume."));

            var playerLoaded = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            Assert.That(playerLoaded.Items, Has.Exactly(3).Items);

            Assert.That(playerLoaded.Items.Select(i => i.Id), Has.No.Member(soulboundPetToTransfer.Id));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new RetrieveSoulboundItems { PlayerId = -99};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("player with ID '-99' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_wrong_location()
        {

            player = new PlayerBuilder()
                .With(p => p.Id, 505)
                .With(p => p.Location, "elsewhere")
                .BuildAndSave();

            var cmd = new RetrieveSoulboundItems { PlayerId = player.Id };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must be in the same location as John Doe to retrieve your soulbound items."));
        }

        [Test]
        public void should_throw_exception_if_player_not_animate()
        {
            var cmd = new RetrieveSoulboundItems { PlayerId = inanimatePlayer.Id };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You must be animate in order to do this."));
        }
    }


}
