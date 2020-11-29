using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class StatsContext : DbContext
    {
        public StatsContext()
            : base("StatsWebConnection")
        {
            Database.SetInitializer(new NullDatabaseInitializer<StatsContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<GameshowStats> GameshowStats { get; set; }
        public DbSet<Player> Players { get; set;}
        public DbSet<LocationLog> LocationLogs { get; set;}
        public DbSet<PlayerLog> PlayerLogs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Models.TFEnergy> TFEnergies { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<PvPWorldStat> PvPWorldStats { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<LocationInfo> LocationInfos { get; set; }
        public DbSet<BlacklistEntry> BlacklistEntries { get; set; }
        public DbSet<InanimateXP> InanimateXPs { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<EffectContribution> EffectContributions { get; set; }
        public DbSet<Covenant> Covenants { get; set; }
        public DbSet<CovenantApplication> CovenantApplications { get; set; }
        public DbSet<ReservedName> ReservedNames { get; set; }
        public DbSet<AIDirective> AIDirectives { get; set; }
        public DbSet<PlayerBio> PlayerBios { get; set; }
        public DbSet<AuthorArtistBio> AuthorArtistBios { get; set; }
        public DbSet<DbStaticSkill> DbStaticSkills { get; set; }
        public DbSet<DbStaticForm> DbStaticForms { get; set; }
        public DbSet<DbStaticItem> DbStaticItems { get; set; }
        public DbSet<DbStaticEffect> DbStaticEffects { get; set; }
        public DbSet<ServerLog> ServerLogs { get; set; }
        public DbSet<TFMessage> TFMessages { get; set; }
        public DbSet<PlayerExtra> PlayerExtras { get; set; }
        public DbSet<Donator> Donators { get; set; }
        public DbSet<ContributorCustomForm> ContributorCustomForms { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<MindControl> MindControls { get; set; }
        public DbSet<BookReading> BookReadings { get; set; }
        public DbSet<JewdewfaeEncounter> JewdewfaeEncounters { get; set; }
        public DbSet<BossDamage> BossDamages { get; set; }
        public DbSet<RPClassifiedAd> RPClassifiedAds { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }

        public DbSet<DbStaticFurniture> DbStaticFurniture { get; set; }
        public DbSet<CovenantLog> CovenantLogs { get; set; }
        public DbSet<ItemTransferLog> ItemTransferLogs { get; set; }

        public DbSet<Reroll> Rerolls { get; set; }

        public DbSet<DMRoll> DMRolls { get; set; }
        public DbSet<PollEntry> PollEntries { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<AchievementBadge> AchievementBadges { get; set; }
        public DbSet<Duel> Duels { get; set; }
        public DbSet<QuestStart> QuestStarts { get; set; }
        public DbSet<QuestState> QuestStates { get; set; }
        public DbSet<QuestConnection> QuestConnections { get; set; }
        public DbSet<QuestConnectionRequirement> QuestConnectionRequirements { get; set; }
        public DbSet<QuestEnd> QuestEnds { get; set; }
        public DbSet<QuestStatePreaction> QuestStatePreactions { get; set; }
        public DbSet<QuestWriterLog> QuestWriterLogs { get; set; }
        public DbSet<QuestPlayerStatus> QuestPlayerStatuses { get; set; }
        public DbSet<QuestPlayerVariable> QuestPlayerVariables { get; set; }
        public DbSet<QuestWriterPermission> QuestWriterPermissions { get; set; }

        public DbSet<Models.SelfRestoreEnergies> SelfRestoreEnergies { get; set; }

    }
}