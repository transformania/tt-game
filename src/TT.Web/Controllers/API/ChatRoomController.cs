using System.Web.Http;
using TT.Domain;
using TT.Domain.Commands.Chat;

namespace TT.Web.Controllers.API
{
    public class ChatRoomController : ApiControllerBase
    {
        public IHttpActionResult Put(CreateChatRoom cmd)
        {
            var userId = GetUserId();
            cmd.CreatorId = userId;

            var chatRoom = DomainRegistry.Repository.Execute(cmd);
            return CreatedAtRoute("DefaultApi", new { id = chatRoom.RoomName }, chatRoom);
        }
    }
}