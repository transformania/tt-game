using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202311161838)]
    public class AddCovenMascot : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Covenants").AddColumn("CovenMascot").AsInt32().WithDefaultValue(0);
            Alter.Table("Covenants").AddColumn("CovenBlurb").AsString(250).Nullable();
        }
    }
}
