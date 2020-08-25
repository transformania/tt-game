using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202008251206)]
    public class AddCovenNoticeboardMessage : ForwardOnlyMigration
    {
        public override void Up()
        {
            Rename.Column("formerMembers").OnTable("Covenants").To("NoticeboardMessage");
        }
    }
}