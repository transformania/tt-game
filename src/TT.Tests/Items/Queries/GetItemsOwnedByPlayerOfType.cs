﻿using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemsOwnedByPlayerOfTypeTests : TestBase
    {
        [Test]
        public void get_all_items_owned_by_player_of_type()
        {

            var player = new PlayerBuilder()
               .With(p => p.FirstName, "Jones")
               .With(p => p.Id, 99)
               .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 21)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 35)
                    .With(i => i.ItemType, "squirrels")
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 37)
                    .With(i => i.ItemType, "fish")
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 100)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 39)
                    .With(i => i.ItemType, "fish")
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 105)
                .With(cr => cr.Owner, null)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 41)
                    .With(i => i.ItemType, "fish")
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemsOwnedByPlayerOfType { OwnerId = 99, ItemType = "fish"};

            var items = DomainRegistry.Repository.Find(cmd).ToList();

            Assert.That(items, Has.Exactly(2).Items);
            Assert.That(items.ElementAt(0).Id, Is.EqualTo(99));
            Assert.That(items.ElementAt(1).Id, Is.EqualTo(100));
        }

    }
}
