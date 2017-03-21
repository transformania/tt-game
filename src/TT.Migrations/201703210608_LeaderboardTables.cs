using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201703210608)]
    public class LeaderboardTables : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("PvPLeaderboards")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("PlayerName").AsString().NotNullable()
                .WithColumn("Sex").AsString().NotNullable()
                .WithColumn("CovenantName").AsString().Nullable()
                .WithColumn("FormName").AsString().NotNullable()
                .WithColumn("Mobility").AsString().NotNullable()
                .WithColumn("FormSourceId").AsInt32().NotNullable().ForeignKey("DbStaticForms", "Id").Indexed()
                .WithColumn("Level").AsInt32().NotNullable()
                .WithColumn("DungeonPoints").AsInt32().NotNullable()
                .WithColumn("Rank").AsInt32().NotNullable()
                .WithColumn("RoundNumber").AsInt32().NotNullable().Indexed();

            Create.Table("XPLeaderboards")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("PlayerName").AsString().NotNullable()
                .WithColumn("Sex").AsString().NotNullable()
                .WithColumn("GameMode").AsInt32().Nullable()
                .WithColumn("CovenantName").AsString().Nullable()
                .WithColumn("FormName").AsString().NotNullable()
                .WithColumn("Mobility").AsString().NotNullable()
                .WithColumn("FormSourceId").AsInt32().NotNullable().ForeignKey("DbStaticForms", "Id").Indexed()
                .WithColumn("Level").AsInt32().NotNullable()
                .WithColumn("XP").AsFloat().NotNullable()
                .WithColumn("Rank").AsInt32().NotNullable()
                .WithColumn("RoundNumber").AsInt32().NotNullable().Indexed();

            Create.Table("ItemLeaderboards")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("PlayerName").AsString().NotNullable()
                .WithColumn("Sex").AsString().NotNullable()
                .WithColumn("ItemName").AsString().NotNullable()
                .WithColumn("ItemType").AsString()
                .WithColumn("GameMode").AsInt32().Nullable()
                .WithColumn("ItemSourceId").AsInt32().NotNullable().ForeignKey("DbStaticItems", "Id").Indexed()
                .WithColumn("Level").AsInt32().NotNullable()
                .WithColumn("XP").AsFloat().NotNullable()
                .WithColumn("Rank").AsInt32().NotNullable()
                .WithColumn("CovenantName").AsString().Nullable()
                .WithColumn("RoundNumber").AsInt32().NotNullable().Indexed();

        }
    }
}
