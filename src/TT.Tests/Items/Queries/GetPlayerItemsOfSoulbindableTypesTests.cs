using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    public class GetPlayerItemsOfSoulbindableTypesTests : TestBase
    {
        [Test]
        public void Should_fetch_all_items_of_bindable_types()
        {
            var owner = new PlayerBuilder()
                .With(p => p.Id, 99)
                .BuildAndSave();

            var accessory = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Accessory)
                    .BuildAndSave())
                .BuildAndSave();

            var reusableConsumable = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Consumable_Reuseable)
                    .BuildAndSave())
                .BuildAndSave();

            var hat = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Hat)
                    .BuildAndSave())
                .BuildAndSave();

            var pants = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            var pet = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            var shirt = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .BuildAndSave();

            var shoes = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Shoes)
                    .BuildAndSave())
                .BuildAndSave();

            var underpants = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Underpants)
                    .BuildAndSave())
                .BuildAndSave();

            var undershirt = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Undershirt)
                    .BuildAndSave())
                .BuildAndSave();

            var items = DomainRegistry.Repository.Find(new GetPlayerItemsOfSoulbindableTypes { OwnerId = owner.Id }).ToList();

            Assert.That(items, Has.Exactly(9).Items);
        }

        [Test]
        public void Should_fetch_no_items_of_nonbindable_types()
        {
            var owner = new PlayerBuilder()
                .With(p => p.Id, 99)
                .BuildAndSave();

            var consumable = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Consumable)
                    .BuildAndSave())
                .BuildAndSave();

            var rune = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            var items = DomainRegistry.Repository.Find(new GetPlayerItemsOfSoulbindableTypes { OwnerId = owner.Id }).ToList();

            Assert.That(items, Has.Exactly(0).Items);
        }

        [Test]
        public void Should_fetch_no_items_with_mismatched_owner()
        {
            var owner = new PlayerBuilder()
                .With(p => p.Id, 99)
                .BuildAndSave();

            var nonowner = new PlayerBuilder()
                .With(p => p.Id, 66)
                .BuildAndSave();

            var nonBindableItem = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Owner, owner)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Consumable)
                    .BuildAndSave())
                .BuildAndSave();

            var bindableItem = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(p => p.ItemSource, new ItemSourceBuilder()
                    .With(s => s.ItemType, PvPStatics.ItemType_Consumable_Reuseable)
                    .BuildAndSave())
                .BuildAndSave();

            var items = DomainRegistry.Repository.Find(new GetPlayerItemsOfSoulbindableTypes { OwnerId = nonowner.Id }).ToList();

            Assert.That(items, Has.Exactly(0).Items);
        }
    }
}
