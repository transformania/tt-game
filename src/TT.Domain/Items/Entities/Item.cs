using System;
using System.Collections.Generic;
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
        public Item EmbeddedOnItem { get; protected set; }
        public ICollection<Item> Runes { get; protected set; }

        private Item()
        {
            Runes = new List<Item>();
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

            foreach (var rune in this.Runes)
            {
                rune.dbLocationName = String.Empty;
                rune.IsEquipped = true;
                rune.Owner = null;
            }

            return this;
        }

        public Item ChangeOwner(Player newOwner, int? gameModeFlag = null)
        {

            TimeDropped = DateTime.UtcNow;
            LastSold = DateTime.UtcNow;

            if (this.Owner != null)
            {
                this.Owner.Items.Remove(this);
            }

            // always equip pets immediately, otherwise unequip
            this.IsEquipped = this.ItemSource.ItemType == PvPStatics.ItemType_Pet;

            Owner = newOwner;
            dbLocationName = String.Empty;

            var gameModeValue = gameModeFlag != null ? gameModeFlag.Value : PvPEnabled;

            PvPEnabled = gameModeValue;
            
            foreach (var rune in this.Runes)
            {
                rune.Owner = newOwner;
                rune.IsEquipped = true;
                rune.dbLocationName = String.Empty;
                rune.PvPEnabled = gameModeValue;
            }

            return this;
        }

        public void ChangeGameMode(int newGameMode)
        {
            PvPEnabled = newGameMode;
        }

        /// <summary>
        /// Returns whether or not this item is a valid type to have runes attached
        /// </summary>
        /// <returns></returns>
        public bool CanAttachRunesToThisItemType()
        {

            var validTypes = new List<string>();
            validTypes.Add(PvPStatics.ItemType_Pants);
            validTypes.Add(PvPStatics.ItemType_Shirt);
            validTypes.Add(PvPStatics.ItemType_Hat);
            validTypes.Add(PvPStatics.ItemType_Shoes);
            validTypes.Add(PvPStatics.ItemType_Accessory);
            validTypes.Add(PvPStatics.ItemType_Pet);
            validTypes.Add(PvPStatics.ItemType_Underpants);
            validTypes.Add(PvPStatics.ItemType_Undershirt);

            if (!validTypes.Contains(this.ItemSource.ItemType))
            {
                return false;
            }

            

            return true;
        }

        public bool HasRoomForRunes()
        {
            if (this.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                if (this.Runes.Count >= 2)
                {
                    return false;

                }
            }
            else
            {
                if (this.Runes.Count >= 1)
                {
                    return false;

                }
            }
            return true;
        }

        public bool IsOfHighEnoughLevelForRune(Item rune)
        {
            return this.Level >= rune.ItemSource.RuneLevel;
        }

        /// <summary>
        /// Attach a rune on to this item
        /// </summary>
        /// <param name="rune"></param>
        public void AttachRune(Item rune)
        {
            this.Runes.Add(rune);
            rune.EmbeddedOnItem = this;
            rune.IsEquipped = true;
            rune.Owner = this.Owner;
        }

        public void RemoveRunes()
        {
            foreach (var rune in this.Runes)
            {
                rune.EmbeddedOnItem = null;
                rune.IsEquipped = false;

                if (this.Owner != null)
                {
                    rune.Owner = this.Owner;
                    rune.dbLocationName = String.Empty;
                }
                else
                {
                    rune.Owner = null;
                    rune.dbLocationName = this.dbLocationName;
                }

            }
            this.Runes.Clear();
        }

        public void SetLocation(string location)
        {
            this.dbLocationName = location;
        }

        public void SetFormerPlayer(Player player)
        {
            this.FormerPlayer = player;
        }

    }
}