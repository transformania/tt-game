using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain.Commands.Assets;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Assets;
using TT.Domain;

namespace TT.Tests.Domain.Commands.Assets
{
    [TestFixture]
    public class UpdateTomeTests : TestBase
    {
        [Test]
        public void Should_update_existing_tome()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(cr => cr.Id, 200).BuildAndSave();

            var cmdEdit = new UpdateTome { TomeId = 7, Text = "new text123", BaseItemId = 200 };

            Repository.Execute(cmdEdit);

            var editedTome = DataContext.AsQueryable<Tome>().FirstOrDefault(cr => cr.Id == 7);
            
            editedTome.Id.Should().Be(7);
            editedTome.Text.Should().Be("new text123");
            editedTome.BaseItem.Id.Should().Be(200);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Should_throw_error_when_text_is_invalid(string text)
        {
            var cmd = new UpdateTome { Text = text, TomeId = 1, BaseItemId = 1 };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("No text was provided for the tome");
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_tome_id_is_invalid(int id)
        {
            var cmd = new UpdateTome { Text = "tome text", TomeId = id, BaseItemId = 1 };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Tome Id must be greater than 0");
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new UpdateTome { Text = "tome text", TomeId = 1, BaseItemId = id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Base item Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_tome_is_not_found()
        {
            const int id = 1;
            var cmd = new UpdateTome { Text = "tome text", TomeId = id, BaseItemId = 1 };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Tome with ID {0} was not found", id));
        }

        [Test]
        public void Should_throw_error_when_new_base_item_is_not_found()
        {
            const int tomeId = 1;
            const int baseItemId = 2;

            new TomeBuilder().With(cr => cr.Id, tomeId)
               .With(cr => cr.Text, "First Tome")
               .BuildAndSave();

            var cmd = new UpdateTome { Text = "tome text", TomeId = tomeId, BaseItemId = baseItemId };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Base item with ID {0} was not found", baseItemId));
        }
    }
}
