using FluentMigrator;
using System.Data;

namespace TT.Migrations
{
    [Migration(201605030603)]
    public class UpdateMessages : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table("Messages").AddColumn("ConversationId").AsInt32().Nullable();

            Create.ForeignKey()
                .FromTable("Messages")
                .ForeignColumn("SenderId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            Create.ForeignKey()
                .FromTable("Messages")
                .ForeignColumn("ReceiverId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

        }
    }
}
