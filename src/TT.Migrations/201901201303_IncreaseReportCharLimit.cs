using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901201303)]
    public class IncreaseReportCharLimit : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Reports").AlterColumn("Reason").AsString(Int32.MaxValue).Nullable();
            Alter.Table("Reports").AlterColumn("ModeratorResponse").AsString(Int32.MaxValue).Nullable();
        }
    }
}
