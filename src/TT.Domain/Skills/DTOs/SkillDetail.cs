using TT.Domain.Players.DTOs;

namespace TT.Domain.Skills.DTOs
{
    public class SkillDetail
    {
        public int Id { get; set; }
        public PlayerDetail Owner { get; set; }
        public string Name { get; set; }
        public SkillSourceDetail SkillSource { get; set; }
        public decimal Duration { get; set; }
        public decimal Charge { get; set; }
        public int TurnStamp { get; set; }
        public bool IsArchived { get; set; }
        public bool Bookmarked { get; set; }
    }
}
