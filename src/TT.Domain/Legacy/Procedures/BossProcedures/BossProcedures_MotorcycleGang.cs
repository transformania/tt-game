using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures.BossProcedures
{
    public static class BossProcedures_MotorcycleGang
    {
        public const string BossFirstName = "Road Queen Harley";
        public const string BossLastName = "Punksplitter";
        public const int BossFormId = 934;

        public const int BikerFollowerSpellSourceId = 1198;
        public const int BikerFollowerFormSourceId = 935;

        public const int EffectLevelOneSourceId = 187;
        public const int EffectLevelTwoSourceId = 188;
        public const int EffectLevelThreeSourceId = 189;
        public const int EffectLevelFourSourceId = 190;
        public const int EffectLevelFiveSourceId = 191;

        public const int LeatherJacketSpellSourceId = 1197;

        private const int SummonToBossAPPenalty = 5;

        public const string SpawnLocation = LocationsStatics.STREET_130_SUNNYGLADE_DRIVE;

        private static readonly Random rand = new Random();

        public static void Spawn()
        {
            var boss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId
            {
                BotId = AIStatics.MotorcycleGangLeaderBotId
            });

            if (boss == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = BossFirstName,
                    LastName = BossLastName,
                    Location = SpawnLocation,
                    Gender = PvPStatics.GenderFemale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = BossFormId,
                    Money = 2000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 20,
                    BotId = AIStatics.MotorcycleGangLeaderBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var bossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                bossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(bossEF));
                playerRepo.SavePlayer(bossEF);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.MOTORCYCLE_RUNE, PlayerId = bossEF.Id });
                }

            }

        }

        private static int ChooseSpell(Player attacker)
        {
            return attacker.FormSourceId == BikerFollowerFormSourceId ? LeatherJacketSpellSourceId : BikerFollowerSpellSourceId;
        }

        public static void RunTurnLogic()
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var boss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MotorcycleGangLeaderBotId);

            // motorcycle gang boss is no longer animate; end the event
            if (boss.Mobility != PvPStatics.MobilityFull)
            {
                EndEvent();
                return;
            }

            // Step 1:  Find the most online characters (psychos and players) on the streets and move there.  Move anyone in the biker follower form to her.

            var targetLocation = GetLocationWithMostEligibleTargets();

            targetLocation = String.IsNullOrEmpty(targetLocation) ? boss.dbLocationName : targetLocation;

            var newlocation = AIProcedures.MoveTo(boss, targetLocation, 9);
            boss.dbLocationName = newlocation;
            boss.Mana = boss.MaxMana;
            playerRepo.SavePlayer(boss);

            var followers = GetFollowers();

            // move all followers to the new location
            foreach (var follower in followers)
            {
                Player followerEF = playerRepo.Players.First(p => p.Id == follower.Id);
                var followerNewLocation = AIProcedures.MoveTo(follower, newlocation, 100000);

                // leave location log and penalize some 5 AP
                if (followerEF.dbLocationName != followerNewLocation)
                {
                    var newlocationFriendlyName = LocationsStatics.LocationList.GetLocation
                        .First(l => l.dbName == followerNewLocation).Name;

                    LocationLogProcedures.AddLocationLog(followerNewLocation, $"{followerEF.GetFullName()} rode off to <b>{newlocationFriendlyName}</b> to help protect {boss.GetFullName()}.");
                    followerEF.ActionPoints = followerEF.ActionPoints >= SummonToBossAPPenalty ? followerEF.ActionPoints - SummonToBossAPPenalty : 0;
                }

                followerEF.dbLocationName = followerNewLocation;
                
                if (follower.BotId == AIStatics.ActivePlayerBotId)
                {
                    PlayerLogProcedures.AddPlayerLog(follower.Id, $"<b>The leader of your gang, {boss.GetFullName()}, beckons for you to follow!  You have no choice but to comply.</b>", true);
                }
                else if (follower.BotId == AIStatics.PsychopathBotId)
                {
                    followerEF.Health += 25;
                    followerEF.Health = followerEF.Health > followerEF.MaxHealth ? followerEF.MaxHealth : followerEF.Health;
                    followerEF.LastActionTimestamp = DateTime.UtcNow;
                }
                playerRepo.SavePlayer(followerEF);
            }

            // Step 2:  Give the boss the appropriate effect, determined by how many followers she has.
            EffectProcedures.GivePerkToPlayer(GetPerkSourceIdToGive(followers.Count), boss);

            // Step 3:  Attack everyone in the region 2-3 times with the biker follower spell if they are not already that form.
            var victims = GetEligibleTargetsAtLocation(newlocation);

            // try and turn everyone there into biker gang followers
            foreach (var victim in victims)
            {
                AttackProcedures.Attack(boss, victim, BikerFollowerSpellSourceId);
                AttackProcedures.Attack(boss, victim, BikerFollowerSpellSourceId);
                AttackProcedures.Attack(boss, victim, BikerFollowerSpellSourceId);
            }

        }

        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = GetOnlineCutoffTime();
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                                                                     p.LastActionTimestamp > cutoff &&
                                                                     p.FormSourceId != BikerFollowerFormSourceId &&
                                                                     (p.BotId == AIStatics.ActivePlayerBotId ||
                                                                      p.BotId == AIStatics.PsychopathBotId) &&
                                                                     p.BotId != AIStatics.MotorcycleGangLeaderBotId &&
                                                                     p.dbLocationName.Contains("street_") &&
                                                                     p.InDuel <= 0 &&
                                                                     p.InQuest <= 0).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            return locs.FirstOrDefault();
        }

        private static List<Player> GetEligibleTargetsAtLocation(string location)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = GetOnlineCutoffTime();
            return playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                                                 p.LastActionTimestamp > cutoff &&
                                                 p.FormSourceId != BikerFollowerFormSourceId &&
                                                 (p.BotId == AIStatics.ActivePlayerBotId ||
                                                 p.BotId == AIStatics.PsychopathBotId) &&
                                                 p.BotId != AIStatics.MotorcycleGangLeaderBotId &&
                                                 p.dbLocationName == location &&
                                                 p.InDuel <= 0 &&
                                                 p.InQuest <= 0).ToList();

        }

        private static List<Player> GetFollowers()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = GetOnlineCutoffTime();
            return playerRepo.Players.Where(p => p.FormSourceId == BikerFollowerFormSourceId &&
                                                 p.Mobility == PvPStatics.MobilityFull &&
                                                 (p.BotId == AIStatics.ActivePlayerBotId || p.BotId == AIStatics.PsychopathBotId) &&
                                                 p.LastActionTimestamp > cutoff &&
                                                 p.InDuel <= 0 &&
                                                 p.InQuest <= 0).ToList();
        }

        private static DateTime GetOnlineCutoffTime()
        {
            return DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
        }

        private static int GetPerkSourceIdToGive(int followerCount)
        {
            if (followerCount >= 9)
            {
                return EffectLevelFiveSourceId;
            }
            else if (followerCount >= 6)
            {
                return EffectLevelFourSourceId;
            }
            else if (followerCount >= 4)
            {
                return EffectLevelThreeSourceId;
            }
            else if (followerCount >= 2)
            {
                return EffectLevelTwoSourceId;
            }
            else 
            {
                return EffectLevelOneSourceId;
            }
        }

        public static void CounterAttack(Player victim, Player boss)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            // boss counterattacks once
            AttackProcedures.Attack(boss, victim, ChooseSpell(victim));

            // have some of the followers also attack the agressor
            var allFollowers = GetFollowers();
            var followersHere = allFollowers.Where(f => f.dbLocationName == boss.dbLocationName &&
                                                          f.Id != victim.Id &&
                                                          f.TimesAttackingThisUpdate < 3
                                                          ).ToList();


            for (var i = 0; i < allFollowers.Count / 2; i++)
            {
                var follower = GetRandomFollower(followersHere);
                if (follower != null)
                {
                    AttackProcedures.Attack(follower, victim, ChooseSpell(victim));
                    PlayerLogProcedures.AddPlayerLog(follower.Id, $"<b>{BossFirstName} {BossLastName} orders you to attack {victim.GetFullName()}!</b>", true);

                    // reset the last attack and online timestamp to before the attack, otherwise she bumps her followers online indefinitely...
                    var player = playerRepo.Players.First(p => p.Id == follower.Id);
                    player.LastActionTimestamp = follower.LastActionTimestamp;
                    player.LastCombatTimestamp = follower.LastCombatTimestamp;
                    playerRepo.SavePlayer(player);

                    // remove this person from the list of eligible attackers so they don't do it more than once
                    followersHere = followersHere.Where(f => f.Id != follower.Id).ToList();

                }
            }

        }

        private static Player GetRandomFollower(List<Player> followers)
        {
            if (followers.IsEmpty())
            {
                return null;
            }
            return followers.ElementAt((int)Math.Floor(followers.Count * rand.NextDouble()));
        }

        public static bool SpellIsValid(string mobilityType)
        {
            return mobilityType == PvPStatics.MobilityPet || mobilityType == PvPStatics.MobilityInanimate;
        }

        /// <summary>
        /// End the boss event and distribute XP to players who fought
        /// </summary>
        public static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndMotorcycleGang();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            var damages = AIProcedures.GetTopAttackers(AIStatics.MotorcycleGangLeaderBotId, 25);

            // top player gets 1000 XP, each player down the line receives 35 fewer
            var l = 0;
            var maxReward = 1000;

            for (var i = 0; i < damages.Count; i++)
            {
                var damage = damages.ElementAt(i);

                var victor = playerRepo.Players.FirstOrDefault(p => p.Id ==damage.PlayerId);
                if (victor == null)
                {
                    continue;
                }
                var reward = maxReward - (l * 35);
                victor.XP += reward;
                l++;

                playerRepo.SavePlayer(victor);
                PlayerLogProcedures.AddPlayerLog(victor.Id, $"<b>For your contribution in defeating {BossFirstName} {BossLastName}, you earn {reward} XP from your spells cast against her</b>", true);

                // top three get runes
                if (i <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.MOTORCYCLE_RUNE, PlayerId = victor.Id }); // TODO:  Make new rune for this boss
                }

            }

        }
    }
}
