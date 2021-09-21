using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;
using TT.Domain.TFEnergies.Commands;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class TFEnergyProcedures
    {
        public static LogBox AddTFEnergyToPlayer(Player victim, Player attacker, SkillViewModel skill, decimal modifier, decimal victimHealthBefore)
        {
            var output = new LogBox
            {
                AttackerLog = "  ",
                VictimLog = "  "
            };

            var victimMaxHealth = victim.MaxHealth;  // Remember this before any form change can affect it

            // Move old energy to pool, add pool energy to the current attack
            (var energyBefore, var energyAfter) = AccumulateTFEnergy(victim, attacker, skill, modifier);

            var eventualForm = FormStatics.GetForm(skill.StaticSkill.FormSourceId.Value);

            var energyAmount = $"  [{energyAfter:0.00} / {eventualForm.TFEnergyRequired:0.00} TF energy]<br />";
            output.AttackerLog += energyAmount;
            output.VictimLog += energyAmount;

            // Return early if victim is already in the target form
            if (victim.FormSourceId == eventualForm.Id && victim.Mobility == eventualForm.MobilityType)
            {
                var noTransformMessage = $"Since {victim.GetFullName()} is already in this form, the spell has no transforming effect.";
                output.AttackerLog += noTransformMessage;
                output.VictimLog += noTransformMessage;
                return output;
            }

            // Perform any form change logic (inc extra health damage)
            var formChangeLog = new LogBox();
            var victimHealthAfter = TFEnergyProcedures.RunFormChangeLogic(victim, attacker.Id, skill, energyAfter, formChangeLog);

            // Add the spell text to the player logs (now that we know the full damage of this hit)
            var percentTransformedBefore = CalculateTransformationProgress(victimHealthBefore, victimMaxHealth, energyBefore, eventualForm);
            var percentTransformedAfter = CalculateTransformationProgress(victimHealthAfter, victimMaxHealth, energyAfter, eventualForm);
            LogTransformationMessages(victim, attacker, eventualForm, percentTransformedBefore, percentTransformedAfter, output);

            // Append energy overflow damage and form change output from earlier
            output.Add(formChangeLog);

            // Only give XP if attacker and victim are not part of the same covenant
            if (attacker.Covenant == null || attacker.Covenant != victim.Covenant)
            {
                AwardXP(victim, attacker, eventualForm.MobilityType, output);
            }

            return output;
        }

        private static (decimal, decimal) AccumulateTFEnergy(Player victim, Player attacker, SkillViewModel skill, decimal modifier)
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
            var tfeGain = skill.StaticSkill.TFPointsAmount * modifier;

            if (energyFromMe == null)
            {

                var cmd = new CreateTFEnergy
                {
                    PlayerId = victim.Id,
                    Amount = tfeGain,
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
                    Amount = tfeGain
                };

            }
            else
            {
                energyFromMe.Amount += tfeGain;
                energyFromMe.Timestamp = DateTime.UtcNow;
                repo.SaveTFEnergy(energyFromMe);
            }

            var totalAfter = energyFromMe.Amount + sharedEnergyAmt + mergeUpEnergyAmt;
            var totalBefore = totalAfter - tfeGain;

            return (totalBefore, totalAfter);
        }

        private static decimal CalculateTransformationProgress(decimal victimHealth, decimal victimMaxHealth, decimal totalEnergy, DbStaticForm eventualForm)
        {
            var percentTransformedByHealth = 1 - (victimHealth / victimMaxHealth);

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

        private static void LogTransformationMessages(Player victim, Player attacker, DbStaticForm eventualForm, decimal percentTransformedBefore, decimal percentTransformedAfter, LogBox output)
        {
            // Stage 0 = 0 - 20%, 1 = 20 - 40%, 2 = 40 - 60%, 3 = 60 - 80%, 4 = 80 - 100%, 5 = 100% complete, -1 = 0% (no TFE)
            var previousStage = (int)(percentTransformedBefore < 1.0M ? percentTransformedBefore / 0.2M : 5);
            var currentStage = (int)(percentTransformedAfter < 1.0M ? percentTransformedAfter / 0.2M : 5);

            if (percentTransformedBefore == 0)
            {
                // Ensure currentStage != previousStage for first hit
                previousStage = -1;
            }

            var percentPrintedOutput = $" ({percentTransformedAfter:0.00%})";
            output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "third", previousStage, currentStage) + percentPrintedOutput;
            output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "first", previousStage, currentStage) + percentPrintedOutput;
        }

        private static void AwardXP(Player victim, Player attacker, string mobilityType, LogBox output)
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
            if (mobilityType == PvPStatics.MobilityFull && xpEarned > 0)
            {
                xpEarned *= GetHigherLevelXPModifier(attacker.Level, 4);
            }

            // decrease the XP earned if the player is high leveled and TFing an inanimate / animal spell AND the xp isn't already negative
            if (mobilityType == PvPStatics.MobilityInanimate || mobilityType == PvPStatics.MobilityPet || mobilityType == PvPStatics.MobilityMindControl)
            {
                xpEarned *= GetHigherLevelXPModifier(attacker.Level, 6);
            }

            // give XP to the attacker
            output.AttackerLog += $" (+{xpEarned} XP)";
            output.ResultMessage += $" (+{xpEarned} XP)";

            var lvlMessage = PlayerProcedures.GiveXP(attacker, xpEarned);
            output.AttackerLog += lvlMessage;
            output.ResultMessage += lvlMessage;
        }

        public static decimal RunFormChangeLogic(Player victim, int attackerId, SkillViewModel skill, decimal energyAccumulated, LogBox output)
        {
            // redundant check to make sure the victim is still in a transformable state
            if (victim.Mobility != PvPStatics.MobilityFull)
            {
                return victim.Health;
            }

            var targetForm = FormStatics.GetForm(skill.StaticSkill.FormSourceId.Value);

            // check and see if the target has enough points accumulated to try the form's energy requirement
            if (energyAccumulated < targetForm.TFEnergyRequired)
            {
                // Less than 100% TFE - don't complete form change
                return victim.Health;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbAttacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);

            // Collect the attacker & victim TFE related buffs
            decimal modifiedTFEnergyPercent = CalculateTFEBonusBuff(victim, dbAttacker);

            // check and see if the target's health is low enough to be eligible for the TF
            var updatedHealth = AddExtraHealthDamage(victim, dbAttacker, targetForm.TFEnergyRequired, energyAccumulated, modifiedTFEnergyPercent, output);
            PotentiallyCompleteTransformation(victim, dbAttacker, targetForm, skill.StaticSkill.Id, updatedHealth, output);

            // if there is a duel going on, end it if all but 1 player is defeated (not in the form they started in)
            if (victim.InDuel > 0)
            {
                EndDuel(victim);
            }

            return updatedHealth;
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

        private static decimal AddExtraHealthDamage(Player victim, Player attacker, decimal energyRequired, decimal energyAccumulated, decimal modifiedTFEnergyPercent, LogBox output)
        {
            var playerRepo = new EFPlayerRepository();
            var dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

            // add in some extra WP damage if TF energy high enough for TF but WP is still high
            // Explode the victim if they somehow reach 1,000 TFEnergy in a PvP encounter
            if (dbVictim.Health > 0)
            {

                if (energyAccumulated > energyRequired * 10M && attacker.BotId == AIStatics.ActivePlayerBotId && victim.BotId == AIStatics.ActivePlayerBotId)
                {
                    var healthDamage = 9999 * modifiedTFEnergyPercent;

                    output.VictimLog += $" <br />Despite your iron will there is only so much transformation energy that a single person can contain within their body, unfortunately for you that limit has been reached. In a spectacular shower of vibrant energy you are consumed. You take an extra {healthDamage:#} willpower damage.";
                    output.AttackerLog += $"<br />Your victim stands resolute for their final moments before a brilliant cascade of chaos errupts from inside of their form. They suffer an extra {healthDamage:#} willpower damage.";
                    dbVictim.Health -= healthDamage;
                    dbVictim.NormalizeHealthMana();
                    playerRepo.SavePlayer(dbVictim);

                    // Causes the victim to 'explode' and deal damage in their area
                    AttackProcedures.SuddenDeathExplosion(attacker, victim, 240);
                }
                else
                {
                    var healthDamage = 0m;
                    if (energyAccumulated > energyRequired * 3M)
                    {
                        healthDamage = 60 * modifiedTFEnergyPercent;

                        if (attacker.BotId == AIStatics.ActivePlayerBotId)
                        {
                            healthDamage *= 2;
                        }

                        output.VictimLog += $"<br />You collapse to your knees and your vision wavers as transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra {healthDamage:#} willpower damage.";
                        output.AttackerLog += $"<br />Your victim has an extremely high amount of transformation energy built up and takes an extra {healthDamage:#} willpower damage.";
                    }
                    else if (energyAccumulated > energyRequired * 2M)
                    {
                        healthDamage = 30 * modifiedTFEnergyPercent;

                        if (attacker.BotId == AIStatics.ActivePlayerBotId)
                        {
                            healthDamage *= 2;
                        }

                        output.VictimLog += $"<br />You body spasms as the surplus of transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra {healthDamage:#} willpower damage.";
                        output.AttackerLog += $"<br />Your victim has a very high amount of transformation energy built up and takes an extra {healthDamage:#} willpower damage.";
                    }
                    else if (energyAccumulated > energyRequired * 1M)
                    {
                        healthDamage = 15 * modifiedTFEnergyPercent;

                        if (attacker.BotId == AIStatics.ActivePlayerBotId)
                        {
                            healthDamage *= 2;
                        }

                        output.VictimLog += $"<br />You gasp as your body shivers with a surplus of transformation energy built up within it, leaving you distracted and your willpower increasingly impaired. You take an extra {healthDamage:#} willpower damage.";
                        output.AttackerLog += $"<br />Your victim has a high amount of transformation energy built up and takes an extra {healthDamage:#} willpower damage.";
                    }

                    if (healthDamage > 0)
                    {
                        dbVictim.Health -= healthDamage;
                        dbVictim.NormalizeHealthMana();
                        playerRepo.SavePlayer(dbVictim);
                    }
                }
            }

            return dbVictim.Health;
        }

        private static void PotentiallyCompleteTransformation(Player victim, Player attacker, DbStaticForm targetForm, int skillSourceId, decimal victimHealth, LogBox output)
        {
            var healthProportion = victimHealth / victim.MaxHealth;

            // target is turning into an animate form
            if (targetForm.MobilityType == PvPStatics.MobilityFull && healthProportion <= PvPStatics.PercentHealthToAllowFullMobilityFormTF)
            {
                PerformAnimateTransformation(victim, attacker, targetForm, output);
                BountyProcedures.ClaimReward(attacker, victim, targetForm);
            }

            // target is turning into an inanimate or animal form, both are endgame
            else if ((targetForm.MobilityType == PvPStatics.MobilityInanimate && healthProportion <= PvPStatics.PercentHealthToAllowInanimateFormTF) ||
                     (targetForm.MobilityType == PvPStatics.MobilityPet && healthProportion <= PvPStatics.PercentHealthToAllowAnimalFormTF))
            {
                PerformInanimateTransformation(victim, attacker, skillSourceId, targetForm, output);
                BountyProcedures.ClaimReward(attacker, victim, targetForm);
            }

            // mind control
            else if (targetForm.MobilityType == PvPStatics.MobilityMindControl && healthProportion <= PvPStatics.PercentHealthToAllowMindControlTF)
            {
                EngageMindControl(victim, attacker, targetForm, output);
            }
        }

        private static void PerformAnimateTransformation(Player victim, Player attacker, DbStaticForm targetForm, LogBox output)
        {
            var playerRepo = new EFPlayerRepository();
            var dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

            SkillProcedures.UpdateFormSpecificSkillsToPlayer(dbVictim, targetForm.Id);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = dbVictim.Id,
                FormSourceId = targetForm.Id
            });

            // wipe out half of the target's mana
            dbVictim.Mana -= dbVictim.MaxMana / 2;
            if (dbVictim.Mana < 0)
            {
                dbVictim.Mana = 0;
            }

            // Remove any Self Restore entires.
            RemoveSelfRestore(dbVictim);

            var targetbuffs = ItemProcedures.GetPlayerBuffs(dbVictim);
            dbVictim = PlayerProcedures.ReadjustMaxes(dbVictim, targetbuffs);


            // take away some of the victim's XP based on the their level
            // target.XP += -2.5M * target.Level;

            playerRepo.SavePlayer(dbVictim);

            output.LocationLog += $"<br><b>{dbVictim.GetFullName()} was completely transformed into a {targetForm.FriendlyName} here.</b>";
            output.AttackerLog += $"<br><b>You fully transformed {dbVictim.GetFullName()} into a {targetForm.FriendlyName}</b>!";
            output.VictimLog += $"<br><b>You have been fully transformed into a {targetForm.FriendlyName}!</b>";

            // Let the target know they are best friends with the angel plush.
            if (attacker.BotId == AIStatics.MinibossPlushAngelId)
            {
                output.VictimLog += $"<br><br><b>{attacker.GetFullName()}</b> was happy to make you into a new friend!<br>";
            }

            TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfFormSourceId(dbVictim.Id, targetForm.Id);

            StatsProcedures.AddStat(dbVictim.MembershipId, StatsProcedures.Stat__TimesAnimateTFed, 1);
            StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimateTFing, 1);
        }

        private static void PerformInanimateTransformation(Player victim, Player attacker, int skillSourceId, DbStaticForm targetForm, LogBox output)
        {
            SkillProcedures.UpdateFormSpecificSkillsToPlayer(victim, targetForm.Id);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = victim.Id,
                FormSourceId = targetForm.Id
            });

            if (targetForm.MobilityType == PvPStatics.MobilityInanimate && victim.BotId != AIStatics.MinibossPlushAngelId) //No reward for monsters that hurt an innocent little plush friend. :(
            {
                StatsProcedures.AddStat(victim.MembershipId, StatsProcedures.Stat__TimesInanimateTFed, 1);
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesInanimateTFing, 1);
            }
            else if (targetForm.MobilityType == PvPStatics.MobilityPet && victim.BotId != AIStatics.MinibossPlushAngelId) //No reward for monsters that hurt an innocent little plush friend. :(
            {
                StatsProcedures.AddStat(victim.MembershipId, StatsProcedures.Stat__TimesAnimalTFed, 1);
                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimalTFing, 1);
            }

            if (targetForm.MobilityType == PvPStatics.MobilityPet || targetForm.MobilityType == PvPStatics.MobilityInanimate)
            {
                if (victim.BotId == AIStatics.PsychopathBotId)
                {
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PsychopathsDefeated, 1);
                }

                if (victim.BotId == AIStatics.ActivePlayerBotId && attacker.GameMode == (int)GameModeStatics.GameModes.PvP && victim.GameMode == (int)GameModeStatics.GameModes.PvP)
                {
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PvPPlayerNumberTakedowns, 1);
                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PvPPlayerLevelTakedowns, victim.Level);
                }
            }

            // extra log stuff for turning into item
            var extra = ItemProcedures.PlayerBecomesItem(victim, targetForm, attacker);
            output.AttackerLog += extra.AttackerLog;
            output.VictimLog += extra.VictimLog;
            output.LocationLog += extra.LocationLog;

            // give some of the victim's money to the attacker, the amount depending on what mode the victim is in
            var moneygain = victim.Money * .35M;
            PlayerProcedures.GiveMoneyToPlayer(attacker, moneygain);
            PlayerProcedures.GiveMoneyToPlayer(victim, -moneygain / 2);

            var levelDifference = attacker.Level - victim.Level;

            // only give the lump sum XP if the victim is not in the same covenant
            if (attacker.Covenant == null || attacker.Covenant != victim.Covenant)
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
                output.AttackerLog += $"  <br>For having sealed your opponent into their new form, you gain an extra <b>{xpGain}</b> XP.";
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
                    output.AttackerLog += $"  {victim.GetFullName()} unfortunately did not have any dungeon points for you to steal for yourself.";
                }
            }

            // Call out a player for being the monster they are when they defeat the plush angel.
            if (victim.BotId == AIStatics.MinibossPlushAngelId)
            {
                output.AttackerLog += "<br><br>Why did you do that to the poor plush? They just wanted to be a friend!<br>";
                output.LocationLog += $"<br><b>{attacker.GetFullName()}</b> went and bullied <b>{victim.GetFullName()}</b>, like some <b>monster</b>. The angelic plush left some flowers to the 'victor', in hope they would forgive it despite doing no wrong.";

                // Give the dummy a bit of madness for being a bully.
                EffectProcedures.GivePerkToPlayer(198, attacker);
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
                        healingPercent /= 2;
                    }

                    // Heal the attacker and restore their Mana
                    var healingTotal = attacker.MaxHealth * healingPercent;
                    var manaRestoredTotal = attacker.MaxMana * healingPercent;
                    PlayerProcedures.ChangePlayerActionMana(0, healingTotal, manaRestoredTotal, attacker.Id, false);

                    // Remove any Self Restore entires.
                    RemoveSelfRestore(victim);

                    output.AttackerLog += $"<br />Invigorated by your victory and fuelled by the scattered essence that was once your foe, you are healed for {healingTotal:#} willpower and {manaRestoredTotal:#} mana.";
                }
            }

            output.AttackerLog += $"  You collect {Math.Round(moneygain, 0)} Arpeyjis your victim dropped during the transformation.";

            // create inanimate XP for the victim
            InanimateXPProcedures.GetStruggleChance(victim, false);

            // if this victim is a bot, clear out some old stuff that is not needed anymore
            if (victim.BotId < AIStatics.ActivePlayerBotId)
            {
                AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(victim.Id);
                PlayerLogProcedures.ClearPlayerLog(victim.Id);
            }

            TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfFormSourceId(victim.Id, targetForm.Id);

            // if the attacker is a psycho, have them change to a new spell and equip whatever they just earned
            if (attacker.BotId == AIStatics.PsychopathBotId)
            {
                SkillProcedures.DeletePlayerSkill(attacker, skillSourceId);

                if (targetForm.MobilityType == PvPStatics.MobilityInanimate || targetForm.MobilityType == PvPStatics.MobilityPet)
                {
                    if (attacker.MembershipId.IsNullOrEmpty())
                    {
                        // give this bot a random replacement inanimate/pet skill
                        var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
                        var rand = new Random();
                        var skillToLearn = eligibleSkills.ElementAt(rand.Next(eligibleSkills.Count));
                        SkillProcedures.GiveSkillToPlayer(attacker.Id, skillToLearn.Id);
                    }
                    else
                    {
                        // Bot is being controlled by a player - re-add the original skill so only the ordering of skills changes
                        SkillProcedures.GiveSkillToPlayer(attacker.Id, skillSourceId);
                    }
                }

                if (!EffectProcedures.PlayerHasActiveEffect(attacker.Id, JokeShopProcedures.PSYCHOTIC_EFFECT))
                {
                    // have the psycho equip any items they are carrying (if they have any duplicates in a slot, they'll take them off later in world update)
                    // Do not apply to temporary psychos to avoid circumventing inventory rules
                    var psychoItems = ItemProcedures.GetAllPlayerItems(attacker.Id)
                         .Where(item => item.Item.ItemType != PvPStatics.ItemType_Rune);

                    foreach (var i in psychoItems)
                    {
                        ItemProcedures.EquipItem(i.dbItem.Id, true);
                    }
                }

            }
        }

        private static void EngageMindControl(Player target, Player attacker, DbStaticForm targetForm, LogBox output)
        {
            //Player attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);
            MindControlProcedures.AddMindControl(attacker, target, targetForm.Id);

            output.LocationLog += $"<br><b>{target.GetFullName()} was partially mind controlled by {attacker.GetFullName()} here.</b>";
            output.AttackerLog += $"<br><b>You have seized the mind of {target.GetFullName()}!  You can now force them into performing certain actions.</b>";
            output.VictimLog += $"<br><b>You are now being partially mind controlled by {targetForm.FriendlyName}!</b>";

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

        private static string FormatVictimString(string input, string victimName, string attackerName, bool isNew)
        {
            var content = CleanString(input, victimName, attackerName);
            return isNew ? content : $"<span style=\"color: #555555\">{content}</span>";
        }

        private static string CleanString(string input, string victimName, string attackerName)
        {
            return input?.Trim().Replace(Environment.NewLine, "</br>").Replace("$VICTIM_NAME$", victimName).Replace("$ATTACKER_NAME$", attackerName);
        }

        private static string GetTFMessage(DbStaticForm form, Player victim, Player attacker, string PoV, int previousStage, int finalStage)
        {
            var output = "";

            ITFMessageRepository tfMessageRepo = new EFTFMessageRepository();
            var tfMessage = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormSourceId == form.Id);

            if (tfMessage == null)
            {
                return "ERROR RETRIEVING TRANSFORMATION TEXT.  This is a bug.";
            }

            var attackerName = attacker.GetFullName();
            var victimName = victim.GetFullName();

            // 0-20 percent TF
            var currentStage = 0;
            if (finalStage == currentStage || (finalStage > currentStage && previousStage < currentStage))
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, currentStage,
                    tfMessage.TFMessage_20_Percent_1st, tfMessage.TFMessage_20_Percent_1st_M, tfMessage.TFMessage_20_Percent_1st_F,
                    tfMessage.TFMessage_20_Percent_3rd, tfMessage.TFMessage_20_Percent_3rd_M, tfMessage.TFMessage_20_Percent_3rd_F);
            }

            // 20-40 percent TF
            currentStage = 1;
            if (finalStage == currentStage || (finalStage > currentStage && previousStage < currentStage))
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, currentStage,
                    tfMessage.TFMessage_40_Percent_1st, tfMessage.TFMessage_40_Percent_1st_M, tfMessage.TFMessage_40_Percent_1st_F,
                    tfMessage.TFMessage_40_Percent_3rd, tfMessage.TFMessage_40_Percent_3rd_M, tfMessage.TFMessage_40_Percent_3rd_F);
           }

            // 40-60 percent TF
            currentStage = 2;
            if (finalStage == currentStage || (finalStage > currentStage && previousStage < currentStage))
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, 2,
                    tfMessage.TFMessage_60_Percent_1st, tfMessage.TFMessage_60_Percent_1st_M, tfMessage.TFMessage_60_Percent_1st_F,
                    tfMessage.TFMessage_60_Percent_3rd, tfMessage.TFMessage_60_Percent_3rd_M, tfMessage.TFMessage_60_Percent_3rd_F);
            }

            // 60-80 percent TF
            currentStage = 3;
            if (finalStage == currentStage || (finalStage > currentStage && previousStage < currentStage))
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, 3,
                    tfMessage.TFMessage_80_Percent_1st, tfMessage.TFMessage_80_Percent_1st_M, tfMessage.TFMessage_80_Percent_1st_F,
                    tfMessage.TFMessage_80_Percent_3rd, tfMessage.TFMessage_80_Percent_3rd_M, tfMessage.TFMessage_80_Percent_3rd_F);
            }

            // 80-100 percent TF
            currentStage = 4;
            if (finalStage == currentStage || (finalStage > currentStage && previousStage < currentStage))
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, currentStage,
                    tfMessage.TFMessage_100_Percent_1st, tfMessage.TFMessage_100_Percent_1st_M, tfMessage.TFMessage_100_Percent_1st_F,
                    tfMessage.TFMessage_100_Percent_3rd, tfMessage.TFMessage_100_Percent_3rd_M, tfMessage.TFMessage_100_Percent_3rd_F);
            }

            // complete TF
            currentStage = 5;
            if (finalStage >= currentStage)
            {
                output += AddStageMessage(victim, attacker, PoV, previousStage, finalStage, currentStage,
                    tfMessage.TFMessage_Completed_1st, tfMessage.TFMessage_Completed_1st_M, tfMessage.TFMessage_Completed_1st_F,
                    tfMessage.TFMessage_Completed_3rd, tfMessage.TFMessage_Completed_3rd_M, tfMessage.TFMessage_Completed_3rd_F);
            }

            return output;
        }

        private static string AddStageMessage(Player victim, Player attacker, string PoV, int previousStage, int eventualStage, int textStage,
            string firstPerson, string firstPersonMale, string firstPersonFemale, string thirdPerson, string thirdPersonMale, string thirdPersonFemale)
        {
            var attackerName = attacker.GetFullName();
            var victimName = victim.GetFullName();

            var output = "";

            if (PoV == "first")
            {
                var isNew = previousStage < textStage;

                if (victim.Gender == PvPStatics.GenderMale)
                {
                    var message = firstPersonMale.IsNullOrEmpty() ? firstPerson : firstPersonMale;
                    output = FormatVictimString(message, victimName, attackerName, isNew);
                }
                else if (victim.Gender == PvPStatics.GenderFemale)
                {
                    var message = firstPersonFemale.IsNullOrEmpty() ? firstPerson : firstPersonFemale;
                    output = FormatVictimString(message, victimName, attackerName, isNew);
                }
            }
            else if (PoV == "third")
            {
                if (victim.Gender == PvPStatics.GenderMale)
                {
                    var message = thirdPersonMale.IsNullOrEmpty() ? thirdPerson : thirdPersonMale;
                    output = CleanString(message, victimName, attackerName);
                }
                else if (victim.Gender == PvPStatics.GenderFemale)
                {
                    var message = thirdPersonFemale.IsNullOrEmpty() ? thirdPerson : thirdPersonFemale;
                    output = CleanString(message, victimName, attackerName);
                }
            }

            if(eventualStage != textStage)
            {
                output += "<br /><br />";
            }

            return output;
        }

        public static decimal GetHigherLevelXPModifier(int lvl, int maxLvlBeforeLoss)
        {
            var modifier = 1 - (lvl - maxLvlBeforeLoss) * .1M;

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