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
    public class MoveAbandonedPetsToWuffieTest : TestBase
    {

        [Test]
        public void Should_move_abandoned_pets_to_Wuffie()
        {

            new PlayerBuilder()
                .With(p => p.Id, 1).BuildAndSave();

            // eligible to get sent to Wuffie
            new ItemBuilder()
                .With(p => p.Id, 100)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .With(i => i.Id, 7)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-9))
                .BuildAndSave();

            // dropped too recently
            new ItemBuilder()
                .With(p => p.Id, 101)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .With(i => i.Id, 5)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-1))
                .BuildAndSave();

            // not a pet type
            new ItemBuilder()
                .With(p => p.Id, 102)
                .With(i => i.Owner, null)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Accessory)
                    .With(i => i.Id, 3)
                    .BuildAndSave())
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-15))
                .BuildAndSave();


            var cmd = new MoveAbandonedPetsToWuffie {WuffieId = 1};
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Item>().Where(i => i.Owner != null && i.Owner.Id == 1),
                Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_exception_if_wuffie_not_found()
        {
            var cmd = new MoveAbandonedPetsToWuffie { WuffieId = 13 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Could not find Wuffie with Id 13"));
        }
    }
}
