using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202005231604)]
    public class AddMoreBuffBox : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("SneakPercent").AsDecimal().WithDefaultValue(0);
            Alter.Table("Players").AddColumn("MoveActionPointDiscount").AsDecimal().WithDefaultValue(0);
        }
    }
}