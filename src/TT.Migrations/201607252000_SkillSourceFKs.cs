using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201607252000)]
    public class SkillSourceFKs : ForwardOnlyMigration
    {

        public override void Up()
        {

            Alter.Table("DbStaticSkills").AddColumn("GivesEffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");
            Execute.Sql("UPDATE DbStaticSkills SET GivesEffectSourceId = DbStaticEffects.Id FROM DbStaticEffects WHERE DbStaticSkills.GivesEffect = DbStaticEffects.dbName");

            Alter.Table("DbStaticSkills").AddColumn("ExclusiveToFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE DbStaticSkills SET ExclusiveToFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticSkills.ExclusiveToForm = DbStaticForms.dbName");

            Alter.Table("DbStaticSkills").AddColumn("ExclusiveToItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE DbStaticSkills SET ExclusiveToItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE DbStaticSkills.ExclusiveToItem = DbStaticItems.dbName");

            Alter.Table("DbStaticSkills").AddColumn("FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE DbStaticSkills SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticSkills.FormdbName = DbStaticForms.dbName");

        }

    }
}
