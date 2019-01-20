using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901131059)]
    public class RemoveFakeFormFKs : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table("Players").AddColumn("OriginalFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE Players SET OriginalFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = Players.OriginalForm");
            Alter.Table("Players").AlterColumn("OriginalFormSourceId").AsInt32().NotNullable();

            Alter.Table("DbStaticItems").AddColumn("CurseTFFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE DbStaticItems SET CurseTFFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = DbStaticItems.CurseTFFormdbName");

            Alter.Table("JewdewfaeEncounters").AddColumn("RequiredFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE JewdewfaeEncounters SET RequiredFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = JewdewfaeEncounters.RequiredForm");
            Alter.Table("JewdewfaeEncounters").AlterColumn("RequiredFormSourceId").AsInt32().NotNullable();
            Delete.Column("RequiredForm").FromTable("JewdewfaeEncounters");

            Alter.Table("DuelCombatants").AddColumn("StartFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE DuelCombatants SET StartFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = DuelCombatants.StartForm");
            Delete.Column("StartForm").FromTable("DuelCombatants");

            Alter.Table("Contributions").AddColumn("FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");

            Execute.Sql("UPDATE Contributions SET FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE ('form_' + REPLACE(Contributions.Form_FriendlyName, ' ', '_') + '_' + REPLACE(Contributions.SubmitterName, ' ', '_')) = DbStaticForms.dbName AND Contributions.ProofreadingCopy = 1");

            Alter.Table("EffectContributions").AddColumn("Skill_UniqueToFormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE EffectContributions SET Skill_UniqueToFormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE DbStaticForms.dbName = EffectContributions.Skill_UniqueToForm");

            Alter.Table("Contributions").AddColumn("CursedTF_FormSourceId").AsInt32().Nullable().ForeignKey("DbStaticForms", "Id");
            Execute.Sql("UPDATE Contributions SET CursedTF_FormSourceId = DbStaticForms.Id FROM DbStaticForms WHERE Contributions.CursedTF_FormdbName = DbStaticForms.dbName AND Contributions.ProofreadingCopy = 1");

            Create.Table("SelfRestoreEnergies")
                .WithColumn("Id").AsInt32().Identity().NotNullable().PrimaryKey()
                .WithColumn("OwnerId").AsInt32().NotNullable().ForeignKey("Players", "Id")
                .WithColumn("Amount").AsFloat().NotNullable()
                .WithColumn("Timestamp").AsCustom("datetime2").NotNullable();
        }
    }
}
