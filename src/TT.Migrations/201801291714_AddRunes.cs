using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201801291714)]
    public class AddRunes : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Items").AddColumn("EmbeddedOnItemId").AsInt32().Nullable().ForeignKey("Items", "Id");
            Alter.Table("DbStaticItems").AddColumn("RuneLevel").AsInt32().Nullable();

            Create.Index("ix_EmbeddedOnItemId").OnTable("Items").OnColumn("EmbeddedOnItemId").Ascending().WithOptions().NonClustered();
        }
    }
}
