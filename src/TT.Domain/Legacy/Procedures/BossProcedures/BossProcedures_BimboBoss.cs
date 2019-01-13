using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
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
        public const int KissSkillSourceId = 528;
        public const string CureEffectdbName = "blessing_bimboboss_cure";
        //public const string RegularTFSpellDbName = "skill_Bringer_of_the_Bimbocalypse_Judoo";
        public const int RegularTFSpellSourceId = 532;
        private const string RegularBimboFormDbName = "form_Bimbocalypse_Plague_Victim_Judoo";
        public const string CureItemDbName = "item_consumeable_bimbo_cure";
        public const int CureItemSourceId = 143;
        private const int BimboBossFormId = 233;

        public static void SpawnBimboBoss()
        {
            var bimboBoss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.BimboBossBotId });

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
                var bimboEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);

                bimboEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(bimboEF));

                playerRepo.SavePlayer(bimboEF);


                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                var directive = new AIDirective
                {
                    OwnerId = id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.BIMBO_RUNE, PlayerId = bimboEF.Id });
                }
                

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
                AttackProcedures.Attack(bimboss, human, KissSkillSourceId);
                AIProcedures.DealBossDamage(bimboss, human, false, 1);
            }

            // otherwise run the regular trasformation
            else if (human.Form != RegularBimboFormDbName)
            {
                var rand = new Random(Guid.NewGuid().GetHashCode());
                var attackCount = (int)Math.Floor(rand.NextDouble() * 2 + 1);
                for (var i = 0; i < attackCount; i++) {
                    AttackProcedures.Attack(bimboss, human, KissSkillSourceId);
                }
                AIProcedures.DealBossDamage(bimboss, human, false, attackCount);
            }

            // otherwise make the human wander away to find more targets
            else
            {
                var targetLocation = GetLocationWithMostEligibleTargets();
                var newlocation = AIProcedures.MoveTo(human, targetLocation, 8);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var dbHuman = playerRepo.Players.FirstOrDefault(p => p.Id == human.Id);
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
                var message = "Lady Lovebringer is not pleased with you attacking her after she has so graciously given you that sexy body and carefree mind.  She whispers something into your ear that causes your body to grow limp in her arms, then injects you with a serum that makes your mind just a bit foggier and loyal to your bimbonic mother.  She orders you away to find new targets to spread the virus to.  The combination of lust and her command leaves you with no choice but to mindlessly obey...";
                PlayerLogProcedures.AddPlayerLog(human.Id, message, true);
            }

        }

        public static void RunActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var bimboBoss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.BimboBossBotId);

            // move her toward the location with the most eligible targets
            if (bimboBoss == null || bimboBoss.Mobility != PvPStatics.MobilityFull) {
                EndEvent();
                return;
            }

            var targetLocation = GetLocationWithMostEligibleTargets();
            var newlocation = AIProcedures.MoveTo(bimboBoss, targetLocation, 12);

            bimboBoss.dbLocationName = newlocation;

            playerRepo.SavePlayer(bimboBoss);
            bimboBoss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.BimboBossBotId);

            var rand = new Random(Guid.NewGuid().GetHashCode());

            // attack all eligible targets here, even if it's not her final destination
            var playersHere = GetEligibleTargetsInLocation(newlocation, bimboBoss);

            foreach (var p in playersHere)
            {
                // if the player doesn't currently have it, give them the infection kiss
                if (!EffectProcedures.PlayerHasEffect(p, KissEffectdbName) && !EffectProcedures.PlayerHasEffect(p, CureEffectdbName))
                {
                    AttackProcedures.Attack(bimboBoss, p, KissSkillSourceId);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 1);
                }


                // otherwise run the regular trasformation
                else if (p.Form != RegularBimboFormDbName)
                {
                    AttackProcedures.Attack(bimboBoss, p, RegularTFSpellSourceId);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 1);
                }
            }

            // have a random chance that infected players spontaneously transform
            IEffectRepository effectRepo = new EFEffectRepository();
            var ownerIds = effectRepo.Effects.Where(e => e.dbName == KissEffectdbName).Select(e => e.OwnerId).ToList();

            foreach (var effectId in ownerIds)
            {

                var infectee = playerRepo.Players.FirstOrDefault(p => p.Id == effectId);

                // if the infectee is no longer animate or is another boss, skip them
                if (infectee.Mobility != PvPStatics.MobilityFull || infectee.BotId < AIStatics.PsychopathBotId)
                {
                    continue;
                }

                var roll = rand.NextDouble();

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

                        var message = "You gasp, your body shifting as the virus infecting you overwhelms your biological and arcane defenses.  Before long you find that your body has been transformed into that of one of the many bimbonic plague victims and you can't help but succumb to the urges to spread your infection--no, your gift!--on to the rest of mankind.";
                        var loclogMessage = "<b style='color: red'>" + infectee.GetFullName() + " succumbed to the bimbonic virus, spontaneously transforming into one of Lady Lovebringer's bimbos.</b>";

                        PlayerLogProcedures.AddPlayerLog(infectee.Id, message, true);
                        LocationLogProcedures.AddLocationLog(infectee.dbLocationName, loclogMessage);
                    }
                }

                // spread the kiss so long as the player is not offline
                if (infectee.Form == RegularBimboFormDbName && !PlayerProcedures.PlayerIsOffline(infectee))
                {
                    // back up the last action timestamp since we don't want these attacks to count against their offline timer
                    var lastActionBackup = infectee.LastActionTimestamp;

                    var eligibleTargets = GetEligibleTargetsInLocation(infectee.dbLocationName, infectee);
                    var attacksMadeCount = 0;

                    foreach (var p in eligibleTargets)
                    {
                        if (!EffectProcedures.PlayerHasEffect(p, KissEffectdbName) && !EffectProcedures.PlayerHasEffect(p, CureEffectdbName) && attacksMadeCount < 3)
                        {
                            attacksMadeCount++;
                            AttackProcedures.Attack(infectee, p, KissSkillSourceId);
                        }
                        else if (attacksMadeCount < 3)
                        {
                            AttackProcedures.Attack(infectee, p, RegularTFSpellSourceId);
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
            if (ShouldHeal(bimboBoss, turnNumber))
            {
                
                var activeCurses = effectRepo.Effects.Count(eff => eff.dbName == KissEffectdbName)*3;
                activeCurses = activeCurses > 75 ? 75 : activeCurses;
                bimboBoss.Health += activeCurses;
                playerRepo.SavePlayer(bimboBoss);
                var message = "<b>" + bimboBoss.GetFullName() + " draws energy from her bimbo horde, regenerating her own willpower by " + activeCurses + ".</b>";
                LocationLogProcedures.AddLocationLog(newlocation, message);
            }

            // drop a cure
            DropCure(turnNumber);


        }

        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = DateTime.UtcNow.AddMinutes(-PvPStatics.OfflineAfterXMinutes);
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull && 
                p.LastActionTimestamp > cutoff && 
                p.Form != RegularBimboFormDbName && 
                !p.dbLocationName.Contains("dungeon_") &&
                p.InDuel <= 0 &&
                p.InQuest <= 0)
                .GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key).ToList();
            return locs.First();
        }

        public static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndBimbo();

            // delete all bimbo effects, both the kiss and the cure
            IEffectRepository effectRepo = new EFEffectRepository();
            var effectsToDelete = effectRepo.Effects.Where(e => e.dbName == KissEffectdbName || e.dbName == CureEffectdbName).ToList();

            foreach (var e in effectsToDelete)
            {
                effectRepo.DeleteEffect(e.Id);
            }

            // delete all the cure vials
            var cmd = new GetAllItemsOfType { ItemSourceId = CureItemSourceId};
            var cures = DomainRegistry.Repository.Find(cmd);

            foreach (var cure in cures)
            {
                var deleteCmd = new DeleteItem {ItemId = cure.Id};
                DomainRegistry.Repository.Execute(deleteCmd);
            }

            // restore any bimbos back to their base form and notify them

            var message = "Your body suddenly returns to normal as the bimbonic virus in your body suddenly goes into submission, the psychic link between you and your plague mother separated for good.  Due to the bravery of your fellow mages the Bimbocalypse has been thwarted... for now.";

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var infected = playerRepo.Players.Where(p => p.Form == RegularBimboFormDbName).ToList();
            
            foreach (var p in infected)
            {
                PlayerProcedures.InstantRestoreToBase(p);
                if (p.BotId == AIStatics.ActivePlayerBotId)
                {
                    PlayerLogProcedures.AddPlayerLog(p.Id, message, true);
                }
            }

            var damages = AIProcedures.GetTopAttackers(-7, 17);

            // top player gets 800 XP, each player down the line receives 35 fewer
            var l = 0;
            var maxReward = 650;

            for (var i = 0; i < damages.Count; i++)
            {
                var damage = damages.ElementAt(i);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                var reward = maxReward - (l * 35);
                victor.XP += reward;
                l++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + BossFirstName + " " + BossLastName + ", you earn " + reward + " XP from your spells cast against the plague mother.</b>", true);

                playerRepo.SavePlayer(victor);

                // top three get runes
                if (i <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.BIMBO_RUNE, PlayerId = victor.Id });
                }

            }

        }

        private static List<Player> GetEligibleTargetsInLocation(string location, Player attacker)
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            var playersHere = PlayerProcedures.GetPlayersAtLocation(location).Where(m => m.Mobility == PvPStatics.MobilityFull && 
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
                    dbLocationName = LocationsStatics.GetRandomLocationNotInDungeon(),
                    dbName = CureItemDbName,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 0,
                    PvPEnabled = 2,
                    OwnerId = null,
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

        private static bool ShouldHeal(Player lovebringer, int turnNumber)
        {
            return turnNumber % 5 == 0 && lovebringer.Health < lovebringer.MaxHealth / 6;
        }

    }
}