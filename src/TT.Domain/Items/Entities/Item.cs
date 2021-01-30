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
            var newItem = new Item
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
            var newItem = new Item
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
            dbLocationName = string.IsNullOrEmpty(locationOverride) ? owner.Location : locationOverride;
            IsEquipped = false;
            var now = DateTime.UtcNow;
            TimeDropped = now;

            return this;
        }

        public Item ChangeOwner(Player newOwner, int? gameModeFlag = null)
        {

            var now = DateTime.UtcNow;
            TimeDropped = now;
            LastSold = now;

            if (Owner != null)
            {
                Owner.Items.Remove(this);
            }

            // always equip pets immediately, otherwise unequip
            IsEquipped = ItemSource.ItemType == PvPStatics.ItemType_Pet;

            Owner = newOwner;
            dbLocationName = string.Empty;

            var gameModeValue = gameModeFlag ?? PvPEnabled;

            PvPEnabled = gameModeValue;
            
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

            var validTypes = new List<string>
            {
                PvPStatics.ItemType_Pants,
                PvPStatics.ItemType_Shirt,
                PvPStatics.ItemType_Hat,
                PvPStatics.ItemType_Shoes,
                PvPStatics.ItemType_Accessory,
                PvPStatics.ItemType_Pet,
                PvPStatics.ItemType_Underpants,
                PvPStatics.ItemType_Undershirt
            };

            return validTypes.Contains(ItemSource.ItemType);
        }

        public bool HasRoomForRunes()
        {
            if (ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                if (Runes.Count >= 2)
                {
                    return false;

                }
            }
            else
            {
                if (Runes.Count >= 1)
                {
                    return false;

                }
            }
            return true;
        }

        public bool IsOfHighEnoughLevelForRune(Item rune)
        {
            return Level >= rune.ItemSource.RuneLevel;
        }

        /// <summary>
        /// Attach a rune on to this item
        /// </summary>
        /// <param name="rune"></param>
        public void AttachRune(Item rune)
        {
            Runes.Add(rune);
            rune.EmbeddedOnItem = this;
            rune.Owner = null;
            rune.dbLocationName = string.Empty;
            rune.EquippedThisTurn = true;
        }

        public void RemoveRunes()
        {
            foreach (var rune in Runes.ToList())
            {
                RemoveRune(rune);
            }
            Runes.Clear();
        }

        public void RemoveRune(Item rune)
        {
            rune.EmbeddedOnItem = null;
            rune.EquippedThisTurn = true;

            if (Owner == null || Owner.BotId == AIStatics.WuffieBotId)
            {
                rune.Owner = null;
                rune.PvPEnabled = (int) GameModeStatics.GameModes.Any;
                rune.dbLocationName = dbLocationName;
                rune.TimeDropped = DateTime.Now;
            }
            else
            {
                rune.Owner = Owner;
                rune.PvPEnabled = Owner.BotId == AIStatics.ActivePlayerBotId
                    ? Owner.GameMode
                    : (int) GameModeStatics.GameModes.Any;
                rune.dbLocationName = string.Empty;
            }

            if (rune.PvPEnabled == (int)GameModeStatics.GameModes.Superprotection)
            {
                rune.PvPEnabled = (int) GameModeStatics.GameModes.Protection;
            }

            Runes.Remove(rune);
        }

        public void SetEquippedThisTurn(bool equippedThisTurn)
        {
            EquippedThisTurn = equippedThisTurn;
        }

        public void SetLocation(string location)
        {
            dbLocationName = location;
        }

        public void SetFormerPlayer(Player player)
        {
            FormerPlayer = player;
        }

        public void SoulbindToPlayer(Player player)
        {
            SoulboundToPlayer = player;
            if (FormerPlayer != null)
            {
                if (player != null)
                {

                    if (FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                    {
                        FormerPlayer.PlayerLogs.Add(PlayerLog.Create(FormerPlayer,
                            $"{player.GetFullName()} has soulbound you!  No other players will be able to claim you as theirs.",
                            DateTime.UtcNow, true));
                    }
                }
                else
                {
                    FormerPlayer.PlayerLogs.Add(PlayerLog.Create(FormerPlayer,
                        "Your past owner has lost the last of their own humanity, shattering the soulbinding between you.",
                        DateTime.UtcNow, true));
                }
            }
        }

        private void SetGameMode(Player player)
        {
            if (player == null || player.BotId != AIStatics.ActivePlayerBotId)
            {
                PvPEnabled = (int) GameModeStatics.GameModes.Any;
            }
            else
            {
                PvPEnabled = player.GameMode;
                if (PvPEnabled == (int)GameModeStatics.GameModes.Superprotection)
                {
                    PvPEnabled = (int) GameModeStatics.GameModes.Protection;
                }
            }
        }

        public void SetSoulbindingConsent(bool isConsenting)
        {
            ConsentsToSoulbinding = isConsenting;
        }

    }
}