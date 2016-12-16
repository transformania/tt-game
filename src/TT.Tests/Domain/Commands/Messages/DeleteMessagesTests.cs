using System;
using NUnit.Framework;
using System.Linq;
using FluentAssertions;
using TT.Domain;
using TT.Domain.Commands.Messages;
using TT.Domain.Entities.Messages;
using TT.Tests.Builders.Messages;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Messages
{
    [TestFixture]
    public class DeleteMessagesTests : TestBase
    {

        [Test]
        public void Should_delete_message()
        {
            new MessageBuilder()
                .With(m => m.Id, 61)
                .With(m => m.Receiver, new PlayerBuilder()
                    .With(p => p.Id, 3).BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteMessage { MessageId = 61, OwnerId = 3 };
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
            action.ShouldThrowExactly<DomainException>().WithMessage($"Message with ID {999} was not found");
        }


        [Test]
        public void Should_throw_error_when_owner_not_set()
        {
            new MessageBuilder()
                .With(m => m.Id, 61)
                .With(m => m.Receiver, new PlayerBuilder()
                    .With(p => p.Id, 3).BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteMessage { MessageId = 61 };
            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("OwnerId is required to delete a message");
        }

        [Test]
        public void Should_throw_error_when_message_not_owned_by_player_is_not_found()
        {

            new MessageBuilder()
                .With(m => m.Id, 61)
                .With(m => m.Receiver, new PlayerBuilder()
                    .With(p => p.Id, 3).BuildAndSave())
                .BuildAndSave();

            var cmd = new DeleteMessage { MessageId = 61, OwnerId = 4};

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("Message 61 not owned by player 4");
        }

        [Test]
        public void Should_delete_all_message_owned_by_player()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 3)
                .BuildAndSave();

            new MessageBuilder()
                .With(m => m.Id, 61)
                .With(m => m.Receiver, player)
                .BuildAndSave();

            new MessageBuilder()
                .With(m => m.Id, 79)
                .With(m => m.Receiver, player)
                .BuildAndSave();

            var cmd = new DeleteAllMessagesOwnedByPlayer { OwnerId = 3 };
            Repository.Execute(cmd);

            DataContext.AsQueryable<Message>().Count().Should().Be(0);
        }

        [Test]
        public void Should_delete_expired_messages_owned_by_player()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 3)
                .BuildAndSave();

            // eligible for deletion
            new MessageBuilder()
                .With(m => m.Id, 61)
                .With(m => m.Receiver, player)
                .With(m => m.Timestamp, DateTime.UtcNow.AddDays(-4))
                .BuildAndSave();

            // not eligible for deletion due to being too new
            new MessageBuilder()
                .With(m => m.Id, 79)
                .With(m => m.Receiver, player)
                .With(m => m.Timestamp, DateTime.UtcNow.AddDays(-1))
                .BuildAndSave();

            // not eligible for deletion due to being protected
            new MessageBuilder()
                .With(m => m.Id, 95)
                .With(m => m.Receiver, player)
                .With(m => m.DoNotRecycleMe, true)
                .With(m => m.Timestamp, DateTime.UtcNow.AddDays(-7))
                .BuildAndSave();

            var cmd = new DeletePlayerExpiredMessages() { OwnerId = 3 };
            Repository.Execute(cmd);

            DataContext.AsQueryable<Message>().Count().Should().Be(2);
        }

    }
}
