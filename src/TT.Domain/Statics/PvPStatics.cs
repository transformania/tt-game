using System.Collections.Generic;
using System.Collections.ObjectModel;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    using TagEnum = PlayerDescriptorStatics.TagBehavior;
    using RoleEnum = PlayerDescriptorStatics.Role;

    public static class PvPStatics
    {

        public static int RoundDuration = 4000;

        public static string AlphaRound = "Alpha Round 39";
        public const string LastTosUpdate = "May 25, 2018";
        public const string LastPrivacyPolicyUpdate = "May 25, 2018";

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
        public const decimal XP__LevelDifferenceXPGainModifier = 1.5M;
        public const decimal XP__EndgameTFCompletionLevelBase = 8;
        public const decimal XP__EnchantmentMaxXP = 3;
        public const int MaxPlayerLevel = 12;

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
        public const string Permissions_SpellApprover = "spellapprover";

        public const int MaxStrikesPerRound = 3;

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

        public const decimal Item_LevelBonusModifier = .1M;

        public const int MaxAttacksPerUpdate = 3;
        public const decimal LocationMoveCost = 1.0M;
        public const decimal AttackCost = 3.0M;
        public const decimal MeditateCost = 4.0M;
        public const decimal CleanseCost = 4.0M;

        public const float SelfRestoreAPCost = 3;
        public const float SelfRestoreManaCost = 10;
        public const float SelfRestoreTFnergyRequirement = 100;

        public const decimal CleanseManaCost = 3M;
        public const decimal CleanseTFEnergyPercentDecrease = 2.0M;
        public const decimal MeditateManaRestoreBase = 10.0M;
        public const decimal CleanseHealthRestoreBase = 7.0M;
        public const decimal SearchAPCost = 4.0M;

        public const int MaxCarryableItemCountBase = 6;
        public const int MaxCleansesMeditatesPerUpdate = 3;

        public const decimal CriticalMissPercentChance = 8;
        public const decimal CriticalHitPercentChance = 8;

        public const decimal PercentHealthToAllowFullMobilityFormTF = 0.50m;
        public const decimal PercentHealthToAllowMindControlTF = 0.25m;
        public const decimal PercentHealthToAllowAnimalFormTF = 0;
        public const decimal PercentHealthToAllowInanimateFormTF = 0;

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
        public const string ItemType_Rune = "rune";

        public const int ItemType_DungeonArtifact_Id = 218;

        public const int Dungeon_ArtifactCurseEffectSourceId = 102;

        public const decimal DungeonArtifact_Value = 2;
        public const int DungeonArtifact_SpawnLimit = 3;
        public const int DungeonDemonFormSourceId = 371;
        public const int DungeonDemon_Limit = 5;

        public const int Dungeon_VanquishSpellSourceId = 686;

        public const int PsychopathDefaultAmount = 15;
        public const int Covenant_MaximumAnimatePlayerCount = 25;

        public const int Covenant_MinimumUpgradeAnimateLvl3PlayerCount = 5;

        public const int LorekeeperSpellPrice = 50;

        public const int Effect_BackOnYourFeetSourceId = 34; // TODO: duplicate of constant in EffectProcedures?

        public const int PaginationPageSize = 100;

        public const int FriendNicknameMaxLength = 100;

        public const int Spell_WeakenId = 312;
    }

    public static class MindControlStatics
    {
        public const int MindControl__MovementFormSourceId = 293;
        public const int MindControl__Movement_Limit = 2;
        public const int MindControl__Movement_DebuffEffectSourceId = 83;

        public const int MindControl__StripFormSourceId = 358;
        public const int MindControl__Strip_Limit = 1;
        public const int MindControl__Strip_DebuffEffectSourceId = 100;

        public const int MindControl__MeditateFormSourceId = 370;
        public const int MindControl__Meditate_Limit = 2;
    }

    public static class InanimateXPStatics
    {
        public const decimal XPGainPerInanimateAction = 5M;
    }

    public static class ChatStatics
    {
        public const int OnlineActivityCutoffMinutes = -2;
        private const string MizuhoThumbnail = "/Images/PvP/portraits/Thumbnails/100/Mizuho.jpg";

        private static IDictionary<string, PlayerDescriptorDTO> staff
            = new Dictionary<string, PlayerDescriptorDTO>
            {

                {
                    "69", new PlayerDescriptorDTO // Judoo
                    {
                        Name = "Judoo",
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