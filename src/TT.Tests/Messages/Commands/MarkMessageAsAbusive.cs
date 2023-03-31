using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Messages.Commands;
using TT.Domain.Messages.DTOs;
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

            var guid1 = new Guid("0cd4537b-f6b3-4cab-ae18-83a880c6d070");

            var message = new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .With(m => m.Receiver, player)
               .With(m => m.ConversationId, guid1)
               .BuildAndSave();

            var cmd = new MarkAsAbusive{ MessageId = 61, OwnerId = player.Id, ConversationId = message.ConversationId };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var messageLoaded = DataContext.AsQueryable<Message>().First(m => m.Id == message.Id);
            Assert.That(messageLoaded.IsReportedAbusive, Is.True);
            Assert.That(messageLoaded.DoNotRecycleMe, Is.True);
        }

        [Test]
        public void should_throw_exception_when_message_not_found()
        {
            var cmd = new MarkAsAbusive { MessageId = 61, OwnerId = 999 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Message with ID 61 could not be found"));
        }

        [Test]
        public void should_throw_exception_when_message_not_owned_by_player()
        {

            new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .BuildAndSave();

            var cmd = new MarkAsAbusive { MessageId = 61, OwnerId = 123};

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Message 61 not owned by player 123"));
        }

        [Test]
        public void should_throw_exception_when_conversation_null()
        {
            var player = new PlayerBuilder()
               .With(p => p.Id, 3)
               .BuildAndSave();

            new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .With(m => m.Receiver, player)
               .BuildAndSave();

            var cmd = new MarkAsAbusive { MessageId = 61, OwnerId = player.Id, ConversationId = null };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Conversation ID cannot be null"));
        }

        [Test]
        public void should_mark_old_messages_do_not_recycle()
        {

            var guid1 = new Guid("0cd4537b-f6b3-4cab-ae18-83a880c6d070");

            var player = new PlayerBuilder()
            .With(p => p.Id, 3)
            .BuildAndSave();

            //Old message, not reported
            new MessageBuilder()
            .With(m => m.Id, 61)
            .With(m => m.ReadStatus, MessageStatics.Read)
            .With(m => m.Receiver, player)
            .With(m => m.ConversationId, guid1)
            .With(m => m.IsReportedAbusive, false)
            .With(m => m.DoNotRecycleMe, false)
            .BuildAndSave();

            //New message, will be reported
            new MessageBuilder()
            .With(m => m.Id, 62)
            .With(m => m.ReadStatus, MessageStatics.Read)
            .With(m => m.Receiver, player)
            .With(m => m.ConversationId, guid1)
            .With(m => m.IsReportedAbusive, false)
            .With(m => m.DoNotRecycleMe, false)
            .BuildAndSave();

            //Command should have no errors
            var cmd = new MarkAsAbusive { MessageId = 62, OwnerId = player.Id, ConversationId = guid1 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            //Command should return 2 items even though only 1 was reported, based on doNotRecycleMe value
            Assert.That(DataContext.AsQueryable<Message>().Where(m =>
                        m.ConversationId == guid1 &&
                        m.DoNotRecycleMe), Has.Exactly(2).Items);

        }

        [Test]
        public void should_mark_new_messages_do_not_recycle()
        {
            var guid1 = new Guid("0cd4537b-f6b3-4cab-ae18-83a880c6d070");

            var player = new PlayerBuilder()
            .With(p => p.Id, 3)
            .BuildAndSave();

            var player2 = new PlayerBuilder()
            .With(p => p.Id, 4)
            .BuildAndSave();

            var mDetail = new MessageDetail { ConversationId = guid1 };

            //Old message, reported
            new MessageBuilder()
            .With(m => m.Id, 61)
            .With(m => m.ReadStatus, MessageStatics.Read)
            .With(m => m.Receiver, player)
            .With(m => m.ConversationId, guid1)
            .With(m => m.IsReportedAbusive, true)
            .With(m => m.DoNotRecycleMe, true)
            .BuildAndSave();

            var cmd = new CreateMessage
            {
                SenderId = player.Id,
                ReceiverId = player2.Id,
                Text = "Message",
                ReplyingToThisMessage = mDetail
            };

            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);
            
            //Command should return 2 items even though only 1 was reported, based on doNotRecycleMe value
            Assert.That(DataContext.AsQueryable<Message>().Where(m =>
                        m.ConversationId == guid1 &&
                        m.DoNotRecycleMe), Has.Exactly(2).Items);
        }

    }
}
