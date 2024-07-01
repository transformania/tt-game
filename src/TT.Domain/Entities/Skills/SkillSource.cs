using TT.Domain.Effects.Entities;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.Entities.Skills
{
    public class SkillSource : Entity<int>
    {
        public string FriendlyName { get; protected set; }

        public string FormdbName { get; protected set; }
        public FormSource FormSource { get; protected set; }

        public string Description { get; protected set; }
        public decimal ManaCost { get; protected set; }
        public decimal TFPointsAmount { get; protected set; }
        public decimal HealthDamageAmount { get; protected set; }
        public string LearnedAtRegion { get; protected set; }
        public string LearnedAtLocation { get; protected set; }
        public string DiscoveryMessage { get; protected set; }
        public string IsLive { get; protected set; }
        public bool IsPlayerLearnable { get; protected set; }

        public string GivesEffect { get; protected set; }
        public EffectSource GivesEffectSource { get; protected set; }

        public string ExclusiveToForm { get; protected set; }
        public FormSource ExclusiveToFormSource { get; protected set; }

        public string ExclusiveToItem { get; protected set; }
        public ItemSource ExclusiveToItemSource { get; protected set; }

        public string MobilityType { get; protected set; }

        private SkillSource() { }

        public void SetSources(FormSource formSource, EffectSource givesEffectSource, FormSource exclusiveToFormSource, ItemSource exlusiveToItemSource)
        {
            this.FormSource = formSource;
            this.GivesEffectSource = givesEffectSource;
            this.ExclusiveToFormSource = exclusiveToFormSource;
            this.ExclusiveToItemSource = exlusiveToItemSource;
        }

        public SkillSourceDetail MapToDto()
        {
            return new SkillSourceDetail
            {
                Id = Id,
                FriendlyName = FriendlyName,
                FormSource = FormSource?.MapToDto(),
                Description = Description,
                ManaCost = ManaCost,
                TFPointsAmount = TFPointsAmount,
                HealthDamageAmount = HealthDamageAmount,
                LearnedAtRegion = LearnedAtRegion,
                LearnedAtLocation = LearnedAtLocation,
                DiscoveryMessage = DiscoveryMessage,
                IsLive = IsLive,
                IsPlayerLearnable = IsPlayerLearnable,
                GivesEffect = GivesEffect,
                GivesEffectSource = GivesEffectSource?.MapToDto(),
                ExclusiveToForm = ExclusiveToForm,
                ExclusiveToFormSource = ExclusiveToFormSource?.MapToDto(),
                ExclusiveToItem = ExclusiveToItem,
                ExclusiveToItemSource = ExclusiveToItemSource?.MapToDto(),
                MobilityType = MobilityType
            };
        }

        public FormSourceNameDetail MapToFormSourceNameDto()
        {
            return new FormSourceNameDetail
            {
                Id = Id,
                FriendlyName = FriendlyName,
                Description = Description,
                GivesEffectSource = GivesEffectSource?.MapToDto(),
                FormSource = FormSource?.MapToFormNameDescriptionDto(),
                MobilityType = MobilityType
            };
        }

        public LearnableSkillsDetail MapToLearnableSkillsDto()
        {
            return new LearnableSkillsDetail
            {
                Id = Id,
                FriendlyName = FriendlyName,
                MobilityType = MobilityType,
                FormSource = FormSource?.MapToFormNameDescriptionDto()
            };
        }
    }
}
