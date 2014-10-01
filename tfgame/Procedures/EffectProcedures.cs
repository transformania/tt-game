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
        public static List<EffectViewModel> GetPlayerEffects(int playerId)
        {
            IEffectRepository effectRepo = new EFEffectRepository();

            //IEnumerable<EffectViewModel> output =
            //from e in effectRepo.Effects
            //where e.OwnerId == playerId
            //from s in EffectStatics.GetStaticEffect
            //where s.dbName == e.dbName
            //select new EffectViewModel { dbEffect = e, Effect = s };

            List<Effect> mydbEffects = effectRepo.Effects.Where(e => e.OwnerId == playerId).ToList();

            List<EffectViewModel> output = new List<EffectViewModel>();

            foreach (Effect e in mydbEffects)
            {
                EffectViewModel addme = new EffectViewModel
                {
                    dbEffect = e,
                    Effect = EffectStatics.GetStaticEffect.FirstOrDefault(eff => eff.dbName == e.dbName)
                };
                output.Add(addme);
            }

            return output;

        }

        public static List<StaticEffect> GetAvailableLevelupPerks(Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id && e.IsPermanent == true);

            // see what perks the player already has

            List<StaticEffect> perkEffects = EffectStatics.GetStaticEffect.Where(e => e.AvailableAtLevel > 0 && e.AvailableAtLevel <= player.Level).ToList();

            List<StaticEffect> availablePerks = new List<StaticEffect>();

            foreach (StaticEffect effect in perkEffects)
            {
                Effect existingEffect = playerEffects.FirstOrDefault(d => d.dbName == effect.dbName);

                bool available = true;

                // if the player already has this effect, it is not available
                if (existingEffect != null)
                {
                    available = false;
                }

                // if the effect has a prerequisite, assert that the player does have it
                //if (effect.PreRequesite != null && effect.PreRequesite != "")
                //{
                //    StaticEffect prereq = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == effect.PreRequesite);

                //    // assert player has this effect
                //    Effect dbPreReq = playerEffects.FirstOrDefault(d => d.dbName == prereq.dbName);

                //    if (dbPreReq == null)
                //    {
                //        available = false;
                //    }

                //}
                if (available == true)
                {
                    availablePerks.Add(effect);
                }
                

            }

            //List<StaticEffect> availablePerksFinal = new List<StaticEffect>();

            //foreach (StaticEffect e in availablePerks)
            //{
            //    availablePerksFinal.Add(e);
            //}

            // iterate through all of the current player perks and remove any that has a prerequisite the player owns
            //foreach (StaticEffect effect in availablePerks)
            //{
            //    if (effect.dbName.Contains("_lvl"))
            //    {
            //        int lplace = effect.dbName.Length - 5;
            //        string srank = effect.dbName.Substring(lplace, 1);
            //        int rank = Convert.ToInt32(srank);

            //        string reconstructed_down1 = effect.dbName.Substring(0, lplace - 1) + "_" + (rank-1) + "_lvl";
            //        string reconstructed_down2 = effect.dbName.Substring(0, lplace - 1) + "_" + (rank-2) + "_lvl";
            //        string reconstructed_down3 = effect.dbName.Substring(0, lplace - 1) + "_" + (rank - 3) + "_lvl";
            //        string reconstructed_down4 = effect.dbName.Substring(0, lplace - 1) + "_" + (rank - 4) + "_lvl";

            //        Effect down1 = playerEffects.FirstOrDefault( x => x.dbName == reconstructed_down1 );
            //        Effect down2 = playerEffects.FirstOrDefault(x => x.dbName == reconstructed_down1);
            //        Effect down3 = playerEffects.FirstOrDefault(x => x.dbName == reconstructed_down1);
            //        Effect down4 = playerEffects.FirstOrDefault(x => x.dbName == reconstructed_down1);

            //        // if player does not have any of the prerequisite perks, accept it
            //        if (down1 != null)
            //        {
            //            availablePerksFinal = availablePerksFinal.Where(a => a.dbName != down1.dbName).ToList();
            //        }
            //        if (down2 != null)
            //        {
            //            availablePerksFinal = availablePerksFinal.Where(a => a.dbName != down2.dbName).ToList();
            //        }
            //        if (down3 != null)
            //        {
            //            availablePerksFinal = availablePerksFinal.Where(a => a.dbName != down3.dbName).ToList();
            //        }
            //        if (down4 != null)
            //        {
            //            availablePerksFinal = availablePerksFinal.Where(a => a.dbName != down4.dbName).ToList();
            //        }


            //    }
            
            //}

            return availablePerks;

        }

        public static List<StaticEffect> GetPlayerEffects(Player player)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<Effect> playerEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id);

            List<StaticEffect> playerPerks = new List<StaticEffect>();

            foreach (Effect effect in playerEffects)
            {
                StaticEffect effectPlus = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == effect.dbName);
                playerPerks.Add(effectPlus);
            }

            return playerPerks;
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

            // grab the static effect for stat
            StaticEffect effectPlus = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == perkName);

            
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

                // if the perk has a prerequisite, delete the old one
                //if (effectPlus.PreRequesite!=null && effectPlus.PreRequesite!="") {
                //    StaticEffect prereq = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == effectPlus.PreRequesite);
                //    Effect dbPrereq = effectRepo.Effects.FirstOrDefault(e => e.OwnerId == person.Id && e.dbName == prereq.dbName);

                //    // assert that the player even has the preprequisite before they can get the new perk
                //    if (dbPrereq == null) {
                //        return "You don't have the necessary prerequisite to earn this perk.";
                //    }

                //    effectRepo.DeleteEffect(dbPrereq.Id);
                //}
                
                effectRepo.SaveEffect(addme);
                return logmessage;
            }


                

           // this is a temporary perk so return the flavor text
            else
            {
                string logmessage = effectPlus.GetMessageWhenHit(player.Gender);
                PlayerLogProcedures.AddPlayerLog(player.Id, effectPlus.MessageWhenHit, false);
                effectRepo.SaveEffect(addme);
                return logmessage;
            }

            
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

        public static IEnumerable<EffectViewModel> GetPlayerEffectViewModels(Player player)
        {

            IEffectRepository effectRepo = new EFEffectRepository();
            IEnumerable<Effect> playerdbEffects = effectRepo.Effects.Where(e => e.OwnerId == player.Id);

            List<EffectViewModel> playerEffects = new List<EffectViewModel>();


            foreach (Effect effect in playerdbEffects)
            {
                EffectViewModel addme = new EffectViewModel
                {
                    Effect = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == effect.dbName),
                    dbEffect = effect
                
                };
                playerEffects.Add(addme);
            }

            return playerEffects;

        }

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