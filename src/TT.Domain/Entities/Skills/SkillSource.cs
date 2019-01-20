using TT.Domain.Effects.Entities;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;

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
    }
}
