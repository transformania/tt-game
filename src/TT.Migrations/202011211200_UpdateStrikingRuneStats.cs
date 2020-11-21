using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202011211200)]
    public class UpdateStrikingRuneStats : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Striking',     Discipline = '0', Perception = '0', Charisma = '10', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '10' WHERE id = '449'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Striking',  Discipline = '0', Perception = '0', Charisma = '11', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '11' WHERE id = '458'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Striking',     Discipline = '0', Perception = '0', Charisma = '12', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '12' WHERE id = '476'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Striking',     Discipline = '0', Perception = '0', Charisma = '13', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '13' WHERE id = '467'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Striking',  Discipline = '0', Perception = '0', Charisma = '14', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '14' WHERE id = '507'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Striking',   Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '15' WHERE id = '485'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Striking', Discipline = '0', Perception = '0', Charisma = '16', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '16' WHERE id = '494'");
        }
    }
}
