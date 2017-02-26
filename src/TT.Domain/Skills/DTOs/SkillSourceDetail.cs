using TT.Domain.Effects.DTOs;
using TT.Domain.Forms.DTOs;
using TT.Domain.Items.DTOs;

namespace TT.Domain.Skills.DTOs
{
    public class SkillSourceDetail
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string FormdbName { get; set; }
        public FormSourceDetail FormSource { get; set; }
        public string Description { get; set; }
        public decimal ManaCost { get; set; }
        public decimal TFPointsAmount { get; set; }
        public decimal HealthDamageAmount { get; set; }
        public string LearnedAtRegion { get; set; }
        public string LearnedAtLocation { get; set; }
        public string DiscoveryMessage { get; set; }
        public string IsLive { get; set; }
        public bool IsPlayerLearnable { get; set; }

        public string GivesEffect { get; set; }
        public EffectSourceDetail GivesEffectSource { get; set; }

        public string ExclusiveToForm { get; set; }
        public FormSourceDetail ExclusiveToFormSource { get; set; }

        public string ExclusiveToItem { get; set; }
        public ItemSourceDetail ExclusiveToItemSource { get; set; }

        public string MobilityType { get; set; }
    }
}
