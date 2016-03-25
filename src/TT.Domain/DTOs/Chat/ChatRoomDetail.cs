using TT.Domain.Entities.Chat;

namespace TT.Domain.DTOs.Chat
{
    public class ChatRoomDetail
    {
        public int      Id          { get; set; }
        public string   Name        { get; set; }
        public string   Topic       { get; set; }
        public int      UserCount   { get; set; }

        public ChatRoomDetail() { }

        public ChatRoomDetail(ChatRoom room)
        {
            Id = room.Id;
            Name = room.Name;
            Topic = room.Topic ?? string.Empty;
            UserCount = 0;
        }
    }
}