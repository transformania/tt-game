using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202504081017)]
    public class AddBossDisable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("BossDisable").AsBoolean().WithDefaultValue(false);
        }
    }
}
