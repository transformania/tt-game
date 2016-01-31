using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class BlacklistEntryViewModel
    {
        public BlacklistEntry dbBlacklistEntry { get; set; }
        public string PlayerName { get; set;}
        public int PlayerId { get; set; }
    }
}