using System;
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
    public class GetMessagesInConversationTests : TestBase
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

            new MessageBuilder().With(m => m.Id, 3)
                    .With(m => m.Receiver, receiver)
                    .With(m => m.ConversationId, guid2)
                    .BuildAndSave();

            new MessageBuilder().With(m => m.Id, 4)
                    .With(m => m.Receiver, otherPlayer)
                    .With(m => m.ConversationId, guid1)
                    .BuildAndSave();

            var cmd = new GetMessagesInConversation { conversationId = guid1};
            var messages = DomainRegistry.Repository.Find(cmd);

            var ids = messages.Select(m => m.Id);

            ids.Should().Contain(1);
            ids.Should().Contain(2);
            ids.Should().Contain(4);
            ids.Should().NotContain(3);
        }

        [Test]
        public void should_throw_exception_if_conversationId_is_null()
        {
            var cmd = new GetMessagesInConversation { conversationId = null };
            var action = new Action(() => { Repository.Find(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ConversationId cannot be null");
        }

        
    }
}
