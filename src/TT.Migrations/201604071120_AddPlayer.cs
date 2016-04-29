using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201604071120)]
    public class AddPlayer : Migration
    {
        

        public override void Up()
        {
            Create.ForeignKey()
                .FromTable("Players")
                .ForeignColumn("MembershipId")
                .ToTable("AspNetUsers")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Players").AddColumn("NPC").AsInt32().Nullable().ForeignKey("NPCs", "Id");

        }

        public override void Down()
        {

        }

    }
}
