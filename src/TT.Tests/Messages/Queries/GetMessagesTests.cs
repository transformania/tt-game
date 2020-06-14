using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Messages.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Messages;
using TT.Tests.Builders.Players;

namespace TT.Tests.Messages.Queries
{

    [TestFixture]
    public class GetMessagesTests : TestBase
    {

        [Test]
        public void should_get_count_of_player_unread_messages()
        {

            var receiver = new PlayerBuilder().With(p => p.Id, 25)
               .BuildAndSave();

            var otherPlayer = new PlayerBuilder().With(p => p.Id, 30)
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(m => m.Receiver, receiver)
                .With(m => m.ReadStatus, MessageStatics.Unread)
                .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 35)
                .With(m => m.Receiver, receiver)
                .With(m => m.ReadStatus, MessageStatics.Unread)
                .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 49)
                .With(m => m.Receiver, receiver)
                .With(m => m.ReadStatus, MessageStatics.Read)
                .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 51)
                .With(m => m.Receiver, otherPlayer)
                .With(m => m.ReadStatus, MessageStatics.Unread)
                .BuildAndSave();

            // don't count this; it is deleted without being read
            new MessageBuilder().With(m => m.Id, 100)
                .With(p => p.Sender, otherPlayer)
                .With(p => p.Receiver, receiver)
                .With(p => p.IsDeleted, true)
               .BuildAndSave();

            var cmd = new GetUnreadMessageCountByPlayer { OwnerId = receiver.Id };
            Assert.That(DomainRegistry.Repository.FindSingle(cmd), Is.EqualTo(2));
        }

        [Test]
        public void Should_fetch_message_by_id()
        {

           var msg = new MessageBuilder().With(m => m.Id, 23)
               .BuildAndSave();

            var cmd = new GetMessage { MessageId = 23, OwnerId = msg.Receiver.Id };

            var message = DomainRegistry.Repository.FindSingle(cmd);

            Assert.That(message.MessageId, Is.EqualTo(23));
            Assert.That(message.Sender.FirstName, Is.EqualTo("Sam"));
            Assert.That(message.Sender.LastName, Is.EqualTo("Houston"));
            Assert.That(message.Receiver.FirstName, Is.EqualTo("Lora"));
            Assert.That(message.Receiver.LastName, Is.EqualTo("Teetoo"));
        }

        [Test]
        public void Should_fetch_all_players_received_messages()
        {

            var sender = new PlayerBuilder().With(p => p.Id, 25)
                .With(p => p.FirstName, "Tom")
                .BuildAndSave();

            var receiver = new PlayerBuilder().With(p => p.Id, 78)
               .With(p => p.FirstName, "Carl").BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "hello")
                .With(p => p.Timestamp, DateTime.UtcNow.AddHours(-1))
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 24)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "world!")
                .With(p => p.Timestamp, DateTime.UtcNow.AddHours(-2))
               .BuildAndSave();

            var deletedMessage = new MessageBuilder().With(m => m.Id, 25)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.IsDeleted, true)
               .BuildAndSave();

            var cmd = new GetPlayerSentMessages
            {
                SenderId = sender.Id,
                Take = 50
            };

            var messages = DomainRegistry.Repository.Find(cmd).ToList();

            Assert.That(messages, Has.Exactly(2).Items);
            Assert.That(messages.First().MessageText, Is.EqualTo("hello"));
            Assert.That(messages.Last().MessageText, Is.EqualTo("world!"));

            Assert.That(messages.Select(i => i.MessageId), Has.No.Member(deletedMessage.Id));
        }

        [Test]
        public void Should_fetch_all_players_receiver_messages()
        {
            var sender = new PlayerBuilder().With(p => p.Id, 25)
                .With(p => p.FirstName, "Tom")
                .BuildAndSave();

            var receiver = new PlayerBuilder().With(p => p.Id, 78)
               .With(p => p.FirstName, "Carl").BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "hello")
                .With(p => p.Timestamp, DateTime.UtcNow.AddHours(-1))
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 24)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "world!")
                .With(p => p.Timestamp, DateTime.UtcNow.AddHours(-2))
               .BuildAndSave();

            var deletedMessage = new MessageBuilder().With(m => m.Id, 25)
               .With(p => p.Sender, sender)
               .With(p => p.Receiver, receiver)
               .With(p => p.IsDeleted, true)
              .BuildAndSave();

            var cmd = new GetPlayerReceivedMessages
            {
                ReceiverId = receiver.Id
            };

            var messages = DomainRegistry.Repository.Find(cmd).ToList();

            Assert.That(messages, Has.Exactly(2).Items);
            Assert.That(messages.First().MessageText, Is.EqualTo("hello"));
            Assert.That(messages.Last().MessageText, Is.EqualTo("world!"));

            Assert.That(messages.Select(i => i.MessageId), Has.No.Member(deletedMessage.Id));
        }

        [Test]
        public void return_true_if_player_owns_test()
        {

            var receiver = new PlayerBuilder()
               .With(p => p.Id, 78)
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Receiver, receiver)
               .BuildAndSave();

            var cmd = new PlayerOwnsMessage
            {
                OwnerId = receiver.Id,
                MessageId = 23
            };

            Assert.That(DomainRegistry.Repository.FindSingle(cmd), Is.True);
        }
    }
}
