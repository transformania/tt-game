namespace TT.Domain.ViewModels
{
    public class StaticSkill
    {
        //this class stores all of the skills as available to everyone.  It has absolutely nothing to do with what an individual player has.
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

        public string GivesEffect { get; set; }

        public int? ExclusiveToFormSourceId { get; set; }
        public string ExclusiveToItem { get; set; }
    }
}