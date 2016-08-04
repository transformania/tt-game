using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201608031909)]
    public class AddRecaptchaEntry : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("CaptchaEntries")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("User_Id").AsString(128).NotNullable().ForeignKey("AspNetUsers", "Id").Indexed()
                .WithColumn("ExpirationTimestamp").AsDateTime()
                .WithColumn("TimesPassed").AsInt32()
                .WithColumn("TimesFailed").AsInt32();
        }
    }
}
