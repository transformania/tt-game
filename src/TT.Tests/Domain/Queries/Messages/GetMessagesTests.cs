using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Messages;
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
        [Ignore("TODO")]
        public void Should_fetch_all_players_received_messages()
        {
            
        }

        [Test]
        [Ignore("TODO")]
        public void Should_fetch_all_players_sent_messages()
        {

        }


    }
}
