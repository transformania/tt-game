using TT.Domain.Entities.Chat;

namespace TT.Domain.DTOs
{
    public class ChatRoomDetail
    {
        public int      Id       { get; set; }
        public string   RoomName { get; set; }

        public ChatRoomDetail(ChatRoom room)
        {
            Id = room.Id;
            RoomName = room.Name;
        }
    }
}