using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201804260056)]
    public class RemoveVictimName : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Column("VictimName").FromTable("Items");
            Delete.Column("Nickname").FromTable("Items");
        }
    }
}