using System.Linq;
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
    public class MarkMessageAsReadTests : TestBase
    {

        [Test]
        public void can_mark_message_as_unread()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 3)
                .BuildAndSave();

            new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .With(m => m.Receiver, player)
               .BuildAndSave();

            var cmd = new MarkAsRead{ MessageId = 61, ReadStatus = MessageStatics.ReadAndMarkedAsUnread, OwnerId = player.Id};
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Message>().Where(p =>
                p.Id == 61 &&
                p.ReadStatus == MessageStatics.ReadAndMarkedAsUnread), Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_exception_when_message_not_found()
        {
            var cmd = new MarkAsRead { MessageId = 999, ReadStatus = MessageStatics.ReadAndMarkedAsUnread, OwnerId = 999 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Message with ID 999 could not be found"));
        }

        [Test]
        public void should_throw_exception_when_message_not_owned_by_player()
        {

            new MessageBuilder()
               .With(m => m.Id, 61)
               .With(m => m.ReadStatus, MessageStatics.Read)
               .BuildAndSave();

            var cmd = new MarkAsRead { MessageId = 61, ReadStatus = MessageStatics.ReadAndMarkedAsUnread, OwnerId = 999 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Message 61 not owned by player 999"));
        }

        [TestCase(-1)]
        [TestCase(3)]
        [Test]
        public void should_throw_exception_when_read_status_invalid(int readStatus)
        {
            var cmd = new MarkAsRead { MessageId = 61, ReadStatus = readStatus, OwnerId = 999 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"{readStatus} is not a valid read status."));
        }
    }
}
