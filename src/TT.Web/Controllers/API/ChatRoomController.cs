using System.Web.Http;
using TT.Domain;
using TT.Domain.Chat.Commands;
using TT.Domain.Chat.Queries;
using TT.Domain.Exceptions;

namespace TT.Web.Controllers.API
{
    public class ChatRoomController : ApiControllerBase
    {
        public IHttpActionResult Put(CreateChatRoom cmd)
        {
            var userId = GetUserId();
            cmd.CreatorId = userId;

            try
            {
                var chatRoom = DomainRegistry.Repository.Execute(cmd);
                return CreatedAtRoute("DefaultApi", new {id = chatRoom.Name}, chatRoom);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);   
            }
        }

        public IHttpActionResult Get()
        {
            return Ok(DomainRegistry.Repository.Find(new GetChatRooms()));
        }
    }
}