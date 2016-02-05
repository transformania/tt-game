using System;
using System.Linq;
using FluentAssertions;
using Highway.Data;
using Highway.Data.Contexts;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Chat;
using TT.Domain.Entities.Chat;
using TT.Tests.Builders.Chat;

namespace TT.Tests.Domain.Commands.Chat
{
    [TestFixture]
    public class CreateChatRoomTests
    {
        [Test]
        public void Should_create_new_chat_room()
        {
            var ctx = new InMemoryDataContext();
            DomainRegistry.Repository = new Repository(ctx);

            var creatorId = Guid.NewGuid().ToString();
            
            var cmd = new CreateChatRoom("Test Room", creatorId);

            DomainRegistry.Repository.Execute(cmd);

            ctx.AsQueryable<ChatRoom>().Count(
                cr => cr.Name == "Test Room" && 
                cr.Creator == creatorId && 
                cr.CreatedAt.Value.Date == DateTime.UtcNow.Date)
            .Should().Be(1);
        }

        [Test]
        public void Should_not_allow_rooms_of_the_same_name_to_exist()
        {
            var ctx = new InMemoryDataContext();
            DomainRegistry.Repository = new Repository(ctx);

            var existingName = "Test Room";

            new ChatRoomBuilder().With(cr => cr.Name, existingName).BuildAndSave();

            var cmd = new CreateChatRoom(existingName, Guid.NewGuid().ToString());

            var action = new Action(() => { DomainRegistry.Repository.Execute(cmd); });
            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Chat room '{0}' already exists", existingName));
        }
    }
}