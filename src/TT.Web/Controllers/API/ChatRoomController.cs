using System.Web.Http;
using TT.Domain;
using TT.Domain.Commands.Chat;

namespace TT.Web.Controllers.API
{
    public class ChatRoomController : ApiController
    {
        public IHttpActionResult Put(CreateChatRoom cmd)
        {
            var chatRoom = DomainRegistry.Repository.Execute(cmd);
            return CreatedAtRoute("DefaultApi", new { id = chatRoom.Name }, chatRoom);
        }
    }
}