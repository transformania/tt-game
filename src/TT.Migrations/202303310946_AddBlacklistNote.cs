using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202303310946)]
    public class AddBlacklistNote : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("BlacklistEntries").AddColumn("Note").AsString(100).Nullable();
        }
    }
}
