using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202312022337)]
    public class LockoutMessage : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AddColumn("AccountLockoutMessage").AsString().WithDefaultValue(string.Empty);
            Alter.Table("AspNetUsers").AddColumn("PvPLockoutMessage").AsString().WithDefaultValue(string.Empty);
            Alter.Table("Players").AddColumn("AbuseLockoutMessage").AsString().WithDefaultValue(string.Empty);
            Alter.Table("Players").AddColumn("ChatLockoutMessage").AsString().WithDefaultValue(string.Empty);
        }
    }
}