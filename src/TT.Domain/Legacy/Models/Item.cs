using System;

namespace TT.Domain.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int ItemSourceId { get; set; }
        public int? OwnerId { get; set; }
        public string dbLocationName { get; set; }
        public bool IsEquipped { get; set; }
        public int TurnsUntilUse { get; set; }
        public int Level { get; set; }
        public DateTime TimeDropped { get; set; }
        public bool EquippedThisTurn { get; set; }
        public int PvPEnabled { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime LastSouledTimestamp { get; set; }
        public DateTime LastSold { get; set; }
        public int? EmbeddedOnItemId { get; set; }
        public int? SoulboundToPlayerId { get; set; }
        public int? FormerPlayerId { get; set; }

        public Item()
        {
            TimeDropped = DateTime.UtcNow;
            LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
            LastSold = DateTime.UtcNow;
        }

    }

    public class Item_VM
    {
        public int Id { get; set; }
        public int ItemSourceId { get; set; }
        public int? OwnerId { get; set; }
        public string dbLocationName { get; set; }
        public bool IsEquipped { get; set; }
        public int TurnsUntilUse { get; set; }
        public int Level { get; set; }
        public DateTime TimeDropped { get; set; }
        public bool EquippedThisTurn { get; set; }
        public int PvPEnabled { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime LastSouledTimestamp { get; set; }
        public DateTime LastSold { get; set; }
        public int? EmbeddedOnItemId { get; set; }
        public int? SoulboundToPlayerId { get; set; }

        public Item_VM()
        {
            TimeDropped = DateTime.UtcNow;
            LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
            LastSold = DateTime.UtcNow;
        }

    }
}