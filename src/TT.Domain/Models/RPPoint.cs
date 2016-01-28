using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class RPPoint
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public int Amount { get; set; }
        public int RemainingPointsToGive { get; set; }
    }
}