using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702260039)]
    public class DropPreRound1Tables : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Table("Buffs");
            Delete.Table("Characters");
            Delete.Table("PlayerQuests");
            Delete.Table("RPPoints");
        }
    }
}