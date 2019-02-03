using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201901302126)]
    public class RemoveFakeEffectFKs : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table("EffectContributions").AddColumn("EffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");

            Execute.Sql("UPDATE EffectContributions SET EffectSourceId = DbStaticEffects.Id FROM DbStaticEffects WHERE ('effect_' + REPLACE(EffectContributions.Effect_FriendlyName, ' ', '_') + '_' + REPLACE(EffectContributions.SubmitterName, ' ', '_')) = DbStaticEffects.dbName AND EffectContributions.ProofreadingCopy = 1");

            Alter.Table("DbStaticEffects").AddColumn("PreRequisiteEffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");
            Execute.Sql("UPDATE BaseEffects SET BaseEffects.PrerequisiteEffectSourceId = PrerequisiteEffects.Id FROM DbStaticEffects PrerequisiteEffects, DbStaticEffects BaseEffects WHERE PrerequisiteEffects.dbName = BaseEffects.PreRequesite");

            Alter.Table("DbStaticFurnitures").AddColumn("GivesEffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");
            Execute.Sql("UPDATE DbStaticFurnitures SET GivesEffectSourceId = DbStaticEffects.Id FROM DbStaticEffects WHERE DbStaticEffects.dbName = DbStaticFurnitures.GivesEffect");

            Alter.Table("DbStaticItems").AddColumn("GivesEffectSourceId").AsInt32().Nullable().ForeignKey("DbStaticEffects", "Id");
            Execute.Sql("UPDATE DbStaticItems SET GivesEffectSourceId = DbStaticEffects.Id FROM DbStaticEffects WHERE DbStaticItems.GivesEffect = DbStaticEffects.dbName");
        }
    }
}
