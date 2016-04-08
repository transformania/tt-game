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

            Create.ForeignKey()
                .FromTable("Players")
                .ForeignColumn("NPC")
                .ToTable("NPCs")
                .PrimaryColumn("Id").OnDelete(Rule.None);
        }

        public override void Down()
        {

        }

    }
}
