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
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '20', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '446'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '22', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '455'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '24', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '473'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '26', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '464'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '28', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '504'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '30', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '482'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Charisma', Discipline = '0', Perception = '0', Charisma = '32', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '491'");

            // Discipline
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Discipline', Discipline = '20', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '447'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Discipline', Discipline = '22', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '456'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Discipline', Discipline = '24', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '474'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Discipline', Discipline = '26', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '465'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Discipline', Discipline = '28', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '505'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Discipline', Discipline = '30', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '483'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Discipline', Discipline = '32', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '492'");

            // Perception
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Perception', Discipline = '0', Perception = '20', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '448'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Perception', Discipline = '0', Perception = '22', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '457'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Perception', Discipline = '0', Perception = '24', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '475'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Perception', Discipline = '0', Perception = '26', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '466'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Perception', Discipline = '0', Perception = '28', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '506'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Perception', Discipline = '0', Perception = '30', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '484'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Perception', Discipline = '0', Perception = '32', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '493'");

            // Fortitude -> Renamed to Rune of Striking
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Striking', Discipline = '0', Perception = '0', Charisma = '10', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '10' WHERE id = '449'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Striking', Discipline = '0', Perception = '0', Charisma = '11', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '11' WHERE id = '458'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Striking', Discipline = '0', Perception = '0', Charisma = '12', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '12' WHERE id = '476'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Striking', Discipline = '0', Perception = '0', Charisma = '13', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '13' WHERE id = '467'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Striking', Discipline = '0', Perception = '0', Charisma = '14', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '14' WHERE id = '507'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Striking', Discipline = '0', Perception = '0', Charisma = '15', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '15' WHERE id = '485'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Striking', Discipline = '0', Perception = '0', Charisma = '16', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '16' WHERE id = '494'");

            // Agility
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '20', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '450'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '22', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '459'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '24', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '477'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '26', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '468'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '28', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '508'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '30', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '486'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Agility', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '32', Allure = '0', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '495'");

            // Restoration aka Allure
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '20', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '451'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '22', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '460'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '24', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '478'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '26', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '469'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '28', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '509'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '30', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '487'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Restoration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '32', Magicka = '0', Succour = '0', Luck = '0' WHERE id = '496'");

            // Magicka
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '20', Succour = '0', Luck = '0' WHERE id = '452'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '22', Succour = '0', Luck = '0' WHERE id = '461'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '24', Succour = '0', Luck = '0' WHERE id = '479'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '26', Succour = '0', Luck = '0' WHERE id = '470'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '28', Succour = '0', Luck = '0' WHERE id = '510'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '30', Succour = '0', Luck = '0' WHERE id = '488'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Magicka', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '32', Succour = '0', Luck = '0' WHERE id = '497'");

            // Regeneration aka Succour

            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '20', Luck = '0' WHERE id = '453'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '22', Luck = '0' WHERE id = '462'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '24', Luck = '0' WHERE id = '480'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '26', Luck = '0' WHERE id = '471'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '28', Luck = '0' WHERE id = '511'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '30', Luck = '0' WHERE id = '489'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Regeneration', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '32', Luck = '0' WHERE id = '498'");

            // Luck
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Minor Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '10', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '20' WHERE id = '454'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Standard Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '11', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '22' WHERE id = '463'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Great Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '12', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '24' WHERE id = '481'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Major Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '13', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '26' WHERE id = '472'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Superior Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '14', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '28' WHERE id = '512'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Supreme Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '15', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '30' WHERE id = '490'");
            Execute.Sql("Update DbStaticItems SET FriendlyName = 'Legendary Rune of Luck', Discipline = '0', Perception = '0', Charisma = '0', Fortitude = '16', Agility = '0', Allure = '0', Magicka = '0', Succour = '0', Luck = '32' WHERE id = '499'");


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
