﻿using System.Linq;
using NUnit.Framework;
using TT.Domain.Chat.Commands;
using TT.Domain.Chat.Entities;

namespace TT.Tests.Chat.Commands
{
    [TestFixture]
    public class CreateChatLogTests : TestBase
    {
        [Test]
        public void Should_create_new_chat_log()
        {
            Repository.Execute(new CreateChatLog { Color = "pink", Message = "hello chat", Name = "Bob Smith", PortraitUrl = "face.jpg", Room = "global" });

            var log = DataContext.AsQueryable<ChatLog>().FirstOrDefault();
            Assert.That(log, Is.Not.Null);
            Assert.That(log.Message, Is.EqualTo("hello chat"));
            Assert.That(log.Color, Is.EqualTo("pink"));
            Assert.That(log.PortraitUrl, Is.EqualTo("face.jpg"));
            Assert.That(log.Room, Is.EqualTo("global"));
            Assert.That(log.Name, Is.EqualTo("Bob Smith"));
        }
    }
}
