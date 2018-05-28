using TT.Domain.Models;
using TT.Domain.Procedures;

namespace TT.Domain.ViewModels
{
    public class FurnitureViewModel
    {
        public Furniture_VM dbFurniture { get; set; }
        public StaticFurniture FurnitureType { get; set; }
        public int MyUserId { get; set; }

        public bool IsLastUser(int playerId)
        {
            return this.dbFurniture.LastUsersIds == playerId.ToString();
        }

        public bool CanUse(int playerId)
        {
            return !this.IsLastUser(playerId) && FurnitureProcedures.GetMinutesUntilReuse(this) <= 0;
        }
    }
}