using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Messages.Queries;
using TT.Tests.Builders.Messages;
using TT.Tests.Builders.Players;

namespace TT.Tests.Messages.Queries
{
    [TestFixture]
    public class GetMessagesInConversationModeratorTests : TestBase
    {

        [Test]
        public void should_get_messages_in_conversation()
        {
            var receiver = new PlayerBuilder().With(p => p.Id, 25)
                  .BuildAndSave();

            var otherPlayer = new PlayerBuilder().With(p => p.Id, 30)
               .BuildAndSave();

            var guid1 = new Guid("0cd4537b-f6b3-4cab-ae18-83a880c6d070");
            var guid2 = new Guid("11309252-9094-45a2-945a-85cd882d4a2e");

            new MessageBuilder().With(m => m.Id, 1)
                    .With(m => m.Receiver, receiver)
                    .With(m => m.ConversationId, guid1)
                    .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 2)
                    .With(m => m.Receiver, receiver)
                    .With(m => m.ConversationId, guid1)
                    .BuildAndSave();

            var otherConversationMessage = new MessageBuilder().With(m => m.Id, 3)
                    .With(m => m.Receiver, receiver)
                    .With(m => m.ConversationId, guid2)
                    .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 4)
                    .With(m => m.Receiver, otherPlayer)
                    .With(m => m.ConversationId, guid1)
                    .BuildAndSave();

            var deletedMessage = new MessageBuilder().With(m => m.Id, 5)
                    .With(m => m.Receiver, otherPlayer)
                    .With(m => m.ConversationId, guid1)
                    .With(m => m.IsDeleted, true)
                    .BuildAndSave();

            var cmd = new GetMessagesInConversationModerator { conversationId = guid1};
            var messages = DomainRegistry.Repository.Find(cmd);

            var ids = messages.Select(m => m.MessageId).ToList();

            Assert.That(ids, Has.Member(1));
            Assert.That(ids, Has.Member(2));
            Assert.That(ids, Has.Member(4));
            Assert.That(ids, Has.Member(deletedMessage.Id));
            Assert.That(ids, Has.No.Member(otherConversationMessage.Id));
        }

        [Test]
        public void should_throw_exception_if_conversationId_is_null()
        {
            var cmd = new GetMessagesInConversationModerator { conversationId = null };
            Assert.That(() => Repository.Find(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ConversationId cannot be null"));
        }

        [Test]
        public void return_list_if_player_owns()
        {
            //Player and a message they have received
            var receiver = new PlayerBuilder()
               .With(p => p.Id, 78)
               .BuildAndSave();

            var message = new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Receiver, receiver)
                .BuildAndSave();

            var cmd = new GetMessageModerator { MessageId = message.Id, OwnerId = message.Receiver.Id };
            var messageList = DomainRegistry.Repository.FindSingle(cmd);

            Assert.That(messageList, Is.Not.Null);
        }

        [Test]
        public void should_fail_by_incorrect_ownership()
        {

            //Player and a message they have received
            var receiver = new PlayerBuilder()
               .With(p => p.Id, 78)
               .BuildAndSave();

            var message = new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Receiver, receiver)
                .BuildAndSave();

            //Second player and message they have received
            var otherReceiver = new PlayerBuilder()
                .With(p => p.Id, 80)
                .BuildAndSave();

            var otherMessage = new MessageBuilder().With(m => m.Id, 25)
                .With(p => p.Receiver, otherReceiver)
                .BuildAndSave();

            //Attempt GetMessageModerator with a mismatching message and receiver
            var cmd = new GetMessageModerator { MessageId = otherMessage.Id, OwnerId = message.Receiver.Id };

            //Assert that the first player did not receive the second message
            Assert.That(() => DomainRegistry.Repository.FindSingle(cmd), Throws.TypeOf<DomainException>());

        }

        [Test]
        public void should_fail_by_messageid()
        {

            //Player and an invalid message they have received
            var receiver = new PlayerBuilder()
               .With(p => p.Id, 82)
               .BuildAndSave();

            var message = new MessageBuilder().With(m => m.Id, -1)
                .With(p => p.Receiver, receiver)
                .BuildAndSave();

            var cmd = new GetMessageModerator { MessageId = message.Id, OwnerId = message.Receiver.Id };

            //Assert that the error was thrown due to an invalid message ID
            Exception ex = Assert.Throws<DomainException>(() => DomainRegistry.Repository.FindSingle(cmd));
            Assert.That(ex.Message, Is.EqualTo("MessageId must be greater than 0"));

        }

        [Test]
        public void should_fail_by_ownerid()
        {

            //Make an invalid owner and a new message
            var receiver = new PlayerBuilder()
                .With(p => p.Id, -1)
                .BuildAndSave();

            var message = new MessageBuilder().With(m => m.Id, 30)
                .With(p => p.Receiver, receiver)
                .BuildAndSave();

            var cmd = new GetMessageModerator { MessageId = message.Id, OwnerId = message.Receiver.Id };

            //Assert that the error was thrown due to an invalid owner ID
            Exception ex = Assert.Throws<DomainException>(() => DomainRegistry.Repository.FindSingle(cmd));
            Assert.That(ex.Message, Is.EqualTo("OwnerId must be greater than 0"));

        }

    }
}
