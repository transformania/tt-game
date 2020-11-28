using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Skills.Commands;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain.World.Queries;

namespace TT.Domain.Procedures
{
    public class PlayerProcedures
    {

        public static PlayerFormViewModel GetPlayerFormViewModel(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from player in playerRepo.Players
                                                      where player.Id == playerId
                                                      join form in playerRepo.DbStaticForms on player.FormSourceId equals form.Id
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = player.Id,
                                                              MembershipId = player.MembershipId,
                                                              FirstName = player.FirstName,
                                                              LastName = player.LastName,
                                                              dbLocationName = player.dbLocationName,
                                                              FormSourceId = player.FormSourceId,
                                                              Health = player.Health,
                                                              MaxHealth = player.MaxHealth,
                                                              Mana = player.Mana,
                                                              MaxMana = player.MaxMana,
                                                              ExtraInventory = player.ExtraInventory,
                                                              SneakPercent = player.SneakPercent,
                                                              MoveActionPointDiscount = player.MoveActionPointDiscount,
                                                              ActionPoints = player.ActionPoints,
                                                              ActionPoints_Refill = player.ActionPoints_Refill,
                                                              Gender = player.Gender,
                                                              Mobility = player.Mobility,
                                                              BotId = player.BotId,
                                                              MindControlIsActive = player.MindControlIsActive,
                                                              XP = player.XP,
                                                              Level = player.Level,
                                                              TimesAttackingThisUpdate = player.TimesAttackingThisUpdate,
                                                              IpAddress = player.IpAddress,
                                                              LastActionTimestamp = player.LastActionTimestamp,
                                                              LastCombatTimestamp = player.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = player.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = player.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = player.UnusedLevelUpPerks,
                                                              GameMode = player.GameMode,
                                                              InRP = player.InRP,
                                                              CleansesMeditatesThisRound = player.CleansesMeditatesThisRound,
                                                              Money = player.Money,
                                                              Covenant = player.Covenant,
                                                              OriginalFormSourceId = player.OriginalFormSourceId,
                                                              PvPScore = player.PvPScore,
                                                              DonatorLevel = player.DonatorLevel,
                                                              OnlineActivityTimestamp = player.OnlineActivityTimestamp,
                                                              Nickname = player.Nickname,
                                                              ShoutsRemaining = player.ShoutsRemaining,
                                                              ChatColor = player.ChatColor,
                                                              IsBannedFromGlobalChat = player.IsBannedFromGlobalChat,
                                                              InDuel = player.InDuel,
                                                              InQuest = player.InQuest,
                                                              InQuestState = player.InQuestState,


                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              FriendlyName = form.FriendlyName,
                                                              Description = form.Description,
                                                              TFEnergyType = form.TFEnergyType,
                                                              TFEnergyRequired = form.TFEnergyRequired,
                                                              Gender = form.Gender,
                                                              MobilityType = form.MobilityType,
                                                              ItemSourceId = form.ItemSourceId,
                                                              PortraitUrl = form.PortraitUrl,
                                                              SecondaryPortraitUrl = form.SecondaryPortraitUrl,
                                                              TertiaryPortraitUrl = form.TertiaryPortraitUrl,
                                                              IsUnique = form.IsUnique,

                                                              HealthBonusPercent = form.HealthBonusPercent,
                                                              ManaBonusPercent = form.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = form.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = form.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = form.ManaRecoveryPerUpdate,
                                                              SneakPercent = form.SneakPercent,
                                                              EvasionPercent = form.EvasionPercent,
                                                              EvasionNegationPercent = form.EvasionNegationPercent,
                                                              MeditationExtraMana = form.MeditationExtraMana,
                                                              CleanseExtraHealth = form.CleanseExtraHealth,
                                                              MoveActionPointDiscount = form.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = form.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = form.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = form.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = form.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = form.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = form.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = form.ExtraInventorySpace,

                                                              Discipline = form.Discipline,
                                                              Perception = form.Perception,
                                                              Charisma = form.Charisma,
                                                              Submission_Dominance = form.Submission_Dominance,

                                                              Fortitude = form.Fortitude,
                                                              Agility = form.Agility,
                                                              Allure = form.Allure,
                                                              Corruption_Purity = form.Corruption_Purity,

                                                              Magicka = form.Magicka,
                                                              Succour = form.Succour,
                                                              Luck = form.Luck,
                                                              Chaos_Order = form.Chaos_Order,
                                                          }

                                                      };

