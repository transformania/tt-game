﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_BimboBoss
    {

        private const string BossFirstName = "Lady";
        private const string BossLastName = "Lovebringer, PHD";
        public const string BossFormDbName = "form_Bimbonic_Plague_Mother_Judoo";
        public const string KissEffectdbName = "curse_bimboboss_kiss";
        public const string KissSkilldbName = "skill_bimboboss_kiss";
        public const string CureEffectdbName = "blessing_bimboboss_cure";
        public const string RegularTFSpellDbName = "skill_Bringer_of_the_Bimbocalypse_Judoo";
        private const string RegularBimboFormDbName = "form_Bimbocalypse_Plague_Victim_Judoo";
        public const string CureItemDbName = "item_consumeable_bimbo_cure";
        public const int CureItemSourceId = 143;
        private const int BimboBossFormId = 233;

        public static void SpawnBimboBoss()
        {
            PlayerDetail bimboBoss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.BimboBossBotId });

            if (bimboBoss == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = BossFirstName,
                    LastName = BossLastName,
                    Location = "stripclub_bar_seats",
                    Gender = PvPStatics.GenderFemale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = BossFormDbName,
                    FormSourceId = BimboBossFormId,
                    Money = 2500,
                    Level = 15,
                    BotId = AIStatics.BimboBossBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                Player bimboEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);

                bimboEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(bimboEF));

                playerRepo.SavePlayer(bimboEF);


                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);

            }
        }

        public static void CounterAttack(Player human, Player bimboss)
        {

            // record that human attacked the boss
            AIProcedures.DealBossDamage(bimboss, human, true, 1);

            // if the bimboboss is inanimate, end this boss event
            if (bimboss.Mobility != PvPStatics.MobilityFull)
            {
                return;
            }

            // if the player doesn't currently have it, give them the infection kiss
            if (!EffectProcedures.PlayerHasEffect(human, KissEffectdbName) && !EffectProcedures.PlayerHasEffect(human, CureEffectdbName))
            {
                AttackProcedures.Attack(bimboss, human, KissSkilldbName);
                AIProcedures.DealBossDamage(bimboss, human, false, 1);
            }

            // otherwise run the regular trasformation
            else if (human.Form != RegularBimboFormDbName)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                int attackCount = (int)Math.Floor(rand.NextDouble() * 2 + 1);
                for (int i = 0; i < attackCount; i++) {
                    AttackProcedures.Attack(bimboss, human, RegularTFSpellDbName);
                }
                AIProcedures.DealBossDamage(bimboss, human, false, attackCount);
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
            if (bimboBoss.Mobility != PvPStatics.MobilityFull) {
                EndEvent();
                return;
            }

            string targetLocation = GetLocationWithMostEligibleTargets();
            string newlocation = AIProcedures.MoveTo(bimboBoss, targetLocation, 12);

            bimboBoss.dbLocationName = newlocation;

            playerRepo.SavePlayer(bimboBoss);
            bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BossFirstName && f.LastName == BossLastName);

            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // attack all eligible targets here, even if it's not her final destination
            List<Player> playersHere = GetEligibleTargetsInLocation(newlocation, bimboBoss);

            foreach (Player p in playersHere)
            {
                // if the player doesn't currently have it, give them the infection kiss
                if (!EffectProcedures.PlayerHasEffect(p, KissEffectdbName) && !EffectProcedures.PlayerHasEffect(p, CureEffectdbName))
                {
                    AttackProcedures.Attack(bimboBoss, p, KissSkilldbName);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 1);
                }


                // otherwise run the regular trasformation
                else if (p.Form != RegularBimboFormDbName)
                {
                    AttackProcedures.Attack(bimboBoss, p, RegularTFSpellDbName);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 1);
                }
            }

            // have a random chance that infected players spontaneously transform
            IEffectRepository effectRepo = new EFEffectRepository();
            List<int> ownerIds = effectRepo.Effects.Where(e => e.dbName == KissEffectdbName).Select(e => e.OwnerId).ToList();

            foreach (int effectId in ownerIds)
            {

                Player infectee = playerRepo.Players.FirstOrDefault(p => p.Id == effectId);

                // if the infectee is no longer animate or is another boss, skip them
                if (infectee.Mobility != PvPStatics.MobilityFull || infectee.BotId < AIStatics.PsychopathBotId)
                {
                    continue;
                }

                double roll = rand.NextDouble();

                // random chance of spontaneously transforming
                if (infectee.Form != RegularBimboFormDbName && !PlayerProcedures.PlayerIsOffline(infectee))
                {
                    if (roll < .16 && infectee.InDuel <= 0 && infectee.InQuest <= 0)
                    {

                        DomainRegistry.Repository.Execute(new ChangeForm
                        {
                            PlayerId = infectee.Id,
                            FormName = RegularBimboFormDbName
                        });

                        DomainRegistry.Repository.Execute(new ReadjustMaxes
                        {
                            playerId = infectee.Id,
                            buffs = ItemProcedures.GetPlayerBuffs(infectee)
                        });

                        infectee = playerRepo.Players.FirstOrDefault(p => p.Id == effectId);

                        string message = "You gasp, your body shifting as the virus infecting you overwhelms your biological and arcane defenses.  Before long you find that your body has been transformed into that of one of the many bimbonic plague victims and you can't help but succumb to the urges to spread your infection--no, your gift!--on to the rest of mankind.";
                        string loclogMessage = "<b style='color: red'>" + infectee.GetFullName() + " succumbed to the bimbonic virus, spontaneously transforming into one of Lady Lovebringer's bimbos.</b>";

                        PlayerLogProcedures.AddPlayerLog(infectee.Id, message, true);
                        LocationLogProcedures.AddLocationLog(infectee.dbLocationName, loclogMessage);
                    }
                }

                // spread the kiss so long as the player is not offline
                if (infectee.Form == RegularBimboFormDbName && !PlayerProcedures.PlayerIsOffline(infectee))
                {
                    // back up the last action timestamp since we don't want these attacks to count against their offline timer
                    DateTime lastActionBackup = infectee.LastActionTimestamp;

                    List<Player> eligibleTargets = GetEligibleTargetsInLocation(infectee.dbLocationName, infectee);
                    int attacksMadeCount = 0;

                    foreach (Player p in eligibleTargets)
                    {
                        if (!EffectProcedures.PlayerHasEffect(p, KissEffectdbName) && !EffectProcedures.PlayerHasEffect(p, CureEffectdbName) && attacksMadeCount < 3)
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
                
                var activeCurses = effectRepo.Effects.Count(eff => eff.dbName == KissEffectdbName)*3;
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
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull && 
            p.LastActionTimestamp > cutoff && 
            p.Form != RegularBimboFormDbName && 
            !p.dbLocationName.Contains("dungeon_") &&
            p.InDuel <= 0 &&
            p.InQuest <= 0).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            return locs.First();
        }

        public static void EndEvent()
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
            var cmd = new GetAllItemsOfType { ItemSourceId = CureItemSourceId};
            var cures = DomainRegistry.Repository.Find(cmd);

            foreach (ItemListingDetail cure in cures)
            {
                var deleteCmd = new DeleteItem {ItemId = cure.Id};
                DomainRegistry.Repository.Execute(deleteCmd);
            }

            // restore any bimbos back to their base form and notify them

            string message = "Your body suddenly returns to normal as the bimbonic virus in your body suddenly goes into submission, the psychic link between you and your plague mother separated for good.  Due to the bravery of your fellow mages the Bimbocalypse has been thwarted... for now.";

            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> infected = playerRepo.Players.Where(p => p.Form == RegularBimboFormDbName).ToList();
            
            foreach (Player p in infected)
            {
                PlayerProcedures.InstantRestoreToBase(p);
                if (p.BotId == AIStatics.ActivePlayerBotId)
                {
                    PlayerLogProcedures.AddPlayerLog(p.Id, message, true);
                }
            }

            List<BossDamage> damages = AIProcedures.GetTopAttackers(-7, 17);

            // top player gets 800 XP, each player down the line receives 35 fewer
            int l = 0;
            int maxReward = 650;

            foreach (BossDamage damage in damages)
            {
                Player victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                int reward = maxReward - (l * 35);
                victor.XP += reward;
                l++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + BossFirstName + " " + BossLastName + ", you earn " + reward + " XP from your spells cast against the plague mother.</b>", true);

                playerRepo.SavePlayer(victor);

            }

        }

        private static List<Player> GetEligibleTargetsInLocation(string location, Player attacker)
        {
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(location).Where(m => m.Mobility == PvPStatics.MobilityFull && 
            m.Id != attacker.Id && 
            m.Form != RegularBimboFormDbName && 
            m.BotId >= AIStatics.PsychopathBotId && 
            m.LastActionTimestamp > cutoff && 
            m.BotId != AIStatics.BimboBossBotId && 
            m.InDuel <= 0 &&
            m.InQuest <= 0).ToList();

            return playersHere;
        }

        private static void DropCure(int turnNumber)
        {

            for (var i = 0; i < 2; i++)
            {

                var cmd = new CreateItem
                {
                    dbLocationName = LocationsStatics.GetRandomLocation(),
                    dbName = CureItemDbName,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 0,
                    PvPEnabled = 2,
                    OwnerId = null,
                    VictimName = "",
                    TurnsUntilUse = 0,
                    EquippedThisTurn = false,
                    ItemSourceId = ItemStatics.GetStaticItem(CureItemDbName).Id
                };

                if (turnNumber % 3 == 0)
                {
                    cmd.PvPEnabled = 1;
                }

                DomainRegistry.Repository.Execute(cmd);

            }

        }

    }
}