using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Messages.Commands;
using TT.Domain.Messages.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Messages;
using TT.Tests.Builders.Players;

namespace TT.Tests.Messages.Commands
{
    [TestFixture]
    public class MarkMessageAsAbusiveTests : TestBase
    {

        [Test]
        public void can_mark_message_as_abusive()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 3)
                .BuildAndSave();

            var message = new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .With(m => m.Receiver, player)
               .BuildAndSave();

            var cmd = new MarkAsAbusive{ MessageId = 61, OwnerId = player.Id};
            DomainRegistry.Repository.Execute(cmd);

            var messageLoaded = DataContext.AsQueryable<Message>().First(m => m.Id == message.Id);
            messageLoaded.IsReportedAbusive.Should().Be(true);
            messageLoaded.DoNotRecycleMe.Should().Be(true);
        }

        [Test]
        public void should_throw_exception_when_message_not_found()
        {
            var cmd = new MarkAsAbusive { MessageId = 61, OwnerId = 999 };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("Message with ID 61 could not be found");
        }

        [Test]
        public void should_throw_exception_when_message_not_owned_by_player()
        {

            new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .BuildAndSave();

            var cmd = new MarkAsAbusive { MessageId = 61, OwnerId = 123};

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("Message 61 not owned by player 123");
        }

    }
}
