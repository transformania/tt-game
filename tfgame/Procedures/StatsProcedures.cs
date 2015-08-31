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

    public class StatsDetailsMap
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

    }

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
        public const string Stat__CovenantFurnitureUsed = "covenant_furniture_used";

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

        public const string Stat__SuccessfulStruggles = "times_struggled_free";

        // items
        public const string Stat__TransmogsUsed = "transmogs_used";
        public const string Stat__CovenantCallbackCrystalsUsed = "callback_crystals_used";
        public const string Stat__DollsWPRestored = "dolls_used";

        public static Dictionary<string, StatsDetailsMap> StatTypesMap = new Dictionary<string, StatsDetailsMap> {
           
        {
                Stat__SearchCount,
                    new StatsDetailsMap{
                        FriendlyName = "Hawkeye",
                        Description="Times searched",
                        ImageUrl="trophy.jpg",
                        }
                    
                    },

                {
                Stat__SpellsCast,
                    new StatsDetailsMap{
                        FriendlyName = "Quantity not Quality!",
                        Description="Spells cast",
                        ImageUrl="trophy.jpg",
                        }
                },
                
                {
                Stat__TimesMoved,
                    new StatsDetailsMap{
                        FriendlyName = "The Restless One",
                        Description="Times moved",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesCleansed,
                    new StatsDetailsMap{
                        FriendlyName = "Hypochondriac",
                        Description="Times cleansed",
                        ImageUrl="trophy.jpg",
                        }
                },

                  {
                Stat__TimesMeditated,
                    new StatsDetailsMap{
                        FriendlyName="Lost in Thought",
                        Description="Times meditated",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesEnchanted,
                    new StatsDetailsMap{
                        FriendlyName="____ the Enchanter",
                        Description="Times enchanting",
                        ImageUrl="trophy.jpg",
                        }
                },

                 // -------- COVENANT STUFF -------------

                 {
                Stat__CovenantDonationTotal,
                    new StatsDetailsMap{
                        FriendlyName = "Fundraiser Fanatic",
                        Description="Arpyjis donated to covenant",
                        ImageUrl="trophy.jpg",
                        }
                },
                  {
                Stat__CovenantFurnitureUsed,
                    new StatsDetailsMap{
                        FriendlyName = "Couch Potato",
                        Description="Times using covenant furniture",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__CovenantGiftsReceived,
                    new StatsDetailsMap{
                        FriendlyName="The Embezzler",
                        Description="Arpeyhis received from covenant",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesAnimateTFed,
                    new StatsDetailsMap{
                        FriendlyName="What's My Form Again?",
                        Description="Times transformed into an animate form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesInanimateTFed,
                    new StatsDetailsMap{
                        FriendlyName="The Inanimated",
                        Description="Timed transformed into an inanimate form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesAnimalTFed,
                    new StatsDetailsMap{
                        FriendlyName="Nothin' But a Hound Dog",
                        Description="Times transformed into a pet form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesAnimateTFing,
                    new StatsDetailsMap{
                        FriendlyName="The Animator",
                        Description="Targets transformed into an animate form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesInanimateTFing,
                    new StatsDetailsMap{
                        FriendlyName="The Inanimator",
                        Description="Targets transformed into an inanimate form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesAnimalTFing,
                    new StatsDetailsMap{
                        FriendlyName="Petmaker",
                        Description="Targets transformed into a pet form",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__PsychopathsDefeated,
                    new StatsDetailsMap{
                        FriendlyName="Psycho Hunter",
                        Description="Psychopathic Spellslingers defeated",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__TimesTeleported_Scroll,
                    new StatsDetailsMap{
                        FriendlyName="Fast Commute",
                        Description="Teleport scrolls used",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__JewdewfaeEncountersCompleted,
                    new StatsDetailsMap{
                        FriendlyName="Friend of the Fae",
                        Description="Times played with Jewdewfae",
                        ImageUrl="trophy.jpg",
                        }
                },

                 {
                Stat__LoreBooksRead,
                    new StatsDetailsMap{
                        FriendlyName="Nerrrrrd!",
                        Description="Tomes read",
                        ImageUrl="trophy.jpg",
                        }
                },

                {
                Stat__InanimateXPEarned,
                    new StatsDetailsMap{
                        FriendlyName="Rub Rub Rub",
                        Description="XP gained as an inanimate item",
                        ImageUrl="trophy.jpg",
                        }
                },

                {
                Stat__PetXPEarned,
                    new StatsDetailsMap{
                        FriendlyName="Petter off this Way",
                        Description="XP gained as an pet.",
                        ImageUrl="trophy.jpg",
                        }
                },

                {
                Stat__LindellaCostsAmount,
                    new StatsDetailsMap{
                        FriendlyName="Shop 'Til You Drop",
                        Description="Arpeyjis spent buying from Lindella",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__LindellaProfitsAmount,
                    new StatsDetailsMap{
                        FriendlyName="Soul Item Business Sense",
                        Description="Arpeyjis earned selling to Lindella",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__WuffieCostsAmount,
                    new StatsDetailsMap{
                        FriendlyName="Puppy Miller",
                        Description="Arpeyjis spent buying from Wuffie",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__WuffieProfitsAmount,
                    new StatsDetailsMap{
                        FriendlyName="Puppy Power!",
                        Description="Most Arpeyjis earned selling to Lindella",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__DungeonArtifactsFound,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Looter",
                        Description="Dungeon artifacts found",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__DungeonDemonsDefeated,
                    new StatsDetailsMap{
                        FriendlyName="Demonslayer",
                        Description="Dungeon demons defeated",
                        ImageUrl="trophy.jpg",
                        }
                },

                   {
                Stat__DungeonPointsStolen,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Assassin",
                        Description="Dungeon points stolen from victims",
                        ImageUrl="trophy.jpg",
                        }
                },

                {
                Stat__DungeonMovements,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Crawler",
                        Description="Times moved in the dungeon.",
                        ImageUrl="trophy.jpg",
                        }
                },
                {
                Stat__SuccessfulStruggles,
                    new StatsDetailsMap{
                        FriendlyName="Freeeeedom!",
                        Description="Times struggled back to an animate form",
                        ImageUrl="trophy.jpg",
                        }
                },
                {
                Stat__TransmogsUsed,
                    new StatsDetailsMap{
                        FriendlyName="Maximally Transmogrified",
                        Description="Autoselftransmogrification Deflector Devices used",
                        ImageUrl="trophy.jpg",
                        }
                },
                {
                Stat__CovenantCallbackCrystalsUsed,
                    new StatsDetailsMap{
                        FriendlyName="Call of the Covenant",
                        Description="Covenant Callback Crystals used",
                        ImageUrl="trophy.jpg",
                        }
                },
                {
                Stat__DollsWPRestored,
                    new StatsDetailsMap{
                        FriendlyName="Dollyamory",
                        Description="Willpower restored from using Inflatable Sex Dolls",
                        ImageUrl="trophy.jpg",
                        }
                },




            };


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

        public static IEnumerable<PlayerAchievementViewModel> GetLeaderPlayersInStat(string type)
        {
            IAchievementRepository repo = new EFAchievementRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            List<Achievement> x = repo.Achievements.Where(a => a.AchievementType == type).OrderByDescending(s => s.Amount).ThenByDescending(a => a.Timestamp).Take(10).ToList();
            List<PlayerAchievementViewModel> output = new List<PlayerAchievementViewModel>();

            foreach (Achievement a in x)
            {
                if (a != null)
                {
                    PlayerAchievementViewModel addMe = new PlayerAchievementViewModel
                    {
                        Player = PlayerProcedures.GetPlayerFormViewModel_FromMembership(a.OwnerMembershipId),
                        Achivement = a,
                    };
                    output.Add(addMe);
                }
            }
            return output;

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

            foreach (Achievement t in types)
            {
                Achievement a = repo.Achievements.Where(b => b.AchievementType == t.AchievementType).OrderByDescending(b => b.Amount).FirstOrDefault();

                if (a != null)
                {
                    PlayerAchievementViewModel addMe = new PlayerAchievementViewModel
                    {
                        Player = PlayerProcedures.GetPlayerFormViewModel_FromMembership(a.OwnerMembershipId),
                        Achivement = a,
                    };
                    output.Add(addMe);
                }

            }

            return output;

        }

        public static string AssignLeadersBadges()
        {
            IAchievementBadgeRepository badgeRepo = new EFAchievementBadgeRepository();

            List<PlayerAchievementViewModel> winners = GetPlayerMaxStats().ToList();
            string output = "";

            string round = PvPStatics.AlphaRound;

            foreach (PlayerAchievementViewModel a in winners)
            {
                AchievementBadge badge = badgeRepo.AchievementBadges.FirstOrDefault(b => b.AchievementType == a.Achivement.AchievementType && b.OwnerMembershipId == a.Player.Player.MembershipId && b.Round == round);

                string nextline = "<b>" + a.Achivement.AchievementType + "</b> for round <b>" + round + "</b> being assigned to <b>" + a.Player.Player.GetFullName() + "</b> of ID " + a.Player.Player.MembershipId + ".  ";

                if (badge == null)
                {
                    badge = new AchievementBadge
                    {
                        OwnerMembershipId = a.Player.Player.MembershipId,
                        Round = round,
                    };
                    nextline += "No existing badge found.  Making new one.";
                }
                else
                {
                    nextline += "EXISTING BADGE FOUND.  Updating.";
                }

                badge.Amount = a.Achivement.Amount;
                badge.AchievementType = a.Achivement.AchievementType;

                badgeRepo.SaveAchievementBadge(badge);
                output += nextline + "<br><br>";

            }

            return output;

        }

        public static IEnumerable<AchievementBadge> GetPlayerBadges(string membershipId)
        {
            IAchievementBadgeRepository repo = new EFAchievementBadgeRepository();

            return repo.AchievementBadges.Where(b => b.OwnerMembershipId == membershipId);
        }
    }
}