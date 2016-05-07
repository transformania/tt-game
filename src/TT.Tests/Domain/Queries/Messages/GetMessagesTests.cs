using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Messages;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Messages;

namespace TT.Tests.Domain.Queries.Messages
{

    [TestFixture]
    public class GetMessagesTests : TestBase
    {
        [Test]
        public void Should_fetch_message_by_id()
        {

            new MessageBuilder().With(m => m.Id, 23)
               .BuildAndSave();

            var cmd = new GetMessage { MessageId = 23 };

            var message = DomainRegistry.Repository.FindSingle(cmd);

            message.Id.Should().Equals(23);
            message.Sender.FirstName.Should().BeEquivalentTo("Sam");
            message.Sender.LastName.Should().BeEquivalentTo("Houston");
            message.Receiver.FirstName.Should().BeEquivalentTo("Lora");
            message.Receiver.LastName.Should().BeEquivalentTo("Teetoo");

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
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "world!")
               .BuildAndSave();

            var cmd = new GetPlayerSentMessages
            {
                SenderId = sender.Id
            };

            var messages = DomainRegistry.Repository.Find(cmd);

            messages.Should().HaveCount(2);
            messages.First().MessageText.Should().BeEquivalentTo("hello");
            messages.Last().MessageText.Should().BeEquivalentTo("world!");

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
               .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 23)
                .With(p => p.Sender, sender)
                .With(p => p.Receiver, receiver)
                .With(p => p.MessageText, "world!")
               .BuildAndSave();

            var cmd = new GetPlayerReceivedMessages
            {
                ReceiverId = receiver.Id
            };

            var messages = DomainRegistry.Repository.Find(cmd);

            messages.Should().HaveCount(2);
            messages.First().MessageText.Should().BeEquivalentTo("hello");
            messages.Last().MessageText.Should().BeEquivalentTo("world!");
        }


    }
}
