using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.ViewModels;

namespace TT.Domain.Statics
{
    using TagEnum = PlayerDescriptorStatics.TagBehavior;
    using RoleEnum = PlayerDescriptorStatics.Role;

    public static class PvPStatics
    {

        public static int RoundDuration = 4000;

        public const int TurnSecondLength = 300;
        public const int StartTurnNoAttackSeconds = 30;
        public const int EndTurnNoAttackSeconds = 30;

        public static string AlphaRound = "Alpha Round 39";
        public const string LastTosUpdate = "February 15, 2017";

        public static bool ChaosMode = false;

        public const string GenderMale = "male";
        public const string GenderFemale = "female";

        public const string MobilityFull = "full";
        public const string MobilityInanimate = "inanimate";
        public const string MobilityPet = "animal";
        public const string MobilityMindControl = "mindcontrol";

        // These three are publicly modified statics, not constants
        public static string LastGameUpdate = "";
        public static int LastGameTurn = 0;
        public static bool AnimateUpdateInProgress = false;

        public const int MinutesToDroppedItemDelete = 20;
        public const int HoursBeforeInanimatesCanSlipFree = 24;

        public const decimal SneakAmountBeforeLocationIsPublic = -25;

        public const int MaxLogMessagesPerLocation = 20;
        public const int MaxLogMessagesPerPlayer = 50;

        public const decimal XP__GainPerAttackBase = 5;
        public const decimal XP__AnimateTFXPBonusModifier = 1.5M;
        public const decimal XP__LevelDifferenceXPGainModifier = 1.5M;
        public const decimal XP__EndgameTFCompletionLevelBase = 8;
        public const decimal ExtraHealthDamagePerLevel = 0.75M;
        public const decimal XP__EnchantmentMaxXP = 3;

        public const double OfflineAfterXMinutes = 30;
        public const int DuelNoCombatMinutes = 15;
        public const int MaximumDuelTurnLength = 10;

        public const string Permissions_Admin = "admin";
        public const string Permissions_Moderator = "moderator";
        public const string Permissions_MultiAccountWhitelist = "whitelisted";
        public const string Permissions_Proofreader = "proofreader";
        public const string Permissions_Publisher = "publisher";
        public const string Permissions_Artist = "artist";
        public const string Permissions_JSON = "json";
        public const string Permissions_Previewer = "previewer";
        public const string Permissions_Killswitcher = "killswitch";
        public const string Permissions_QuestWriter = "questwriter";
        public const string Permissions_Chaoslord = "chaoslord";

        public static readonly int[] Item_SoulActivityLevels_Minutes = {
                                                                 240,
                                                                 1440,
                                                                 4320,
                                                             };

        public static readonly int[] RerollTimer = {
            30,     // 30 minutes
            60,     // 1 hour
            240,    // 4 hours
            1440    // 24 hours
        };

        public const decimal LevelUpHealthMaxIncreaseBase = 10;
        public const decimal LevelUpManaMaxIncreaseBase = 10;
        public const decimal LevelUpHealthMaxIncreasePerLevel = 10;
        public const decimal LevelUpManaMaxIncreasePerLevel = 5;

        public const decimal Item_LevelBonusModifier = .1M;
        public const int ItemTurnBuildupMaximum = 48;

        public const int MaxAttacksPerUpdate = 3;
        public const int NoMovingAfterAttackSeconds = 45;
        public const decimal LocationMoveCost = 1.0M;
        public const decimal AttackCost = 3.0M;
        public const decimal MeditateCost = 4.0M;
        public const decimal CleanseCost = 4.0M;

        public const float SelfRestoreAPCost = 3;
        public const float SelfRestoreManaCost = 10;
        public const float SelfRestoreTFnergyRequirement = 100;
        public const float SelfRestoreBaseTFEnergyPerCast = 10;

        public const decimal CleanseManaCost = 3M;
        public const decimal CleanseTFEnergyPercentDecrease = 2.0M;
        public const decimal MeditateManaRestoreBase = 10.0M;
        public const decimal CleanseHealthRestoreBase = 7.0M;
        public const decimal SearchAPCost = 4.0M;

        public const int MaxCarryableItemCountBase = 6;
        public const int MaxCleansesMeditatesPerUpdate = 3;

        public const decimal MaximumStoreableActionPoints = 120;
        public const decimal MaximumStoreableActionPoints_Refill = 360;
        public const decimal APRestoredPerUpdate = 10;
        public const decimal APRestoredPerUpdateRefillBonus = 20;

        public const decimal CriticalMissPercentChance = 8;
        public const decimal CriticalHitPercentChance = 8;

        public const string NoActionPointsErrorMessage = "Wait a while; you will receive more action points every half hour.";


        public const decimal PercentHealthToAllowFullMobilityFormTF = 0.50m;
        public const decimal PercentHealthToAllowMindControlTF = 0.25m;
        public const decimal PercentHealthToAllowAnimalFormTF = 0;
        public const decimal PercentHealthToAllowInanimateFormTF = 0;

        public const decimal NonPvPXPGainModifier = .5M;

        public const string ItemType_Accessory = "accessory";
        public const string ItemType_Consumable = "consumable";
        public const string ItemType_Consumable_Reuseable = "consumable_reuseable";
        public const string ItemType_Shirt = "shirt";
        public const string ItemType_Undershirt = "undershirt";
        public const string ItemType_Pants = "pants";
        public const string ItemType_Underpants = "underpants";
        public const string ItemType_Hat = "hat";
        public const string ItemType_Shoes = "shoes";
        public const string ItemType_Pet = "pet";

        public const string ItemType_DungeonArtifact = "item_consumeable_dungeon_artifact";
        public const int ItemType_DungeonArtifact_Id = 218;

        public const string Dungeon_ArtifactCurse = "effect_Dark_Transfixation_Judoo";

        public const decimal DungeonArtifact_Value = 2;
        public const int DungeonArtifact_SpawnLimit = 3;
        public const string DungeonDemon = "form_Dark_Demonic_Guardian_Judoo";
        public const int DungeonDemon_Limit = 5;

        public const string Dungeon_VanquishSpell = "skill_Vanquish_Judoo";

        public const string ItemType_TGBomb = "item_consumable_tg_bomb";


        public enum GameModes { Superprotection = 0, Protection = 1, PvP = 2}

        public const int PsychopathDefaultAmount = 15;
        public const int Covenant_MaximumAnimatePlayerCount = 25;

        public const int Covenant_MinimumUpgradeAnimateLvl3PlayerCount = 5;

        public const int LorekeeperSpellPrice = 50;

        public const string Effect_Back_On_Your_Feet = "help_animate_recovery";

        public const int PaginationPageSize = 100;

    }

    public static class BonusStatics
    {
        public const string HealthBonusPercent_Description = "Maximum willpower increase (%)";
        public const string ManaBonusPercent_Description = "Maximum mana increase (%)";
        public const string ExtraSkillCriticalPercent_Description = "Extra spell critical hit chance (%)";
        public const string HealthRecoveryPerUpdate_Description = "Willpower recovery per game update (Amt)";
        public const string ManaRecoveryPerUpdate_Description = "Mana recovery per game update (Amt)";
        public const string SneakPercent_Description = "Sneak chance (%)";
        public const string EvasionPercent_Description = "Spell evasion (%)";
        public const string EvasionNegationPercent_Description = "Spell Evasion NEGATION (%)";
        public const string MeditationExtraMana_Description = "Extra mana recovered from meditating (Amt)";
        public const string CleanseExtraHealth_Description = "Extra willpower recovered from cleansing (Amt)";
        public const string MoveActionPointDiscount_Description = "Action point discount when moving (Amt)";
        public const string SpellExtraTFEnergyPercent_Description = "Extra transformation energy from spells (%)";
        public const string SpellExtraHealthDamagePercent_Description = "Extra willpower damage from spells (%)";
        public const string CleanseExtraTFEnergyRemovalPercent_Description = "Extra transformation energy removal when cleansing (%)";
        public const string SpellMisfireChanceReduction_Description = "Misfire chance reduction (%)";
        public const string SpellHealthDamageResistance_Description = "Willpower damage reduction when hit by a spell (%)";
        public const string SpellTFEnergyDamageResistance_Description = "Transformation energy damage reduction when hit by a spell (%)";
        public const string ExtraInventorySpace_Description = "Extra inventory spaces (Amt)";
    }

