﻿using TT.Domain.Players.Entities;
using TT.Domain.Skills.Commands;

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

    }
}
