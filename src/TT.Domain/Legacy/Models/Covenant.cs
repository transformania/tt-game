﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
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

        [StringLength(200, ErrorMessage = "Noticeboard message cannot exceed 200 characters.")]
        public string NoticeboardMessage { get; set; }

        public DateTime LastMemberAcceptance { get; set; }
        public int LeaderId { get; set; }

        public bool IsPvP { get; set; }

        public string Captains { get; set; }
        public decimal Money { get; set; }
        public string HomeLocation { get; set; }

        public int Level { get; set; }
        public int CovenMascot { get; set; }
        public string CovenBlurb { get; set; }
    }
}