    public static class MindControlStatics
    {
        public const string MindControl__Movement = "form_(MC-Movement)_Judoo";
        public const int MindControl__Movement_Limit = 2;
        public const string MindControl__Movement_DebuffEffect = "effect_Mind_Controlled_-_Movement_Judoo";

        public const string MindControl__Strip = "form_(MC-Strip)_Martiandawn";
        public const int MindControl__Strip_Limit = 1;
        public const string MindControl__Strip_DebuffEffect = "effect_Mind_Control_-_Strip_Judoo";

        public const string MindControl__Meditate = "form_(MC-Meditation)_Duhad";
        public const int MindControl__Meditate_Limit = 2;
        public const string MindControl__Meditate_DebuffEffect = "";
    }

    public static class InanimateXPStatics
    {
        public const decimal XPGainPerInanimateAction = 5M;
        public const int ItemMaxTurnsBuildup = 48;

    }

    public static class LocationsStatics
    {

        /*
        - Street: 230 Sunnyglade Drive
        */


        public const string STREET_40_EAST_9TH_AVE = "street_e9th_westof_valley";
        public const string STREET_70_EAST_9TH_AVE = "street_70e9th";

        public const string STREET_270_WEST_9TH_AVE =  "270_west_9th_ave";
        public const string STREET_320_WEST_9TH_AVE = "320_west_9th_ave";

        public const string STREET_200_MAIN_STREET = "street_01";
        public const string STREET_170_MAIN_STREET = "street_8th";
        public const string STREET_140_MAIN_STREET = "street_140_main";

        public const string STREET_130_SUNNYGLADE_DRIVE = "street_130_sunnyglade_drive";
        public const string STREET_160_SUNNYGLADE_DRIVE = "street_14th_east";
        public const string STREET_200_SUNNYGLADE_DRIVE = "street_200_sunnyglade_drive";
        public const string STREET_230_SUNNYGLADE_DRIVE = "street_230_sunnyglade_drive";

        public static readonly string[] BusStops =
        {
            STREET_40_EAST_9TH_AVE,
            STREET_70_EAST_9TH_AVE,
            STREET_270_WEST_9TH_AVE,
            STREET_320_WEST_9TH_AVE,
            STREET_140_MAIN_STREET,
            STREET_170_MAIN_STREET,
            STREET_200_MAIN_STREET,
            STREET_130_SUNNYGLADE_DRIVE,
            STREET_160_SUNNYGLADE_DRIVE,
            STREET_200_SUNNYGLADE_DRIVE,
            STREET_230_SUNNYGLADE_DRIVE
        };

        /// <summary>
        /// Returns the price of a bus ticket to get from one location to another.  Currently this price is merely the 1x delta X plus delta Y.
        /// </summary>
        /// <param name="departure">Location the player is leaving from</param>
        /// <param name="destination">Location the player is arriving to</param>
        /// <returns></returns>
        public static int GetTicketPriceBetweenLocations(string departure, string destination)
        {
            return 2*GetDistanceBetweenLocations(departure, destination);
        }

        /// <summary>
        /// Returns the distance between two locations, calculated as the delta of X plus the delta of Y
        /// </summary>
        /// <param name="departure">Location the player is leaving from</param>
        /// <param name="destination">Location the player is arriving to</param>
        /// <returns></returns>
        public static int GetDistanceBetweenLocations(string departure, string destination)
        {
            Location departureLocation = LocationList.GetLocation.First(l => l.dbName == departure);
            Location destinationLocation = LocationList.GetLocation.First(l => l.dbName == destination);

            return Math.Abs(departureLocation.X - destinationLocation.X) + Math.Abs(departureLocation.Y - destinationLocation.Y);
        }

        public static string GetRandomLocation()
        {
            // set a random location for this character to spawn in
            List<string> spawnableLocations = LocationList.GetLocation.Where(s => s.Region != "dungeon").Select(l => l.dbName).ToList();
            double max = spawnableLocations.Count();
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string locationToSpawnIn = spawnableLocations.ElementAt(index);
            return locationToSpawnIn;
        }

        public static string GetRandomLocation_NoStreets()
        {
            // set a random location for this character to spawn in
            List<string> spawnableLocations = LocationList.GetLocation.Where(l => l.Region != "streets" && l.Region != "dungeon").Select(l => l.dbName).ToList();
            double max = spawnableLocations.Count();
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string locationToSpawnIn = spawnableLocations.ElementAt(index);
            return locationToSpawnIn;
        }

