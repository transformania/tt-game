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
    public static class StatsProcedures
    {

        public const string Stat__SearchCount = "times_searched";
        public const string Stat__SpellsCast = "spells_cast";
        public const string Stat__TimesMoved = "times_moved";
        public const string Stat__TimesCleansed = "times_cleansed";
        public const string Stat__TimesMeditated = "times_meditated";
        public const string Stat__TimesEnchanted = "times_enchanted";


        public const string Stat__CovenantDonationTotal = "covenant_donations";
        public const string Stat__CovenantGiftsReceived = "covenant_gif_receieved";

        public const string Stat__TimesAnimateTFed = "times_self_animated";
        public const string Stat__TimesInanimateTFed = "times_self_inanimated";
        public const string Stat__TimesAnimalTFed = "times_self_animal";

        public const string Stat__TimesAnimateTFing = "times_self_animating";
        public const string Stat__TimesInanimateTFing = "times_self_inanimating";
        public const string Stat__TimesAnimalTFing = "times_self_animaling";

        public const string Stat__PsychopathsDefeated = "psychos_defeated";

        public const string Stat__TimesTeleported_Scroll = "times_teleported_scroll";
        public const string Stat__JewdewfaeEncountersCompleted = "jewdewfae_completions";
        public const string Stat__LoreBooksRead = "lore_books_read";

        public const string Stat__InanimateXPEarned = "inanimateXPEarned";
        public const string Stat__PetXPEarned = "petXPEarned";

        public const string Stat__LindellaCostsAmount = "lindella_costs_amount";
        public const string Stat__LindellaProfitsAmount = "lindella_profit_amount";

        public const string Stat__WuffieCostsAmount = "wuffie_costs_amount";
        public const string Stat__WuffieProfitsAmount = "wuffie_profit_amount";

        public const string Stat__DungeonArtifactsFound = "dungeon_artifacts_found";
        public const string Stat__DungeonDemonsDefeated = "dungeon_demons_defeated";
        public const string Stat__DungeonPointsStolen = "dungeon_points_stolen";
        public const string Stat__DungeonMovements = "dungeon_movements";
        

           //new Thread(() =>
           //     StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TimesMoved, 1)
           // ).Start();

        public static void AddStat(string membershipId, string type, float amount)
        {
            // don't keep stats for an AI characters

          //  if (membershipId > 0 && PvPStatics.ChaosMode == false) 
            if (PlayerProcedures.GetPlayerFromMembership(membershipId).BotId == AIStatics.ActivePlayerBotId)
            {
                IAchievementRepository repo = new EFAchievementRepository();
                Achievement x = repo.Achievements.FirstOrDefault(a => a.OwnerMembershipId == membershipId && a.AchievementType == type);

                if (x == null)
                {
                    x = new Achievement
                    {
                        OwnerMembershipId = membershipId,
                        AchievementType = type,
                        Amount = amount,
                        Timestamp = DateTime.UtcNow,
                    };
                }
                else
                {
                    x.Amount += amount;
                    x.Timestamp = DateTime.UtcNow;
                }

                repo.SaveAchievement(x);
            }
        }

        public static IEnumerable<Achievement> GetPlayerStats(string membershipId)
        {
            IAchievementRepository repo = new EFAchievementRepository();

            return repo.Achievements.Where(a => a.OwnerMembershipId == membershipId);
        }

        public static IEnumerable<PlayerAchievementViewModel> GetPlayerMaxStats()
        {
            IAchievementRepository repo = new EFAchievementRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

         // return repo.Achievements.GroupBy(a => a.AchievementType).OrderByDescending(a => a.Amount).First();
          //  IEnumerable<Achievement> output = repo.Achievements.OrderByDescending(t => t.Amount).GroupBy(t => t.AchievementType).First();

            //IEnumerable<Achievement> output = from a in repo.Achievements
            //                                  group a by a.AchievementType into dptgrp
            //                                  let topsal = dptgrp.Max(x => x.Amount)
            //                                  select new Achievement
            //                                  {
            //                                      AchievementType = dptgrp.Key,
            //                                      Amount = dptgrp.First(y => y.Amount == topsal).Amount,
            //                                      OwnerMembershipId = dptgrp.First(y => y.Amount == topsal).OwnerMembershipId,
            //                                  };

            IEnumerable<Achievement> types = repo.Achievements.GroupBy(a => a.AchievementType).Select(grp => grp.FirstOrDefault()).ToList();

            List<PlayerAchievementViewModel> output = new List<PlayerAchievementViewModel>();

            foreach (Achievement t in types) {
                Achievement a = repo.Achievements.Where(b => b.AchievementType == t.AchievementType).OrderByDescending(b => b.Amount).FirstOrDefault();

                if (a != null)
                {
                    PlayerAchievementViewModel addMe = new PlayerAchievementViewModel
                    {
                        Player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == a.OwnerMembershipId),
                        Achivement = a,
                    };
                    output.Add(addMe);
                }

            }

            return output;

        }
    }
}