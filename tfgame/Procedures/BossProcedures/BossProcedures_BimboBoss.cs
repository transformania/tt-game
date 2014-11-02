using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_BimboBoss
    {

        public const string BossFirstName = "Lady";
        public const string BossLastName = "Lovebringer, PHD";
        public const string KissEffectdbName = "curse_bimboboss_kiss";
        public const string KissSkilldbName = "skill_bimboboss_kiss";
        public const string CureEffectdbName = "blessing_bimboboss_cure";
        public const string RegularTFSpellDbName = "skill_Dawn_of_the_Ditz_Varn";
        public const string RegularBimboFormDbName = "form_Bimbonic_Plague-Bearer_Varn";

        public static void SpawnBimboBoss()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);

            if (bimboBoss == null)
            {
                bimboBoss = new Player()
                {
                    FirstName = BossFirstName,
                    LastName = BossLastName,
                    ActionPoints = 120,
                    dbLocationName = "street_140_sunnyglade_drive",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_Bimbonic_Plague_Mother_Judoo",
                    IsPetToId = -1,
                    Money = 0,
                    Mobility = "full",
                    Level = 8,
                    MembershipId = -7,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(bimboBoss);

                bimboBoss = PlayerProcedures.ReadjustMaxes(bimboBoss, ItemProcedures.GetPlayerBuffs(bimboBoss));

                playerRepo.SavePlayer(bimboBoss);

                // give bimbo the plague spell to attack with
                //bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);
                //DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("skill_Dawn_of_the_Ditz_Varn");

                //// give bimbo the kiss spell
                //DbStaticSkill skilltoAdd2 = SkillStatics.GetStaticSkill("skill_bimboboss_kiss");

                //SkillProcedures.GiveSkillToPlayer(bimboBoss.Id, skillToAdd);
                //SkillProcedures.GiveSkillToPlayer(bimboBoss.Id, skilltoAdd2);

                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = bimboBoss.Id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);

            }
        }

        public static void CounterAttack(Player human, Player bimboss)
        {
            // if the bimboboss is inanimate, end this boss event
            if (bimboss.Mobility != "full")
            {
                return;
            }

            // if the player doesn't currently have it, give them the infection kiss
            if (EffectProcedures.PlayerHasEffect(human, KissEffectdbName) == false && EffectProcedures.PlayerHasEffect(human, CureEffectdbName) == false)
            {
                AttackProcedures.Attack(bimboss, human, KissSkilldbName);
            }

            // otherwise run the regular trasformation
            else if (human.Form != RegularBimboFormDbName)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                int attackCount = (int)Math.Floor(rand.NextDouble()*4);
                for (int i = 0; i < attackCount; i++) {
                    AttackProcedures.Attack(bimboss, human, RegularTFSpellDbName);
                }
            }

            // otherwise make the human wander away to find more targets
            else
            {
                string targetLocation = GetLocationWithMostEligibleTargets();
                string newlocation = AIProcedures.MoveTo(human, targetLocation, 8);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player dbHuman = playerRepo.Players.FirstOrDefault(p => p.Id == human.Id);
                dbHuman.TimesAttackingThisUpdate = 3;
                dbHuman.Health = 0;
                dbHuman.Mana = 0;
                dbHuman.XP -= 25;
                dbHuman.dbLocationName = newlocation;
                dbHuman.ActionPoints -= 10;

                if (dbHuman.XP < 0)
                {
                    dbHuman.XP = 0;
                }

                if (dbHuman.ActionPoints < 0)
                {
                    dbHuman.ActionPoints = 0;
                }

                playerRepo.SavePlayer(dbHuman);
                string message = "Lady Lovebringer is not pleased with you attacking her after she has so graciously given you that sexy body and carefree mind.  She injects you with a serum that makes you just a bit foggier and obediant, then orders you away to find new targets to spread the virus to.  You have no choice but to obey...";
                PlayerLogProcedures.AddPlayerLog(human.Id, message, true);
            }

        }

        public static void RunActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);

            if (bimboBoss.Mobility == "full") {

            }
            
        }

        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);
            return playerRepo.Players.Where(p => p.Mobility == "full" && p.LastActionTimestamp > cutoff && p.Form != RegularBimboFormDbName).OrderByDescending(p => p.dbLocationName).First().dbLocationName;
        }

    }
}