        public static string GetRandomLocation_InDungeon()
        {
            // set a random location for this character to spawn in
            List<string> spawnableLocations = LocationList.GetLocation.Where(l => l.Region == "dungeon").Select(l => l.dbName).ToList();
            double max = spawnableLocations.Count();


            // someone wants to enter the dungeon but it hasn't been generated yet; build it now
            if (!spawnableLocations.Any())
            {
                DungeonProcedures.GenerateDungeon();
                spawnableLocations = LocationList.GetLocation.Where(l => l.Region == "dungeon").Select(l => l.dbName).ToList();
                max = spawnableLocations.Count();
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string locationToSpawnIn = spawnableLocations.ElementAt(index);
            return locationToSpawnIn;
        }

        public static string GetRandomLocation_InRegion(string region)
        {
            // set a random location for this character to spawn in
            List<string> spawnableLocations = LocationList.GetLocation.Where(l => l.Region == region).Select(l => l.dbName).ToList();
            double max = spawnableLocations.Count();
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string locationToSpawnIn = spawnableLocations.ElementAt(index);
            return locationToSpawnIn;
        }



        public static class LocationList
        {

            public static List<Location> GetLocation;

              public static void AddLocation(Location location) {
                GetLocation.Add(location);
            }

            static LocationList() {
                GetLocation = new List<Location>{


          

           

            new Location {
                dbName = "coffee_shop",
                Name = "Carolyne's Coffee Shop (Front Counter)",
                Region = "coffee_shop",
                X = 0,
                Y = 0,
                IsSafe = false,
                ImageUrl = "coffee_shop",
                Name_North = "coffee_shop_patio",
                Name_East = "coffee_shop_kitchen"
            }, new Location {
                dbName = "coffee_shop_patio",
                Name = "Carolyne's Coffee Shop (Patio)",
                Region = "coffee_shop",
                X = 0,
                Y = 1,
                IsSafe = false,
                ImageUrl = "coffee_shop_patio",
                Name_South = "coffee_shop",
                Name_West = "street_01"

            }, new Location {
                dbName = "street_01",
                Name = "Street: 200 Main Street",
                Region = "streets",
                X = -1,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "coffee_shop_patio",
                Name_West = "bookstore_front",
                Name_North = "street_02",
                Name_South = "street_03",

            },  new Location {
                dbName = "coffee_shop_kitchen",
                Name = "Carolyne's Coffee Shop (Kitchen)",
                Region = "coffee_shop",
                X = 1,
                Y = 0,
                IsSafe = false,
                ImageUrl = "coffee_shop_kitchen",
                Name_West = "coffee_shop"

            }, new Location {
                dbName = "bookstore_front",
                Name = "Words of Wisdom Bookstore (Front)",
                Region = "bookstore",
                X = -2,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "street_01",
                Name_West = "bookstore_back"


            } , new Location {
                dbName = "bookstore_back",
                Name = "Words of Wisdom Bookstore (Back)",
                Region = "bookstore",
                X = -3,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "bookstore_front",


            }, new Location {
                dbName = "street_02",
                Name = "Street: 210 Main Street",
                Region = "streets",
                X = -1,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "street_01",
                Name_North = "street_13th",
                Name_West = "apartment_rental_office",

            }, new Location {
                dbName = "street_13th",
                Name = "Street: 220 Main Street",
                Region = "streets",
                X = -1,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "street_02",
                Name_North = "street_14th",
                Name_East = "park_entrance"

            }, new Location {
                dbName = "park_entrance",
                Name = "Sunnyglade Park (Entrance)",
                Region = "park",
                X = 0,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "street_13th",
                Name_East = "park_fountain"

            }, new Location {
                dbName = "park_fountain",
                Name = "Sunnyglade Park (Memorial Fountain)",
                Region = "park",
                X = 1,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "park_entrance",
                Name_North = "park_merrygoround",
                Name_South = "park_rose_garden",
                Name_East = "park_tennis",

            }, new Location {
                dbName = "park_merrygoround",
                Name = "Sunnyglade Park (Carousel)",
                Region = "park",
                X = 1,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "park_fountain",
                Name_West="park_duck_pond",
                Name_East="park_shrine",

            }, new Location {
                dbName = "park_rose_garden",
                Name = "Sunnyglade Park (Rose Garden)",
                Region = "park",
                X = 1,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "park_fountain",
                Name_East = "park_toolshed",
                Name_West = "park_boardwalk",

            }, new Location {
                dbName = "park_tennis",
                Name = "Sunnyglade Park (Tennis Courts)",
                Region = "park",
                X = 2,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "park_fountain",

            
             }, new Location {
                dbName = "park_duck_pond",
                Name = "Sunnyglade Park (Duck Pond)",
                Region = "park",
                X = 0,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="park_merrygoround"
         

            }, new Location {
                dbName = "park_shrine",
                Name = "Sunnyglade Park (Strange Shrine)",
                Region = "park",
                X = 2,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_West= "park_merrygoround",
         

           }, new Location {
                dbName = "park_toolshed",
                Name = "Sunnyglade Park (Old Tool Shed)",
                Region = "park",
                X = 2,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "park_rose_garden",

            },

            new Location {
                dbName = "park_boardwalk",
                Name = "Sunnyglade Park (Boardwalk)",
                Region = "park",
                X = 0,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "park_rose_garden",

            },
            
            new Location {
                dbName = "street_14th",
                Name = "Street: 230 Main Street",
                Region = "streets",
                X = -1,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "street_13th",
                Name_North = "street_14th_north"


            }, new Location {
                dbName = "street_03",
                Name = "Street: 190 Main Street",
                Region = "streets",
                X = -1,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_01",
                Name_South = "street_9th",
                Name_West = "350_west_9th_ave",


            }, new Location {
                dbName = "street_9th",
                Name = "Street: Main Street and E. 9th Avenue Intersection",
                Region = "streets",
                X = -1,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_03",
                Name_South = "street_8th",
                Name_West = "gas_station_pumps",
                Name_East = "street_e9th_eastof_main",

            }, new Location {
                dbName = "street_8th",
                Name = "Street: 170 Main Street",
                Region = "streets",
                X = -1,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_9th",
                Name_East = "record_store_front",
                Name_West = "gas_station_parking",
                Name_South="street_7th_main"

            }, new Location {
                dbName = "record_store_front",
                Name = "Needles and Scratches (Front Counter)",
                Region = "record_store",
                X = 0,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "record_store_usedInstruments",
                Name_West = "street_8th",

            }, new Location {
                dbName = "record_store_usedInstruments",
                Name = "Needles and Scratches (Used Instruments Room)",
                Region = "record_store",
                X = 1,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "record_store_front",

            }, new Location {
                dbName = "gas_station_parking",
                Name = "Pump-N-Dash Gas Station (Parking Lot)",
                Region = "gas_station",
                X = -2,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "street_8th",
                Name_North = "gas_station_pumps",
                Name_West = "gas_station_carwash",

            }, new Location {
                dbName = "gas_station_pumps",
                Name = "Pump-N-Dash Gas Station (Pumps)",
                Region = "gas_station",
                X = -2,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "street_9th",
                Name_West = "gas_station_counter",
                Name_South = "gas_station_parking",

            }, new Location {
                dbName = "gas_station_counter",
                Name = "Pump-N-Dash Gas Station  (Front Counter)",
                Region = "gas_station",
                X = -3,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "gas_station_pumps",
                Name_South = "gas_station_carwash"

            }, new Location {
                dbName = "gas_station_carwash",
                Name = "Pump-N-Dash  (Car Wash)",
                Region = "gas_station",
                X = -3,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "gas_station_parking",
                Name_North =  "gas_station_counter"

            
            
            }, new Location {
                dbName = "street_7th_main",
                Name = "Street: 160 Main Street",
                Region = "streets",
                X = -1,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_8th",
                Name_South = "street_6th_main",
                Name_West="pool_entrance",
                 },

              new Location {
                dbName = "street_6th_main",
                Name = "Street: 150 Main Street",
                Region = "streets",
                X = -1,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_7th_main",
                Name_East = "clothing_front",
                Name_South = "street_140_main",
            
            },  new Location {
                dbName = "clothing_front",
                Name = "Ophalia's Embroidery (Front Counter)",
                Region = "clothing",
                X = 0,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_West="street_6th_main",
                Name_North = "clothing_mens",
                Name_East="clothing_womens",
            
            },  new Location {
                dbName = "clothing_womens",
                Name = "Ophalia's Embroidery (Women's Section)",
                Region = "clothing",
                X = 1,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_West="clothing_front",
                Name_North="clothing_intimate"
            
            },  new Location {
                dbName = "clothing_intimate",
                Name = "Ophalia's Embroidery (Intimates Section)",
                Region = "clothing",
                X = 1,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_South="clothing_womens",
                Name_West="clothing_mens",
            
            },  new Location {
                dbName = "clothing_mens",
                Name = "Ophalia's Embroidery (Men's Section)",
                Region = "clothing",
                X = 0,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_East="clothing_intimate",
                Name_South="clothing_front",
            
            }, new Location {
                dbName = "street_14th_north",
                Name = "Street: Main Street and Sunnyglade Drive Intersection",
                Region="streets",
                X = -1,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_South="street_14th",
                Name_North="street_15th_south",
                Name_East="street_14th_east",
                Name_West = "street_140_sunnyglade_drive",
            
            }, new Location {
                dbName = "street_15th_south",
                Name = "Street: 250 Main Street",
                Region="streets",
                X = -1,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_South="street_14th_north",
                Name_East="tavern_counter",
            }, 

             new Location {
                dbName = "street_14th_east",
                Name = "Street: 160 Sunnyglade Drive",
                Region="streets",
                X = 0,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_North="tavern_counter",
                Name_West="street_14th_north",
                Name_East="street_170_sunnyglade_drive",

             },

               new Location {
                dbName = "tavern_counter",
                Name = "The Smelly Sorceress Tavern (Bar Counter)",
                Region = "tavern",
                X = 0,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
               Name_South="street_14th_east",
               Name_West="street_15th_south",
               Name_East = "tavern_pool",
               Name_North = "tavern_private_room",
                
               },  new Location {
                dbName = "tavern_pool",
                Name = "The Smelly Sorceress Tavern (Pool Room)",
                Region = "tavern",
                X = 1,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "tavern_counter",
                Name_North = "tavern_dumpsters",

            },  new Location {
                dbName = "tavern_private_room",
                Name = "The Smelly Sorceress Tavern (Private Room)",
                Region = "tavern",
                X = 0,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "tavern_counter",
                Name_East = "tavern_dumpsters",

            },  new Location {
                dbName = "tavern_dumpsters",
                Name = "The Smelly Sorceress Tavern (Dumpsters)",
                Region = "tavern",
                X = 1,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_West = "tavern_private_room",
                Name_South = "tavern_pool",

            }, 

                  new Location {
                dbName = "street_e9th_eastof_main",
                Name = "Street: 10 E. 9th East of Main",
                Region = "streets",
                X = 0,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "street_e9th_eastof_valley",
                Name_West= "street_9th",

                 }, 

                  new Location {
                dbName = "street_e9th_eastof_valley",
                Name = "Street: 20 E. 9th Avenue",
                Region = "streets",
                X = 1,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_West="street_e9th_eastof_main",
                Name_East="street_e9th_valley_crossing",

                 }, 

                  new Location {
                dbName = "street_e9th_valley_crossing",
                Name = "Street: 30 E. 9th Avenue",
                Region = "streets",
                X = 2,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_West="street_e9th_eastof_valley",
                Name_East="street_e9th_westof_valley",
            }, 
                new Location {
                dbName = "street_e9th_westof_valley",
                Name = "Street: 40 E. 9th Avenue",
                Region = "streets",
                X = 3,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_50e9th",
                Name_West="street_e9th_valley_crossing",
                Name_South = "concert_hall_front_door",
                Name_North = "lab_lobby",
          }, 
                new Location {
                dbName = "apartment_rental_office",
                Name = "Oldoak Apartments (Rental Office)",
                Region = "oldoak_apartments",
                X = -2,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_02",
                Name_West="apartment_parking_lot",
                Name_North = "apartment_complex_a",
            }, 
                new Location {
                dbName = "apartment_parking_lot",
                Name = "Oldoak Apartments (Parking Lot)",
                Region = "oldoak_apartments",
                X = -3,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "apartment_rental_office",
                Name_North = "apartment_dog_park",
                
            }, 
                new Location {
                dbName = "apartment_dog_park",
                Name = "Oldoak Apartments (Dog Park)",
                Region = "oldoak_apartments",
                X = -3,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_South = "apartment_parking_lot",
                Name_East = "apartment_complex_a",
           }, 
                new Location {
                dbName = "apartment_complex_a",
                Name = "Oldoak Apartments (Apartment Building)",
                Region = "oldoak_apartments",
                X = -2,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_West="apartment_dog_park",
                Name_South="apartment_rental_office",
               }, 

            new Location {
                dbName = "concert_hall_front_door",
                Name = "Oldport Concert Hall (Front Door)",
                Region = "concert_hall",
                X = 3,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_e9th_westof_valley",
                Name_South = "concert_hall_seats",
                Name_East = "concert_hall_bar",

                }, 

        new Location {
                dbName = "concert_hall_seats",
                Name = "Oldport Concert Hall (Audience Area)",
                Region = "concert_hall",
                X = 3,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North="concert_hall_front_door",
                Name_East="concert_hall_stage",
                }, 

         new Location {
                dbName = "concert_hall_stage",
                Name = "Oldport Concert Hall (Stage)",
                Region = "concert_hall",
                X = 4,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North="concert_hall_bar",
                Name_East="concert_hall_backstage",
                Name_West="concert_hall_seats",

                }, 

           new Location {
                dbName = "concert_hall_bar",
                Name = "Oldport Concert Hall (Bar Counter)",
                Region = "concert_hall",
                X = 4,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="concert_hall_dressingroom",
                Name_South="concert_hall_stage",
                Name_West="concert_hall_front_door",

                }, 

          new Location {
                dbName = "concert_hall_dressingroom",
                Name = "Oldport Concert Hall (Dressing Room)",
                Region = "concert_hall",
                X = 5,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_South="concert_hall_backstage",
                Name_West="concert_hall_bar",

                }, 

          new Location {
                dbName = "concert_hall_backstage",
                Name = "Oldport Concert Hall (Backstage)",
                Region = "concert_hall",
                X = 5,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North="concert_hall_dressingroom",
                Name_West="concert_hall_stage",

                      },

     new Location {
                dbName = "350_west_9th_ave",
                Name = "Street: 350 W. 9th Avenue",
                Region = "streets",
                X = -2,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_03",
                Name_West="340_west_9th_ave",

                },

          new Location {
                dbName = "340_west_9th_ave",
                Name = "Street: 340 W. 9th Avenue",
                Region = "streets",
                X = -3,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="350_west_9th_ave",
                Name_West="330_west_9th_ave",

                },

          new Location {
                dbName = "330_west_9th_ave",
                Name = "Street: 330 W. 9th Avenue",
                Region = "streets",
                X = -4,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="340_west_9th_ave",
                Name_West="320_west_9th_ave",

                },

           new Location {
                dbName = "320_west_9th_ave",
                Name = "Street: 320 W. 9th Avenue",
                Region = "streets",
                X = -5,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "gym_entrance",
                Name_East="330_west_9th_ave",
                Name_West="310_west_9th_ave",

                },

          new Location {
                dbName = "310_west_9th_ave",
                Name = "Street: 310 W. 9th Avenue",
                Region = "streets",
                X = -6,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="320_west_9th_ave",
                Name_West = "300_west_9th_ave",
                Name_South = "college_foyer",
                },

          new Location {
                dbName = "gym_entrance",
                Name = "Sweater Girls Gym (Front Desk)",
                Region = "gym",
                X = -5,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_North="gym_cardio",
                Name_South="320_west_9th_ave",
                Name_West="gym_aerobics",

           }, new Location {
                dbName = "gym_aerobics",
                Name = "Sweater Girls Gym (Aerobics Room)",
                Region = "gym",
                X = -6,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_North="gym_weights",
                Name_East="gym_entrance",

         }, new Location {
                dbName = "gym_weights",
                Name = "Sweater Girls Gym (Weight Machines)",
                Region = "gym",
                X = -6,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_North="gym_laundry",
                Name_East="gym_cardio",
                Name_South="gym_aerobics",

          }, new Location {
                dbName = "gym_cardio",
                Name = "Sweater Girls Gym (Cardio Machines)",
                Region = "gym",
                X = -5,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_South="gym_entrance",
                Name_West="gym_weights",

         }, new Location {
                dbName = "gym_laundry",
                Name = "Sweater Girls Gym (Laundry Room)",
                Region = "gym",
                X = -6,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_South="gym_weights",

           }, new Location {
                dbName = "300_west_9th_ave",
                Name = "Street: 300 W. 9th Avenue",
                Region = "streets",
                X = -7,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="310_west_9th_ave",
                Name_West="290_west_9th_ave",

        }, new Location {
                dbName = "college_foyer",
                Name = "SCCC (Admissions Office) ",
                Region = "college",
                X = -6,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_North="310_west_9th_ave",
                Name_East="college_sciences",
                Name_South="college_humanities",
                Name_West="college_arts",

        }, new Location {
                dbName = "college_sciences",
                Name = "SCCC (Sciences Classroom)",
                Region = "college",
                X = -5,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_South="college_vet",
                Name_West="college_foyer",

        }, new Location {
                dbName = "college_vet",
                Name = "SCCC (Veterinarian Center)",
                Region = "college",
                X = -5,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North="college_sciences",
                Name_West="college_humanities",

        }, new Location {
                dbName = "college_humanities",
                Name = "SCCC (Humanities Classroom)",
                Region = "college",
                X = -6,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North="college_foyer",
                Name_East="college_vet",
                Name_West="college_business",

          }, new Location {
                dbName = "college_business",
                Name = "SCCC (Business Classroom)",
                Region = "college",
                X = -7,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North="college_arts",
                Name_East="college_humanities",
                Name_South="college_track",

          }, new Location {
                dbName = "college_arts",
                Name = "SCCC (Arts Classroom)",
                Region = "college",
                X = -7,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="college_foyer",
                Name_South="college_business",

          }, new Location {
                dbName = "college_track",
                Name = "SCCC (Athletics Field)",
                Region = "college",
                X = -7,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North="college_business",
                Name_South="pathway_sccc",
 
          }, new Location {
                dbName = "lab_lobby",
                Name = "Dr. Hadkin's Research Clinic (Lobby)",
                Region = "lab",
                X = 3,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North="lab_laboratory",
                Name_East="lab_offices",
                Name_South="street_e9th_westof_valley",

       }, new Location {
                dbName = "lab_offices",
                Name = "Dr. Hadkin's Research Clinic (Offices)",
                Region = "lab",
                X = 4,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North="lab_freezers",
                Name_West="lab_lobby",

       }, new Location {
                dbName = "lab_freezers",
                Name = "Dr. Hadkin's Research Clinic (Freezers)",
                Region = "lab",
                X = 4,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_South="lab_offices",
                Name_West="lab_laboratory",

       }, new Location {
                dbName = "lab_laboratory",
                Name = "Dr. Hadkin's Research Clinic (Laboratory)",
                Region = "lab",
                X = 3,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="lab_freezers",
                Name_South="lab_lobby",
                Name_West="lab_secret_laboratory",

       }, new Location {
                dbName = "lab_secret_laboratory",
                Name = "Dr. Hadkin's Research Clinic (Secret Laboratory)",
                Region = "lab",
                X = 2,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="lab_laboratory",


        }, new Location {
                dbName = "pool_entrance",
                Name = "Sunnyglade Pool (Entrance)",
                Region = "pool",
                X = -2,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_7th_main",
                Name_South="pool_shallow",
                Name_West="pool_slide",

        }, new Location {
                dbName = "pool_shallow",
                Name = "Sunnyglade Pool (Shallow Swimming Area)",
                Region = "pool",
                X = -2,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North="pool_entrance",
                Name_West="pool_concessions",

         }, new Location {
                dbName = "pool_concessions",
                Name = "Sunnyglade Pool (Concession Stand)",
                Region = "pool",
                X = -3,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North="pool_slide",
                Name_East="pool_shallow",
                Name_West="pool_playground",

          }, new Location {
                dbName = "pool_slide",
                Name = "Sunnyglade Pool (Water Slide)",
                Region = "pool",
                X = -3,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_East="pool_entrance",
                Name_South="pool_concessions",
                Name_West="pool_deep",

          }, new Location {
                dbName = "pool_deep",
                Name = "Sunnyglade Pool (Deep Swimming Area)",
                Region = "pool",
                X = -4,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_East="pool_slide",
                Name_South="pool_playground",

          }, new Location {
                dbName = "pool_playground",
                Name = "Sunnyglade Pool (Playground)",
                Region = "pool",
                X = -4,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North="pool_deep",
                Name_East="pool_concessions",
                Name_West="pathway_pool",

          },  new Location {
                dbName = "street_140_sunnyglade_drive",
                Name = "Street: 140 Sunnyglade Drive",
                Region = "streets",
                X = -2,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_North="comicstore_counter",
                Name_East="street_14th_north",
                Name_West="street_130_sunnyglade_drive",

          },  new Location {
                dbName = "comicstore_counter",
                Name = "Extended Universe Comics (Front Counter)",
                Region = "comicstore",
                X = -2,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="comicstore_videogames",
                Name_South="street_140_sunnyglade_drive",
                Name_West="comicstore_gaming_room",

          },  new Location {
                dbName = "comicstore_gaming_room",
                Name = "Extended Universe Comics (Gaming Room)",
                Region = "comicstore",
                X = -3,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="comicstore_comics",
                Name_East="comicstore_counter",

          },  new Location {
                dbName = "comicstore_comics",
                Name = "Extended Universe Comics (Comics Shelves)",
                Region = "comicstore",
                X = -3,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_East="comicstore_videogames",
                Name_South="comicstore_gaming_room",

           },  new Location {
                dbName = "comicstore_videogames",
                Name = "Extended Universe Comics (Video/Board Game Shelves)",
                Region = "comicstore",
                X = -2,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_South="comicstore_counter",
                Name_West="comicstore_comics",

           }, new Location {
                dbName = "street_170_sunnyglade_drive",
                Name = "Street: 170 Sunnyglade Drive",
                Region = "streets",
                X = 1,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_180_sunnyglade_drive",
                Name_West="street_14th_east",

           }, new Location {
                dbName = "street_180_sunnyglade_drive",
                Name = "Street: 180 Sunnyglade Drive",
                Region = "streets",
                X = 2,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_190_sunnyglade_drive",
                Name_West="street_170_sunnyglade_drive",

           }, new Location {
                dbName = "street_190_sunnyglade_drive",
                Name = "Street: 190 Sunnyglade Drive",
                Region = "streets",
                X = 3,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_200_sunnyglade_drive",
                Name_West="street_180_sunnyglade_drive",

           }, new Location {
                dbName = "street_200_sunnyglade_drive",
                Name = "Street: 200 Sunnyglade Drive",
                Region = "streets",
                X = 4,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_gate",
                Name_West="street_190_sunnyglade_drive",
                Name_East="street_210_sunnyglade_drive"

           }, new Location {
                dbName = "mansion_gate",
                Name = "Erie Estate (Front Gate)",
                Region = "mansion",
                X = 4,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_foyer",
                Name_East="mansion_eastgarden",
                Name_South="street_200_sunnyglade_drive",
                Name_West="mansion_westgarden",

          }, new Location {
                dbName = "mansion_westgarden",
                Name = "Erie Estate (Western Garden)",
                Region = "mansion",
                X = 3,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_dining",
                Name_East="mansion_gate",

          }, new Location {
                dbName = "mansion_eastgarden",
                Name = "Erie Estate (Eastern Garden)",
                Region = "mansion",
                X = 5,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_bedroom",
                Name_West="mansion_gate",

          }, new Location {
                dbName = "mansion_foyer",
                Name = "Erie Estate (Foyer)",
                Region = "mansion",
                X = 4,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_courtyard",
                Name_East="mansion_bedroom",
                Name_South="mansion_gate",
                Name_West="mansion_dining",

          }, new Location {
                dbName = "mansion_dining",
                Name = "Erie Estate (Dining Hall)",
                Region = "mansion",
                X = 3,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_ballroom",
                Name_East="mansion_foyer",
                Name_South="mansion_westgarden",

          }, new Location {
                dbName = "mansion_bedroom",
                Name = "Erie Estate (Master Bedroom)",
                Region = "mansion",
                X = 5,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_baths",
                Name_South="mansion_eastgarden",
                Name_West="mansion_foyer",

         }, new Location {
                dbName = "mansion_courtyard",
                Name = "Erie Estate (Courtyard)",
                Region = "mansion",
                X = 4,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_chapel",
                Name_South="mansion_foyer",

         }, new Location {
                dbName = "mansion_ballroom",
                Name = "Erie Estate (Ballroom)",
                Region = "mansion",
                X = 3,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_servant_quarters",
                Name_South="mansion_dining",

             }, new Location {
                dbName = "mansion_baths",
                Name = "Erie Estate (Master Baths)",
                Region = "mansion",
                X = 5,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_study",
                Name_South="mansion_bedroom",

          }, new Location {
                dbName = "mansion_chapel",
                Name = "Erie Estate (Chapel)",
                Region = "mansion",
                X = 4,
                Y = 9,
                IsSafe = false,
                ImageUrl = "",
                Name_North="mansion_mausoleum",
                Name_East="mansion_study",
                Name_South="mansion_courtyard",
                Name_West="mansion_servant_quarters",

          }, new Location {
                dbName = "mansion_servant_quarters",
                Name = "Erie Estate (Servant Quarters) ",
                Region = "mansion",
                X = 3,
                Y = 9,
                IsSafe = false,
                ImageUrl = "",
                Name_East="mansion_chapel",
                Name_South="mansion_ballroom",

        }, new Location {
                dbName = "mansion_study",
                Name = "Erie Estate (Study)",
                Region = "mansion",
                X = 5,
                Y = 9,
                IsSafe = false,
                ImageUrl = "",
                Name_South="mansion_baths",
                Name_West="mansion_chapel",

         }, new Location {
                dbName = "mansion_mausoleum",
                Name = "Erie Estate (Mausoleum)",
                Region = "mansion",
                X = 4,
                Y = 10,
                IsSafe = false,
                ImageUrl = "",
                Name_South="mansion_chapel",


         }, new Location {
                dbName = "street_140_main",
                Name = "Street: 140 Main Street",
                Region = "streets",
                X = -1,
                Y = -5,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_6th_main",
                Name_South="street_130_main",
                Name_West = "medclinic_lobby",

             }, new Location {
                dbName = "street_130_main",
                Name = "Street: 130 Main Street",
                Region = "streets",
                X = -1,
                Y = -6,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_140_main",
                Name_East="stripclub_bar_seats",

            }, new Location {
                dbName = "stripclub_bar_seats",
                Name = "The Treasure Chest (Stage)",
                Region = "stripclub",
                X = 0,
                Y = -6,
                IsSafe = false,
                ImageUrl = "",
                Name_East="stripclub_store",
                Name_South="stripclub_booths",
                Name_West="street_130_main",

            }, new Location {
                dbName = "stripclub_booths",
                Name = "The Treasure Chest (Private Booths)",
                Region = "stripclub",
                X = 0,
                Y = -7,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "stripclub_bar_seats",

           }, new Location {
                dbName = "stripclub_office",
                Name = "The Treasure Chest (Manager's Office)",
                Region = "stripclub",
                X = 1,
                Y = -7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="stripclub_store",

           }, new Location {
                dbName = "stripclub_store",
                Name = "The Treasure Chest (Adult Toy Store)",
                Region = "stripclub",
                X = 1,
                Y = -6,
                IsSafe = false,
                ImageUrl = "",
                Name_South="stripclub_office",
                Name_West="stripclub_bar_seats",

          }, new Location {
                dbName = "street_210_sunnyglade_drive",
                Name = "Street: 210 Sunnyglade Drive",
                Region = "streets",
                X = 5,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_220_sunnyglade_drive",
                Name_West="street_200_sunnyglade_drive",

           }, new Location {
                dbName = "street_220_sunnyglade_drive",
                Name = "Street: 220 Sunnyglade Drive",
                Region = "streets",
                X = 6,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_230_sunnyglade_drive",
                Name_South="ranch_entrance",
                Name_West="street_210_sunnyglade_drive",

           }, new Location {
                dbName = "ranch_entrance",
                Name = "Milton Ranch (Entrance)",
                Region = "ranch_inside",
                X = 6,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_220_sunnyglade_drive",
                Name_West="ranch_porch",

           }, new Location {
                dbName = "ranch_bedroom",
                Name = "Milton Ranch (Master Bedroom)",
                Region = "ranch_inside",
                X = 6,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_West="ranch_hallway",

           }, new Location {
                dbName = "ranch_hallway",
                Name = "Milton's Ranch (Hallway and Kitchen)",
                Region = "ranch_inside",
                X = 5,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "ranch_porch",
                Name_East="ranch_bedroom",
                Name_West="ranch_bedroom_teenager",
                Name_South = "ranch_pasture",

           }, new Location {
                dbName = "ranch_porch",
                Name = "Milton Ranch (Porch)",
                Region = "ranch_inside",
                X = 5,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="ranch_entrance",
                Name_South="ranch_hallway",
                Name_West="ranch_livingroom",

           }, new Location {
                dbName = "ranch_livingroom",
                Name = "Milton Ranch (Living Room)",
                Region = "ranch_inside",
                X = 4,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="ranch_porch",

          }, new Location {
                dbName = "ranch_bedroom_teenager",
                Name = "Milton Ranch (Teenager's Bedroom)",
                Region = "ranch_inside",
                X = 4,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_East="ranch_hallway",
 

        }, new Location {
                dbName = "ranch_pens",
                Name = "Milton Ranch (Livestock Pens)",
                Region = "ranch_outside",
                X = 4,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="ranch_pasture",

           },  new Location {
                dbName = "ranch_pasture",
                Name = "Milton Ranch (Grassy Pasture)",
                Region = "ranch_outside",
                X = 5,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "ranch_hallway",
                Name_East="ranch_barn",
                Name_West="ranch_pens",

           },  new Location {
                dbName = "ranch_barn",
                Name = "Milton Ranch (Barn)",
                Region = "ranch_outside",
                X = 6,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_West="ranch_pasture",

            }, new Location {
                dbName = "290_west_9th_ave",
                Name = "Street: 290 W. 9th Avenue",
                Region = "streets",
                X = -8,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="300_west_9th_ave",
                Name_West="280_west_9th_ave",

            }, new Location {
                dbName = "280_west_9th_ave",
                Name = "Street: 280 W. 9th Avenue",
                Region = "streets",
                X = -9,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_East="290_west_9th_ave",
                Name_West="270_west_9th_ave",

           }, new Location {
                dbName = "270_west_9th_ave",
                Name = "Street: 270 W. 9th Avenue",
                Region = "streets",
                X = -10,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_garden",
                Name_East="280_west_9th_ave",
                Name_South = "sorority_yard",

          }, new Location {
                dbName = "castle_garden",
                Name = "Valentine Castle (Enchanted Garden) ",
                Region = "castle",
                X = -10,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_greathall",
                Name_South="270_west_9th_ave",


         }, new Location {
                dbName = "castle_greathall",
                Name = "Valentine Castle (Great Hall)",
                Region = "castle",
                X = -10,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_tower",
                Name_East="castle_kitchen",
                Name_South="castle_garden",
                Name_West="castle_armory",

              }, new Location {
                dbName = "castle_kitchen",
                Name = "Valentine Castle (Kitchen)",
                Region = "castle",
                X = -9,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_servants",
                Name_East="castle_cellar",
                Name_West="castle_greathall",

       }, new Location {
                dbName = "castle_armory",
                Name = "Valentine Castle (Armory)",
                Region = "castle",
                X = -11,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="castle_greathall",
                Name_West="castle_dungeon",

        }, new Location {
                dbName = "castle_dungeon",
                Name = "Valentine Castle (Dungeon)",
                Region = "castle",
                X = -12,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="castle_armory",

         }, new Location {
                dbName = "castle_tower",
                Name = "Valentine Castle (Central Tower)",
                Region = "castle",
                X = -10,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_throneroom",
                Name_East="castle_servants",
                Name_South="castle_greathall",
                Name_West="castle_baths",

         }, new Location {
                dbName = "castle_baths",
                Name = "Valentine Castle (Bath Chambers)",
                Region = "castle",
                X = -11,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_lordroom",
                Name_East="castle_tower",

         }, new Location {
                dbName = "castle_servants",
                Name = "Valentine Castle (Servant Quarters)",
                Region = "castle",
                X = -9,
                Y = 3,
                IsSafe = false,
                ImageUrl = "",
                Name_South="castle_kitchen",
                Name_West="castle_tower",

        }, new Location {
                dbName = "castle_throneroom",
                Name = "Valentine Castle (Throne Room)",
                Region = "castle",
                X = -10,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "castle_beach",
                Name_East="castle_solar",
                Name_South="castle_tower",
                Name_West="castle_lordroom",

        }, new Location {
                dbName = "castle_lordroom",
                Name = "Valentine Castle (Lord's Room)",
                Region = "castle",
                X = -11,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="castle_throneroom",
                Name_South="castle_baths",
                Name_West = "castle_treasury",

         }, new Location {
                dbName = "castle_solar",
                Name = "Valentine Castle (Solar)",
                Region = "castle",
                X = -9,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="castle_training",
                Name_West="castle_throneroom",

         }, new Location {
                dbName = "castle_treasury",
                Name = "Valentine Castle (Treasury)",
                Region = "castle",
                X = -12,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="castle_lordroom",

       }, new Location {
                dbName = "castle_cellar",
                Name = "Valentine Castle (Cellar)",
                Region = "castle",
                X = -8,
                Y = 2,
                IsSafe = false,
                ImageUrl = "",
                Name_West="castle_kitchen",

         }, new Location {
                dbName = "castle_beach",
                Name = "Valentine Castle (Beach)",
                Region = "castle",
                X = -10,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_South="castle_throneroom",

         }, new Location {
                dbName = "castle_training",
                Name = "Valentine Castle (Training Halls)",
                Region = "castle",
                X = -8,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_West="castle_solar",

          }, new Location {
                dbName = "street_130_sunnyglade_drive",
                Name = "Street: 130 Sunnyglade Drive",
                Region = "streets",
                X = -3,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_140_sunnyglade_drive",
                Name_West="street_120_sunnyglade_drive",

          }, new Location {
                dbName = "street_120_sunnyglade_drive",
                Name = "Street: 120 Sunnyglade Drive",
                Region = "streets",
                X = -4,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_130_sunnyglade_drive",
                Name_South="candystore_counter",
                //Name_West="",

          }, new Location {
                dbName = "candystore_shelves",
                Name = "Sugarsand Station Candy Store (Shelves)",
                Region = "candystore",
                X = -3,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_West="candystore_counter",

           }, new Location {
                dbName = "candystore_counter",
                Name = "Sugarsand Station Candy Store (Front Counter)",
                Region = "candystore",
                X = -4,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "street_120_sunnyglade_drive",
                Name_East="candystore_shelves",
                Name_West="candystore_kitchen",

          }, new Location {
                dbName = "candystore_kitchen",
                Name = "Sugarsand Station Candy Store (Kitchen)",
                Region = "candystore",
                X = -5,
                Y = 4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="candystore_counter",

           }, new Location {
                dbName = "street_50e9th",
                Name = "Street: 50 E. 9th Avenue",
                Region = "streets",
                X = 4,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_60e9th",
                Name_West="street_e9th_westof_valley",

           }, new Location {
                dbName = "street_60e9th",
                Name = "Street: 60 E. 9th Avenue",
                Region = "streets",
                X = 5,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_East="street_70e9th",
                Name_West="street_50e9th",

           }, new Location {
                dbName = "street_70e9th",
                Name = "Street: 70 E. 9th Avenue",
                Region = "streets",
                X = 6,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_West="street_60e9th",
                Name_North="salon_front_desk",

           }, new Location {
                dbName = "salon_front_desk",
                Name = "Blazes and Glamour Beauty Spa (Front Desk)",
                Region = "salon",
                X = 6,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "salon_hairdressing",
                Name_East="salon_massage",
                Name_South="street_70e9th",

           }, new Location {
                dbName = "salon_hairdressing",
                Name = "Blazes and Glamour Beauty Spa (Hairdressing Seats)",
                Region = "salon",
                X = 6,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_South="salon_front_desk",

          }, new Location {
                dbName = "salon_massage",
                Name = "Blazes and Glamour Beauty Spa (Massage Parlor)",
                Region = "salon",
                X = 7,
                Y = 0,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "salon_storage",
                Name_West="salon_front_desk",

          }, new Location {
                dbName = "salon_storage",
                Name = "Blazes and Glamour Beauty Spa (Storage Room)",
                Region = "salon",
                X = 7,
                Y = 1,
                IsSafe = false,
                ImageUrl = "",
                Name_South="salon_massage",

             }, new Location {
                dbName = "sorority_yard",
                Name = "EGM Sorority House (Front Yard)",
                Region = "sorority",
                X = -10,
                Y = -1,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "270_west_9th_ave",
                Name_South="sorority_common",

            }, new Location {
                dbName = "sorority_common",
                Name = "EGM Sorority House (Common Room)",
                Region = "sorority",
                X = -10,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_yard",
                Name_East="sorority_dining",
                Name_West="sorority_bedrooms",

            }, new Location {
                dbName = "sorority_bedrooms",
                Name = "EGM Sorority House (Bedrooms)",
                Region = "sorority",
                X = -11,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_East="sorority_common",
                Name_South="sorority_motherbed",

            }, new Location {
                dbName = "sorority_motherbed",
                Name = "EGM Sorority House (House Mother's Bedroom)",
                Region = "sorority",
                X = -11,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_bedrooms",
                Name_South="sorority_basement",

           }, new Location {
                dbName = "sorority_basement",
                Name = "EGM Sorority House (Basement)",
                Region = "sorority",
                X = -11,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_motherbed",
                Name_East="sorority_patio",

           }, new Location {
                dbName = "sorority_patio",
                Name = "EGM Sorority House (Back Patio)",
                Region = "sorority",
                X = -10,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_rooftop",
                Name_East="sorority_backyard",
                Name_West="sorority_basement",

            }, new Location {
                dbName = "sorority_rooftop",
                Name = "EGM Sorority House (Rooftop)",
                Region = "sorority",
                X = -10,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_South="sorority_patio",

            }, new Location {
                dbName = "sorority_backyard",
                Name = "EGM Sorority House (Backyard)",
                Region = "sorority",
                X = -9,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_kitchen",
                Name_East="pathway_sorority",
                Name_West="sorority_patio",

            }, new Location {
                dbName = "sorority_kitchen",
                Name = "EGM Sorority House (Kitchen)",
                Region = "sorority",
                X = -9,
                Y = -3,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "sorority_dining",
                Name_South="sorority_backyard",

           }, new Location {
                dbName = "sorority_dining",
                Name = "EGM Sorority House (Dining Room)",
                Region = "sorority",
                X = -9,
                Y = -2,
                IsSafe = false,
                ImageUrl = "",
                Name_South="sorority_kitchen",
                Name_West="sorority_common",

            }, new Location {
                dbName = "pathway_sorority",
                Name = "Scenic Pathway (Near Sorority House)",
                Region = "streets",
                X = -8,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="pathway_sccc",
                Name_West="sorority_backyard",

             }, new Location {
                dbName = "pathway_sccc",
                Name = "Scenic Pathway (Near SCCC)",
                Region = "streets",
                X = -7,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "college_track",
                Name_East="pathway_campground",
                Name_South="campground_lake",
                Name_West="pathway_sorority",

            }, new Location {
                dbName = "pathway_campground",
                Name = "Scenic Pathway (near Sapphire Lake Campground)",
                Region = "streets",
                X = -6,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="pathway_pool",
                Name_West="pathway_sccc",

            }, new Location {
                dbName = "pathway_pool",
                Name = "Scenic Pathway (near Sunnyglade Pool)",
                Region = "streets",
                X = -5,
                Y = -4,
                IsSafe = false,
                ImageUrl = "",
                Name_East="pool_playground",
                Name_West="pathway_campground",

           }, new Location {
                dbName = "campground_lake",
                Name = "Sapphire Lake (Campgrounds) ",
                Region = "campground",
                X = -7,
                Y = -5,
                IsSafe = false,
                ImageUrl = "",
                Name_North = "pathway_sccc",
                Name_East="campground_makeout",

           }, new Location {
                dbName = "campground_makeout",
                Name = "Sapphire Lake (Makeout Point)",
                Region = "campground",
                X = -6,
                Y = -5,
                IsSafe = false,
                ImageUrl = "",
                Name_West="campground_lake",


           }, new Location {
                dbName = "medclinic_lobby",
                Name = "Henryson Medical Clinic (Lobby and Payee Window)",
                Region = "medclinic",
                X = -2,
                Y = -5,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "street_140_main",
                Name_South = "medclinic_physician_a",
                Name_West = "medclinic_records",

           }, new Location {
                dbName = "medclinic_physician_a",
                Name = "Henryson Medical Clinic (Physician's Room)",
                Region = "medclinic",
                X = -2,
                Y = -6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="medclinic_lobby",
                Name_West = "medclinic_physician_b",

           }, new Location {
                dbName = "medclinic_physician_b",
                Name = "Henryson Medical Clinic (Emergency Room)",
                Region = "medclinic",
                X = -3,
                Y = -6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="medclinic_records",
                Name_East = "medclinic_physician_a",

           }, new Location {
                dbName = "medclinic_records",
                Name = "Henryson Medical Clinic (Medical Records Room)",
                Region = "medclinic",
                X = -3,
                Y = -5,
                IsSafe = false,
                ImageUrl = "",
                Name_East = "medclinic_lobby",
                Name_South = "medclinic_physician_b",

           }, new Location {
                dbName = "street_230_sunnyglade_drive",
                Name = "Street: 230 Sunnyglade Drive",
                Region = "streets",
                X = 7,
                Y = 5,
                IsSafe = false,
                ImageUrl = "",
                Name_North="forest_parking",
                Name_West="street_220_sunnyglade_drive",

           }, new Location {
                dbName = "forest_parking",
                Name = "Scarlet Forest (Parking Lot)",
                Region = "forest",
                X = 7,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="cave_mouth",
                Name_East="forest_pinecove",
                Name_South="street_230_sunnyglade_drive",
                Name_West="forest_ancestor_tree",

          }, new Location {
                dbName = "forest_ancestor_tree",
                Name = "Scarlet Forest (Ancestor Tree)",
                Region = "forest",
                X = 6,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_North="forest_hotspring",
                Name_East="forest_parking",

          }, new Location {
                dbName = "forest_hotspring",
                Name = "Scarlet Forest (Hot Springs)",
                Region = "forest",
                X = 6,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="fairygrove_entrance",
                Name_East="cave_mouth",
                Name_South="forest_ancestor_tree",

          }, new Location {
                dbName = "cave_mouth",
                Name = "Silent Cavern (Mouth of the Cave)",
                Region = "cave",
                X = 7,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_East="cave_antechamber",
                Name_South="forest_parking",
                Name_West="forest_hotspring",

          }, new Location {
                dbName = "cave_antechamber",
                Name = "Silent Cavern (Antechamber)",
                Region = "cave",
                X = 8,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_North="cave_burialchamber",
                Name_East="cave_lake",
                Name_West="cave_mouth",

          }, new Location {
                dbName = "cave_burialchamber",
                Name = "Silent Cavern (Burial Chamber) ",
                Region = "cave",
                X = 8,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_South="cave_antechamber",

          }, new Location {
                dbName = "cave_lake",
                Name = "Silent Cavern (Underground Lake)",
                Region = "cave",
                X = 9,
                Y = 7,
                IsSafe = false,
                ImageUrl = "",
                Name_West="cave_antechamber",

          }, new Location {
                dbName = "forest_pinecove",
                Name = "Scarlet Forest (Pine Cove) ",
                Region = "forest",
                X = 8,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_East="forest_cabin",
                Name_West="forest_parking",

          }, new Location {
                dbName = "forest_cabin",
                Name = "Scarlet Forest (Burnt Cabin)",
                Region = "forest",
                X = 9,
                Y = 6,
                IsSafe = false,
                ImageUrl = "",
                Name_West="forest_pinecove",

           },

           new Location {
                dbName = "fairygrove_entrance",
                Name = "Fairy Grove (Entrance)",
                Region = "fairygrove",
                X = 6,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_North="fairygrove_flowerhomes",
                Name_East="fairygrove_springs",
                Name_South="forest_hotspring",

           },

           new Location {
                dbName = "fairygrove_flowerhomes",
                Name = "Fairy Grove (Flower Homes)",
                Region = "fairygrove",
                X = 6,
                Y = 9,
                IsSafe = false,
                ImageUrl = "",
                Name_East="fairygrove_greathall",
                Name_South="fairygrove_entrance",

           },

           new Location {
                dbName = "fairygrove_greathall",
                Name = "Fairy Grove (Great Hall)",
                Region = "fairygrove",
                X = 7,
                Y = 9,
                IsSafe = false,
                ImageUrl = "",
                Name_South="fairygrove_springs",
                Name_West="fairygrove_flowerhomes",

           },

           new Location {
                dbName = "fairygrove_springs",
                Name = "Fairy Grove (Springs)",
                Region = "fairygrove",
                X = 7,
                Y = 8,
                IsSafe = false,
                ImageUrl = "",
                Name_North="fairygrove_greathall",
                Name_West="fairygrove_entrance",

           },

        };

            }
    
        }



        public static string GetConnectionName(string locDbName)
        {
            var Location = LocationList.GetLocation.FirstOrDefault(l => l.dbName == locDbName);
            if (Location != null)
            {
                return Location.Name;
            }
            return "";
        }

    }

