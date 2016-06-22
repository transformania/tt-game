
using System.Data;
using FluentMigrator;

namespace TT.Migrations
{

    [Migration(201606221825)]
    public class UpdateSkills : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Skills").AddColumn("SkillSourceId").AsInt32().Nullable().ForeignKey("DbStaticSkills", "Id");
            Execute.Sql("UPDATE Skills SET SkillSourceId = DbStaticSkills.Id FROM DbStaticSkills WHERE DbStaticSkills.dbName = Skills.Name");
            Alter.Table("Skills").AlterColumn("SkillSourceId").AsInt32().NotNullable();

            Create.ForeignKey()
                .FromTable("Skills")
                .ForeignColumn("OwnerId")
                .ToTable("Players")
                .PrimaryColumn("Id").OnDelete(Rule.None);

        }

    }
}
