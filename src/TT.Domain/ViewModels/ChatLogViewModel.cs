using System.Collections.Generic;
using TT.Domain.Chat.DTOs;

namespace TT.Domain.ViewModels
{
    public class ChatLogViewModel
    {
        public string Room { get; set; }
        public string RoomName { get; set; }
        public string Filter { get; set; }
        public IEnumerable<ChatLogDetail> ChatLog { get; set; }
    }
}