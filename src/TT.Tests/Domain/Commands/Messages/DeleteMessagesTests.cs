using System;
using NUnit.Framework;
using System.Linq;
using FluentAssertions;
using TT.Domain;
using TT.Domain.Commands.Assets;
using TT.Domain.Commands.Messages;
using TT.Domain.Entities.Messages;
using TT.Tests.Builders.Messages;

namespace TT.Tests.Domain.Commands.Messages
{
    [TestFixture]
    public class DeleteMessagesTests : TestBase
    {

        [Test]
        public void Should_delete_message()
        {
            new MessageBuilder().With(cr => cr.Id, 61)
                .BuildAndSave();

            var cmd = new DeleteMessage { MessageId = 61 };
            Repository.Execute(cmd);

            DataContext.AsQueryable<Message>().Count().Should().Be(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_message_id_is_invalid(int id)
        {
            new MessageBuilder().With(cr => cr.Id, id)
                .BuildAndSave();
            var cmd = new DeleteMessage { MessageId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("Message Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_message_is_not_found()
        {
            var cmd = new DeleteMessage { MessageId = 999 };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Message with ID {0} was not found", 999));
        }

    }
}
