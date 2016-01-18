using System;
using System.ComponentModel.DataAnnotations;

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

        //public bool StartInPVP { get; set; }

        public int StartGameMode { get; set; }

        public bool StartInRP { get; set; }

        public bool MigrateLetters { get; set; }

        public bool StartAsInanimate { get; set; }

        public enum InanimateTypesEnum
        {
            [Display(Name = "Accessory")]
            accessory,
            [Display(Name = "Reusable Consumable")]
            consumable_reuseable,
            [Display(Name = "Shirt")]
            shirt,
            [Display(Name = "Undershirt")]
            undershirt,
            [Display(Name = "Pants")]
            pants,
            [Display(Name = "Underpants")]
            underpants,
            [Display(Name = "Hat")]
            hat,
            [Display(Name = "Shoes")]
            shoes,
            [Display(Name = "Pet")]
            pet,
            [Display(Name = "Surprise me!")]
            random
        }

        public virtual Nullable<InanimateTypesEnum> InanimateForm { get; set; }

    }
}