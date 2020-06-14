using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Commands;
using TT.Domain.Forms.Entities;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Item;

namespace TT.Tests.Forms.Commands
{
    [TestFixture]
    public class SetFormSourceBecomesItemFKTests : TestBase
    {
        [Test]
        public void can_set_item_source_as_FK()
        {
            var itemSource = new ItemSourceBuilder()
                .With(i => i.Id, 50)
                .BuildAndSave();

            var formSource = new FormSourceBuilder()
                .With(f => f.Id, 33)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new SetFormSourceBecomesItemFK
                    {FormSourceId = formSource.Id, ItemSourceId = itemSource.Id}), Throws.Nothing);
            Assert.That(DataContext.AsQueryable<FormSource>().First(t => t.Id == formSource.Id).ItemSource.Id,
                Is.EqualTo(itemSource.Id));
        }

        [Test]
        public void throws_exception_if_formSource_not_found()
        {
            Assert.That(() =>
                    Repository.Execute(new SetFormSourceBecomesItemFK {FormSourceId = 12345}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("FormSource with Id '12345' could not be found"));
        }

        [Test]
        public void throws_exception_if_itemSource_not_found()
        {
            var formSource = new FormSourceBuilder()
                 .With(f => f.Id, 33)
                 .BuildAndSave();

            Assert.That(
                () => Repository.Execute(new SetFormSourceBecomesItemFK
                    {FormSourceId = formSource.Id, ItemSourceId = -999}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemSource with Id '-999' could not be found"));
        }
    }
}
