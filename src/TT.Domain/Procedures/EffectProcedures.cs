using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class EffectProcedures
    {

        public const string BackOnYourFeetEffect = "help_animate_recovery";

        public static IEnumerable<EffectViewModel2> GetPlayerEffects2(int playerId)
        {

            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<EffectViewModel2> output = from i in effectRepo.Effects
                                                   where i.OwnerId == playerId
                                                   join si in effectRepo.DbStaticEffects on i.dbName equals si.dbName
                                                   select new EffectViewModel2
                                                   {
                                                       dbEffect = new Effect_VM
                                                       {
                                                           Id = i.Id,
                                                           OwnerId = i.OwnerId,
                                                           Duration = i.Duration,
                                                           IsPermanent = i.IsPermanent,
                                                           Level = i.Level,
                                                           Cooldown = i.Cooldown,
                                                           dbName = i.dbName,


                                                       },

                                                       Effect = new TT.Domain.ViewModels.StaticEffect
                                                       {
                                                           dbName = si.dbName,
                                                           FriendlyName = si.FriendlyName,
                                                           Description = si.Description,
                                                           AvailableAtLevel = si.AvailableAtLevel,
                                                           PreRequesite = si.PreRequesite,
                                                           isLevelUpPerk = si.isLevelUpPerk,
                                                           Duration = si.Duration,
                                                           Cooldown = si.Cooldown,
                                                           ObtainedAtLocation = si.ObtainedAtLocation,
                                                           IsRemovable = si.IsRemovable,
                                                           BlessingCurseStatus = si.BlessingCurseStatus,
                                                          
                                                           MessageWhenHit = si.MessageWhenHit,
                                                           MessageWhenHit_M = si.MessageWhenHit_M,
                                                           MessageWhenHit_F = si.MessageWhenHit_F,
                                                           AttackerWhenHit = si.AttackerWhenHit,
                                                           AttackerWhenHit_M = si.AttackerWhenHit_M,
                                                           AttackerWhenHit_F = si.AttackerWhenHit_F,

                                                           HealthBonusPercent = si.HealthBonusPercent,
                                                           ManaBonusPercent = si.ManaBonusPercent,
                                                           ExtraSkillCriticalPercent = si.ExtraSkillCriticalPercent,
                                                           HealthRecoveryPerUpdate = si.HealthRecoveryPerUpdate,
                                                           ManaRecoveryPerUpdate = si.ManaRecoveryPerUpdate,
                                                           SneakPercent = si.SneakPercent,
                                                           EvasionPercent = si.EvasionPercent,
                                                           EvasionNegationPercent = si.EvasionNegationPercent,
                                                           MeditationExtraMana = si.MeditationExtraMana,
                                                           CleanseExtraHealth = si.CleanseExtraHealth,
                                                           MoveActionPointDiscount = si.MoveActionPointDiscount,
                                                           SpellExtraHealthDamagePercent = si.SpellExtraHealthDamagePercent,
                                                           SpellExtraTFEnergyPercent = si.SpellExtraTFEnergyPercent,
                                                           CleanseExtraTFEnergyRemovalPercent = si.CleanseExtraTFEnergyRemovalPercent,
                                                           SpellMisfireChanceReduction = si.SpellMisfireChanceReduction,
                                                           SpellHealthDamageResistance = si.SpellHealthDamageResistance,
                                                           SpellTFEnergyDamageResistance = si.SpellTFEnergyDamageResistance,
                                                           ExtraInventorySpace = si.ExtraInventorySpace,

                                                           Discipline = si.Discipline,
                                                           Perception = si.Perception,
                                                           Charisma = si.Charisma,
                                                           Submission_Dominance = si.Submission_Dominance,

                                                           Fortitude = si.Fortitude,
                                                           Agility = si.Agility,
                                                           Allure = si.Allure,
                                                           Corruption_Purity = si.Corruption_Purity,

                                                           Magicka = si.Magicka,
                                                           Succour = si.Succour,
                                                           Luck = si.Luck,
                                                           Chaos_Order = si.Chaos_Order,


                                                       }

                                                   };

            return output;

        }

        public static IEnumerable<Effect> GetPlayerEffects_EffectOnly(int playerId)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            return effectRepo.Effects.Where(e => e.OwnerId == playerId);
        }

        public static List<DbStaticEffect> GetAvailableLevelupPerks(Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            // see what perks the player already has
            IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id && e.IsPermanent == true);
            List<string> playerEffectsString = playerEffects.Select(e => e.dbName).ToList();


             List<DbStaticEffect> availablePerks = effectRepo.DbStaticEffects.Where(e => e.AvailableAtLevel > 0 && e.AvailableAtLevel <= player.Level && e.isLevelUpPerk == true).ToList();

            List<DbStaticEffect> availablePerksFinal = new List<DbStaticEffect>();

            foreach (DbStaticEffect effect in availablePerks)
            {

                bool available = true;

                // if the player already has this effect, it is not available
                if (playerEffectsString.Contains(effect.dbName))
                {
                    continue;
                }

                // filter out any effects that have a prerequisite that the player does not yet have
                if (effect.PreRequesite != null && effect.PreRequesite != "" && playerEffectsString.Contains(effect.PreRequesite) == false)
                {
                    continue;
                }

                if (available == true)
                {
                    availablePerksFinal.Add(effect);
                }
            }

            return availablePerksFinal;

        }

        public static string GivePerkToPlayer(string perkName, int playerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            return GivePerkToPlayer(perkName, dbPlayer);
        }

        public static string GivePerkToPlayer(string perkName, Player player)
        {

            IEffectRepository effectRepo = new EFEffectRepository();
            
            // see if this player already has this perk.  If so, reject it
            Effect possibleSamePerk = effectRepo.Effects.FirstOrDefault(e => e.dbName == perkName && e.OwnerId == player.Id);

            if (possibleSamePerk != null)
            {
                return "You already have this perk.  Choose another.";
            }

            // grab the static part of this effect
            DbStaticEffect effectPlus = EffectStatics.GetStaticEffect2(perkName);


            if (effectPlus.isLevelUpPerk == true)
            {
                // assert that the perk doesn't require a level higher than the player
                if (effectPlus.AvailableAtLevel > player.Level)
                {
                    return "You are not at a high enough level to earn this perk.";
                }

                // assert that the perk doesn't require a prerequisite that the player does not yet have
                if (effectPlus.PreRequesite != null && effectPlus.PreRequesite != "")
                {
                    Effect requiredPrerequisite = effectRepo.Effects.FirstOrDefault(e => e.OwnerId == player.Id && e.dbName == effectPlus.PreRequesite);
                    if (requiredPrerequisite == null)
                    {
                        return "This perk requires the <b>" + EffectStatics.GetStaticEffect2(effectPlus.PreRequesite).FriendlyName + "</b> prerequisite perk which you do not have.";
                    }
                }

            }


            Effect addme = new Effect();

            addme.dbName = perkName;
            addme.OwnerId = player.Id;

            // this effect is a permanent levelup perk
            if (effectPlus.AvailableAtLevel > 0)
            {

                addme.Duration = 99999;
                addme.Cooldown = 99999;
                addme.IsPermanent = true;
                addme.Level = 1;

            }

            // this effect is temporary, grab some of its stats from the effect static
            if (effectPlus.AvailableAtLevel == 0)
            {
                addme.Duration = effectPlus.Duration;
                addme.Cooldown = effectPlus.Cooldown;
                addme.IsPermanent = false;
            }


            // okay to proceed--give this player this perk.







            // this is a level up perk so just return a simple message
            if (addme.IsPermanent == true)
            {
                string logmessage = "You have gained the perk " + effectPlus.FriendlyName + ".";
                PlayerLogProcedures.AddPlayerLog(player.Id, logmessage, false);

                //remove an unused perk from the player
                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player person = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
                person.UnusedLevelUpPerks--;
                playerRepo.SavePlayer(person);

                effectRepo.SaveEffect(addme);
                return logmessage;
            }

           // this is a temporary perk so return the flavor text
            else
            {

                string logmessage = "";

                if (player.Gender == "male" && effectPlus.MessageWhenHit_M != null && effectPlus.MessageWhenHit_M != "")
                {
                    logmessage = effectPlus.MessageWhenHit_M;
                }
                else if (player.Gender == "female" && effectPlus.MessageWhenHit_F != null && effectPlus.MessageWhenHit_F != "")
                {
                    logmessage = effectPlus.MessageWhenHit_F;
                }
                else
                {
                    logmessage = effectPlus.MessageWhenHit;
                }

                PlayerLogProcedures.AddPlayerLog(player.Id, effectPlus.MessageWhenHit, false);
                effectRepo.SaveEffect(addme);
                return logmessage;
            }


        }

        public static void RemovePerkFromPlayer(string perkName, Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            Effect effect = effectRepo.Effects.FirstOrDefault(e => e.dbName == perkName && e.OwnerId == player.Id);
            effectRepo.DeleteEffect(effect.Id);
        }


        public static void SetPerkDurationToZero(string perkName, Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            Effect effect = effectRepo.Effects.FirstOrDefault(e => e.dbName == perkName && e.OwnerId == player.Id);
            effect.Duration = 0;
            effectRepo.SaveEffect(effect);
        }

        public static void DeleteAllPlayerEffects(int playerId)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            List<Effect> effectsToDelete = effectRepo.Effects.Where(e => e.OwnerId == playerId).ToList();

            foreach (Effect e in effectsToDelete)
            {
                effectRepo.DeleteEffect(e.Id);
            }
        }

        //public static IEnumerable<EffectViewModel> GetPlayerEffectViewModels(Player player)
        //{

        //    IEffectRepository effectRepo = new EFEffectRepository();
        //    IEnumerable<Effect> playerdbEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id);

        //    List<EffectViewModel> playerEffects = new List<EffectViewModel>();


        //    foreach (Effect effect in playerdbEffects)
        //    {
        //        EffectViewModel addme = new EffectViewModel
        //        {
        //            Effect = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == effect.dbName),
        //            dbEffect = effect

        //        };
        //        playerEffects.Add(addme);
        //    }

        //    return playerEffects;

        //}

        public static bool PlayerHasEffect(Player player, string effectName)
        {

            IEffectRepository effectRepo = new EFEffectRepository();

            Effect possibleEffect = effectRepo.Effects.FirstOrDefault(e => e.OwnerId == player.Id && e.dbName == effectName);

            if (possibleEffect == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
    }
}