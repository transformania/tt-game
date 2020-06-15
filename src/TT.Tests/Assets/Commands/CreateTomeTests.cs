using NUnit.Framework;
using TT.Tests.Builders.Item;
using TT.Domain.Assets.Commands;
using TT.Domain.Exceptions;

namespace TT.Tests.Assets.Commands
{
    [TestFixture]
    public class CreateTomeTests : TestBase
    {
        [Test]
        public void Should_create_new_tome()
        {
            var item = new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave();
            var cmd = new CreateTome { Text = "This is a tome.", BaseItemId = item.Id };

            Assert.That(Repository.Execute(cmd), Is.GreaterThan(0));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Should_throw_error_when_text_is_invalid(string text)
        {
            var cmd = new CreateTome { Text = text, BaseItemId = 1 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("No text was provided for the tome"));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new CreateTome { Text = "tome text", BaseItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Base item id must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_base_item_is_not_found()
        {
            const int id = 1;
            var cmd = new CreateTome { Text = "tome text", BaseItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Base item with Id {id} could not be found"));
        }
    }
}
