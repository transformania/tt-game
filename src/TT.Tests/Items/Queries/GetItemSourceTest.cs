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

            Assert.That(item.Id, Is.EqualTo(23));
            Assert.That(item.FriendlyName, Is.EqualTo("Hello!"));
            Assert.That(item.IsUnique, Is.True);
        }
    }
}