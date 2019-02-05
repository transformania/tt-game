using System;
using System.Linq;
using FluentAssertions;
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

            DomainRegistry.Repository.Execute(new SetFormSourceBecomesItemFK {FormSourceId = formSource.Id, ItemSourceId = itemSource.Id});

            var loadedform = DataContext.AsQueryable<FormSource>().First(t => t.Id == formSource.Id);

            loadedform.ItemSource.Id.Should().Be(itemSource.Id);
        }

        [Test]
        public void throws_exception_if_formSource_not_found()
        {
            Action action = () => Repository.Execute(new SetFormSourceBecomesItemFK { FormSourceId = 12345});
            action.Should().ThrowExactly<DomainException>().WithMessage("FormSource with Id '12345' could not be found");
        }

        [Test]
        public void throws_exception_if_itemSource_not_found()
        {
            var formSource = new FormSourceBuilder()
                 .With(f => f.Id, 33)
                 .BuildAndSave();

            Action action = () => Repository.Execute(new SetFormSourceBecomesItemFK { FormSourceId = formSource.Id, ItemSourceId = -999});
            action.Should().ThrowExactly<DomainException>().WithMessage("ItemSource with Id '-999' could not be found");
        }
    }
}
