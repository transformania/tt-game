using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201612280045)]
    public class ItemFKToPlayer : ForwardOnlyMigration
    {

        public override void Up()
        {

            Alter.Table("Items").AddColumn("FormerPlayerId").AsInt32().Nullable().ForeignKey("Players", "Id");
            Execute.Sql("UPDATE Items SET FormerPlayerId = Players.Id FROM Players WHERE (Players.FirstName + ' ' + Players.LastName) = Items.VictimName");

            Create.Index("ix_FormerPlayerId").OnTable("Items").OnColumn("FormerPlayerId").Ascending().WithOptions().NonClustered();
        }

    }
}
