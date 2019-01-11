using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901102308)]
    public class ContributionSkillFKs : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Contributions")
               .AddColumn("SkillSourceId").AsInt32().Nullable().ForeignKey("DbStaticSkills", "Id");

            Execute.Sql("UPDATE Contributions SET SkillSourceId = DbStaticSkills.Id FROM DbStaticSkills WHERE ('skill_' + REPLACE(Contributions.Skill_FriendlyName, ' ', '_') + '_' + REPLACE(Contributions.SubmitterName, ' ', '_')) = DbStaticSkills.dbName AND Contributions.ProofreadingCopy = 1");

            Alter.Table("EffectContributions")
                .AddColumn("SkillSourceId").AsInt32().Nullable().ForeignKey("DbStaticSkills", "Id");

            Execute.Sql("UPDATE EffectContributions SET SkillSourceId = DbStaticSkills.Id FROM DbStaticSkills WHERE ('skill_' + REPLACE(EffectContributions.Skill_FriendlyName, ' ', '_') + '_' + REPLACE(EffectContributions.SubmitterName, ' ', '_')) = DbStaticSkills.dbName AND EffectContributions.ProofreadingCopy = 1");

        }
    }
}
