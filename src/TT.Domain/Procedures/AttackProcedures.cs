using System;
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

        public static string Attack(Player attacker, Player victim, SkillViewModel skillBeingUsed)
        {

            

            string result = "";

            Player me = PlayerProcedures.GetPlayer(attacker.Id);
            Player targeted = PlayerProcedures.GetPlayer(victim.Id);

            if (targeted.Mobility != PvPStatics.MobilityFull || me.Mobility != PvPStatics.MobilityFull)
            {
                return "";
            }

            LogBox logs = new LogBox();

            decimal manaCost = (decimal)AttackProcedures.GetSpellManaCost(me, targeted);

            // all of our checks seem to be okay.  So let's lower the player's mana and action points
            PlayerProcedures.ChangePlayerActionMana(PvPStatics.AttackCost, 0, -manaCost, me.Id);

            PlayerProcedures.LogCombatTimestampsAndAddAttackCount(targeted, me);

            string attackerFullName = me.GetFullName();
            string victimFullName = targeted.GetFullName();

            // if the spell is a curse, give the effect and that's all
            if (skillBeingUsed.Skill.GivesEffect != null)
            {
                DbStaticEffect effectBeingGiven = EffectStatics.GetStaticEffect2(skillBeingUsed.Skill.GivesEffect);

                EffectProcedures.GivePerkToPlayer(skillBeingUsed.Skill.GivesEffect, victim);

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

                logs.AttackerLog += "<br><br>";

                logs.LocationLog = "<span class='playerAttackNotification'>" + attackerFullName + " cursed " + victimFullName + " with " + skillBeingUsed.Skill.FriendlyName + ".</span>";
                logs.AttackerLog += "You cursed " + victimFullName + " with " + skillBeingUsed.Skill.FriendlyName +".";
                logs.AttackerLog += "  (+1 XP)  ";
                logs.AttackerLog += PlayerProcedures.GiveXP(attacker, 1);
                logs.VictimLog = effectBeingGiven.MessageWhenHit;
                logs.VictimLog += "  <span class='playerAttackNotification'>" + attackerFullName + " cursed you with <b>" + skillBeingUsed.Skill.FriendlyName + "</b>.</b></span>  ";
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

                if (me.Gender == PvPStatics.GenderMale)
                {
                    attackerPronoun = "his";
                }
                else if (me.Gender == PvPStatics.GenderFemale)
                {
                    attackerPronoun = "Her";
                }
                else
                {
                    attackerPronoun = "their";
                }
                if (targeted.Gender == PvPStatics.GenderMale)
                {
                    victimPronoun = "his";
                }
                else if (targeted.Gender == PvPStatics.GenderFemale)
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

                // clamp evasion at 66% max
                if (evasionPercentChance > 66)
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
                        decimal midLevel = AttackProcedures.GetMiddleLevel(me, targeted);
                        decimal extraDamageFromLevels = PvPStatics.ExtraHealthDamagePerLevel * (midLevel - 1);

                        // add even more damage if the spell is "weaken"
                        extraDamageFromLevels = (skillBeingUsed.Skill.TFPointsAmount == 0) ? extraDamageFromLevels + (1.15M * midLevel) : extraDamageFromLevels;


                        // calculator the modifier as extra attack - defense.      15 - 20 = -5 modifier
                        decimal willpowerDamageModifierFromBonuses = 1 + ((meDmgExtra - targetProt) / 100.0M);

                        // cap the modifier at at 50 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses < .5M)
                        {
                            willpowerDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at at 200 % IF the target is a human
                        if (willpowerDamageModifierFromBonuses > 2 && victim.BotId == AIStatics.ActivePlayerBotId)
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
                        if (tfEnergyDamageModifierFromBonuses < .5M)
                        {
                            tfEnergyDamageModifierFromBonuses = .5M;
                        }

                        // cap the modifier at at 200 % IF the target is a human
                        if (tfEnergyDamageModifierFromBonuses > 2 && victim.BotId == AIStatics.ActivePlayerBotId)
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
            
            DomainRegistry.AttackNotificationBroker.Notify(targeted.Id, logs.VictimLog);

            // if this is a psycho-on-psycho battle, have a chance for the victim bot to switch targets to the attacker bot
            if (attacker.BotId == AIStatics.PsychopathBotId && victim.BotId == AIStatics.PsychopathBotId)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                double botAggroRoll = rand.NextDouble();
                if (botAggroRoll < .08)
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(victim.Id, attacker.Id);
                }
            }

            return result;
        }

        public static string Attack(Player attacker, Player victim, string skillBeingUsed)
        {
            SkillViewModel vm = SkillProcedures.GetSkillViewModel_NotOwned(skillBeingUsed);
            return Attack(attacker, victim, vm);
        }

        public static string ThrowGrenade(Player attacker, decimal damage, string orbStrengthName)
        {

            IPlayerRepository playerREpo = new EFPlayerRepository();

            Location here = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == attacker.dbLocationName);

            List<Player> playersHere = new List<Player>();
            List<Player> playersHereOnline = new List<Player>();
            if (attacker.GameMode == GameModeStatics.PvP)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attacker.dbLocationName &&
                    (p.GameMode == GameModeStatics.PvP || p.BotId < AIStatics.RerolledPlayerBotId) &&
                    p.Mobility == PvPStatics.MobilityFull &&
                     p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }
            else if (attacker.GameMode == GameModeStatics.Protection || attacker.GameMode == GameModeStatics.SuperProtection)
            {
                playersHere = playerREpo.Players.Where(p => p.dbLocationName == attacker.dbLocationName &&
                    p.BotId < AIStatics.RerolledPlayerBotId &&
                    p.Mobility == PvPStatics.MobilityFull &&
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();
            }

            // filter out offline players as well as the attacker
            foreach (Player p in playersHere)
            {
                if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != attacker.Id)
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
                string message = "<span class='playerAttackNotification'>" + attacker.GetFullName() + " threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering your willpower by " + damage + " along with " + (playersHereOnline.Count() - 1) + " others.</span>";
                PlayerLogProcedures.AddPlayerLog(p.Id, message, true);

            }

            string logMessage = attacker.FirstName + " " + attacker.LastName + " threw a Submissiveness Splash Orb here.";
            LocationLogProcedures.AddLocationLog(attacker.dbLocationName, logMessage);

            string attackerMessage = "You threw a " + orbStrengthName + " Submissiveness Splash Orb at " + here.Name + ", lowering " + playersHereOnline.Count() + " people's willpower by " + damage + " each.";
            PlayerLogProcedures.AddPlayerLog(attacker.Id, attackerMessage, false);

            // set the player's last action flag
            Player dbAttacker = playerREpo.Players.First(p => p.Id == attacker.Id);
            dbAttacker.LastActionTimestamp = DateTime.UtcNow;
            dbAttacker.TimesAttackingThisUpdate++;
            playerREpo.SavePlayer(dbAttacker);


            return attackerMessage;
        }

        public static void InstantTakeoverLocation(Covenant cov, string location)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            LocationInfo info = repo.LocationInfos.FirstOrDefault(l => l.dbName == location);
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
            List<LocationInfo> info = repo.LocationInfos.ToList();
            foreach (Location loc in LocationsStatics.LocationList.GetLocation)
            {
                LocationInfo temp = info.FirstOrDefault(l => l.dbName == loc.dbName);
                if (temp == null)
                {
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == loc.dbName).CovenantController = -1;
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == loc.dbName).TakeoverAmount = 0;
                }
                else
                {
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == loc.dbName).CovenantController = temp.CovenantId;
                    LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == loc.dbName).TakeoverAmount = temp.TakeoverAmount;
                }
            }
        }


        /// <summary>
        /// Get the mana cost of a spell which cast between two players
        /// </summary>
        /// <param name="attacker">Attacking player</param>
        /// <param name="victim">Target of the spell</param>
        /// <returns></returns>
        public static float GetSpellManaCost(Player attacker, Player victim)
        {
            float baseManaCost = 4;

            float extra = (.75F * victim.Level) + (.25F * attacker.Level);

            float cost = (float)Math.Round((baseManaCost + extra),1);

            return cost;

        }

        public static decimal GetMiddleLevel(Player attacker, Player victim)
        {
            int lvlDiff = attacker.Level - victim.Level;

            int lowerLevel = 0;

            if (attacker.Level <= victim.Level)
            {
                lowerLevel = attacker.Level;
            } else
            {
                lowerLevel = victim.Level;
            }

            decimal mid = (decimal)(Math.Round(lowerLevel + (lvlDiff * .5),1));

            return mid;
        }



    }
}