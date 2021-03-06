﻿using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemsOwnedByPlayerTests : TestBase
    {
        [Test]
        public void get_all_items_owned_by_player()
        {

            var player = new PlayerBuilder()
               .With(p => p.Id, 99)
               .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 21)
                .With(cr => cr.Owner, null)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 35)
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 37)
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 100)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 39)
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemsOwnedByPlayer { OwnerId = 99 };

            var items = DomainRegistry.Repository.Find(cmd).ToList();

            Assert.That(items, Has.Exactly(2).Items);
            Assert.That(items.ElementAt(0).Id, Is.EqualTo(99));
            Assert.That(items.ElementAt(1).Id, Is.EqualTo(100));
        }

    }
}
