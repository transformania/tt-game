using System;
using FluentMigrator;

namespace TT.Migrations
{
    [Migration(201902031713)]
    public class RemoveFakeItemFK : ForwardOnlyMigration
    {
        public override void Up()
        {

            Alter.Table("Contributions").AddColumn("ItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE Contributions SET ItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE ('item_' + REPLACE(Contributions.Item_FriendlyName, ' ', '_') + '_' + REPLACE(Contributions.SubmitterName, ' ', '_')) = DbStaticItems.dbName AND Contributions.ProofreadingCopy = 1");
            Execute.Sql("UPDATE Contributions SET ItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE ('animal_' + REPLACE(Contributions.Item_FriendlyName, ' ', '_') + '_' + REPLACE(Contributions.SubmitterName, ' ', '_')) = DbStaticItems.dbName AND Contributions.ProofreadingCopy = 1");

            Alter.Table("DbStaticForms").AddColumn("BecomesItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE DbStaticForms SET BecomesItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE DbStaticForms.BecomesItemDbName = DbStaticItems.dbName");

            Alter.Table("DbStaticFurnitures").AddColumn("GivesItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE DbStaticFurnitures SET GivesItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE DbStaticFurnitures.GivesItem = DbStaticItems.dbName");

            Alter.Table("EffectContributions").AddColumn("Skill_UniqueToItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE EffectContributions SET Skill_UniqueToItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE EffectContributions.Skill_UniqueToItem = DbStaticItems.dbName");

            Alter.Table("BookReadings").AddColumn("BookItemSourceId").AsInt32().Nullable().ForeignKey("DbStaticItems", "Id");
            Execute.Sql("UPDATE BookReadings SET BookItemSourceId = DbStaticItems.Id FROM DbStaticItems WHERE BookReadings.BookDbName = DbStaticItems.dbName");

            Alter.Table("DbStaticItems").AddColumn("ConsumableSubItemType").AsInt32().Nullable();
            Execute.Sql("UPDATE DbStaticItems SET ConsumableSubItemType = 0 WHERE DbStaticItems.dbName LIKE 'rune_%'");
            Execute.Sql("UPDATE DbStaticItems SET ConsumableSubItemType = 1 WHERE DbStaticItems.dbName LIKE 'item_consumable_tome-%'");
            Execute.Sql("UPDATE DbStaticItems SET ConsumableSubItemType = 2 WHERE DbStaticItems.dbName LIKE 'item_consumable_spellbook_%'");
            Execute.Sql("UPDATE DbStaticItems SET ConsumableSubItemType = 4 WHERE DbStaticItems.dbName LIKE 'item_consumeable_willpower_bomb_%'");
        }
    }
}
