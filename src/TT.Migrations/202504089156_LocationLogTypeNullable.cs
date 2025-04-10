using FluentMigrator;
using System;

namespace TT.Migrations
{
    [Migration(202504089156)]
    public class LocationLogTypeNullable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("LocationLogs").AddColumn("LogType").AsInt32().Nullable(); ;
        }
    }
}
