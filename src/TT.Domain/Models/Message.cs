﻿using System;

namespace TT.Domain.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool IsRead { get; set; }
        public int ReadStatus { get; set; }

        //[StringLength(30, MinimumLength = 15, ErrorMessage = "Your message must be at least 15 letters long.")]
        //[Required(ErrorMessage = "You must include a message.")]
        public string MessageText { get; set; }

        public DateTime Timestamp { get; set; }

        public bool DoNotRecycleMe { get; set; }
        public bool ReceiverMarkedAsDeleted { get; set; }
    }
}