using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702190840)]
    public class UserStrikes : ForwardOnlyMigration
    {

        public override void Up()
        {
            Create.Table("Strikes")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("UserMembershipId").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("FromModerator").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("Timestamp").AsCustom("datetime2").NotNullable()
                .WithColumn("Reason").AsString()
                .WithColumn("Round").AsInt32().NotNullable();
        }

    }
}