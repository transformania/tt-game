using NUnit.Framework;
using System.Linq;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Assets;
using TT.Domain.Assets.Commands;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Tests.Assets.Commands
{
    [TestFixture]
    public class UpdateTomeTests : TestBase
    {
        [Test]
        public void Should_update_existing_tome()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            new ItemSourceBuilder().With(cr => cr.Id, 200).BuildAndSave();

            var cmdEdit = new UpdateTome { TomeId = 7, Text = "new text123", BaseItemId = 200 };

            Assert.That(() => Repository.Execute(cmdEdit), Throws.Nothing);

            var editedTome = DataContext.AsQueryable<Tome>().FirstOrDefault(cr => cr.Id == 7);

            Assert.That(editedTome, Is.Not.Null);
            Assert.That(editedTome.Id, Is.EqualTo(7));
            Assert.That(editedTome.Text, Is.EqualTo("new text123"));
            Assert.That(editedTome.BaseItem.Id, Is.EqualTo(200));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Should_throw_error_when_text_is_invalid(string text)
        {
            var cmd = new UpdateTome { Text = text, TomeId = 1, BaseItemId = 1 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("No text was provided for the tome"));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_tome_id_is_invalid(int id)
        {
            var cmd = new UpdateTome { Text = "tome text", TomeId = id, BaseItemId = 1 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Tome Id must be greater than 0"));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new UpdateTome { Text = "tome text", TomeId = 1, BaseItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Base item id must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_tome_is_not_found()
        {
            const int id = 1;
            var cmd = new UpdateTome { Text = "tome text", TomeId = id, BaseItemId = 1 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Tome with ID {id} was not found"));
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

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Base item with ID {baseItemId} was not found"));
        }
    }
}
