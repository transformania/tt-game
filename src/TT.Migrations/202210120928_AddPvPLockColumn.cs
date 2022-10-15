using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202210120928)]
    public class AddPvPLockColumn : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("PvPLock").AsBoolean().WithDefaultValue(false);
        }
    }
}