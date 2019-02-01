using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class EffectContribution
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMemberhipId { get; set; }
        public string SubmitterName { get; set; }

        public string AdditionalSubmitterNames { get; set; }

        public string SubmitterURL { get; set; }

        public string Skill_FriendlyName { get; set; }
        public int? Skill_UniqueToFormSourceId { get; set; }
        public string Skill_UniqueToItem { get; set; }
        public string Skill_UniqueToLocation { get; set; }
        public string Skill_Description { get; set; }
        public decimal Skill_ManaCost { get; set; }

        public string Effect_FriendlyName { get; set; }
        public string Effect_Description { get; set; }
        public bool Effect_IsRemovable { get; set; }

        public int Effect_Duration { get; set; }
        public int Effect_Cooldown { get; set; }
        public string Effect_Bonuses { get; set; }

        public string Effect_VictimHitText { get; set; }
        public string Effect_VictimHitText_M { get; set; }
        public string Effect_VictimHitText_F { get; set; }

        public string Effect_AttackHitText { get; set; }
        public string Effect_AttackHitText_M { get; set; }
        public string Effect_AttackHitText_F { get; set; }

      
        public bool ReadyForReview { get; set; }
        public bool ApprovedByAdmin { get; set; }
        public bool IsLive { get; set; }

        public bool ProofreadingCopy { get; set; }
        public bool ProofreadingLockIsOn { get; set; }
        public string CheckedOutBy { get; set; }
        public string Notes { get; set; }
        public string History { get; set; }
        public int ProofreadingCopyForOriginalId { get; set; }
        public DateTime Timestamp { get; set; }

        public bool NeedsToBeUpdated { get; set; }

        public decimal HealthBonusPercent { get; set; }
        public decimal ManaBonusPercent { get; set; }
        public decimal ExtraSkillCriticalPercent { get; set; }
        public decimal HealthRecoveryPerUpdate { get; set; }
        public decimal ManaRecoveryPerUpdate { get; set; }
        public decimal SneakPercent { get; set; }
        public decimal EvasionPercent { get; set; }
        public decimal EvasionNegationPercent { get; set; }
        public decimal MeditationExtraMana { get; set; }
        public decimal CleanseExtraHealth { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SpellExtraTFEnergyPercent { get; set; }
        public decimal SpellExtraHealthDamagePercent { get; set; }
        public decimal CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal SpellMisfireChanceReduction { get; set; }
        public decimal SpellHealthDamageResistance { get; set; }
        public decimal SpellTFEnergyDamageResistance { get; set; }
        public decimal ExtraInventorySpace { get; set; }

        public float Discipline { get; set; }
        public float Perception { get; set; }
        public float Charisma { get; set; }
        public float Submission_Dominance { get; set; }

        public float Fortitude { get; set; }
        public float Agility { get; set; }
        public float Allure { get; set; }
        public float Corruption_Purity { get; set; }

        public float Magicka { get; set; }
        public float Succour { get; set; }
        public float Luck { get; set; }
        public float Chaos_Order { get; set; }

        public int? SkillSourceId { get; set; }

        public int? EffectSourceId { get; set; }

    }
}