using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;
using TT.Domain.TFEnergies.Commands;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class TFEnergyProcedures
    {
        public static LogBox AddTFEnergyToPlayer(Player victim, Player attacker, SkillViewModel skill, decimal modifier)
        {
            var output = new LogBox();
            output.AttackerLog = "  ";
            output.VictimLog = "  ";

            // Move old energy to pool, add pool energy to the current attack
            decimal totalEnergy = AccumulateTFEnergy(victim, attacker, skill, modifier);

            var eventualForm = FormStatics.GetForm(skill.StaticSkill.FormSourceId.Value);

            var energyAmount = $"  [{totalEnergy:0.00} / {eventualForm.TFEnergyRequired:0.00} TF energy]<br />";
            output.AttackerLog += energyAmount;
            output.VictimLog += energyAmount;

            // Return early if victim is already in the target form
            if (victim.FormSourceId == eventualForm.Id)
            {
                var noTransformMessage = $"Since {victim.GetFullName()} is already in this form, the spell has no transforming effect.";
                output.AttackerLog += noTransformMessage;
                output.VictimLog += noTransformMessage;
                return output;
            }

            // Add the spell text to the player logs
            var percentTransformed = CalculateTransformationProgress(victim, totalEnergy, eventualForm);
            var percentPrintedOutput = Math.Round(percentTransformed * 100, 3);
            LogTransformationMessages(victim, attacker, output, eventualForm, percentTransformed, percentPrintedOutput);

            // Only give XP if attacker and victim are not part of the same covenant
            if (attacker.Covenant == null || attacker.Covenant != victim.Covenant)
            {
                AwardXP(victim, attacker, output, eventualForm);
            }

            return output;
        }

        private static decimal AccumulateTFEnergy(Player victim, Player attacker, SkillViewModel skill, decimal modifier)
        {
            ITFEnergyRepository repo = new EFTFEnergyRepository();

            // assert modifier is never negative (reduced TF damage instead of adding it)
            if (modifier < 0)
            {
                modifier = 0;
            }

            // crunch down any old TF Energies into one public energy
            var energiesOnPlayer = repo.TFEnergies.Where(e => e.PlayerId == victim.Id && e.FormSourceId == skill.StaticSkill.FormSourceId).ToList();

            var energiesEligibleForDelete = new List<Models.TFEnergy>();
            decimal mergeUpEnergyAmt = 0;
            decimal sharedEnergyAmt = 0;

            foreach (var e in energiesOnPlayer)
            {

                var minutesAgo = Math.Abs(Math.Floor(e.Timestamp.Subtract(DateTime.UtcNow).TotalMinutes));

                if (e.CasterId == null)
                {
                    sharedEnergyAmt += e.Amount;
                }
                else if (minutesAgo > 180)
                {
                    mergeUpEnergyAmt += e.Amount;
                    energiesEligibleForDelete.Add(e);
                }
            }

            // if the amount of old energies is greater than 0, write up a new one and save it as 'public domain' TF Energy
            if (mergeUpEnergyAmt > 0)
            {
                var publicEnergy = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormSourceId == skill.StaticSkill.FormSourceId && e.CasterId == null);

                if (publicEnergy == null)
                {
                    var cmd = new CreateTFEnergy
                    {
                        PlayerId = victim.Id,
                        Amount = mergeUpEnergyAmt,
                        CasterId = null,
                        FormSourceId = skill.StaticSkill.FormSourceId
                    };

                    DomainRegistry.Repository.Execute(cmd);
                }
                else
                {
                    publicEnergy.Amount += mergeUpEnergyAmt;
                    repo.SaveTFEnergy(publicEnergy);
                }

                foreach (var e in energiesEligibleForDelete)
                {
                    repo.DeleteTFEnergy(e.Id);
                }

            }

            // get the amount of TF Energy the attacker has on the player
            var energyFromMe = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormSourceId == skill.StaticSkill.FormSourceId && e.CasterId == attacker.Id);

            if (energyFromMe == null)
            {

                var cmd = new CreateTFEnergy
                {
                    PlayerId = victim.Id,
                    Amount = skill.StaticSkill.TFPointsAmount * modifier,
                    CasterId = attacker.Id,
                    FormSourceId = skill.StaticSkill.FormSourceId
                };

                DomainRegistry.Repository.Execute(cmd);

                // create an old entity just so it doesn't break functionality below
                energyFromMe = new Models.TFEnergy
                {
                    PlayerId = victim.Id,
                    FormSourceId = skill.StaticSkill.FormSourceId.Value,
                    CasterId = attacker.Id,
                    Amount = skill.StaticSkill.TFPointsAmount * modifier
                };

            }
            else
            {
                energyFromMe.Amount += skill.StaticSkill.TFPointsAmount * modifier;
                energyFromMe.Timestamp = DateTime.UtcNow;
                repo.SaveTFEnergy(energyFromMe);
            }

            return  energyFromMe.Amount + sharedEnergyAmt + mergeUpEnergyAmt;
        }

        private static decimal CalculateTransformationProgress(Player victim, decimal totalEnergy, DbStaticForm eventualForm)
        {
            var percentTransformedByHealth = 1 - (victim.Health / victim.MaxHealth);

            // animate forms only need half of health requirement, so double the amount completed
            decimal PercentHealthToAllowTF = 0;
            var mobility = eventualForm.MobilityType;
            if (mobility == PvPStatics.MobilityFull)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowFullMobilityFormTF;
            }
            else if (mobility == PvPStatics.MobilityInanimate)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowInanimateFormTF;
            }
            else if (mobility == PvPStatics.MobilityPet)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowAnimalFormTF;
            }
            else if (mobility == PvPStatics.MobilityMindControl)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowMindControlTF;
            }

            percentTransformedByHealth /= 1 - PercentHealthToAllowTF;

            var percentTransformedByEnergy = Math.Min(1, totalEnergy / eventualForm.TFEnergyRequired);

            // only return the lower of the two tf % values
            return Math.Min(percentTransformedByEnergy, percentTransformedByHealth);
        }

        private static void LogTransformationMessages(Player victim, Player attacker, LogBox output, DbStaticForm eventualForm, decimal percentTransformed, decimal percentPrintedOutput)
        {
            if (percentTransformed < 0.20m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "20", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "20", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.40m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "40", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "40", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.60m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "60", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "60", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.80m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "80", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "80", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 1)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "100", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "100", "first") + " (" + percentPrintedOutput + "%)";
            }
            else
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "complete", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "complete", "first") + " (" + percentPrintedOutput + "%)";
            }
        }

        private static void AwardXP(Player victim, Player attacker, LogBox output, DbStaticForm eventualForm)
        {
            // calculate the xp earned for this transformation
            var xpEarned = PvPStatics.XP__GainPerAttackBase - (attacker.Level - victim.Level) * PvPStatics.XP__LevelDifferenceXPGainModifier;

            if (xpEarned < 1)
            {
                xpEarned = 1;
            }
            if (xpEarned > 5)
            {
                xpEarned = 5;
            }

            // decrease the XP earned if the player is high leveled and TFing an animate spell AND the xp isn't already negative
            if (eventualForm.MobilityType == PvPStatics.MobilityFull && xpEarned > 0)
            {
                xpEarned *= GetHigherLevelXPModifier(attacker.Level, 4);
            }

            // decrease the XP earned if the player is high leveled and TFing an inanimate / animal spell AND the xp isn't already negative
            if (eventualForm.MobilityType == PvPStatics.MobilityInanimate || eventualForm.MobilityType == PvPStatics.MobilityPet || eventualForm.MobilityType == PvPStatics.MobilityMindControl)
            {
                xpEarned *= GetHigherLevelXPModifier(attacker.Level, 6);
            }

            // give XP to the attacker
            output.AttackerLog += " (+" + xpEarned + " XP)";
            output.ResultMessage += " (+" + xpEarned + " XP)";

            var lvlMessage = PlayerProcedures.GiveXP(attacker, xpEarned);
            output.AttackerLog += lvlMessage;
            output.ResultMessage += lvlMessage;
        }

        public static LogBox RunFormChangeLogic(Player victim, int skillSourceId, int attackerId)
        {

            var output = new LogBox();

            // redundant check to make sure the victim is still in a transformable state
            if (victim.Mobility != PvPStatics.MobilityFull)
            {
                return output;
            }

            var skill = SkillStatics.GetStaticSkill(skillSourceId);
            var energyAccumulated = CalculateAccumulatedTFEnergy(victim, attackerId, skill);

            var targetForm = FormStatics.GetForm(skill.FormSourceId.Value);

            // check and see if the target has enough points accumulated to try the form's energy requirement
            if (energyAccumulated < targetForm.TFEnergyRequired)
            {
                // Less than 100% TFE - don't complete form change
                return output;
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();

            // check and see if the target's health is low enough to be eligible for the TF
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);
            var attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);

            // Collect the attacker & victim TFE related buffs
            decimal modifiedTFEnergyPercent = CalculateTFEBonusBuff(victim, attacker);

            AddExtraHealthDamage(victim, output, playerRepo, targetForm, energyAccumulated, target, attacker, modifiedTFEnergyPercent);

            target = CompleteTransformation(victim, output, playerRepo, targetForm, target, attacker);

            // if there is a duel going on, end it if all but 1 player is defeated (not in the form they started in)
            if (victim.InDuel > 0)
            {
                EndDuel(victim);
            }

            return output;
        }

        private static decimal CalculateAccumulatedTFEnergy(Player victim, int attackerId, DbStaticSkill skill)
        {
            ITFEnergyRepository repo = new EFTFEnergyRepository();

            var pooledEnergy = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormSourceId == skill.FormSourceId && e.CasterId == null);
            var myEnergy = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormSourceId == skill.FormSourceId && e.CasterId == attackerId);

            decimal energyAccumulated = 0;
            if (pooledEnergy != null)
            {
                energyAccumulated = pooledEnergy.Amount + myEnergy.Amount;
            }
            else
            {
                energyAccumulated = myEnergy.Amount;
            }

            return energyAccumulated;
        }

        private static decimal CalculateTFEBonusBuff(Player victim, Player attacker)
        {
            var attackerTFEnergyBonus = ItemProcedures.GetPlayerBuffs(attacker).SpellExtraTFEnergyPercent();
            var victimTFEnergyReduction = ItemProcedures.GetPlayerBuffs(victim).SpellTFEnergyDamageResistance();
            // Collect the attacker ExtraTFEnergyPercent and modify it by the defenders TFEnergyDamageResistance
            var modifiedTFEnergyPercent = 1 + ((attackerTFEnergyBonus - victimTFEnergyReduction) / 100M);

            // Cap the damage modifier at 0.5 / 2.0 
            if (modifiedTFEnergyPercent < 0.5M)
            {
                modifiedTFEnergyPercent = 0.5M;
            }
            if (modifiedTFEnergyPercent > 2.0M)
            {
                modifiedTFEnergyPercent = 2.0M;
            }

            return modifiedTFEnergyPercent;
        }

        private static void AddExtraHealthDamage(Player victim, LogBox output, IPlayerRepository playerRepo, DbStaticForm targetForm, decimal energyAccumulated, Player target, Player attacker, decimal modifiedTFEnergyPercent)
        {
            // add in some extra WP damage if TF energy high enough for TF but WP is still high
            // Explode the victim if they somehow reach 1,000 TFEnergy in a PvP encounter
            if (target.Health > 0)
            {
                if (energyAccumulated > targetForm.TFEnergyRequired * 10M && attacker.BotId == AIStatics.ActivePlayerBotId && victim.BotId == AIStatics.ActivePlayerBotId)
                {
                    var HealthDamage = Math.Round(9999 * modifiedTFEnergyPercent, 2);

                    output.VictimLog += " <br />Despite your iron will there is only so much transformation energy that a single person can contain within their body, unfortunately for you that limit has been reached. In a spectacular shower of vibrant energy you are consumed. You take an extra " + HealthDamage + " willpower damage.";
                    output.AttackerLog += "<br />Your victim stands resolute for their final moments before a brilliant cascade of chaos errupts from inside of their form. They suffer an extra " + HealthDamage + " willpower damage.";
                    target.Health -= HealthDamage;
                    target.NormalizeHealthMana();
                    playerRepo.SavePlayer(target);

                    // Causes the victim to 'explode' and deal damage in their area
                    AttackProcedures.SuddenDeathExplosion(attacker, victim, 240);
                }
                else if (energyAccumulated > targetForm.TFEnergyRequired * 3M)
                {
                    var HealthDamage = Math.Round(60 * modifiedTFEnergyPercent, 2);
                    // Make players deal full damage with TFEnergy buildup
                    if (attacker.BotId == AIStatics.ActivePlayerBotId)
                    {
                        HealthDamage = Math.Round(120 * modifiedTFEnergyPercent, 2);
                    }

                    output.VictimLog += "<br />You collapse to your knees and your vision wavers as transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra " + HealthDamage + " willpower damage.";
                    output.AttackerLog += "<br />Your victim has an extremely high amount of transformation energy built up and takes an extra " + HealthDamage + " willpower damage.";
                    target.Health -= HealthDamage;
                    target.NormalizeHealthMana();
                    playerRepo.SavePlayer(target);
                }
                else if (energyAccumulated > targetForm.TFEnergyRequired * 2M)
                {
                    var HealthDamage = Math.Round(30 * modifiedTFEnergyPercent, 2);
                    // Make players deal full damage with TFEnergy buildup
                    if (attacker.BotId == AIStatics.ActivePlayerBotId)
                    {
                        HealthDamage = Math.Round(60 * modifiedTFEnergyPercent, 2);
                    }
                    output.VictimLog += "<br />You body spasms as the surplus of transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra " + HealthDamage + " willpower damage.";
                    output.AttackerLog += "<br />Your victim has an extremely high amount of transformation energy built up and takes an extra " + HealthDamage + " willpower damage.";
                    target.Health -= HealthDamage;
                    target.NormalizeHealthMana();
                    playerRepo.SavePlayer(target);
                }
                else if (energyAccumulated > targetForm.TFEnergyRequired * 1M)
                {
                    var HealthDamage = Math.Round(15 * modifiedTFEnergyPercent, 2);
                    // Make players deal full damage with TFEnergy buildup
                    if (attacker.BotId == AIStatics.ActivePlayerBotId)
                    {
                        HealthDamage = Math.Round(30 * modifiedTFEnergyPercent, 2);
                    }
                    output.VictimLog += "<br />You gasp as your body shivers with a surplus of transformation energy built up within it, leaving you distracted and your willpower increasingly impaired. You take an extra " + HealthDamage + " willpower damage.";
                    output.AttackerLog += "<br />Your victim has a high amount of transformation energy built up and takes an extra " + HealthDamage + " willpower damage.";
                    target.Health -= HealthDamage;
                    target.NormalizeHealthMana();
                    playerRepo.SavePlayer(target);
                }
            }
        }

        private static Player CompleteTransformation(Player victim, LogBox output, IPlayerRepository playerRepo, DbStaticForm targetForm, Player target, Player attacker)
        {
            var healthProportion = target.Health / target.MaxHealth;

            // target is turning into an animate form
            if (targetForm.MobilityType == PvPStatics.MobilityFull && healthProportion <= PvPStatics.PercentHealthToAllowFullMobilityFormTF)
            {
                target = PerformAnimateTransformation(output, playerRepo, targetForm, target, attacker);
            }

            // target is turning into an inanimate or animal form, both are endgame
            else if ((targetForm.MobilityType == PvPStatics.MobilityInanimate && healthProportion <= PvPStatics.PercentHealthToAllowInanimateFormTF) ||
                     (targetForm.MobilityType == PvPStatics.MobilityPet && healthProportion <= PvPStatics.PercentHealthToAllowAnimalFormTF))
            {
                PerformInanimateTransformation(victim, output, playerRepo, targetForm, target, attacker);
            }

            // mind control
            else if (targetForm.MobilityType == PvPStatics.MobilityMindControl && healthProportion <= PvPStatics.PercentHealthToAllowMindControlTF)
            {
                EngageMindControl(victim, output, targetForm, target, attacker);
            }

            return target;
        }

        private static Player PerformAnimateTransformation(LogBox output, IPlayerRepository playerRepo, DbStaticForm targetForm, Player target, Player attacker)
        {
            SkillProcedures.UpdateFormSpecificSkillsToPlayer(target, targetForm.Id);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = target.Id,
                FormSourceId = targetForm.Id
            });

            // wipe out half of the target's mana
            target.Mana -= target.MaxMana / 2;
            if (target.Mana < 0)
            {
                target.Mana = 0;
            }

            // Remove any Self Restore entires.
            RemoveSelfRestore(target);

            var targetbuffs = ItemProcedures.GetPlayerBuffs(target);
            target = PlayerProcedures.ReadjustMaxes(target, targetbuffs);


            // take away some of the victim's XP based on the their level
            // target.XP += -2.5M * target.Level;

            playerRepo.SavePlayer(target);

            output.LocationLog += "<br><b>" + target.GetFullName() + " was completely transformed into a " + targetForm.FriendlyName + " here.</b>";
            output.AttackerLog += "<br><b>You fully transformed " + target.GetFullName() + " into a " + targetForm.FriendlyName + "</b>!";
            output.VictimLog += "<br><b>You have been fully transformed into a " + targetForm.FriendlyName + "!</b>";

            TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfFormSourceId(target.Id, targetForm.Id);

            StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesAnimateTFed, 1);

            StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimateTFing, 1);
            return target;
        }

        private static void PerformInanimateTransformation(Player victim, LogBox output, IPlayerRepository playerRepo, DbStaticForm targetForm, Player target, Player attacker)
        {
            SkillProcedures.UpdateFormSpecificSkillsToPlayer(target, targetForm.Id);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = target.Id,
                FormSourceId = targetForm.Id
            });

            if (targetForm.MobilityType == PvPStatics.MobilityInanimate)
            {
                StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesInanimateTFed, 1);

                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesInanimateTFing, 1);


            }
            else if (targetForm.MobilityType == PvPStatics.MobilityPet)
            {
                StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesAnimalTFed, 1);

                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimalTFing, 1);

            }

            if (targetForm.MobilityType == PvPStatics.MobilityPet || targetForm.MobilityType == PvPStatics.MobilityInanimate)
            {

                if (target.BotId == AIStatics.PsychopathBotId)
                {
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PsychopathsDefeated,
                        1);
                }

                if (target.BotId == AIStatics.ActivePlayerBotId && attacker.GameMode == (int)GameModeStatics.GameModes.PvP && victim.GameMode == (int)GameModeStatics.GameModes.PvP)
                {
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PvPPlayerNumberTakedowns, 1);
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PvPPlayerLevelTakedowns, victim.Level);
                }

            }

            // extra log stuff for turning into item
            var extra = ItemProcedures.PlayerBecomesItem(target, targetForm, attacker);
            output.AttackerLog += extra.AttackerLog;
            output.VictimLog += extra.VictimLog;
            output.LocationLog += extra.LocationLog;

            // give some of the victim's money to the attacker, the amount depending on what mode the victim is in
            var moneygain = victim.Money * .35M;
            PlayerProcedures.GiveMoneyToPlayer(attacker, moneygain);
            PlayerProcedures.GiveMoneyToPlayer(victim, -moneygain / 2);

            var levelDifference = attacker.Level - target.Level;

            // only give the lump sum XP if the victim is not in the same covenant
            if (attacker.Covenant == null || attacker.Covenant != target.Covenant)
            {

                var xpGain = 100 - (PvPStatics.XP__EndgameTFCompletionLevelBase * levelDifference);

                if (xpGain < 50)
                {
                    xpGain = 50;
                }
                else if (xpGain > 200)
                {
                    xpGain = 200;
                }

                // give the attacker a nice lump sum for having completed the transformation
                output.AttackerLog += "  <br>For having sealed your opponent into their new form, you gain an extra <b>" + xpGain + "</b> XP.";
                output.AttackerLog += PlayerProcedures.GiveXP(attacker, xpGain);
            }

            // exclude PvP score for bots
            if (victim.BotId == AIStatics.ActivePlayerBotId)
            {
                var score = PlayerProcedures.GetPvPScoreFromWin(attacker, victim);

                if (score > 0)
                {

                    output.AttackerLog += PlayerProcedures.GivePlayerPvPScore(attacker, victim, score);
                    output.VictimLog += PlayerProcedures.RemovePlayerPvPScore(victim, attacker, score);

                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__DungeonPointsStolen, (float)score);

                }
                else
                {
                    output.AttackerLog += "  " + victim.GetFullName() + " unfortunately did not have any dungeon points for you to steal for yourself.";
                }
            }

            // Heals the victorious player provided that the target was eligible
            if (attacker.BotId == AIStatics.ActivePlayerBotId)
            {
                // Provide no healing if the victim shared a coven with the attacker
                if (attacker.Covenant != null && attacker.Covenant == victim.Covenant)
                {
                    output.AttackerLog += "  <br>There is no glory to be had in this victory, your willpower & mana are not restored.";
                }
                else
                {
                    // Figure out the modifier to be used
                    double modifier = (levelDifference * 5) / 100;
                    // Cap the modifier to prevent too much / too little healing.
                    if (modifier > 0.3)
                    {
                        modifier = 0.3;
                    }
                    if (modifier < -0.55)
                    {
                        modifier = -0.55;
                    }
                    decimal healingPercent = (decimal)(0.6 + modifier);

                    if (victim.BotId != AIStatics.ActivePlayerBotId)
                    {
                        // The victim is not a player, provide half of the healing.
                        healingPercent = healingPercent / 2;
                    }
                    // Calculate the final amount of health to provide
                    var healingTotal = attacker.MaxHealth * healingPercent;
                    // Cap the healing to prevent over healing
                    if (attacker.Health + healingTotal > attacker.MaxHealth)
                    {
                        healingTotal = (attacker.MaxHealth - attacker.Health);
                    }
                    // Calculate the final amount of mana to provide
                    var manaRestoredTotal = attacker.MaxMana * healingPercent;
                    // Cap the mana restoration to prevent over healing
                    if (attacker.Mana + manaRestoredTotal > attacker.MaxMana)
                    {
                        manaRestoredTotal = (attacker.MaxMana - attacker.Mana);
                    }

                    // Heal the attacker
                    attacker.Health += healingTotal;
                    // Restore the attackers Mana
                    attacker.Mana += manaRestoredTotal;

                    // Remove any Self Restore entires.
                    RemoveSelfRestore(target);

                    playerRepo.SavePlayer(target);

                    output.AttackerLog += "<br />Invigorated by your victory and fuelled by the scattered essence that was once your foe, you are healed for " + healingTotal + " willpower and " + manaRestoredTotal + " mana.";
                }
            }

            output.AttackerLog += "  You collect " + Math.Round(moneygain, 0) + " Arpeyjis your victim dropped during the transformation.";

            // create inanimate XP for the victim
            InanimateXPProcedures.GetStruggleChance(victim);

            // if this victim is a bot, clear out some old stuff that is not needed anymore
            if (victim.BotId < AIStatics.ActivePlayerBotId)
            {
                AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(victim.Id);
                PlayerLogProcedures.ClearPlayerLog(victim.Id);
            }

            TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfFormSourceId(target.Id, targetForm.Id);

            // if the attacker is a psycho, have them change to a new spell and equip whatever they just earned
            if (attacker.BotId == AIStatics.PsychopathBotId)
            {
                SkillProcedures.DeleteAllPlayerSkills(attacker.Id);

                // give this bot a random skill
                var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
                var rand = new Random();
                double max = eligibleSkills.Count();
                var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                var skillToLearn = eligibleSkills.ElementAt(randIndex);
                SkillProcedures.GiveSkillToPlayer(attacker.Id, skillToLearn.Id);

                // have the psycho equip any items they are carrying (if they have any duplicates in a slot, they'll take them off later in world update)
                var psychoItems = ItemProcedures.GetAllPlayerItems(attacker.Id)
                     .Where(item => item.Item.ItemType != PvPStatics.ItemType_Rune);

                foreach (var i in psychoItems)
                {
                    ItemProcedures.EquipItem(i.dbItem.Id, true);
                }

            }
        }

        private static void EngageMindControl(Player victim, LogBox output, DbStaticForm targetForm, Player target, Player attacker)
        {
            //Player attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);
            MindControlProcedures.AddMindControl(attacker, victim, targetForm.Id);

            output.LocationLog += "<br><b>" + target.GetFullName() + " was partially mind controlled by " + attacker.GetFullName() + " here.</b>";
            output.AttackerLog += "<br><b>You have seized the mind of " + target.GetFullName() + "!  You can now force them into performing certain actions.</b>";
            output.VictimLog += "<br><b>You are now being partially mind controlled by " + targetForm.FriendlyName + "!</b>";

            TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfFormSourceId(target.Id, targetForm.Id);
            // Remove any Self Restore entires.
            RemoveSelfRestore(target);

            // give curse debuff
            if (targetForm.Id == MindControlStatics.MindControl__MovementFormSourceId)
            {
                EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Movement_DebuffEffectSourceId, target);
            }
            else if (targetForm.Id == MindControlStatics.MindControl__StripFormSourceId)
            {
                EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Strip_DebuffEffectSourceId, target);
            }
            else if (targetForm.Id == MindControlStatics.MindControl__MeditateFormSourceId)
            {
                EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Strip_DebuffEffectSourceId, target);
            }
        }

        private static void EndDuel(Player victim)
        {
            var duel = DuelProcedures.GetDuel(victim.InDuel);
            var duelParticipants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);

            var remainders = duelParticipants.Count();

            foreach (var p in duelParticipants)
            {
                if (p.Player.FormSourceId != duel.Combatants.FirstOrDefault(dp => dp.PlayerId == p.Player.Id).StartFormSourceId)
                {
                    remainders--;
                }
            }

            if (remainders <= 1)
            {
                DuelProcedures.EndDuel(duel.Id, DuelProcedures.FINISHED);
            }
        }

        public static void RemoveSelfRestore(Player player)
        {
            ISelfRestoreEnergies repo = new EFSelfRestoreEnergies();
            IEnumerable<Models.SelfRestoreEnergies> CleanseSelfRestore = repo.SelfRestoreEnergies.Where(q => q.OwnerId == player.Id).ToList();

            foreach (var t in CleanseSelfRestore)
            {
                repo.DeleteSelfRestoreEnergies(t.Id);
            }

        }

        public static void CleanseTFEnergies(Player player, decimal bonusPercentageFromBuffs)
        {
            ITFEnergyRepository repo = new EFTFEnergyRepository();
            IEnumerable<Models.TFEnergy> mydbEnergies = repo.TFEnergies.Where(e => e.PlayerId == player.Id).ToList();

            foreach (var energy in mydbEnergies)
            {
                energy.Amount -= bonusPercentageFromBuffs;
                if (energy.Amount > 0)
                {
                    repo.SaveTFEnergy(energy);
                }
                else
                {
                    repo.DeleteTFEnergy(energy.Id);
                }
            }

        }

        public static void DeleteAllPlayerTFEnergies(int playerId)
        {
            ITFEnergyRepository tfEnergyRepo = new EFTFEnergyRepository();
            IEnumerable<Models.TFEnergy> energiesToDelete = tfEnergyRepo.TFEnergies.Where(s => s.PlayerId == playerId).ToList();

            foreach (var s in energiesToDelete)
            {
                tfEnergyRepo.DeleteTFEnergy(s.Id);
            }
        }

        public static void DeleteAllPlayerTFEnergiesOfFormSourceId(int playerId, int formSourceId)
        {
            ITFEnergyRepository tfEnergyRepo = new EFTFEnergyRepository();
            IEnumerable<Models.TFEnergy> energiesToDelete = tfEnergyRepo.TFEnergies.Where(s => s.PlayerId == playerId && s.FormSourceId == formSourceId).ToList();

            foreach (var s in energiesToDelete)
            {
                tfEnergyRepo.DeleteTFEnergy(s.Id);
            }
        }

        private static string CleanString(string input, Player victim, Player attacker)
        {
            if (input == null)
            {
                return null;
            }
            else
            {
                return input.Trim().Replace(Environment.NewLine, "</br>").Replace("$VICTIM_NAME$", victim.GetFullName()).Replace("$ATTACKER_NAME$", attacker.GetFullName());
            }
        }

        private static string GetTFMessage(DbStaticForm form, Player player, Player attacker, string percent, string PoV)
        {

            ITFMessageRepository tfMessageRepo = new EFTFMessageRepository();
            var tfMessage = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormSourceId == form.Id);

            if (tfMessage == null)
            {
                return "ERROR RETRIEVING TRANSFORMATION TEXT.  This is a bug.";
            }

            #region 20 percent TF
            if (percent == "20")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_20_Percent_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_20_Percent_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_3rd, player, attacker);
                        }
                    }
                } 
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_20_Percent_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_20_Percent_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_20_Percent_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion

            #region 40 percent TF
            if (percent == "40")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_40_Percent_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_40_Percent_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_3rd, player, attacker);
                        }
                    }
                }
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_40_Percent_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_40_Percent_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_40_Percent_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion

            #region 60 percent TF
            if (percent == "60")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_60_Percent_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_60_Percent_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_3rd, player, attacker);
                        }
                    }
                }
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_60_Percent_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_60_Percent_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_60_Percent_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion

            #region 80 percent TF
            if (percent == "80")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_80_Percent_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_80_Percent_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_3rd, player, attacker);
                        }
                    }
                }
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_80_Percent_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_80_Percent_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_80_Percent_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion

            #region 100 percent TF
            if (percent == "100")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_100_Percent_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_100_Percent_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_3rd, player, attacker);
                        }
                    }
                }
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_100_Percent_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_100_Percent_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_100_Percent_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion

            #region 100 percent TF
            if (percent == "complete")
            {
                if (player.Gender == PvPStatics.GenderMale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_Completed_1st_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_Completed_1st_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_Completed_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_Completed_3rd_M.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_Completed_3rd_M, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_Completed_3rd, player, attacker);
                        }
                    }
                }
                else if (player.Gender == PvPStatics.GenderFemale)
                {
                    if (PoV == "first")
                    {
                        if (!tfMessage.TFMessage_Completed_1st_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_Completed_1st_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_Completed_1st, player, attacker);
                        }

                    }
                    else if (PoV == "third")
                    {
                        if (!tfMessage.TFMessage_Completed_3rd_F.IsNullOrEmpty())
                        {
                            return CleanString(tfMessage.TFMessage_Completed_3rd_F, player, attacker);
                        }
                        else
                        {
                            return CleanString(tfMessage.TFMessage_Completed_3rd, player, attacker);
                        }
                    }
                }
            }
            #endregion


            return "";
        }

        public static decimal GetHigherLevelXPModifier(int lvl, int maxLvlBeforeLoss)
        {
            var modifier = 1.0M;

            modifier = 1 - ((lvl - maxLvlBeforeLoss) * .1M);

            if (modifier > 1)
            {
                modifier = 1;
            }
            else if (modifier < .1M)
            {
                modifier = .1M;
            }

            return modifier;
        }

    }
}