using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Models;
using tfgame.Procedures.BossProcedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Procedures
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
                                                              ResistanceModifier = p.ResistanceModifier,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              IsItemId = p.IsItemId,
                                                              IsPetToId = p.IsPetToId,
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
                                                              InPvP = p.InPvP,
                                                              GameMode = p.GameMode,
                                                              NonPvP_GameoverSpellsAllowed = p.NonPvP_GameoverSpellsAllowed,
                                                              NonPvP_GameOverSpellsAllowedLastChange = p.NonPvP_GameOverSpellsAllowedLastChange,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                          },

                                                          Form = new tfgame.ViewModels.Form
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
                                                          }

                                                      };

            return output.First();
        }

        public static IEnumerable<PlayerFormViewModel> GetPlayerFormViewModelsAtLocation(string destinationDbName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<PlayerFormViewModel> output = from p in playerRepo.Players
                                                      where p.dbLocationName == destinationDbName && p.MembershipId != WebSecurity.CurrentUserId
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
                                                              ResistanceModifier = p.ResistanceModifier,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              IsItemId = p.IsItemId,
                                                              IsPetToId = p.IsPetToId,

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
                                                              InPvP = p.InPvP,
                                                              GameMode = p.GameMode,
                                                              NonPvP_GameoverSpellsAllowed = p.NonPvP_GameoverSpellsAllowed,
                                                              NonPvP_GameOverSpellsAllowedLastChange = p.NonPvP_GameOverSpellsAllowedLastChange,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                          },

                                                          Form = new tfgame.ViewModels.Form
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
                                                              ResistanceModifier = p.ResistanceModifier,
                                                              Gender = p.Gender,
                                                              Mobility = p.Mobility,
                                                              IsItemId = p.IsItemId,
                                                              IsPetToId = p.IsPetToId,

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
                                                              InPvP = p.InPvP,
                                                              GameMode = p.GameMode,
                                                              NonPvP_GameoverSpellsAllowed = p.NonPvP_GameoverSpellsAllowed,
                                                              NonPvP_GameOverSpellsAllowedLastChange = p.NonPvP_GameOverSpellsAllowedLastChange,
                                                              InRP = p.InRP,
                                                              CleansesMeditatesThisRound = p.CleansesMeditatesThisRound,
                                                              Money = p.Money,
                                                              Covenant = p.Covenant,
                                                              OriginalForm = p.OriginalForm,
                                                              PvPScore = p.PvPScore,
                                                              DonatorLevel = p.DonatorLevel,
                                                              OnlineActivityTimestamp = p.OnlineActivityTimestamp,
                                                              Nickname = p.Nickname,
                                                          },

                                                          Form = new tfgame.ViewModels.Form
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
                                                          }

                                                      };

            return output;
        }

        public static Player GetPlayerFromMembership(int id)
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == id);

            return player;

        }

        public static Player GetPlayerFromMembership()
        {

            return GetPlayerFromMembership(WebSecurity.CurrentUserId);

        }

        public static Player GetPlayer(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            return player;
        }

        public static string SaveNewPlayer(NewCharacterViewModel player)
        {

             IPlayerRepository playerRepo = new EFPlayerRepository();

             //check that the last name / first name combo is not already in use
             Player ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == player.FirstName && p.LastName == player.LastName);
             if (ghost != null)
             {
                 return "A character of this name already exists.";
             }

            // check that the name has not been reserved by someone else with a different Membership Id

            IReservedNameRepository resNameRepo = new EFReservedNameRepository();
            ReservedName resNameGhost = resNameRepo.ReservedNames.FirstOrDefault(r => r.FullName == player.FirstName + " " + player.LastName);

            if (resNameGhost != null && resNameGhost.MembershipId != WebSecurity.CurrentUserId)
            {
                return "This name has been reserved for a different player.";
            }

            // assert that the form is a valid staring form
             string x = player.FormName;

             if (x != "man_01" && x != "man_02" && x != "man_03" && x != "man_04" && x != "man_05" && x != "woman_01" && x != "woman_02" && x != "woman_03" && x != "woman_04" && x != "woman_05")
             {
                 return "That is not a valid starting form.";
             }

            // remove the old Player--Membership binding
             Player oldplayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == WebSecurity.CurrentUserId);

             int oldCovId = 0;

             if (oldplayer != null)
             {
                 // remove all of the old player's player logs
                 PlayerLogProcedures.ClearPlayerLog(oldplayer.Id);



                 // remove all of the old player's TF energies
                 TFEnergyProcedures.DeleteAllPlayerTFEnergies(oldplayer.Id);
                 oldplayer.MembershipId = -1;
                 playerRepo.SavePlayer(oldplayer);

                 // remove the old player's effects
                 EffectProcedures.DeleteAllPlayerEffects(oldplayer.Id);

                 oldCovId = oldplayer.Covenant;

                 // turn the item they player became permanent
                 IItemRepository itemRepo = new EFItemRepository();
                 Item oldItemMe = itemRepo.Items.FirstOrDefault(i => i.VictimName == oldplayer.FirstName + " " + oldplayer.LastName);
                 oldItemMe.IsPermanent = true;
                 itemRepo.SaveItem(oldItemMe);
                
             }
            

            // clean the name entered by the player
             string cleanFirstName = char.ToUpper(player.FirstName[0]) + player.FirstName.Substring(1).ToLower();
             string cleanLastName = char.ToUpper(player.LastName[0]) + player.LastName.Substring(1).ToLower();

             player.FirstName = cleanFirstName;
             player.LastName = cleanLastName;
           
            Player newplayer = new Player();
            newplayer.FirstName = player.FirstName;
            newplayer.LastName = player.LastName;
            newplayer.Gender = player.Gender;
            newplayer.Health = 100;
            newplayer.Mana = 100;
            newplayer.MaxHealth = 100;
            newplayer.MaxMana = 100;
            newplayer.ResistanceModifier = 0;
            newplayer.ActionPoints = PvPStatics.MaximumStoreableActionPoints;
            newplayer.dbLocationName = "coffee_shop";
            newplayer.MembershipId = WebSecurity.CurrentUserId;
            newplayer.Form = player.FormName;
            newplayer.OriginalForm = player.FormName;
            newplayer.Level = 1;
            newplayer.XP = 0;
            newplayer.LastActionTimestamp = DateTime.UtcNow;
            newplayer.LastCombatTimestamp = DateTime.UtcNow;
            newplayer.LastCombatAttackedTimestamp = DateTime.UtcNow;
            newplayer.OnlineActivityTimestamp = DateTime.UtcNow;
            newplayer.Money = 0;
            newplayer.ActionPoints_Refill = 360;
            newplayer.CleansesMeditatesThisRound = 0;
            newplayer.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
            newplayer.IsPetToId = -1;
            newplayer.Mobility = Statics.PvPStatics.MobilityFull;
            newplayer.ChatColor = "black";
          
            

            if (oldplayer != null)
            {
                newplayer.Covenant = oldplayer.Covenant;
                oldplayer.Covenant = 0;
                newplayer.Level = oldplayer.Level - 3;
                if (newplayer.Level < 1)
                {
                    newplayer.Level = 1;
                }
                newplayer.UnusedLevelUpPerks = newplayer.Level - 1;
                newplayer.ChatColor = oldplayer.ChatColor;


            }

            // start player in PvP if they choose.  Remember, PvP = protection now...
            newplayer.InPvP = !player.StartInPVP;

            if (player.StartInRP == true)
            {
                newplayer.InRP = true;
            }
            else
            {
                newplayer.InRP = false;
            }

            // set a random location for this character to spawn in
            List<string> spawnableLocations = LocationsStatics.GetLocation.Select(l => l.dbName).ToList();
            double max = spawnableLocations.Count();
            Random rand = new Random();
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num*max));
            string locationToSpawnIn = spawnableLocations.ElementAt(index);
            newplayer.dbLocationName = locationToSpawnIn;

            playerRepo.SavePlayer(newplayer);

            if (oldplayer != null)
            {
                // transfer all of the old player's skills that are NOT form specific or weaken
                SkillProcedures.TransferAllPlayerSkills(oldplayer.Id, newplayer.Id);
            }

            // assign the player their appropriate donation level
            DonatorProcedures.SetNewPlayerDonationRank(newplayer.FirstName + " " + newplayer.LastName);

            // if the player was in a covenant, they might have been the leader.  Check this and make a new player the leader
            if (oldCovId > 0)
            {
                ICovenantRepository covRepo = new EFCovenantRepository();
                Covenant oldCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == oldCovId);

                // we need to regrab the new player from the repo again to get their Id
                Player newmeFromDb = PlayerProcedures.GetPlayerWithExactName(newplayer.FirstName + " " + newplayer.LastName);

                if (oldCovenant != null && oldCovenant.LeaderId == oldplayer.Id)
                {
                    oldCovenant.LeaderId = newmeFromDb.Id;
                    covRepo.SaveCovenant(oldCovenant);
                }

            }

          

            ISkillRepository skillRepo = new EFSkillRepository();
            Skill baseskill = new Skill
            {
                OwnerId = newplayer.Id,
                Name = "lowerHealth",
                Charge = -1,
                Duration = -1
            };
            skillRepo.SaveSkill(baseskill);

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
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            string oldForm = dbPlayer.Form;
            dbPlayer.Form = dbPlayer.OriginalForm;
            DbStaticForm baseForm = FormStatics.GetForm(dbPlayer.OriginalForm);

            dbPlayer.Gender = baseForm.Gender;
            
            dbPlayer.NormalizeHealthMana();

            playerRepo.SavePlayer(dbPlayer);

            SkillProcedures.UpdateFormSpecificSkillsToPlayer(dbPlayer, oldForm, dbPlayer.Form);

        }

        public static void InstantChangeToForm(Player player, string formName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            DbStaticForm form = FormStatics.GetForm(formName);
            string oldForm = dbPlayer.Form;
            dbPlayer.Form = formName;

            
            dbPlayer.Gender = form.Gender;

            dbPlayer.NormalizeHealthMana();
            playerRepo.SavePlayer(dbPlayer);

            SkillProcedures.UpdateFormSpecificSkillsToPlayer(dbPlayer, oldForm, dbPlayer.Form);
        }

        public static void SavePlayer(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.dbLocationName = player.dbLocationName;

            playerRepo.SavePlayer(player);
        }

        public static void MarkOnlineActivityTimestamp(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.OnlineActivityTimestamp = DateTime.UtcNow;

            playerRepo.SavePlayer(player);
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

            // move any of this player's pets
            Player pet = playerRepo.Players.FirstOrDefault(p => p.IsPetToId == dbPlayer.Id);

            if (pet != null)
            {
                pet.dbLocationName = dbPlayer.dbLocationName;
                playerRepo.SavePlayer(pet);
            }

            return "";
        }

        public static void MovePlayer_InstantNoLog(int playerId, string newLocation)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.dbLocationName = newLocation;
            playerRepo.SavePlayer(player);
        }

        public static string MovePlayer(string destinationDbName, decimal actionPointDiscount)
        {
            return MovePlayer(GetPlayerFromMembership().Id, destinationDbName, actionPointDiscount);
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

            Location oldLocation = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == user.dbLocationName);
            Location newLocation = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == destination);

            user.dbLocationName = destination;
            playerRepo.SavePlayer(user);

            string playerLogMessage = "";
            string locationMessageOld = "";
            string locationMessageNew = "";

            if (showDestinationInLocationLog == true)
            {
                locationMessageOld = player.FirstName + " " + player.LastName + " used a Covenant Call Crystal, teleporting to their safeground at " + newLocation.Name + ".";
                locationMessageNew = player.FirstName + " " + player.LastName + " teleported home using a Covenant Call Crystal.";
                playerLogMessage = "You used a Covenant Call Crystal, teleporting you from " + oldLocation.Name + " to your safeground at " + newLocation.Name + ".";
            }
            else { 
                locationMessageOld = player.FirstName + " " + player.LastName + " used a scroll of teleportation.";
                locationMessageNew = player.FirstName + " " + player.LastName + " teleported to here.";
                playerLogMessage = "You teleported from " + oldLocation.Name + " to " + newLocation.Name + ".";
            }
            

            LocationLogProcedures.AddLocationLog(oldLocation.dbName, locationMessageOld);
            LocationLogProcedures.AddLocationLog(destination, locationMessageNew);
            PlayerLogProcedures.AddPlayerLog(player.Id, playerLogMessage, false);


            // teleport this player's pet if they have one
            Player pet = playerRepo.Players.FirstOrDefault(p => p.IsPetToId == player.Id);

            if (pet != null)
            {
                pet.dbLocationName = destination;
                playerRepo.SavePlayer(pet);
            }


            return playerLogMessage;
        }

        public static IEnumerable<Player> GetPlayersAtLocation(string destinationDbName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<Player> output = playerRepo.Players.Where(p => p.dbLocationName == destinationDbName).ToList();

            return output;
        }

        public static void ChangePlayerActionMana(decimal actionPoints, decimal health, decimal mana,  int playerId)
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
            if (player.Health > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
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

            return "You lower " + player.FirstName + "'s willpower by " + amount +".  ";

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
                    else if (roll < 1 && dbLocationName == "lab_lobby")
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
                }
            }

            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == dbLocationName);
            

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
                if (eligibleSkills.Count() == 0)
                {
                    IEnumerable<DbStaticSkill> skillsAtThisRegion = SkillStatics.GetSkillsLearnedAtRegion(here.Region);
                    eligibleSkills = from s in skillsAtThisRegion
                                     let sx = myKnownSkills.Select(r => r.dbName)
                                     where !sx.Contains(s.dbName)
                                     select s;
                }

                // there are no new spells to be learned that are not-region specific, so player is just out of luck.
                if (eligibleSkills.Count() == 0)
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
                if (ItemProcedures.PlayerIsCarryingTooMuch(player.Id, 1, myBuffs) == true)
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
                PlayerProcedures.GiveXP(player.Id, 3);
                return "Although you didn't find anything or learn any new spells, you note down a few things about your surroundings, which may come in useful in the future.  (+3 XP)";
            }

            // get effect or find nothing useful
            else if (roll <= 100)
            {
                // see if there is an effect that can be found in this area
                List<DbStaticEffect> effectsHere = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToList();

                if (effectsHere.Count() <= 0)
                {
                    PlayerProcedures.GiveXP(player.Id, 1.5M);
                    return "Although you didn't find anything or learn any new spells, you feel as though you know this town a little better, which may come in useful in the future.  (+1.5 XP)";
                }
                else
                {


                    // give the player a random effect found here
                    double max = effectsHere.Count();
                    int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                    DbStaticEffect effectToGet = effectsHere.ElementAt(randIndex);

                    // assert that the player doesn't already have this effect.  IF they do, break out
                    if (EffectProcedures.PlayerHasEffect(player, effectToGet.dbName) == true)
                    {
                        PlayerProcedures.GiveXP(player.Id, 1.5M);
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
                TotalPlayers = players.Where(p => p.MembershipId > 0).Count(),
                CurrentOnlinePlayers = players.Where(p => p.MembershipId > 0 && p.OnlineActivityTimestamp >= cutoff).Count(),
                //TotalAnimalPlayers = players.Where(p => p.Mobility == "animal").Count(),
                //TotalInanimatePlayers = players.Where(p => p.Mobility == "inanimate").Count(),
                //TotalLivingPlayers = players.Where(p => p.Mobility == "full").Count(),

            };



            return output;

        }

        public static string GiveXP(int playerId, decimal amount)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);

            // decrease XP gain by 40% for psychos
            if (player.MembershipId == -2)
            {
                amount = amount * .6M;
            }

            player.XP += amount;

            string output = "";

            if (amount > 0)
            {


                #region xp level checks positive
                if (player.Level == 1)
                {
                    if (player.XP > PvPStatics.XP__Level_2)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_2;
                        output += GiveLevelingBonus(player, 2);
                    }
                }
                else if (player.Level == 2)
                {
                    if (player.XP > PvPStatics.XP__Level_3)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_3;
                        output += GiveLevelingBonus(player, 3);
                    }
                }
                else if (player.Level == 3)
                {
                    if (player.XP > PvPStatics.XP__Level_4)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_4;
                        output += GiveLevelingBonus(player, 4);
                    }
                }
                else if (player.Level == 4)
                {
                    if (player.XP > PvPStatics.XP__Level_5)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_5;
                        output += GiveLevelingBonus(player, 5);
                    }
                }
                else if (player.Level == 5)
                {
                    if (player.XP > PvPStatics.XP__Level_6)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_6;
                        output += GiveLevelingBonus(player, 6);
                    }
                }
                else if (player.Level == 6)
                {
                    if (player.XP > PvPStatics.XP__Level_7)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_7;
                        output += GiveLevelingBonus(player, 7);
                    }
                }
                else if (player.Level == 7)
                {
                    if (player.XP > PvPStatics.XP__Level_8)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_8;
                        output += GiveLevelingBonus(player, 8);
                    }
                }
                else if (player.Level == 8)
                {
                    if (player.XP > PvPStatics.XP__Level_9)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_9;
                        output += GiveLevelingBonus(player, 9);
                    }
                }
                else if (player.Level == 9)
                {
                    if (player.XP > PvPStatics.XP__Level_10)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_10;
                        output += GiveLevelingBonus(player, 10);
                    }
                }
                else if (player.Level == 10)
                {
                    if (player.XP > PvPStatics.XP__Level_11)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_11;
                        output += GiveLevelingBonus(player, 11);
                    }
                }
                else if (player.Level == 11)
                {
                    if (player.XP > PvPStatics.XP__Level_12)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_12;
                        output += GiveLevelingBonus(player, 12);
                    }
                }
                else if (player.Level == 12)
                {
                    if (player.XP > PvPStatics.XP__Level_13)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_13;
                        output += GiveLevelingBonus(player, 13);
                    }
                }
                else if (player.Level == 13)
                {
                    if (player.XP > PvPStatics.XP__Level_14)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_14;
                        output += GiveLevelingBonus(player, 14);
                    }
                }
                else if (player.Level == 14)
                {
                    if (player.XP > PvPStatics.XP__Level_15)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_15;
                        output += GiveLevelingBonus(player, 15);
                    }
                }
                else if (player.Level == 15)
                {
                    if (player.XP > PvPStatics.XP__Level_16)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_16;
                        output += GiveLevelingBonus(player, 16);
                    }
                }
                else if (player.Level == 16)
                {
                    if (player.XP > PvPStatics.XP__Level_17)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_17;
                        output += GiveLevelingBonus(player, 17);
                    }
                }
                else if (player.Level == 17)
                {
                    if (player.XP > PvPStatics.XP__Level_18)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_18;
                        output += GiveLevelingBonus(player, 18);
                    }
                }
                else if (player.Level == 18)
                {
                    if (player.XP > PvPStatics.XP__Level_19)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_19;
                        output += GiveLevelingBonus(player, 19);
                    }
                }
                else if (player.Level == 19)
                {
                    if (player.XP > PvPStatics.XP__Level_20)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_20;
                        output += GiveLevelingBonus(player, 20);
                    }
                }
                else if (player.Level == 20)
                {
                    if (player.XP > PvPStatics.XP__Level_21)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_21;
                        output += GiveLevelingBonus(player, 21);
                    }
                }
                else if (player.Level == 21)
                {
                    if (player.XP > PvPStatics.XP__Level_22)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_22;
                        output += GiveLevelingBonus(player, 22);
                    }
                }
                else if (player.Level == 22)
                {
                    if (player.XP > PvPStatics.XP__Level_23)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_23;
                        output += GiveLevelingBonus(player, 23);
                    }
                }
                else if (player.Level == 23)
                {
                    if (player.XP > PvPStatics.XP__Level_24)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_24;
                        output += GiveLevelingBonus(player, 24);
                    }
                }
                else if (player.Level == 24)
                {
                    if (player.XP > PvPStatics.XP__Level_25)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_25;
                        output += GiveLevelingBonus(player, 25);
                    }
                }
                else if (player.Level == 25)
                {
                    if (player.XP > PvPStatics.XP__Level_26)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_26;
                        output += GiveLevelingBonus(player, 26);
                    }
                }
                else if (player.Level == 26)
                {
                    if (player.XP > PvPStatics.XP__Level_27)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_27;
                        output += GiveLevelingBonus(player, 27);
                    }
                }
                else if (player.Level == 27)
                {
                    if (player.XP > PvPStatics.XP__Level_28)
                    {
                        player.Level++;
                        player.XP -= PvPStatics.XP__Level_28;
                        output += GiveLevelingBonus(player, 28);
                    }
                }

            }
                #endregion

            #region xp level checks negative
            else
            {
                if (player.XP < 0 && player.Level > 1)
                {
                    player.Level--;
                    player.UnusedLevelUpPerks--;
                    if (player.Mobility != "full")
                    {
                        output = "  Unfortunately, as your last animate form becomes a more distant memory, you lose an experience level, which will set you back when or if you do regain an animate body.";
                    }
                    else
                    {
                        output = "  Unfortunately, due to your recent transformation into a strange new body you lose an experience level for the time being.";
                    }
                    
                    player.XP = PvPStatics.XP__LevelupRequirementByLevel[player.Level] - 1;
                }
            }
            #endregion


            playerRepo.SavePlayer(player);

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

        public static void SetTimestampToNow(Player player, bool PvPOnly)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbplayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            if (PvPOnly == true && dbplayer.InPvP == false)
            {
                dbplayer.LastActionTimestamp = DateTime.UtcNow;
                playerRepo.SavePlayer(dbplayer);
            }
            else if (PvPOnly == false)
            {
                dbplayer.LastActionTimestamp = DateTime.UtcNow;
                playerRepo.SavePlayer(dbplayer);
            }
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

        public static void LogIP(string ip)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.MembershipId == WebSecurity.CurrentUserId);
            dbPlayer.IpAddress = ip;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static bool IsMyIPInUseAndAnimate(string ip)
        {



            IPlayerRepository playerRepo = new EFPlayerRepository();
           decimal num = playerRepo.Players.Where(p => p.IpAddress == ip && p.Mobility == "full").Count();
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

            int extra = 0;

            // 3800 = Luxianne
            // 3490 = Mizuho
            // 224 = Lexam
            // 254 = Addie (Lexam's girlfriend)
            // 4481 = Lilith (Luxianne's alt)
           //  5931 = Mitsuho (Mizuho's PvP alt)
            if (player.MembershipId == 3800 || 
                player.MembershipId == 3490 || 
                player.MembershipId == 224 || 
                player.MembershipId == 254 || 
                player.MembershipId == 4481 || 
                player.MembershipId == 5931)
            {
                return false;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            decimal num = playerRepo.Players.Where(p => p.MembershipId > 0 && p.IpAddress == ip && p.Mobility == "full" && p.InPvP == player.InPvP).Count() + extra;
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
                try
                {
                    if (player.MembershipId <-1)
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
                catch
                {
                    return false;
                }
        }

        public static bool PlayerIsOffline(Player_VM player)
        {
            try
            {
                if (player.MembershipId < -1)
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
            catch
            {
                return false;
            }
        }

        public static bool PlayerIsVeryOffline(Player player)
        {


            try
            {

                if (player.MembershipId < -1)
                {
                    return false;
                }

                double minutesAgo = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

                if (minutesAgo > PvPStatics.OfflineAfterXMinutes*2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
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

        public static void FlagPlayerForSuspicousActivity(int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            player.FlaggedForAbuse = !player.FlaggedForAbuse;
            playerRepo.SavePlayer(player);
        }

        public static bool AccountIsTrusted(int membershipId)
        {
            // 69 = me
            // 224 = Lexam (REMOVED)
            // 272 = Wrenzephyr2
            if ((membershipId == 69) || (membershipId == 272))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static IEnumerable<Player> GetLeadingPlayers__XP(int number)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
          //  return playerRepo.Players.Where(p => p.InPvP==true).OrderByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);
            return playerRepo.Players.Where(p => p.MembershipId > 0).OrderByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);
        }

        public static IEnumerable<Player> GetLeadingPlayers__PvP(int number)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            //return playerRepo.Players.Where(p => p.MembershipId > 0 && p.InPvP == false).OrderByDescending(p => p.Level).ThenByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);

            return playerRepo.Players.Where(p => p.MembershipId > 0).OrderByDescending(p => p.PvPScore).ThenByDescending(p => p.Level).ThenByDescending(p => p.XP).Take(number);

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




            string logmessage = "<span class='playerCleansingNotification'>" + player.FirstName + " " + player.LastName + " cleansed here.</span>";
            LocationLogProcedures.AddLocationLog(player.dbLocationName, logmessage);


            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
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

            string logmessage = "<span class='playerMediatingNotification'>" + player.FirstName + " " + player.LastName + " meditated here.</span>";
            LocationLogProcedures.AddLocationLog(player.dbLocationName, logmessage);


            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            string playerLogMessage = "You meditated at " + here.Name + ".";
            PlayerLogProcedures.AddPlayerLog(player.Id, playerLogMessage, false);

            return result;

        }

        public static string SetRPFlag(Player player, bool turnOn)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            if (turnOn == true)
            {
                dbPlayer.InRP = true;
            }
            else
            {
                dbPlayer.InRP = false;
            }
            playerRepo.SavePlayer(dbPlayer);

            return "You are now in RP mode.";
        }

        public static string SetPvPFlag(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InPvP = true;
            dbPlayer.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
            playerRepo.SavePlayer(dbPlayer);
            return "You are now in PvP mode.";
        }

        public static string SetPvPFlag(Player player, bool trueOrFalse)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InPvP = trueOrFalse;
            dbPlayer.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
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

        public static Player ReadjustMaxes(Player player, BuffBox buffs)
        {
            // readjust player health/mana by grabbing base amount plus effects from buffs
            player.MaxHealth = PvPStatics.XP__HealthManaBaseByLevel[player.Level] * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
            player.MaxMana = PvPStatics.XP__HealthManaBaseByLevel[player.Level] * (1.0M + (buffs.ManaBonusPercent() / 100.0M));


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
            return playerRepo.Players.Where(p => p.Covenant == covenantId && p.Mobility == "full").Count();
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
            return "  You have gained " + amount + " PvP score from your victory over " + loser.FirstName + " " + loser.LastName + ".";
        }

        public static string RemovePlayerPvPScore(Player loser, Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == loser.Id);

            decimal loss = Math.Floor(dbPlayer.PvPScore / 4);

            dbPlayer.PvPScore -= loss;

            if (dbPlayer.PvPScore < 0)
            {
                dbPlayer.PvPScore = 0;
            }

            // loser is in PvP mode and attacker is not; double the loss penalty
            if (loser.InPvP == false && attacker.InPvP == true)
            {
                dbPlayer.PvPScore -= loss;
            }

            playerRepo.SavePlayer(dbPlayer);
            return "  You have lost " + loss + " PvP score from your defeat to " + attacker.FirstName + " " + attacker.LastName + ".";
            //return "";
        }

        public static decimal GetPvPScoreFromWin(Player attacker, Player victim)
        {
            decimal scoreFromLevel = 0;
            decimal scoreFromSteal = 0;
            int levelDiff = Math.Abs(attacker.Level - victim.Level);
            if (levelDiff <= 3)
            {
                scoreFromLevel = victim.Level * 10;

                if (attacker.PvPScore / 2 <= victim.PvPScore) { 
                    scoreFromSteal = Math.Floor(victim.PvPScore / 4);
                }
            }

            return scoreFromLevel + scoreFromSteal;
        }

        public static void SetNickname(string nickname)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == WebSecurity.CurrentUserId);
            player.Nickname = nickname;
            playerRepo.SavePlayer(player);
        }

        public static void LoadFormRAMBuffBox()
        {
            IDbStaticFormRepository dbStaticFormRepo = new EFDbStaticFormRepository();

            FormStatics.FormRAMBuffBoxes = new List<RAMBuffBox>();

            foreach (DbStaticForm f in dbStaticFormRepo.DbStaticForms.Where(c => c.dbName != null && c.dbName != ""))
            {
                RAMBuffBox temp = new RAMBuffBox
                {
                    dbName = f.dbName.ToLower(),

                    HealthBonusPercent = (float)f.HealthBonusPercent,
                    ManaBonusPercent = (float)f.ManaBonusPercent,
                    HealthRecoveryPerUpdate = (float)f.HealthRecoveryPerUpdate,
                    ManaRecoveryPerUpdate = (float)f.ManaRecoveryPerUpdate,
                };
                FormStatics.FormRAMBuffBoxes.Add(temp);
            }
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

    }
}