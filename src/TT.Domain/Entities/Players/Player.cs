using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.Effects;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.NPCs;
using TT.Domain.Entities.Skills;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Entities.Players
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
        public ICollection<Items.Item> Items { get; protected set; } 
        public ICollection<Effect> Effects { get; protected set; }
        public ICollection<Skill> Skills { get; protected set; }
        public ICollection<PlayerLog> PlayerLogs { get; protected set; }
        public ICollection<TFEnergy> TFEnergies { get; protected set; }

        public FormSource FormSource { get; protected set; }

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
        public int Covenant { get; protected set; } // TODO:  Convert this to nullable FK referencing Covenants table
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

        private Player()
        {
            Items = new List<Items.Item>();
            Skills = new List<Skill>();
            PlayerLogs = new List<PlayerLog>();
            Effects = new List<Effect>();
        }

        public static Player Create(User user, NPC npc, FormSource form, CreatePlayer cmd)
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
                Covenant = cmd.Covenant,
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
            if (DonatorLevel >= 2)
            {
                return FirstName + " '" + Nickname + "' " + LastName;
            }
            else
            {
                return FirstName + " " + LastName;
            }
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
            foreach (Items.Item i in Items)
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
            if (Mana > MaxMana) Mana = MaxHealth;

            if (Health < 0) Health = 0;
            if (Mana < 0) Mana = 0;
        }
    }
}