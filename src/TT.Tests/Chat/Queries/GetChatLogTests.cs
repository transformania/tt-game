using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Chat.Queries;
using TT.Tests.Builders.Chat;

namespace TT.Tests.Chat.Queries
{
    [TestFixture]
    public class GetChatLogTests : TestBase
    {
        [Test]
        public void Should_get_chat_logs_in_room()
        {

            var recent_log = new ChatLogBuilder()
                .With(l => l.Id, 1)
                .With(l => l.Message, "Hello chat from 15 minutes ago")
                .With(l => l.Timestamp, DateTime.UtcNow.AddMinutes(-15))
                .With(l => l.Room, "global")
                .BuildAndSave();

            new ChatLogBuilder()
                .With(l => l.Id, 2)
                .With(l => l.Message, "This is an old message")
                .With(l => l.Timestamp, DateTime.UtcNow.AddHours(-3))
                .With(l => l.Room, "global")
                .BuildAndSave();

            new ChatLogBuilder()
                .With(l => l.Id, 3)
                .With(l => l.Message, "This is an old message")
                .With(l => l.Timestamp, DateTime.UtcNow.AddHours(-3))
                .With(l => l.Room, "elsewhere")
                .BuildAndSave();

            var less_recent_log = new ChatLogBuilder()
                .With(l => l.Id, 4)
                .With(l => l.Message, "Hello chat from 45 minutes ago")
                .With(l => l.Timestamp, DateTime.UtcNow.AddMinutes(-45))
                .With(l => l.Room, "global")
                .BuildAndSave();

            var logs = DomainRegistry.Repository.Find(new GetChatLogs {Room = "global", Filter = "lasth"}).ToArray();
            logs.Count().Should().Be(2);

            logs[0].Id.Should().Be(less_recent_log.Id);
            logs[0].Message.Should().Be("Hello chat from 45 minutes ago");

            logs[1].Id.Should().Be(recent_log.Id);
        }
    }
}
