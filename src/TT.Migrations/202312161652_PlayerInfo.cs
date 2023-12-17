using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202312161652)]
    public class PlayerInfo : ForwardOnlyMigration
    {
        public override void Up()
        {
            Rename.Column("WebsiteURL").OnTable("PlayerBios").To("PlayerInfo");
        }
    }
}