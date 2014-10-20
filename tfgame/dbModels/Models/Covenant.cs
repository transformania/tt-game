using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Covenant
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "You can only use letters in your covenant's name.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "The covenant name must be between 8 and 50 characters long.")]
        [Required(ErrorMessage = "Covenant name is required.")]
        public string Name { get; set; }


        public string FlagUrl { get; set; }

        [StringLength(200, MinimumLength = 25, ErrorMessage = "The covenant description must be between 25 and 200 characters long.")]
        [Required(ErrorMessage = "Covenant description is required.")]
        public string SelfDescription { get; set; }


        public DateTime LastMemberAcceptance { get; set; }
        public int LeaderId { get; set; }
        public int FounderMembershipId { get; set; }
        public bool IsPvP { get; set; }
        public string formerMembers { get; set; }
        public decimal Money { get; set; }
        public string HomeLocation { get; set; }
    }
}