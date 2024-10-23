using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202410221107)]
    public class AddApproval : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Set existing users as true.
            Alter.Table("AspNetUsers").AddColumn("Approved").AsBoolean().Nullable();
            Execute.Sql("UPDATE AspNetUsers SET Approved = 1 WHERE CreateDate < '2024-10-15' OR CreateDate IS NULL");
        }
    }
}
