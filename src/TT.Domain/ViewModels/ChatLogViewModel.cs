using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class ChatLogViewModel
    {
        public string Room { get; set; }
        public string Filter { get; set; }
        public IEnumerable<DTOs.Chat.ChatLogDetail> ChatLog { get; set; }
    }
}