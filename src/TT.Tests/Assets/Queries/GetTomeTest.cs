using NUnit.Framework;
using TT.Domain;
using TT.Domain.Assets.Queries;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Assets.Queries
{
    [TestFixture]
    public class GetTomeTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_id()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var query = new GetTome { TomeId = 7};

            var tome = DomainRegistry.Repository.FindSingle(query);

            Assert.That(tome.Id, Is.EqualTo(7));
            Assert.That(tome.Text, Is.EqualTo("First Tome"));
            Assert.That(tome.BaseItem.Id, Is.EqualTo(195));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_tome_id_is_invalid(int id)
        {
            var query = new GetTome { TomeId = id };

            Assert.That(() => Repository.FindSingle(query),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Tome Id must be greater than 0"));
        }

        [Test]
        public void Should_return_null_if_tome_is_not_found()
        {
            var query = new GetTome { TomeId = 1 };

            Assert.That(Repository.FindSingle(query), Is.Null);
        }
    }
}