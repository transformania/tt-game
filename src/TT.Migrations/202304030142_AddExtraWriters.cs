using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202304030142)]
    public class AddExtraWriters : ForwardOnlyMigration
    {
        public override void Up()
        {
            // AsString(Int32.MaxValue) wasn't playing nice for whatever reason, and this has been done before. Given it allows for multiple membership IDs to be stored, and each one over 25+ characters long, this seemed appropriate.
            Alter.Table("Contributions").AddColumn("AllowedEditor").AsCustom("nvarchar(MAX)").Nullable();
        }
    }
}
