namespace TT.Domain.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int? SkillSourceId { get; set; }
        public decimal Duration { get; set; }
        public decimal Charge { get; set; }
        public int TurnStamp { get; set; }
        public bool IsArchived { get; set; }
        public bool Bookmarked { get; set; }

    }

    public class Skill_VM
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int? SkillSourceId { get; set; }
        public decimal Duration { get; set; }
        public decimal Charge { get; set; }
        public int TurnStamp { get; set; }
        public bool IsArchived { get; set; }
        public bool Bookmarked { get; set; }

    }
}