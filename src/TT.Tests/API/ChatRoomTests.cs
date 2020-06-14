using System.Collections.Generic;
using System.Web.Http.Results;
using NUnit.Framework;
using TT.Domain.Chat.Commands;
using TT.Domain.Chat.DTOs;
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

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.RouteName, Is.EqualTo("DefaultApi"));
            Assert.That(createdResult.RouteValues["id"], Is.EqualTo("Test_Room"));
        }

        [Test]
        public void Should_return_bad_request_on_error()
        {
            var creator = new UserBuilder().BuildAndSave();
            var controller = new ChatRoomController();
            controller.OverrideGetUserId(() => creator.Id);

            var actionResult = controller.Put(new CreateChatRoom());
            var badRequestResult = actionResult as BadRequestErrorMessageResult;

            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Message, Is.Not.Empty);
        }

        [Test]
        public void Should_return_list_of_chat_rooms()
        {
            new ChatRoomBuilder().With(cr => cr.Name, "Room 1").BuildAndSave();
            new ChatRoomBuilder().With(cr => cr.Name, "Room 2").BuildAndSave();

            var controller = new ChatRoomController();

            var actionResult = controller.Get();
            var rooms = actionResult as OkNegotiatedContentResult<IEnumerable<ChatRoomDetail>>;

            Assert.That(rooms, Is.Not.Null);
            Assert.That(rooms.Content, Has.Exactly(2).Items);
        }
    }
}
