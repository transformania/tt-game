using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202103070000)]
    public class JokeShopToggle : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("PvPWorldStats").AddColumn("JokeShop").AsBoolean().WithDefaultValue(false);
        }
    }
}
