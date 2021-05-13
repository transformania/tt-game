using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202105110954)]
    public class UpdateReusables : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Health and Mana items
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '0', ReuseableManaRestore = '150' WHERE id = '21'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '725', ReuseableManaRestore = '0' WHERE id = '30'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '-100', ReuseableManaRestore = '200' WHERE id = '40'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '300', ReuseableManaRestore = '0' WHERE id = '48'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '400', ReuseableManaRestore = '-75' WHERE id = '82'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '363', ReuseableManaRestore = '126' WHERE id = '286'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '9', ReuseableHealthRestore = '525', ReuseableManaRestore = '100' WHERE id = '290'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '300', ReuseableManaRestore = '0' WHERE id = '361'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '0', ReuseableManaRestore = '252' WHERE id = '379'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '800', ReuseableManaRestore = '-100' WHERE id = '440'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '9', ReuseableHealthRestore = '525', ReuseableManaRestore = '100' WHERE id = '629'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '0', ReuseableManaRestore = '252' WHERE id = '657'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '725', ReuseableManaRestore = '0' WHERE id = '658'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '5', ReuseableHealthRestore = '300', ReuseableManaRestore = '0' WHERE id = '659'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12', ReuseableHealthRestore = '725', ReuseableManaRestore = '0' WHERE id = '665'");

            // Item effects.
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '60', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '61'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '8', Perception = '8', Charisma = '8', Fortitude = '8', Agility = '8', Allure = '8', Magicka = '8', Succour = '8', Luck = '8' WHERE id = '103'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '20', Perception = '0', Charisma = '20', Fortitude = '0', Agility = '0', Allure = '20', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '163'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '60', Succour = '0', Luck = '0' WHERE id = '167'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '-20', Allure = '80', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '174'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '60', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '178'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '120', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '179'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '0', Succour = '60', Luck = '0' WHERE id = '180'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '60', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '185'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '80', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '0', Succour = '-20', Luck = '0' WHERE id = '186'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '30', Succour = '0', Luck = '30' WHERE id = '192'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '0', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '60' WHERE id = '193'");
            Execute.Sql("Update DbStaticEffects SET Discipline = '30', Perception = '0', Charisma = '0', Fortitude = '30', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '195'");

            // Cooldowns of items with effects.
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '153'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '9' WHERE id = '192'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '224'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '260'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '284'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '291'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '317'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '322'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '360'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '385'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '404'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '561'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '579'");
            Execute.Sql("Update DbStaticItems SET UseCooldown = '12' WHERE id = '590'");
        }
    }
}
