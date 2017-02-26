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
                return ctx.AsQueryable<ChatRoom>().Select(cr => new ChatRoomDetail
                {
                    Id = cr.Id,
                    Name = cr.Name,
                    Topic = cr.Topic ?? string.Empty
                });
            };

            return ExecuteInternal(context);
        }
    }
}