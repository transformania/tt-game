using System;
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

            var query = new GetTome { TomeId = 7};

            var tome = DomainRegistry.Repository.FindSingle(query);

            tome.Id.Should().Equals(7);
            tome.Text.Should().BeEquivalentTo("First Tome");
            tome.BaseItem.Id.Should().Equals(195);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_tome_id_is_invalid(int id)
        {
            var query = new GetTome { TomeId = id };

            var action = new Action(() => { Repository.FindSingle(query); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Tome Id must be greater than 0");
        }

        [Test]
        public void Should_return_null_if_tome_is_not_found()
        {
            var query = new GetTome { TomeId = 1 };

            var tome = Repository.FindSingle(query);

            tome.Should().BeNull();
        }
    }
}