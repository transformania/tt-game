using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    public class GetPlayerItemsThatCanBeSoulboundTests : TestBase
    {
        [Test]
        public void Should_fetch_item_by_former_player()
        {

            var owner = new PlayerBuilder()
                .With(p => p.Id, 99)
                .BuildAndSave();

            var nonSouledItem = new ItemBuilder()
                .With(p => p.Id, 1)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, null)
                .With(i => i.Owner, owner)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var nonPermanentItem = new ItemBuilder()
                .With(p => p.Id, 2)
                .With(i => i.IsPermanent, false)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.Id, 50)
                    .BuildAndSave())
                .With(i => i.Owner, owner)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var alreadySoulboundItem = new ItemBuilder()
                .With(p => p.Id, 3)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.Id, 50)
                    .BuildAndSave())
                .With(i => i.SoulboundToPlayer, owner)
                .With(i => i.Owner, owner)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var eligibleItem = new ItemBuilder()
                .With(p => p.Id, 4)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.Id, 50)
                    .BuildAndSave())
                .With(i => i.SoulboundToPlayer, null)
                .With(i => i.Owner, owner)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var nonConsentingItem = new ItemBuilder()
                .With(p => p.Id, 4)
                .With(i => i.IsPermanent, true)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.Id, 50)
                    .BuildAndSave())
                .With(i => i.SoulboundToPlayer, null)
                .With(i => i.Owner, owner)
                .With(i => i.ConsentsToSoulbinding, false)
                .BuildAndSave();

            var items = DomainRegistry.Repository.Find(new GetPlayerItemsThatCanBeSoulbound { OwnerId = owner.Id }).ToList();

            items.Count.Should().Be(1);
            items.ElementAt(0).Id.Should().Be(eligibleItem.Id);
            items.ElementAt(0).IsPermanent.Should().Be(true);
            items.ElementAt(0).FormerPlayer.Id.Should().Be(50);

        }
    }
}
