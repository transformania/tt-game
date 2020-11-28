using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202011271146)]
    public class AddHideColumn : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("PlayerLogs").AddColumn("HideLog").AsBoolean().WithDefaultValue(false);
        }
    }
}
