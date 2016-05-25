
using System.Data;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201605191700)]
    public class IsPetToNullable : ForwardOnlyMigration
    {

        public override void Up()
        {

            Alter.Table("Players").AlterColumn("IsPetToId").AsInt32().Nullable();

            Update.Table("Players").Set(new { IsPetToId = null as object }).Where(new { IsPetToId = 0 });

            Create.ForeignKey()
                .FromTable("Players")
                .ForeignColumn("IsPetToId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

            

        }

    }
}
