using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Players;
using TT.Domain.Concrete;
using TT.Domain.DTOs.Players;
using TT.Domain.Models;
using TT.Domain.Queries.Players;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Thieves
    {

        private const string MaleBossFirstName = "Brother Lujako";
        private const string MaleBossLastName = "Seekshadow";
        public const string MaleBossFormDbName = "form_Apprentice_Seekshadow_Thief_Judoo";
        private const string FemaleBossFirstName = "Sister Lujienne";
        private const string FemaleBossLastName = "Seekshadow";
        public const string FemaleBossFormDbName = "form_Master_Seekshadow_Thief_Judoo";
        public const string GoldenTrophySpellDbName = "skill_Seekshadow's_Triumph_Judoo";

        private const int MaleBossFormId = 278;
        private const int FemaleBossFormId = 279;

        public static void SpawnThieves()
        {
            PlayerDetail malethief = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.MaleRatBotId });

            if (malethief == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = MaleBossFirstName,
                    LastName = MaleBossLastName,
                    Location = "tavern_pool",
                    Gender = PvPStatics.GenderMale,
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = MaleBossFormDbName,
                    FormSourceId = MaleBossFormId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 5,
                    BotId = AIStatics.MaleRatBotId
                };

                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                Player malethiefEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                malethiefEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(malethiefEF));
                playerRepo.SavePlayer(malethiefEF);


                EFAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective maleDirective = new AIDirective
                {
                    OwnerId = id,
                    DoNotRecycleMe = true,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    sVar1 = GetRichestPlayerIds(),
                    Var1 = 0,
                };
                aiRepo.SaveAIDirective(maleDirective);
            }


            PlayerDetail femalethief = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.FemaleRatBotId });

            if (femalethief == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = FemaleBossFirstName,
                    LastName = FemaleBossLastName,
                    Location = "tavern_pool",
                    Gender = PvPStatics.GenderFemale,
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = FemaleBossFormDbName,
                    FormSourceId = FemaleBossFormId,
                    Money = 0,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 7,
                    BotId = AIStatics.FemaleRatBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                Player femalethiefEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                femalethiefEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(femalethiefEF));
                playerRepo.SavePlayer(femalethiefEF);

                EFAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective femaleDirective = new AIDirective
                {
                    OwnerId = id,
                    DoNotRecycleMe = true,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    sVar1 = GetRichestPlayerIds(),
                    Var1 = 0,
                };
                aiRepo.SaveAIDirective(femaleDirective);

            }

        }

        public static void RunThievesAction(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);
            Player femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective maleAI = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == malethief.Id);
            AIDirective femaleAI = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == femalethief.Id);

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
                    BuffBox malebuffs = ItemProcedures.GetPlayerBuffs(malethief);
                    DomainRegistry.Repository.Execute(new Cleanse { PlayerId = malethief.Id, Buffs = malebuffs, NoValidate = true });
                    malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);
                }

                if (femalethief.Health < femalethief.MaxHealth / 4)
                {
                    BuffBox femalebuffs = ItemProcedures.GetPlayerBuffs(femalethief);
                    DomainRegistry.Repository.Execute(new Cleanse { PlayerId = femalethief.Id, Buffs = femalebuffs, NoValidate = true });
                    femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);
                }


                string[] idArray = maleAI.sVar1.Split(';');
                idArray = idArray.Take(idArray.Length - 1).ToArray();

                if (maleAI.Var1 >= idArray.Length)
                {
                    maleAI.Var1 = 0;
                }

                int targetId = Convert.ToInt32(idArray[Convert.ToInt32(maleAI.Var1)]);

                Player target = playerRepo.Players.FirstOrDefault(p => p.Id == targetId);

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

                    AttackProcedures.Attack(femalethief, target, "skill_Seekshadow's_Silence_Judoo");
                    AIProcedures.DealBossDamage(femalethief, target, false, 1);

                    string message = malethief.GetFullName() + " and " + femalethief.GetFullName() + " the Seekshadow rat thieves suddenly appear in front of you!  In the blink of an eye they've swept you off your feet and have expertly swiped " + Math.Floor(target.Money * .10M) + " of your Arpeyjis.";
                    string locationMessage = "<b>" + malethief.GetFullName() + " and " + femalethief.GetFullName() + " robbed " + target.GetFullName() + " here.</b>";
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

                Item victimThiefItem = ItemProcedures.GetItemByVictimName(itemThief.FirstName, itemThief.LastName);
                ItemViewModel victimThiefItemPlus = ItemProcedures.GetItemViewModel(victimThiefItem.Id);

                // the transformed thief is owned by someone, try and get it back!
                if (victimThiefItem.OwnerId > 0) {
                    Player target = playerRepo.Players.FirstOrDefault(p => p.Id == victimThiefItem.OwnerId);

                    if (target.BotId == AIStatics.MaleRatBotId || target.BotId == AIStatics.FemaleRatBotId)
                    {
                        // do nothing, the thief already has the item... equip it if not
                        if (!victimThiefItem.IsEquipped)
                        {
                            ItemProcedures.EquipItem(victimThiefItem.Id, true);
                        }
                        string newlocation = LocationsStatics.GetRandomLocation_NoStreets();
                        AIProcedures.MoveTo(attackingThief, newlocation, 99999);
                        attackingThief.dbLocationName = newlocation;
                        playerRepo.SavePlayer(attackingThief);
                        BuffBox buffs = ItemProcedures.GetPlayerBuffs(attackingThief);

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
                        LocationLogProcedures.AddLocationLog(target.dbLocationName, "<b>" + attackingThief.GetFullName() + " stole " + victimThiefItem.GetFullName() + " the " + victimThiefItemPlus.Item.FriendlyName + " from Lindella.</b>");
                    }

                    // target is a human and they are not offline
                    else if (target != null && !PlayerProcedures.PlayerIsOffline(target))
                    {
                        attackingThief.dbLocationName = target.dbLocationName;
                        playerRepo.SavePlayer(attackingThief);
                        AttackProcedures.Attack(attackingThief, target, "lowerHealth");
                        AttackProcedures.Attack(attackingThief, target, "lowerHealth");
                        AttackProcedures.Attack(attackingThief, target, "skill_Seekshadow's_Triumph_Judoo");
                        AttackProcedures.Attack(attackingThief, target, "skill_Seekshadow's_Triumph_Judoo");
                        AIProcedures.DealBossDamage(attackingThief, target, false, 4);
                        target = playerRepo.Players.FirstOrDefault(p => p.Id == victimThiefItem.OwnerId && p.BotId != AIStatics.MaleRatBotId && p.BotId != AIStatics.FemaleRatBotId);

                        // if we have managed to turn the target, take back the victim-item
                        if (target.Mobility != PvPStatics.MobilityFull)
                        {
                            ItemProcedures.GiveItemToPlayer(victimThiefItem.Id, attackingThief.Id);
                            LocationLogProcedures.AddLocationLog(target.dbLocationName, "<b>" + attackingThief.GetFullName() + " recovered " + victimThiefItem.GetFullName() + " the " + victimThiefItemPlus.Item.FriendlyName + ".</b>");
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
                    LocationLogProcedures.AddLocationLog(attackingThief.dbLocationName, "<b>" + attackingThief.GetFullName() + " recovered " + victimThiefItem.GetFullName() + " the " + victimThiefItemPlus.Item.FriendlyName + ".</b>");
                }
            }
            #endregion

        }

        private static string GetRichestPlayerIds()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            DateTime cutoff = DateTime.UtcNow.AddHours(-1);
            IEnumerable<int> ids = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                p.BotId >= AIStatics.PsychopathBotId &&
                p.OnlineActivityTimestamp >= cutoff &&
                !p.dbLocationName.Contains("dungeon_") &&
                p.InDuel <= 0 &&
                p.InQuest <= 0).OrderByDescending(p => p.Money).Take(20).Select(p => p.Id);

            string output = "";
            foreach (int s in ids)
            {
                output += s.ToString() + ";";
            }

            return output;
        }

        public static void CounterAttack(Player attacker)
        {
        

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);
            Player femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // both thieves are full, dont' attack too hard
            if (malethief.Mobility == PvPStatics.MobilityFull && femalethief.Mobility == PvPStatics.MobilityFull)
            {
                if (malethief.Mobility == PvPStatics.MobilityFull)
                {
                    AttackProcedures.Attack(malethief, attacker, "lowerHealth");
                    AIProcedures.DealBossDamage(malethief, attacker, true, 1);
                    malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);
                }

                if (femalethief.Mobility == PvPStatics.MobilityFull)
                {
                    AttackProcedures.Attack(femalethief, attacker, "skill_Seekshadow's_Triumph_Judoo");
                    AttackProcedures.Attack(femalethief, attacker, "skill_Seekshadow's_Triumph_Judoo");
                    AIProcedures.DealBossDamage(femalethief, attacker, true, 2);
                    femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);
                }

                
                double roll = rand.NextDouble();

                // random chance of moving to a new random location
                if (roll < .166)
                {
                    AttackProcedures.Attack(femalethief, attacker, "skill_Seekshadow's_Silence_Judoo");
                    AIProcedures.DealBossDamage(femalethief, attacker, false, 1);
                    string locationMessage = "<b>" + malethief.GetFullName() + " and " + femalethief.GetFullName() + " ran off in an unknown direction.</b>";
                    LocationLogProcedures.AddLocationLog(femalethief.dbLocationName, locationMessage);
                    string newlocation = LocationsStatics.GetRandomLocation_NoStreets();
                    malethief.dbLocationName = newlocation;
                    femalethief.dbLocationName = newlocation;
                    playerRepo.SavePlayer(malethief);
                    playerRepo.SavePlayer(femalethief);
                }
            }

            // one thief is defeated, the other goes berserk
            else 
            {
                double roll = rand.NextDouble() * 3;
                if (malethief.Mobility == PvPStatics.MobilityFull)
                {
                    for (int i = 0; i < roll; i++)
                    {
                        AttackProcedures.Attack(malethief, attacker, "lowerHealth");
                        AttackProcedures.Attack(malethief, attacker, "skill_Seekshadow's_Triumph_Judoo");
                        AIProcedures.DealBossDamage(malethief, attacker, false, 2);
                    }
                }
                else if (femalethief.Mobility == PvPStatics.MobilityFull)
                {
                    for (int i = 0; i < roll; i++)
                    {
                        AttackProcedures.Attack(femalethief, attacker, "lowerHealth");
                        AttackProcedures.Attack(femalethief, attacker, "skill_Seekshadow's_Triumph_Judoo");
                        AIProcedures.DealBossDamage(femalethief, attacker, false, 2);
                    }
                }
            }

        }

        private static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndThieves();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);
            Player femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);

            AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(malethief.Id);
            AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(femalethief.Id);

            // find the players who dealt the most damage and award them with money
            List<BossDamage> damages_male = AIProcedures.GetTopAttackers(malethief.BotId, 10);
            List<BossDamage> damages_female = AIProcedures.GetTopAttackers(femalethief.BotId, 10);

            // top player gets 500 XP, each player down the line receives 25 fewer
            decimal maxReward_Female = 1000 + Math.Floor(femalethief.Money);
            decimal maxReward_Male = 300 + Math.Floor(malethief.Money);

            foreach (BossDamage damage in damages_male)
            {
                Player victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                decimal reward = Math.Floor(maxReward_Male);
                victor.Money += maxReward_Male;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + MaleBossFirstName + " you have been given " + (int)reward + " Arpeyjis from an old bounty placed on him.</b>", true);

                playerRepo.SavePlayer(victor);
                maxReward_Male *= .75M; 
            }

            foreach (BossDamage damage in damages_female)
            {
                Player victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                decimal reward = Math.Floor(maxReward_Female);
                victor.Money += maxReward_Female;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + FemaleBossFirstName + " you have been given " + (int)reward + " Arpeyjis from an old bounty placed on her.</b>", true);

                playerRepo.SavePlayer(victor);
                maxReward_Female *= .75M;
            }

        }

    }

}