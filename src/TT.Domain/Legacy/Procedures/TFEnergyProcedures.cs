using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TT.Domain.Abstract;
using TT.Domain.Combat.Commands;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class TFEnergyProcedures
    {
        public static LogBox AddTFEnergyToPlayer(Player victim, Player attacker, SkillViewModel skill, decimal modifier)
        {

            // assert modifier is never negative (reduced TF damage instead of adding it)
            if (modifier < 0)
            {
                modifier = 0;
            }

            LogBox output = new LogBox();
            ITFEnergyRepository repo = new EFTFEnergyRepository();

            output.AttackerLog = "  ";
            output.VictimLog = "  ";

            
            // crunch down any old TF Energies into one public energy
            List<TFEnergy> energiesOnPlayer = repo.TFEnergies.Where(e => e.PlayerId == victim.Id && e.FormName == skill.Skill.FormdbName).ToList();

            List<TFEnergy> energiesEligibleForDelete = new List<TFEnergy>();
            decimal mergeUpEnergyAmt = 0;

            foreach (TFEnergy e in energiesOnPlayer) {

                double minutesAgo = Math.Abs(Math.Floor(e.Timestamp.Subtract(DateTime.UtcNow).TotalMinutes));

                if (minutesAgo > 180)
                {
                    mergeUpEnergyAmt += e.Amount;
                    energiesEligibleForDelete.Add(e);
                }
            }

            // if the amount of old energies is greater than 0, write up a new one and save it as 'public domain' TF Energy
            if (mergeUpEnergyAmt > 0)
            {

                var cmd = new CreateTFEnergy
                {
                    PlayerId = victim.Id,
                    Amount = mergeUpEnergyAmt,
                    CasterId = null,
                    FormName = skill.Skill.FormdbName
                };

                var form = FormStatics.GetForm(skill.Skill.FormdbName);

                if (form != null)
                {
                    cmd.FormSourceId = FormStatics.GetForm(skill.Skill.FormdbName).Id;
                }

                DomainRegistry.Repository.Execute(cmd);


                foreach (TFEnergy e in energiesEligibleForDelete)
                {
                    repo.DeleteTFEnergy(e.Id);
                }

            }

            // get the amount of TF Energy the attacker has on the player
            TFEnergy energyFromMe = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormName == skill.Skill.FormdbName && e.CasterId == attacker.Id);

            if (energyFromMe == null)
            {

                var cmd = new CreateTFEnergy
                {
                    PlayerId = victim.Id,
                    Amount = skill.Skill.TFPointsAmount*modifier,
                    CasterId = attacker.Id,
                    FormName = skill.Skill.FormdbName
                };

                var form = FormStatics.GetForm(skill.Skill.FormdbName);

                if (form != null)
                {
                    cmd.FormSourceId = FormStatics.GetForm(skill.Skill.FormdbName).Id;
                }

                DomainRegistry.Repository.Execute(cmd);

                // create an old entity just so it doesn't break functionality below
                energyFromMe = new TFEnergy
                {
                    PlayerId = victim.Id,
                    FormName = skill.Skill.FormdbName,
                    CasterId = attacker.Id,
                    Amount = skill.Skill.TFPointsAmount * modifier
                };

            }
            else
            {
                energyFromMe.Amount += skill.Skill.TFPointsAmount * modifier;
                energyFromMe.Timestamp = DateTime.UtcNow;
                repo.SaveTFEnergy(energyFromMe);
            }

            

            var totalEnergy = energyFromMe.Amount + mergeUpEnergyAmt;

            DbStaticForm eventualForm = FormStatics.GetForm(skill.Skill.FormdbName);

            output.AttackerLog += "  [" + totalEnergy + " / " + eventualForm.TFEnergyRequired + " TF energy]  ";
            output.VictimLog += "  [" + totalEnergy + " / " + eventualForm.TFEnergyRequired + " TF energy]  ";

            if (victim.Form == eventualForm.dbName)
            {
                output.AttackerLog += "Since " + victim.GetFullName() + " is already in this form, the spell has no transforming effect.";
                output.VictimLog += "Since " + victim.GetFullName() + " is already in this form, the spell has no transforming effect.";
                return output;
            }


            decimal percentTransformedByHealth = 1 - (victim.Health / victim.MaxHealth);

            // animate forms only need half of health requirement, so double the amount completed
            decimal PercentHealthToAllowTF = 0;
            if (eventualForm.MobilityType == PvPStatics.MobilityFull)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowFullMobilityFormTF;
            }
            else if (eventualForm.MobilityType == PvPStatics.MobilityInanimate)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowInanimateFormTF;
            }
            else if (eventualForm.MobilityType == PvPStatics.MobilityPet)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowAnimalFormTF;
            }
            else if (eventualForm.MobilityType == PvPStatics.MobilityMindControl)
            {
                PercentHealthToAllowTF = PvPStatics.PercentHealthToAllowMindControlTF;
            }

            percentTransformedByHealth /= 1-PercentHealthToAllowTF;

            decimal percentTransformed = totalEnergy / eventualForm.TFEnergyRequired;

            if (percentTransformed > 1)
            {
                percentTransformed = 1;
            }

            // only print the lower of the two tf % values
            decimal percentPrintedOutput;
            if (percentTransformed < percentTransformedByHealth)
            {
                percentPrintedOutput = percentTransformed;
            }
            else
            {
                percentPrintedOutput = percentTransformedByHealth;
            }

            percentPrintedOutput = Math.Round(percentPrintedOutput*100, 3);

            if (percentTransformed < 0.20m || percentTransformedByHealth < 0.20m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "20", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "20", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.40m || percentTransformedByHealth < 0.40m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "40", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "40", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.60m || percentTransformedByHealth < 0.60m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "60", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "60", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 0.80m || percentTransformedByHealth < 0.80m)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "80", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "80", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed < 1 || percentTransformedByHealth < 1)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "100", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "100", "first") + " (" + percentPrintedOutput + "%)";
            }
            else if (percentTransformed >= 1)
            {
                output.AttackerLog += GetTFMessage(eventualForm, victim, attacker, "complete", "third") + " (" + percentPrintedOutput + "%)";
                output.VictimLog += GetTFMessage(eventualForm, victim, attacker, "complete", "first") + " (" + percentPrintedOutput + "%)";
            }

            // calculate the xp earned for this transformation
            decimal xpEarned = PvPStatics.XP__GainPerAttackBase - (attacker.Level - victim.Level) * PvPStatics.XP__LevelDifferenceXPGainModifier;

            if (xpEarned < 0)
            {
                xpEarned = 0;
            }
            if (xpEarned > 15)
            {
                xpEarned = 15;
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

                string lvlMessage = PlayerProcedures.GiveXP(attacker, xpEarned);
                output.AttackerLog += lvlMessage;
                output.ResultMessage += lvlMessage;

            return output;

        }

        public static LogBox RunFormChangeLogic(Player victim, string skilldbName, int attackerId)
        {

            LogBox output = new LogBox();

            // redundant check to make sure the victim is still in a transformable state
            if (victim.Mobility != PvPStatics.MobilityFull)
            {
                return output;
            }

            ITFEnergyRepository repo = new EFTFEnergyRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            DbStaticSkill skill = SkillStatics.GetStaticSkill(skilldbName);

            TFEnergy pooledEnergy = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormName == skill.FormdbName && e.CasterId == null);
            TFEnergy myEnergy = repo.TFEnergies.FirstOrDefault(e => e.PlayerId == victim.Id && e.FormName == skill.FormdbName && e.CasterId == attackerId);

            DbStaticForm targetForm = FormStatics.GetForm(skill.FormdbName);

            decimal energyAccumulated = 0;
            if (pooledEnergy != null)
            {
                energyAccumulated = pooledEnergy.Amount + myEnergy.Amount;
            }
            else
            {
                energyAccumulated = myEnergy.Amount;
            }

            // check and see if the target has enough points accumulated to try the form's energy requirement
            if (energyAccumulated < targetForm.TFEnergyRequired)
            {
                return output;
            }

            // check and see if the target's health is low enough to be eligible for the TF
            Player target = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);
            Player attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);

            // add in some extra WP damage if TF energy high enough for TF but WP is still high
            if (energyAccumulated > targetForm.TFEnergyRequired * 1.25M && energyAccumulated <= targetForm.TFEnergyRequired * 1.5M && (target.BotId == AIStatics.ActivePlayerBotId || target.BotId == AIStatics.PsychopathBotId))
            {
                output.VictimLog += "  You gasp as your body shivers with a surplus of transformation energy built up within it, leaving you distracted and your willpower increasingly impaired. You take an extra 3 willpower damage.";
                output.AttackerLog += "  Your victim has a high amount of transformation energy built up and takes an extra 3 willpower damage.";
                target.Health -= 3;
                target.NormalizeHealthMana();
                playerRepo.SavePlayer(target);
            }
            else if (energyAccumulated > targetForm.TFEnergyRequired * 1.5M && (target.BotId == AIStatics.ActivePlayerBotId || target.BotId == AIStatics.PsychopathBotId))
            {
                output.VictimLog += "  You body spasms as the surplus of transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra 6 willpower damage.";
                output.AttackerLog += "  Your victim has an extremely high amount of transformation energy built up and takes an extra 6 willpower damage.";
                target.Health -= 6;
                target.NormalizeHealthMana();
                playerRepo.SavePlayer(target);
            }
            else if (energyAccumulated > targetForm.TFEnergyRequired * 1.75M && (target.BotId == AIStatics.ActivePlayerBotId || target.BotId == AIStatics.PsychopathBotId))
            {
                output.VictimLog += "  You collapse to your knees and your vision wavers as transformation energy threatens to transform you spontaneously.  You fight it but only after it drains you of more of your precious remaining willpower! You take an extra 9 willpower damage.";
                output.AttackerLog += "  Your victim has an extremely high amount of transformation energy built up and takes an extra 9 willpower damage.";
                target.Health -= 9;
                target.NormalizeHealthMana();
                playerRepo.SavePlayer(target);
            }

                DbStaticForm oldForm = FormStatics.GetForm(target.Form);

                #region animate transformation
                // target is turning into an animate form
                if (targetForm.MobilityType == PvPStatics.MobilityFull && (target.Health / target.MaxHealth) <= PvPStatics.PercentHealthToAllowFullMobilityFormTF)
                {

                    SkillProcedures.UpdateFormSpecificSkillsToPlayer(target, oldForm.dbName, targetForm.dbName);
                    DomainRegistry.Repository.Execute(new ChangeForm
                    {
                        PlayerId = target.Id,
                        FormName = targetForm.dbName
                    });

                    // wipe out half of the target's mana
                    target.Mana -= target.MaxMana / 2;
                    if (target.Mana < 0)
                    {
                        target.Mana = 0;
                    }

                    BuffBox targetbuffs = ItemProcedures.GetPlayerBuffs(target);
                    target = PlayerProcedures.ReadjustMaxes(target, targetbuffs);


                    // take away some of the victim's XP based on the their level
                   // target.XP += -2.5M * target.Level;

                    playerRepo.SavePlayer(target);

                    output.LocationLog = "<br><b>" + target.GetFullName() + " was completely transformed into a " + targetForm.FriendlyName + " here.</b>";
                    output.AttackerLog = "<br><b>You fully transformed " + target.GetFullName() + " into a " + targetForm.FriendlyName + "</b>!";
                    output.VictimLog = "<br><b>You have been fully transformed into a " + targetForm.FriendlyName + "!</b>";

                    TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfType(target.Id, targetForm.dbName);

                    new Thread(() =>
                         StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesAnimateTFed, 1)
                     ).Start();

                    new Thread(() =>
                         StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimateTFing, 1)
                     ).Start();

                }
                #endregion

                #region inanimate and animal
                // target is turning into an inanimate or animal form, both are endgame
                else if ((targetForm.MobilityType == PvPStatics.MobilityInanimate && (target.Health / target.MaxHealth) <= PvPStatics.PercentHealthToAllowInanimateFormTF) || (targetForm.MobilityType == "animal" && (target.Health / target.MaxHealth) <= PvPStatics.PercentHealthToAllowAnimalFormTF))
                {

                    SkillProcedures.UpdateFormSpecificSkillsToPlayer(target, oldForm.dbName, targetForm.dbName);
                    DomainRegistry.Repository.Execute(new ChangeForm
                    {
                        PlayerId = target.Id,
                        FormName = targetForm.dbName
                    });

                    if (targetForm.MobilityType == PvPStatics.MobilityInanimate)
                    {
                        new Thread(() =>
                             StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesInanimateTFed, 1)
                        ).Start();

                        new Thread(() =>
                            StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesInanimateTFing, 1)
                        ).Start();

                        if (target.BotId == AIStatics.PsychopathBotId)
                        {
                            new Thread(() =>
                                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PsychopathsDefeated, 1)
                            ).Start();
                        }

                    }
                    else if (targetForm.MobilityType == "animal")
                    {
                        new Thread(() =>
                             StatsProcedures.AddStat(target.MembershipId, StatsProcedures.Stat__TimesAnimalTFed, 1)
                        ).Start();


                        new Thread(() =>
                             StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__TimesAnimalTFing, 1)
                        ).Start();

                        if (target.BotId == AIStatics.PsychopathBotId)
                        {
                            new Thread(() =>
                                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__PsychopathsDefeated, 1)
                            ).Start();
                        }


                    }
                   
                    // extra log stuff for turning into item
                    LogBox extra = ItemProcedures.PlayerBecomesItem(target, targetForm, attacker);
                    output.AttackerLog += extra.AttackerLog;
                    output.VictimLog += extra.VictimLog;
                    output.LocationLog += extra.LocationLog;

                    // give some of the victim's money to the attacker, the amount depending on what mode the victim is in
                    decimal moneygain = victim.Money * .35M;
                PlayerProcedures.GiveMoneyToPlayer(attacker, moneygain);
                PlayerProcedures.GiveMoneyToPlayer(victim, -moneygain / 2);

                int levelDifference = attacker.Level - target.Level;

                // only give the lump sum XP if the target is within 3 levels of the attacker AND the attack is in PvP mode AND the victim is not in the same covenant
                if (levelDifference <= 5)
                {

                    decimal xpGain = 50 - (PvPStatics.XP__EndgameTFCompletionLevelBase * levelDifference);

                    if (xpGain < 5)
                    {
                        xpGain = 5;
                    } else if (xpGain > 75)
                    {
                        xpGain = 75;
                    }

                    // give the attacker a nice lump sum for having completed the transformation
                    output.AttackerLog += "  <br>For having sealed your opponent into their new form, you gain an extra <b>" + xpGain + "</b> XP.";
                    output.AttackerLog += PlayerProcedures.GiveXP(attacker, xpGain);
                }

                // exclude PvP score for bots
                if (victim.BotId == AIStatics.ActivePlayerBotId)
                {
                    decimal score = PlayerProcedures.GetPvPScoreFromWin(attacker, victim);

                    if (score > 0)
                    {

                        output.AttackerLog += PlayerProcedures.GivePlayerPvPScore(attacker, victim, score);
                        output.VictimLog += PlayerProcedures.RemovePlayerPvPScore(victim, attacker, score);

                        new Thread(() =>
                            StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__DungeonPointsStolen, (float)score)
                        ).Start();

                    }
                    else
                    {
                        output.AttackerLog += "  " + victim.GetFullName() + " unfortunately did not have any dungeon points for you to steal for yourself.";
                    }
                }



                output.AttackerLog += "  You collect " + Math.Round(moneygain,0) + " Arpeyjis your victim dropped during the transformation.";

                    // create inanimate XP for the victim
                    InanimateXPProcedures.GetStruggleChance(victim);

                    // if this victim is a bot, clear out some old stuff that is not needed anymore
                    if (victim.BotId < AIStatics.ActivePlayerBotId)
                    {
                        AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(victim.Id);
                        PlayerLogProcedures.ClearPlayerLog(victim.Id);
                    }

                    TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfType(target.Id, targetForm.dbName);

                    // if the attacker is a psycho, have them change to a new spell and equip whatever they just earned
                    if (attacker.BotId == AIStatics.PsychopathBotId)
                    {
                       SkillProcedures.DeleteAllPlayerSkills(attacker.Id);

                       // give this bot a random skill
                       List<DbStaticSkill> eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
                       Random rand = new Random();
                       double max = eligibleSkills.Count();
                       int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                       DbStaticSkill skillToLearn = eligibleSkills.ElementAt(randIndex);
                       SkillProcedures.GiveSkillToPlayer(attacker.Id, skillToLearn);

                        // have the psycho equip any items they are carrying (if they have any duplicates in a slot, they'll take them off later in world update)
                       List<ItemViewModel> psychoItems = ItemProcedures.GetAllPlayerItems(attacker.Id).ToList();

                       foreach (ItemViewModel i in psychoItems)
                       {
                           ItemProcedures.EquipItem(i.dbItem.Id, true);
                       }

                    }


                   

                }
                #endregion

                #region mind control
                else if (targetForm.MobilityType == PvPStatics.MobilityMindControl && (target.Health / target.MaxHealth) <= PvPStatics.PercentHealthToAllowMindControlTF)
                {
                    //Player attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);
                    MindControlProcedures.AddMindControl(attacker, victim, targetForm.dbName);

                    output.LocationLog = "<br><b>" + target.GetFullName() + " was partially mind controlled by " + attacker.GetFullName() + " here.</b>";
                    output.AttackerLog = "<br><b>You have seized the mind of " + target.GetFullName() + "!  You can now force them into forming certain actions.";
                    output.VictimLog = "<br><b>You are now being partially mind controlled by " + targetForm.FriendlyName + "!</b>";

                    TFEnergyProcedures.DeleteAllPlayerTFEnergiesOfType(target.Id, targetForm.dbName);

                    // give curse debuff
                    if (targetForm.dbName == MindControlStatics.MindControl__Movement)
                    {
                        EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Movement_DebuffEffect, target);
                    }
                    else if (targetForm.dbName == MindControlStatics.MindControl__Strip)
                    {
                        EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Strip_DebuffEffect, target);
                    }
                    else if (targetForm.dbName == MindControlStatics.MindControl__Meditate)
                    {
                        // TODO
                        //EffectProcedures.GivePerkToPlayer(MindControlStatics.MindControl__Strip_DebuffEffect, target);
                    }

                }
                #endregion

                // if there is a duel going on, end it if all but 1 player is defeated (not in the form they started in)
                if (victim.InDuel > 0)
                {
                    Duel duel = DuelProcedures.GetDuel(victim.InDuel);
                    List<PlayerFormViewModel> duelParticipants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);

                    int remainders = duelParticipants.Count();

                    foreach (PlayerFormViewModel p in duelParticipants)
                    {
                        if (p.Player.Form != duel.Combatants.FirstOrDefault(dp => dp.PlayerId == p.Player.Id).StartForm)
                        {
                            remainders--;
                        }
                    }

                    if (remainders <= 1)
                    {
                        DuelProcedures.EndDuel(duel.Id, DuelProcedures.FINISHED);
                    }


                }

                return output;

        }

        public static void CleanseTFEnergies(Player player, decimal bonusPercentageFromBuffs)
        {
            ITFEnergyRepository repo = new EFTFEnergyRepository();
            IEnumerable<TFEnergy> mydbEnergies = repo.TFEnergies.Where(e => e.PlayerId == player.Id).ToList();

            foreach (TFEnergy energy in mydbEnergies)
            {
                energy.Amount *= 1 - (bonusPercentageFromBuffs / 100.0M);
                repo.SaveTFEnergy(energy);
            }

        }

        public static void DeleteAllPlayerTFEnergies(int playerId)
        {
            ITFEnergyRepository tfEnergyRepo = new EFTFEnergyRepository();
            IEnumerable<TFEnergy> energiesToDelete = tfEnergyRepo.TFEnergies.Where(s => s.PlayerId == playerId).ToList();

            foreach (TFEnergy s in energiesToDelete)
            {
                tfEnergyRepo.DeleteTFEnergy(s.Id);
            }
        }

        public static void DeleteAllPlayerTFEnergiesOfType(int playerId, string spellType)
        {
            ITFEnergyRepository tfEnergyRepo = new EFTFEnergyRepository();
            IEnumerable<TFEnergy> energiesToDelete = tfEnergyRepo.TFEnergies.Where(s => s.PlayerId == playerId && s.FormName == spellType).ToList();

            foreach (TFEnergy s in energiesToDelete)
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
            TFMessage tfMessage = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormDbName == form.dbName);

            if (tfMessage == null)
            {
                return "ERROR RETRIEVING TRANSFORMATION TEXT.  This is a bug.  Please post this error at the forums in the Bug Reports section:  http://luxianne.com/forum/index.php";
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
            decimal modifier = 1.0M;

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