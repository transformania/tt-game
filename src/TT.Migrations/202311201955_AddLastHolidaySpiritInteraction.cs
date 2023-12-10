using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202311201955)]
    public class AddLastHolidaySpiritInteraction : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("LastHolidaySpiritInteraction").AsInt32().WithDefaultValue(0);
        }
    }
}