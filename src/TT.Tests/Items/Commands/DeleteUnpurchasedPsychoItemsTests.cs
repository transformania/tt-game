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

namespace TT.Tests.Items.Commands
{
    public class DeleteUnpurchasedPsychoItemsTests : TestBase
    {
        [Test]
        public void should_delete_unwanted_psycho_items_on_lindella_or_wuffie()
        {
            var lindella = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            var wuffie = new PlayerBuilder()
                .With(p => p.Id, 2)
                .With(p => p.BotId, AIStatics.WuffieBotId)
                .BuildAndSave();

            var randomPerson = new PlayerBuilder()
                .With(p => p.Id, 3)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var psychoOne = new PlayerBuilder()
                .With(p => p.Id, 4)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var psychoTwo = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var psychoThree = new PlayerBuilder()
                .With(p => p.Id, 6)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var psychoFour = new PlayerBuilder()
                .With(p => p.Id, 7)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            // a pyscho item owned by Lindella of sufficiently long age.  Should be deleted.
            var deletablePsychoItem = new ItemBuilder()
                .With(p => p.Id, 100)
                .With(i => i.Owner, lindella)
                .With(i => i.FormerPlayer, psychoOne)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shoes)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddDays(-5))
                .BuildAndSave();

            // a pyscho pet owned by Wuffie of sufficiently long age.  Should be deleted.
            var deletablePetItem = new ItemBuilder()
                .With(p => p.Id, 101)
                .With(i => i.Owner, wuffie)
                .With(i => i.FormerPlayer, psychoTwo)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddDays(-5))
                .BuildAndSave();

            // a pyscho item owned by a random person.  Should not be deleted.
            var nonDeleteableItem = new ItemBuilder()
                .With(p => p.Id, 102)
                .With(i => i.Owner, randomPerson)
                .With(i => i.FormerPlayer, psychoThree)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddDays(-5))
                .BuildAndSave();

            // a pyscho item owned by a Lindella but owned recently.  Should not be deleted.
            var nonDeleteableRecentItem = new ItemBuilder()
                .With(p => p.Id, 103)
                .With(i => i.Owner, lindella)
                .With(i => i.FormerPlayer, psychoFour)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddDays(-1))
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new DeleteUnpurchasedPsychoItems());

            var items = DataContext.AsQueryable<Item>();
            var itemIds = items.Select(i => i.Id);

            items.Count().Should().Be(2);
            itemIds.Should().NotContain(deletablePsychoItem.Id);
            itemIds.Should().NotContain(deletablePetItem.Id);
            itemIds.Should().Contain(nonDeleteableItem.Id);
            itemIds.Should().Contain(nonDeleteableRecentItem.Id);

        }

        [Test]
        public void should_throw_exception_if_lindella_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 2)
                .With(p => p.BotId, AIStatics.WuffieBotId)
                .BuildAndSave();

            var action = new Action(() => { Repository.Execute(new DeleteUnpurchasedPsychoItems()); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Could not find Lindella with BotId -3");

        }

        [Test]
        public void should_throw_exception_if_wuffienot_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 2)
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            var action = new Action(() => { Repository.Execute(new DeleteUnpurchasedPsychoItems()); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Could not find Wuffie with BotId -10");

        }
    }
}
