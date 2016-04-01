using System.Linq;
using System.Text.RegularExpressions;
using Highway.Data;
using TT.Domain.DTOs.Chat;
using TT.Domain.Entities.Chat;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Commands.Chat
{
    public class CreateChatRoom : DomainCommand<ChatRoomDetail>
    {
        public string RoomName { get; set; }
        public string CreatorId { get; set; }

        public override ChatRoomDetail Execute(IDataContext context)
        {
            ChatRoomDetail result = null;

            ContextQuery = ctx =>
            {
                if (ctx.AsQueryable<ChatRoom>().Any(cr => cr.Name == RoomName))
                    throw new DomainException("Chat room '{0}' already exists", RoomName);

                var creator = ctx.AsQueryable<User>().SingleOrDefault(u => u.Id == CreatorId);
                if (creator == null)
                    throw new DomainException("Room creator does not exist");

                var room = ChatRoom.Create(creator, RoomName);

                ctx.Add(room);
                ctx.Commit();

                result = new ChatRoomDetail(room);
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(RoomName))
                throw new DomainException("No room name was provided");

            if (string.IsNullOrWhiteSpace(CreatorId))
                throw new DomainException("No room creator was provided");

            var regex = new Regex("^[a-zA-Z0-9_-]*$");
            if (!regex.IsMatch(RoomName))
                throw new DomainException("Chat room '{0}' contains unsupported characters, only alphanumeric names with _ or - are allowed",RoomName);
        }
    }
}