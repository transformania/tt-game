using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class DeleteExpiredRunesOnMerchantsTests : TestBase
    {
        [Test]
        public void should_delete_expired_runes_on_merchants()
        {
            var lindella = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            var skaldyr = new PlayerBuilder()
                .With(p => p.Id, 2)
                .With(p => p.BotId, AIStatics.LoremasterBotId)
                .BuildAndSave();

            var runeSource = new ItemSourceBuilder()
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.Id, 7)
                .BuildAndSave();

            var pet = new ItemSourceBuilder()
                .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                .With(i => i.Id, 8)
                .BuildAndSave();

            // eligible -- owned by lindella
            new ItemBuilder()
                .With(p => p.Id, 1)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, runeSource)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // eligible -- owned by skaldyr
            new ItemBuilder()
                .With(p => p.Id, 2)
                .With(i => i.Owner, skaldyr)
                .With(i => i.ItemSource, runeSource)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- dropped too recently
            new ItemBuilder()
                .With(p => p.Id, 3)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, runeSource)
                .With(i => i.TimeDropped, DateTime.UtcNow)
                .BuildAndSave();

            // ineligible -- not owned
            new ItemBuilder()
                .With(p => p.Id, 4)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, runeSource)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- not a rune type
            new ItemBuilder()
                .With(p => p.Id, 5)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, pet)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // ineligible -- embedded on an item
            new ItemBuilder()
                .With(p => p.Id, 6)
                .With(i => i.Owner, lindella)
                .With(i => i.ItemSource, runeSource)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .With(i => i.EmbeddedOnItem, new ItemBuilder()
                    .With(i => i.Id, 999)
                    .With(i => i.Owner, lindella)
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteExpiredRunesOnMerchants();
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var idsRemaining = DataContext.AsQueryable<Item>().Select(i => i.Id);

            Assert.That(idsRemaining, Has.Member(3));
            Assert.That(idsRemaining, Has.Member(4));
            Assert.That(idsRemaining, Has.Member(5));
            Assert.That(idsRemaining, Has.Member(6));

            Assert.That(idsRemaining, Has.No.Member(1));
            Assert.That(idsRemaining, Has.No.Member(2));
        }

        [Test]
        public void should_throw_exception_if_lindella_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.BotId, AIStatics.LoremasterBotId)
                .BuildAndSave();

            var cmd = new DeleteExpiredRunesOnMerchants();
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Could not find Lindella with BotId -3"));
        }

        [Test]
        public void should_throw_exception_if_skaldyr_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            var cmd = new DeleteExpiredRunesOnMerchants();
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Could not find Lorekeeper with BotId -15"));
        }
    }
}