    public static class ChatStatics
    {
        public const int OnlineActivityCutoffMinutes = -2;
        private const string JudooThumbnail = "Images/PvP/portraits/Thumbnails/100/Judoo.jpg";
        private const string MizuhoThumbnail = "/Images/PvP/portraits/Thumbnails/100/Mizuho.jpg";
     // private const string ArrhaeThumbnail =
     // private const string EricThumbnail =
     // private const string TempestThumbnail =

        private static IDictionary<string, PlayerDescriptorDTO> staff
            = new Dictionary<string, PlayerDescriptorDTO>
            {

                {
                    "69", new PlayerDescriptorDTO // Judoo
                    {
                        Name = "Judoo",
                     // PictureURL = JudooThumbnail, turning off thumbnail for a while
                        TagBehaviorEnum = TagEnum.ReplaceFullName,
                        RoleEnum = RoleEnum.Admin
                    }
                },
                {
                    "3490", new PlayerDescriptorDTO // Mizuho
                    {
                        Name = "Mizuho",
                        PictureURL = MizuhoThumbnail,
                        TagBehaviorEnum = TagEnum.ReplaceFullName,
                        RoleEnum = RoleEnum.Developer
                    }
                },
                {
                    "251", new PlayerDescriptorDTO // Arrhae
                    {
                     // PictureURL = ArrhaeThumbnail, Arrhae wants to keep regular portrait for now, not admin/dev custom one
                        RoleEnum = RoleEnum.Developer
                    }
                },
                {
                    "834", new PlayerDescriptorDTO // Eric
                    {
                     // PictureURL = EricThumbnail, Not likely
                        RoleEnum = RoleEnum.Developer
                    }
                },
                {
                    "14039", new PlayerDescriptorDTO // Tempest
                    {
                        Name = "Tempest",
                     // PictureURL = TempestThumbnail, no custom portrait yet
                        TagBehaviorEnum = TagEnum.ReplaceFullName,
                        RoleEnum = RoleEnum.Developer
                    }
                }
            };

        /// <summary>
        /// Gets the read only dictionary of the staff's <see cref="PlayerDescriptorDTO"/> using their membership id as a key.
        /// </summary>
        public static IReadOnlyDictionary<string, PlayerDescriptorDTO> Staff { get; }
            = new ReadOnlyDictionary<string, PlayerDescriptorDTO>(staff);

        public static readonly IEnumerable<string> HideOnJoinChat = new List<string>
        {
            "69",
            "3490",
        };

        public static readonly IEnumerable<string> ReservedText = new List<string>
        {
            "[luxa]",
            "[blanca]",
            "[poll]",
            "[fp]",
            "[sd]",
        };

        public static readonly IEnumerable<string> ActionTypes = new List<string>
        {
            "creature",
            "item",
            "event",
            "trap",
            "tf.animate",
            "tf.inanimate",
            "tf.animal",
            "tf.partial",
        };

        public static readonly IEnumerable<string> Tags = new List<string>
        {
            "forest",
            "highschool",
            "bimbocalypse",
            "latexfactory",
            "highschool",
            "forest",
            "bimbocalypse",
            "latexfactory",
        };
    }
}