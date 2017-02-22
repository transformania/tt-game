using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201702211090)]
    public class RestockItemBotId : ForwardOnlyMigration
    {

        public override void Up()
        {
            Create.Column("BotId")
                .OnTable("RestockItems")
                .AsInt32()
                .NotNullable();

            Delete.Index("ix_NPC_Id").OnTable("RestockItems");
            Delete.ForeignKey("FK_RestockItems_NPC_Id_NPCs_Id").OnTable("RestockItems");

            Delete.Column("NPC_Id").FromTable("RestockItems");
        }

    }
}