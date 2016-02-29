using System;

namespace TT.Domain.ViewModels
{
    public class ProofreadingContributionsViewModel
    {
        public int Id { get; set; }
        public bool IsLive { get; set; }
        public bool ProofreadingCopy { get; set; }
        public bool ProofreadingLockIsOn { get; set; }
        public string CheckedOutBy { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string Skill_FriendlyName { get; set; }
        public string Form_FriendlyName { get; set; }
        public bool NeedsToBeUpdated { get; set; }
    }
}