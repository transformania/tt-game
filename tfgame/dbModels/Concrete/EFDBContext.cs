using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class StatsContext : DbContext
    {

        public StatsContext()
           // : base("DefaultConnection")
           : base("StatsWebConnection")
        {

        }

        public DbSet<GameshowStats> GameshowStats { get; set; }
        public DbSet<Character> Characters { get; set; }

        public DbSet<Player> Players { get; set;}
        public DbSet<LocationLog> LocationLogs { get; set;}
        public DbSet<PlayerLog> PlayerLogs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<TFEnergy> TFEnergies { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Message> Messages { get; set; }
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
        public DbSet<Buff> Buffs { get; set; }
        public DbSet<TFMessage> TFMessages { get; set; }
        public DbSet<PlayerExtra> PlayerExtras { get; set; }
        public DbSet<RPPoint> RPPoints { get; set; }
        public DbSet<Donator> Donators { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<MindControl> MindControls { get; set; }
        public DbSet<BookReading> BookReadings { get; set; }
        public DbSet<JewdewfaeEncounter> JewdewfaeEncounters { get; set; }

        public DbSet<DbStaticFurniture> DbStaticFurniture { get; set; }
        public DbSet<CovenantLog> CovenantLogs { get; set; }
        public DbSet<ItemTransferLog> ItemTransferLogs { get; set; }

        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<DMRoll> DMRolls { get; set; }
        public DbSet<PollEntry> PollEntries { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
    }
}