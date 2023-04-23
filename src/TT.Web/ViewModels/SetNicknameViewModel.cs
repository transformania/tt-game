using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Web.ViewModels

{
    public class SetNicknameViewModel
    {
        public string Nickname { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "You can only use letters in the first name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The first name must be between 2 and 30 letters long.")]
        [Required(ErrorMessage = "You need a first name.")]
        public string OriginalFirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "You can only use letters in the last name.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The last name must be between 2 and 30 letters long.")]
        [Required(ErrorMessage = "You need a last name.")]
        public string OriginalLastName { get; set; }
        public bool HasSelfRenamed { get; set; }
        public int ItemId { get; set; }
        public enum PersonalityTypesEnum
        {
            [Display(Name = "Vapid")]
            BIMBOS,
            [Display(Name = "Catty")]
            CATS_AND_NEKOS,
            [Display(Name = "Festive")]
            CHRISTMAS_FORMS,
            [Display(Name = "Loyal")]
            DOGS,
            [Display(Name = "Compliant")]
            DRONES,
            [Display(Name = "Bright")]
            EASTER_FORMS,
            [Display(Name = "Playful")]
            FAIRIES,
            [Display(Name = "Invisible")]
            GHOSTS,
            [Display(Name = "Eerie")]
            HALLOWEEN_FORMS,
            [Display(Name = "Servile")]
            MAIDS,
            [Display(Name = "Energetic")]
            MANA_FORMS,
            [Display(Name = "Puckish")]
            MISCHIEVOUS_FORMS,
            [Display(Name = "Mousy")]
            RODENTS,
            [Display(Name = "Amorous")]
            ROMANTIC_FORMS,
            [Display(Name = "Timid")]
            SHEEP,
            [Display(Name = "Voyeuristic")]
            STRIPPERS,
            [Display(Name = "Sly")]
            THIEVES,
            [Display(Name = "Rooted")]
            TREES,
        }

        public virtual Nullable<PersonalityTypesEnum> Personalities { get; set; }
    }
}