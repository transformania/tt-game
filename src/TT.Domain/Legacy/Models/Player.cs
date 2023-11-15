using System;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TT.Domain.Procedures;
using System.Collections.Generic;
using TT.Domain.Players.Commands;
using static TT.Domain.Models.PlayerDescriptorStatics;

namespace TT.Domain.Models
{
    public class Player
    {
        public int Id { get; set; }
        [Index("IX_MembershipIdAndCovenant", 1)]
        [Index("IX_MembershipIdAndInPvP", 1)]
        [Index]
        [StringLength(128)]
        public string MembershipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string dbLocationName { get; set; }
        public int FormSourceId { get; set; }

        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
        public int ExtraInventory { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SneakPercent { get; set; }
        public decimal ActionPoints { get; set; }
        public decimal ActionPoints_Refill { get; set; }
        public string Gender { get; set; }
        public string Mobility { get; set; }

        public int BotId { get; set; }
        public bool MindControlIsActive { get; set; }

        public decimal XP { get; set; }
        public int Level { get; set; }
        public int TimesAttackingThisUpdate { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
        public DateTime LastCombatAttackedTimestamp { get; set; }
        public bool FlaggedForAbuse { get; set; }
        public int UnusedLevelUpPerks { get; set; }
        [Index("IX_MembershipIdAndInPvP", 2)]
        public int GameMode { get; set; }
        public bool InRP { get; set; }
        public bool InHardmode { get; set; }
        public int CleansesMeditatesThisRound { get; set; }
        public decimal Money { get; set; }
        [Index("IX_MembershipIdAndCovenant", 2)]
        public int? Covenant { get; set; }
        public int OriginalFormSourceId { get; set; }
        public string OriginalFirstName { get; set; }
        public string OriginalLastName { get; set; }
        public bool HasSelfRenamed { get; set; }
        public decimal PvPScore { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
        public DateTime OnlineActivityTimestamp { get; set; }
        public bool IsBannedFromGlobalChat { get; set; }
        public string ChatColor { get; set; }
        public int ShoutsRemaining { get; set; }
        public int InDuel { get; set; }
        public int InQuest { get; set; }
        public int InQuestState { get; set; }
        public int ItemsUsedThisTurn { get; set; }
        public bool FriendOnlyMessages { get; set; }

        public string GetFullName()
        {
            if (this.DonatorLevel >= 2 && !this.Nickname.IsNullOrEmpty())
            {
                return this.FirstName + " '" + this.Nickname + "' " + this.LastName;
            }
            else
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        public bool IsUsingOriginalName()
        {
            return FirstName == OriginalFirstName && LastName == OriginalLastName;
        }

        public void NormalizeHealthMana()
        {

            if (this.Health < 0)
            {
                this.Health = 0;
            }
            else if (this.Health > this.MaxHealth)
            {
                this.Health = this.MaxHealth;
            }
            if (this.Mana < 0)
            {
                this.Mana = 0;
            }
            else if (this.Mana > this.MaxMana)
            {
                this.Mana = this.MaxMana;
            }
        }

        public void ReadjustMaxes(BuffBox buffs)
        {
            // readjust this health/mana by grabbing base amount plus effects from buffs
            this.MaxHealth = Convert.ToDecimal(PlayerProcedures.GetWillpowerBaseByLevel(this.Level)) * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
            this.MaxMana = Convert.ToDecimal(PlayerProcedures.GetManaBaseByLevel(this.Level)) * (1.0M + (buffs.ManaBonusPercent() / 100.0M));


            // keep this's health within proper bounds
            if (this.MaxHealth < 1)
            {
                this.MaxHealth = 1;
            }

            if (this.MaxMana < 1)
            {
                this.MaxMana = 1;
            }


            if (this.Health > this.MaxHealth)
            {
                this.Health = this.MaxHealth;
            }
            if (this.Mana > this.MaxMana)
            {
                this.Mana = this.MaxMana;
            }
            if (this.Health < 0)
            {
                this.Health = 0;
            }
            if (this.Mana < 0)
            {
                this.Mana = 0;
            }

            this.ExtraInventory = buffs.ExtraInventorySpace();
            SneakPercent = buffs.SneakPercent();
            MoveActionPointDiscount = buffs.MoveActionPointDiscount();

        }

        public DateTime GetLastCombatTimestamp()
        {
            if (this.LastCombatTimestamp > this.LastCombatAttackedTimestamp)
            {
                return this.LastCombatTimestamp;
            }
            else
            {
                return this.LastCombatAttackedTimestamp;
            }
        }

        public bool IsInDungeon()
        {
            if (this.dbLocationName.Contains("dungeon_"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DonatorGetsNickname()
        {
            return DonatorLevel >= 2;
        }

        public bool DonatorGetsMessagesRewards()
        {
            return DonatorLevel >= 3;
        }

        public bool WillGoOverMoneyLimitIfPaid(decimal amount)
        {
            return this.Money + amount > PvPStatics.MaxMoney;
        }

    }

    public class Player_VM
    {
        public int Id { get; set; }
        public string MembershipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OriginalFirstName { get; set; }
        public string OriginalLastName { get; set; }
        public string dbLocationName { get; set; }
        public int FormSourceId { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
        public int ExtraInventory { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SneakPercent { get; set; }
        public bool HasSelfRenamed { get; set; }
        public decimal ActionPoints { get; set; }
        public decimal ActionPoints_Refill { get; set; }
        public string Gender { get; set; }
        public string Mobility { get; set; }
        public int BotId { get; set; }

        public bool MindControlIsActive { get; set; }

        public decimal XP { get; set; }
        public int Level { get; set; }
        public int TimesAttackingThisUpdate { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
        public DateTime LastCombatAttackedTimestamp { get; set; }
        public bool FlaggedForAbuse { get; set; }
        public int UnusedLevelUpPerks { get; set; }
        public int GameMode { get; set; }
        public bool InRP { get; set; }
        public bool InHardmode { get; set; }
        public int CleansesMeditatesThisRound { get; set; }
        public decimal Money { get; set; }
        public int? Covenant { get; set; }
        public int OriginalFormSourceId { get; set; }
        public decimal PvPScore { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
        public DateTime OnlineActivityTimestamp { get; set; }
        public bool IsBannedFromGlobalChat { get; set; }


        public string ChatColor { get; set; }
        public int ShoutsRemaining { get; set; }
        public int InDuel { get; set; }
        public int InQuest { get; set; }
        public int InQuestState { get; set; }

        public int ItemsUsedThisTurn { get; set; }
        public bool FriendOnlyMessages { get; set; }

        public string GetFullName()
        {
            if (this.DonatorLevel >= 2 && !this.Nickname.IsNullOrEmpty())
            {
                return this.FirstName + " '" + this.Nickname + "' " + this.LastName;
            }
            else
            {
                return this.FirstName + " " + this.LastName;
            }

        }

        public bool IsUsingOriginalName()
        {
            return FirstName == OriginalFirstName && LastName == OriginalLastName;
        }

        protected virtual IReadOnlyDictionary<string, PlayerDescriptorDTO> staffDictionary { get { return ChatStatics.Staff; } }

        public Tuple<string, string> GetDescriptor()
        {
            PlayerDescriptorDTO desc;

            if (MembershipId == "-1")
                return new Tuple<string, string>(string.Empty, string.Empty);

            if (MembershipId.IsNullOrEmpty())
                return new Tuple<string, string>(GetFullName(), string.Empty);

            if (staffDictionary.TryGetValue(MembershipId, out desc))
            {
                var name = desc.Name;

                switch (desc.TagBehaviorEnum)
                {
                    case TagBehavior.Append:
                        name = GetFullName();
                        break;
                    case TagBehavior.ReplaceLastName:
                        name = $"{FirstName}{(DonatorLevel >= 2 && !Nickname.IsNullOrEmpty() ? $" '{Nickname}'" : "")}";
                        break;
                    case TagBehavior.ReplaceLastNameAndNick:
                        name = $"{FirstName}";
                        break;
                }

                return new Tuple<string, string>(name, desc.PictureURL);
            }

            return new Tuple<string, string>(GetFullName(), string.Empty);
        }

        public DateTime GetLastCombatTimestamp()
        {
            if (this.LastCombatTimestamp > this.LastCombatAttackedTimestamp)
            {
                return this.LastCombatTimestamp;
            }
            else
            {
                return this.LastCombatAttackedTimestamp;
            }
        }

        public Player ToDbPlayer()
        {
            var output = new Player
            {
                Id = this.Id,
                MembershipId = this.MembershipId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                dbLocationName = this.dbLocationName,
                FormSourceId = this.FormSourceId,
                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Mana = this.Mana,
                MaxMana = this.MaxMana,
                ExtraInventory =  this.ExtraInventory,
                SneakPercent = this.SneakPercent,
                MoveActionPointDiscount = this.MoveActionPointDiscount,
                ActionPoints = this.ActionPoints,
                ActionPoints_Refill = this.ActionPoints_Refill,
                Gender = this.Gender,
                Mobility = this.Mobility,
                BotId = this.BotId,

                MindControlIsActive = this.MindControlIsActive,

                XP = this.XP,
                Level = this.Level,
                TimesAttackingThisUpdate = this.TimesAttackingThisUpdate,
                IpAddress = this.IpAddress,
                LastActionTimestamp = this.LastActionTimestamp,
                LastCombatTimestamp = this.LastCombatTimestamp,
                LastCombatAttackedTimestamp = this.LastCombatAttackedTimestamp,
                FlaggedForAbuse = this.FlaggedForAbuse,
                UnusedLevelUpPerks = this.UnusedLevelUpPerks,
                GameMode = this.GameMode,
                InRP = this.InRP,
                InHardmode = this.InHardmode,
                CleansesMeditatesThisRound = this.CleansesMeditatesThisRound,
                Money = this.Money,
                Covenant = this.Covenant,
                OriginalFormSourceId = this.OriginalFormSourceId,
                PvPScore = this.PvPScore,
                DonatorLevel = this.DonatorLevel,
                Nickname = this.Nickname,
                OnlineActivityTimestamp = this.OnlineActivityTimestamp,

                InDuel = this.InDuel,

                IsBannedFromGlobalChat = this.IsBannedFromGlobalChat,
                InQuest = this.InQuest,
                InQuestState = this.InQuestState,
                ItemsUsedThisTurn = this.ItemsUsedThisTurn,
                FriendOnlyMessages = this.FriendOnlyMessages,

            };
            return output;
        }


        public bool IsInDungeon()
        {
            if (this.dbLocationName.Contains("dungeon_"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateOnlineActivityTimestamp()
        {
            var markOnlineCutoff = DateTime.UtcNow.AddMinutes(ChatStatics.OnlineActivityCutoffMinutes);

            // update the player's "last online" attribute if it's been long enough
            if (OnlineActivityTimestamp >= markOnlineCutoff || PvPStatics.AnimateUpdateInProgress)
                return;

            var cmd = new UpdateOnlineActivityTimestamp { Player = this };
            DomainRegistry.Root.Execute(cmd);
        }
    }

    public class PlayerDescriptorDTO
    {
        /// <summary>
        /// <para>Gets or sets the name that will be used if <see cref="TagBehaviorEnum"/> is set to <see cref="TagBehavior.ReplaceFullName"/>.</para>
        /// <para>Defaults to <see cref="string.Empty"/>.</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <para>Gets or sets the URL tumbnail.</para>
        /// <para>Defaults to <see cref="string.Empty"/>.</para>
        /// </summary>
        public string PictureURL { get; set; } = string.Empty;

        /// <summary>
        /// <para>Gets or sets the behavior for the tag</para>
        /// <para>Defaults to <see cref="TagBehavior.Append"/>.</para>
        /// <para>See <see cref="TagBehavior"/> for more description.</para>
        /// </summary>
        public TagBehavior TagBehaviorEnum { get; set; }
    }

    public static class PlayerDescriptorStatics
    {
        public enum TagBehavior
        {
            /// <summary>
            /// Leave the name and just append the appropriate tag at the end.
            /// </summary>
            Append,

            /// <summary>
            /// Replace the entire full name, including nickname, with <see cref="PlayerDescriptorDTO.Name"/> and append the appropriate tag.
            /// </summary>
            ReplaceFullName,

            /// <summary>
            /// Replace just the last name with the appropriate tag, leave the nickname.
            /// </summary>
            ReplaceLastName,

            /// <summary>
            /// Replace both the last name and the nickname with the appropriate tag.
            /// </summary>
            ReplaceLastNameAndNick,
        }
    }
}