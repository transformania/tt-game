using System.Collections.Generic;
using TT.Domain.Chat.DTOs;

namespace TT.Domain.ViewModels
{
    public class ChatViewModel
    {
        public string Room { get; set; }
        public string RoomName { get; set; }
        public string ChatUser { get; set; }
        public string ChatColor { get; set; }
        public bool ChatV2 { get; set; }
        public IEnumerable<ChatLogDetail> ChatLog { get; set; }
    }
}