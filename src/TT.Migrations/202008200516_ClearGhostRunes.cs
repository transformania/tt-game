using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202008200516)]
    public class ClearGhostRunes : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql("Update Items SET OwnerId = NULL, dbLocationName = '', IsEquipped = 'FALSE' WHERE EmbeddedOnItemId IS NOT NULL");
        }
    }
}
