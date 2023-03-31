using TT.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.ViewModels
{
    public class BlacklistEntryViewModel
    {
        public BlacklistEntry dbBlacklistEntry { get; set; }
        public string PlayerName { get; set;}
        public int PlayerId { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s\\.\\!\\?,'-:\/]*$", ErrorMessage = "Please only use alpha-numberical or normal punctuation symbols.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The note must be between 2 and 100 characters long.")]
        [Required(ErrorMessage = "You need to include a note.")]
        public string Note { get; set; }
        public string MembershipId { get; set; }
        public int BlacklistId { get; set; }
    }
}