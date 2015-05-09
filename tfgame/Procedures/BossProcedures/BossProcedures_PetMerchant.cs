using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_PetMerchant
    {

        // pet merchant, MembershipId = -10

        public static void SpawnPetMerchant()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player petMerchant = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -10);

            if (petMerchant == null)
            {
                petMerchant = new Player();
                petMerchant.MembershipId = -10;
                petMerchant.Level = 5;
                petMerchant.FirstName = "Wüffie";
                petMerchant.LastName = "the Soul Pet Vendor";
                petMerchant.Health = 5000;
                petMerchant.Mana = 5000;
                petMerchant.MaxHealth = 500;
                petMerchant.MaxMana = 500;
                petMerchant.Mobility = "full";
                petMerchant.Money = 1000;
                petMerchant.TimesAttackingThisUpdate = 0;
                petMerchant.UnusedLevelUpPerks = 0;
                petMerchant.LastActionTimestamp = DateTime.UtcNow;
                petMerchant.LastCombatTimestamp = DateTime.UtcNow;
                petMerchant.LastCombatAttackedTimestamp = DateTime.UtcNow;
                petMerchant.OnlineActivityTimestamp = DateTime.UtcNow;
                petMerchant.Form = "form_Soul_Pet_Vendor_Judoo";
                petMerchant.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
                petMerchant.dbLocationName = "270_west_9th_ave"; // Lindella starts her rounds here
                petMerchant.Gender = "female";
                petMerchant.IsItemId = -1;
                petMerchant.IsPetToId = -1;
                petMerchant.ActionPoints = 120;

                playerRepo.SavePlayer(petMerchant);

                petMerchant = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -10);

                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();

                AIDirective directive = new AIDirective
                {
                    OwnerId = petMerchant.Id,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    State = "move",
                    Timestamp = DateTime.UtcNow,
                    TargetLocation = "",
                    TargetPlayerId = -1,
                    DoNotRecycleMe = true,
                };

            }
        }

        public static void CounterAttack(Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player petMerchant = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -10);

            if (petMerchant.Mobility == "full" && attacker.Mobility == "full") {
                AttackProcedures.Attack(petMerchant, attacker, "skill_Sarmoti_Zatur");
            }
            
        }

        public static void RunPetMerchantActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player petMerchant = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -10);
            

            if (petMerchant.Mobility == "full")
            {
                if (petMerchant.Health < petMerchant.MaxHealth || petMerchant.Mana < petMerchant.MaxMana)
                {
                    BuffBox buffs = ItemProcedures.GetPlayerBuffs(petMerchant);
                    if (petMerchant.Health < petMerchant.MaxHealth) {
                        petMerchant.Health += 200;
                        string logmessage = "<span class='playerCleansingNotification'>" + petMerchant.GetFullName() + " cleansed here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }
                    if ( petMerchant.Mana < petMerchant.MaxMana) {
                        petMerchant.Mana += 200;
                        string logmessage = "<span class='playerMediatingNotification'>" + petMerchant.GetFullName() + " meditated here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }

                    petMerchant.NormalizeHealthMana();
                    
                }

                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
               
                int turnMod = turnNumber % (24*4);

                string newLocation = "";
                if (turnMod < 24 * 1)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("ranch_outside");
                }
                else if (turnMod > 24 * 1 && turnMod <= 24 * 2)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("forest");
                }
                else if (turnMod > 24 * 2 && turnMod <= 24 * 3)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("campground");
                }
                else
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("campground");
                }

                string actualNewLocation = AIProcedures.MoveTo(petMerchant, newLocation, 5);
                petMerchant.dbLocationName = actualNewLocation;
                playerRepo.SavePlayer(petMerchant);

                if (turnNumber % 11 == 5)
                {
                    using (var context = new StatsContext())
                    {
                        try
                        {
                            context.Database.ExecuteSqlCommand("UPDATE [Stats].[dbo].[Items] SET OwnerId = " + petMerchant.Id + ", PvPEnabled = -1, dbLocationName = '', TimeDropped = '" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "'  WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId = -1 AND dbName LIKE 'animal_%'");

                            context.Database.ExecuteSqlCommand("UPDATE [Stats].[dbo].[Players] SET dbLocationName = '" + petMerchant.dbLocationName + ", IsPetToId = " + petMerchant.Id + " WHERE (FirstName + ' ' + LastName) IN ( SELECT VictimName FROM [Stats].[dbo].[Items] WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId = -1 AND dbName LIKE 'animal_%' )");

                           

                        }
                        catch
                        {
                           
                        }
                    }
                }
            }
        }
    }
}