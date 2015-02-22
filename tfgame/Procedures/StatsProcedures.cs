using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

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
        public const string Stat__TimesTeleported_Scroll = "times_teleported_scroll";
        public const string Stat__JewdewfaeEncountersCompleted = "jewdewfae_completions";
        public const string Stat__LoreBooksRead = "lore_books_read";


           //new Thread(() =>
           //     StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TimesMoved, 1)
           // ).Start();

        public static void AddStat(int membershipId, string type, float amount)
        {
            // don't keep stats for an AI characters
            if (membershipId > 0)
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

        public static IEnumerable<Achievement> GetPlayerStats(int membershipId)
        {
            IAchievementRepository repo = new EFAchievementRepository();

            return repo.Achievements.Where(a => a.OwnerMembershipId == membershipId);
        }
    }
}