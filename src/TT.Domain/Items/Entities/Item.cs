using System;
using TT.Domain.Entities;
using TT.Domain.Items.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Entities
{
    public class Item : Entity<int>
    {
        public string dbName { get; protected set; }
        public ItemSource ItemSource { get; protected set; }
        public Player Owner { get; protected set; }
        public Player FormerPlayer { get; protected set; }
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

        public static Item Create(Player owner, Player formerPlayer, ItemSource itemSource, CreateItem cmd)
        {
            var newItem = new Item()
            {
                Owner = owner,
                FormerPlayer = formerPlayer,
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

            if (owner != null && owner.BotId != AIStatics.ActivePlayerBotId)
            {
                newItem.LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
            }

            return newItem;
        }
        
        /// <summary>
        /// Creates a consumable type item
        /// </summary>
        /// <param name="owner">Owner of the item</param>
        /// <param name="itemSource">ItemSource of the item</param>
        /// <returns>newly created item</returns>
        public static Item Create(Player owner, ItemSource itemSource)
        {
            return new Item
            {
                Owner = owner,
                dbLocationName = "",
                IsPermanent = false,
                ItemSource = itemSource,
                dbName = itemSource.DbName,
                PvPEnabled = GameModeStatics.Any,
                TimeDropped = DateTime.UtcNow,
                LastSouledTimestamp = DateTime.UtcNow.AddYears(-1),
                LastSold = DateTime.UtcNow
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

        public void PickUp(Player newOwner)
        {
            Owner = newOwner;
            TimeDropped = DateTime.UtcNow;
            dbLocationName = null;
            LastSold = DateTime.UtcNow;
        }

        public void ChangeGameMode(int newGameMode)
        {
            PvPEnabled = newGameMode;
        }

    }
}