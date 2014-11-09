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
    public static class BossProcedures_BimboBoss
    {

        private const string BossFirstName = "Lady";
        private const string BossLastName = "Lovebringer, PHD";
        public const string KissEffectdbName = "curse_bimboboss_kiss";
        public const string KissSkilldbName = "skill_bimboboss_kiss";
        public const string CureEffectdbName = "blessing_bimboboss_cure";
        private const string RegularTFSpellDbName = "skill_Bringer_of_the_Bimbocalypse_Judoo";
        private const string RegularBimboFormDbName = "form_Bimbocalypse_Plague_Victim_Judoo";
        public const string CureItemDbName = "item_consumeable_bimbo_cure";

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
                    dbLocationName = "stripclub_bar_seats",
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
                    Money = 2500,
                    Mobility = "full",
                    Level = 15,
                    MembershipId = -7,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(bimboBoss);

                bimboBoss = PlayerProcedures.ReadjustMaxes(bimboBoss, ItemProcedures.GetPlayerBuffs(bimboBoss));

                playerRepo.SavePlayer(bimboBoss);

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
                EndThisBossEvent();
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
                int attackCount = (int)Math.Floor(rand.NextDouble() * 2 + 1);
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
                string message = "Lady Lovebringer is not pleased with you attacking her after she has so graciously given you that sexy body and carefree mind.  She whispers something into your ear that causes your body to grow limp in her arms, then injects you with a serum that makes your mind just a bit foggier and loyal to your bimbonic mother.  She orders you away to find new targets to spread the virus to.  The combination of lust and her command leaves you with no choice but to mindlessly obey...";
                PlayerLogProcedures.AddPlayerLog(human.Id, message, true);
            }

        }

        public static void RunActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);

            // move her toward the location with the most eligible targets
            if (bimboBoss.Mobility != "full") {
                EndThisBossEvent();
                return;
            }

            string targetLocation = GetLocationWithMostEligibleTargets();
            string newlocation = AIProcedures.MoveTo(bimboBoss, targetLocation, 12);

            bimboBoss.dbLocationName = newlocation;

            playerRepo.SavePlayer(bimboBoss);
            bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);

            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // attack all eligible targets here, even if it's not her final destination
            List<Player> playersHere = GetEligibleTargetsInLocation(newlocation, bimboBoss);

            foreach (Player p in playersHere)
            {
                // if the player doesn't currently have it, give them the infection kiss
                if (EffectProcedures.PlayerHasEffect(p, KissEffectdbName) == false && EffectProcedures.PlayerHasEffect(p, CureEffectdbName) == false)
                {
                    AttackProcedures.Attack(bimboBoss, p, KissSkilldbName);
                }

                // otherwise run the regular trasformation
                else if (p.Form != RegularBimboFormDbName)
                {
                   // int attackCount = (int)Math.Floor(rand.NextDouble() * 3 + 1);
                   // for (int i = 0; i < attackCount; i++)
                   // {
                        AttackProcedures.Attack(bimboBoss, p, RegularTFSpellDbName);
                   // }
                }
            }

            // have a random chance that infected players spontaneously transform
            IEffectRepository effectRepo = new EFEffectRepository();
            List<int> ownerIds = effectRepo.Effects.Where(e => e.dbName == KissEffectdbName).Select(e => e.OwnerId).ToList();

            foreach (int effectId in ownerIds)
            {

                Player infectee = playerRepo.Players.FirstOrDefault(p => p.Id == effectId);

                // if the infectee is no longer animate or is another boss, skip them
                if (infectee.Mobility != "full" || infectee.MembershipId < -2)
                {
                    continue;
                }

                double roll = rand.NextDouble();

                // random chance of spontaneously transforming
                if (infectee.Form != RegularBimboFormDbName && PlayerProcedures.PlayerIsOffline(infectee) == false)
                {
                    if (roll < .16)
                    {
                        infectee.Form = RegularBimboFormDbName;
                        infectee.Gender = "female";
                        infectee = PlayerProcedures.ReadjustMaxes(infectee,ItemProcedures.GetPlayerBuffsRAM(infectee));
                        playerRepo.SavePlayer(infectee);

                        string message = "You gasp, your body shiftig as the virus infecting you overwhelms your biological and arcane defenses.  Before long you find that your body has been transformed into that of one of the many bimbonic plague victims and you can't help but succumb to the urges to spread your infection--no, your gift!--on to the rest of mankind.";
                        string loclogMessage = "<b style='color: red'>" + infectee.GetFullName() + " succumbed to the bimbonic virus, spontaneously transforming into one of Lady Lovebringer's bimbos.</b>";

                        PlayerLogProcedures.AddPlayerLog(infectee.Id, message, true);
                        LocationLogProcedures.AddLocationLog(infectee.dbLocationName, loclogMessage);
                    }
                }

                // spread the kiss so long as the player is not offline
                if (infectee.Form == RegularBimboFormDbName && PlayerProcedures.PlayerIsOffline(infectee) == false)
                {
                    // back up the last action timestamp since we don't want these attacks to count against their offline timer
                    DateTime lastActionBackup = infectee.LastActionTimestamp;

                    List<Player> eligibleTargets = GetEligibleTargetsInLocation(infectee.dbLocationName, infectee);
                    int attacksMadeCount = 0;

                    foreach (Player p in eligibleTargets)
                    {
                        if (EffectProcedures.PlayerHasEffect(p, KissEffectdbName) == false && EffectProcedures.PlayerHasEffect(p, CureEffectdbName) == false && attacksMadeCount < 3)
                        {
                            attacksMadeCount++;
                            AttackProcedures.Attack(infectee, p, KissSkilldbName);
                        }
                        else if (attacksMadeCount < 3)
                        {
                            AttackProcedures.Attack(infectee, p, RegularTFSpellDbName);
                            attacksMadeCount++;
                        }
                    }

                    // if there were any attacked players, restore the old last action timestamp and make sure AP and mana has not gone into negative
                    if (attacksMadeCount > 0)
                    {
                        infectee = playerRepo.Players.FirstOrDefault(p => p.Id == effectId);
                        infectee.LastActionTimestamp = lastActionBackup;

                        if (infectee.ActionPoints < 0)
                        {
                            infectee.ActionPoints = 0;
                        }

                        if (infectee.Mana < 0)
                        {
                            infectee.Mana = 0;
                        }

                        playerRepo.SavePlayer(infectee);
                    }
                }
            }


            // every 5 turns heal the boss based on how many infected bots there are
            if (turnNumber % 5 == 0)
            {
                
                int activeCurses = effectRepo.Effects.Where(eff => eff.dbName == KissEffectdbName).Count()*3;
                if (activeCurses > 75)
                {
                    activeCurses = 100;
                }
                bimboBoss.Health += activeCurses;
                playerRepo.SavePlayer(bimboBoss);
                string message = "<b>" + bimboBoss.GetFullName() + " draws energy from her bimbo horde, regenerating her own willpower by " + activeCurses + ".</b>";
                LocationLogProcedures.AddLocationLog(newlocation, message);
            }

            // drop a cure
            DropCure(turnNumber);


        }

        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == "full" && p.LastActionTimestamp > cutoff && p.Form != RegularBimboFormDbName).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            return locs.First();
        }

        public static void EndThisBossEvent()
        {
            PvPWorldStatProcedures.Boss_EndBimbo();

            // delete all bimbo effects, both the kiss and the cure
            IEffectRepository effectRepo = new EFEffectRepository();
            List<Effect> effectsToDelete = effectRepo.Effects.Where(e => e.dbName == KissEffectdbName || e.dbName == CureEffectdbName).ToList();

            foreach (Effect e in effectsToDelete)
            {
                effectRepo.DeleteEffect(e.Id);
            }

            // delete all the cure vials
            IItemRepository itemRepo = new EFItemRepository();
            List<Item> cureVials = itemRepo.Items.Where(i => i.dbName == CureItemDbName).ToList();

            foreach (Item vial in cureVials)
            {
                itemRepo.DeleteItem(vial.Id);
            }

            // restore any bimbos back to their base form and notify them

            string message = "Your body suddenly returns to normal as the bimbonic virus in your body suddenly goes into submission, the psychic link between you and your plague mother separated for good.  Due to the bravery of your fellow mages the Bimbocalypse has been thwarted... for now.";

            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> infected = playerRepo.Players.Where(p => p.Form == RegularBimboFormDbName).ToList();
            
            foreach (Player p in infected)
            {
                PlayerProcedures.InstantRestoreToBase(p);
                if (p.MembershipId > 0)
                {
                    PlayerLogProcedures.AddPlayerLog(p.Id, message, true);
                }
            }
        }

        private static List<Player> GetEligibleTargetsInLocation(string location, Player attacker)
        {
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(location).Where(m => m.Mobility == "full" && m.Id != attacker.Id && m.Form != RegularBimboFormDbName && m.MembershipId >= -2 && m.LastActionTimestamp > cutoff && m.MembershipId != -7).ToList();

            return playersHere;
        }

        private static void DropCure(int turnNumber)
        {

            IItemRepository itemRepo = new EFItemRepository();

            Item newVial = new Item
            {
                dbLocationName = LocationsStatics.GetRandomLocation(),
                dbName = CureItemDbName,
                IsEquipped = false,
                IsPermanent = false,
                Level = 0,
                PvPEnabled = true,
                OwnerId = -1,
                VictimName = "",
                TimeDropped = DateTime.UtcNow,
                TurnsUntilUse = 0,
                EquippedThisTurn = false,
            };

            if (turnNumber % 3 == 0)
            {
                newVial.PvPEnabled = false;
            }

            itemRepo.SaveItem(newVial);

        }

    }
}