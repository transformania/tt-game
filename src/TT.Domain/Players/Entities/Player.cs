using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TT.Domain.AI.Entities;
using TT.Domain.Covenants.Entities;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities;
using TT.Domain.Entities.MindControl;
using TT.Domain.Entities.Skills;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Forms.Entities;
using TT.Domain.Identity.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Entities
{

    public class Player : Entity<int>
    {
        public User User { get; protected set; }
        public string MembershipId { get; protected set; }

        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        [Column("dbLocationName")]
        public string Location { get; protected set; }
        public string Form { get; protected set; } // TODO:  Convert to FK to FormSource (DbStaticForms)
        public ICollection<Item> Items { get; protected set; } 
        public ICollection<Effect> Effects { get; protected set; }
        public ICollection<Skill> Skills { get; protected set; }
        public ICollection<PlayerLog> PlayerLogs { get; protected set; }
        public ICollection<TFEnergy> TFEnergies { get; protected set; }
        public ICollection<TFEnergy> TFEnergiesCast { get; protected set; }

        public ICollection<VictimMindControl> VictimMindControls { get; protected set; }

        public FormSource FormSource { get; protected set; }

        public Item Item { get; protected set; }

        public decimal Health { get; protected set; }
        public decimal MaxHealth { get; protected set; }
        public decimal Mana { get; protected set; }
        public decimal MaxMana { get; protected set; }

        public decimal XP { get; protected set; }
        public int Level { get; protected set; }
        public int TimesAttackingThisUpdate { get; protected set; }

        public decimal ActionPoints { get; protected set; }
        public decimal ActionPoints_Refill { get; protected  set; }

        public string Gender { get; protected set; } // TODO:  remove this as gender should be part of FormSource
        public string Mobility { get; protected set; } // TODO:  remove this as gender should be part of FormSource

        public int BotId { get; protected set; } // TODO:  convert this to a nullable FK referencing NPC table
        public NPC NPC { get; protected set; }

        public bool MindControlIsActive { get; protected set; }

        public string IpAddress { get; protected set; }

        public DateTime LastActionTimestamp { get; protected set; }
        public DateTime LastCombatTimestamp { get; protected set; }
        public DateTime LastCombatAttackedTimestamp { get; protected set; }

        public bool FlaggedForAbuse { get; protected set; }
        public int UnusedLevelUpPerks { get; protected set; }

        public int GameMode { get; protected set; }
        public bool InRP { get; protected set; }

        public int CleansesMeditatesThisRound { get; protected set; }
        public decimal Money { get; protected set; }
        public Covenant Covenant { get; protected set; }
        public string OriginalForm { get; protected set; } // TODO:  Convert to FK to FormSource (DbStaticForms)

        public decimal PvPScore { get; protected set; }
        public int DonatorLevel { get; protected set; }
        public string Nickname { get; protected  set; }
        public DateTime OnlineActivityTimestamp { get; protected set; }
        public bool IsBannedFromGlobalChat { get; protected set; }
        public string ChatColor { get; protected set; }
        public int ShoutsRemaining { get; protected set; }
        public int InDuel { get; protected set; } // TODO:  Convert to nullable FK to Duels
        public int InQuest { get; protected set; } // TODO:  Convert to nullable FK to Quests
        public int InQuestState { get; protected set; } // TODO:  Convert to nullable FK to QuestStates
        public int ItemsUsedThisTurn { get; protected set; }

        public Covenant CovenantLed { get; protected set; }

        public InanimateXP ItemXP { get; protected set; } 

        private Player()
        {
            Items = new List<Item>();
            Skills = new List<Skill>();
            TFEnergies = new List<TFEnergy>();
            TFEnergiesCast = new List<TFEnergy>();
            PlayerLogs = new List<PlayerLog>();
            Effects = new List<Effect>();
            VictimMindControls = new List<VictimMindControl>();
        }

        public static Player Create(User user, NPC npc, FormSource form, CreatePlayer cmd, Covenant covenant)
        {
            return new Player
            {
                User = user,
                FirstName = cmd.FirstName,
                LastName = cmd.LastName,
                Location = cmd.Location,
                Form = cmd.Form,
                FormSource = form,
                Health = cmd.Health,
                MaxHealth = cmd.MaxHealth,
                Mana = cmd.Mana,
                MaxMana = cmd.MaxMana,
                Level = cmd.Level,
                XP = cmd.XP,
                TimesAttackingThisUpdate = cmd.TimesAttackingThisUpdate,
                ActionPoints = cmd.ActionPoints,
                ActionPoints_Refill = cmd.ActionPoints_Refill,
                Gender = cmd.Gender,
                Mobility = cmd.Mobility,
                BotId = cmd.BotId,
                NPC = npc,
                MindControlIsActive = cmd.MindControlIsActive,
                IpAddress =  cmd.IpAddress,
                LastActionTimestamp = cmd.LastActionTimestamp,
                LastCombatTimestamp = cmd.LastCombatTimestamp,
                LastCombatAttackedTimestamp = cmd.LastCombatAttackedTimestamp,
                FlaggedForAbuse = cmd.FlaggedForAbuse,
                UnusedLevelUpPerks = cmd.UnusedLevelUpPerks,
                GameMode = cmd.GameMode,
                InRP = cmd.InRP,
                CleansesMeditatesThisRound = cmd.CleansesMeditatesThisRound,
                Money = cmd.Money,
                Covenant = covenant,
                OriginalForm = cmd.OriginalForm,
                PvPScore = cmd.PvPScore,
                DonatorLevel = cmd.DonatorLevel,
                Nickname = cmd.Nickname,
                OnlineActivityTimestamp = cmd.OnlineActivityTimestamp,
                IsBannedFromGlobalChat = cmd.IsBannedFromGlobalChat,
                ChatColor = cmd.ChatColor,
                ShoutsRemaining = cmd.ShoutsRemaining,
                InDuel = cmd.InDuel,
                InQuest = cmd.InQuest,
                InQuestState = cmd.InQuestState,
                ItemsUsedThisTurn = cmd.ItemsUsedThisTurn

            };
        }

        public string GetFullName()
        {
            return DonatorLevel < 2 || Nickname.IsNullOrEmpty()
                ? $"{FirstName} {LastName}"
                : $"{FirstName} \'{Nickname}\' {LastName}";
        }

        public void AddHealth(decimal amount)
        {
            Health += amount;
            ForceWithinBounds();
        }

        public void AddMana(decimal amount)
        {
            Mana += amount;
            ForceWithinBounds();
        }

        public void DropAllItems()
        {
            foreach (Item i in Items)
            {
                i.Drop(this);
            }
        }

        public void ChangeForm(FormSource form)
        {
            FormSource = form;
            Form = form.dbName; // keep here for legacy purposes
            Gender = form.Gender;
            Mobility = form.MobilityType;
            ForceWithinBounds();
        }

        public void ReadjustMaxes(BuffBox buffs)
        {
            MaxHealth = Convert.ToDecimal(GetWillpowerBaseByLevel(Level)) * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
            MaxMana = Convert.ToDecimal(GetManaBaseByLevel(Level)) * (1.0M + (buffs.ManaBonusPercent() / 100.0M));
            ForceWithinBounds();
        }

        /// <summary>
        /// Sets the player's DonatorTier to the specified tier and creates a new message in their logs informing them such.
        /// </summary>
        /// <param name="tier">The new tier to set the player to</param>
        public void SetTier(int tier)
        {
            DonatorLevel = tier;
            if (tier > 0)
            {
                PlayerLogs.Add(PlayerLog.Create(this, "<b>An admin has set your donator status to Tier " + tier + ".  <span class='good'>Thank you for supporting Transformania Time!</span></b>", DateTime.UtcNow, true));
            }
            else
            {
                PlayerLogs.Add(PlayerLog.Create(this, "<b>An admin has set your donator status to Tier " + tier + ".</b>", DateTime.UtcNow, true));
            }
            
        }

        /// <summary>
        /// Sets the player's location to the location provided.
        /// </summary>
        /// <param name="location"></param>
        public void SetLocation(string location)
        {
            Location = location;

        }

        /// <summary>
        /// Adds a new player log message of specified importance
        /// </summary>
        /// <param name="logMessage">The log message visible to the player</param>
        /// <param name="isImportant">Whether or not the log is important and should continually appear in the player's logs until dismissed.</param>
        public void AddLog(string logMessage, bool isImportant)
        {
            PlayerLogs.Add(PlayerLog.Create(this, logMessage, DateTime.UtcNow, isImportant));
        }

        public void ChangeMoney(int amount)
        {
            Money += amount;
        }

        public void ChangeActionPoints(decimal amount)
        {
            ActionPoints += amount;
        }

        public void SetOnlineActivityToNow()
        {
            LastActionTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Have the player cleanse, restoring some willpower and reduce TF Energies, modified by the player's buffs
        /// </summary>
        /// <param name="buffs">Player's buffs</param>
        /// <returns>Cleanse amount</returns>
        public string Cleanse(BuffBox buffs)
        {

            CleansesMeditatesThisRound++;
            ActionPoints -= PvPStatics.CleanseCost;
            Mana -= PvPStatics.CleanseManaCost;
            LastActionTimestamp = DateTime.UtcNow;

            var result = "";

            var cleanseBonusTFEnergyRemovalPercent = buffs.CleanseExtraTFEnergyRemovalPercent() + PvPStatics.CleanseTFEnergyPercentDecrease;
            var cleanseWPRestore = PvPStatics.CleanseHealthRestoreBase + buffs.CleanseExtraHealth() + Level;

            if (cleanseWPRestore <= 0)
            {
                result = "You try to cleanse, but due to the magical effects on your body you fail to restore any willpower.";
            }
            else
            {
                AddHealth(cleanseWPRestore);
                result = $"You quickly cleanse, restoring {cleanseWPRestore} willpower.";
            }

            if (cleanseBonusTFEnergyRemovalPercent > 0)
            {
                CleanseTFEnergies(buffs);
            }

            if (BotId == AIStatics.ActivePlayerBotId)
            {
                var location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == Location);
                AddLog($"You cleansed at {location.Name}.", false);
            }

            return result;

        }

        public string Meditate(BuffBox buffs)
        {
            CleansesMeditatesThisRound++;
            ActionPoints -= PvPStatics.MeditateCost;
            LastActionTimestamp = DateTime.UtcNow;

            var result = "";

            var meditateManaRestore = PvPStatics.MeditateManaRestoreBase + buffs.MeditationExtraMana() + Level;

            if (meditateManaRestore < 0)
            {
                result = "You try to meditate, but due to the magical effects on your body you fail to restore any mana.";
            }
            else
            {
                result = "You quickly meditate, restoring " + meditateManaRestore + " mana.";
                AddMana(meditateManaRestore);
            }

            if (BotId == AIStatics.ActivePlayerBotId)
            {
                var location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == Location);
                AddLog($"You meditated at {location.Name}.", false);
            }

            return result;
        }
         
        /// <summary>
        /// Removes a percentage of TF energy that this player has, determined by the stats passed in
        /// </summary>
        /// <param name="buffs">Buffs owned by the player</param>
        public void CleanseTFEnergies(BuffBox buffs)
        {
            var cleansePercentage = buffs.CleanseExtraTFEnergyRemovalPercent() + PvPStatics.CleanseTFEnergyPercentDecrease;

            foreach (TFEnergy energy in TFEnergies)
            {
                var newValue = energy.Amount * (1 - (cleansePercentage / 100.0M));
                energy.SetAmount(newValue);
            }

        }

        public void ChangeRPMode(bool inRP)
        {
            InRP = inRP;
        }

        public void ChangeGameMode(int gameMode)
        {
            GameMode = gameMode;
        }

        public void Shout(string message)
        {
            ShoutsRemaining--;
            var location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == Location);
            AddLog($"You shouted '{message}' at {location.Name}.", false);
        }

        public void UpdateItemUseCounter(int amount)
        {
            ItemsUsedThisTurn += amount;
        }

        /// <summary>
        /// Returns the count of a particular type of item that this player is carrying.
        /// </summary>
        /// <param name="itemSourceId">ItemSource ID of the item being queried</param>
        /// <returns>the count of a particular type of item that this player is carrying</returns>
        public int GetCountOfItem(int itemSourceId)
        {
            return Items.Count(i => i.ItemSource.Id == itemSourceId);
        }

        /// <summary>
        /// Return the maximum amount of items a player is allowed to hold in their inventory.  Once this limit is reached, a player cannot pick up any new items.  If exceeded, the player is unable to move.
        /// </summary>
        /// <param name="buffs"></param>
        /// <returns></returns>
        public int GetMaxInventorySize(BuffBox buffs)
        {
            return (int)Math.Floor(buffs.ExtraInventorySpace()) + PvPStatics.MaxCarryableItemCountBase;
        }

        /// <summary>
        /// Returns whether or not the player is carrying too many items to be able to move.  This value does not count any items that are equipped.
        /// </summary>
        /// <param name="buffs"></param>
        /// <returns></returns>
        public bool IsCarryingTooMuchToMove(BuffBox buffs)
        {
            var carriedNonWornItems = this.Items.Count(i => !i.IsEquipped);
            return carriedNonWornItems >= GetMaxInventorySize(buffs) + 1;
        }

        /// <summary>
        /// Returns the count of items this player is carrying that are NOT equipped
        /// </summary>
        /// <returns></returns>
        public int GetCarriedItemCount()
        {
            return this.Items.Count(i => !i.IsEquipped);
        }

        /// <summary>
        /// Returns whether or not this player is not allowed to move due to having the Forced March mind control active on them.
        /// </summary>
        /// <returns></returns>
        public bool CantMoveBecauseOfForcedMarch()
        {
            return VictimMindControls.Any(v =>
                v.TurnsRemaining > 0 && v.Type == MindControlStatics.MindControl__Movement);
        }

        /// <summary>
        /// Gives this player a certain amount of a particular type of item
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="amount"></param>
        public void GiveItemsOfType(ItemSource itemSource, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var newItem = Item.Create(this, itemSource);
                Items.Add(newItem);
            }
        }

        public void GiveItem(Item item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Gives the player experience points (XP).  If they have enough to level up, they will do so and gain a levelup perk slot
        /// </summary>
        /// <param name="amount">Amount of XP to give player</param>
        public void AddXP(decimal amount)
        {
            var xpNeeded = (decimal) GetXPNeededForLevelUp();
            XP += amount;
            if (XP >= xpNeeded)
            {
                XP -= xpNeeded;
                Level++;
                UnusedLevelUpPerks++;
            }
        }

        /// <summary>
        /// Returns the most recent of the two timestamps of when the player has attacked or been attacked.
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastCombatTimestamp()
        {
            return LastCombatTimestamp > LastCombatAttackedTimestamp ? LastCombatTimestamp : LastCombatAttackedTimestamp;
        }

        private float GetManaBaseByLevel(int level)
        {
            float manaBase = 5 * (level - 1) + 50;
            return manaBase;
        }

        private static float GetWillpowerBaseByLevel(int level)
        {
            float willpowerBase = (float)(PvPStatics.LevelUpHealthMaxIncreasePerLevel * (level - 1) + 100);
            return willpowerBase;
        }

        /// <summary>
        /// Clamp WP and mana to no lower than 1 and no higher than the player's maximum WP and mana
        /// </summary>
        private void ForceWithinBounds()
        {
            if (MaxHealth < 1) MaxHealth = 1;
            if (MaxMana < 1) MaxMana = 1;

            if (Health > MaxHealth) Health = MaxHealth;
            if (Mana > MaxMana) Mana = MaxMana;

            if (Health < 0) Health = 0;
            if (Mana < 0) Mana = 0;
        }

        /// <summary>
        /// Return how much total XP is needed for the player to reach the next level based off a hyperbolic formula currently set to 11x^2+x*0+89 .  The number is rounded up to the nearest 10.
        /// </summary>
        /// <returns>XP needed for levelup</returns>
        public float GetXPNeededForLevelUp()
        {

            // WARNING:  There is a nearly identical method to this on PlayerProcedures with the same name.  Updates to the logic here
            // must also be done there to keep new code consistent with legacy code.

            float xp = 11 * Level * Level + 0 + 89;
            float leftover = xp % 10;

            xp = (float)Math.Round(xp / 10) * 10; // round to nearest 10

            if (leftover != 0)
            {
                xp += 10;
            }

            return xp;
        }

        /// <summary>
        /// Sets this player's number of action points
        /// </summary>
        /// <returns>Amount to set to</returns>
        public void SetActionPoints(decimal amount)
        {
            ActionPoints = amount;
        }

        public MoveLogBox MoveToAsAnimal(string destination)
        {

            var currentLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == this.Location);
            var nextLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == destination);

            this.Location = nextLocation.dbName;

            string leavingMessage = this.GetFullName() + " (feral) left toward " + nextLocation.Name;
            string enteringMessage = this.GetFullName() + " (feral) entered from " + currentLocation.Name;

            var playerLog = $"You moved from <b>{currentLocation.Name}</b> to <b>{nextLocation.Name}</b>.";

            AddLog(playerLog, false);
            this.User.AddStat(StatsProcedures.Stat__TimesMoved, 1);
            this.LastActionTimestamp = DateTime.UtcNow;

            var logBox = new MoveLogBox();
            logBox.SourceLocationLog = leavingMessage;
            logBox.DestinationLocationLog = enteringMessage;
            logBox.PlayerLog = playerLog;
            return logBox;
        }

        public MoveLogBox MoveTo(string destination, BuffBox buffs)
        {
            var currentLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == this.Location);
            var nextLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == destination);

            string leavingMessage = this.GetFullName() + " left toward " + nextLocation.Name;
            string enteringMessage = this.GetFullName() + " entered from " + currentLocation.Name;

            var playerLog = $"You moved from <b>{currentLocation.Name}</b> to <b>{nextLocation.Name}</b>.";

            int sneakLevel = this.CalculateSneakLevel(buffs);
            if (sneakLevel > 0)
            {
                playerLog += $" (Concealment lvl <b>{sneakLevel}</b>)";
            }

            this.Location = destination;
            AddLog(playerLog, false);
            var movementCost = PvPStatics.LocationMoveCost - buffs.MoveActionPointDiscount();
            this.ActionPoints -= movementCost;
            this.User.AddStat(StatsProcedures.Stat__TimesMoved, 1);
            this.LastActionTimestamp = DateTime.UtcNow;

            // set location of all owned items/pets to this to blank so they don't appear on the ground
            foreach (var item in this.Items)
            {
                item.SetLocation(String.Empty);
            }

            var logBox = new MoveLogBox();
            logBox.SourceLocationLog = leavingMessage;
            logBox.DestinationLocationLog = enteringMessage;
            logBox.PlayerLog = playerLog;
            logBox.ConcealmentLevel = sneakLevel;
            return logBox;
        }

        /// <summary>
        /// Returns a random value between 0 and 75 to determine the concealment level of a player's movement
        /// </summary>
        /// <param name="buffs"></param>
        /// <returns></returns>
        private int CalculateSneakLevel(BuffBox buffs)
        {
            var sneakLevel = (int)buffs.SneakPercent();
            if (sneakLevel < 0)
            {
                sneakLevel = -999;
            }
            else
            {
                sneakLevel -= (int)(new Random().NextDouble() * 75);
            }
            
            return sneakLevel;
        }

        /// <summary>
        /// Returns whether or not this player is in a location that is considered to be part of the PvP dungeon
        /// </summary>
        /// <returns></returns>
        public bool IsInDungeon()
        {
            if (this.Location.Contains("dungeon_"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetItem(Item item)
        {
            this.Item = item;
        }

    }
}
