using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202210232200)]
    public class HardmodeAddition : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("InHardmode").AsBoolean().WithDefaultValue(false);
        }
    }
}