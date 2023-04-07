using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202304060903)]
    public class AddOnlineToggle : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("OnlineToggle").AsBoolean().WithDefaultValue(false);
        }
    }
}
