using TT.Domain.DTOs.Players;
using TT.Domain.Entities.Skills;

namespace TT.Domain.DTOs.Skills
{
    public class SkillDetail
    {
        public int Id { get; set; }
        public PlayerDetail Owner { get; set; }
        public string Name { get; set; }
        public SkillSource SkillSource { get; set; }
        public decimal Duration { get; set; }
        public decimal Charge { get; set; }
        public int TurnStamp { get; set; }
        public bool IsArchived { get; set; }
    }
}
