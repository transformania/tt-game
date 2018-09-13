using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201809102113)]
    public class PlayerReports : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Reports")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("Reporter").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("Reported").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("Timestamp").AsCustom("datetime2").NotNullable()
                .WithColumn("Reason").AsString()
                .WithColumn("ModeratorResponse").AsString()
                .WithColumn("Round").AsInt32().NotNullable();
        }
    }
}
