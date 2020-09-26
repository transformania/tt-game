using FluentMigrator;

namespace TT.Migrations
{
    [Migration(202008291745)]
    public class UpdateItems : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Base Consumables
            // Willpower Petals
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 240, InstantManaRestore = 0 WHERE id = '10'");
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 480, InstantManaRestore = 0 WHERE id = '1'");
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 720, InstantManaRestore = 0 WHERE id = '2'");
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 0 WHERE id = '11'");
            // Spellweaver Petals
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 21 WHERE id = '3'");
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 126 WHERE id = '4'");
            Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 252 WHERE id = '5'");
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 0 WHERE id = '12'");
            // Throwables
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 240, InstantManaRestore = 0 WHERE id = '10'");
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 480, InstantManaRestore = 0 WHERE id = '1'");
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 720, InstantManaRestore = 0 WHERE id = '2'");
            //Execute.Sql("Update DbStaticItems SET InstantHealthRestore = 0, InstantManaRestore = 0 WHERE id = '11'");


            // Reusable Consumables
            // Willpower & Mana
                // Sex Doll
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 725, ReuseableManaRestore = 0, UseCooldown = 12 WHERE id = '30'");
                // Bondage Magazine
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 725, ReuseableManaRestore = 0, UseCooldown = 12 WHERE id = '48'");
                // Onahole
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 200, ReuseableManaRestore = -21, UseCooldown = 3 WHERE id = '82'");
                // Double-Ended Dildo
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 725, ReuseableManaRestore = 0, UseCooldown = 3 WHERE id = '286'");
                // Eternal Condom
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 363, ReuseableManaRestore = 126, UseCooldown = 12 WHERE id = '286'");
                // Delicious Cupcake
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 360, ReuseableManaRestore = 0, UseCooldown = 6 WHERE id = '361'");
                // Humanoid Onahole
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 725, ReuseableManaRestore = -63, UseCooldown = 9 WHERE id = '440'");
                // Self-refilling silver whiskey flask
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 0, ReuseableManaRestore = 252, UseCooldown = 12 WHERE id = '21'");
                // Candy Cane Girl
            Execute.Sql("Update DbStaticItems SET ReuseableHealthRestore = 0, ReuseableManaRestore = 252, UseCooldown = 12 WHERE id = '379'");

            // Buff Items
                // Strawberry Cheesecake
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '224'");
                // Popsickle
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '260'");
                // Sponge
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '284'");
                // Regenerating Chocolate Bunny Girl
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '291'");
                // Bubblegum
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '317'");
                // Fae in a bottle
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '322'");
                // Tampon
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '360'");
                // Soup
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '404'");
                // Deck of cards
            Execute.Sql("Update DbStaticItems SET UseCooldown = 12 WHERE id = '561'");

            // Buff Effects 
            // Concealment Cookie
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 60, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '48'");
                // Fire Fritter
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 60, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '49'");
                // Barricade Brownies
            Execute.Sql("Update DbStaticEffects SET Discipline = 60, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '50'");
                // Running_Hot
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 60, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '61'");
                // Trueshot truffles
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 60, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '62'");
                // Nirvana Nougat
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 60, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '64'");
                // Sugar_Rush
            Execute.Sql("Update DbStaticEffects SET Discipline = 8, Perception = 8, Charisma = 8, Fortitude = 8, Agility = 8, Allure = 8, Magicka = 8, Succour = 8, Luck = 8, Duration  = 6, Cooldown  = 12 WHERE id = '103'");
                // Perception Puff
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 60, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '161'");
                // Lucky Lemoncake
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 60, Duration  = 6, Cooldown  = 12 WHERE id = '162'");
                // Brain Freeze
            Execute.Sql("Update DbStaticEffects SET Discipline = 25, Perception = 0, Charisma = 25, Fortitude = 0, Agility = 0, Allure = 25, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '163'");
                // Squeaky_Clean
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = -20, Allure = 80, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '174'");
                // Chocolate_Rush
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 60, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '167'");
                // Blunderbus_Bubblegum
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 60, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '178'");
                // Narcissa
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 120, Magicka = 0, Succour = 0, Luck = 0, Duration  = 3, Cooldown  = 12 WHERE id = '179'");
                // Tampon
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = 60, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '180'");
                // Soup
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 80, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 0, Succour = -20, Luck = 0, Duration  = 6, Cooldown  = 12 WHERE id = '186'");
                // Cards
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 0, Allure = 0, Magicka = 30, Succour = 0, Luck = 30, Duration  = 6, Cooldown  = 12 WHERE id = '192'");

                // Readjusts Espresso Jitters 
            Execute.Sql("Update DbStaticEffects SET Discipline = 0, Perception = 0, Charisma = 0, Fortitude = 0, Agility = 20, Allure = 0, Magicka = 0, Succour = 0, Luck = 0, MeditationExtraMana = 0, EvasionPercent = 0, EvasionNegationPercent = 0, MoveActionPointDiscount = 0, SpellMisfireChanceReduction = 0, Duration  = 6, Cooldown  = 12 WHERE id = '39'");
        }
    }
}
