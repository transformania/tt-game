using FluentMigrator;
using System;

namespace TT.Migrations
{
    [Migration(202404282334)]
    public class LockoutMessageNullable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("AspNetUsers").AlterColumn("AccountLockoutMessage").AsString().Nullable();
            Alter.Table("AspNetUsers").AlterColumn("PvPLockoutMessage").AsString().Nullable();
            Alter.Table("Players").AlterColumn("AbuseLockoutMessage").AsString().Nullable();
            Alter.Table("Players").AlterColumn("ChatLockoutMessage").AsString().Nullable();
            Delete.DefaultConstraint().OnTable("AspNetUsers").OnColumn("AccountLockoutMessage");
            Delete.DefaultConstraint().OnTable("AspNetUsers").OnColumn("PvPLockoutMessage");
            Delete.DefaultConstraint().OnTable("Players").OnColumn("AbuseLockoutMessage");
            Delete.DefaultConstraint().OnTable("Players").OnColumn("ChatLockoutMessage");
            Update.Table("AspNetUsers").Set(new {AccountLockoutMessage = DBNull.Value}).Where(new {AccountLockoutMessage = string.Empty});
            Update.Table("AspNetUsers").Set(new {PvPLockoutMessage = DBNull.Value}).Where(new {PvPLockoutMessage = string.Empty});
            Update.Table("Players").Set(new {AbuseLockoutMessage = DBNull.Value}).Where(new {AbuseLockoutMessage = string.Empty});
            Update.Table("Players").Set(new {ChatLockoutMessage = DBNull.Value}).Where(new {ChatLockoutMessage = string.Empty});
        }
    }
}
