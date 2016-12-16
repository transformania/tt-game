using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain;
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
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteTome { TomeId = 7 };

            Repository.Execute(cmd);

            DataContext.AsQueryable<Tome>().Count().Should().Be(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_tome_id_is_invalid(int id)
        {
            var cmd = new DeleteTome { TomeId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("Tome Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_tome_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteTome { TomeId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage($"Tome with ID {id} was not found");
        }
    }
}
