using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Chat.DTOs;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Queries
{
    public class GetChatRooms : DomainQuery<ChatRoomDetail>
    {
        public override IEnumerable<ChatRoomDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => {
                var rooms = ctx.AsQueryable<ChatRoom>().ToList();
                return rooms.Select(cr => cr.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}