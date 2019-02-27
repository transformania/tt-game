using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201902172143)]
    public class SoulbindItems : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Items").AddColumn("SoulboundToPlayerId").AsInt32().Nullable().ForeignKey("Players", "Id");
        }
    }
}
