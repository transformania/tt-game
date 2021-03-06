﻿using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Chat.Commands;
using TT.Domain.Chat.Entities;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Chat;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Chat.Commands
{
    [TestFixture]
    public class CreateChatRoomTests : TestBase
    {
        [Test]
        public void Should_create_new_chat_room()
        {
            var creator = new UserBuilder().BuildAndSave();
            
            var cmd = new CreateChatRoom { RoomName = "Test_Room", CreatorId = creator.Id };

            var room = Repository.Execute(cmd);

            Assert.That(DataContext.AsQueryable<ChatRoom>().Where(cr =>
                    cr.Id == room.Id && cr.Name == "Test_Room" && cr.Creator.Id == creator.Id &&
                    cr.CreatedAt.Value.Date == DateTime.UtcNow.Date), Has.Exactly(1).Items);
        }

        [Test]
        public void Should_not_allow_rooms_of_the_same_name_to_exist()
        {
            var existingName = "Test_Room";

            new ChatRoomBuilder().With(cr => cr.Name, existingName).BuildAndSave();

            var cmd = new CreateChatRoom { RoomName = existingName, CreatorId = Guid.NewGuid().ToString() };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Chat room '{existingName}' already exists"));
        }

        [TestCase("Test!")]
        [TestCase("Test<")]
        [TestCase("Test*")]
        [TestCase("Test?")]
        [TestCase("Test|")]
        [TestCase("Test%")]
        public void Should_not_allow_special_characters_in_room_name(string roomName)
        {
            var cmd = new CreateChatRoom { RoomName = roomName, CreatorId = Guid.NewGuid().ToString() };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    $"Chat room '{roomName}' contains unsupported characters, only alphanumeric names with _ or - are allowed"));
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
            var creator = new UserBuilder().BuildAndSave();

            Assert.That(() => Repository.Execute(new CreateChatRoom {RoomName = roomName, CreatorId = creator.Id}),
                Throws.Nothing);

            Assert.That(
                DataContext.AsQueryable<ChatRoom>().Where(cr => cr.Name == roomName && cr.Creator.Id == creator.Id),
                Is.Not.Empty);
        }

        [Test]
        public void Should_require_a_room_name_to_be_provided()
        {
            var cmd = new CreateChatRoom { CreatorId = Guid.NewGuid().ToString() };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("No room name was provided"));
        }

        [Test]
        public void Should_require_a_creator_id()
        {
            var cmd = new CreateChatRoom { RoomName = "Test_Room" };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("No room creator was provided"));
        }

        [Test]
        public void Should_not_be_able_to_create_room_if_creator_does_not_exist()
        {
            var cmd = new CreateChatRoom { RoomName = "Test_Room", CreatorId = Guid.NewGuid().ToString()};

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Room creator does not exist"));
        }
    }
}