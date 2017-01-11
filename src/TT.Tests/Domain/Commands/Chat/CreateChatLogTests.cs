using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Commands.Chat;
using TT.Domain.Entities.Chat;

namespace TT.Tests.Domain.Commands.Chat
{
    [TestFixture]
    public class CreateChatLogTests : TestBase
    {
        [Test]
        public void Should_create_new_chat_log()
        {
            Repository.Execute(new CreateChatLog { Color = "pink", Message = "hello chat", Name = "Bob Smith", PortraitUrl = "face.jpg", Room = "global" });

            var log = DataContext.AsQueryable<ChatLog>().First();
            log.Message.Should().Be("hello chat");
            log.Color.Should().Be("pink");
            log.PortraitUrl.Should().Be("face.jpg");
            log.Room.Should().Be("global");
            log.Name.Should().Be("Bob Smith");
        }
    }
}
