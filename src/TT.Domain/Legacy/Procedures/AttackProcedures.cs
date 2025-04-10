﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class AttackProcedures
    {

       public static string AttackSequence(Player attacker, Player victim, SkillViewModel skillBeingUsed, bool timestamp = true)
        {
            // Actual attack
            var (_, message) = AttackProcedures.Attack(attacker, victim, skillBeingUsed, timestamp);

            // record into statistics
            StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__SpellsCast, 1);

            // Ignore those with boss interactions disabled.
            var BossDisabled = PlayerProcedures.GetPlayerBossDisable(attacker.MembershipId);

            if (AIStatics.IsABoss(victim.BotId) && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__BossAllAttacks, 1);
            }

            if ((victim.BotId == AIStatics.FemaleRatBotId || victim.BotId == AIStatics.MaleRatBotId) && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__BossRatThiefAttacks, 1);
            }
            else if (victim.BotId == AIStatics.BimboBossBotId && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__BossLovebringerAttacks, 1);
            }
            else if (victim.BotId == AIStatics.DonnaBotId && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__BossDonnaAttacks, 1);
            }
            else if (victim.BotId == AIStatics.FaebossBotId && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__FaebossAttacks, 1);
            }
            else if ((victim.BotId == AIStatics.MouseNerdBotId || victim.BotId == AIStatics.MouseBimboBotId) && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__MouseSisterAttacks, 1);
            }
            else if (victim.BotId == AIStatics.MotorcycleGangLeaderBotId && !BossDisabled)
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__MotorcycleGangAttacks, 1);
            }
            else if (AIStatics.IsAMiniboss(victim.BotId) && !(victim.BotId == AIStatics.MinibossPlushAngelId || victim.BotId == AIStatics.MinibossPlushDemonId)) // The plushies just want to make friends. No rewards for monsters.
            {
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__MinibossAttacks, 1);
            }

            // Trigger any counterattack
            AIProcedures.CheckAICounterattackRoutine(attacker, victim);

            return message;
        }

        public static (bool, string) Attack(Player attackingPlayer, Player attackedPlayer, SkillViewModel skillBeingUsed, bool timestamp = true)
        {

            var result = "";

            var attacker = PlayerProcedures.GetPlayer(attackingPlayer.Id);
            var victim = PlayerProcedures.GetPlayer(attackedPlayer.Id);

            if (victim.Mobility != PvPStatics.MobilityFull ||
                attacker.Mobility != PvPStatics.MobilityFull ||
                victim.GameMode == (int)GameModeStatics.GameModes.Invisible ||
                attacker.GameMode == (int)GameModeStatics.GameModes.Invisible)
            {
                return (false, "");
            }

            var complete = false;
            var logs = new LogBox();

            // all of our checks seem to be okay.  So let's lower the player's mana and action points
            PlayerProcedures.ChangePlayerActionMana(-PvPStatics.AttackCost, 0, -PvPStatics.AttackManaCost, attacker.Id, timestamp);

            PlayerProcedures.LogCombatTimestampsAndAddAttackCount(victim, attacker);

            var attackerFullName = attacker.GetFullName();
            var victimFullName = victim.GetFullName();

            // if the spell is a curse, give the effect and that's all
            if (skillBeingUsed.StaticSkill.GivesEffectSourceId != null)
            {
                var effectBeingGiven = EffectStatics.GetDbStaticEffect(skillBeingUsed.StaticSkill.GivesEffectSourceId.Value);

                EffectProcedures.GivePerkToPlayer(skillBeingUsed.StaticSkill.GivesEffectSourceId.Value, victim);

                if (attacker.Gender == PvPStatics.GenderMale && !effectBeingGiven.AttackerWhenHit_M.IsNullOrEmpty())
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit_M;
                }
                else if (attacker.Gender == PvPStatics.GenderFemale && !effectBeingGiven.AttackerWhenHit_F.IsNullOrEmpty())
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit_F;
                }
                else
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit;
                }

                if (!String.IsNullOrEmpty(logs.AttackerLog))
                {
                    logs.AttackerLog += "<br><br>";
                }

                logs.LocationLog = attackerFullName + " cursed " + victimFullName + " with " + skillBeingUsed.StaticSkill.FriendlyName + ".";
                logs.AttackerLog += "You cursed " + victimFullName + " with " + skillBeingUsed.StaticSkill.FriendlyName + ".";
                logs.AttackerLog += "  (+1 XP)  ";
                logs.AttackerLog += PlayerProcedures.GiveXP(attacker, 1);
                logs.VictimLog = effectBeingGiven.MessageWhenHit;
                logs.VictimLog += "  <span class='playerAttackNotification'>" + attackerFullName + " cursed you with <b>" + skillBeingUsed.StaticSkill.FriendlyName + "</b>.</b></span>  ";
                result = logs.AttackerLog;

            }

            // the spell is a regular attack
            else
            {
                logs.LocationLog = attackerFullName + " cast " + skillBeingUsed.StaticSkill.FriendlyName + " against " + victimFullName + ".";
                logs.AttackerLog = "You cast " + skillBeingUsed.StaticSkill.FriendlyName + " against " + victimFullName + ".  ";
                logs.VictimLog = "<span class='playerAttackNotification'>" + attackerFullName + " cast " + skillBeingUsed.StaticSkill.FriendlyName + " against you.</span>  ";

                var meBuffs = ItemProcedures.GetPlayerBuffs(attacker);
                var targetedBuffs = ItemProcedures.GetPlayerBuffs(victim);

                var rand = new Random(Guid.NewGuid().GetHashCode());
                var basehitChance = rand.NextDouble() * 100;

                var meDmgExtra = meBuffs.SpellExtraHealthDamagePercent();

                var criticalMissPercentChance = PvPStatics.CriticalMissPercentChance - meBuffs.SpellMisfireChanceReduction();

                var criticalPercentChance = meBuffs.ExtraSkillCriticalPercent() + PvPStatics.CriticalHitPercentChance;
                var evasionPercentChance = targetedBuffs.EvasionPercent() - meBuffs.EvasionNegationPercent();
                var evasionUpgrade = false;
                var failedAttack = false;

                // clamp modifiedEvasion at 50% max
                if (evasionPercentChance > 50)
                {
                    evasionPercentChance = 50;
                }

                // critical miss!  damage caster instead
                if (basehitChance < (double)criticalMissPercentChance)
                {
                    // check if there is a health damage aspect to this spell
                    if (skillBeingUsed.StaticSkill.HealthDamageAmount > 0)
                    {
                        var amountToDamage = skillBeingUsed.StaticSkill.HealthDamageAmount *
                                             (1 + meDmgExtra / 100);
                        PlayerProcedures.DamagePlayerHealth(attacker.Id, amountToDamage);
                        logs.AttackerLog += $"Misfire!  Your spell accidentally lowered your own willpower by {amountToDamage:N2}.  ";
                        logs.VictimLog += $"Misfire!  {GetPronoun_HisHer(attacker.Gender)} spell accidentally lowered {GetPronoun_hisher(attacker.Gender)} own willpower by {amountToDamage:N2}.";
                        result += logs.AttackerLog;
                    }
                    failedAttack = true;
                }
                // spell is evaded
                else if (basehitChance < (double)criticalMissPercentChance + (double)evasionPercentChance)
                {
                    // Check for a crit to upgrade the miss to a hit
                    var criticalHitChance = rand.NextDouble() * 100;
                    if (criticalHitChance < (double)criticalPercentChance)
                    {
                        evasionUpgrade = true;
                    }
                    else
                    {
                        logs.AttackerLog += victimFullName + " managed to leap out of the way of your spell.";
                        logs.VictimLog += "You managed to leap out of the way of " + attackerFullName + "'s spell.";
                        result = logs.AttackerLog;
                        failedAttack = true;
                    }
                }

                // not a  miss, so let's deal some damage, possibly
                if (!failedAttack)
                {
                    decimal criticalModifier = 1;

                    if (evasionUpgrade)
                    {
                        logs.AttackerLog += "<b>Piercing hit!</b>  ";
                        logs.VictimLog += "<b>Piercing hit!</b>  ";
                    }
                    else if (rand.NextDouble() * 100 < (double)criticalPercentChance)
                    {
                        criticalModifier = 2;
                        logs.AttackerLog += "<b>Critical hit!</b>  ";
                        logs.VictimLog += "<b>Critical hit!</b>  ";
                    }

                    var initialVictimHealth = victim.Health;

                    // check if there is a health damage aspect to this spell
                    if (skillBeingUsed.StaticSkill.HealthDamageAmount > 0)
                    {
                        var targetProt = targetedBuffs.SpellHealthDamageResistance();

                        // calculator the modifier as extra attack - defense.      15 - 20 = -5 modifier
                        var willpowerDamageModifierFromBonuses = 1 + ((meDmgExtra - targetProt) / 100.0M);

                        // cap the modifier at at 50 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses < .5M)
                        {
                            willpowerDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at 200 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses > 2 && victim.BotId == AIStatics.ActivePlayerBotId)
                        {
                            willpowerDamageModifierFromBonuses = 2;
                        }

                        var totalHealthDamage = skillBeingUsed.StaticSkill.HealthDamageAmount * willpowerDamageModifierFromBonuses * criticalModifier;

                        // make sure damage is never in the negatives (which would heal instead)
                        if (totalHealthDamage < 0)
                        {
                            totalHealthDamage = 0;
                        }

                        PlayerProcedures.DamagePlayerHealth(victim.Id, totalHealthDamage);

                        // even though it's been done in the db, change the player health here as well
                        victim.Health -= totalHealthDamage;


                        logs.AttackerLog += $"Your spell lowered {GetPronoun_hisher(victim.Gender)} willpower by {Math.Round(totalHealthDamage, 2)}.  ";
                        logs.VictimLog += $"{GetPronoun_HisHer(attacker.Gender)} spell lowered your willpower by {Math.Round(totalHealthDamage, 2)}.  ";
                        result += logs.AttackerLog;
                    }

                    // if this skill has any TF power, add energy and check for form change
                    if (skillBeingUsed.StaticSkill.TFPointsAmount > 0)
                    {

                        var TFEnergyDmg = meBuffs.SpellExtraTFEnergyPercent();
                        var TFEnergyArmor = targetedBuffs.SpellTFEnergyDamageResistance();

                        // calculator the modifier as extra attack - defense.
                        var tfEnergyDamageModifierFromBonuses = 1 + ((TFEnergyDmg - TFEnergyArmor) / 100.0M);

                        // cap the modifier at at 50 % IF the target is a human
                        if (tfEnergyDamageModifierFromBonuses < .5M)
                        {
                            tfEnergyDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at at 200 % IF the target is a human
                        if (tfEnergyDamageModifierFromBonuses > 2 && victim.BotId == AIStatics.ActivePlayerBotId)
                        {
                            tfEnergyDamageModifierFromBonuses = 2;
                        }

                        var totalTFEnergyModifier = criticalModifier * tfEnergyDamageModifierFromBonuses;

                        LogBox tfEnergyResult;
                        (complete, tfEnergyResult) = TFEnergyProcedures.AddTFEnergyToPlayer(victim, attacker, skillBeingUsed, totalTFEnergyModifier, initialVictimHealth);
                        logs.Add(tfEnergyResult);

                        result = logs.AttackerLog;

                    }

                }

            }

            LocationLogProcedures.AddLocationLog(attacker.dbLocationName, logs.LocationLog, LogStatics.LOG_TYPE_ATTACK);
            PlayerLogProcedures.AddPlayerLog(attacker.Id, logs.AttackerLog, false);
            PlayerLogProcedures.AddPlayerLog(victim.Id, logs.VictimLog, true);

            DomainRegistry.AttackNotificationBroker.Notify(victim.Id, logs.VictimLog);

            // Provide victim with a link to their aggressor, without saving it in logs.
            DomainRegistry.AttackNotificationBroker.Notify(victim.Id, "You were just attacked by <a href='/pvp/lookatplayer/" + attacker.Id + "'>" + attackerFullName + "</a>!");

            // if this is a psycho-on-psycho battle, have a chance for the victim bot to switch targets to the attacker bot
            if (attacker.BotId == AIStatics.PsychopathBotId && victim.BotId == AIStatics.PsychopathBotId)
            {
                var rand = new Random(Guid.NewGuid().GetHashCode());
                var botAggroRoll = rand.NextDouble();
                if (botAggroRoll < .08)
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(victim.Id, attacker.Id);
                }
            }

            return (complete, result);
        }

        public static (bool, string) Attack(Player attacker, Player victim, int skillSourceId)
        {
            var vm = SkillProcedures.GetSkillViewModel_NotOwned(skillSourceId);

            if (vm == null)
            {
                return (false, "");
            }

            return Attack(attacker, victim, vm);
        }

        public static string ThrowGrenade(Player attacker, decimal damage, string orbStrengthName)
        {

            IPlayerRepository playerREpo = new EFPlayerRepository();

            var attackerLocation = attacker.dbLocationName;
            var here = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == attackerLocation);

            var playersHere = new List<Player>();
            var playersHereOnline = new List<Player>();
            if (attacker.GameMode == (int)GameModeStatics.GameModes.PvP)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attackerLocation &&
                    (p.GameMode == (int)GameModeStatics.GameModes.PvP || p.BotId < AIStatics.RerolledPlayerBotId) &&
                    p.Mobility == PvPStatics.MobilityFull &&
                     p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }
            else if (attacker.GameMode == (int)GameModeStatics.GameModes.Protection || attacker.GameMode == (int)GameModeStatics.GameModes.Superprotection)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attackerLocation &&
                    p.BotId < AIStatics.RerolledPlayerBotId &&
                    p.Mobility == PvPStatics.MobilityFull &&
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }

            // filter out offline players as well as the attacker
            foreach (var p in playersHere)
            {
                if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != attacker.Id)
                {
                    playersHereOnline.Add(p);
                }
            }

            foreach (var p in playersHereOnline)
            {
                p.Health -= damage;
                if (p.Health < 0)
                {
                    p.Health = 0;
                }
                playerREpo.SavePlayer(p);
                var message = "<span class='playerAttackNotification'>" + attacker.GetFullName() + " threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering your willpower by " + damage + " along with " + (playersHereOnline.Count - 1) + " others.</span>";
                PlayerLogProcedures.AddPlayerLog(p.Id, message, true);

            }

            var logMessage = attacker.FirstName + " " + attacker.LastName + " threw a Submissiveness Splash Orb here.";
            LocationLogProcedures.AddLocationLog(attackerLocation, logMessage);

            var attackerMessage = "You threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering " + playersHereOnline.Count + " people's willpower by " + damage + " each.";
            PlayerLogProcedures.AddPlayerLog(attacker.Id, attackerMessage, false);

            // set the player's last action flag, combat time
            var dbAttacker = playerREpo.Players.First(p => p.Id == attacker.Id);
            dbAttacker.LastActionTimestamp = DateTime.UtcNow;
            dbAttacker.LastCombatTimestamp = DateTime.UtcNow;
            dbAttacker.TimesAttackingThisUpdate++;
            playerREpo.SavePlayer(dbAttacker);


            return attackerMessage;
        }

        public static string SuddenDeathExplosion(Player attacker, Player victim, decimal damage)
        {

            IPlayerRepository playerREpo = new EFPlayerRepository();

            var attackerLocation = attacker.dbLocationName;
            var here = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == attackerLocation);

            var playersHere = new List<Player>();
            var playersHereOnline = new List<Player>();
            if (attacker.GameMode == (int)GameModeStatics.GameModes.PvP)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attackerLocation &&
                    (p.GameMode == (int)GameModeStatics.GameModes.PvP || p.BotId < AIStatics.RerolledPlayerBotId) &&
                    p.Mobility == PvPStatics.MobilityFull &&
                     p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }
            else if (attacker.GameMode == (int)GameModeStatics.GameModes.Protection || attacker.GameMode == (int)GameModeStatics.GameModes.Superprotection)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attackerLocation &&
                    p.BotId < AIStatics.RerolledPlayerBotId &&
                    p.Mobility == PvPStatics.MobilityFull &&
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }

            // filter out offline players as well as the attacker
            foreach (var p in playersHere)
            {
                if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != attacker.Id)
                {
                    playersHereOnline.Add(p);
                }
            }

            foreach (var p in playersHereOnline)
            {
                p.Health -= damage;
                if (p.Health < 0)
                {
                    p.Health = 0;
                }
                playerREpo.SavePlayer(p);
                var message = "<span class='playerAttackNotification'>" + victim.GetFullName() + " convulses and shakes before exploding into a roiling tide of chaotic energies damaging you for " + damage + " along with " + (playersHereOnline.Count - 1) + " others.</span>";
                PlayerLogProcedures.AddPlayerLog(p.Id, message, true);
            }

            var logMessage = victim.FirstName + " " + victim.LastName + " exploded into a violent shower of chaotic energies.";
            LocationLogProcedures.AddLocationLog(attackerLocation, logMessage);

            var attackerMessage = "The explosion caused by " + victim.FirstName + " " + victim.LastName + " scattered violent energies throughout  " + here + ", lowering " + playersHereOnline.Count + " people's willpower by " + damage + " each.";
            PlayerLogProcedures.AddPlayerLog(attacker.Id, attackerMessage, false);

            // set the player's last action flag
            var dbAttacker = playerREpo.Players.First(p => p.Id == attacker.Id);
            dbAttacker.LastActionTimestamp = DateTime.UtcNow;
            playerREpo.SavePlayer(dbAttacker);

            return attackerMessage;
        }

        public static void InstantTakeoverLocation(Covenant cov, string location)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            var info = repo.LocationInfos.FirstOrDefault(l => l.dbName == location);
            if (info == null)
            {
                info = new LocationInfo
                {
                    dbName = location,

                };
            }
            info.TakeoverAmount = 100;
            info.CovenantId = cov.Id;
            info.LastTakeoverTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            repo.SaveLocationInfo(info);

            LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == location).CovenantController = cov.Id;

        }

        public static void LoadCovenantOwnersIntoRAM()
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            var info = repo.LocationInfos.ToList();
            foreach (var loc in LocationsStatics.LocationList.GetLocation)
            {
                var locName = loc.dbName;
                var temp = info.FirstOrDefault(l => l.dbName == locName);
                if (temp == null)
                {
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locName).CovenantController = null;
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locName).TakeoverAmount = 0;
                }
                else
                {
                    if (temp.CovenantId != null)
                    {
                        LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locName).CovenantController = (int)temp.CovenantId;
                    }
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locName).TakeoverAmount = temp.TakeoverAmount;

                }
            }
        }

        private static string GetPronoun_hisher(string sex)
        {
            return sex == PvPStatics.GenderMale ? "his" : "her";
        }

        private static string GetPronoun_HisHer(string sex)
        {
            return sex == PvPStatics.GenderMale ? "His" : "Her";
        }

    }
}