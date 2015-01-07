using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

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
                petMerchant.MembershipId = -3;
                petMerchant.Level = 5;
                petMerchant.FirstName = "Wüf";
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

        public static void Move(int turnNumber)
        {

        }
    }
}