            return output.FirstOrDefault();
        }

        public static PlayerFormViewModel GetPlayerFormViewModel_FromMembership(string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from player in playerRepo.Players
                                                      where player.MembershipId == membershipId
                                                      join form in playerRepo.DbStaticForms on player.FormSourceId equals form.Id
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = player.Id,
                                                              MembershipId = player.MembershipId,
                                                              FirstName = player.FirstName,
                                                              LastName = player.LastName,
                                                              dbLocationName = player.dbLocationName,
                                                              FormSourceId = player.FormSourceId,
                                                              Health = player.Health,
                                                              MaxHealth = player.MaxHealth,
                                                              Mana = player.Mana,
                                                              MaxMana = player.MaxMana,
                                                              ExtraInventory = player.ExtraInventory,
                                                              SneakPercent = player.SneakPercent,
                                                              MoveActionPointDiscount = player.MoveActionPointDiscount,
                                                              ActionPoints = player.ActionPoints,
                                                              ActionPoints_Refill = player.ActionPoints_Refill,
                                                              Gender = player.Gender,
                                                              Mobility = player.Mobility,
                                                              BotId = player.BotId,
                                                              MindControlIsActive = player.MindControlIsActive,
                                                              XP = player.XP,
                                                              Level = player.Level,
                                                              TimesAttackingThisUpdate = player.TimesAttackingThisUpdate,
                                                              IpAddress = player.IpAddress,
                                                              LastActionTimestamp = player.LastActionTimestamp,
                                                              LastCombatTimestamp = player.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = player.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = player.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = player.UnusedLevelUpPerks,
                                                              GameMode = player.GameMode,
                                                              InRP = player.InRP,
                                                              CleansesMeditatesThisRound = player.CleansesMeditatesThisRound,
                                                              Money = player.Money,
                                                              Covenant = player.Covenant,
                                                              OriginalFormSourceId = player.OriginalFormSourceId,
                                                              PvPScore = player.PvPScore,
                                                              DonatorLevel = player.DonatorLevel,
                                                              OnlineActivityTimestamp = player.OnlineActivityTimestamp,
                                                              Nickname = player.Nickname,
                                                              ShoutsRemaining = player.ShoutsRemaining,
                                                              ChatColor = player.ChatColor,
                                                              IsBannedFromGlobalChat = player.IsBannedFromGlobalChat,
                                                              InDuel = player.InDuel,
                                                              InQuest = player.InQuest,
                                                              InQuestState = player.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              FriendlyName = form.FriendlyName,
                                                              Description = form.Description,
                                                              TFEnergyType = form.TFEnergyType,
                                                              TFEnergyRequired = form.TFEnergyRequired,
                                                              Gender = form.Gender,
                                                              MobilityType = form.MobilityType,
                                                              ItemSourceId = form.ItemSourceId,
                                                              PortraitUrl = form.PortraitUrl,
                                                              IsUnique = form.IsUnique,

                                                              HealthBonusPercent = form.HealthBonusPercent,
                                                              ManaBonusPercent = form.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = form.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = form.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = form.ManaRecoveryPerUpdate,
                                                              SneakPercent = form.SneakPercent,
                                                              EvasionPercent = form.EvasionPercent,
                                                              EvasionNegationPercent = form.EvasionNegationPercent,
                                                              MeditationExtraMana = form.MeditationExtraMana,
                                                              CleanseExtraHealth = form.CleanseExtraHealth,
                                                              MoveActionPointDiscount = form.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = form.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = form.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = form.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = form.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = form.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = form.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = form.ExtraInventorySpace,

                                                              Discipline = form.Discipline,
                                                              Perception = form.Perception,
                                                              Charisma = form.Charisma,
                                                              Submission_Dominance = form.Submission_Dominance,

                                                              Fortitude = form.Fortitude,
                                                              Agility = form.Agility,
                                                              Allure = form.Allure,
                                                              Corruption_Purity = form.Corruption_Purity,

                                                              Magicka = form.Magicka,
                                                              Succour = form.Succour,
                                                              Luck = form.Luck,
                                                              Chaos_Order = form.Chaos_Order,
                                                          }

                                                      };

            return output.First();
        }

        public static IEnumerable<PlayerFormViewModel> GetPlayerFormViewModelsAtLocation(string destinationDbName, string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from player in playerRepo.Players
                                                      where player.dbLocationName == destinationDbName && player.MembershipId != membershipId
                                                      join form in playerRepo.DbStaticForms on player.FormSourceId equals form.Id
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = player.Id,
                                                              MembershipId = player.MembershipId,
                                                              FirstName = player.FirstName,
                                                              LastName = player.LastName,
                                                              dbLocationName = player.dbLocationName,
                                                              FormSourceId = player.FormSourceId,
                                                              Health = player.Health,
                                                              MaxHealth = player.MaxHealth,
                                                              Mana = player.Mana,
                                                              MaxMana = player.MaxMana,
                                                              ExtraInventory = player.ExtraInventory,
                                                              SneakPercent = player.SneakPercent,
                                                              MoveActionPointDiscount = player.MoveActionPointDiscount,
                                                              ActionPoints = player.ActionPoints,
                                                              ActionPoints_Refill = player.ActionPoints_Refill,
                                                              Gender = player.Gender,
                                                              Mobility = player.Mobility,
                                                              BotId = player.BotId,
                                                              MindControlIsActive = player.MindControlIsActive,

                                                              XP = player.XP,
                                                              Level = player.Level,
                                                              TimesAttackingThisUpdate = player.TimesAttackingThisUpdate,
                                                              IpAddress = player.IpAddress,
                                                              LastActionTimestamp = player.LastActionTimestamp,
                                                              LastCombatTimestamp = player.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = player.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = player.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = player.UnusedLevelUpPerks,
                                                              GameMode = player.GameMode,
                                                              InRP = player.InRP,
                                                              CleansesMeditatesThisRound = player.CleansesMeditatesThisRound,
                                                              Money = player.Money,
                                                              Covenant = player.Covenant,
                                                              OriginalFormSourceId = player.OriginalFormSourceId,
                                                              PvPScore = player.PvPScore,
                                                              DonatorLevel = player.DonatorLevel,
                                                              OnlineActivityTimestamp = player.OnlineActivityTimestamp,
                                                              Nickname = player.Nickname,
                                                              ShoutsRemaining = player.ShoutsRemaining,
                                                              ChatColor = player.ChatColor,
                                                              IsBannedFromGlobalChat = player.IsBannedFromGlobalChat,
                                                              InDuel = player.InDuel,
                                                              InQuest = player.InQuest,
                                                              InQuestState = player.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              FriendlyName = form.FriendlyName,
                                                              Description = form.Description,
                                                              TFEnergyType = form.TFEnergyType,
                                                              TFEnergyRequired = form.TFEnergyRequired,
                                                              Gender = form.Gender,
                                                              MobilityType = form.MobilityType,
                                                              ItemSourceId = form.ItemSourceId,
                                                              PortraitUrl = form.PortraitUrl,
                                                              IsUnique = form.IsUnique,

                                                              HealthBonusPercent = form.HealthBonusPercent,
                                                              ManaBonusPercent = form.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = form.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = form.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = form.ManaRecoveryPerUpdate,
                                                              SneakPercent = form.SneakPercent,
                                                              EvasionPercent = form.EvasionPercent,
                                                              EvasionNegationPercent = form.EvasionNegationPercent,
                                                              MeditationExtraMana = form.MeditationExtraMana,
                                                              CleanseExtraHealth = form.CleanseExtraHealth,
                                                              MoveActionPointDiscount = form.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = form.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = form.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = form.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = form.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = form.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = form.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = form.ExtraInventorySpace,

                                                              Discipline = form.Discipline,
                                                              Perception = form.Perception,
                                                              Charisma = form.Charisma,
                                                              Submission_Dominance = form.Submission_Dominance,

                                                              Fortitude = form.Fortitude,
                                                              Agility = form.Agility,
                                                              Allure = form.Allure,
                                                              Corruption_Purity = form.Corruption_Purity,

                                                              Magicka = form.Magicka,
                                                              Succour = form.Succour,
                                                              Luck = form.Luck,
                                                              Chaos_Order = form.Chaos_Order,

                                                          }

                                                      };

            return output;
        }

        public static IEnumerable<PlayerFormViewModel> GetPlayerFormViewModelsInCovenant(int covenantId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from player in playerRepo.Players
                                                      where player.Covenant == covenantId
                                                      join form in playerRepo.DbStaticForms on player.FormSourceId equals form.Id
                                                      select new PlayerFormViewModel
                                                      {
                                                          Player = new Player_VM
                                                          {
                                                              Id = player.Id,
                                                              MembershipId = player.MembershipId,
                                                              FirstName = player.FirstName,
                                                              LastName = player.LastName,
                                                              dbLocationName = player.dbLocationName,
                                                              FormSourceId = player.FormSourceId,
                                                              Health = player.Health,
                                                              MaxHealth = player.MaxHealth,
                                                              Mana = player.Mana,
                                                              MaxMana = player.MaxMana,
                                                              ExtraInventory = player.ExtraInventory,
                                                              SneakPercent = player.SneakPercent,
                                                              MoveActionPointDiscount = player.MoveActionPointDiscount,
                                                              ActionPoints = player.ActionPoints,
                                                              ActionPoints_Refill = player.ActionPoints_Refill,
                                                              Gender = player.Gender,
                                                              Mobility = player.Mobility,
                                                              BotId = player.BotId,
                                                              MindControlIsActive = player.MindControlIsActive,

                                                              XP = player.XP,
                                                              Level = player.Level,
                                                              TimesAttackingThisUpdate = player.TimesAttackingThisUpdate,
                                                              IpAddress = player.IpAddress,
                                                              LastActionTimestamp = player.LastActionTimestamp,
                                                              LastCombatTimestamp = player.LastCombatTimestamp,
                                                              LastCombatAttackedTimestamp = player.LastCombatAttackedTimestamp,
                                                              FlaggedForAbuse = player.FlaggedForAbuse,
                                                              UnusedLevelUpPerks = player.UnusedLevelUpPerks,
                                                              GameMode = player.GameMode,
                                                              InRP = player.InRP,
                                                              CleansesMeditatesThisRound = player.CleansesMeditatesThisRound,
                                                              Money = player.Money,
                                                              Covenant = player.Covenant,
                                                              OriginalFormSourceId = player.OriginalFormSourceId,
                                                              PvPScore = player.PvPScore,
                                                              DonatorLevel = player.DonatorLevel,
                                                              OnlineActivityTimestamp = player.OnlineActivityTimestamp,
                                                              Nickname = player.Nickname,
                                                              ShoutsRemaining = player.ShoutsRemaining,
                                                              ChatColor = player.ChatColor,
                                                              IsBannedFromGlobalChat = player.IsBannedFromGlobalChat,
                                                              InDuel = player.InDuel,
                                                              InQuest = player.InQuest,
                                                              InQuestState = player.InQuestState,
                                                          },

                                                          Form = new TT.Domain.ViewModels.Form
                                                          {
                                                              FriendlyName = form.FriendlyName,
                                                              Description = form.Description,
                                                              TFEnergyType = form.TFEnergyType,
                                                              TFEnergyRequired = form.TFEnergyRequired,
                                                              Gender = form.Gender,
                                                              MobilityType = form.MobilityType,
                                                              ItemSourceId = form.ItemSourceId,
                                                              PortraitUrl = form.PortraitUrl,
                                                              IsUnique = form.IsUnique,

                                                              HealthBonusPercent = form.HealthBonusPercent,
                                                              ManaBonusPercent = form.ManaBonusPercent,
                                                              ExtraSkillCriticalPercent = form.ExtraSkillCriticalPercent,
                                                              HealthRecoveryPerUpdate = form.HealthRecoveryPerUpdate,
                                                              ManaRecoveryPerUpdate = form.ManaRecoveryPerUpdate,
                                                              SneakPercent = form.SneakPercent,
                                                              EvasionPercent = form.EvasionPercent,
                                                              EvasionNegationPercent = form.EvasionNegationPercent,
                                                              MeditationExtraMana = form.MeditationExtraMana,
                                                              CleanseExtraHealth = form.CleanseExtraHealth,
                                                              MoveActionPointDiscount = form.MoveActionPointDiscount,
                                                              SpellExtraHealthDamagePercent = form.SpellExtraHealthDamagePercent,
                                                              SpellExtraTFEnergyPercent = form.SpellExtraTFEnergyPercent,
                                                              CleanseExtraTFEnergyRemovalPercent = form.CleanseExtraTFEnergyRemovalPercent,
                                                              SpellMisfireChanceReduction = form.SpellMisfireChanceReduction,
                                                              SpellHealthDamageResistance = form.SpellHealthDamageResistance,
                                                              SpellTFEnergyDamageResistance = form.SpellTFEnergyDamageResistance,
                                                              ExtraInventorySpace = form.ExtraInventorySpace,

                                                              Discipline = form.Discipline,
                                                              Perception = form.Perception,
                                                              Charisma = form.Charisma,
                                                              Submission_Dominance = form.Submission_Dominance,

                                                              Fortitude = form.Fortitude,
                                                              Agility = form.Agility,
                                                              Allure = form.Allure,
                                                              Corruption_Purity = form.Corruption_Purity,

                                                              Magicka = form.Magicka,
                                                              Succour = form.Succour,
                                                              Luck = form.Luck,
                                                              Chaos_Order = form.Chaos_Order,
                                                          }

                                                      };

            return output;
        }

        public static Player GetPlayerFromMembership(string id)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == id);

            return player;

        }

        public static Player GetPlayerFromBotId(int id)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.BotId == id);

            return player;

        }

        public static Player GetPlayer(int? playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            return player;
        }

        public static string SaveNewPlayer(NewCharacterViewModel newCharacterViewModel, string membershipId)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();

            var noGenerationLastName = newCharacterViewModel.LastName.Split(' ')[0];

            var ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == noGenerationLastName);

            if (ghost != null && ghost.BotId != AIStatics.RerolledPlayerBotId && ghost.MembershipId != membershipId)
            {
                return "A character of this name already exists.";
            }

            var generationTitle = "";

            if (ghost != null && (ghost.BotId == AIStatics.RerolledPlayerBotId || ghost.MembershipId == membershipId) && ghost.FirstName == newCharacterViewModel.FirstName && ghost.LastName == newCharacterViewModel.LastName)
            {

                var possibleOldGens = playerRepo.Players.Where(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName.Contains(newCharacterViewModel.LastName)).ToList();

                if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName) == null)
                {
                    generationTitle = " II";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " II") == null) {
                    generationTitle = " II";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " III") == null)
                {
                    generationTitle = " III";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " IV") == null)
                {
                    generationTitle = " IV";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " V") == null)
                {
                    generationTitle = " V";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " VI") == null)
                {
                    generationTitle = " VI";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " VII") == null)
                {
                    generationTitle = " VII";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " VIII") == null)
                {
                    generationTitle = " VIII";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " IX") == null)
                {
                    generationTitle = " IX";
                }
                else if (possibleOldGens.FirstOrDefault(p => p.FirstName == newCharacterViewModel.FirstName && p.LastName == newCharacterViewModel.LastName + " X") == null)
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
            var resNameGhost = resNameRepo.ReservedNames.FirstOrDefault(r => r.FullName == newCharacterViewModel.FirstName + " " + newCharacterViewModel.LastName);

            if (resNameGhost != null && resNameGhost.MembershipId != membershipId)
            {
                return "This name has been reserved by a different player.  Choose another.";
            }

            // assert that the form is a valid staring form
            if (!DomainRegistry.Repository.FindSingle(new IsBaseForm {formSourceId = newCharacterViewModel.FormSourceId}))
            {
                return "That is not a valid starting form.";
            }

            Player vendor = null;
            if (newCharacterViewModel.InanimateForm != null && newCharacterViewModel.StartAsInanimate)
            {
                if (newCharacterViewModel.InanimateForm.ToString() == "pet" || newCharacterViewModel.InanimateForm.ToString() == "random")
                {
                    vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);
                    if (vendor == null)
                    {
                        return "Wüffie is not currently available to accept new pets. Please try again later.";
                    }
                }
                if (newCharacterViewModel.InanimateForm.ToString() != "pet")
                {
                    vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
                    if (vendor == null)
                    {
                        return "Lindella is not currently available to accept new items. Please try again later.";
                    }
                }
            }

            // remove the old Player--Membership binding
            var oldplayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);

            int? oldCovId = null;

            if (oldplayer != null)
            {
                var rerollTime = RerollProcedures.GetTimeUntilReroll(oldplayer);
                if (rerollTime.TotalSeconds > 0)
                {
                    return "It is too soon for you to start again. Please try again in " + rerollTime.ToString(@"hh\:mm\:ss") + ".";
                }

                // remove all of the old player's logs
                PlayerLogProcedures.ClearPlayerLog(oldplayer.Id);

                // delete the achivements for that player that get reset upon reroll, such as reading tomes or passing quests
                StatsProcedures.DeleteAchivemenstOfTypeForPlayer(oldplayer, StatsProcedures.GetAchivementNamesThatReset());

                // remove all of the old player's TF energies
                TFEnergyProcedures.DeleteAllPlayerTFEnergies(oldplayer.Id);
                oldplayer.MembershipId = null;
                oldplayer.BotId = AIStatics.RerolledPlayerBotId;

                // remove the old player's effects
                EffectProcedures.DeleteAllPlayerEffects(oldplayer.Id);

                oldCovId = oldplayer.Covenant;
                oldplayer.Covenant = null;
                playerRepo.SavePlayer(oldplayer);

                // turn the item they player became permanent
                IItemRepository itemRepo = new EFItemRepository();
                var oldItemMeHack = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = oldplayer.Id});
                var oldItemMe = itemRepo.Items.FirstOrDefault(i => i.Id == oldItemMeHack.Id);
                oldItemMe.IsPermanent = true;
                oldItemMe.LastSouledTimestamp = DateTime.UtcNow.AddYears(1);
                itemRepo.SaveItem(oldItemMe);
                DomainRegistry.Repository.Execute(new RemoveSoulbindingOnPlayerItems {PlayerId = oldplayer.Id});
                DomainRegistry.Repository.Execute(new SetSoulbindingConsent { IsConsenting = true, PlayerId = oldplayer.Id });
            }


            // clean the name entered by the player, capitalize first letter and downcase the rest
            var cleanFirstName = char.ToUpper(newCharacterViewModel.FirstName[0]) + newCharacterViewModel.FirstName.Substring(1).ToLower();
            var cleanLastName = char.ToUpper(newCharacterViewModel.LastName[0]) + newCharacterViewModel.LastName.Substring(1).ToLower();

            newCharacterViewModel.FirstName = cleanFirstName;
            newCharacterViewModel.LastName = cleanLastName + generationTitle;


            var cmd = new CreatePlayer
            {
                FirstName = newCharacterViewModel.FirstName,
                LastName = newCharacterViewModel.LastName,
                Gender = newCharacterViewModel.Gender,
                Location = "coffee_shop",
                UserId = membershipId,
                BotId = AIStatics.ActivePlayerBotId,
                GameMode = newCharacterViewModel.StartGameMode,
                InRP = newCharacterViewModel.StartInRP,
                FormSourceId = newCharacterViewModel.FormSourceId
            };

            // if player is not choosing to start in an inanimate/pet form, start them off in Welcome to Sunnyglade quest
            if (newCharacterViewModel.InanimateForm == null)
            {
                cmd.InQuest = 6; // Welcome to Sunnyglade quest
                cmd.InQuestState = 93; // first stage of Welcome to Sunnyglade
            }

            if (oldplayer != null)
            {
                cmd.Covenant = oldCovId;
                cmd.Level = oldplayer.Level - 3;
                if (cmd.Level < 1)
                {
                    cmd.Level = 1;
                }
                cmd.UnusedLevelUpPerks = cmd.Level - 1;
                cmd.ChatColor = oldplayer.ChatColor;

            }

            cmd.Location = LocationsStatics.GetRandomLocationNotInDungeon();

            var newPlayerId = DomainRegistry.Repository.Execute(cmd);
            RerollProcedures.AddRerollGeneration(cmd.UserId);

            if (oldplayer != null)
            {
                // transfer all of the old player's skills that are NOT form specific or weaken
                SkillProcedures.TransferAllPlayerSkills(oldplayer.Id, newPlayerId);

                // transfer their old messages to new account
                if (newCharacterViewModel.MigrateLetters)
                {
                    using (var context = new StatsContext())
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Messages] SET ReceiverId = " + newPlayerId + " WHERE ReceiverId = " + oldplayer.Id);
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Messages] SET SenderId = " + newPlayerId + " WHERE SenderId = " + oldplayer.Id);
                    }

                }

            }

            // if the player was in a covenant, they might have been the leader.  Check this and make a new player the leader
            if (oldCovId != null && oldCovId > 0)
            {
                ICovenantRepository covRepo = new EFCovenantRepository();
                var oldCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == oldCovId);

                // we need to regrab the new player from the repo again to get their Id
                var newmeFromDb = PlayerProcedures.GetPlayerWithExactName(cmd.FirstName + " " + cmd.LastName);

                if (oldCovenant != null && oldCovenant.LeaderId == oldplayer.Id)
                {
                    oldCovenant.LeaderId = newmeFromDb.Id;
                    covRepo.SaveCovenant(oldCovenant);
                }

            }

            DomainRegistry.Repository.Execute(new CreateSkill {ownerId = newPlayerId, skillSourceId = SkillStatics.WeakenSkillSourceId });

            if (newCharacterViewModel.InanimateForm != null)
            {
                var startform = ItemProcedures.GetFormFromItem(ItemProcedures.GetRandomItemOfType(newCharacterViewModel.InanimateForm.ToString()));
                if (newCharacterViewModel.InanimateForm.ToString() == "random" && startform.MobilityType == "animal") vendor = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);

                DomainRegistry.Repository.Execute(new ChangeForm
                {
                    PlayerId = newPlayerId,
                    FormSourceId = startform.Id
                });

                var newplayer = playerRepo.Players.FirstOrDefault(p => p.Id == newPlayerId);
                newplayer.Health = 0;
                newplayer.Mana = 0;
                newplayer.ActionPoints = TurnTimesStatics.GetActionPointLimit();

                playerRepo.SavePlayer(newplayer);
                ItemProcedures.PlayerBecomesItem(newplayer, startform, vendor);
                ItemProcedures.LockItem(newplayer);
            }
            return "saved";

        }

        public static void SetCustomBase(Player player, int newFormSourceId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.OriginalFormSourceId = newFormSourceId;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static bool CheckAllowedInDungeon(Player me, out string message)
        {
            // assert player is in PvP mode
            if (me.GameMode < (int)GameModeStatics.GameModes.PvP)
            {
                message = "You must be in PvP mode in order to enter the dungeon.  It is not a safe place...";
                return false;
            }

            // assert player is at least level 4
            if (me.Level < 4)
            {
                message = "You must be at least level four to enter the dungeon.";
                return false;
            }

            // assert player is not currently under the effects of back on your feet
            if (EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_BackOnYourFeetSourceId))
            {
                message = "You must wait until you are no longer under the effects of 'Back on your feet'.";
                return false;
            }

            message = "";
            return true;
        }

        public static void InstantRestoreToBase(Player player)
        {

           
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = player.Id,
                FormSourceId = player.OriginalFormSourceId
            });

            if (player.Mobility != PvPStatics.MobilityFull)
            {
                var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = player.Id });

                if (itemMe != null)
                {
                    // drop any runes embedded on the newCharacterViewModel-item, or return them to the former owner's inventory
                    DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = itemMe.Id });
                    DomainRegistry.Repository.Execute(new DeleteItem { ItemId = itemMe.Id });
                }
            }

            SkillProcedures.UpdateFormSpecificSkillsToPlayer(player, player.OriginalFormSourceId);

            var message = "";
            if (player.IsInDungeon() && !CheckAllowedInDungeon(player, out message))
            {
                var overworldLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                TeleportPlayer(player, overworldLocation, false);
            }
        }

        public static void InstantChangeToForm(Player player, int formSourceId)
        {
            var oldFormSourceId = player.FormSourceId;
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = player.Id,
                FormSourceId = formSourceId
            });
            SkillProcedures.UpdateFormSpecificSkillsToPlayer(player, formSourceId);
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

            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.OnlineActivityTimestamp = DateTime.UtcNow;

            playerRepo.SavePlayer(dbPlayer);
        }

        public static void MovePlayer_InstantNoLog(int playerId, string newLocation)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.dbLocationName = newLocation;
            playerRepo.SavePlayer(player);
        }

        public static void MovePlayerMultipleLocations(Player player, string destinationDbName, decimal actionPointCost, bool timestamp = true)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            AIProcedures.MoveTo(dbPlayer, destinationDbName, 100000);
            dbPlayer.ActionPoints -= actionPointCost;
            dbPlayer.dbLocationName = destinationDbName;
            if(timestamp)
            {
                dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            }
            playerRepo.SavePlayer(dbPlayer);
        }

        public static string TeleportPlayer(Player player, string destination, bool showDestinationInLocationLog)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            var oldLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == user.dbLocationName);
            var newLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == destination);

            user.dbLocationName = destination;
            playerRepo.SavePlayer(user);

            var playerLogMessage = "";
            var locationMessageOld = "";
            var locationMessageNew = "";

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

        public static void ChangePlayerActionMana(decimal actionPoints, decimal health, decimal mana, int playerId, bool timestamp = true)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.ActionPoints -= actionPoints;
            player.Mana += mana;
            player.Health += health;

            if(timestamp) 
            {
                player.LastActionTimestamp = DateTime.UtcNow;
            }

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

        public static string DamagePlayerHealth(int playerId, decimal amount)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.Health -= amount;

            if (player.Health < 0)
            {
                player.Health = 0;
            }

            playerRepo.SavePlayer(player);

            return "You lower " + player.FirstName + "'s willpower by " + amount + ".  ";

        }

        public static string SearchLocation(Player player, string dbLocationName, bool findSpellsOnly)
        {

            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            // check to see if this is a location that has a summonable boss.  If so, do the random roll for it
            if (BossSummonDictionary.GlobalBossSummonDictionary.ContainsKey(dbLocationName))
            {
                // check and see if any other boss events are active
                if (!PvPWorldStatProcedures.IsAnyBossActive())
                {
                    var world = DomainRegistry.Repository.FindSingle(new GetWorld());
                    if (roll < 1 && dbLocationName == "ranch_bedroom")
                    {
                        if (world.IsDonnaAvailable())
                        {
                            BossProcedures_Donna.SpawnDonna();
                            PvPWorldStatProcedures.Boss_StartDonna();
                            var summontext = BossSummonDictionary.GetActivationText("Donna");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }

                    }
                    else if (roll < 1 && dbLocationName == "forest_pinecove") //Normally castle_armory
                    {
                        if (world.IsValentineAvailable())
                        {
                            BossProcedures_Valentine.SpawnValentine();
                            /* To stop the prevention of other bosses from spawning,
                             * opting to comment this out. I'm not removing it because
                             * this is still a reskin in my mind and the boss may be
                             * returned at some point in some similar capacity.
                             * 
                             * PvPWorldStatProcedures.Boss_StartValentine();
                            */
                            var summontext = BossSummonDictionary.GetActivationText("Valentine");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "stripclub_bar_seats")
                    {
                        if (world.IsBimboAvailable())
                        {
                            BossProcedures_BimboBoss.SpawnBimboBoss();
                            PvPWorldStatProcedures.Boss_StartBimbo();
                            var summontext = BossSummonDictionary.GetActivationText("BimboBoss");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "tavern_pool")
                    {
                        if (world.IsTheifAvailable())
                        {
                            BossProcedures_Thieves.SpawnThieves();
                            PvPWorldStatProcedures.Boss_StartThieves();
                            var summontext = BossSummonDictionary.GetActivationText("Thieves");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == "college_foyer")
                    {
                        if (world.IsSistersAvailable())
                        {
                            BossProcedures_Sisters.SpawnSisters();
                            PvPWorldStatProcedures.Boss_StartSisters();
                            var summontext = BossSummonDictionary.GetActivationText("Sisters");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == BossProcedures_FaeBoss.SpawnLocation)
                    {
                        if (world.IsFaeBossAvailable())
                        {
                            BossProcedures_FaeBoss.SpawnFaeBoss();
                            PvPWorldStatProcedures.Boss_StartFaeBoss();
                            var summontext = BossSummonDictionary.GetActivationText("FaeBoss");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                    else if (roll < 1 && dbLocationName == BossProcedures_MotorcycleGang.SpawnLocation)
                    {
                        if (world.IsMotorCycleGangBossAvailable())
                        {
                            BossProcedures_MotorcycleGang.Spawn();
                            PvPWorldStatProcedures.Boss_StartMotorcycleGang();
                            var summontext = BossSummonDictionary.GetActivationText("RoadQueen");
                            PlayerLogProcedures.AddPlayerLog(player.Id, summontext, true);
                            return summontext;
                        }
                    }
                }
            }

            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == dbLocationName);


            // learn a new skill
            if (roll < 30 || findSpellsOnly)
            {

                IEnumerable<DbStaticSkill> eligibleSkills;

                var myKnownSkills = SkillProcedures.GetStaticSkillsOwnedByPlayer(player.Id);

                // get all the skills that are found in THIS EXACT LOCATION
                var skillsAtThisLocation = SkillStatics.GetSkillsLearnedAtLocation(here.dbName);
                eligibleSkills = from s in skillsAtThisLocation
                                 let sx = myKnownSkills.Select(r => r.Id)
                                 where !sx.Contains(s.Id)
                                 select s;

                // get all the skills that are found in the region this location is in
                if (!eligibleSkills.Any())
                {
                    var skillsAtThisRegion = SkillStatics.GetSkillsLearnedAtRegion(here.Region);
                    eligibleSkills = from s in skillsAtThisRegion
                                     let sx = myKnownSkills.Select(r => r.Id)
                                     where !sx.Contains(s.Id)
                                     select s;
                }

                // there are no new spells to be learned that are not-region specific, so player is just out of luck.
                if (!eligibleSkills.Any())
                {
                    return "You get the feeling there are no new spells for you to discover around here.";
                }


                double max = eligibleSkills.Count();
                var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                var skillToLearn = eligibleSkills.ElementAt(randIndex);
                var output = SkillProcedures.GiveSkillToPlayer(player.Id, skillToLearn.Id);

                return output;


            }

            // give the player some money (30-48)
            else if (roll < 48)
            {
                var moneyamount = Convert.ToDecimal(1 + Math.Floor(rand.NextDouble() * 3));
                GiveMoneyToPlayer(player, moneyamount);
                return "You collected " + (int)moneyamount + " Arpeyjis that were scattered on the ground.";


            }

            // find a findable item (48-60)
            else if (roll < 60)
            {
                var justFound = ItemProcedures.GetRandomFindableItem();

                var output = ItemProcedures.GiveNewItemToPlayer(player, justFound);

                // drop an item of the same type that you are carrying if you are over the limit
                if (ItemProcedures.PlayerIsCarryingTooMuch(player))
                {
                    var randomItem = ItemProcedures.GetAllPlayerItems(player.Id).Last(i => i.dbItem.ItemSourceId == justFound.Id);
                    ItemProcedures.DropItem(randomItem.dbItem.Id);
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
                var effectsHere = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToList();

                if (!effectsHere.Any())
                {
                    PlayerProcedures.GiveXP(player, 1.5M);
                    return "Although you didn't find anything or learn any new spells, you feel as though you know this town a little better, which may come in useful in the future.  (+1.5 XP)";
                }
                else
                {


                    // give the player a random effect found here
                    double max = effectsHere.Count();
                    var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                    var effectToGet = effectsHere.ElementAt(randIndex);

                    // assert that the player doesn't already have this effect.  IF they do, break out
                    if (EffectProcedures.PlayerHasEffect(player, effectToGet.Id))
                    {
                        PlayerProcedures.GiveXP(player, 1.5M);
                        return "Although you didn't find anything or learn any new spells, you feel as though you know this town a little better, which may come in useful in the future.  (+1.5 XP)";
                    }

                    return EffectProcedures.GivePerkToPlayer(effectToGet.Id, player);

                }

            }

            return "Unfortunately, you did not find anything useful.";


        }

        public static WorldStats GetWorldPlayerStats()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var players = playerRepo.Players;

            var cutoff = DateTime.UtcNow.AddHours(-1);

            var output = new WorldStats
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

            // WARNING:  There is a nearly identical method to this on the new Player entity with the same name.  Updates to the logic here
            // must also be done there to keep new code consistent with legacy code.

            float xp = 11 * level * level + 0 + 89;
            var leftover = xp % 10;

            xp = (float)Math.Round(xp / 10) * 10;

            if (leftover != 0)
            {
                xp += 10;
            }

            return xp;
        }

        public static float GetManaBaseByLevel(int level)
        {
            float manaBase = 210;

            return manaBase;
        }

        public static float GetWillpowerBaseByLevel(int level)
        {
            var willpowerBase = 1000;

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
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            if (dbPlayer == null || dbPlayer.Level >= PvPStatics.MaxPlayerLevel)
            {
                return "";
            }
            // decrease XP gain by 40% for psychos
            if (dbPlayer.BotId == AIStatics.PsychopathBotId)
            {
                amount = amount * .6M;
            }

            dbPlayer.XP += amount;

            var output = "";

            if (amount > 0)
            {
                var xpNeeded = GetXPNeededForLevelUp(dbPlayer.Level);

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
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate += amount;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(player);
        }

        public static void SetAttackCount(Player player, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate = amount;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(player);
        }

        public static void LogCombatTimestampsAndAddAttackCount(Player victim, Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbvictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);
           // dbvictim.LastCombatTimestamp = DateTime.UtcNow;
           // playerRepo.SavePlayer(dbvictim);

            var dbAttacker = playerRepo.Players.FirstOrDefault(p => p.Id == attacker.Id);
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
            var dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbplayer.LastActionTimestamp = DateTime.UtcNow;
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
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);
            dbPlayer.IpAddress = ip;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static bool IsMyIPInUseAndAnimate(string ip)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var num = playerRepo.Players.Count(p => p.IpAddress == ip && p.Mobility == PvPStatics.MobilityFull);
            return num > 1;
        }

        public static bool IsMyIPInUseAndAnimate(string ip, Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var num = playerRepo.Players.Count(p => p.BotId == AIStatics.ActivePlayerBotId && p.IpAddress == ip && p.Mobility == PvPStatics.MobilityFull && p.GameMode == player.GameMode);
            return num > 1;
        }

        public static bool PlayerIsOffline(Player player)
        {
            if (player.BotId < AIStatics.RerolledPlayerBotId)
            {
                return false;
            }

            var minutesAgo = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (minutesAgo > TurnTimesStatics.GetOfflineAfterXMinutes())
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

            var minutesAgo = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (minutesAgo > TurnTimesStatics.GetOfflineAfterXMinutes())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Player GetPlayerWithExactName(string fullname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cleanedName = fullname.ToLower();
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
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
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

        public static string DeMeditate(Player player, Player mindcontroller, BuffBox buffs) {
            var meditateManaRestore = PvPStatics.MeditateManaRestoreBase + buffs.MeditationExtraMana() + player.Level;

            if (meditateManaRestore < 0)
            {
                meditateManaRestore = 0;
            }

            PlayerProcedures.ChangePlayerActionMana(PvPStatics.MeditateCost, 0, -meditateManaRestore, player.Id, false);


            var result = "Your mind partially possessed by " + mindcontroller.GetFullName() +", your head swims with strange and random thoughts implanted by your agressor, shattering your focus and leaving your mana drained.";
          

            PlayerLogProcedures.AddPlayerLog(player.Id, result, true);


            PlayerProcedures.AddCleanseMeditateCount(player);

            return result;

        }

        public static void AddCleanseMeditateCount(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbplayer.CleansesMeditatesThisRound++;
            playerRepo.SavePlayer(dbplayer);

        }

        public static void SetCleanseMeditateCount(Player player, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
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

            player.ExtraInventory = buffs.ExtraInventorySpace();
            player.SneakPercent = buffs.SneakPercent();
            player.MoveActionPointDiscount = buffs.MoveActionPointDiscount();

            return player;
        }

        public static void GiveMoneyToPlayer(Player player, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.Money += amount;
            playerRepo.SavePlayer(dbPlayer);
         
        }

        public static int GetAnimatePlayerCountInCovenant(int covenantId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Count(p => p.Covenant == covenantId && p.Mobility == PvPStatics.MobilityFull);
        }

        public static int RollDie(int size)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var num = 1 + rand.Next(size);
            return num;
        }

        public static string GivePlayerPvPScore(Player winner, Player loser, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == winner.Id);
            dbPlayer.PvPScore += amount;

            playerRepo.SavePlayer(dbPlayer);
            return "  You steal " + amount + " Dungeon Points from your victory over " + loser.GetFullName() + "!";
        }

        public static void GivePlayerPvPScore_NoLoser(Player player, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.PvPScore += amount;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static string RemovePlayerPvPScore(Player loser, Player attacker, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == loser.Id);

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

            var scoreFromSteal = Math.Floor(victim.PvPScore / 3);

           // return scoreFromLevel + scoreFromSteal;
            return scoreFromSteal;
        }

        public static void SetNickname(string nickname, string membershipId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == membershipId);

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
            var me = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            me.ChatColor = color;
            playerRepo.SavePlayer(me);
        }

        public static void EnterDuel(int playerId, int duelId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.InDuel = duelId;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            dbPlayer.LastCombatTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static IEnumerable<DbStaticForm> GetAllDbStaticForms()
        {
            IDbStaticFormRepository repo = new EFDbStaticFormRepository();
            return repo.DbStaticForms;
        }

        public static void AddItemUses(int playerId, int amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
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