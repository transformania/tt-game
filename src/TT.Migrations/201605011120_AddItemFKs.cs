using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201604071120)]
    public class AddItemFKs : Migration
    {
        
        public override void Up()
        {

            Create.ForeignKey()
                .FromTable("Items")
                .ForeignColumn("OwnerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Items").AddColumn("ItemSourceId").AsInt32().ForeignKey("DbStaticItems", "Id");

        }

        public override void Down()
        {

        }

    }
}
