using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Chat;
using TT.Domain.Entities.Chat;
using TT.Tests.Builders.Chat;

namespace TT.Tests.Domain.Commands.Chat
{
    [TestFixture]
    public class CreateChatRoomTests : DomainTestBase
    {
        [Test]
        public void Should_create_new_chat_room()
        {
            var creatorId = Guid.NewGuid().ToString();
            
            var cmd = new CreateChatRoom("Test_Room", creatorId);

            Repository.Execute(cmd);

            DataContext.AsQueryable<ChatRoom>().Count(
                cr => cr.Name == "Test_Room" && 
                cr.Creator == creatorId && 
                cr.CreatedAt.Value.Date == DateTime.UtcNow.Date)
            .Should().Be(1);
        }

        [Test]
        public void Should_not_allow_rooms_of_the_same_name_to_exist()
        {
            var existingName = "Test_Room";

            new ChatRoomBuilder().With(cr => cr.Name, existingName).BuildAndSave();

            var cmd = new CreateChatRoom(existingName, Guid.NewGuid().ToString());

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Chat room '{0}' already exists", existingName));
        }

        [TestCase("Test!")]
        [TestCase("Test'")]
        [TestCase("Test<")]
        [TestCase("Test*")]
        [TestCase("Test?")]
        [TestCase("Test|")]
        [TestCase("Test%")]
        public void Should_not_allow_special_characters_in_room_name(string roomName)
        {
            var cmd = new CreateChatRoom(roomName, Guid.NewGuid().ToString());

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(
                string.Format("Chat room '{0}' contains unsupported characters, only alphanumeric names with _ or - are allowed", roomName));
        }

        [TestCase("Test_")]
        [TestCase("_Test")]
        [TestCase("_")]
        [TestCase("Test_Test")]
        [TestCase("_test_")]
        [TestCase("_0_")]
        [TestCase("-Test-")]
        [TestCase("-_-")]
        [TestCase("0-0")]
        public void Should_allow_underscores_or_hypens_in_room_names(string roomName)
        {
            Repository.Execute(new CreateChatRoom(roomName, Guid.NewGuid().ToString()));

            DataContext.AsQueryable<ChatRoom>().Any(cr => cr.Name == roomName).Should().BeTrue();
        }
    }
}