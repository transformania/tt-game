using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    public class GetItemByFormerPlayerTests : TestBase
    {

        [Test]
        public void Should_fetch_item_by_former_player()
        {

           var player = new PlayerBuilder()
               .With(p => p.Id, 99)
               .BuildAndSave();
            var victim = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 31)
                .With(cr => cr.Owner, player)
                .With(i => i.FormerPlayer, victim)
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemByFormerPlayer { PlayerId = victim.Id };
            Assert.That(DomainRegistry.Repository.FindSingle(cmd).Id, Is.EqualTo(31));
        }

    }
}
