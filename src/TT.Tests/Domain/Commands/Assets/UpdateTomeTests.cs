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

            var builder = new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var item2Builder = new ItemBuilder().With(cr => cr.Id, 200).BuildAndSave();

            var cmdEdit = new UpdateTome { Id = 7, Text = "new text123", BaseItemId = 200 };

            Repository.Execute(cmdEdit);

            var editedTome = DataContext.AsQueryable<Tome>().FirstOrDefault(cr =>
                cr.Id == 7);
            
            editedTome.Id.Should().Be(7);
            editedTome.Text.Should().Be("new text123");
            editedTome.BaseItem.Id.Should().Be(200);

        }

        [Test]
        public void Should_throw_error_when_text_is_empty()
        {
            var builder = new TomeBuilder().With(cr => cr.Id, 7)
               .With(cr => cr.Text, "First Tome")
               .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
               .BuildAndSave();

            var cmd = new UpdateTome { Text = "", BaseItemId = 7 };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("No text"));
        }

        [Test]
        public void Should_throw_error_when_base_item_id_is_0()
        {

            var builder = new TomeBuilder().With(cr => cr.Id, 7)
               .With(cr => cr.Text, "First Tome")
               .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
               .BuildAndSave();

            var cmd = new UpdateTome { Text = "tome text", BaseItemId = 0 };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("No base item was provided"));
        }
    }
}
