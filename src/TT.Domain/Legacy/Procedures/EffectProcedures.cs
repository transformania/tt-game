using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Effects.Commands;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class EffectProcedures
    {
        public static IEnumerable<EffectViewModel2> GetPlayerEffects2(int playerId)
        {

            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<EffectViewModel2> output = from effect in effectRepo.Effects
                                                   where effect.OwnerId == playerId
                                                   join dbStaticEffect in effectRepo.DbStaticEffects on effect.EffectSourceId equals dbStaticEffect.Id
                                                   select new EffectViewModel2
                                                   {
                                                       dbEffect = new Effect_VM
                                                       {
                                                           Id = effect.Id,
                                                           OwnerId = effect.OwnerId,
                                                           Duration = effect.Duration,
                                                           IsPermanent = effect.IsPermanent,
                                                           Level = effect.Level,
                                                           Cooldown = effect.Cooldown,
                                                           EffectSourceId = effect.EffectSourceId


                                                       },

                                                       Effect = new TT.Domain.ViewModels.StaticEffect
                                                       {
                                                          
                                                           FriendlyName = dbStaticEffect.FriendlyName,
                                                           Description = dbStaticEffect.Description,
                                                           AvailableAtLevel = dbStaticEffect.AvailableAtLevel,
                                                           PreRequisiteEffectSourceId = dbStaticEffect.PreRequisiteEffectSourceId,
                                                           RequiredGameMode = dbStaticEffect.RequiredGameMode,
                                                           isLevelUpPerk = dbStaticEffect.isLevelUpPerk,
                                                           Duration = dbStaticEffect.Duration,
                                                           Cooldown = dbStaticEffect.Cooldown,
                                                           ObtainedAtLocation = dbStaticEffect.ObtainedAtLocation,
                                                           IsRemovable = dbStaticEffect.IsRemovable,
                                                           BlessingCurseStatus = dbStaticEffect.BlessingCurseStatus,
                                                          
                                                           MessageWhenHit = dbStaticEffect.MessageWhenHit,
                                                           MessageWhenHit_M = dbStaticEffect.MessageWhenHit_M,
                                                           MessageWhenHit_F = dbStaticEffect.MessageWhenHit_F,
                                                           AttackerWhenHit = dbStaticEffect.AttackerWhenHit,
                                                           AttackerWhenHit_M = dbStaticEffect.AttackerWhenHit_M,
                                                           AttackerWhenHit_F = dbStaticEffect.AttackerWhenHit_F,

                                                           HealthBonusPercent = dbStaticEffect.HealthBonusPercent,
                                                           ManaBonusPercent = dbStaticEffect.ManaBonusPercent,
                                                           ExtraSkillCriticalPercent = dbStaticEffect.ExtraSkillCriticalPercent,
                                                           HealthRecoveryPerUpdate = dbStaticEffect.HealthRecoveryPerUpdate,
                                                           ManaRecoveryPerUpdate = dbStaticEffect.ManaRecoveryPerUpdate,
                                                           SneakPercent = dbStaticEffect.SneakPercent,
                                                           EvasionPercent = dbStaticEffect.EvasionPercent,
                                                           EvasionNegationPercent = dbStaticEffect.EvasionNegationPercent,
                                                           MeditationExtraMana = dbStaticEffect.MeditationExtraMana,
                                                           CleanseExtraHealth = dbStaticEffect.CleanseExtraHealth,
                                                           MoveActionPointDiscount = dbStaticEffect.MoveActionPointDiscount,
                                                           SpellExtraHealthDamagePercent = dbStaticEffect.SpellExtraHealthDamagePercent,
                                                           SpellExtraTFEnergyPercent = dbStaticEffect.SpellExtraTFEnergyPercent,
                                                           CleanseExtraTFEnergyRemovalPercent = dbStaticEffect.CleanseExtraTFEnergyRemovalPercent,
                                                           SpellMisfireChanceReduction = dbStaticEffect.SpellMisfireChanceReduction,
                                                           SpellHealthDamageResistance = dbStaticEffect.SpellHealthDamageResistance,
                                                           SpellTFEnergyDamageResistance = dbStaticEffect.SpellTFEnergyDamageResistance,
                                                           ExtraInventorySpace = dbStaticEffect.ExtraInventorySpace,

                                                           Discipline = dbStaticEffect.Discipline,
                                                           Perception = dbStaticEffect.Perception,
                                                           Charisma = dbStaticEffect.Charisma,
                                                           Submission_Dominance = dbStaticEffect.Submission_Dominance,

                                                           Fortitude = dbStaticEffect.Fortitude,
                                                           Agility = dbStaticEffect.Agility,
                                                           Allure = dbStaticEffect.Allure,
                                                           Corruption_Purity = dbStaticEffect.Corruption_Purity,

                                                           Magicka = dbStaticEffect.Magicka,
                                                           Succour = dbStaticEffect.Succour,
                                                           Luck = dbStaticEffect.Luck,
                                                           Chaos_Order = dbStaticEffect.Chaos_Order,


                                                       }

                                                   };

            return output;

        }

        public static List<DbStaticEffect> GetAvailableLevelupPerks(Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            // see what perks the player already has
            IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id && e.IsPermanent);
            var playerEffectSourceIds = playerEffects.Select(e => e.EffectSourceId).ToList();


             var availablePerks = effectRepo.DbStaticEffects.Where(e => e.AvailableAtLevel > 0 && e.AvailableAtLevel <= player.Level && e.isLevelUpPerk).ToList();

            var availablePerksFinal = new List<DbStaticEffect>();

            foreach (var effect in availablePerks)
            {
                // if the player already has this effect, it is not available
                if (playerEffectSourceIds.Contains(effect.Id))
                {
                    continue;
                }

                // filter out any effects that have a prerequisite that the player does not yet have
                if (effect.PreRequisiteEffectSourceId != null && !playerEffectSourceIds.Contains(effect.PreRequisiteEffectSourceId.Value))
                {
                    continue;
                }

                if (effect.RequiredGameMode != null && effect.RequiredGameMode != player.GameMode)
                {
                    continue;
                }

               availablePerksFinal.Add(effect);
            }

            return availablePerksFinal;

        }

        public static string GivePerkToPlayer(int effectSourceId, int playerId, int? Duration = null, int? Cooldown = null)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            return GivePerkToPlayer(effectSourceId, dbPlayer);
        }

        public static string GivePerkToPlayer(int effectSourceId, Player player, int? Duration = null, int? Cooldown = null)
        {

            IEffectRepository effectRepo = new EFEffectRepository();
            
            // see if this player already has this perk.  If so, reject it
            var possibleSamePerk = effectRepo.Effects.FirstOrDefault(e => e.EffectSourceId == effectSourceId && e.OwnerId == player.Id);

            if (possibleSamePerk != null)
            {
                return "You already have this perk.  Choose another.";
            }

            // grab the static part of this effect
            var effectPlus = EffectStatics.GetDbStaticEffect(effectSourceId);


            if (effectPlus.isLevelUpPerk)
            {
                // assert that the perk doesn't require a level higher than the player
                if (effectPlus.AvailableAtLevel > player.Level)
                {
                    return "You are not at a high enough level to earn this perk.";
                }

                // assert that the perk doesn't require a prerequisite that the player does not yet have
                if (effectPlus.PreRequisiteEffectSourceId != null)
                {
                    var requiredPrerequisite = effectRepo.Effects.FirstOrDefault(e => e.OwnerId == player.Id && e.EffectSourceId == effectPlus.PreRequisiteEffectSourceId);
                    if (requiredPrerequisite == null)
                    {
                        return "This perk requires the <b>" + EffectStatics.GetDbStaticEffect(effectPlus.PreRequisiteEffectSourceId.Value).FriendlyName + "</b> prerequisite perk which you do not have.";
                    }
                }

            }

            var cmd = new CreateEffect();

            cmd.EffectSourceId = EffectStatics.GetDbStaticEffect(effectSourceId).Id;
            cmd.OwnerId = player.Id;

            // this effect is a permanent levelup perk
            if (effectPlus.AvailableAtLevel > 0)
            {

                cmd.Duration = 99999;
                cmd.Cooldown = 99999;
                cmd.IsPermanent = true;
                cmd.Level = 1;

            }

            // this effect is temporary, grab some of its stats from the effect static
            if (effectPlus.AvailableAtLevel == 0)
            {
                var duration = Duration ?? effectPlus.Duration;
                var cooldown = Cooldown.HasValue ? Math.Max(duration, Cooldown.Value) : effectPlus.Cooldown;

                cmd.Duration = duration;
                cmd.Cooldown = cooldown;
                cmd.IsPermanent = false;
            }


            // okay to proceed--give this player this perk.
            DomainRegistry.Repository.Execute(cmd);
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var person = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var logMessage = $"You have gained the perk {effectPlus.FriendlyName}.";

            if (cmd.IsPermanent)
            {
                // this is a level up perk so just return a simple message
                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, false);

                //remove an unused perk from the player
                person.UnusedLevelUpPerks--;
            }
            else
            {
                // this is a temporary perk so return the flavor text
                if (player.Gender == PvPStatics.GenderMale && !effectPlus.MessageWhenHit_M.IsNullOrEmpty())
                {
                    logMessage = effectPlus.MessageWhenHit_M;
                }
                else if (player.Gender == PvPStatics.GenderFemale && !effectPlus.MessageWhenHit_F.IsNullOrEmpty())
                {
                    logMessage = effectPlus.MessageWhenHit_F;
                }
                else
                {
                    logMessage = effectPlus.MessageWhenHit;
                }

                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, false);
            }
            person.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(person));
            playerRepo.SavePlayer(person);

            return logMessage;
        }

        public static string MergePlayerPerk(int effectSourceId, Player player, int? duration = null, int? cooldown = null)
        {
            var effectRepo = new EFEffectRepository();
            var perk = effectRepo.Effects.FirstOrDefault(e => e.EffectSourceId == effectSourceId && e.OwnerId == player.Id);

            if (perk == null)
            {
                return GivePerkToPlayer(effectSourceId, player, duration, cooldown);
            }

            if (perk.IsPermanent)
            {
                return null;
            }

            if (duration.HasValue)
            {
                perk.Duration = Math.Max(perk.Duration, duration.Value);
            }

            if (cooldown.HasValue)
            {
                perk.Cooldown = Math.Max(Math.Max(perk.Cooldown, cooldown.Value), perk.Duration);
            }

            effectRepo.SaveEffect(perk);
            var staticPerk = effectRepo.DbStaticEffects.FirstOrDefault(e => e.Id == effectSourceId);

            return staticPerk == null ? "One of your effects has been renewed!" : $"Your {staticPerk.FriendlyName} effect has been renewed!";
        }

        public static void RemovePerkFromPlayer(int effectSourceId, Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            var effect = effectRepo.Effects.FirstOrDefault(e => e.EffectSourceId == effectSourceId && e.OwnerId == player.Id);
            effectRepo.DeleteEffect(effect.Id);
        }

        public static void SetPerkDurationToZero(int effectSourceId, Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            var effect = effectRepo.Effects.FirstOrDefault(e => e.EffectSourceId == effectSourceId && e.OwnerId == player.Id);
            effect.Duration = 0;
            effectRepo.SaveEffect(effect);
        }

        public static void DeleteAllPlayerEffects(int playerId)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            var effectsToDelete = effectRepo.Effects.Where(e => e.OwnerId == playerId).ToList();

            foreach (var e in effectsToDelete)
            {
                effectRepo.DeleteEffect(e.Id);
            }
        }

        public static bool PlayerHasEffect(Player player, int effectSourceId)
        {

            IEffectRepository effectRepo = new EFEffectRepository();

            var possibleEffect = effectRepo.Effects.FirstOrDefault(e => e.OwnerId == player.Id && e.EffectSourceId == effectSourceId);

            if (possibleEffect == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        public static bool PlayerHasActiveEffect(int playerId, int effectSourceId)
        {
            return PlayerHasActiveEffect(PlayerProcedures.GetPlayer(playerId), effectSourceId);
        }

        public static bool PlayerHasActiveEffect(Player player, int effectSourceId)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            return effectRepo.Effects.FirstOrDefault(
                e => e.OwnerId == player.Id &&
                     e.EffectSourceId == effectSourceId && 
                     e.Duration > 0) != null;
        }

    }
}