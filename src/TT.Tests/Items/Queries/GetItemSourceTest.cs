using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemSourceTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_id()
        {
            new ItemSourceBuilder().With(cr => cr.Id, 23)
                .With(cr => cr.FriendlyName, "Hello!")
                .With(cr => cr.IsUnique, true)
                .BuildAndSave();

            var cmd = new GetItemSource { ItemSourceId = 23};

            var item = DomainRegistry.Repository.FindSingle(cmd);

            item.Id.Should().Be(23);
            item.FriendlyName.Should().BeEquivalentTo("Hello!");
            item.IsUnique.Should().Be(true);
        }
    }
}