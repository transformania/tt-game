using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202007270924)]
    public class UpdateRuneStats : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Base Runes
            // Charisma
            Execute.Sql("Update DbStaticItems SET Charisma = '20', Fortitude = '10' WHERE id = '446'");
            Execute.Sql("Update DbStaticItems SET Charisma = '22', Fortitude = '11' WHERE id = '455'");
            Execute.Sql("Update DbStaticItems SET Charisma = '24', Fortitude = '12' WHERE id = '473'");
            Execute.Sql("Update DbStaticItems SET Charisma = '26', Fortitude = '13' WHERE id = '464'");
            Execute.Sql("Update DbStaticItems SET Charisma = '28', Fortitude = '14' WHERE id = '504'");
            Execute.Sql("Update DbStaticItems SET Charisma = '30', Fortitude = '15' WHERE id = '482'");
            Execute.Sql("Update DbStaticItems SET Charisma = '32', Fortitude = '16' WHERE id = '491'");

            // Discipline
            Execute.Sql("Update DbStaticItems SET Discipline = '20', Fortitude = '10' WHERE id = '447'");
            Execute.Sql("Update DbStaticItems SET Discipline = '22', Fortitude = '11' WHERE id = '456'");
            Execute.Sql("Update DbStaticItems SET Discipline = '24', Fortitude = '12' WHERE id = '474'");
            Execute.Sql("Update DbStaticItems SET Discipline = '26', Fortitude = '13' WHERE id = '465'");
            Execute.Sql("Update DbStaticItems SET Discipline = '28', Fortitude = '14' WHERE id = '505'");
            Execute.Sql("Update DbStaticItems SET Discipline = '30', Fortitude = '15' WHERE id = '483'");
            Execute.Sql("Update DbStaticItems SET Discipline = '32', Fortitude = '16' WHERE id = '492'");

            // Perception
            Execute.Sql("Update DbStaticItems SET Perception = '20', Fortitude = '10' WHERE id = '448'");
            Execute.Sql("Update DbStaticItems SET Perception = '22', Fortitude = '11' WHERE id = '457'");
            Execute.Sql("Update DbStaticItems SET Perception = '24', Fortitude = '12' WHERE id = '475'");
            Execute.Sql("Update DbStaticItems SET Perception = '26', Fortitude = '13' WHERE id = '466'");
            Execute.Sql("Update DbStaticItems SET Perception = '28', Fortitude = '14' WHERE id = '506'");
            Execute.Sql("Update DbStaticItems SET Perception = '30', Fortitude = '15' WHERE id = '484'");
            Execute.Sql("Update DbStaticItems SET Perception = '32', Fortitude = '16' WHERE id = '493'");

            // Fortitude -> Renamed to Rune of Glass
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Glass', Charisma = '15', Luck = '15', Perception = '0', Fortitude = '-6' WHERE id = '448'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Glass', Charisma = '16', Luck = '15', Perception = '0', Fortitude = '-7' WHERE id = '457'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Glass', Charisma = '18', Luck = '18', Perception = '0', Fortitude = '-7' WHERE id = '475'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Glass', Charisma = '19', Luck = '19', Perception = '0', Fortitude = '-8' WHERE id = '466'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Glass', Charisma = '21', Luck = '21', Perception = '0', Fortitude = '-8' WHERE id = '506'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Glass', Charisma = '22', Luck = '22', Perception = '0', Fortitude = '-9' WHERE id = '484'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Glass', Charisma = '24', Luck = '24', Perception = '0', Fortitude = '-10' WHERE id = '493'");

            // Agility
            Execute.Sql("Update DbStaticItems SET Agility = '20', Perception = '0', Fortitude = '10' WHERE id = '450'");
            Execute.Sql("Update DbStaticItems SET Agility = '22', Perception = '0', Fortitude = '11' WHERE id = '459'");
            Execute.Sql("Update DbStaticItems SET Agility = '24', Perception = '0', Fortitude = '12' WHERE id = '477'");
            Execute.Sql("Update DbStaticItems SET Agility = '26', Perception = '0', Fortitude = '13' WHERE id = '468'");
            Execute.Sql("Update DbStaticItems SET Agility = '28', Perception = '0', Fortitude = '14' WHERE id = '508'");
            Execute.Sql("Update DbStaticItems SET Agility = '30', Perception = '0', Fortitude = '15' WHERE id = '486'");
            Execute.Sql("Update DbStaticItems SET Agility = '32', Perception = '0', Fortitude = '16' WHERE id = '495'");

            // Restoration aka Allure
            Execute.Sql("Update DbStaticItems SET Allure = '20', Fortitude = '10' WHERE id = '451'");
            Execute.Sql("Update DbStaticItems SET Allure = '22', Fortitude = '11' WHERE id = '460'");
            Execute.Sql("Update DbStaticItems SET Allure = '24', Fortitude = '12' WHERE id = '478'");
            Execute.Sql("Update DbStaticItems SET Allure = '26', Fortitude = '13' WHERE id = '469'");
            Execute.Sql("Update DbStaticItems SET Allure = '28', Fortitude = '14' WHERE id = '509'");
            Execute.Sql("Update DbStaticItems SET Allure = '30', Fortitude = '15' WHERE id = '487'");
            Execute.Sql("Update DbStaticItems SET Allure = '32', Fortitude = '16' WHERE id = '496'");

            // Magicka
            Execute.Sql("Update DbStaticItems SET Magicka = '20', Fortitude = '10' WHERE id = '452'");
            Execute.Sql("Update DbStaticItems SET Magicka = '22', Fortitude = '11' WHERE id = '461'");
            Execute.Sql("Update DbStaticItems SET Magicka = '24', Fortitude = '12' WHERE id = '479'");
            Execute.Sql("Update DbStaticItems SET Magicka = '26', Fortitude = '13' WHERE id = '470'");
            Execute.Sql("Update DbStaticItems SET Magicka = '28', Fortitude = '14' WHERE id = '510'");
            Execute.Sql("Update DbStaticItems SET Magicka = '30', Fortitude = '15' WHERE id = '488'");
            Execute.Sql("Update DbStaticItems SET Magicka = '32', Fortitude = '16' WHERE id = '497'");

            // Regeneration aka Succour
            Execute.Sql("Update DbStaticItems SET Succour = '20', Fortitude = '10' WHERE id = '453'");
            Execute.Sql("Update DbStaticItems SET Succour = '22', Fortitude = '11' WHERE id = '462'");
            Execute.Sql("Update DbStaticItems SET Succour = '24', Fortitude = '12' WHERE id = '480'");
            Execute.Sql("Update DbStaticItems SET Succour = '26', Fortitude = '13' WHERE id = '471'");
            Execute.Sql("Update DbStaticItems SET Succour = '28', Fortitude = '14' WHERE id = '511'");
            Execute.Sql("Update DbStaticItems SET Succour = '30', Fortitude = '15' WHERE id = '489'");
            Execute.Sql("Update DbStaticItems SET Succour = '32', Fortitude = '16' WHERE id = '498'");

            // Luck
            Execute.Sql("Update DbStaticItems SET Luck = '20', Fortitude = '10' WHERE id = '454'");
            Execute.Sql("Update DbStaticItems SET Luck = '22', Fortitude = '11' WHERE id = '463'");
            Execute.Sql("Update DbStaticItems SET Luck = '24', Fortitude = '12' WHERE id = '481'");
            Execute.Sql("Update DbStaticItems SET Luck = '26', Fortitude = '13' WHERE id = '472'");
            Execute.Sql("Update DbStaticItems SET Luck = '28', Fortitude = '14' WHERE id = '512'");
            Execute.Sql("Update DbStaticItems SET Luck = '30', Fortitude = '15' WHERE id = '490'");
            Execute.Sql("Update DbStaticItems SET Luck = '32', Fortitude = '16' WHERE id = '499'");

            // Fox
            Execute.Sql("Update DbStaticItems SET Charisma = '15', Agility = '15', Discipline = '-6', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '525'");
            Execute.Sql("Update DbStaticItems SET Charisma = '16', Agility = '16', Discipline = '-7', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '526'");
            Execute.Sql("Update DbStaticItems SET Charisma = '18', Agility = '18', Discipline = '-7', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '527'");
            Execute.Sql("Update DbStaticItems SET Charisma = '19', Agility = '19', Discipline = '-8', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '528'");
            Execute.Sql("Update DbStaticItems SET Charisma = '21', Agility = '21', Discipline = '-8', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '529'");
            Execute.Sql("Update DbStaticItems SET Charisma = '22', Agility = '22', Discipline = '-9', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '530'");
            Execute.Sql("Update DbStaticItems SET Charisma = '24', Agility = '24', Discipline = '-10', Fortitude = '0', Allure = '0', Succour = '0' WHERE id = '531'");

            // Guardian
            Execute.Sql("Update DbStaticItems SET Discipline = '10', Fortitude = '20', Magicka = '-6', Charisma = '0', Luck = '0' WHERE id = '532'");
            Execute.Sql("Update DbStaticItems SET Discipline = '11', Fortitude = '21', Magicka = '-7', Charisma = '0', Luck = '0' WHERE id = '533'");
            Execute.Sql("Update DbStaticItems SET Discipline = '13', Fortitude = '23', Magicka = '-7', Charisma = '0', Luck = '0' WHERE id = '534'");
            Execute.Sql("Update DbStaticItems SET Discipline = '14', Fortitude = '24', Magicka = '-8', Charisma = '0', Luck = '0' WHERE id = '535'");
            Execute.Sql("Update DbStaticItems SET Discipline = '16', Fortitude = '26', Magicka = '-8', Charisma = '0', Luck = '0' WHERE id = '536'");
            Execute.Sql("Update DbStaticItems SET Discipline = '17', Fortitude = '27', Magicka = '-9', Charisma = '0', Luck = '0' WHERE id = '537'");
            Execute.Sql("Update DbStaticItems SET Discipline = '19', Fortitude = '29', Magicka = '-10', Charisma = '0', Luck = '0' WHERE id = '538'");

            // Enchantress
            Execute.Sql("Update DbStaticItems SET Magicka = '15', Discipline = '15', Fortitude = '-6', Charisma = '0', Allure = '0', Succour = '0'  WHERE id = '539'");
            Execute.Sql("Update DbStaticItems SET Magicka = '16', Discipline = '16', Fortitude = '-7', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '540'");
            Execute.Sql("Update DbStaticItems SET Magicka = '18', Discipline = '18', Fortitude = '-7', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '541'");
            Execute.Sql("Update DbStaticItems SET Magicka = '19', Discipline = '19', Fortitude = '-8', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '542'");
            Execute.Sql("Update DbStaticItems SET Magicka = '21', Discipline = '21', Fortitude = '-8', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '543'");
            Execute.Sql("Update DbStaticItems SET Magicka = '22', Discipline = '22', Fortitude = '-9', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '544'");
            Execute.Sql("Update DbStaticItems SET Magicka = '24', Discipline = '24', Fortitude = '-10', Charisma = '0', Allure = '0', Succour = '0' WHERE id = '545'");

            // Healer -> Renamed to Acrobat
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of the Acrobat', Agility = '15', Luck = '15', Discipline = '-6', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '546'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of the Acrobat', Agility = '16', Luck = '16', Discipline = '-7', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '547'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of the Acrobat', Agility = '18', Luck = '18', Discipline = '-7', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '548'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of the Acrobat', Agility = '19', Luck = '19', Discipline = '-8', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '549'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of the Acrobat', Agility = '21', Luck = '21', Discipline = '-8', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '550'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of the Acrobat', Agility = '22', Luck = '22', Discipline = '-9', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '551'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of the Acrobat', Agility = '24', Luck = '24', Discipline = '-10', Fortitude = '0', Succour = '0', Allure = '0'  WHERE id = '552'");

            // Special Runes
            // Miniboss - Housemother
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '20', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '20', Succour = '0', Luck = '0' WHERE id = '564'");
            // Miniboss - Musician
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '20', Fortitude = '15', Agility = '20', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '565'");
            // Miniboss - Maid
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '20', Succour = '0', Luck = '20'  WHERE id = '566'");
            // Miniboss - Groundskeeper
            Execute.Sql("Update DbStaticItems SET Discipline = '20', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '20', Luck = '0'  WHERE id = '573'");
            // Miniboss - Threadmistress
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '20', Magicka = '20', Succour = '0', Luck = '0' WHERE id = '567'");

            // Boss - Rats
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '20', Agility = '20', Allure = '0', Magicka = '0', Succour = '0', Luck = '20' WHERE id = '513'");
            // Boss - Road Lady
            Execute.Sql("Update DbStaticItems SET Discipline = '20', Perception = '0', Charisma = '0', Fortitude = '20', Agility = '20', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '557'");
            // Boss - Bimboss 
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '20', Fortitude = '20', Agility = '0', Allure = '0', Magicka = '20', Succour = '0', Luck = '0'   WHERE id = '518'");
            // Boss - Donna
            Execute.Sql("Update DbStaticItems SET Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '20', Agility = '0', Allure = '0', Magicka = '20', Succour = '0', Luck = '20'   WHERE id = '515'");
            // Boss - Narcissa
            Execute.Sql("Update DbStaticItems SET Discipline = '20', Perception = '0', Charisma = '20', Fortitude = '20', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0'   WHERE id = '517'");
        }
    }
}
