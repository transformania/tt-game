using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemsOwnedByPsychopathTests : TestBase
    {
        [Test]
        public void get_all_items_owned_by_psychopath()
        {

            var player = new PlayerBuilder()
               .With(p => p.Id, 9)
               .With(p => p.FirstName, "Psycho")
               .With(p => p.LastName, "Path")
               .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 21)
                .With(cr => cr.Owner, null)
                .With(cr => cr.FormerPlayer, new PlayerBuilder()
                    .With(i => i.Id, 111)
                    .With(i => i.FirstName, "Nobody")
                    .With(i => i.LastName, "Noone")
                    .With(i => i.Nickname, "Zero")
                    .With(i => i.DonatorLevel, 1)
                    .BuildAndSave())
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 35)
                    .With(i => i.ItemType, "hat")
                    .With(i => i.FriendlyName, "Halo")
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, player)
                .With(cr => cr.FormerPlayer, new PlayerBuilder()
                    .With(i => i.Id, 112)
                    .With(i => i.FirstName, "Hollow")
                    .With(i => i.LastName, "Harry")
                    .With(i => i.Nickname, "Empty")
                    .With(i => i.DonatorLevel, 0)
                    .BuildAndSave())
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 37)
                    .With(i => i.ItemType, "hat")
                    .With(i => i.FriendlyName, "Horns")
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 100)
                .With(cr => cr.Owner, player)
                .With(cr => cr.FormerPlayer, new PlayerBuilder()
                    .With(i => i.Id, 113)
                    .With(i => i.FirstName, "Harpreet")
                    .With(i => i.LastName, "Halfway")
                    .With(i => i.Nickname, "Middle")
                    .With(i => i.DonatorLevel, 2)
                    .BuildAndSave())
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 39)
                    .With(i => i.ItemType, "shoes")
                    .With(i => i.FriendlyName, "Sneakers")
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemsOwnedByPsychopath { OwnerId = 9 };

            var items = DomainRegistry.Repository.Find(cmd).ToList();

            Assert.That(items, Has.Exactly(2).Items);

            Assert.That(items.ElementAt(0).Id, Is.EqualTo(99));
            Assert.That(items.ElementAt(0).FormerPlayer.Id, Is.EqualTo(112));
            Assert.That(items.ElementAt(0).FormerPlayer.FirstName, Is.EqualTo("Hollow"));
            Assert.That(items.ElementAt(0).FormerPlayer.LastName, Is.EqualTo("Harry"));
            Assert.That(items.ElementAt(0).FormerPlayer.Nickname, Is.EqualTo("Empty"));
            Assert.That(items.ElementAt(0).FormerPlayer.DonatorLevel, Is.EqualTo(0));
            Assert.That(items.ElementAt(0).FormerPlayer.FullName, Is.EqualTo("Hollow Harry"));
            Assert.That(items.ElementAt(0).ItemSource.ItemType, Is.EqualTo("hat"));
            Assert.That(items.ElementAt(0).ItemSource.FriendlyName, Is.EqualTo("Horns"));

            Assert.That(items.ElementAt(1).Id, Is.EqualTo(100));
            Assert.That(items.ElementAt(1).FormerPlayer.Id, Is.EqualTo(113));
            Assert.That(items.ElementAt(1).FormerPlayer.FirstName, Is.EqualTo("Harpreet"));
            Assert.That(items.ElementAt(1).FormerPlayer.LastName, Is.EqualTo("Halfway"));
            Assert.That(items.ElementAt(1).FormerPlayer.Nickname, Is.EqualTo("Middle"));
            Assert.That(items.ElementAt(1).FormerPlayer.DonatorLevel, Is.EqualTo(2));
            Assert.That(items.ElementAt(1).FormerPlayer.FullName, Is.EqualTo("Harpreet 'Middle' Halfway"));
            Assert.That(items.ElementAt(1).ItemSource.ItemType, Is.EqualTo("shoes"));
            Assert.That(items.ElementAt(1).ItemSource.FriendlyName, Is.EqualTo("Sneakers"));
        }

    }
}
