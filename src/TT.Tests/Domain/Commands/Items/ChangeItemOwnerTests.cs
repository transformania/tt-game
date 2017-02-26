using NUnit.Framework;
using TT.Tests.Builders.Item;
using System.Linq;
using FluentAssertions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Items
{
    [TestFixture]
    public class ChangeItemOwnerTests : TestBase
    {
        private Player bob;
        private Player sam;
        private Item item;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            bob = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.FirstName, "Bob")
                .BuildAndSave();

            sam = new PlayerBuilder()
                .With(p => p.Id, 68)
                .With(p => p.FirstName, "Sam")
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, bob)
                .With(i => i.Id, 87)
                .BuildAndSave();
        }

        [Test]
        public void can_change_item_owner()
        {

            var cmd = new ChangeItemOwner
            {
                ItemId = item.Id,
                OwnerId = sam.Id
            };

            Repository.Execute(cmd);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == item.Id);

            editedItem.Owner.FirstName.Should().Be("Sam");

        }

        [Test]
        [Ignore("TODO")]
        public void throw_exception_if_item_not_found()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void throw_exception_if_player_not_found()
        {

        }

    }
}
