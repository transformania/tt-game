using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class PollEntry
    {
        
        public int Id { get; set; }
        public int OwnerMembershipId { get; set; }
        public int PollId { get; set; }
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Num3 { get; set; }
        public int Num4 { get; set; }
        public int Num5 { get; set; }

        [StringLength(500, ErrorMessage = "Your response cannot be more than 500 characters long.")]
        public string String1 { get; set; }
        [StringLength(500, ErrorMessage = "Your response cannot be more than 500 characters long.")]
        public string String2 { get; set; }
        [StringLength(500, ErrorMessage = "Your response cannot be more than 500 characters long.")]
        public string String3 { get; set; }
        [StringLength(500, ErrorMessage = "Your response cannot be more than 500 characters long.")]
        public string String4 { get; set; }
        [StringLength(500, ErrorMessage = "Your response cannot be more than 500 characters long.")]
        public string String5 { get; set; }
        public DateTime Timestamp { get; set; }
        public string Round { get; set; }
    }
}