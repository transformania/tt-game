using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsEquipped { get; protected set; }
        public int TurnsUntilUse { get; protected set; }
        public int Level { get; protected set; }
        public DateTime TimeDropped { get; protected set; }
        public bool EquippedThisTurn { get; protected set; }
        public int PvPEnabled { get; protected set; }
        public bool IsPermanent { get; protected set; }
        public DateTime LastSouledTimestamp { get; protected set; }
        public DateTime LastSold { get; protected set; }
        public Item EmbeddedOnItem { get; protected set; }
        public ICollection<Item> Runes { get; protected set; }
        public Player SoulboundToPlayer { get; protected set; }
        public bool ConsentsToSoulbinding { get; protected set; }

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
                dbLocationName = cmd.dbLocationName,
                IsEquipped = cmd.IsEquipped,
                TurnsUntilUse = cmd.TurnsUntilUse,
                Level = cmd.Level,
                TimeDropped = cmd.TimeDropped,
                EquippedThisTurn = cmd.EquippedThisTurn,
                PvPEnabled = cmd.PvPEnabled,
                IsPermanent = cmd.IsPermanent,
                LastSouledTimestamp = cmd.LastSouledTimestamp,
                LastSold = cmd.LastSold
            };

            if (owner != null && owner.BotId != AIStatics.ActivePlayerBotId)
            {
                newItem.LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
            }

            return newItem;
        }

        public static Item CreateFromPlayer(Player formerPlayer, ItemSource itemSource, Player attacker)
        {
            var newItem = new Item()
            {
               
                FormerPlayer = formerPlayer,
                ItemSource = itemSource,
                IsEquipped = false,
                TurnsUntilUse = 0,
                Level = formerPlayer.Level,
                TimeDropped = DateTime.UtcNow,
                EquippedThisTurn = false,
               
                LastSold = DateTime.UtcNow
            };

            if (formerPlayer.BotId == AIStatics.ActivePlayerBotId)
            {
                newItem.IsPermanent = false;
                newItem.LastSouledTimestamp = DateTime.UtcNow;
                newItem.ConsentsToSoulbinding = false;
                newItem.SetGameMode(attacker);
            }
            else
            {
                newItem.IsPermanent = true;
                newItem.LastSouledTimestamp = DateTime.UtcNow.AddYears(-1);
                newItem.PvPEnabled = (int)GameModeStatics.GameModes.Any;
                newItem.ConsentsToSoulbinding = true;
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
                IsPermanent = itemSource.IsPermanentFromCreation(),
                ItemSource = itemSource,
                PvPEnabled = (int)GameModeStatics.GameModes.Any,
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

        public Item Drop(Player owner, string locationOverride = null)
        {
            Owner = null;
            dbLocationName = String.IsNullOrEmpty(locationOverride) ? owner.Location : locationOverride;
            IsEquipped = false;
            var now = DateTime.UtcNow;
            TimeDropped = now;

            foreach (var rune in this.Runes)
            {
                rune.dbLocationName = String.Empty;
                rune.IsEquipped = true;
                rune.Owner = null;
                rune.TimeDropped = now;
            }

            return this;
        }

        public Item ChangeOwner(Player newOwner, int? gameModeFlag = null)
        {

            var now = DateTime.UtcNow;
            TimeDropped = now;
            LastSold = now;

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
                rune.TimeDropped = now;
                rune.LastSold = now;
            }

            if (!newOwner.Items.Contains(this))
            {
                newOwner.Items.Add(this);
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
            rune.EquippedThisTurn = true;
        }

        public void RemoveRunes()
        {
            foreach (var rune in this.Runes.ToList())
            {
                RemoveRune(rune);
            }
            this.Runes.Clear();
        }

        public void RemoveRune(Item rune)
        {
            rune.EmbeddedOnItem = null;
            rune.IsEquipped = false;
            rune.EquippedThisTurn = true;

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

            this.Runes.Remove(rune);
        }

        public void SetEquippedThisTurn(bool equippedThisTurn)
        {
            this.EquippedThisTurn = equippedThisTurn;
        }

        public void SetLocation(string location)
        {
            this.dbLocationName = location;
        }

        public void SetFormerPlayer(Player player)
        {
            this.FormerPlayer = player;
        }

        public void SoulbindToPlayer(Player player)
        {
            this.SoulboundToPlayer = player;
            if (this.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
            {
                this.FormerPlayer.PlayerLogs.Add(PlayerLog.Create(this.FormerPlayer, $"{player.GetFullName()} has soulbound you!  No other players will be able to claim you as theirs.", DateTime.UtcNow, true));
            }

        }

        private void SetGameMode(Player player)
        {
            if (player == null)
            {
                this.PvPEnabled = (int) GameModeStatics.GameModes.Any;
            }
            else
            {
                this.PvPEnabled = player.GameMode;
                if (this.PvPEnabled == (int)GameModeStatics.GameModes.Superprotection)
                {
                    this.PvPEnabled = (int) GameModeStatics.GameModes.Superprotection;
                }
            }
        }

        public void SetSoulbindingConsent(bool isConsenting)
        {
            ConsentsToSoulbinding = isConsenting;
        }

    }
}