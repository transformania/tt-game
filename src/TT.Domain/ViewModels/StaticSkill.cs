namespace TT.Domain.ViewModels
{
    public class StaticSkill
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public int? FormSourceId { get; set; }
        public string Description { get; set; }
        public decimal ManaCost { get; set; }
        public decimal TFPointsAmount { get; set; }
        public decimal HealthDamageAmount { get; set; }
        public string LearnedAtRegion { get; set; }
        public string LearnedAtLocation { get; set; }
        public string DiscoveryMessage { get; set; }

        public bool IsPlayerLearnable { get; set; }

        public int? GivesEffectSourceId { get; set; }

        public int? ExclusiveToFormSourceId { get; set; }
        public int? ExclusiveToItemSourceId { get; set; }
        
    }
}