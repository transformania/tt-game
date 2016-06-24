using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201606241348)]
    public class AddPlayerLogFKs : ForwardOnlyMigration
    {

        public override void Up()
        {
            Execute.Sql("DELETE FROM PlayerLogs WHERE PlayerLogs.PlayerId <= 0");

            Create.ForeignKey()
                .FromTable("PlayerLogs")
                .ForeignColumn("PlayerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);
        }

    }
}
