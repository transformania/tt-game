﻿
using System;

namespace TT.Domain.Identity.DTOs
{
    public class UserDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool AllowChaosChanges { get; set; }
        public bool? Approved { get; set; }
        public DateTime? CreateDate { get; protected internal set; }
    }
}
