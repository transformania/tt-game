using System.Linq;
using TT.Domain.DTOs.Chat;
using TT.Domain.Entities.Chat;

namespace TT.Domain.Queries.Chat
{
    public class GetChatRooms : Highway.Data.Query<ChatRoomDetail>
    {
        public GetChatRooms()
        {
            ContextQuery = ctx => {
                return ctx.AsQueryable<ChatRoom>().Select(cr => new ChatRoomDetail
                {
                    Id = cr.Id,
                    Name = cr.Name,
                    Topic = cr.Topic ?? string.Empty
                });
            };
        }
    }
}