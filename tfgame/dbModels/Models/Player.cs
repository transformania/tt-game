using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Player
    {
        public int Id {get; set;}
        public int MembershipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string dbLocationName { get; set; }
        public string Form { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
        public decimal ActionPoints { get; set; }
        public decimal ActionPoints_Refill { get; set; }
        public decimal ResistanceModifier { get; set; }
        public string Gender { get; set; }
        public string Mobility { get; set; }
        public int IsItemId { get; set; }
        public int IsPetToId { get; set; }
        public decimal XP { get; set; }
        public int Level { get; set; }
        public int TimesAttackingThisUpdate { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
        public bool FlaggedForAbuse { get; set; }
        public int UnusedLevelUpPerks { get; set; }
        public bool InPvP { get; set; }
        public bool NonPvP_GameoverSpellsAllowed { get; set; }
        public DateTime NonPvP_GameOverSpellsAllowedLastChange { get; set; }
        public bool InRP { get; set; }
        public int CleansesMeditatesThisRound { get; set; }
        public decimal Money { get; set; }
        public int Covenant { get; set; }
        public string OriginalForm { get; set; }
        public decimal PvPScore { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
        public DateTime OnlineActivityTimestamp { get; set; }
    }

    public class Player_VM
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string dbLocationName { get; set; }
        public string Form { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
        public decimal ActionPoints { get; set; }
        public decimal ActionPoints_Refill { get; set; }
        public decimal ResistanceModifier { get; set; }
        public string Gender { get; set; }
        public string Mobility { get; set; }
        public int IsItemId { get; set; }
        public int IsPetToId { get; set; }
        public decimal XP { get; set; }
        public int Level { get; set; }
        public int TimesAttackingThisUpdate { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
        public bool FlaggedForAbuse { get; set; }
        public int UnusedLevelUpPerks { get; set; }
        public bool InPvP { get; set; }
        public bool NonPvP_GameoverSpellsAllowed { get; set; }
        public DateTime NonPvP_GameOverSpellsAllowedLastChange { get; set; }
        public bool InRP { get; set; }
        public int CleansesMeditatesThisRound { get; set; }
        public decimal Money { get; set; }
        public int Covenant { get; set; }
        public string OriginalForm { get; set; }
        public decimal PvPScore { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
        public DateTime OnlineActivityTimestamp { get; set; }

        public Player ToDbPlayer()
        {
            Player output = new Player{
                Id = this.Id,
                MembershipId = this.MembershipId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                dbLocationName = this.dbLocationName,
                Form = this.Form,
                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Mana = this.Mana,
                MaxMana = this.MaxMana,
                ActionPoints = this.ActionPoints,
                ActionPoints_Refill = this.ActionPoints_Refill,
                ResistanceModifier = this.ResistanceModifier,
                Gender = this.Gender,
                Mobility = this.Mobility,
                IsItemId = this.IsItemId,
                IsPetToId = this.IsPetToId,
                XP = this.XP,
                Level = this.Level,
                TimesAttackingThisUpdate = this.TimesAttackingThisUpdate,
                IpAddress = this.IpAddress,
                LastActionTimestamp = this.LastActionTimestamp,
                LastCombatTimestamp = this.LastCombatTimestamp,
                FlaggedForAbuse = this.FlaggedForAbuse,
                UnusedLevelUpPerks = this.UnusedLevelUpPerks,
                InPvP = this.InPvP,
                NonPvP_GameoverSpellsAllowed = this.NonPvP_GameoverSpellsAllowed,
                NonPvP_GameOverSpellsAllowedLastChange = this.NonPvP_GameOverSpellsAllowedLastChange,
                InRP = this.InRP,
                CleansesMeditatesThisRound = this.CleansesMeditatesThisRound,
                Money = this.Money,
                Covenant = this.Covenant,
                OriginalForm = this.OriginalForm,
                PvPScore = this.PvPScore,
                DonatorLevel = this.DonatorLevel,
                Nickname = this.Nickname,
                OnlineActivityTimestamp = this.OnlineActivityTimestamp,

            };
            return output;
        }
    }
}