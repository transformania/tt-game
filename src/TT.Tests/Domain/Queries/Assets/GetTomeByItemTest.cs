using System;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Assets.Queries;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Assets
{
    [TestFixture]
    public class GetTomeByItemTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_item_id()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var query = new GetTomeByItem { ItemSourceId = 195};

            var tome = Repository.FindSingle(query);

            tome.Id.Should().Be(7);
            tome.Text.Should().BeEquivalentTo("First Tome");
            tome.BaseItem.Id.Should().Be(195);
        }

        [Test]
        public void Should_return_null_if_tome_is_not_found()
        {
            var query = new GetTomeByItem { ItemSourceId = 1 };

            var tome = Repository.FindSingle(query);

            tome.Should().BeNull();
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_not_allow_id_to_be_invalid(int id)
        {
            var query = new GetTomeByItem { ItemSourceId = id };

            var action = new Action(() => { Repository.FindSingle(query); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ItemSourceID must be a number greater than 0");
        }
    }
}