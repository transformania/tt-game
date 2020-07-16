using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202007132133)]
    public class UpdateNPCStatsSnek : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Psychopaths
            // Level 1
            Execute.Sql("Update DbStaticEffects set Discipline = '-25', Perception = '-15', Charisma = '20', Fortitude = '-25', Agility = '5', Allure = '5', Magicka = '10', Succour = '5', Luck = '10' Where Id = 19 ");
            // Level 3
            Execute.Sql("Update DbStaticEffects set Discipline = '0', Perception = '0', Charisma = '40', Fortitude = '0', Agility = '10', Allure = '10', Magicka = '20', Succour = '10', Luck = '20' Where Id = 20");
            // Level 5
            Execute.Sql("Update DbStaticEffects set Discipline = '25', Perception = '15', Charisma = '60', Fortitude = '25', Agility = '15', Allure = '15', Magicka = '30', Succour = '15', Luck = '30' Where Id = 21");
            // Level 7
            Execute.Sql("Update DbStaticEffects set Discipline = '50', Perception = '30', Charisma = '80', Fortitude = '50', Agility = '20', Allure = '20', Magicka = '40', Succour = '20', Luck = '40' Where Id = 22");
            // Level 9
            Execute.Sql("Update DbStaticEffects set Discipline = '75', Perception = '45', Charisma = '100', Fortitude = '75', Agility = '25', Allure = '25', Magicka = '50', Succour = '25', Luck = '40' Where Id = 23");

            // Minibosses
            // Pop Goddess
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '0', Perception = '25', Charisma = '150', Fortitude = '50', Agility = '100', Allure = '0', Magicka = '25', Succour = '25', Luck = '0' Where Id = 956");
            // Sorority House Mother
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '25', Perception = '25', Charisma = '150', Fortitude = '50', Agility = '25', Allure = '0', Magicka = '25', Succour = '25', Luck = '50' Where Id = 957");
            // Possessed Maid
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '100', Perception = '25', Charisma = '75', Fortitude = '50', Agility = '0', Allure = '0', Magicka = '50', Succour = '25', Luck = '0' Where Id = 958");
            // Sanguine Seamstress
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '0', Perception = '50', Charisma = '75', Fortitude = '75', Agility = '50', Allure = '0', Magicka = '50', Succour = '25', Luck = '25' Where Id = 959");
            // Groundskeeper
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '50', Perception = '25', Charisma = '75', Fortitude = '100', Agility = '0', Allure = '0', Magicka = '50',Succour = '25', Luck = '0' Where Id = 976");

            // Bossess
            // Master Shadowseek Thief
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '800', Discipline = '0', Perception = '30', Charisma = '25', Fortitude = '50', Agility = '50', Allure = '0', Magicka = '0', Succour = '0',Luck = '50' Where Id = 279");
            // Apprentice Shadowseek Thief
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '600', Discipline = '-25', Perception = '0', Charisma = '0', Fortitude = '25', Agility = '25', Allure = '0', Magicka = '0', Succour = '0', Luck = '25' Where Id = 278");
            // Road Queen
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '1800', Discipline = '50', Perception = '0', Charisma = '50', Fortitude = '50', Agility = '25', Allure = '0', Magicka = '0', Succour = '0',Luck = '35' Where Id = 934");
            // Bimbonic Plague Mother
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '2600', Discipline = '0', Perception = '50', Charisma = '100', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '50', Succour = '0', Luck = '0' Where Id = 233");
            // Mythical Sorceress
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '3100', Discipline = '100', Perception = '0', Charisma = '150', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '50', Succour = '0', Luck = '25' Where Id = 287");
            // Headmistress of SCCC
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '3600', Discipline = '50', Perception = '50', Charisma = '100', Fortitude = '50', Agility = '0', Allure = '0', Magicka = '50', Succour = '0', Luck = '15' Where Id = 317");
            // Head Beautician of Blazes and Glamour
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '3600', Discipline = '0', Perception = '50', Charisma = '100', Fortitude = '50', Agility = '50', Allure = '0', Magicka = '50', Succour = '0', Luck = '15' Where Id = 522");
            // Corrupted Lunar Fae
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '4000', Discipline = '0', Perception = '0', Charisma = '150', Fortitude = '200', Agility = '100', Allure = '0', Magicka = '50', Succour = '0', Luck = '30' Where Id = 582");

            // Dungeon NPC's aren't going to be included in this as I want to look at the dungeon overall rather than just tweaking a few things and hoping for the best
            // Similarly vendors aren't included as there's no reason they should be used in any normal setting

            // Update base spelldamage
            // TFEnergy builders
            Execute.Sql("Update DbStaticSkills set HealthDamageAmount = '50' where HealthDamageAmount = '10'");
            // Weaken
            Execute.Sql("Update DbStaticSkills set HealthDamageAmount = '100' where HealthDamageAmount = '20'");
        }
    }
}
