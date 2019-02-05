using System;

namespace TT.Domain.Models
{
    public class BookReading
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BookItemSourceId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}