using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202304011040)]
    public class AddRenameUse : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Players").AddColumn("HasSelfRenamed").AsBoolean().WithDefaultValue(false);
        }
    }
}