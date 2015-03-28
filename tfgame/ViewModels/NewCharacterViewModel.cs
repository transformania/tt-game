using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class NewCharacterViewModel
    {
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "You can only use letters in your first name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Your first name must be between 2 and 12 letters long.")]
        [Required(ErrorMessage = "You need a first name.")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "You can only use letters in your last name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Your last name must be between 2 and 12 letters long.")]
        [Required(ErrorMessage = "You need a last name.")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Your gender must be 'male' or 'female'.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "You must choose a character form.")]
        public string FormName { get; set; }

        public bool StartInPVP { get; set; }
        public bool StartInRP { get; set; }

        public bool MigrateLetters { get; set; }

    }
}