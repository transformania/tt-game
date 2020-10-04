using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202009261652)]
    public class UpdateHybridStats : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Fox
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '10', Fortitude = '10', Agility = '10', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '525'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '11', Fortitude = '11', Agility = '11', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '526'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '12', Fortitude = '12', Agility = '12', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '527'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '13', Fortitude = '13', Agility = '13', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '528'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '14', Fortitude = '14', Agility = '14', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '529'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '15', Agility = '15', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '530'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of the Fox', Discipline = '0', Perception = '0', Charisma = '16', Fortitude = '16', Agility = '16', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '531'");

            // Guardian
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of the Guardian', Discipline = '10', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '10', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '532'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of the Guardian', Discipline = '11', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '11', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '533'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of the Guardian', Discipline = '12', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '12', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '534'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of the Guardian', Discipline = '13', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '13', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '535'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of the Guardian', Discipline = '14', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '14', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '536'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of the Guardian', Discipline = '15', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '15', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '537'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of the Guardian', Discipline = '16', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '16', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '538'");

            // Enchantress
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '10', Succour = '0', Luck = '10' WHERE id = '539'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '11', Succour = '0', Luck = '11' WHERE id = '540'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '12', Succour = '0', Luck = '12' WHERE id = '541'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of The Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '13', Succour = '0', Luck = '13' WHERE id = '542'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '14', Succour = '0', Luck = '14' WHERE id = '543'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '15', Succour = '0', Luck = '15' WHERE id = '544'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of the Enchantress', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '16', Succour = '0', Luck = '16' WHERE id = '545'");

            // Healer -> Renamed to Acrobat
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '10', Allure = '0', Magicka = '0', Succour = '0', Luck = '10' WHERE id = '546'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '11', Allure = '0', Magicka = '0', Succour = '0', Luck = '11' WHERE id = '547'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '12', Allure = '0', Magicka = '0', Succour = '0', Luck = '12' WHERE id = '548'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '13', Allure = '0', Magicka = '0', Succour = '0', Luck = '13' WHERE id = '549'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '14', Allure = '0', Magicka = '0', Succour = '0', Luck = '14' WHERE id = '550'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '15', Allure = '0', Magicka = '0', Succour = '0', Luck = '15' WHERE id = '551'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of the Acrobat', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '16', Allure = '0', Magicka = '0', Succour = '0', Luck = '16' WHERE id = '552'");

            // Special Runes
            // Miniboss - Housemother
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '5', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '15' WHERE id = '564'");
            // Miniboss - Musician
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '5', Agility = '15', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '565'");
            // Miniboss - Maid
            Execute.Sql("Update DbStaticItems SET Discipline = '15', Perception = '0', Charisma = '0', Fortitude = '5', Agility = '0', Allure = '15', Magicka = '0', Succour = '0', Luck = '0'  WHERE id = '566'");
            // Miniboss - Groundskeeper
            Execute.Sql("Update DbStaticItems SET Discipline = '15', Perception = '0', Charisma = '0', Fortitude = '5', Agility = '0', Allure = '0', Magicka = '15', Succour = '0', Luck = '0'  WHERE id = '573'");
            // Miniboss - Threadmistress
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '5', Agility = '0', Allure = '0', Magicka = '15', Succour = '0', Luck = '15' WHERE id = '567'");
            // Miniboss - Professor
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '15', Charisma = '0', Fortitude = '5', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '15' WHERE id = '574'");

            // Boss - Rats
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '15', Allure = '0', Magicka = '0', Succour = '0', Luck = '10' WHERE id = '513'");
            // Boss - Road Lady
            Execute.Sql("Update DbStaticItems SET Discipline = '10', Perception = '0', Charisma = '15', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '557'");
            // Boss - Bimboss 
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '10', Succour = '0', Luck = '0'   WHERE id = '518'");
            // Boss - Donna
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '15', Succour = '0', Luck = '10'   WHERE id = '515'");
            // Boss - Narcissa
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '10'   WHERE id = '517'");
            // Boss - The smart one
            Execute.Sql("Update DbStaticItems SET Discipline = '15', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '10', Luck = '0'   WHERE id = '516'");
        }
    }
}
