
using System;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.Items
{
    public class Item : Entity<int>
    {
        public string dbName { get; protected set; }
        public ItemSource ItemSource { get; protected set; }
        public int OwnerId { get; protected set; }
        public Player Owner { get; protected set; }
        public string dbLocationName { get; protected set; }
        public string VictimName { get; protected set; }
        public bool IsEquipped { get; protected set; }
        public int TurnsUntilUse { get; protected set; }
        public int Level { get; protected set; }
        public DateTime TimeDropped { get; protected set; }
        public bool EquippedThisTurn { get; protected set; }
        public int PvPEnabled { get; protected set; }
        public bool IsPermanent { get; protected set; }
        public string Nickname { get; protected set; }
        public DateTime LastSouledTimestamp { get; protected set; }
        public DateTime LastSold { get; protected set; }

        private Item()
        {
            //TimeDropped = DateTime.UtcNow;
            //LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
            //LastSold = DateTime.UtcNow;
        }

        public static Item Create(Player owner, ItemSource itemSource)
        {
            return new Item
            {
                Owner = owner,
                ItemSource = itemSource
            };
        }

    }
}