using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    public class GetItemByVictimNameTests : TestBase
    {

        [Test]
        public void Should_fetch_item_by_victim_name()
        {

           var player = new PlayerBuilder()
               .With(p => p.Id, 99)
               .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 31)
                .With(cr => cr.Owner, player)
                .With(i => i.VictimName, "Tomtom Brown")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemByVictimName {FirstName = "Tomtom", LastName = "Brown" };
            var item = DomainRegistry.Repository.FindSingle(cmd);

            item.Id.Should().Be(31);
        }

    }
}
