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
    public static class EffectProcedures
    {

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

                                                          Effect = new tfgame.ViewModels.StaticEffect
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

                                                          }

                                                      };

            return output;
        
        }

        //public static List<EffectViewModel> GetPlayerEffects(int playerId)
        //{
        //    IEffectRepository effectRepo = new EFEffectRepository();

        //    //IEnumerable<EffectViewModel> output =
        //    //from e in effectRepo.Effects
        //    //where e.OwnerId == playerId
        //    //from s in EffectStatics.GetStaticEffect
        //    //where s.dbName == e.dbName
        //    //select new EffectViewModel { dbEffect = e, Effect = s };

        //    List<Effect> mydbEffects = effectRepo.Effects.Where(e => e.OwnerId == playerId).ToList();

        //    List<EffectViewModel> output = new List<EffectViewModel>();

        //    foreach (Effect e in mydbEffects)
        //    {
        //        EffectViewModel addme = new EffectViewModel
        //        {
        //            dbEffect = e,
        //            Effect = EffectStatics.GetStaticEffect.FirstOrDefault(f => f.dbName == f.dbName),
        //        };
        //        output.Add(addme);
        //    }

        //    return output;

        //}

        public static List<DbStaticEffect> GetAvailableLevelupPerks(Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id && e.IsPermanent == true);

            // see what perks the player already has

            List<DbStaticEffect> perkEffects = EffectStatics.GetAvailableLevelupPerks(player);

            List<DbStaticEffect> availablePerks = new List<DbStaticEffect>();

            foreach (DbStaticEffect effect in perkEffects)
            {
                Effect existingEffect = playerEffects.FirstOrDefault(d => d.dbName == effect.dbName);

                bool available = true;

                // if the player already has this effect, it is not available
                if (existingEffect != null)
                {
                    available = false;
                }

                if (available == true)
                {
                    availablePerks.Add(effect);
                }
                

            }

            return availablePerks;

        }

        //public static List<StaticEffect> GetPlayerEffects(Player player)
        //{
        //    IEffectRepository effectRepo = new EFEffectRepository();
        //    IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id);

        //    List<DbStaticEffect> playerPerks = new List<DbStaticEffect>();

        //    foreach (Effect effect in playerEffects)
        //    {
        //        DbStaticEffect effectPlus = EffectStatics.GetStaticEffect2(effect.dbName);
        //        playerPerks.Add(effectPlus);
        //    }

        //    return playerPerks;
        //}

        public static string GivePerkToPlayer(string perkName, Player player)
        {

            IEffectRepository effectRepo = new EFEffectRepository();


            // see if this player already has this perk.  If so, reject it
            Effect possibleSamePerk = effectRepo.Effects.FirstOrDefault(e => e.dbName == perkName && e.OwnerId == player.Id);

            if (possibleSamePerk != null)
            {
                return "You already have this perk.  Choose another.";
            }

            // grab the static effect for stat
            DbStaticEffect effectPlus = EffectStatics.GetStaticEffect2(perkName);

            
            if (effectPlus.AvailableAtLevel > 0 && effectPlus.AvailableAtLevel!=9999)
            {
                // assert that the perk doesn't require a level higher than the player
                if (effectPlus.AvailableAtLevel > player.Level)
                {
                    return "You are not at a high enough level to earn this perk.";
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

        public static void LoadEffectRAMBuffBox()
        {
            IDbStaticEffectRepository dbStaticEffectRepo = new EFDbStaticEffectRepository();

            EffectStatics.EffectRAMBuffBoxes = new List<RAMBuffBox>();

            foreach (DbStaticEffect e in dbStaticEffectRepo.DbStaticEffects.Where(c => c.dbName != null && c.dbName != ""))
            {
                RAMBuffBox temp = new RAMBuffBox
                {
                    dbName = e.dbName.ToLower(),

                    HealthBonusPercent = (float)e.HealthBonusPercent,
                    ManaBonusPercent = (float)e.ManaBonusPercent,
                    HealthRecoveryPerUpdate = (float)e.HealthRecoveryPerUpdate,
                    ManaRecoveryPerUpdate = (float)e.ManaRecoveryPerUpdate,
                };
                EffectStatics.EffectRAMBuffBoxes.Add(temp);
            }
        }

       

    }
}