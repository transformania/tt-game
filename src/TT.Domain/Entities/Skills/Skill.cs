using TT.Domain.Players.Entities;
using TT.Domain.Skills.Commands;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.Entities.Skills
{
    public class Skill : Entity<int>
    {
        public Player Owner { get; protected set; }
        public string Name { get; protected set; }
        public SkillSource SkillSource { get; protected set; }
        public decimal Duration { get; protected set; }
        public decimal Charge { get; protected set; }
        public int TurnStamp { get; protected set; }
        public bool IsArchived { get; protected set; }
        public bool Bookmarked { get; protected set; }

        private Skill() { }

        public static Skill Create(Player player, SkillSource skillSource, CreateSkill cmd)
        {
            var newSkill = new Skill
            {
                Owner = player,
                SkillSource = skillSource,
                IsArchived = false,
                Bookmarked = false,
            };
            return newSkill;
        }

        public static Skill CreateAll(Player player, SkillSource skillSource, CreateAllSkills cmd)
        {
            var newSkill = new Skill
            {
                Owner = player,
                SkillSource = skillSource,
                IsArchived = false,
                Bookmarked = false,
            };
            return newSkill;
        }

        public SkillDetail MapToDto()
        {
            return new SkillDetail
            {
                Id = Id,
                Owner = Owner.MapToDto(),  // MapToDetailDto() are assumed methods in Player and SkillSource
                Name = Name,
                SkillSource = SkillSource.MapToDto(),
                Duration = Duration,
                Charge = Charge,
                TurnStamp = TurnStamp,
                IsArchived = IsArchived,
                Bookmarked = Bookmarked
            };
        }

        public SkillSourceFormSourceDetail MapToFormSourceDto()
        {
            return new SkillSourceFormSourceDetail
            {
                Id = Id,
                IsArchived = IsArchived,
                Bookmarked = Bookmarked,
                SkillSource = SkillSource.MapToFormSourceNameDto()
            };
        }
    }
}
