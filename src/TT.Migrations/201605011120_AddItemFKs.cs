using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201605011120)]
    public class AddItemFKs : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            Alter.Table("Items").AlterColumn("OwnerId").AsInt32().Nullable();
            Update.Table("Items").Set(new { OwnerId = null as object }).Where(new { OwnerId = -1 });

            Create.ForeignKey()
                .FromTable("Items")
                .ForeignColumn("OwnerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Alter.Table("Items").AddColumn("ItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE Items SET ItemSourceId = DbStaticItems.Id FROM DBStaticItems WHERE DbStaticItems.DbName = Items.DbName");
            Alter.Table("Items").AlterColumn("ItemSourceId").AsInt32().NotNullable();
        }

    }
}
