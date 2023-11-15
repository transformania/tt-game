﻿using System.Collections.Generic;
using TT.Domain.Messages.DTOs;

namespace TT.Domain.ViewModels
{
    public class MessageBag
    {
        public IEnumerable<MessageDetail> Messages { get; set; }
        public IEnumerable<MessageDetail> SentMessages { get; set; }
        public int WearerId { get; set; }
        public int WearerBotId { get; set; }
        public string WearerName { get; set; }
        public Paginator Paginator { get; set; }
        public int InboxSize { get; set; }
        public bool FriendOnlyMessages { get; set; }
    }
}