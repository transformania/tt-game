using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201907260553)]
    public class AddExtraInventory : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("ExtraInventory").AsInt32().WithDefaultValue(0);
        }
    }
}
