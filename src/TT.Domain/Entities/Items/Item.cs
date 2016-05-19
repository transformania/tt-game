
using System;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.Items
{
    public class Item : Entity<int>
    {
        public string dbName { get; protected set; }
        public ItemSource ItemSource { get; protected set; }
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

        }

        public static Item Create(Player owner, ItemSource itemSource, CreateItem cmd)
        {
            return new Item
            {
                Owner = owner,
                ItemSource = itemSource,
                dbName = cmd.dbName,
                dbLocationName = cmd.dbLocationName,
                VictimName = cmd.VictimName,
                IsEquipped = cmd.IsEquipped,
                TurnsUntilUse = cmd.TurnsUntilUse,
                Level = cmd.Level,
                TimeDropped = cmd.TimeDropped,
                EquippedThisTurn = cmd.EquippedThisTurn,
                PvPEnabled = cmd.PvPEnabled,
                IsPermanent = cmd.IsPermanent,
                Nickname = cmd.Nickname,
                LastSouledTimestamp = cmd.LastSouledTimestamp,
                LastSold = cmd.LastSold
            };
        }

        public Item Update(UpdateItem cmd, Player owner)
        {
            Owner = owner;
            Id = cmd.ItemId;
            dbLocationName = cmd.dbLocationName;
            IsEquipped = cmd.IsEquipped;
            TimeDropped = cmd.TimeDropped;
            return this;
        }

        public Item Drop(Player owner)
        {
            Owner = null;
            dbLocationName = owner.Location;
            IsEquipped = false;
            TimeDropped = DateTime.UtcNow;
            return this;
        }

        public Item ChangeOwner(Player newOwner)
        {
            Owner = newOwner;
            return this;
        }

    }
}