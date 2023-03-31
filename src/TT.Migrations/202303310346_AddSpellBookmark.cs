using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202303310346)]
    public class AddSpellBookmark : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Skills").AddColumn("Bookmarked").AsBoolean().WithDefaultValue(false);
        }
    }
}