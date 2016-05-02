using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201604071120)]
    public class AddPlayer : ForwardOnlyMigration
    {
        public override void Up()
        {
            for (var id = -16; id < 0; id++)
            {
                Update.Table("Players").Set(new {MembershipId = null as object}).Where(new {MembershipId = id.ToString()});
            }
            Create.ForeignKey()
                .FromTable("Players")
                .ForeignColumn("MembershipId")
                .ToTable("AspNetUsers")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Players").AddColumn("NPC").AsInt32().Nullable().ForeignKey("NPCs", "Id");

        }
    }
}
