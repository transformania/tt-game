using System;

namespace tfgame.dbModels.Models
{
    public class ItemTransferLog
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int OwnerId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ItemTransferLog_VM
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int OwnerId { get; set; }
        public DateTime Timestamp { get; set; }
    }

}