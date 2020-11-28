using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202011280936)]
    public class UpdateKrampusVendors : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Bosses
            // The Krampus, formerly known as Valentine
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '200', Perception = '25', Charisma = '35', Fortitude = '50', Agility = '40', Allure = '0', Magicka = '20', Succour = '0',Luck = '40' Where Id = 739");

            // Vendors
            // Pervfae
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 210");
            // Puppie
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 286");
            // The Sunnyglade Tourism Council Representative
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 400");
            // Good ol' Rusty
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 403");
            // Snek
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 467");
            // Succubutt - That's a lot of zeros. Let's add a few ones into there.
            Execute.Sql("Update DbStaticForms set HealthBonusPercent = '10000', Discipline = '10000', Perception = '10000', Charisma = '10000', Fortitude = '10000', Agility = '10000', Allure = '10000', Magicka = '10000', Succour = '10000',Luck = '10000' Where Id = 1000");

        }
    }
}
