using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702251000)]
    public class DropEfMigrations : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Table("__MigrationHistory");
        }
    }
}