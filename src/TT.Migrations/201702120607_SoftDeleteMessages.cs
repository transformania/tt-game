using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702120607)]
    public class SoftDeleteMessages : ForwardOnlyMigration
    {

        public override void Up()
        {
            Alter.Table("Messages").AddColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("Messages").AddColumn("IsReportedAbusive").AsBoolean().NotNullable().WithDefaultValue(false);
        }

    }
}