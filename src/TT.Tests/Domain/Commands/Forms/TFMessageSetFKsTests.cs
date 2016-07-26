using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Forms;
using TT.Domain.Entities.Forms;
using TT.Tests.Builders.Form;

namespace TT.Tests.Domain.Commands.Forms
{
    [TestFixture]
    public class TFMessageSetFKsTests : TestBase
    {
        [Test]
        public void can_set_form_source_as_FK()
        {
            var tfMessage = new TFMessageBuilder()
                .With(i => i.Id, 50)
                .BuildAndSave();

            var form = new FormSourceBuilder()
                .With(f => f.Id, 33)
                .With(f => f.dbName, "bobform")
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new SetTFMessageFKs {TFMessageId = 50, FormSource = form.dbName});

            form = DataContext.AsQueryable<FormSource>().First(t => t.Id == form.Id);

            form.TfMessage.Id.Should().Be(50);
        }

        [Test]
        public void throws_exception_if_tfmessage_not_found()
        {
            Action action = () => Repository.Execute(new SetTFMessageFKs {TFMessageId = 12345});
            action.ShouldThrowExactly<DomainException>().WithMessage("TFMessage with Id 12345 could not be found");
        }

        [Test]
        public void throws_exception_if_formsource_not_found()
        {

            new TFMessageBuilder()
                .With(i => i.Id, 50)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SetTFMessageFKs { TFMessageId = 50, FormSource = "fake"});
            action.ShouldThrowExactly<DomainException>().WithMessage("FormSource with name 'fake' could not be found");
        }
    }
}
