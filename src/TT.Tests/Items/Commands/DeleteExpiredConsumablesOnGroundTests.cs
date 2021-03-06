﻿using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class DeleteExpiredConsumablesOnGroundTests : TestBase
    {
        [Test]
        public void should_delete_expired_consumable_items_on_ground()
        {
            var person = new PlayerBuilder()
                .With(p => p.Id, 1).BuildAndSave();

            var consumable = new ItemSourceBuilder()
                .With(i => i.ItemType, PvPStatics.ItemType_Consumable)
                .With(i => i.Id, 7)
                .BuildAndSave();

            var pet = new ItemSourceBuilder()
                .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                .With(i => i.Id, 8)
                .BuildAndSave();

            var dungeonArtifact = new ItemSourceBuilder()
                .With(i => i.ItemType, PvPStatics.ItemType_Consumable)
                .With(i => i.Id, PvPStatics.ItemType_DungeonArtifact_Id)
                .BuildAndSave();

            // ineligible -- owned by a player
            new ItemBuilder()
                .With(p => p.Id, 1)
                .With(i => i.Owner, person)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- dropped too recently
            new ItemBuilder()
                .With(p => p.Id, 2)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow)
                .BuildAndSave();

            // eligible -- not owned
            new ItemBuilder()
                .With(p => p.Id, 3)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- not a consumable type
            new ItemBuilder()
                .With(p => p.Id, 4)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, pet)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- dungeon artifact
            new ItemBuilder()
                .With(p => p.Id, 6)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, dungeonArtifact)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            var cmd = new DeleteExpiredConsumablesOnGround();
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var idsRemaining = DataContext.AsQueryable<Item>().Select(i => i.Id);

            Assert.That(idsRemaining, Has.Member(1));
            Assert.That(idsRemaining, Has.Member(2));
            Assert.That(idsRemaining, Has.Member(4));
            Assert.That(idsRemaining, Has.No.Member(3));
        }

    }
}
