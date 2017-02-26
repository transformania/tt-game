using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Items
{
    [TestFixture]
    public class DeleteExpiredConsumablesOnMerchantsTests : TestBase
    {
        [Test]
        public void should_delete_expired_consumable_items_on_merchants()
        {
            var lindella = new PlayerBuilder()
                .With(p => p.Id, 1).BuildAndSave();

            var skaldyr = new PlayerBuilder()
                .With(p => p.Id, 2).BuildAndSave();

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

            // eligible -- owned by lindella
            new ItemBuilder()
                .With(p => p.Id, 1)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // eligible -- owned by skaldyr
            new ItemBuilder()
                .With(p => p.Id, 2)
                .With(i => i.Owner, skaldyr)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- dropped too recently
            new ItemBuilder()
                .With(p => p.Id, 3)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow)
                .BuildAndSave();

            // ineligible -- not owned
            new ItemBuilder()
                .With(p => p.Id, 4)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, consumable)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- not a consumable type
            new ItemBuilder()
                .With(p => p.Id, 5)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, pet)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- dungeon artifact
            new ItemBuilder()
                .With(p => p.Id, 6)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, dungeonArtifact)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            var cmd = new DeleteExpiredConsumablesOnMerchants { LindellaId = 1, LorekeeperId = 2};
            Repository.Execute(cmd);

            var idsRemaining = DataContext.AsQueryable<Item>().Select(i => i.Id);

            idsRemaining.Should().Contain(3);
            idsRemaining.Should().Contain(4);
            idsRemaining.Should().Contain(5);
            idsRemaining.Should().Contain(6);

            idsRemaining.Should().NotContain(1);
            idsRemaining.Should().NotContain(2);
        }

        [Test]
        public void should_throw_exception_if_lindella_not_found()
        {
            var cmd = new DeleteExpiredConsumablesOnMerchants { LindellaId = 1, LorekeeperId = 2 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Could not find Lindella with Id 1");
        }

        [Test]
        public void should_throw_exception_if_skaldyr_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 1).BuildAndSave();

            var cmd = new DeleteExpiredConsumablesOnMerchants { LindellaId = 1, LorekeeperId = 2 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Could not find Lorekeeper with Id 2");
        }
    }
}
