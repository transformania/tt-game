using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Players;
using TT.Domain.Commands.Skills;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public class PlayerProcedures
    {

        public static PlayerFormViewModel GetPlayerFormViewModel(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from p in playerRepo.Players
                                                      where p.Id == playerId
                                                      join f in playerRepo.DbStaticForms on p.Form equals f.dbName
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = p.Id,
                                                              MembershipId = p.MembershipId,
                                                              FirstName = p.FirstName,
                                                              LastName = p.LastName,
                                                              dbLocationName = p.dbLocationName,
                                                              Form = p.Form,
                                                              Health = p.Health,
                                                              MaxHealth = p.MaxHealth,
                                                              Mana = p.Mana,
                                                              MaxMana = p.MaxMana,
                                                              ActionPoints = p.ActionPoints,
                                                              ActionPoints_Refill = p.ActionPoints_Refill,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              BotId = p.BotId,
                                                              MindControlIsActive = p.MindControlIsActive,
                                                              XP = p.XP,
                                                              Level = p.Level,
                                                              TimesAttackingThisUpdate = p.TimesAttackingThisUpdate,
                                                              IpAddress = p.IpAddress,
                                                              LastActionTimestamp = p.LastActionTimestamp,
                                                              LastCombatTimestamp = p.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = p.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = p.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = p.UnusedLevelUpPerks,
                                                              GameMode = p.GameMode,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                              ShoutsRemaining = p.ShoutsRemaining,
                                                              ChatColor = p.ChatColor,
                                                              IsBannedFromGlobalChat = p.IsBannedFromGlobalChat,
                                                              InDuel = p.InDuel,
                                                              InQuest = p.InQuest,
                                                              InQuestState = p.InQuestState,


                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              dbName = f.dbName,
                                                              FriendlyName = f.FriendlyName,
                                                              Description = f.Description,
                                                              TFEnergyType = f.TFEnergyType,
                                                              TFEnergyRequired = f.TFEnergyRequired,
                                                              Gender = f.Gender,
                                                              MobilityType = f.MobilityType,
                                                              BecomesItemDbName = f.BecomesItemDbName,
                                                              PortraitUrl = f.PortraitUrl,
                                                              IsUnique = f.IsUnique,

                                                              HealthBonusPercent = f.HealthBonusPercent,
                                                              ManaBonusPercent = f.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = f.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = f.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = f.ManaRecoveryPerUpdate,
                                                              SneakPercent = f.SneakPercent,
                                                              EvasionPercent = f.EvasionPercent,
                                                              EvasionNegationPercent = f.EvasionNegationPercent,
                                                              MeditationExtraMana = f.MeditationExtraMana,
                                                              CleanseExtraHealth = f.CleanseExtraHealth,
                                                              MoveActionPointDiscount = f.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = f.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = f.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = f.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = f.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = f.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = f.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = f.ExtraInventorySpace,

                                                              Discipline = f.Discipline,
                                                              Perception = f.Perception,
                                                              Charisma = f.Charisma,
                                                              Submission_Dominance = f.Submission_Dominance,

                                                              Fortitude = f.Fortitude,
                                                              Agility = f.Agility,
                                                              Allure = f.Allure,
                                                              Corruption_Purity = f.Corruption_Purity,

                                                              Magicka = f.Magicka,
                                                              Succour = f.Succour,
                                                              Luck = f.Luck,
                                                              Chaos_Order = f.Chaos_Order,
                                                          }

                                                      };

            return output.First();
        }

        public static PlayerFormViewModel GetPlayerFormViewModel_FromMembership(string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from p in playerRepo.Players
                                                      where p.MembershipId == membershipId
                                                      join f in playerRepo.DbStaticForms on p.Form equals f.dbName
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = p.Id,
                                                              MembershipId = p.MembershipId,
                                                              FirstName = p.FirstName,
                                                              LastName = p.LastName,
                                                              dbLocationName = p.dbLocationName,
                                                              Form = p.Form,
                                                              Health = p.Health,
                                                              MaxHealth = p.MaxHealth,
                                                              Mana = p.Mana,
                                                              MaxMana = p.MaxMana,
                                                              ActionPoints = p.ActionPoints,
                                                              ActionPoints_Refill = p.ActionPoints_Refill,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              BotId = p.BotId,
                                                              MindControlIsActive = p.MindControlIsActive,
                                                              XP = p.XP,
                                                              Level = p.Level,
                                                              TimesAttackingThisUpdate = p.TimesAttackingThisUpdate,
                                                              IpAddress = p.IpAddress,
                                                              LastActionTimestamp = p.LastActionTimestamp,
                                                              LastCombatTimestamp = p.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = p.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = p.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = p.UnusedLevelUpPerks,
                                                              GameMode = p.GameMode,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                              ShoutsRemaining = p.ShoutsRemaining,
                                                              ChatColor = p.ChatColor,
                                                              IsBannedFromGlobalChat = p.IsBannedFromGlobalChat,
                                                              InDuel = p.InDuel,
                                                              InQuest = p.InQuest,
                                                              InQuestState = p.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              dbName = f.dbName,
                                                              FriendlyName = f.FriendlyName,
                                                              Description = f.Description,
                                                              TFEnergyType = f.TFEnergyType,
                                                              TFEnergyRequired = f.TFEnergyRequired,
                                                              Gender = f.Gender,
                                                              MobilityType = f.MobilityType,
                                                              BecomesItemDbName = f.BecomesItemDbName,
                                                              PortraitUrl = f.PortraitUrl,
                                                              IsUnique = f.IsUnique,

                                                              HealthBonusPercent = f.HealthBonusPercent,
                                                              ManaBonusPercent = f.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = f.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = f.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = f.ManaRecoveryPerUpdate,
                                                              SneakPercent = f.SneakPercent,
                                                              EvasionPercent = f.EvasionPercent,
                                                              EvasionNegationPercent = f.EvasionNegationPercent,
                                                              MeditationExtraMana = f.MeditationExtraMana,
                                                              CleanseExtraHealth = f.CleanseExtraHealth,
                                                              MoveActionPointDiscount = f.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = f.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = f.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = f.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = f.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = f.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = f.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = f.ExtraInventorySpace,

                                                              Discipline = f.Discipline,
                                                              Perception = f.Perception,
                                                              Charisma = f.Charisma,
                                                              Submission_Dominance = f.Submission_Dominance,

                                                              Fortitude = f.Fortitude,
                                                              Agility = f.Agility,
                                                              Allure = f.Allure,
                                                              Corruption_Purity = f.Corruption_Purity,

                                                              Magicka = f.Magicka,
                                                              Succour = f.Succour,
                                                              Luck = f.Luck,
                                                              Chaos_Order = f.Chaos_Order,
                                                          }

                                                      };

            return output.First();
        }

        public static IEnumerable<PlayerFormViewModel> GetPlayerFormViewModelsAtLocation(string destinationDbName, string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from p in playerRepo.Players
                                                      where p.dbLocationName == destinationDbName && p.MembershipId != membershipId
                                                      join f in playerRepo.DbStaticForms on p.Form equals f.dbName
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = p.Id,
                                                              MembershipId = p.MembershipId,
                                                              FirstName = p.FirstName,
                                                              LastName = p.LastName,
                                                              dbLocationName = p.dbLocationName,
                                                              Form = p.Form,
                                                              Health = p.Health,
                                                              MaxHealth = p.MaxHealth,
                                                              Mana = p.Mana,
                                                              MaxMana = p.MaxMana,
                                                              ActionPoints = p.ActionPoints,
                                                              ActionPoints_Refill = p.ActionPoints_Refill,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              BotId = p.BotId,
                                                              MindControlIsActive = p.MindControlIsActive,

                                                              XP = p.XP,
                                                              Level = p.Level,
                                                              TimesAttackingThisUpdate = p.TimesAttackingThisUpdate,
                                                              IpAddress = p.IpAddress,
                                                              LastActionTimestamp = p.LastActionTimestamp,
                                                              LastCombatTimestamp = p.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = p.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = p.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = p.UnusedLevelUpPerks,
                                                              GameMode = p.GameMode,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                              ShoutsRemaining = p.ShoutsRemaining,
                                                              ChatColor = p.ChatColor,
                                                              IsBannedFromGlobalChat = p.IsBannedFromGlobalChat,
                                                              InDuel = p.InDuel,
                                                              InQuest = p.InQuest,
                                                              InQuestState = p.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              dbName = f.dbName,
                                                              FriendlyName = f.FriendlyName,
                                                              Description = f.Description,
                                                              TFEnergyType = f.TFEnergyType,
                                                              TFEnergyRequired = f.TFEnergyRequired,
                                                              Gender = f.Gender,
                                                              MobilityType = f.MobilityType,
                                                              BecomesItemDbName = f.BecomesItemDbName,
                                                              PortraitUrl = f.PortraitUrl,
                                                              IsUnique = f.IsUnique,

                                                              HealthBonusPercent = f.HealthBonusPercent,
                                                              ManaBonusPercent = f.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = f.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = f.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = f.ManaRecoveryPerUpdate,
                                                              SneakPercent = f.SneakPercent,
                                                              EvasionPercent = f.EvasionPercent,
                                                              EvasionNegationPercent = f.EvasionNegationPercent,
                                                              MeditationExtraMana = f.MeditationExtraMana,
                                                              CleanseExtraHealth = f.CleanseExtraHealth,
                                                              MoveActionPointDiscount = f.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = f.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = f.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = f.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = f.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = f.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = f.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = f.ExtraInventorySpace,

                                                              Discipline = f.Discipline,
                                                              Perception = f.Perception,
                                                              Charisma = f.Charisma,
                                                              Submission_Dominance = f.Submission_Dominance,

                                                              Fortitude = f.Fortitude,
                                                              Agility = f.Agility,
                                                              Allure = f.Allure,
                                                              Corruption_Purity = f.Corruption_Purity,

                                                              Magicka = f.Magicka,
                                                              Succour = f.Succour,
                                                              Luck = f.Luck,
                                                              Chaos_Order = f.Chaos_Order,

                                                          }

                                                      };

            return output;
        }

        public static IEnumerable<PlayerFormViewModel> GetPlayerFormViewModelsInCovenant(int covenantId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from p in playerRepo.Players
                                                      where p.Covenant == covenantId
                                                      join f in playerRepo.DbStaticForms on p.Form equals f.dbName
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = p.Id,
                                                              MembershipId = p.MembershipId,
                                                              FirstName = p.FirstName,
                                                              LastName = p.LastName,
                                                              dbLocationName = p.dbLocationName,
                                                              Form = p.Form,
                                                              Health = p.Health,
                                                              MaxHealth = p.MaxHealth,
                                                              Mana = p.Mana,
                                                              MaxMana = p.MaxMana,
                                                              ActionPoints = p.ActionPoints,
                                                              ActionPoints_Refill = p.ActionPoints_Refill,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              BotId = p.BotId,
                                                              MindControlIsActive = p.MindControlIsActive,

                                                              XP = p.XP,
                                                              Level = p.Level,
                                                              TimesAttackingThisUpdate = p.TimesAttackingThisUpdate,
                                                              IpAddress = p.IpAddress,
                                                              LastActionTimestamp = p.LastActionTimestamp,
                                                              LastCombatTimestamp = p.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = p.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = p.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = p.UnusedLevelUpPerks,
                                                              GameMode = p.GameMode,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                              ShoutsRemaining = p.ShoutsRemaining,
                                                              ChatColor = p.ChatColor,
                                                              IsBannedFromGlobalChat = p.IsBannedFromGlobalChat,
                                                              InDuel = p.InDuel,
                                                              InQuest = p.InQuest,
                                                              InQuestState = p.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              dbName = f.dbName,
                                                              FriendlyName = f.FriendlyName,
                                                              Description = f.Description,
                                                              TFEnergyType = f.TFEnergyType,
                                                              TFEnergyRequired = f.TFEnergyRequired,
                                                              Gender = f.Gender,
                                                              MobilityType = f.MobilityType,
                                                              BecomesItemDbName = f.BecomesItemDbName,
                                                              PortraitUrl = f.PortraitUrl,
                                                              IsUnique = f.IsUnique,

                                                              HealthBonusPercent = f.HealthBonusPercent,
                                                              ManaBonusPercent = f.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = f.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = f.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = f.ManaRecoveryPerUpdate,
                                                              SneakPercent = f.SneakPercent,
                                                              EvasionPercent = f.EvasionPercent,
                                                              EvasionNegationPercent = f.EvasionNegationPercent,
                                                              MeditationExtraMana = f.MeditationExtraMana,
                                                              CleanseExtraHealth = f.CleanseExtraHealth,
                                                              MoveActionPointDiscount = f.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = f.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = f.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = f.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = f.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = f.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = f.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = f.ExtraInventorySpace,

                                                              Discipline = f.Discipline,
                                                              Perception = f.Perception,
                                                              Charisma = f.Charisma,
                                                              Submission_Dominance = f.Submission_Dominance,

                                                              Fortitude = f.Fortitude,
                                                              Agility = f.Agility,
                                                              Allure = f.Allure,
                                                              Corruption_Purity = f.Corruption_Purity,

                                                              Magicka = f.Magicka,
                                                              Succour = f.Succour,
                                                              Luck = f.Luck,
                                                              Chaos_Order = f.Chaos_Order,
                                                          }

                                                      };

            return output;
        }

        public static Player GetPlayerFromMembership(string id)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == id);

            return player;

        }

        public static Player GetPlayerFromBotId(int id)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.BotId == id);

            return player;

        }

        public static Player GetPlayer(int? playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            return player;
        }

        public static string SaveNewPlayer(NewCharacterViewModel player, string membershipId)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();

            string noGenerationLastName = player.LastName.Split(' ')[0];

            Player ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == noGenerationLastName);

            if (ghost != null && ghost.BotId != AIStatics.RerolledPlayerBotId && ghost.MembershipId != membershipId)
            {
                return "A character of this name already exists.";
            }

            string generationTitle = "";

            if (ghost != null && (ghost.BotId == AIStatics.RerolledPlayerBotId || ghost.MembershipId == membershipId) && ghost.FirstName == player.FirstName && ghost.LastName == player.LastName)
            {

                List<Player> possibleOldGens = playerRepo.Players.Where(p => p.FirstName == player.FirstName && p.LastName.Contains(player.LastName)).ToList();

                if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName) == null)
                {
                    generationTitle = " II";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " II") == null) {
                    generationTitle = " II";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " III") == null)
                {
                    generationTitle = " III";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " IV") == null)
                {
                    generationTitle = " IV";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " V") == null)
                {
                    generationTitle = " V";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " VI") == null)
                {
                    generationTitle = " VI";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " VII") == null)
                {
                    generationTitle = " VII";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " VIII") == null)
                {
                    generationTitle = " VIII";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " IX") == null)
                {
                    generationTitle = " IX";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName + " X") == null)
                {
                    generationTitle = " X";
                }
                else
                {
                    return "This name has gone through too many generations.  Choose another one.";
                }





            }

            // check that the name has not been reserved by someone else with a different Membership Id

            IReservedNameRepository resNameRepo = new EFReservedNameRepository();
            ReservedName resNameGhost = resNameRepo.ReservedNames.FirstOrDefault(r => r.FullName == player.FirstName + " " + player.LastName);

            if (resNameGhost != null && resNameGhost.MembershipId != membershipId)
            {
                return "This name has been reserved by a different player.  Choose another.";
            }

            // assert that the form is a valid staring form
            string x = player.FormName;

            if (x != "man_01" && x != "man_02" && x != "man_03" && x != "man_04" && x != "man_05" && x != "woman_01" && x != "woman_02" && x != "woman_03" && x != "woman_04" && x != "woman_05")
            {
                return "That is not a valid starting form.";
            }

            Player vendor = null;
            if (player.InanimateForm != null && player.StartAsInanimate)
            {
                if (player.InanimateForm.ToString() == "pet" || player.InanimateForm.ToString() == "random")
                {
                    vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);
                    if (vendor == null)
                    {
                        return "Wüffie is not currently available to accept new pets. Please try again later.";
                    }
                }
                if (player.InanimateForm.ToString() != "pet")
                {
                    vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
                    if (vendor == null)
                    {
                        return "Lindella is not currently available to accept new items. Please try again later.";
                    }
                }
            }

            // remove the old Player--Membership binding
            Player oldplayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);

            int oldCovId = 0;

            if (oldplayer != null)
            {
                TimeSpan rerollTime = RerollProcedures.GetTimeUntilReroll(oldplayer);
                if (rerollTime.TotalSeconds > 0)
                {
                    return "It is too soon for you to start again. Please try again in " + rerollTime.ToString(@"hh\:mm\:ss") + ".";
                }

                // remove all of the old player's player logs
                PlayerLogProcedures.ClearPlayerLog(oldplayer.Id);

                // delete the achivements for that player that get reset upon reroll, such as reading tomes or passing quests
                StatsProcedures.DeleteAchivemenstOfTypeForPlayer(oldplayer, StatsProcedures.GetAchivementNamesThatReset());

                // remove all of the old player's TF energies
                TFEnergyProcedures.DeleteAllPlayerTFEnergies(oldplayer.Id);
                oldplayer.MembershipId = null;
                oldplayer.BotId = AIStatics.RerolledPlayerBotId;
                playerRepo.SavePlayer(oldplayer);

                // remove the old player's effects
                EffectProcedures.DeleteAllPlayerEffects(oldplayer.Id);

                oldCovId = oldplayer.Covenant;

                // turn the item they player became permanent
                IItemRepository itemRepo = new EFItemRepository();
                Item oldItemMe = itemRepo.Items.FirstOrDefault(i => i.VictimName == oldplayer.FirstName + " " + oldplayer.LastName);
                oldItemMe.IsPermanent = true;
                oldItemMe.LastSouledTimestamp = DateTime.UtcNow.AddYears(1);
                itemRepo.SaveItem(oldItemMe);

            }


            // clean the name entered by the player, capitalize first letter and downcase the rest
            string cleanFirstName = char.ToUpper(player.FirstName[0]) + player.FirstName.Substring(1).ToLower();
            string cleanLastName = char.ToUpper(player.LastName[0]) + player.LastName.Substring(1).ToLower();

            player.FirstName = cleanFirstName;
            player.LastName = cleanLastName + generationTitle;


            var cmd = new CreatePlayer();

            cmd.FirstName = player.FirstName;
            cmd.LastName = player.LastName;
            cmd.Gender = player.Gender;
            cmd.Location = "coffee_shop";
            cmd.UserId = membershipId;
            cmd.Form = player.FormName;
            cmd.FormSourceId = FormStatics.GetForm(cmd.Form).Id;
            cmd.OriginalForm = player.FormName;
            cmd.BotId = AIStatics.ActivePlayerBotId;

            // if player is not choosing to start in an inanimate/pet form, start them off in Welcome to Sunnyglade quest
            if (player.InanimateForm == null)
            {
                cmd.InQuest = 6; // Welcome to Sunnyglade quest
                cmd.InQuestState = 93; // first stage of Welcome to Sunnyglade
            }

            if (oldplayer != null)
            {
                cmd.Covenant = oldplayer.Covenant;
                oldplayer.Covenant = 0;
                cmd.Level = oldplayer.Level - 3;
                if (cmd.Level < 1)
                {
                    cmd.Level = 1;
                }
                cmd.UnusedLevelUpPerks = cmd.Level - 1;
                cmd.ChatColor = oldplayer.ChatColor;

            }

            // start player in PvP if they choose, otherwise put them in protection
            cmd.GameMode = player.StartGameMode;


            if (player.StartInRP)
            {
                cmd.InRP = true;
            }
            else
            {
                cmd.InRP = false;
            }

            cmd.Location = LocationsStatics.GetRandomLocation();

            int newPlayerId = DomainRegistry.Repository.Execute(cmd);
           // playerRepo.SavePlayer(newplayer);
            RerollProcedures.AddRerollGeneration(cmd.UserId);

            if (oldplayer != null)
            {
                // transfer all of the old player's skills that are NOT form specific or weaken
                SkillProcedures.TransferAllPlayerSkills(oldplayer.Id, newPlayerId);

                // transfer their old messages to new account
                if (player.MigrateLetters)
                {
                    using (var context = new StatsContext())
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Messages] SET ReceiverId = " + newPlayerId + " WHERE ReceiverId = " + oldplayer.Id);
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Messages] SET SenderId = " + newPlayerId + " WHERE SenderId = " + oldplayer.Id);
                    }

                }

            }

            // assign the player their appropriate donation level
            DonatorProcedures.SetNewPlayerDonationRank(newPlayerId);

            // if the player was in a covenant, they might have been the leader.  Check this and make a new player the leader
            if (oldCovId > 0)
            {
                ICovenantRepository covRepo = new EFCovenantRepository();
                Covenant oldCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == oldCovId);

                // we need to regrab the new player from the repo again to get their Id
                Player newmeFromDb = PlayerProcedures.GetPlayerWithExactName(cmd.FirstName + " " + cmd.LastName);

                if (oldCovenant != null && oldCovenant.LeaderId == oldplayer.Id)
                {
                    oldCovenant.LeaderId = newmeFromDb.Id;
                    covRepo.SaveCovenant(oldCovenant);
                }

            }

            DomainRegistry.Repository.Execute(new CreateSkill {ownerId = newPlayerId, skillSourceId = SkillStatics.WeakenSkillSourceId });

            if (player.InanimateForm != null)
            {
                DbStaticForm startform = ItemProcedures.GetFormFromItem(ItemProcedures.GetRandomItemOfType(player.InanimateForm.ToString()));
                if (player.InanimateForm.ToString() == "random" && startform.MobilityType == "animal") vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);

                DomainRegistry.Repository.Execute(new ChangeForm
                {
                    PlayerId = newPlayerId,
                    FormName = startform.dbName
                });

                Player newplayer = playerRepo.Players.FirstOrDefault(p => p.Id == newPlayerId);
                newplayer.Health = 0;
                newplayer.Mana = 0;
                newplayer.ActionPoints = 120;

                playerRepo.SavePlayer(newplayer);
                ItemProcedures.PlayerBecomesItem(newplayer, startform, vendor);
                ItemProcedures.LockItem(newplayer);
            }
            return "saved";

        }

        public static void SetCustomBase(Player player, string newFormName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.OriginalForm = newFormName;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static void InstantRestoreToBase(Player player)
        {
            var oldForm = player.Form;

            if (player.Mobility != PvPStatics.MobilityFull)
            {
                IItemRepository itemRepo = new EFItemRepository();
                Item itemMe = itemRepo.Items.FirstOrDefault(i => i.VictimName == player.FirstName + " " + player.LastName);

                if (itemMe != null)
                {
                    itemRepo.DeleteItem(itemMe.Id);
                }
            }
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = player.Id,
                FormName = player.OriginalForm
            });

            SkillProcedures.UpdateFormSpecificSkillsToPlayer(player, oldForm, player.OriginalForm);

        }

        public static void InstantChangeToForm(Player player, string formName)
        {
            var oldForm = player.Form;
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = player.Id,
                FormName = formName
            });
            SkillProcedures.UpdateFormSpecificSkillsToPlayer(player, oldForm, formName);
        }

        public static void MarkOnlineActivityTimestamp(Player player)
        {
            MarkOnlineActivityTimestamp(player.Id);
        }

        public static void MarkOnlineActivityTimestamp(Player_VM player)
        {
            MarkOnlineActivityTimestamp(player.Id);
        }

        private static void MarkOnlineActivityTimestamp(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.OnlineActivityTimestamp = DateTime.UtcNow;

            playerRepo.SavePlayer(dbPlayer);
        }

        public static string MovePlayer(int playerId, string destinationDbName, decimal actionPointDiscount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.dbLocationName = destinationDbName;

            decimal totalMoveCost = PvPStatics.LocationMoveCost - actionPointDiscount;

            // TEMP
            BuffBox mybuffs = ItemProcedures.GetPlayerBuffs(dbPlayer);

            dbPlayer = ReadjustMaxes(dbPlayer, mybuffs);

            if (totalMoveCost < .1M)
            {
                totalMoveCost = .1M;
            }

            dbPlayer.ActionPoints -= totalMoveCost;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;

            playerRepo.SavePlayer(dbPlayer);

            return "";
        }

        public static void MovePlayer_InstantNoLog(int playerId, string newLocation)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.dbLocationName = newLocation;
            playerRepo.SavePlayer(player);
        }

        public static string MovePlayer(string destinationDbName, decimal actionPointDiscount, string membershipId)
        {
            return MovePlayer(GetPlayerFromMembership(membershipId).Id, destinationDbName, actionPointDiscount);
        }

        public static void MovePlayerMultipleLocations(Player player, string destinationDbName, decimal actionPointCost)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            AIProcedures.MoveTo(dbPlayer, destinationDbName, 99999);
            dbPlayer.ActionPoints -= actionPointCost;
            dbPlayer.dbLocationName = destinationDbName;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static string TeleportPlayer(Player player, string destination, bool showDestinationInLocationLog)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            Player user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            Location oldLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == user.dbLocationName);
            Location newLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == destination);

            user.dbLocationName = destination;
            playerRepo.SavePlayer(user);

            string playerLogMessage = "";
            string locationMessageOld = "";
            string locationMessageNew = "";

            if (showDestinationInLocationLog)
            {
                locationMessageOld = player.GetFullName() + " used a Covenant Call Crystal, teleporting to their safeground at " + newLocation.Name + ".";
                locationMessageNew = player.GetFullName() + " teleported home using a Covenant Call Crystal.";
                playerLogMessage = "You used a Covenant Call Crystal, teleporting you from " + oldLocation.Name + " to your safeground at " + newLocation.Name + ".";
            }
            else {
                locationMessageOld = player.GetFullName() + " used a scroll of teleportation.";
                locationMessageNew = player.GetFullName() + " teleported to here.";
                playerLogMessage = "You teleported from " + oldLocation.Name + " to " + newLocation.Name + ".";
            }


            LocationLogProcedures.AddLocationLog(oldLocation.dbName, locationMessageOld);
            LocationLogProcedures.AddLocationLog(destination, locationMessageNew);
            PlayerLogProcedures.AddPlayerLog(player.Id, playerLogMessage, false);

            return playerLogMessage;
        }

        /// <summary>
        /// Return all players in a location, whether or not they are online or animate.
        /// </summary>
        /// <param name="destinationDbName"></param>
        /// <returns></returns>
        public static IEnumerable<Player> GetPlayersAtLocation(string destinationDbName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<Player> output = playerRepo.Players.Where(p => p.dbLocationName == destinationDbName).ToList();

            return output;
        }

        public static void ChangePlayerActionMana(decimal actionPoints, decimal health, decimal mana, int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.ActionPoints -= actionPoints;
            player.Mana += mana;
            player.Health += health;

            player.LastActionTimestamp = DateTime.UtcNow;

            if (player.Mana > player.MaxMana)
            {
                player.Mana = player.MaxMana;
            }
            if (player.Mana < 0)
            {
                player.Mana = 0;
            }
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
            if (player.Health < 0)
            {
                player.Health = 0;
            }

            playerRepo.SavePlayer(player);
        }

        public static void ChangePlayerActionManaNoTimestamp(decimal actionPoints, decimal health, decimal mana, int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.ActionPoints -= actionPoints;
            player.Mana += mana;
            player.Health += health;

            // player.LastActionTimestamp = DateTime.UtcNow;

            if (player.Mana > player.MaxMana)
            {
                player.Mana = player.MaxMana;
            }
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }

            playerRepo.SavePlayer(player);
        }

        public static string DamagePlayerHealth(int playerId, decimal amount)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.Health -= amount;

            if (player.Health < 0)
            {
                player.Health = 0;
            }

            playerRepo.SavePlayer(player);

            return "You lower " + player.FirstName + "'s willpower by " + amount + ".  ";

        }

        public static string SearchLocation(Player player, string dbLocationName)
        {

            Random rand = new Random();
            double roll = rand.NextDouble() * 100;

            // check to see if this is a location that has a summonable boss.  If so, do the random roll for it
            if (BossSummonDictionary.GlobalBossSummonDictionary.ContainsKey(dbLocationName))
            {
                // check and see if any other boss events are active
                if (!PvPWorldStatProcedures.IsAnyBossActive())
                {
                    PvPWorldStat worldStats = PvPWorldStatProcedures.GetWorldStats();
                    BossSummon bossSummon = BossSummonDictionary.GlobalBossSummonDictionary[dbLocationName];
                    if (roll < 1 && dbLocationName == "ranch_bedroom")
                    {
                        if (worldStats.IsDonnaAvailable())
                        {
                            BossProcedures_Donna.SpawnDonna();
                            PvPWorldStatProcedures.Boss_StartDonna();
                            string summontext = BossSummonDictionary.GetActivationText("Donna");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }

                    }
                    else if (roll < 1 && dbLocationName == "castle_armory")
                    {
                        if (worldStats.IsValentineAvailable())
                        {
                            BossProcedures_Valentine.SpawnValentine();
                            PvPWorldStatProcedures.Boss_StartValentine();
                            string summontext = BossSummonDictionary.GetActivationText("Valentine");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "stripclub_bar_seats")
                    {
                        if (worldStats.IsBimboAvailable())
                        {
                            BossProcedures_BimboBoss.SpawnBimboBoss();
                            PvPWorldStatProcedures.Boss_StartBimbo();
                            string summontext = BossSummonDictionary.GetActivationText("BimboBoss");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "tavern_pool")
                    {
                        if (worldStats.IsTheifAvailable())
                        {
                            BossProcedures_Thieves.SpawnThieves();
                            PvPWorldStatProcedures.Boss_StartThieves();
                            string summontext = BossSummonDictionary.GetActivationText("Thieves");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "college_foyer")
                    {
                        if (worldStats.IsSistersAvailable())
                        {
                            BossProcedures_Sisters.SpawnSisters();
                            PvPWorldStatProcedures.Boss_StartSisters();
                            string summontext = BossSummonDictionary.GetActivationText("Sisters");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == BossProcedures_FaeBoss.SpawnLocation)
                    {
                        if (worldStats.IsFaeBossAvailable())
                        {
                            BossProcedures_FaeBoss.SpawnFaeBoss();
                            PvPWorldStatProcedures.Boss_StartFaeBoss();
                            string summontext = BossSummonDictionary.GetActivationText("FaeBoss");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                }
            }

            Location here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == dbLocationName);


            // learn a new skill
            if (roll < 30)
            {
                Random rand2 = new Random();
                double roll2 = rand.NextDouble() * 100;

                IEnumerable<DbStaticSkill> eligibleSkills;

                IEnumerable<StaticSkill> myKnownSkills = SkillProcedures.GetStaticSkillsOwnedByPlayer(player.Id);

                // get all the skills that are found in THIS EXACT LOCATION
                IEnumerable<DbStaticSkill> skillsAtThisLocation = SkillStatics.GetSkillsLearnedAtLocation(here.dbName);
                eligibleSkills = from s in skillsAtThisLocation
                                 let sx = myKnownSkills.Select(r => r.dbName)
                                 where !sx.Contains(s.dbName)
                                 select s;

                // get all the skills that are found in the region this location is in
                if (!eligibleSkills.Any())
                {
                    IEnumerable<DbStaticSkill> skillsAtThisRegion = SkillStatics.GetSkillsLearnedAtRegion(here.Region);
                    eligibleSkills = from s in skillsAtThisRegion
                                     let sx = myKnownSkills.Select(r => r.dbName)
                                     where !sx.Contains(s.dbName)
                                     select s;
                }

                // there are no new spells to be learned that are not-region specific, so player is just out of luck.
                if (!eligibleSkills.Any())
                {
                    return "You get the feeling there are no new spells for you to discover around here.";
                }


                double max = eligibleSkills.Count();
                int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                DbStaticSkill skillToLearn = eligibleSkills.ElementAt(randIndex);
                string output = SkillProcedures.GiveSkillToPlayer(player.Id, skillToLearn);

                return output;


            }

            // give the player some money (30-48)
            else if (roll < 48)
            {
                decimal moneyamount = Convert.ToDecimal(1 + Math.Floor(rand.NextDouble() * 3));
                GiveMoneyToPlayer(player, moneyamount);
                return "You collected " + (int)moneyamount + " Arpeyjis that were scattered on the ground.";


            }

            // find a findable item (48-60)
            else if (roll < 60)
            {


                List<DbStaticItem> eligibleItems = ItemStatics.GetAllFindableItems().ToList();

                DbStaticItem justFound = ItemProcedures.GetRandomFindableItem();

                string output = ItemProcedures.GiveNewItemToPlayer(player, justFound);

                Player me = PlayerProcedures.GetPlayer(player.Id);
                BuffBox myBuffs = ItemProcedures.GetPlayerBuffs(me);

                // drop an item of the same type that you are carrying if you are over the limit
                if (ItemProcedures.PlayerIsCarryingTooMuch(player.Id, 1, myBuffs))
                {
                    ItemViewModel randomItem = ItemProcedures.GetAllPlayerItems(player.Id).Where(i => i.dbItem.dbName == justFound.dbName).Last();
                    ItemProcedures.DropItem(randomItem.dbItem.Id, here.dbName);
                    output += "  However, your arms are full and you dropped it.";
                }

                return output;

            }

            // give some xp (60-80)
            else if (roll < 80)
            {
                PlayerProcedures.GiveXP(player, 3);
                return "Although you didn't find anything or learn any new spells, you note down a few things about your surroundings, which may come in useful in the future.  (+3 XP)";
            }

            // get effect or find nothing useful
            else if (roll <= 100)
            {
                // see if there is an effect that can be found in this area
                List<DbStaticEffect> effectsHere = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToList();

                if (!effectsHere.Any())
                {
                    PlayerProcedures.GiveXP(player, 1.5M);
                    return "Although you didn't find anything or learn any new spells, you feel as though you know this town a little better, which may come in useful in the future.  (+1.5 XP)";
                }
                else
                {


                    // give the player a random effect found here
                    double max = effectsHere.Count();
                    int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                    DbStaticEffect effectToGet = effectsHere.ElementAt(randIndex);

                    // assert that the player doesn't already have this effect.  IF they do, break out
                    if (EffectProcedures.PlayerHasEffect(player, effectToGet.dbName))
                    {
                        PlayerProcedures.GiveXP(player, 1.5M);
                        return "Although you didn't find anything or learn any new spells, you feel as though you know this town a little better, which may come in useful in the future.  (+1.5 XP)";
                    }

                    return EffectProcedures.GivePerkToPlayer(effectToGet.dbName, player);

                }

            }

            return "Unfortunately, you did not find anything useful.";


        }

        public static void ChangeMaxHealthMana(int playerId, decimal extraHealthMax, decimal extraManaMax) {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.MaxHealth += extraHealthMax;
            player.MaxMana += extraManaMax;

            if (player.Health > player.MaxHealth) {
                player.Health = player.MaxHealth;
            }

            if (player.Mana > player.MaxMana)
            {
                player.Mana = player.MaxMana;
            }

            if (player.Health < 0)
            {
                player.Health = 0;
            }

            if (player.Mana < 0)
            {
                player.Mana = 0;
            }

            if (player.MaxHealth < 5)
            {
                player.MaxHealth = 5;
            }

            if (player.MaxMana < 5)
            {
                player.MaxMana = 5;
            }

            playerRepo.SavePlayer(player);
        }

        public static WorldStats GetWorldPlayerStats()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<Player> players = playerRepo.Players.AsEnumerable();

            DateTime cutoff = DateTime.UtcNow.AddHours(-1);

            WorldStats output = new WorldStats
            {
                TotalPlayers = players.Count(p => p.BotId == AIStatics.ActivePlayerBotId),
                CurrentOnlinePlayers = players.Count(p => p.BotId == AIStatics.ActivePlayerBotId && p.OnlineActivityTimestamp >= cutoff)
            };



            return output;

        }

        /// <summary>
        /// Calculate how much XP is needed for the player to reach the next level given their current level, based off a hyperbolic formula currently set to 11x^2+x*0+89 .  The number is rounded up to the nearest 10.
        /// 
        /// </summary>
        /// <param name="level">The player's current level</param>
        /// <returns></returns>
        public static float GetXPNeededForLevelUp(int level)
        {
            float xp = 11 * level * level + 0 + 89;
            float leftover = xp % 10;

            xp = (float)Math.Round(xp / 10) * 10;

            if (leftover != 0)
            {
                xp += 10;
            }

            return xp;
        }

        public static float GetManaBaseByLevel(int level)
        {
            float manaBase = 5 * (level - 1) + 50;
            return manaBase;
        }

        public static float GetWillpowerBaseByLevel(int level)
        {
            float willpowerBase = 15 * (level - 1) + 100;
            return willpowerBase;
        }


        /// <summary>
        /// Assign a player XP and calculate whether or not they should level up
        /// </summary>
        /// <param name="player">Player earning XP</param>
        /// <param name="amount">Amount of XP the player is earning</param>
        /// <returns></returns>
        public static string GiveXP(Player player, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            // decrease XP gain by 40% for psychos
            if (dbPlayer.BotId == AIStatics.PsychopathBotId)
            {
                amount = amount * .6M;
            }

            dbPlayer.XP += amount;

            string output = "";

            if (amount > 0)
            {
                float xpNeeded = GetXPNeededForLevelUp(dbPlayer.Level);

                if ((float)dbPlayer.XP > xpNeeded)
                {
                    dbPlayer.Level++;
                    dbPlayer.XP -= Convert.ToDecimal(xpNeeded);
                    output += GiveLevelingBonus(dbPlayer, 2);
                }
            }

            playerRepo.SavePlayer(dbPlayer);

            return output;
        }

        public static void AddAttackCount(Player player)
        {
            AddAttackCount(player, 1);
        }

        public static void AddAttackCount(Player player, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate += amount;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(player);
        }

        public static void SetAttackCount(Player player, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate = amount;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(player);
        }

        public static void LogCombatTimestampsAndAddAttackCount(Player victim, Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbvictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);
           // dbvictim.LastCombatTimestamp = DateTime.UtcNow;
           // playerRepo.SavePlayer(dbvictim);

            Player dbAttacker = playerRepo.Players.FirstOrDefault(p => p.Id == attacker.Id);
            dbAttacker.LastCombatTimestamp = DateTime.UtcNow;
            dbAttacker.TimesAttackingThisUpdate++;
            dbvictim.LastCombatAttackedTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbAttacker);
            playerRepo.SavePlayer(dbvictim);

        }

        /// <summary>
        /// Updates the last activity timestamp for a player to be current server time
        /// </summary>
        /// <param name="player">Player to be updated</param>
        public static void SetTimestampToNow(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbplayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbplayer);
        }

        public static void AddMinutesToTimestamp(Player player, int amount, bool PvPOnly)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            dbplayer.LastActionTimestamp = dbplayer.LastActionTimestamp.AddMinutes(amount);

            int timecompare = DateTime.Compare(dbplayer.LastActionTimestamp, DateTime.UtcNow);

           // t1 is greater than t2, aka last action timestamp is larger than now
            if (timecompare > 0)
            {
                dbplayer.LastActionTimestamp = DateTime.UtcNow;
            }

            playerRepo.SavePlayer(dbplayer);
        }

        private static string GiveLevelingBonus(Player player, int level)
        {
            player.MaxHealth += PvPStatics.LevelUpHealthMaxIncreaseBase + level*PvPStatics.LevelUpHealthMaxIncreasePerLevel;
            player.MaxMana += PvPStatics.LevelUpManaMaxIncreaseBase + level*PvPStatics.LevelUpHealthMaxIncreasePerLevel;

            player.UnusedLevelUpPerks++;
            return "  <b><i>Congratulations, you have gained an experience level!</i></b>";
        }

        public static void LogIP(string ip, string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);
            dbPlayer.IpAddress = ip;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static bool IsMyIPInUseAndAnimate(string ip)
        {



            IPlayerRepository playerRepo = new EFPlayerRepository();
           decimal num = playerRepo.Players.Where(p => p.IpAddress == ip && p.Mobility == PvPStatics.MobilityFull).Count();
           if (num > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsMyIPInUseAndAnimate(string ip, Player player)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            decimal num = playerRepo.Players.Count(p => p.BotId == AIStatics.ActivePlayerBotId && p.IpAddress == ip && p.Mobility == PvPStatics.MobilityFull && p.GameMode == player.GameMode);
            if (num > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsOffline(Player player)
        {
            if (player.BotId < AIStatics.RerolledPlayerBotId)
            {
                return false;
            }

            double minutesAgo = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (minutesAgo > PvPStatics.OfflineAfterXMinutes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsOffline(Player_VM player)
        {
            if (player.BotId < AIStatics.RerolledPlayerBotId)
            {
                return false;
            }

            double minutesAgo = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (minutesAgo > PvPStatics.OfflineAfterXMinutes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static IEnumerable<Player> GetPlayersWithFirstNameOf(string firstname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.FirstName == firstname);
        }

        public static IEnumerable<Player> GetPlayersWithLastNameOf(string lastname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.LastName == lastname);
        }

        public static Player GetPlayerWithExactName(string fullname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            string cleanedName = fullname.ToLower();
            return playerRepo.Players.FirstOrDefault(p => (p.FirstName + " " + p.LastName).ToLower() == cleanedName);
        }

        public static IEnumerable<Player> GetPlayersWithPartialName(string partialname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => (p.FirstName + " " + p.LastName).Contains(partialname)).Take(25).ToList();
        }

        public static void FlagPlayerForSuspicousActivity(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.FlaggedForAbuse = !player.FlaggedForAbuse;
            playerRepo.SavePlayer(player);
        }

        public static IEnumerable<Player> GetLeadingPlayers__XP(int number)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.BotId == AIStatics.ActivePlayerBotId).OrderByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);
        }

        public static IEnumerable<Player> GetLeadingPlayers__PvP(int number)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();


            return playerRepo.Players.Where(p => p.BotId == AIStatics.ActivePlayerBotId).OrderByDescending(p => p.PvPScore).ThenByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);

        }

        /// <summary>
        /// Allow a player to attempt to restore themself back to their own base form
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string SelfRestoreToBase(Player player, BuffBox buffs)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.ActionPoints -= (decimal)PvPStatics.SelfRestoreAPCost;
            dbPlayer.Mana -= (decimal)PvPStatics.SelfRestoreManaCost;
            dbPlayer.CleansesMeditatesThisRound++;
            playerRepo.SavePlayer(dbPlayer);

            ITFEnergyRepository energyRepo = new EFTFEnergyRepository();
            TFEnergy restoreEnergy = energyRepo.TFEnergies.FirstOrDefault(e => e.PlayerId == player.Id && e.FormName == "selfrestore");

            if (restoreEnergy==null)
            {
                restoreEnergy = new TFEnergy
                {
                    Amount = 0,
                    CasterId = -1,
                    Timestamp = DateTime.UtcNow,
                    FormName = "selfrestore",
                    PlayerId = player.Id
                };
            }

            string output = "";

            // build up some restoration energy

            float restoreAmount = PvPStatics.SelfRestoreBaseTFEnergyPerCast;
            float restoreBonus = (float)Math.Floor(buffs.Allure() / 10 );
            restoreAmount += restoreBonus;

            restoreEnergy.Amount += (decimal)restoreAmount;
            if (restoreEnergy.Amount > (decimal)PvPStatics.SelfRestoreTFnergyRequirement)
            {
                restoreEnergy.Amount = (decimal)PvPStatics.SelfRestoreTFnergyRequirement;
            }

            output += "You rest and attempt to restore yourself to your base form.  [+" + (int)restoreAmount + ", " + (int)restoreEnergy.Amount + "/" + PvPStatics.SelfRestoreTFnergyRequirement + "]";

            // enough energy built up for restore to be successful
            if (restoreEnergy.Amount >= (decimal)PvPStatics.SelfRestoreTFnergyRequirement)
            {
               
                PlayerProcedures.InstantRestoreToBase(player);
                PlayerProcedures.SetTimestampToNow(player);
                energyRepo.DeleteTFEnergy(restoreEnergy.Id);
                DbStaticForm newform = FormStatics.GetForm(dbPlayer.OriginalForm);

                output += "<span class='meditate'>With this final cast, you manage to restore yourself back to your base form as a <b>" + newform.FriendlyName + "</b>!<span>";

                PlayerLogProcedures.AddPlayerLog(player.Id, output, true);
            }

            // player does not have enough energy built up.
            else
            {
                output += "  Keep trying and you'll find yourself in a familiar form in no time...";
                energyRepo.SaveTFEnergy(restoreEnergy);
            }

            return output;
        }

        public static string Cleanse(Player player, BuffBox buffs)
        {
            string result = "";

            PlayerProcedures.AddCleanseMeditateCount(player);

            decimal cleanseBonusTFEnergyRemovalPercent = buffs.CleanseExtraTFEnergyRemovalPercent() + PvPStatics.CleanseTFEnergyPercentDecrease;
            decimal cleanseWPRestore = PvPStatics.CleanseHealthRestoreBase + buffs.CleanseExtraHealth() + player.Level;

            if (cleanseWPRestore <= 0)
            {
                cleanseWPRestore = 0;
                result = "You try to cleanse, but due to the magical effects on your body you fail to restore any willpower.";
            }
            else
            {
                result = "You quickly cleanse, restoring " + cleanseWPRestore + " willpower.";
            }


            // player is okay to cleanse; restore some health
            PlayerProcedures.ChangePlayerActionMana(PvPStatics.CleanseCost, cleanseWPRestore, -PvPStatics.CleanseManaCost, player.Id);

            if (cleanseBonusTFEnergyRemovalPercent > 0) { 
                TFEnergyProcedures.CleanseTFEnergies(player, cleanseBonusTFEnergyRemovalPercent);
            }




            string logmessage = "<span class='playerCleansingNotification'>" + player.GetFullName() + " cleansed here.</span>";
            LocationLogProcedures.AddLocationLog(player.dbLocationName, logmessage);


            Location here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            string playerLogMessage = "You cleansed at " + here.Name + ".";
            PlayerLogProcedures.AddPlayerLog(player.Id, playerLogMessage, false);

            return result;
        }

        public static string Meditate(Player player, BuffBox buffs)
        {

            decimal meditateManaRestore = PvPStatics.MeditateManaRestoreBase + buffs.MeditationExtraMana() + player.Level;

            if (meditateManaRestore < 0)
            {
                meditateManaRestore = 0;
            }

            PlayerProcedures.ChangePlayerActionMana(PvPStatics.MeditateCost, 0, meditateManaRestore, player.Id);

            PlayerProcedures.AddCleanseMeditateCount(player);

            string result = "";

            if (meditateManaRestore == 0)
            {
                result = "You try to meditate, but due to the magical effects on your body you fail to restore any mana.";
            }
            else
            {
                result = "You quickly meditate, restoring " + meditateManaRestore + " mana.";
            }

            string logmessage = "<span class='playerMediatingNotification'>" + player.GetFullName() + " meditated here.</span>";
            LocationLogProcedures.AddLocationLog(player.dbLocationName, logmessage);


            Location here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            string playerLogMessage = "You meditated at " + here.Name + ".";
            PlayerLogProcedures.AddPlayerLog(player.Id, playerLogMessage, false);

            return result;

        }

        public static string DeMeditate(Player player, Player mindcontroller, BuffBox buffs) {
            decimal meditateManaRestore = PvPStatics.MeditateManaRestoreBase + buffs.MeditationExtraMana() + player.Level;

            if (meditateManaRestore < 0)
            {
                meditateManaRestore = 0;
            }

            PlayerProcedures.ChangePlayerActionMana(PvPStatics.MeditateCost, 0, -meditateManaRestore, player.Id);


            string result = "Your mind partially possessed by " + mindcontroller.GetFullName() +", your head swims with strange and random thoughts implanted by your agressor, shattering your focus and leaving your mana drained.";
          

            PlayerLogProcedures.AddPlayerLog(player.Id, result, true);


            PlayerProcedures.AddCleanseMeditateCount(player);

            return result;

        }

        public static string SetRPFlag(Player player, bool turnOn)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            string message = "";

            if (turnOn)
            {
                dbPlayer.InRP = true;
                message = "You have turned on your RP flag.";
            }
            else
            {
                dbPlayer.InRP = false;
                message = "You have turned off your RP flag.";
            }
            playerRepo.SavePlayer(dbPlayer);

            return message;
        }

        public static string SetPvPFlag(Player player, int level)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            dbPlayer.GameMode = level;

            playerRepo.SavePlayer(dbPlayer);
            return "You are now in PvP mode.";
        }

        public static void AddCleanseMeditateCount(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbplayer.CleansesMeditatesThisRound++;
            playerRepo.SavePlayer(dbplayer);

        }

        public static void SetCleanseMeditateCount(Player player, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbplayer.CleansesMeditatesThisRound = amount;
            playerRepo.SavePlayer(dbplayer);

        }

        public static Player ReadjustMaxes(Player player, BuffBox buffs)
        {
            // readjust player health/mana by grabbing base amount plus effe cts from buffs
            player.MaxHealth = Convert.ToDecimal(PlayerProcedures.GetWillpowerBaseByLevel(player.Level)) * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
            player.MaxMana = Convert.ToDecimal(PlayerProcedures.GetManaBaseByLevel(player.Level)) * (1.0M + (buffs.ManaBonusPercent() / 100.0M));

            // keep player's health within proper bounds
            if (player.MaxHealth < 1)
            {
                player.MaxHealth = 1;
            }

            if (player.MaxMana < 1)
            {
                player.MaxMana = 1;
            }

            
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
            if (player.Mana > player.MaxMana)
            {
                player.Mana = player.MaxMana;
            }
            if (player.Health < 0)
            {
                player.Health = 0;
            }
            if (player.Mana < 0)
            {
                player.Mana = 0;
            }

            return player;
        }

        public static void GiveMoneyToPlayer(Player player, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.Money += amount;
            playerRepo.SavePlayer(dbPlayer);
         
        }

        public static int GetAnimatePlayerCountInCovenant(int covenantId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.Covenant == covenantId && p.Mobility == PvPStatics.MobilityFull).Count();
        }

        public static int RollDie(int size)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int num = 1 + rand.Next(size);
            return num;
        }

        public static string GivePlayerPvPScore(Player winner, Player loser, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == winner.Id);
            dbPlayer.PvPScore += amount;

            playerRepo.SavePlayer(dbPlayer);
            return "  You steal " + amount + " Dungeon Points from your victory over " + loser.GetFullName() + "!";
        }

        public static void GivePlayerPvPScore_NoLoser(Player player, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.PvPScore += amount;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static string RemovePlayerPvPScore(Player loser, Player attacker, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == loser.Id);

            // impose a greater net loss to discourage swapping
            amount = Math.Floor(amount * 1.5M);


            dbPlayer.PvPScore -= amount;

            if (dbPlayer.PvPScore < 0)
            {
                dbPlayer.PvPScore = 0;
            }

            // loser is in PvP mode and attacker is not; double the loss penalty
            //if (loser.GameMode < 2 && attacker.GameMode == 2)
            //{
            //    dbPlayer.PvPScore -= loss;
            //}

            playerRepo.SavePlayer(dbPlayer);
            return "  You have lost " + amount + " Dungeon Points from your defeat to " + attacker.GetFullName() + ".";
            //return "";
        }

        public static decimal GetPvPScoreFromWin(Player attacker, Player victim)
        {
            //decimal scoreFromLevel = 0;
            //decimal scoreFromSteal = 0;

            //int levelDiff = Math.Abs(attacker.Level - victim.Level);
            //if (levelDiff <= 3)
            //{
            //    //scoreFromLevel = victim.Level * 10;

            //    if (attacker.PvPScore / 2 <= victim.PvPScore) { 
            //        scoreFromSteal = Math.Floor(victim.PvPScore / 4);
            //    }
            //}

            decimal scoreFromSteal = Math.Floor(victim.PvPScore / 3);

           // return scoreFromLevel + scoreFromSteal;
            return scoreFromSteal;
        }

        public static void SetNickname(string nickname, string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);

            if (nickname != null && nickname.Length > 0)
            {
                player.Nickname = nickname;
            }
            else
            {
                player.Nickname = "";
            }
            
            playerRepo.SavePlayer(player);
        }

        public static void SetChatColor(Player player, string color)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player me = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            me.ChatColor = color;
            playerRepo.SavePlayer(me);
        }

        public static void ResetAllPlayersWithIPAddress(string ipAddress)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> players = playerRepo.Players.Where(p => p.IpAddress == ipAddress).ToList();
            foreach (Player p in players)
            {
                p.IpAddress = "reset";
                playerRepo.SavePlayer(p);
            }
        }

        public static void EnterDuel(int playerId, int duelId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.InDuel = duelId;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static IEnumerable<DbStaticForm> GetAllDbStaticForms()
        {
            IDbStaticFormRepository repo = new EFDbStaticFormRepository();
            return repo.DbStaticForms.Where(f => f.dbName != "");
        }

        public static void AddItemUses(int playerId, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.ItemsUsedThisTurn += amount;
            playerRepo.SavePlayer(dbPlayer);
        }

        /// <summary>
        /// Return a player's reserved name from the database if they have one.
        /// </summary>
        /// <param name="membershipId">Membership Id of the player whose reserved name is retrieved</param>
        /// <returns>ReservedName object if found, null if not</returns>
        public static ReservedName GetPlayerReservedName(string membershipId)
        {
            IReservedNameRepository resNameRepo = new EFReservedNameRepository();
            return resNameRepo.ReservedNames.FirstOrDefault(rn => rn.MembershipId == membershipId);
        }

    }
}