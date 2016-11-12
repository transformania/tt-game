using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201611121347)]
    public class AddRoundTurnNumber : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Column("RoundNumber")
                .OnTable("PvPWorldStats")
                .AsString()
                .Nullable();
        }
    }
}
