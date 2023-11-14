using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202311042308)]
    public class QuestFormLockout : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("QuestStarts").AddColumn("PrerequisiteForm").AsInt32().WithDefaultValue(0);
            Alter.Table("QuestStarts").AddColumn("LockoutQuest").AsInt32().WithDefaultValue(0);
        }
    }
}
