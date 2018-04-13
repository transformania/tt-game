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

    public class StatsDetailsMap
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public bool ResetsOnReroll { get; set; }

    }

    public static class StatsProcedures
    {

        public const string Stat__SearchCount = "times_searched";
        public const string Stat__SpellsCast = "spells_cast";
        public const string Stat__TimesMoved = "times_moved";
        public const string Stat__TimesCleansed = "times_cleansed";
        public const string Stat__TimesMeditated = "times_meditated";
        public const string Stat__TimesEnchanted = "times_enchanted";

        // Covenants
        public const string Stat__CovenantDonationTotal = "covenant_donations";
        public const string Stat__CovenantGiftsReceived = "covenant_gif_receieved";
        public const string Stat__CovenantFurnitureUsed = "covenant_furniture_used";
        public const string Stat__CovenantNetDonation = "covenant_net_donations";

        // Combat
        public const string Stat__TimesAnimateTFed = "times_self_animated";
        public const string Stat__TimesInanimateTFed = "times_self_inanimated";
        public const string Stat__TimesAnimalTFed = "times_self_animal";

        public const string Stat__TimesAnimateTFing = "times_self_animating";
        public const string Stat__TimesInanimateTFing = "times_self_inanimating";
        public const string Stat__TimesAnimalTFing = "times_self_animaling";

        public const string Stat__PsychopathsDefeated = "psychos_defeated";

        // Items Used
        public const string Stat__TimesTeleported_Scroll = "times_teleported_scroll";
        public const string Stat__TransmogsUsed = "transmogs_used";
        public const string Stat__CovenantCallbackCrystalsUsed = "callback_crystals_used";
        public const string Stat__DollsWPRestored = "dolls_used";
        public const string Stat__LoreBooksRead = "lore_books_read";
        public const string Stat__TgOrbVictims = "tg_bomb_hits";

        // Inanimate/Pet actions
        public const string Stat__InanimateXPEarned = "inanimateXPEarned";
        public const string Stat__PetXPEarned = "petXPEarned";

        // NPCs
        public const string Stat__LindellaCostsAmount = "lindella_costs_amount"; // RETIRED
        public const string Stat__LindellaProfitsAmount = "lindella_profit_amount"; // RETIRED
        public const string Stat__LindellaNetProfit = "lindella_net_profit";
        public const string Stat__LindellaNetLoss = "lindella_net_loss";

        public const string Stat__WuffieCostsAmount = "wuffie_costs_amount"; // RETIRED
        public const string Stat__WuffieProfitsAmount = "wuffie_profit_amount"; // RETIRED
        public const string Stat__WuffieNetProfit = "wuffie_net_profit";
        public const string Stat__WuffieNetLoss = "wuffie_net_loss";

        public const string Stat__LorekeeperSpellsLearned = "lorekeeper_spells_learned";

        public const string Stat__JewdewfaeEncountersCompleted = "jewdewfae_completions";

        // PvP
        public const string Stat__PvPPlayerNumberTakedowns = "pvp_player_takedowns";
        public const string Stat__PvPPlayerLevelTakedowns = "pvp_total_level_takedowns";

        // bosses
        public const string Stat__BossAllAttacks = "boss_totalBossAttacks";
        public const string Stat__BossRatThiefAttacks = "boss_ratThiefAttacks";
        public const string Stat__BossLovebringerAttacks = "boss_lovebringerAttacks";
        public const string Stat__BossDonnaAttacks = "boss_donnaAttacks";
        public const string Stat__FaebossAttacks = "boss_faebossAttacks";
        public const string Stat__MouseSisterAttacks = "boss_mouseSisterAttacks";

        // Dungeon
        public const string Stat__DungeonArtifactsFound = "dungeon_artifacts_found";
        public const string Stat__DungeonDemonsDefeated = "dungeon_demons_defeated";
        public const string Stat__DungeonPointsStolen = "dungeon_points_stolen";
        public const string Stat__DungeonMovements = "dungeon_movements";

        public const string Stat__SuccessfulStruggles = "times_struggled_free";

        // Quests
        public const string Stat__QuestsFailed = "quests_failed";
        public const string Stat__QuestsPassed = "quests_passed";

        // Buses
        public const string Stat__BusRides = "busrides";

        public static Dictionary<string, StatsDetailsMap> StatTypesMap = new Dictionary<string, StatsDetailsMap> {
           
        {
                Stat__SearchCount,
                    new StatsDetailsMap{
                        FriendlyName = "Hawkeye",
                        Description="Times searched",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                    
                    },

                {
                Stat__SpellsCast,
                    new StatsDetailsMap{
                        FriendlyName = "Quantity not Quality!",
                        Description="Spells cast",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },
                
                {
                Stat__TimesMoved,
                    new StatsDetailsMap{
                        FriendlyName = "The Restless One",
                        Description="Times moved",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesCleansed,
                    new StatsDetailsMap{
                        FriendlyName = "Hypochondriac",
                        Description="Times cleansed",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                  {
                Stat__TimesMeditated,
                    new StatsDetailsMap{
                        FriendlyName="Lost in Thought",
                        Description="Times meditated",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesEnchanted,
                    new StatsDetailsMap{
                        FriendlyName="____ the Enchanter",
                        Description="Times enchanting",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 // -------- COVENANT STUFF -------------

                  {
                Stat__CovenantNetDonation,
                    new StatsDetailsMap{
                        FriendlyName = "Covenent Benefactor",
                        Description="Net Arpyjis donated to covenant",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__CovenantGiftsReceived,  // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="The Embezzler",
                        Description="Arpeyhis received from covenant",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },

                 {
                Stat__CovenantDonationTotal, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName = "Fundraiser Fanatic",
                        Description="Arpyjis donated to covenant",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },
                  {
                Stat__CovenantFurnitureUsed,
                    new StatsDetailsMap{
                        FriendlyName = "Couch Potato",
                        Description="Times using covenant furniture",
                        ImageUrl="couch_potato_Ninian_Fae.jpg",
                        Active = true
                        }
                },

                

                 {
                Stat__TimesAnimateTFed,
                    new StatsDetailsMap{
                        FriendlyName="What's My Form Again?",
                        Description="Times transformed into an animate form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesInanimateTFed,
                    new StatsDetailsMap{
                        FriendlyName="The Inanimated",
                        Description="Times transformed into an inanimate form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesAnimalTFed,
                    new StatsDetailsMap{
                        FriendlyName="Nothin' But a Hound Dog",
                        Description="Times transformed into a pet form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesAnimateTFing,
                    new StatsDetailsMap{
                        FriendlyName="The Animator",
                        Description="Targets transformed into an animate form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesInanimateTFing,
                    new StatsDetailsMap{
                        FriendlyName="The Inanimator",
                        Description="Targets transformed into an inanimate form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesAnimalTFing,
                    new StatsDetailsMap{
                        FriendlyName="Petmaker",
                        Description="Targets transformed into a pet form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__PsychopathsDefeated,
                    new StatsDetailsMap{
                        FriendlyName="Psycho Hunter",
                        Description="Psychopathic Spellslingers defeated",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__TimesTeleported_Scroll,
                    new StatsDetailsMap{
                        FriendlyName="Fast Commute",
                        Description="Teleport scrolls used",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                 {
                Stat__JewdewfaeEncountersCompleted,
                    new StatsDetailsMap{
                        FriendlyName="Friend of the Fae",
                        Description="Times played with Jewdewfae",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = true
                        }
                },

                 {
                Stat__LoreBooksRead,
                    new StatsDetailsMap{
                        FriendlyName="Nerrrrrd!",
                        Description="Tomes read",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = true
                        }
                },

                {
                Stat__InanimateXPEarned,
                    new StatsDetailsMap{
                        FriendlyName="Rub Rub Rub",
                        Description="XP gained as an inanimate item",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__PetXPEarned,
                    new StatsDetailsMap{
                        FriendlyName="Petter off this Way",
                        Description="XP gained as an pet.",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__LindellaNetProfit,
                    new StatsDetailsMap{
                        FriendlyName="Soul Seller",
                        Description="Net profit from selling to Lindella",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__LindellaNetLoss,
                    new StatsDetailsMap{
                        FriendlyName="Soul Consumer",
                        Description="Net loss from buying from Lindella",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__LindellaCostsAmount, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="Shop 'Til You Drop",
                        Description="Arpeyjis spent buying from Lindella",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },

                {
                Stat__LindellaProfitsAmount, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="Soul Item Business Sense",
                        Description="Arpeyjis earned selling to Lindella",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },

                {
                Stat__WuffieNetProfit,
                    new StatsDetailsMap{
                        FriendlyName="Puppy Profit!",
                        Description="Profit from selling to Wuffie",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__WuffieNetLoss,
                    new StatsDetailsMap{
                        FriendlyName="Puppy Purchaser",
                        Description="Loss from buying from Wuffie",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__WuffieCostsAmount, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="Puppy Miller",
                        Description="Arpeyjis spent buying from Wuffie",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },

                {
                Stat__WuffieProfitsAmount, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="Puppy Power!",
                        Description="Most Arpeyjis earned selling to Wuffie",
                        ImageUrl="trophy.jpg",
                        Active = false
                        }
                },

                {
                Stat__LorekeeperSpellsLearned, // RETIRED
                    new StatsDetailsMap{
                        FriendlyName="Exiled's Apprentice",
                        Description="Most spells learned from Skaldrlyr the Forbidden",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__DungeonArtifactsFound,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Looter",
                        Description="Dungeon artifacts found",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__DungeonDemonsDefeated,
                    new StatsDetailsMap{
                        FriendlyName="Demonslayer",
                        Description="Dungeon demons defeated",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__DungeonPointsStolen,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Assassin",
                        Description="Dungeon points stolen from victims",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__DungeonMovements,
                    new StatsDetailsMap{
                        FriendlyName="Dungeon Crawler",
                        Description="Times moved in the dungeon.",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                {
                Stat__SuccessfulStruggles,
                    new StatsDetailsMap{
                        FriendlyName="Freeeeedom!",
                        Description="Times struggled back to an animate form",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },
                {
                Stat__TransmogsUsed,
                    new StatsDetailsMap{
                        FriendlyName="Maximally Transmogrified",
                        Description="Autoselftransmogrification Deflector Devices used",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },
                {
                Stat__CovenantCallbackCrystalsUsed,
                    new StatsDetailsMap{
                        FriendlyName="Call of the Covenant",
                        Description="Covenant Callback Crystals used",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },
                {
                Stat__DollsWPRestored,
                    new StatsDetailsMap{
                        FriendlyName="Dollyamory",
                        Description="Willpower restored from using Inflatable Sex Dolls",
                        ImageUrl="trophy.jpg",
                        Active = true
                        }
                },

                // Quests
                {
                Stat__QuestsFailed,
                    new StatsDetailsMap{
                        FriendlyName="Try Staying Home Today",
                        Description="Quests failed",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = true
                        }
                },

                {
                Stat__QuestsPassed,
                    new StatsDetailsMap{
                        FriendlyName="Adventure Time!",
                        Description="Quests passed",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = true
                        }
                },

                {
                Stat__BusRides,
                    new StatsDetailsMap{
                        FriendlyName="The Wheels on the Bus",
                        Description="Total distance riding the bus",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = false
                        }
                },

                {
                Stat__TgOrbVictims,
                    new StatsDetailsMap{
                        FriendlyName="Sex Swap Scoundrel",
                        Description="Total targets hit with TG Splash Orbs",
                        ImageUrl="trophy.jpg",
                        Active = true,
                        ResetsOnReroll = false
                        }
                },

            {
                Stat__PvPPlayerNumberTakedowns,
                new StatsDetailsMap{
                    FriendlyName="Player Transformayer",
                    Description="Total PvP-PvP players transformed into items or pets",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__PvPPlayerLevelTakedowns,
                new StatsDetailsMap{
                    FriendlyName="Level Shmevel",
                    Description="Sum of all PvP-PvP levels of players transformed into items or pets",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__BossAllAttacks,
                new StatsDetailsMap{
                    FriendlyName="You Aren't the Boss of Me!",
                    Description="Times attacking boss NPCs",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__BossRatThiefAttacks,
                new StatsDetailsMap{
                    FriendlyName="I Am the Law!",
                    Description="Times attacking the Seekshadow thieves",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__BossLovebringerAttacks,
                new StatsDetailsMap{
                    FriendlyName="A.B.S Agent-In-Training",
                    Description="Times attacking the Lady Lovebringer, PHD",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__BossDonnaAttacks,
                new StatsDetailsMap{
                    FriendlyName="Down With Donna!",
                    Description="Times attacking 'Aunt' Donna Milton",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__FaebossAttacks,
                new StatsDetailsMap{
                    FriendlyName="Fae-Away",
                    Description="Times attacking Narcissa the Corrupted Lunar Fae",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            {
                Stat__MouseSisterAttacks,
                new StatsDetailsMap{
                    FriendlyName="Brisby Battler",
                    Description="Times attacking either of the Brisby twins",
                    ImageUrl="trophy.jpg",
                    Active = true,
                    ResetsOnReroll = true
                }
            },

            };


        public static void AddStat(string membershipId, string type, float amount)
        {

            // don't do anything if the achivement is marked as inactive
            if (!StatsProcedures.StatTypesMap[type].Active)
            {
                return;
            }

            // don't keep stats for an AI characters
            if (PlayerProcedures.GetPlayerFromMembership(membershipId).BotId == AIStatics.ActivePlayerBotId)
            {
                IAchievementRepository repo = new EFAchievementRepository();
                var x = repo.Achievements.FirstOrDefault(a => a.OwnerMembershipId == membershipId && a.AchievementType == type);

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

        public static IEnumerable<PlayerAchievementViewModel> GetLeaderPlayersInStat(string type)
        {
            IAchievementRepository repo = new EFAchievementRepository();

            var x = repo.Achievements.Where(a => a.AchievementType == type).OrderByDescending(s => s.Amount).ThenBy(a => a.Timestamp).Take(10).ToList();
            var output = new List<PlayerAchievementViewModel>();

            foreach (var a in x)
            {
                if (a != null)
                {
                    var addMe = new PlayerAchievementViewModel
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

            IEnumerable<Achievement> types = repo.Achievements.GroupBy(a => a.AchievementType).Select(grp => grp.FirstOrDefault()).ToList();

            var output = new List<PlayerAchievementViewModel>();

            foreach (var t in types)
            {
                var a = repo.Achievements.Where(b => b.AchievementType == t.AchievementType).OrderByDescending(b => b.Amount).ThenBy(b => b.Timestamp).FirstOrDefault();

                if (a != null)
                {
                    var addMe = new PlayerAchievementViewModel
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

            var winners = GetPlayerMaxStats().ToList();
            var output = "";

            var round = PvPStatics.AlphaRound;

            foreach (var a in winners)
            {
                var badge = badgeRepo.AchievementBadges.FirstOrDefault(b => b.AchievementType == a.Achivement.AchievementType && b.OwnerMembershipId == a.Player.Player.MembershipId && b.Round == round);

                var nextline = "<b>" + a.Achivement.AchievementType + "</b> for round <b>" + round + "</b> being assigned to <b>" + a.Player.Player.GetFullName() + "</b> of ID " + a.Player.Player.MembershipId + ".  ";

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

        /// <summary>
        /// Deletes all achivements for given types owned by a particular player
        /// </summary>
        /// <param name="player">Player whose achivements should be removed</param>
        /// <param name="achivements">The names of the achivements owned by the player to be removed</param>
        public static void DeleteAchivemenstOfTypeForPlayer(Player player, List<string> achivements)
        {
            IAchievementRepository repo = new EFAchievementRepository();
            var dbAchivements = repo.Achievements.Where(a => a.OwnerMembershipId == player.MembershipId && achivements.Contains(a.AchievementType)).ToList();

            foreach (var a in dbAchivements)
            {
                repo.DeleteAchievement(a.Id);
            }

        }

        public static List<string> GetAchivementNamesThatReset()
        {
            return StatTypesMap.Where(d => d.Value.ResetsOnReroll).Select(a => a.Key).ToList();
        }
    }
}