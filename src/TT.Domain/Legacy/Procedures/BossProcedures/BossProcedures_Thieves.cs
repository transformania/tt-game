using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Thieves
    {

        private const string MaleBossFirstName = "Brother Lujako";
        private const string MaleBossLastName = "Seekshadow";
        private const string FemaleBossFirstName = "Sister Lujienne";
        private const string FemaleBossLastName = "Seekshadow";

        public const int GoldenTrophySpellSourceId = 609;

        public const int StunSpellSourceId = 588;

        public const int MaleBossFormSourceId = 278;
        public const int FemaleBossFormSourceId = 279;

        public static void SpawnThieves()
        {
            var malethief = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.MaleRatBotId });

            if (malethief == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = MaleBossFirstName,
                    LastName = MaleBossLastName,
                    Location = "tavern_pool",
                    Gender = PvPStatics.GenderMale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = MaleBossFormSourceId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 5,
                    BotId = AIStatics.MaleRatBotId
                };

                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var malethiefEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                malethiefEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(malethiefEF));
                playerRepo.SavePlayer(malethiefEF);


                var aiRepo = new EFAIDirectiveRepository();
                var maleDirective = new AIDirective
                {
                    OwnerId = id,
                    DoNotRecycleMe = true,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    sVar1 = GetRichestPlayerIds(),
                    Var1 = 0,
                };
                aiRepo.SaveAIDirective(maleDirective);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.RAT_THIEF_RUNE, PlayerId = malethiefEF.Id });
                }

            }


            var femalethief = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.FemaleRatBotId });

            if (femalethief == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = FemaleBossFirstName,
                    LastName = FemaleBossLastName,
                    Location = "tavern_pool",
                    Gender = PvPStatics.GenderFemale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = FemaleBossFormSourceId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 7,
                    BotId = AIStatics.FemaleRatBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var femalethiefEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                femalethiefEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(femalethiefEF));
                playerRepo.SavePlayer(femalethiefEF);

                var aiRepo = new EFAIDirectiveRepository();
                var femaleDirective = new AIDirective
                {
                    OwnerId = id,
                    DoNotRecycleMe = true,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    sVar1 = GetRichestPlayerIds(),
                    Var1 = 0,
                };
                aiRepo.SaveAIDirective(femaleDirective);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.RAT_THIEF_RUNE, PlayerId = femalethiefEF.Id });
                }

            }

        }

        public static void RunThievesAction(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var malethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MaleRatBotId);
            var femalethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FemaleRatBotId);
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            var maleAI = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == malethief.Id);
            var femaleAI = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == femalethief.Id);

            // both male and female are no longer animate, end boss event
            if (malethief.Mobility != PvPStatics.MobilityFull && femalethief.Mobility != PvPStatics.MobilityFull)
            {
                EndEvent();
            }

            #region both animate
            // both male and female are animate, have them go to players and loot them!
            if (malethief.Mobility == PvPStatics.MobilityFull && femalethief.Mobility == PvPStatics.MobilityFull)
            {

                // periodically refresh list of targets
                if (turnNumber % 12 == 0)
                {
                    maleAI.sVar1 = GetRichestPlayerIds();
                    maleAI.Var1 = 0;
                }

                if (malethief.Health < malethief.MaxHealth / 6)
                {
                    var malebuffs = ItemProcedures.GetPlayerBuffs(malethief);
                    DomainRegistry.Repository.Execute(new Cleanse { PlayerId = malethief.Id, Buffs = malebuffs, NoValidate = true });
                    malethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MaleRatBotId);
                }

                if (femalethief.Health < femalethief.MaxHealth / 4)
                {
                    var femalebuffs = ItemProcedures.GetPlayerBuffs(femalethief);
                    DomainRegistry.Repository.Execute(new Cleanse { PlayerId = femalethief.Id, Buffs = femalebuffs, NoValidate = true });
                    femalethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FemaleRatBotId);
                }


                var idArray = maleAI.sVar1.Split(';');
                idArray = idArray.Take(idArray.Length - 1).ToArray();

                if (maleAI.Var1 >= idArray.Length)
                {
                    maleAI.Var1 = 0;
                }

                var targetId = Convert.ToInt32(idArray[Convert.ToInt32(maleAI.Var1)]);

                var target = playerRepo.Players.FirstOrDefault(p => p.Id == targetId);

                while ((target == null || PlayerProcedures.PlayerIsOffline(target) || target.Mobility != PvPStatics.MobilityFull || target.Money < 20) && maleAI.Var1 < idArray.Length-1)
                {
                    maleAI.Var1++;
                    targetId = Convert.ToInt32(idArray[Convert.ToInt32(maleAI.Var1)]);
                    target = playerRepo.Players.FirstOrDefault(p => p.Id == targetId);
                }

                // we should hopefully by now have a valid target.  Hopefully.  Now move to them and loot away.
                try { 
                    malethief.dbLocationName = target.dbLocationName;
                    femalethief.dbLocationName = target.dbLocationName;

                    // take money from victim and give some to the thieves with an uneven split.  Multiple the thieves' gain by 3
                    // because only about a third of Arpeyis are actually collected from a completed inanimation
                    target.Money = Math.Floor(target.Money * .90M);
                    malethief.Money += Math.Floor(target.Money * .025M);
                    femalethief.Money += Math.Floor(target.Money * .075M);

                    playerRepo.SavePlayer(target);
                    playerRepo.SavePlayer(malethief);
                    playerRepo.SavePlayer(femalethief);

                    AttackProcedures.Attack(femalethief, target, StunSpellSourceId);
                    AIProcedures.DealBossDamage(femalethief, target, false, 1);

                    var message = malethief.GetFullName() + " and " + femalethief.GetFullName() + " the Seekshadow rat thieves suddenly appear in front of you!  In the blink of an eye they've swept you off your feet and have expertly swiped " + Math.Floor(target.Money * .10M) + " of your Arpeyjis.";
                    var locationMessage = "<b>" + malethief.GetFullName() + " and " + femalethief.GetFullName() + " robbed " + target.GetFullName() + " here.</b>";
                    PlayerLogProcedures.AddPlayerLog(target.Id, message, true);
                    LocationLogProcedures.AddLocationLog(malethief.dbLocationName, locationMessage);

                    maleAI.Var1++;

                    if (maleAI.Var1 >= 20)
                    {
                        maleAI.Var1 = 0;
                    }
                    aiRepo.SaveAIDirective(maleAI);
                }
                catch
                {
                    maleAI.Var1 = 0;
                }
            }
            #endregion

            #region veangance mode
            // one of the thieves has been taken down.  The other will try and steal their inanimate friend back!
            if (malethief.Mobility != PvPStatics.MobilityFull || femalethief.Mobility != PvPStatics.MobilityFull)
            {

                Player attackingThief;
                Player itemThief;
               
                if (malethief.Mobility == PvPStatics.MobilityFull && femalethief.Mobility != PvPStatics.MobilityFull)
                {
                    attackingThief = malethief;
                    itemThief = femalethief;
                }
                else
                {
                    attackingThief = femalethief;
                    itemThief = malethief;
                }

                var victimThiefItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = itemThief.Id});

                // the transformed thief is owned by someone, try and get it back!
                if (victimThiefItem.Owner != null) {
                    var target = playerRepo.Players.FirstOrDefault(p => p.Id == victimThiefItem.Owner.Id);

                    if (target.BotId == AIStatics.MaleRatBotId || target.BotId == AIStatics.FemaleRatBotId)
                    {
                        // do nothing, the thief already has the item... equip it if not
                        if (!victimThiefItem.IsEquipped)
                        {
                            ItemProcedures.EquipItem(victimThiefItem.Id, true);
                        }
                        var newlocation = LocationsStatics.GetRandomLocation_NoStreets();
                        AIProcedures.MoveTo(attackingThief, newlocation, 100000);
                        attackingThief.dbLocationName = newlocation;
                        playerRepo.SavePlayer(attackingThief);
                        var buffs = ItemProcedures.GetPlayerBuffs(attackingThief);

                        if (attackingThief.Health < attackingThief.Health / 10)
                        {
                            DomainRegistry.Repository.Execute(new Cleanse { PlayerId = attackingThief.Id, Buffs = buffs, NoValidate = true});
                        }

                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = attackingThief.Id, Buffs = buffs, NoValidate = true });

                    }

                    // Lindella, steal from her right away
                    else if (target.BotId == AIStatics.LindellaBotId)
                    {
                        ItemProcedures.GiveItemToPlayer(victimThiefItem.Id, attackingThief.Id);
                        LocationLogProcedures.AddLocationLog(target.dbLocationName, "<b>" + attackingThief.GetFullName() + " stole " + victimThiefItem.FormerPlayer.FullName + " the " + victimThiefItem.ItemSource.FriendlyName + " from Lindella.</b>");
                    }

                    // target is a human and they are not offline
                    else if (target != null && !PlayerProcedures.PlayerIsOffline(target))
                    {
                        attackingThief.dbLocationName = target.dbLocationName;
                        playerRepo.SavePlayer(attackingThief);
                        AttackProcedures.Attack(attackingThief, target, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(attackingThief, target, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(attackingThief, target, GoldenTrophySpellSourceId);
                        AttackProcedures.Attack(attackingThief, target, GoldenTrophySpellSourceId);
                        AIProcedures.DealBossDamage(attackingThief, target, false, 4);
                        target = playerRepo.Players.FirstOrDefault(p => p.Id == victimThiefItem.Owner.Id && p.BotId != AIStatics.MaleRatBotId && p.BotId != AIStatics.FemaleRatBotId);

                        // if we have managed to turn the target, take back the victim-item
                        if (target.Mobility != PvPStatics.MobilityFull)
                        {
                            ItemProcedures.GiveItemToPlayer(victimThiefItem.Id, attackingThief.Id);
                            LocationLogProcedures.AddLocationLog(target.dbLocationName, "<b>" + attackingThief.GetFullName() + " recovered " + victimThiefItem.FormerPlayer.FullName + " the " + victimThiefItem.ItemSource.FriendlyName + ".</b>");
                        }
                    }

                    // target is a human and they are offline... just go and camp out there.
                    else if (target != null && PlayerProcedures.PlayerIsOffline(target))
                    {
                        attackingThief.dbLocationName = target.dbLocationName;
                        playerRepo.SavePlayer(attackingThief);
                    }
                }

                // item is on the ground, just go and pick it up.
                else
                {
                    attackingThief.dbLocationName = victimThiefItem.dbLocationName;
                    playerRepo.SavePlayer(attackingThief);
                    ItemProcedures.GiveItemToPlayer(victimThiefItem.Id, attackingThief.Id);
                    LocationLogProcedures.AddLocationLog(attackingThief.dbLocationName, "<b>" + attackingThief.GetFullName() + " recovered " + victimThiefItem.FormerPlayer.FullName + " the " + victimThiefItem.ItemSource.FriendlyName + ".</b>");
                }
            }
            #endregion

        }

        private static string GetRichestPlayerIds()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
            IEnumerable<int> ids = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                p.BotId >= AIStatics.PsychopathBotId &&
                p.OnlineActivityTimestamp >= cutoff &&
                !p.dbLocationName.Contains("dungeon_") &&
                p.InDuel <= 0 &&
                p.InQuest <= 0).OrderByDescending(p => p.Money).Take(20).Select(p => p.Id);

            var output = "";
            foreach (var s in ids)
            {
                output += s.ToString() + ";";
            }

            return output;
        }

        public static void CounterAttack(Player attacker)
        {
        

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var malethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MaleRatBotId);
            var femalethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FemaleRatBotId);
            var rand = new Random(Guid.NewGuid().GetHashCode());

            // both thieves are full, dont' attack too hard
            if (malethief.Mobility == PvPStatics.MobilityFull && femalethief.Mobility == PvPStatics.MobilityFull)
            {
                if (malethief.Mobility == PvPStatics.MobilityFull)
                {
                    AttackProcedures.Attack(malethief, attacker, PvPStatics.Spell_WeakenId);
                    AIProcedures.DealBossDamage(malethief, attacker, true, 1);
                    malethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MaleRatBotId);
                }

                if (femalethief.Mobility == PvPStatics.MobilityFull)
                {
                    AttackProcedures.Attack(femalethief, attacker, GoldenTrophySpellSourceId);
                    AttackProcedures.Attack(femalethief, attacker, GoldenTrophySpellSourceId);
                    AIProcedures.DealBossDamage(femalethief, attacker, true, 2);
                    femalethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FemaleRatBotId);
                }

                
                var roll = rand.NextDouble();

                // random chance of moving to a new random location
                if (roll < .166)
                {
                    AttackProcedures.Attack(femalethief, attacker, StunSpellSourceId);
                    AIProcedures.DealBossDamage(femalethief, attacker, false, 1);
                    var locationMessage = "<b>" + malethief.GetFullName() + " and " + femalethief.GetFullName() + " ran off in an unknown direction.</b>";
                    LocationLogProcedures.AddLocationLog(femalethief.dbLocationName, locationMessage);
                    var newlocation = LocationsStatics.GetRandomLocation_NoStreets();
                    malethief.dbLocationName = newlocation;
                    femalethief.dbLocationName = newlocation;
                    playerRepo.SavePlayer(malethief);
                    playerRepo.SavePlayer(femalethief);
                }
            }

            // one thief is defeated, the other goes berserk
            else 
            {
                var roll = rand.NextDouble() * 3;
                if (malethief.Mobility == PvPStatics.MobilityFull)
                {
                    for (var i = 0; i < roll; i++)
                    {
                        AttackProcedures.Attack(malethief, attacker, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(malethief, attacker, GoldenTrophySpellSourceId);
                        AIProcedures.DealBossDamage(malethief, attacker, false, 2);
                    }
                }
                else if (femalethief.Mobility == PvPStatics.MobilityFull)
                {
                    for (var i = 0; i < roll; i++)
                    {
                        AttackProcedures.Attack(femalethief, attacker, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(femalethief, attacker, GoldenTrophySpellSourceId);
                        AIProcedures.DealBossDamage(femalethief, attacker, false, 2);
                    }
                }
            }

        }

        private static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndThieves();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var malethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.MaleRatBotId);
            var femalethief = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FemaleRatBotId);

            AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(malethief.Id);
            AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(femalethief.Id);

            // find the players who dealt the most damage and award them with money
            var damages_male = AIProcedures.GetTopAttackers(malethief.BotId, 10);
            var damages_female = AIProcedures.GetTopAttackers(femalethief.BotId, 10);

            // top player gets 500 XP, each player down the line receives 25 fewer
            var maxReward_Female = 1000 + Math.Floor(femalethief.Money);
            var maxReward_Male = 300 + Math.Floor(malethief.Money);

            for (var i = 0; i < damages_male.Count; i++)
            {
                var damage = damages_male.ElementAt(i);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                var reward = Math.Floor(maxReward_Male);
                victor.Money += maxReward_Male;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + MaleBossFirstName + " you have been given " + (int)reward + " Arpeyjis from an old bounty placed on him.</b>", true);

                playerRepo.SavePlayer(victor);
                maxReward_Male *= .75M;

                // top two get runes
                if (i <= 1 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.RAT_THIEF_RUNE, PlayerId = victor.Id });
                }

            }

            for (var i = 0; i < damages_female.Count; i++)
            {
                var damage = damages_female.ElementAt(i);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                var reward = Math.Floor(maxReward_Female);
                victor.Money += maxReward_Female;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + FemaleBossFirstName + " you have been given " + (int)reward + " Arpeyjis from an old bounty placed on her.</b>", true);

                playerRepo.SavePlayer(victor);
                maxReward_Female *= .75M;

                // top two get runes
                if (i <= 1 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.RAT_THIEF_RUNE, PlayerId = victor.Id });
                }

            }

        }

    }

}