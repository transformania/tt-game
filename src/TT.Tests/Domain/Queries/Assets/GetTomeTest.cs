using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Assets;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Assets
{
    [TestFixture]
    public class GetTomeTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_id()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetTome { TomeId = 7};

            var tome = DomainRegistry.Repository.FindSingle(cmd);

            tome.Id.Should().Equals(7);
            tome.Text.Should().BeEquivalentTo("First Tome");
            tome.BaseItem.Id.Should().Equals(195);

        }
    }
}