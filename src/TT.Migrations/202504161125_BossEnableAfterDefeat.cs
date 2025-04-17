using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202504161125)]
    public class BossEnableAfterDefeat : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("BossEnableAfterDefeat").AsBoolean().WithDefaultValue(false);
        }
    }
}
