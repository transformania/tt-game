using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901211044)]
    public class AddTurnTimeStatics : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("PvPWorldStats").AddColumn("TurnTimeConfiguration").AsString(Int32.MaxValue).Nullable();
            Execute.Sql("UPDATE PvPWorldStats SET TurnTimeConfiguration = '5min'");

        }
    }
}
