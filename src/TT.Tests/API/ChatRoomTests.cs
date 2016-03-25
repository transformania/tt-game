using System.Collections.Generic;
using System.Web.Http.Results;
using FluentAssertions;
using Highway.Data.Contexts;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Chat;
using TT.Domain.DTOs.Chat;
using TT.Tests.Builders.Chat;
using TT.Tests.Builders.Identity;
using TT.Web.Controllers.API;

namespace TT.Tests.API
{
    [TestFixture]
    public class ChatRoomTests : TestBase
    {
        [Test]
        public void Should_create_chat_room()
        {
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
            var creator = new UserBuilder().BuildAndSave();
            var controller = new ChatRoomController();
            controller.OverrideGetUserId(() => creator.Id);

            var actionResult = controller.Put(new CreateChatRoom());
            var badRequestResult = actionResult as BadRequestErrorMessageResult;

            badRequestResult.Message.Should().NotBeEmpty();
        }

        [Test]
        public void Should_return_list_of_chat_rooms()
        {
            new ChatRoomBuilder().With(cr => cr.Name, "Room 1").BuildAndSave();
            new ChatRoomBuilder().With(cr => cr.Name, "Room 2").BuildAndSave();

            var controller = new ChatRoomController();

            var actionResult = controller.Get();
            var rooms = actionResult as OkNegotiatedContentResult<IEnumerable<ChatRoomDetail>>;

            rooms.Content.Should().HaveCount(2);
        }
    }
}
