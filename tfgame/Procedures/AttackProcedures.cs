using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public static class AttackProcedures
    {

        public static string Attack(Player attacker, Player victim, SkillViewModel2 skillBeingUsed)
        {

            

            string result = "";

            Player me = PlayerProcedures.GetPlayer(attacker.Id);
            Player targeted = PlayerProcedures.GetPlayer(victim.Id);

            if (targeted.Mobility != "full" || me.Mobility != "full")
            {
                return "";
            }

            LogBox logs = new LogBox();



            // all of our checks seem to be okay.  So let's lower the player's mana and action points
            PlayerProcedures.ChangePlayerActionMana(PvPStatics.AttackCost, 0, -skillBeingUsed.Skill.ManaCost, me.Id);

            PlayerProcedures.LogCombatTimestampsAndAddAttackCount(targeted, me);

            string attackerFullName = me.FirstName + " " + me.LastName;
            string victimFullName = targeted.FirstName + " " + targeted.LastName;

            // if the spell is a curse, give the effect and that's all
            if (skillBeingUsed.Skill.GivesEffect != null)
            {
                DbStaticEffect effectBeingGiven = EffectStatics.GetStaticEffect2(skillBeingUsed.Skill.GivesEffect);

                EffectProcedures.GivePerkToPlayer(skillBeingUsed.Skill.GivesEffect, victim);

                if (attacker.Gender == "male" && effectBeingGiven.AttackerWhenHit_M != null && effectBeingGiven.AttackerWhenHit_M != "")
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit_M;
                }
                else if (attacker.Gender == "female" && effectBeingGiven.AttackerWhenHit_F != null && effectBeingGiven.AttackerWhenHit_F != "")
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit_F;
                }
                else
                {
                    logs.AttackerLog += effectBeingGiven.AttackerWhenHit;
                }

                logs.AttackerLog += "<br><br>";

                logs.LocationLog = "<span class='playerAttackNotification'>" + attackerFullName + " cursed " + victimFullName + " with " + skillBeingUsed.Skill.FriendlyName + ".</span>";
                logs.AttackerLog += "You cursed " + victimFullName + " with " + skillBeingUsed.Skill.FriendlyName +".";
                logs.AttackerLog += "  (+1 XP)  ";
                logs.AttackerLog += PlayerProcedures.GiveXP(attacker.Id, 1);
                logs.VictimLog = effectBeingGiven.MessageWhenHit;
                logs.VictimLog += "  <span class='playerAttackNotification'><b>" + attackerFullName + " cursed you with " + skillBeingUsed.Skill.FriendlyName + ".</b></span>  ";
                result = logs.AttackerLog;
                
            }

            // the spell is a regular attack
            else
            {

                logs.LocationLog = "<span class='playerAttackNotification'>" + attackerFullName + " cast " + skillBeingUsed.Skill.FriendlyName + " against " + victimFullName + ".</span>";
                logs.AttackerLog = "You cast " + skillBeingUsed.Skill.FriendlyName + " against " + victimFullName + ".  ";
                logs.VictimLog = "<span class='playerAttackNotification'>" + attackerFullName + " cast " + skillBeingUsed.Skill.FriendlyName + " against you.</span>  ";

                string attackerPronoun = "";
                string victimPronoun = "";

                if (me.Gender == "male")
                {
                    attackerPronoun = "his";
                }
                else if (me.Gender == "female")
                {
                    attackerPronoun = "Her";
                }
                else
                {
                    attackerPronoun = "their";
                }
                if (targeted.Gender == "male")
                {
                    victimPronoun = "his";
                }
                else if (targeted.Gender == "female")
                {
                    victimPronoun = "her";
                }
                else
                {
                    victimPronoun = "their";
                }

                BuffBox meBuffs = ItemProcedures.GetPlayerBuffs(me);
                BuffBox targetedBuffs = ItemProcedures.GetPlayerBuffs(targeted);

                Random rand = new Random(Guid.NewGuid().GetHashCode());
                double basehitChance = rand.NextDouble() * 100;

                decimal meDmgExtra = meBuffs.SpellExtraHealthDamagePercent();
                decimal targetProt = targetedBuffs.SpellHealthDamageResistance();

                decimal myEvasionNegation = meBuffs.EvasionNegationPercent();

                decimal criticalMissPercentChance = PvPStatics.CriticalMissPercentChance - meBuffs.SpellMisfireChanceReduction();
                decimal evasionPercentChance = targetedBuffs.EvasionPercent() - meBuffs.EvasionNegationPercent();

                // clamp evasion at 66% max for human players
                if (evasionPercentChance > 66 && victim.MembershipId > 0)
                {
                    evasionPercentChance = 66;
                }

                // critical miss!  damange caster instead
                if (basehitChance < (double)criticalMissPercentChance)
                {
                    // check if there is a health damage aspect to this spell
                    if (skillBeingUsed.Skill.HealthDamageAmount > 0)
                    {
                        PlayerProcedures.DamagePlayerHealth(me.Id, skillBeingUsed.Skill.HealthDamageAmount * (1 + meBuffs.SpellExtraHealthDamagePercent() / 100));
                        logs.AttackerLog += "Misfire!  Your spell accidentally lowered your own willpower by " + Math.Round(skillBeingUsed.Skill.HealthDamageAmount, 2) + ".  ";
                        logs.VictimLog += "Misfire!  " + attackerPronoun + "'s spell accidentally lowered " + attackerPronoun + " own willpower by " +  Math.Round(skillBeingUsed.Skill.HealthDamageAmount, 2) + ".";
                        result += logs.AttackerLog;
                    }


               // spell is evaded
                }
                else if (basehitChance < (double)criticalMissPercentChance + (double)evasionPercentChance)
                {
                    logs.AttackerLog += victimFullName + " managed to leap out of the way of your spell.";
                    logs.VictimLog += "You managed to leap out of the way " + attackerFullName + "'s spell.";
                    result = logs.AttackerLog;
                }

              // not a  miss, so let's deal some damage, possibly
                else
                {


                    Random rand2 = new Random();
                    double criticalHitChance = rand.NextDouble() * 100;
                    decimal criticalModifier = 1;

                    if (criticalHitChance < (double)(PvPStatics.CriticalHitPercentChance + meBuffs.ExtraSkillCriticalPercent()))
                    {
                        criticalModifier = 2;
                        logs.AttackerLog += "<b>Critical hit!</b>  ";
                        logs.VictimLog += "<b>Critical hit!</b>  ";
                    }


                    // check if there is a health damage aspect to this spell
                    if (skillBeingUsed.Skill.HealthDamageAmount > 0)
                    {

                        // get the lowest level involved in this combat
                        int lowestLevel = (me.Level > targeted.Level) ? targeted.Level : me.Level;
                        decimal extraDamageFromLevels = PvPStatics.ExtraHealthDamagePerLevel * (decimal)(lowestLevel - 1);

                        // add even more damage if the spell is "weaken"
                        extraDamageFromLevels = (skillBeingUsed.Skill.TFPointsAmount == 0) ? extraDamageFromLevels + (1.15M * (decimal)lowestLevel) : extraDamageFromLevels;


                        // calculator the modifier as extra attack - defense.      15 - 20 = -5 modifier
                        decimal willpowerDamageModifierFromBonuses = 1 + ((meDmgExtra - targetProt) / 100.0M);

                        // cap the modifier at at 50 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses < .5M && victim.MembershipId > 0)
                        {
                            willpowerDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at at 200 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses > 2 && victim.MembershipId > 0)
                        {
                            willpowerDamageModifierFromBonuses = 2;
                        }

                        decimal totalHealthDamage = (skillBeingUsed.Skill.HealthDamageAmount + extraDamageFromLevels) * willpowerDamageModifierFromBonuses * criticalModifier;

                        // make sure damage is never in the negatives (which would heal instead)
                        if (totalHealthDamage < 0)
                        {
                            totalHealthDamage = 0;
                        }

                        PlayerProcedures.DamagePlayerHealth(targeted.Id, totalHealthDamage);

                        // even though it's been done in the db, change the player health here as well
                        targeted.Health -= totalHealthDamage;

                        logs.AttackerLog += "Your spell lowered " + victimPronoun + " willpower by " + Math.Round(totalHealthDamage,2) + ".  ";
                        logs.VictimLog += attackerPronoun + " spell lowered your willpower by " + Math.Round(totalHealthDamage,2) + ".";
                        result += logs.AttackerLog;
                    }

                    // if this skill has any TF power, add energy and check for form change
                    if (skillBeingUsed.Skill.TFPointsAmount > 0)
                    {

                        decimal TFEnergyDmg = meBuffs.SpellExtraTFEnergyPercent();
                        decimal TFEnergyArmor = targetedBuffs.SpellTFEnergyDamageResistance();

                        // calculator the modifier as extra attack - defense.
                        decimal tfEnergyDamageModifierFromBonuses = 1 + ((TFEnergyDmg - TFEnergyArmor) / 100.0M);

                        // cap the modifier at at 50 % IF the target is a human
                        if (tfEnergyDamageModifierFromBonuses < .5M && victim.MembershipId > 0)
                        {
                            tfEnergyDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at at 200 % IF the target is a human
                        if (tfEnergyDamageModifierFromBonuses > 2 && victim.MembershipId > 0)
                        {
                            tfEnergyDamageModifierFromBonuses = 2;
                        }

                        decimal totalTFEnergyModifier = criticalModifier * tfEnergyDamageModifierFromBonuses;

                        LogBox tfEnergyResult = TFEnergyProcedures.AddTFEnergyToPlayer(targeted, me, skillBeingUsed, totalTFEnergyModifier);


                        logs.Add(tfEnergyResult);
                        LogBox formChangeLog = TFEnergyProcedures.RunFormChangeLogic(targeted, skillBeingUsed.Skill.dbName, me.Id);
                        logs.Add(formChangeLog);
                        result = logs.AttackerLog;

                    }

                }

            }

            LocationLogProcedures.AddLocationLog(me.dbLocationName, logs.LocationLog);
            PlayerLogProcedures.AddPlayerLog(me.Id, logs.AttackerLog, false);
            PlayerLogProcedures.AddPlayerLog(targeted.Id, logs.VictimLog, true);

            return result;
        }

        public static string Attack(Player attacker, Player victim, string skillBeingUsed)
        {
            SkillViewModel2 vm = SkillProcedures.GetSkillViewModel_NotOwned(skillBeingUsed);
            return Attack(attacker, victim, vm);
        }

        public static string ThrowGrenade(Player attacker, decimal damage, string orbStrengthName)
        {

            IPlayerRepository playerREpo = new EFPlayerRepository();

            Location here = LocationsStatics.GetLocation.First(l => l.dbName == attacker.dbLocationName);

            List<Player> playersHere = new List<Player>();
            List<Player> playersHereOnline = new List<Player>();
            if (attacker.InPvP == true)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attacker.dbLocationName && (p.InPvP == true || p.MembershipId < -1) && p.Mobility == "full").ToList();
            }
            else
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attacker.dbLocationName && (p.InPvP == false || p.MembershipId < -1) && p.Mobility == "full").ToList();
            }

            // filter out offline players as well as the attacker
            foreach (Player p in playersHere)
            {
                if (PlayerProcedures.PlayerIsOffline(p) == false && p.Id != attacker.Id)
                {
                    playersHereOnline.Add(p);
                }
            }

            foreach (Player p in playersHereOnline)
            {
                p.Health -= damage;
                if (p.Health < 0)
                {
                    p.Health = 0;
                }
                playerREpo.SavePlayer(p);
                string message = "<span class='playerAttackNotification'>" + attacker.FirstName + " " + attacker.LastName + " threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering your willpower by " + damage + " along with " + (playersHereOnline.Count() - 1) + " others.</span>";
                PlayerLogProcedures.AddPlayerLog(p.Id, message, true);

            }

            string logMessage = attacker.FirstName + " " + attacker.LastName + " threw a Submissiveness Splash Orb here.";
            LocationLogProcedures.AddLocationLog(attacker.dbLocationName, logMessage);

            string attackerMessage = "<span class='playerAttackNotification'>You threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering " + playersHereOnline.Count() + " people's willpower by " + damage + " each.</span>";
            PlayerLogProcedures.AddPlayerLog(attacker.Id, attackerMessage, false);

            // set the player's last action flag
            Player dbAttacker = playerREpo.Players.First(p => p.Id == attacker.Id);
            dbAttacker.LastActionTimestamp = DateTime.UtcNow;
            dbAttacker.TimesAttackingThisUpdate++;
            playerREpo.SavePlayer(dbAttacker);


            return attackerMessage;
        }

        public static void AddMindControl(Player attacker, Player victim, string type)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbAttacker = playerRepo.Players.FirstOrDefault(p => p.Id == attacker.Id);
            Player dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

            MindControl mc = new MindControl
            {
                TurnsRemaining = 12,
                MasterId = attacker.Id,
                VictimId = victim.Id,
                Type = type,
            };

            mcRepo.SaveMindControl(mc);

            dbVictim.MindControlIsActive = true;


        }

    }
}