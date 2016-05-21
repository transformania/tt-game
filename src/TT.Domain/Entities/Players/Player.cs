using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.NPCs;
using TT.Domain.ViewModels;

namespace TT.Domain.Entities.Players
{
    public class Player : Entity<int>
    {
        public User User { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        [Column("dbLocationName")]
        public string Location { get; protected set; }
        public string Form { get; protected set; } // TODO:  Convert to FK to FormSource (DbStaticForms)
        public ICollection<Items.Item> Items { get; protected set; } 

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

        public void DropAllItems()
        {
            foreach (Items.Item i in this.Items)
            {
                i.Drop(this);
            }
        }

        public void ChangeForm(FormSource form)
        {
            this.FormSource = form;
            this.Form = form.dbName; // keep here for legacy purposes
            this.Gender = form.Gender;
            this.ForceWithinBounds();
        }

        public void ReadjustMaxes(BuffBox buffs)
        {
            this.MaxHealth = Convert.ToDecimal(GetWillpowerBaseByLevel(this.Level)) * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
            this.MaxMana = Convert.ToDecimal(GetManaBaseByLevel(this.Level)) * (1.0M + (buffs.ManaBonusPercent() / 100.0M));
            this.ForceWithinBounds();
        }

        private float GetManaBaseByLevel(int level)
        {
            float manaBase = 5 * (level - 1) + 50;
            return manaBase;
        }

        private static float GetWillpowerBaseByLevel(int level)
        {
            float willpowerBase = 15 * (level - 1) + 100;
            return willpowerBase;
        }

        private void ForceWithinBounds()
        {
            if (this.MaxHealth < 1) this.MaxHealth = 1;
            if (this.MaxMana < 1) this.MaxMana = 1;

            if (this.Health > this.MaxHealth) this.Health = this.MaxHealth;
            if (this.Mana > this.MaxMana) this.Mana = this.MaxHealth;

            if (this.Health < 0) this.Health = 0;
            if (this.Mana < 0) this.Mana = 0;
        }
    }
}