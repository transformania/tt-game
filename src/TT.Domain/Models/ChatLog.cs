﻿using System;

namespace TT.Domain.Models
{
    public class ChatLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Room { get; set; }
    }
}