using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.ViewModels
{
    public class PublicBroadcastViewModel
    {
        public string Message { get; set; }
    }

    public class PlayerNameViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }

        [RegularExpression(@"^[a-zA-Z'’-]+$", ErrorMessage = "You can only use letters in the first name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The first name must be between 2 and 30 letters long.")]
        [Required(ErrorMessage = "You need a first name.")]
        public string NewFirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z'’-]+$", ErrorMessage = "You can only use letters in the last name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The last name must be between 2 and 30 letters long.")]
        [Required(ErrorMessage = "You need a last name.")]
        public string NewLastName { get; set; }

        public int NewFormSourceId { get; set; }
        public int Level { get; set; }
        public decimal Money { get; set; }
    }
}