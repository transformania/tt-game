using System;
using System.Web.Http.Results;
using FluentAssertions;
using Highway.Data.Contexts;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Chat;
using TT.Domain.DTOs;
using TT.Tests.Builders.Identity;
using TT.Web.Controllers.API;

namespace TT.Tests.API
{
    [TestFixture]
    public class ChatRoomTests
    {
        [Test]
        public void Should_create_chat_room()
        {
            DomainRegistry.Repository = new DomainRepository(new InMemoryDataContext());
            var controller = new ChatRoomController();

            var creator = new UserBuilder().BuildAndSave();

            var actionResult = controller.Put(new CreateChatRoom { RoomName = "Test_Room", CreatorId = creator.Id });
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<ChatRoomDetail>;

            createdResult.Should().NotBeNull();
            createdResult.RouteName.Should().Be("DefaultApi");
            createdResult.RouteValues["id"].Should().Be("Test_Room");
        }
    }
}