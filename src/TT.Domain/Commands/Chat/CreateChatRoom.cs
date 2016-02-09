using System.Linq;
using System.Text.RegularExpressions;
using Highway.Data;
using TT.Domain.Entities.Chat;

namespace TT.Domain.Commands.Chat
{
    public class CreateChatRoom : Highway.Data.Command
    {
        public string RoomName { get; private set; }
        public string CreatorId { get; private set; }

        public CreateChatRoom(string roomName, string creatorId)
        {
            RoomName = roomName;
            CreatorId = creatorId;
        }

        public override void Execute(IDataContext context)
        {
            Validate();

            ContextQuery = ctx =>
            {
                if (ctx.AsQueryable<ChatRoom>().Any(cr => cr.Name == RoomName))
                    throw new DomainException("Chat room '{0}' already exists", RoomName);

                ctx.Add(ChatRoom.Create(this));
                ctx.Commit();
            };

            base.Execute(context);
        }

        private void Validate()
        {
            var regex = new Regex("^[a-zA-Z0-9_-]*$");
            if (!regex.IsMatch(RoomName))
                throw new DomainException("Chat room '{0}' contains unsupported characters, only alphanumeric names with _ or - are allowed",RoomName);
        }
    }
}