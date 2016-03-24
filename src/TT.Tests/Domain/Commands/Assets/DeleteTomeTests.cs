using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Assets;
using TT.Domain.Commands.Assets;

namespace TT.Tests.Domain.Commands.Assets
{
    [TestFixture]
    public class DeleteTomeTests : TestBase
    {
        [Test]
        public void Should_delete_tome()
        {

            var builder = new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteTome(7);

            Repository.Execute(cmd);

            DataContext.AsQueryable<Tome>().Count().Should().Be(0);

        }
    }
}
