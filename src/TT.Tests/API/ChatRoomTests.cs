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
            var creator = new UserBuilder().BuildAndSave();
            var controller = new ChatRoomController();
            controller.OverrideGetUserId(() => creator.Id);

            var actionResult = controller.Put(new CreateChatRoom { RoomName = "Test_Room" });
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<ChatRoomDetail>;

            createdResult.Should().NotBeNull();
            createdResult.RouteName.Should().Be("DefaultApi");
            createdResult.RouteValues["id"].Should().Be("Test_Room");
        }

        [Test]
        public void Should_return_bad_request_on_error()
        {
            DomainRegistry.Repository = new DomainRepository(new InMemoryDataContext());
            var creator = new UserBuilder().BuildAndSave();
            var controller = new ChatRoomController();
            controller.OverrideGetUserId(() => creator.Id);

            var actionResult = controller.Put(new CreateChatRoom());
            var badRequestResult = actionResult as BadRequestErrorMessageResult;

            badRequestResult.Message.Should().NotBeEmpty();
        }
    }
}