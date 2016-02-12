
using NUnit.Framework;
using TT.Domain;
using FluentAssertions;
using System.Linq;
using TT.Domain.Commands.Assets;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Chat;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Commands.Assets
{
    [TestFixture]
    public class CreateTomeTests : TestBase
    {
        [Test]
        public void Should_create_new_tome()
        {

            var item = new ItemBuilder().BuildAndSave();
            var cmd = new CreateTome { Text = "This is a tome.", BaseItemId = item.Id };

            var tome = Repository.Execute(cmd);

            DataContext.AsQueryable<Tome>().Count(cr =>
                cr.Id == tome.Id &&
                cr.Text == "This is a tome." &&
                cr.BaseItem.Id == item.Id)
            .Should().Be(1);

        }
    }
}
