using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Statics
{
    public static class ItemStatics
    {

        public static DbStaticItem GetStaticItem(string dbName)
        {

            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.FirstOrDefault(i => i.dbName == dbName);

        }

        public static IEnumerable<DbStaticItem> GetAllFindableItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.Findable == true);
        }

        public static IEnumerable<DbStaticItem> GetAllNonPetItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.ItemType != PvPStatics.ItemType_Pet);
        }

        public static IEnumerable<DbStaticItem> GetAllPetItems()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.Where(i => i.ItemType == PvPStatics.ItemType_Pet);
        }

//            get
//            {
//                return new[]
//                {

//#region consumeables

         

//             new StaticItem {
//                dbName = "item_consumeable_willflower_fresh",
//                FriendlyName = "Willflower Petal (Fresh)",
//                 PortraitUrl = "willflower_petal_meddle.png",
//                Description = "A petal that comes from a pretty red flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that eating the petals can draw out some unwanted magic from your body of magic, recovering some lost willpower.  This petal looks decently fresh and should be decently effective. ",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 10,
//                MoneyValue = 30,
//                InstantHealthRestore = 20,

//            }, new StaticItem {
//                dbName = "item_consumeable_willflower_root_sparse",
//                FriendlyName = "Willflower Roots (Sparse)",
//                 PortraitUrl = "willflower_root_meddle.png",
//                Description = "A pretty red flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that eating the petals can draw out some unwanted magic from your body of magic, recovering some lost willpower.  All of the petals have fallen off of this plant, but what's more useful is that you found the roots intact.  While tasting particularly bitter, its nutrient-rich roots are a good deal more potent than the plant's mere petals, though these roots seem a little sparse.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 3,
//                MoneyValue = 60,
//                InstantHealthRestore = 40,

//            }, new StaticItem {
//                dbName = "item_consumeable_spellweaver_dry",
//                FriendlyName = "Spellweaver Petal (Dry)",
//                PortraitUrl = "spellweaver_petal_meddle.png",
//                Description = "A petal that comes from a pretty blue flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that these petals are teeming with magic and that eating some will restore a bit of mana.  This petal looks dry and crumbly and probably won't be very effective. ",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 20,
//                MoneyValue = 15,
//                InstantManaRestore = 10,
                
//            },  new StaticItem {
//                dbName = "item_consumeable_spellweaver_fresh",
//                FriendlyName = "Spellweaver Petal (Fresh)",
//                 PortraitUrl = "spellweaver_petal_meddle.png",
//                Description = "A petal that comes from a pretty blue flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that these petals are teeming with magic and that eating some will restore a bit of mana.  This petal looks decently fresh and should be decently effective. ",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 10,
//                MoneyValue = 30,
//                InstantManaRestore = 20,

//            }, new StaticItem {
//                dbName = "item_consumeable_spellweaver_root_sparse",
//                FriendlyName = "Spellweaver Roots (Sparse)",
//                 PortraitUrl = "spellweaver_root_meddle.png",
//                Description = "A petal that comes from a pretty blue flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that these petals are teeming with magic and that eating some will restore a bit of mana.  All of the petals have fallen off of this plant, but what's more useful is that you found the roots intact.  While tasting particularly bitter, its nutrient-rich roots are a good deal more potent than the plant's mere petals, though these roots seem a little sparse.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 3,
//                MoneyValue = 60,
//                InstantManaRestore = 40,

//           }, new StaticItem {
//                dbName = "item_consumeable_teleportation_scroll",
//                FriendlyName = "Scroll of Teleportation",
//                 PortraitUrl = "scroll_of_teleportation.jpg",
//                Description = "This rare and powerful scroll will let its owner place his or her hand against a sketch of a palm in the center of the scroll inked on by highly distilled spellweaver flower juices.  All the user has to do is shut their eyes and think of where they want to go, and when they next open their eyes, they are there no matter how far the distance.  Unfortunately the scroll will crumble to dust after being used.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = .5D,
//                MoneyValue = 300,

//           }, new StaticItem {
//                dbName = "item_consumeable_willpower_bomb_weak",
//                FriendlyName = "Submissiveness Splash Orb (Weak)",
//                 PortraitUrl = "willbomb_weak.jpg",
//                Description = "An unstable mixture of volatile herbs and magic, held together in a magical enchantment that keeps everything condensed into a glowing orb small enough to hold in the palm of one's hand.  A careful mage can throw the orb at a group of enemies upon which the orb explodes, showering all nearby humans and humanoids with powerful energy that captivates their senses, decreasing some of their willpower.  This orb has been rushly made and will not deal a considerable amount of damage.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 3,
//                MoneyValue = 100,

//            }, new StaticItem {
//                dbName = "item_consumeable_willpower_bomb_strong",
//                FriendlyName = "Submissiveness Splash Orb (Strong)",
//                 PortraitUrl = "willbomb_weak.jpg",
//                Description = "An unstable mixture of volatile herbs and magic, held together in a magical enchantment that keeps everything condensed into a glowing orb small enough to hold in the palm of one's hand.  A careful mage can throw the orb at a group of enemies upon which the orb explodes, showering all nearby humans and humanoids with powerful energy that captivates their senses, decreasing some of their willpower.  This orb has been packed extra full of magic, making it considerably more damage than its weaker counterparts.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 1.0D,
//                MoneyValue = 200,

//             }, new StaticItem {
//                dbName = "item_consumeable_willpower_bomb_volatile",
//                FriendlyName = "Submissiveness Splash Orb (Volatile)",
//                 PortraitUrl = "willbomb_weak.jpg",
//                Description = "An unstable mixture of volatile herbs and magic, held together in a magical enchantment that keeps everything condensed into a glowing orb small enough to hold in the palm of one's hand.  A careful mage can throw the orb at a group of enemies upon which the orb explodes, showering all nearby humans and humanoids with powerful energy that captivates their senses, decreasing some of their willpower.  This orb has been expertly squeezed full of magic by a true dominatrix, making it far more damaging than its weaker counterparts.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = .33D,
//                MoneyValue = 400,

//           }, new StaticItem {
//                dbName = "item_consumeable_willflower_dry",
//                FriendlyName = "Willflower Petal (Dry)",
//                PortraitUrl = "willflower_petal_meddle.png",
//                Description = "A petal that comes from a pretty red flower, unextraordinary to the untrained eye.  However, anyone who has studied alchemy will know that eating the petals can draw out some unwanted magic from your body of magic, recovering some lost willpower.  This petal looks dry and crumbly and probably won't be very effective. ",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 20,
//                MoneyValue = 15,
//                InstantHealthRestore = 10,

//#endregion

//#region consumeables that give effects

//           }, new StaticItem {
//                dbName = "item_consumeable_willflower_jawbreaker_cheap",
//                FriendlyName = "Willflower Jawbreaker (Small)",
//                PortraitUrl = "willflower_jawbreaker_Meddle.png",
//                Description = "A small red jawbreaker made out of the juice of willflower roots, artificial flavoring added to the mixture, and baked into a jawbreaker.  Sucking on this tastes a lot better than eating the bitter petals and roots of the willflower plant straight out of the ground, and its hard shells ensures that it lasts a fifteen minute in your mouth, restoring some willpower for a while even after it finishes dissolving in your mouth.  The jawbreaker is pretty small but will restore 5 willpower for 6 turns.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 7,
//                MoneyValue = 40,
//               GivesEffect = "blessing_health_jawbreaker_small",

//          }, new StaticItem {
//                dbName = "item_consumeable_spellweaver_jawbreaker_cheap",
//                FriendlyName = "Spellweaver Jawbreaker (Small)",
//                PortraitUrl = "spellweaver_jawbreaker_Meddle.png",
//                Description = "A small blue jawbreaker made out of the juice of spellweaver roots, artificial flavoring added to the mixture, and baked into a jawbreaker.  Sucking on this tastes a lot better than eating the bitter petals and roots of the spellweaver plant straight out of the ground, and its hard shells ensures that it lasts a fifteen minute in your mouth, restoring some mana for a while even after it finishes dissolving in your mouth.  The jawbreaker is pretty small but will restore 5 mana for 6 turns.",
//                ItemType = "consumable",
//                Findable = true,
//                FindWeight = 7,
//                MoneyValue = 40,
//               GivesEffect = "blessing_mana_jawbreaker_small",
               

//#endregion


//            }, new StaticItem {
//                dbName = "marble_statue",
//                FriendlyName = "Voluptuous Marble Statuette",
//                Description = "A solid marble statuette, very lifelike in its apperance, as if it had once been a breathing human being.  Which it actually was.  Carrying this statue with you seems to focus your spells a little more carefully.",
//                PortraitUrl = "statuette_tfnymic.jpg",
//                ItemType = "accessory",
//                MoneyValue = 50 ,
//                ExtraSkillCriticalPercent = 5,

//            },  new StaticItem {
//                dbName = "fuzzy_slippers",
//                FriendlyName = "Pink Fuzzy Rabbit Slippers",
//                PortraitUrl = "",
//                Description = "A pair of fuzzy pink slippers made to look cartoonishly like a rabbit.",
//                ItemType = "shoes",
//                MoneyValue = 50,
//                SneakPercent = 20,

//            //},  new StaticItem {
//            //    dbName = "panty_plain_1",
//            //    FriendlyName = "Plain Pink Panties",
//            //    PortraitUrl = "panty_plain_1_tfnymic.jpg",
//            //    Description = "A pair of pink panties made out of a smooth, silky fabric.  The design is plain, designed for comfort instead.  Wearing this relaxes the body and frees the mind, allowing the wearer to regenerate mana over time and receive a bonus when meditating.",
//            //    ItemType = "underpants",
//            //    MoneyValue = 50,
//            //    ManaRecoveryPerUpdate = 1,
//            //    MeditationExtraMana= 4,

//            }, new StaticItem {
//         dbName = "item_Plain_Pink_Panties_Judoo",
//         FriendlyName = "Plain Pink Panties",
//         PortraitUrl = "panty_plain_1_tfnymic.jpg",
//         Description = "A pair of pink panties made out of a smooth, silky fabric.  The design is plain, designed for comfort instead.  Wearing this relaxes the body and frees the mind, allowing the wearer to regenerate mana over time and receive a bonus when meditating.",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         HealthBonusPercent = 5,


//            },  new StaticItem {
//                dbName = "item_bunny_slippers",
//                FriendlyName = "Fuzzy Pink Bunny Slippers",
//                PortraitUrl = "pink_bunny_slippers_tfnymic.jpg",
//                Description = "A pair of fuzzy pink bunny slippers, two floppy ears and black bead eyes on each slipper expressing the ultimate form of cuteness.  These slippers are incredibly comfy and make little noise when one takes a step in then.",
//                ItemType = "shoes",
//                MoneyValue = 50,
//                SneakPercent = 8,
            

                
//            }, new StaticItem {
//                dbName = "item_skateboard",
//                FriendlyName = "Sleek, Fiery Skateboard",
//                PortraitUrl = "item_skateboard_nymic-tf.jpg",
//                Description = "A wooden skateboard absolutely plastered in flame decals, sleek black wheels perfectly oiled for maximum speed, paint fresh and unscratched--until its owner gets some good rail grinds in, anyway.  Riding on this will allow its rider to move around more quickly, though the wheels will make some extra noise. ",
//                ItemType = "accessory",
//                Findable = false,
//                MoneyValue = 50,
//                MoveActionPointDiscount = .25M,
//                SneakPercent = -25,
                
//            }, new StaticItem {
//                dbName = "item_tight_blue_jeans",
//                FriendlyName = "Loyal Hip-Hugging Blue Jeans",
//                PortraitUrl = "item_tight_blue_jeans_tfnymic.jpg",
//                Description = "A pair of skin-tight, hip-hugging blue jeans, embroidered with flowers and and stylish patterns.  The fabric tingles at the touch, lusting to be worn.  In return the human-fabric of the jeans help to deflect incoming spells, protecting its wearer like the faithful pair of blue jeans that it is.",
//                ItemType = "pants",
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = 5,
                
//            }, 

//            new StaticItem {
//                dbName = "item_dress_jbovinne",
//                FriendlyName = "Fashionable Pink Dress",
//                PortraitUrl = "dress_jbovinne.png",
//                Description = "Classier than a lot of the ‘clothes’ around town, this long sleeved pink dress will give you confidence while still showing off its wearer's legs.",
//                ItemType = "shirt",
//                Findable = false,
//                MoneyValue = 50,
//                CleanseExtraHealth = 5.25M,
                
//            }, 

//             new StaticItem {
//                dbName = "item_black_latex_pants_jbovinne",
//                FriendlyName = "Black Latex Pants",
//                PortraitUrl = "black_latex_pants_jbovinne.png",
//                Description = "A pair of skin-tight, black latex trousers. They accentuate your every curve and highlight every movement. They are extremely tough, but seem to squeak more than they should when you walk, probably a carryover from when they were a person.",
//                ItemType = "pants",
//                Findable = false,
//                MoneyValue = 50,
//               SneakPercent = -3,
//               HealthBonusPercent = 5.6M,
                
//            }, 

//            new StaticItem {
//                dbName = "item_mana_flask",
//                FriendlyName = "Self-Refilling Silver Whiskey Flask",
//                PortraitUrl = "item_mana_flask_tfnymic.jpg",
//                Description = "A small flattened, silver flask.  Previously human, this flask taps from the transformee's consciousness and slowly fills itself with a refreshing sweet necter seeped in mana.  Sometimes when the owner screw off the top and touches his or her tongue to its glistening wet opening, the flask will softly moan and quiver in erotic anticipation...",
//                ItemType = PvPStatics.ItemType_Consumable_Reuseable,
//                Findable = false,
//                MoneyValue = 50,
//               ReuseableManaRestore = 50,
//               UseCooldown = 40,
                
//            }, 

//              new StaticItem {
//                dbName = "item_leopard_bra",
//                FriendlyName = "Sexy Leopard-Spot Bra",
//                PortraitUrl = "item_leopard_bra_nymic-tf.jpg",
//                Description = "A bra textured with a spotted leopard pattern that strongly hints at a raw, animalistic sexuality.  The cups both inside and out are coated with a fine fuzz like cat's fur, the linings fluffy with white fabric.  Once human, it now desires nothing more than to enhance its wearer's sexiness.  Wearing this bra will enhance one's agility with catlike reflexes, enhancing their ability to evade spells cast against them.",
//                ItemType = PvPStatics.ItemType_Undershirt,
//                Findable = false,
//                MoneyValue = 50,
//                EvasionPercent = 6,
                
//            }, 

//              new StaticItem {
//                dbName = "item_99_red_balloons",
//                FriendlyName = "Bundle of Bright Red Balloons",
//                PortraitUrl = "item_99_red_balloons_nymic-tf.jpg",
//                Description = "A couple dozen or so bright red, helium-filled balloons.  Thin white ribbons are tied from the bottom of the balloons to a small plastic square that keeps them from floating off into the air.  The conciousness contained within these balloons would like nothing more than to please their master so that maybe one day he or she will free them into the infinite sky.  As such, they will lend their owner some of their old magic, making the owner's spells add a little extra transformation energy per cast.",
//                ItemType = PvPStatics.ItemType_Accessory,
//                Findable = false,
//                MoneyValue = 50,
//                SpellExtraTFEnergyPercent = 4.75M,
                
//            }, 

//              new StaticItem {
//                dbName = "item_silk_tube_top",
//                FriendlyName = "Sky Blue Silk Tube Top",
//                PortraitUrl = "silk_tube_top_by_nymic-tf.jpg",
//                Description = "A sky-blue, short tube top made out of a fine, soft silk.  It wraps tightly around its wearer's breasts and leaves the rest of the torso exposed to the breeze, thin fabric allowing the wearer skin's to breathe.  Wearing this comfortable garment allows its owner to regenerate both willpower and mana over time.",
//                ItemType = "shirt",
//                Findable = false,
//                MoneyValue = 50,
//                ManaRecoveryPerUpdate = .85M,
//                HealthRecoveryPerUpdate = .65M,
                
//            }, 

//                  new StaticItem {
//                dbName = "item_black_stillettos",
//                FriendlyName = "Black 3-Inch Stillettos",
//                PortraitUrl = "black_stilettos_by_nymic-tf.jpg",
//                Description = "A pair of stilettos with 3-inch heels.  Its curves are sleek and feminine, expertly crafted with its leather exterior smartly polished.  Tiny gemstones line the the rims, glittering in the light and beaming small facets of light around its surroundings.  There's a deep sense of pride around these heels, a sense of confidence that it is above all other forms of footwear.  While wearing these stilettos are a little noisy and slow the wearer's walking speed a bit, they enhance the ability wearer's self-confidence, increasing their willpower and dealing a bit of extra transformation damage per spell cast.",
//                ItemType = PvPStatics.ItemType_Shoes,
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = 4.4M,
//                SneakPercent = -5,
//                MoveActionPointDiscount = -.05M,
//                SpellExtraTFEnergyPercent = 4,
                
//            }, new StaticItem {
//                dbName = "item_vibrating_latex_thong",
//                FriendlyName = "Vibrating Purple Latex Thong",
//                PortraitUrl = "purple_vibrating_panties_by_nymic-tf.jpg",
//                Description = "An oiled, purple latex thong with a vibrator built in right where a woman's clitoris should go.  This is no ordinary vibrating thong powered by a mere battery or remote control--no, this garment was once a human being and can vibrate at will, reading the pulse of its wearer and changing its vibration strength and rhythm for maximum, sustained pleasure, no matter of the gender of the genitalia it is wrapped around.  Wearing this will ease its master's mind, allowing them to better dissipate their worldly tensions and grant extra willpower when cleansing as well as restore a bit over time, though it may also distract its master and decrease their chance of striking a critical hit.",
//                ItemType = PvPStatics.ItemType_Underpants,
//                Findable = false,
//                MoneyValue = 50,
//                HealthRecoveryPerUpdate = 1,
//                MeditationExtraMana = 3,
//                ExtraSkillCriticalPercent = -1.5M,

                
//            }, new StaticItem {
//                dbName = "item_catears_varn1234",
//                FriendlyName = "Cat Ear Headband",
//                PortraitUrl = "CatEars_HB_Eq.jpg",
//                Description = "This unfortunate rival has been transformed into a simple headband, affixed to which are a pair of cat-like ears - wearing them grants you a substantial feline nature, nya!  In addition to also sprouting a tail, your entire body becomes more graceful - able to move more quickly and evade some attacks.  However, the ears also change your hands into paws, your lack of practice with which cause your spells to fail at a much greater rate than before. What a cat-astrophe, nya!",
//                ItemType = PvPStatics.ItemType_Hat,
//                Findable = false,
//                MoneyValue = 50,
//                SneakPercent = 11,
//                MoveActionPointDiscount = .1M,
//                EvasionPercent = 10,
//                CleanseExtraHealth = 3,
//                SpellMisfireChanceReduction = -8,
//                ManaBonusPercent = -2, 
//            }, 

//              new StaticItem {
//                dbName = "item_attachable_udder_varn12345",
//                FriendlyName = "Detachable Udder",
//                PortraitUrl = "ProstheticUdders_Nymic_TF.jpg",
//                Description = "In lieu of a shirt, this item is a magical detachable udder, the remains of a former unlucky opponent.  Placed near one's navel, it hangs heavily beneath its wearer, constantly leaking milk from the four teats hanging from its large fleshy globe.  The milk is exceptionally refreshing - consuming it greatly restores one's willpower and mana - however, it will dry up if not constantly worn near human flesh.  In addition to issues of hygiene (and fashion) - its owner may be somewhat concerned that the trail of milk it leaves dripping behind then will enable determined opponents to track them incredibly easily.",
//                ItemType = PvPStatics.ItemType_Shirt,
//                Findable = false,
//                MoneyValue = 50,
//                HealthRecoveryPerUpdate = .5M,
//                ManaRecoveryPerUpdate = .5M,
//                SneakPercent = -50,
//                CleanseExtraHealth = 2.5M,
//                MeditationExtraMana = 2.5M,
                
//            }, 
//        new StaticItem {
//                dbName = "item_catnip_mouse_Elynsynos",
//                FriendlyName = "Catnip Mouse Toy",
//                PortraitUrl = "CatnipMouseToy_Meddle.png",
//                Description = "A mouse toy made from a soft brown cloth filled with catnip.  The scent invigorates you, honing one's reflexes and giving their spells a bit of extra power.  Unfortunately the same scent will make it easier for others to track its owner.",
//                ItemType = PvPStatics.ItemType_Accessory,
//                Findable = false,
//                MoneyValue = 50,
//                EvasionNegationPercent = 5,
//                ExtraSkillCriticalPercent = 5,
//                SpellExtraTFEnergyPercent = 5,
//                SneakPercent = -10,
                
//            }, 

//        new StaticItem {
//                dbName = "item_sexdoll_lexam",
//                FriendlyName = "Inflatable Sex Doll",
//                PortraitUrl = "doll_by_night_glare-d7ephf5.jpg",
//                Description = "A ready to please blow up doll. She used to be a human but now she is an eager fuck toy. Her utter devotion and eagerness to serve her owner loyally is a great asset, her little bit of magic left giving her the ability to restore some of her lover's willpower so you she keep fucking them again and again.",
//                ItemType = "consumable_reuseable",
//                Findable = false,
//                MoneyValue = 50,
//                UseCooldown = 40,
//                ReuseableHealthRestore = 50,
                
//         },  new StaticItem {
//                dbName = "item_boxer_briefs_PsychoticPie",
//                FriendlyName = "Boxer Briefs",
//                PortraitUrl = "BoxerShorts_Meddle.png",
//                Description = "A pair of boxer briefs, formerly of an unfortunate human. Useful for keeping the boys downtown under wraps. Since these boxers used to be a human, they can wiggle ever so slightly, and open their 'pouch' at will. Their snug comfiness increases their owner's willpower, and helps them recover willpower more quickly.",
//                ItemType = PvPStatics.ItemType_Underpants,
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = 4,
//                HealthRecoveryPerUpdate = 1.25M,
//            }, 

//           new StaticItem {
//                dbName = "item_phallus_equus_varn",
//                FriendlyName = "Prosthetic Horsecock",
//                PortraitUrl = "ProstheticHorseCock_Meddle.png",
//                Description = "This magical item is shaped like an enormous horsecock - 18 inches long, with two giant balls sloshing with liquid hanging below.  A pink, flared tip emerges from a thick black leathery sheath.  The sheath looks odd - hollow on the inside, with the back of it's base coated with an odd, sticky, organic looking substance.  It seems that not only may a man insert his dick inside to receive a substantial upgrade to his equipment - but that a woman may also wear the horsecock as a strap-on, without compromising access to her native genitalia.  In addition to its obvious uses, this artifact bestows considerable resistance against feminizing effects.  Well, really, resistance against all magic - but can't having any less than this splendid tool be considered a feminizing effect as well? Truly, a wonder for the ages.  Well worth a fellow mage giving up their very human life for its creation!  (Or perhaps not, as the prosthesis quite often seems to pulse and waggle under its own power...)",
//                ItemType = PvPStatics.ItemType_Underpants,
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = 3,
//                EvasionPercent = 10,

//           },
                
//           new StaticItem {
//                dbName = "item_black_latex_bra_Arrhae",
//                FriendlyName = "Black Latex Bra",
//                PortraitUrl = "LatexBra_Equipped.jpg",
//                Description = "This tight fitting black latex bra helps support its wearer's breasts, making it easier to move around, along with keeping their breasts under control while spellcasting. Unfortunately it also squeaks when it rubs together, making it easier for others to track them. It's also quite distracting, slowly draining their will to resist temptation.",
//                ItemType = PvPStatics.ItemType_Undershirt,
//                Findable = false,
//                MoneyValue = 50,
//                SneakPercent = -10,
//                MoveActionPointDiscount = .10M,
//                HealthRecoveryPerUpdate = -.5M,
//                EvasionNegationPercent = 20,

                
//            }, 

//            new StaticItem {
//                dbName = "item_straight_tie_CCWS",
//                FriendlyName = "Smart looking Tie",
//                PortraitUrl = "SmartLookingTie.png",
//                Description = "A long, silken red tie. When worn, it gives the wearer an air of authority - Or acts as an alluring distraction! Either way, it's a little harder to escape the wearer's attacks with one's attention so captivated...",
//                ItemType = PvPStatics.ItemType_Undershirt,
//                Findable = false,
//                MoneyValue = 50,
//                EvasionNegationPercent = 15,
//                SneakPercent = -5,
                
//            }, 

//           new StaticItem {
//                dbName = "item_athletic_sneakers_PsychoticPie",
//                FriendlyName = "Athletic Sneakers",
//                PortraitUrl = "AthleticShoes_Meddle.png",
//                Description = "A pair of sneakers. Great for extended use and athletic activity. If you happen to come across them and look closely, you'll notice eyes and a nose inside. This comes from the fact that these sneakers used to be human. Another side effect of this is that they alter their shape and size to fit their wearer's feet, providing optimal comfort regardless of who's wearing them. Moving around will be a bit easier with these puppies on, obviously, and their pragmatic design makes it easier to recover willpower when you're exerting yourself. They aren't the flashiest shoes however, so you might not want to wear them if you're trying to impress someone.",
//                ItemType = PvPStatics.ItemType_Shoes,
//                Findable = false,
//                MoneyValue = 50,
//                MoveActionPointDiscount = .15M,
//                CleanseExtraHealth = 2,
//                ManaBonusPercent = -5,  
//            },

//               new StaticItem {
//                dbName = "item_black_silk_top_hat_Zatur",
//                FriendlyName = "Black Silk Top Hat",
//                PortraitUrl = "BlackSilkTopHat.png",
//                Description = "A stylish silk top hat that, until recently, used to be human. It seems some magic still clings to it, as even though it reforms to perfectly fit its wearer's head, it feels like there's more room inside it to hold almost anything. Some notes, an apple, perhaps even a rabbit or two. Either way, it is quite dapper. Very noticeable as well, but that's the price of great fashion.",
//                ItemType = PvPStatics.ItemType_Hat,
//                Findable = false,
//                MoneyValue = 50,
//                ManaBonusPercent = 1,
//                SpellExtraTFEnergyPercent = 1,
//                EvasionPercent = 2,
//                EvasionNegationPercent = -5,
//                SneakPercent = -2,
//                ExtraInventorySpace = 1,

                
//            }, 

//           new StaticItem {
//                dbName = "item_fertility_idol_varn",
//                FriendlyName = "Fertility Idol",
//                PortraitUrl = "FertilityIdol1_Meddle.png",
//                Description = "A small stone statue of a well-endowed, extremely pregnant woman - only an odd reflection in its onyx eyes give any hint that it is not a Neolithic relic, but instead the prison of a fellow mage's spirit.  Grasping it, its owner can feel its power flowing through them - the longer one holds it, the more they will take on the aspects of pregnancy - their breasts engorging, filling with milk - their belly expanding, swelling, filling with a simulacrum of new life.  The additional heft will make it harder to run or sneak... but, despite the owner knowing logically that the pregnancy is artificial... their motherly instinct causes then to shrug off attacks, and strike back viciously at any who would harm the unborn child.  (Magister General's Warning: This item is equally effective on males - prolonged exposure may eventually necessitate a C-section.)",
//                ItemType = PvPStatics.ItemType_Accessory,
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = 3,
//                EvasionNegationPercent = 6,
//                ExtraSkillCriticalPercent = 3,
//                MoveActionPointDiscount = -.25M,
                
//            }, 

//         new StaticItem {
//                dbName = "item_schoolgirl_top_PsychoticPie",
//                FriendlyName = "Schoolgirl Top",
//                PortraitUrl = "ButtonUpSchoolgirlBlouse_Meddle.png",
//                Description = "A cute little button up schoolgirl top. Goes well with a tie. Although most people wouldn't be able to tell on first glance, this item used to be a human. The stalwart pure essence of this top makes it's wearer less likely to misfire, and its innocent appeal lowers the willpower of all who see it. On the flipside, it also makes its wearer less likely to evade incoming spells.",
//                ItemType = PvPStatics.ItemType_Shirt,
//                Findable = false,
//                MoneyValue = 50,
//                SpellMisfireChanceReduction = 2,
//                SpellExtraHealthDamagePercent = 6,
//                EvasionPercent = -7,
//            }, 

//         new StaticItem {
//                dbName = "item_schoolbottom_bop_PsychoticPie",
//                FriendlyName = "Schoolgirl Skirt",
//                PortraitUrl = "PleatedPlaidSkirt.png",
//                Description = "A very short, pleated plaid, red schoolgirl skirt. You can see upon closer examination that it was formerly a human. Given just how short this skirt is, its wearer could easily flash people if they aren't careful with the wind. The youthful, pure essence of this skirt makes its wearer less likely to misfire, and its innocent, schoolgirl appeal lowers the willpower of all who see it. By the same token, it makes its wearer less likely to evade incoming spells. ",
//                ItemType = PvPStatics.ItemType_Pants,
//                Findable = false,
//                MoneyValue = 50,
//                SpellMisfireChanceReduction = 2,
//                SpellExtraHealthDamagePercent = 6,
//                EvasionPercent = -7,
//            }, 

//      new StaticItem {
//                dbName = "item_the_pink_pulsar_PsychoticPie",
//                FriendlyName = "Pink Pulsar ",
//                PortraitUrl = "Vibrator_meddle.png",
//                Description = "A large pink vibrator that resembles a human penis, with a clear display of black rotating beads in the middle. It comes with three settings: slow, medium, and fast. This particular vibrator is powered by soul energy, an innovative new technology! Using this item will make you feel great, obviously, restoring some mana at the cost of willpower.",
//                ItemType = PvPStatics.ItemType_Consumable_Reuseable,
//                Findable = false,
//                MoneyValue = 50,
//                ReuseableManaRestore = 30,
//                ReuseableHealthRestore = -10,
//                UseCooldown = 15,
                
//            }, 

//               new StaticItem {
//                dbName = "item_cateye_glasses_psychoticpie",
//                FriendlyName = "Cat Eye Glasses",
//                PortraitUrl = "CatEyeGlasses_meddle.png",
//                Description = "A pair of Cat Eye Glasses. Since they used to be human, they can adjust themselves according to their wearer's size and needs. Wearing them will enhances one's vision, making one more likely to spot weaknesses in their enemies and land a critical hit. This also increases their wearer's accuracy. Of course, there are the stigmas of 'nerd', 'geek, 'loser', and other such things attached to glasses, so wearing them will reduce one's willpower and mana maximums.",
//                ItemType = PvPStatics.ItemType_Hat,
//                Findable = false,
//                MoneyValue = 50,
//                ExtraSkillCriticalPercent = 7,
//                HealthBonusPercent = -2.4M,
//                ManaBonusPercent = -2.4M,

                
//            }, 

//          new StaticItem {
//                dbName = "item_miniature_horse_whiteflameK",
//                FriendlyName = "Miniature Carousel Horse",
//                PortraitUrl = "carousel_horse.png",
//                Description = "This orphaned carousel horse was created via curse and is imbued with the vengeful, yet lifeless spirit of its original owner. It desperately tries to absorb magic and essence in order to become living matter once again, but it can never achieve it. Absorbed magic just reinforces the form as part of the nature of the curse. This accessory drains mana and willpower from its user per turn and greatly inhibits mobility due to its size and weight, but the item's desire for magic also greatly increases the chance to evade spells which may strike the carousel horse instead. While not particularly effective for combat, it lends some assurance of protection while sleeping.",
//                ItemType = PvPStatics.ItemType_Accessory,
//                Findable = false,
//                MoneyValue = 50,
//                HealthRecoveryPerUpdate = -1.25M,
//                ManaRecoveryPerUpdate = -2.0M,
//                MoveActionPointDiscount = -.8M,
//                EvasionPercent = 25,
                
//            },  new StaticItem {
//                dbName = "item_living_lingerie_PsychoticPie",
//                FriendlyName = "Living Lingerie",
//                PortraitUrl = "gartered_panties_with_stockings_meddle.png",
//                Description = "A slightly transparent pair of panties with garter straps and back seam stockings. Extremely feminine, and great for attracting the attention of hot guys. The delicate appeal of this lingerie aids in the removal of transformation energy upon cleansing, and reduces your chance of misfiring.  With all the lustful gazes you'll be getting, however, your willpower won't be in the best shape.",
//                ItemType = PvPStatics.ItemType_Underpants,
//                Findable = false,
//                HealthBonusPercent = -12,
//                SpellMisfireChanceReduction = 4,
//                CleanseExtraTFEnergyRemovalPercent = 2.15M,
                
//            },  new StaticItem {
//                dbName = "animal_hung_stallion_lexam_hachik",
//                FriendlyName = "Hung Stallion",
//                PortraitUrl = "stallion_Danaume.jpg",
//                Description = "This majestic stallion gives it's owner a faster movement speed, although very loud and obvious. They also gain a willpower boost from dominating such a powerfully dominant creature",
//                ItemType = PvPStatics.ItemType_Pet,
//                Findable = false,
//                MoneyValue = 50,
//                SneakPercent = -30,
//                MoveActionPointDiscount = .35M,
//                HealthBonusPercent = 4,
                
//            },  new StaticItem {
//                dbName = "animal_fairy_familiar_Varn",
//                FriendlyName = "Fairy Familiar",
//                PortraitUrl = "fairy_familiar_Meddle.png",
//                Description = "From a distance, she looks like a tiny ball of incandescent light, supported in the air by a large set of wings - though looking closer, you see she is a glowing, 3-inch tall, naked, redheaded female - and an attractive one at that.  Her magical nature lends quite a bit to her owner's spellpower.  While not technically inanimate, she is permanently bound to her owner; escape is not only impossible, but the thought of it would never even occur to her.  Therefore - it is odd that she spends much of her time trapped in a bottle - perhaps her owner is merely seeking some peace and quiet?  She does seem quite... hyperactive... and you can imagine her getting on anyone's nerves very quickly.",
//                ItemType = PvPStatics.ItemType_Pet,
//                Findable = false,
//                MoneyValue = 50,
//                HealthBonusPercent = -3M,
//                SpellExtraTFEnergyPercent = 4,
//                ExtraSkillCriticalPercent = 2,
//                SpellExtraHealthDamagePercent = 2,

                
//            }, 

//                new StaticItem {
//                dbName = "item_ball_mask_Haretia",
//                FriendlyName = "Sparkly Masquerade Ball Mask",
//                PortraitUrl = "masquerade_mask_Meddle.png",
//                Description = "Many masks hide secrets behind them. This mask hides its own. This mask used to be a person. Now their soul is bound in this red masquerade ball style mask with red and black feathers at the right side and decorative gems all over. Wearing this mask might narrow your view a little but that is a small price to pay for this gorgeous mask and its magical abilities.",
//                ItemType = PvPStatics.ItemType_Hat,
//                Findable = false,
//                MoneyValue = 50,
//                SpellHealthDamageResistance = 5,
//                SpellTFEnergyDamageResistance = 5,
//                SpellMisfireChanceReduction = -5,
//                ExtraSkillCriticalPercent = -5,


                
//            }, 

//         new StaticItem {
//                dbName = "item_maid_dress_Budugu2004",
//                FriendlyName = "Maid Dress",
//                PortraitUrl = "maid_dress_Meddle.png",
//                Description = "Once human, this fallen player met an opponent who was able to channel his naughtiest fantasies and turn him into a satin french maid dress. Tainted by this strong spell, this dress lust for cleaning and will make its owner a good industrious maid. But, as a servant of the manor, the maid knows she's at the bottom of the chain and suffers a strong willpower penalty.",
//                ItemType = PvPStatics.ItemType_Shirt,
//                Findable = false,
//                MoneyValue = 50,
//                MoveActionPointDiscount = .25M,
//                CleanseExtraTFEnergyRemovalPercent = 1,
//                HealthBonusPercent = -4,
//            }, 

//           new StaticItem {
//                dbName = "item_bondage_kitten_magazine_Haretia",
//                FriendlyName = "Bondage Kitten Magazine",
//                PortraitUrl = "bondage_magazine_Meddle.png",
//                Description = "The bondage kitten magazine is unlike any other publication. If you look at the cover you see one of your defeated foes. Trapped in the picture, but not in time like other models. Your enemy is very much alive and looking at you with a pleading expression. How you would like to play with her, a petite twenty something with red hair and green eyes, covered in the most exquisite bondage equipment you could imagine. But the strangest thing happens when you open up the magazine. Before you can even look at the first page a wave of magic flows through you. When you open the page you are astonished anew seeing and reading about a bondage session with you and your “bondage kitten” from your submissive foe's perspective. Even pictures are provided, for every highlight of your “session”. Reading about it makes you wish you could play with your “bondage kitten” yourself. Yet every time you read the stories inside you have a slight déjà vu as if as you already know what happens. At the end, when you close the magazine, you are greeted by your bondage kitten who seems now to be quite satisfied. It saddens you that the magic of the magazine takes some time to recharge but reading about these sessions manages to lift your spirit every time.",
//                ItemType = PvPStatics.ItemType_Consumable_Reuseable,
//                Findable = false,
//                MoneyValue = 50,
//                UseCooldown = 25,
//                ReuseableHealthRestore = 32,
                
//            }, 

//         new StaticItem {
//                dbName = "item_sparkly_cocktail_dress_Haretia",
//                FriendlyName = "Sparkly Cocktail Dress",
//                PortraitUrl = "cocktail_dress_Meddle.png",
//                Description = "Your very own sparkly cocktail-dress, designed to catch everyone’s eyes with its decorative gems.  Tailored to highlight your figure with its pinkish-red velvet form, it's perfect for a night of fun in clubs or for small informal gatherings.",
//                ItemType = PvPStatics.ItemType_Shirt,
//                Findable = false,
//                HealthBonusPercent = 4,
//                SpellHealthDamageResistance = 8,
//                SneakPercent = -20,
//            },

//             new StaticItem {
//                dbName = "animal_wolf_cub_Alessandro",
//                FriendlyName = "Wolf Cub",
//                PortraitUrl = "wolf_cub_lux.png",
//                Description = "This cute form attacks for its owner, ferociously holding any enemy from moving as fast. Its speed and stealth makes it a great ally for any mage's or witches' menagerie. Follows it's owner around lovingly and loyally.  Anytime the wolf cub gets to serve it does so without any hesitation. It will occasionally be seen carrying an item in its mouth when not full of an enemy.",
//                ItemType = PvPStatics.ItemType_Pet,
//                Findable = false,
//                MoneyValue = 50,
//                EvasionPercent = 10,
//                EvasionNegationPercent = 4,
//                MeditationExtraMana = -2,
//                CleanseExtraTFEnergyRemovalPercent = -1,
//                ExtraInventorySpace = 1,
//                SneakPercent = 20,
//                HealthBonusPercent = -2,

//            }, new StaticItem {
//                dbName = "item_Frilly_Petticoat_Budugu2004",
//                FriendlyName = "Frilly Petticoat",
//                PortraitUrl = "french_maid_petticoat_Meddle.png",
//                Description = "This white frilly petticoat is made to add some fluff to a dress or can simply be worn as a tutu. It makes its owner feel    rejuvenated, restoring a good amount of will each turn. But watch your step, the sightliest bent will show some of your undies to anyone     crossing your part. The childish soul trapped into the frills will also tries to distract you in the most critical situation.",
//                ItemType = PvPStatics.ItemType_Pants,
//                Findable = false,
//                HealthBonusPercent = 7,
//                SneakPercent = -10,
//                SpellMisfireChanceReduction = -10,
//                EvasionPercent = -15,

//}, new StaticItem {
//         dbName = "item_Fishnet_Tights_Zatur",
//         FriendlyName = "Fishnet Tights",
//         PortraitUrl = "fishnet_tights_Meddle.png",
//         Description = "These lovely and sexy looking tights are designed to completely encase one's lower torso, legs and all. The holes which give them a net like appearance are close enough to just barely hide the skin, giving others only a tantalizing peak. Wearing them will draw many an eye to your legs, and the stares you'll feel might not be easy to keep at bay, but there's something definitely magical about them. Well, more magical than they used to be, as they were some poor person at first.",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         HealthRecoveryPerUpdate = -.25M,
//         ManaRecoveryPerUpdate = .3M,
//         SpellExtraTFEnergyPercent = 5,
//         CleanseExtraTFEnergyRemovalPercent = -1,

//}, new StaticItem {
//         dbName = "animal_Familiar_Feline_Blood_Knight",
//         FriendlyName = "Familiar Feline",
//         PortraitUrl = "cat_familiar_lux.png",
//         Description = "This pet gives its owner an increased flow of magical energy, and support in casting their spells making them more powerful. Even so its curiosity and mischievous nature are a little contagious, making its owner's magic more likely to blow up in their face, and slowly sapping their will.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthRecoveryPerUpdate = -.1M,
//         SpellMisfireChanceReduction = -2M,
//         MeditationExtraMana = 5,
//         ExtraSkillCriticalPercent = 3,
//         SpellExtraTFEnergyPercent = 5,

//}, new StaticItem {
//         dbName = "animal_Cursed_Doll_Rust",
//         FriendlyName = "Cursed Doll",
//         PortraitUrl = "strawdoll_Meddle.png",
//         Description = " Just being around this straw doll makes you feel sick to your stomach, you can't help but feel pity for whoever this object was originally but anything that was left of them is long gone. The doll saps away all life around it and is near painful to touch but at the same time gives them a feeling of immense power. Only a fool would keep this doll lest they be consumed by it.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = -14,
//         HealthRecoveryPerUpdate = -3.5M,
//         ManaRecoveryPerUpdate = -2.5M,
//         SpellExtraHealthDamagePercent = 20,
//         ExtraSkillCriticalPercent = 10,

//}, new StaticItem {
//         dbName = "item_Leather_Whip_Christy_D",
//         FriendlyName = "Leather Whip",
//         PortraitUrl = "Whip_Meddle.png",
//         Description = "This leather whip gives it's holder a bolder outlook on life. No goal they can't reach, no struggle they can't overcome, no sexy slave-girl or boy they can't bring to heel.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthBonusPercent =2,
//         ManaBonusPercent = -2,
//         ExtraSkillCriticalPercent = 4,
//         SpellExtraHealthDamagePercent = 3,

//}, new StaticItem {
//         dbName = "item_Magic_choker_Budugu2004",
//         FriendlyName = "Magic choker",
//         PortraitUrl = "magic_chocker_Meddle.png",
//         Description = "This delicate piece of jewelry is not what it seems. If you try to take a closer look at the cameo, you might see a Victorian lady moving and reacting to your presence! A powerful wizard trapped this poor soul into a glass prison and now uses it as a magical artifact to protect themselves from spells. If you look close enough, the lady in the cameo may even wink and blow you a kiss, but this will not be a sign of good fortune...",
//         ItemType = PvPStatics.ItemType_Undershirt,
//         Findable = false,
//         SpellHealthDamageResistance = 1.5M,
//         SpellTFEnergyDamageResistance = 1.5M,
//         SpellMisfireChanceReduction = .25M,

//}, new StaticItem {
//         dbName = "item_Maid_Headband_Budugu2004",
//         FriendlyName = "Maid headband",
//         PortraitUrl = "crown_of_order_Meddle.png",
//         Description = "This cute little maid cap is more than an accessory. The pour soul trapped in this form is cursed with a strong obsessive-compulsive disorder for cleaning. Like a radar, it will point its owner every single chores that need to be attended every room crossed. Some people went crazy wearing this headband, staying mindless maid forever even after removing the cap. They wanted to benefit from the extra awareness this headband can procure, but got their will slowly drained away.",
//         ItemType = PvPStatics.ItemType_Hat,
//         Findable = false,
//         HealthRecoveryPerUpdate = -.5M,
//         SpellHealthDamageResistance = 8,
//         SpellTFEnergyDamageResistance = 8,
//         SpellMisfireChanceReduction = 3,

//}, new StaticItem {
//         dbName = "animal_White_Tiger_Zatur",
//         FriendlyName = "White Tiger",
//         PortraitUrl = "white_tiger_Luxianne.png",
//         Description = "This large and majestic looking tiger, with the rare coloration of white fur with black stripes, stalks through the town as if it owns the place. And it can do whatever it pleases. Then again, who's going to tell a 500 lb. carnivore that they can't do what it wants? The large feline simply radiates mystery and magic.  Whether this is because they might have been a former mage or because of white tigers' reputation in many a Vegas magic show is anyone's guess.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         ManaBonusPercent = 5,
//         SpellTFEnergyDamageResistance = 3.5M,
//         ManaRecoveryPerUpdate = -.1M,
//         HealthBonusPercent = -1,

// }, new StaticItem {
//         dbName = "animal_Magic_Slut_Ball_Varn",
//         FriendlyName = "Magic Slut Ball",
//         PortraitUrl = "bimboSphere_Meddle.png",
//         Description = "It's not just a pet, it's a Magic Slut Ball!  Know-er of the unknown, See-er of the unseen, source of all mystical knowledge in the universe!  Or... not.  While one could pick up this sphere, less than a foot in diameter, and shake it - all it would likely do is cause the tiny, improbably proportioned woman inside to take a brief break from her marathon masturbation session, look up from the pool of her milk and sexual fluids she was lying in (and which you have just caused to drench her) - and curse them out.  Then again, she seems so focused on bringing herself off, that she may lack the will to stop even to acknowledge her owner; her answer may well be 'All signs point to 'Yes! YES!  GOD YES!!!!'  Truly a valuable item, her owner will likely take great pains not to lose or break her - perhaps moving more carefully, or filling their pack with fewer lesser goods...",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = 8,
//         HealthRecoveryPerUpdate = 1.5M,
//         SpellHealthDamageResistance = 2,
//         EvasionPercent = -10,
//         ExtraInventorySpace = -.2M,

//}, new StaticItem {
//         dbName = "item_Runic_Dildo_Blood_Knight",
//         FriendlyName = "Runic Dildo",
//         PortraitUrl = "runic_dildo_Meddle.png",
//         Description = "This item saps its owners will to fuel its dark powers. In exchange for this tribute it grants them increased offensive powers, making their attacks harder to evade, transform targets more swiftly, and more likely to inflict devastating blows.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthRecoveryPerUpdate = -.6M,
//         HealthBonusPercent = -6,
//         ExtraSkillCriticalPercent = 3,
//         SpellExtraHealthDamagePercent = 3,
//         SpellExtraTFEnergyPercent = 3,

//}, new StaticItem {
//         dbName = "animal_Fennec_Fox_Elynsynos",
//         FriendlyName = "Fennec Fox",
//         PortraitUrl = "fennec_fox_luxianne.png",
//         Description = "This pet gives its owner extraordinary warning of enemy attacks. Its large ears able to pinpoint potential threats before they strike and giving their owner time to prepare themselves before an attack. Their excellent nose is also capable of following the scent of any fleeing prey. However, the Fox loves to be played with and is a drain on its owners willpower because of all the love and attention it wants.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         SpellMisfireChanceReduction = 2,
//         EvasionNegationPercent = 10,
//         HealthBonusPercent = -1.4M,
//         SpellTFEnergyDamageResistance = 4,
//         SpellHealthDamageResistance = 4,

//}, new StaticItem {
//         dbName = "animal_Adorable_Donkey_Foal_LexamTheGemFox_&_Hachik0048",
//         FriendlyName = "Adorable Donkey Foal",
//         PortraitUrl = "adorable_donkey_dominque.png",
//         Description = "Standing on all fours and looking around curiously is a very cute little donkey. Even though they once were human their mind has been regressed to little more than an adorable little newborn jenny. She brays cutely and prances around, happy to play and frolic like a cute little foal. This pet gives it's owner a way to distract their opponent with the cute little foals overwhelming innocence and cuteness.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         SneakPercent = -10,
//         EvasionNegationPercent = 5,
//         MoveActionPointDiscount = -.2M,
//         SpellHealthDamageResistance = 2.5M,
//         SpellTFEnergyDamageResistance = 2.5M,

// }, new StaticItem {
//         dbName = "item_Wizard's_Wand_themorpher606",
//         FriendlyName = "Wizard Wand",
//         PortraitUrl = "magic_wand_Meddle.png",
//         Description = "A magic wand, a thin black stick with a mesmerizing pink, glowing crystal. It must have been owned by a powerful wizard.  If one were to look closely inside the crystal, they might still see the spirit of the mage that this wand used to be.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         SneakPercent = -13,
//         CleanseExtraHealth = -3,
//         MoveActionPointDiscount = -.25M,
//         SpellExtraHealthDamagePercent = 4,
//         SpellTFEnergyDamageResistance = 4,
//         SpellMisfireChanceReduction = 2,

//}, new StaticItem {
//         dbName = "item_Captured_Souls_Alessandro_Stamegna",
//         FriendlyName = "Latex Collar (Unisex)",
//         PortraitUrl = "latex_collar_Meddle.png",
//         Description = "You are a latex collar and you sit around your owners neck quite snugly. You refract the lights from the clubs strobes as well as the black-lights that surround some of the rooms. You have D-rings that encompass you from all sides allowing for leashes and chains to be run through their openings. A clasp lies on the back of your form with a lock hanging from it. You drain a small amount of will from your wearer allowing yourself to live in their pleasure eternally.",
//         ItemType = PvPStatics.ItemType_Hat,
//         Findable = false,
//         HealthBonusPercent = -3,
//         ManaBonusPercent = -5,
//         HealthRecoveryPerUpdate = -.2M,
//         ManaRecoveryPerUpdate = 3,
//         SpellExtraHealthDamagePercent = 8.35M,

//}, new StaticItem {
//         dbName = "item_Feather_duster_Budugu2004",
//         FriendlyName = "Feather duster",
//         PortraitUrl = "Feather_Duster_Meddle.png",
//         Description = "This feather duster can be used to remove magical dust on your body left by your enemies' transformation spells. If you inspect it closely, you may see faint details of a face melted in the tip of the handle, since it actually used to be someone. It actually enjoy beings a duster now, and its joy is contagious. You even may find yourself dusting shelves here and there, slowing you down in your adventures.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         MoveActionPointDiscount=-.1M,
//         SpellExtraTFEnergyPercent = 6,
//         CleanseExtraTFEnergyRemovalPercent=.5M,

//}, new StaticItem {
//         dbName = "item_Black_Suit_Jacket_Arbitrary_Hal",
//         FriendlyName = "Black Suit Jacket",
//         PortraitUrl = "suitjacket_Meddle.png",
//         Description = "This well-tailored silk suit jacket helps project an aura of competence, even if the wearer doesn't really know what they are doing.  It seems to shine with unnatural life, drawing the eye and making the wearer seem to be more than they are.",
//         ItemType = PvPStatics.ItemType_Shirt,
//         Findable = false,
//         HealthBonusPercent = 1.4M,
//         ManaRecoveryPerUpdate = -1,
//         SpellHealthDamageResistance = 3,
//         SpellMisfireChanceReduction = 2,

//}, new StaticItem {
//         dbName = "item_Magic_Nursing_Bra_Purple_Autumn",
//         FriendlyName = "Magic Nursing Bra",
//         PortraitUrl = "MaternityBra_Meddle.png",
//         Description = "A very soft and comfy nursing bra, typically worn by women who are lactating. This bra does not discriminate, however. If you're not lactating, just slip this on and you will be. A blessing or a curse, depending on your perspective. Your magic milk makes you feel more powerful and your spells are significantly more effective. Although, powerful magic is a double-edged sword, and you do feel a bit more vulnerable as well.",
//         ItemType = PvPStatics.ItemType_Undershirt,
//         Findable = false,
//         ManaRecoveryPerUpdate = 2,
//         ExtraSkillCriticalPercent = 3.5M,
//         SpellExtraTFEnergyPercent = 10,
//         SpellTFEnergyDamageResistance = -10,


//}, new StaticItem {
//         dbName = "item_Black_Silk_Pants_Arbitrary_Hal",
//         FriendlyName = "Black Silk Pants",
//         PortraitUrl = "silk_pants_Meddle.png",
//         Description = "These black silk pants are entirely suitable for use as part of a formal or semi-formal outfit.  They are elegant and classy, whilst also being easy to move in, aiding their owner in keeping focused on the task at hand.  The pants seem to aid in their wearer's balance on their own, allowing them to comport themselves with a supernatural grace.",
//         ItemType = PvPStatics.ItemType_Pants,
//         Findable = false,
//         EvasionPercent = 5,
//         SpellMisfireChanceReduction = 3,
//         MoveActionPointDiscount = .07M,
//         SpellTFEnergyDamageResistance = -3.5M,

//}, new StaticItem {
//         dbName = "animal_BDSM_Pony_Girl_LexamTheGemFox",
//         FriendlyName = "BDSM Pony Girl",
//         PortraitUrl = "bdsm_pony_girl_Meddle.png",
//         Description = "Your faithful and obedient pony girl. You obliterated her old self, leaving her little more than an animal brain in a human body. She is obedient, sexy, and loyal and although a bit of a handful at times you can definately rely on your little pony.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//        ManaBonusPercent = -20,
//        SpellHealthDamageResistance= 7.5M,
//        SpellTFEnergyDamageResistance= 7.5M,


//}, new StaticItem {
//         dbName = "item_Mary_Janes_Christopher",
//         FriendlyName = "Mary Janes",
//         PortraitUrl = "5_inch_heels_Meddle.png",
//         Description = "These feminine Mary Janes topped with silk bobby socks imbue their owner with an innocent and demure appearance, their black surface smooth and elegant as a woman should be, it seems to say. The five inch heels may be somewhat less innocent, but they are certainly sexy, giving you that much needed lift so you can confidently strut.",
//         ItemType = PvPStatics.ItemType_Shoes,
//         Findable = false,
//         SneakPercent = -10,
//         MoveActionPointDiscount = -.05M,
//         ExtraSkillCriticalPercent = 3.5M,
//         SpellExtraTFEnergyPercent = 5,

//}, new StaticItem {
//         dbName = "item_Ioun_Stone_Aidyn_Bright",
//         FriendlyName = "Ioun Stone",
//         PortraitUrl = "gemstone_Lux.gif",
//         Description = "This item is a small purple crystal that orbits its owner and gives it improved mana and willpower recovery at the cost of evasion and misfire due to having to concentrate heavily upon its use.  It used to be a productive human being and even now produces willpower and mana like a good, diligent worker.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         EvasionPercent = -20,
//         SpellMisfireChanceReduction = -20,
//         HealthRecoveryPerUpdate = 7.5M,
//         ManaRecoveryPerUpdate = 7.5M,

//}, new StaticItem {
//         dbName = "item_Red_Wavy_Wig_Kirshwasser",
//         FriendlyName = "Luscious Red Wig",
//         PortraitUrl = "red_wavy_wig_Meddle.png",
//         Description = "This wig is not at all like any other wigs, for its quality is unmatched. Made from real human hair, and then some, this wig will forever maintain its beauty and shine, feeding of the magical energy of the transformed human. Anyone who lays eyes upon this wig are sure to be enthralled, the wearer included, but the ",
//         ItemType = PvPStatics.ItemType_Hat,
//         Findable = false,
//         EvasionPercent = -5,
//         EvasionNegationPercent = 5,
//         SpellExtraTFEnergyPercent = 10,
//         SpellMisfireChanceReduction = -3,

//}, new StaticItem {
//         dbName = "animal_Milk_Cow_Sampleguy",
//         FriendlyName = "Milk Cow",
//         PortraitUrl = "cow_sampleguy.jpg",
//         Description = "This big lumbering milk machine is quite useful. When you need a little more resolve, you can always grab a teat and pour a cup (or a gallon or two) and take a quick breather. When you need to dodge a spell's harmfull blast you can let your little milk cow take some of the transformative effects. However sneaking around is essentially impossible with all the mooing that your victim will be mindlessly making. Also moving at a fast pace will be 'udderly' impossible.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = 20,
//         HealthRecoveryPerUpdate = 3,
//         SneakPercent = -18,
//         MoveActionPointDiscount = -.4M,

//}, new StaticItem {
//         dbName = "item_Count_Cuddles_Berrie_Valentine",
//         FriendlyName = "Count Cuddles Plush Bear",
//         PortraitUrl = "Count_Cuddles_Luxianne.png",
//         Description = "This Cuddly little noble here is a plush Polar bear styled after the Lord of Valentine Castle. The soul trapped in the little Darling has been made to believe they are the cursed lover of their owner and will do everything in their power to help them purge harmful Energies  as well as recover their will when carried around so that one day they may be able to Restore them to their 'former' self and then live happily ever after.  While originally quite small at around 2 feet this magical plush seems to grow in size when it increases in power, making it extremely hard to lug around let alone dodge spells while being hugged.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthRecoveryPerUpdate = 2,
//         EvasionPercent = -17,
//         MoveActionPointDiscount = -.25M,
//         CleanseExtraTFEnergyRemovalPercent = 4,

//}, new StaticItem {
//         dbName = "item_Platform_heels_Christopher",
//         FriendlyName = "Platform Heels",
//         PortraitUrl = "platform_heels_Meddle.png",
//         Description = "These sexy high heels can't help but show off, their height and sound preventing any attempt of going unnoticed. They also limit your movement somewhat, forcing you to swing your hips in a wide fashion and place on foot in front of the other. All this attention isn't so bad however, as anyone seeing you wearing these can't help but stop and stare, distracting them from their actions. ",
//         ItemType = PvPStatics.ItemType_Shoes,
//         Findable = false,
//         HealthBonusPercent = 10,
//         MoveActionPointDiscount = -.15M,
//         SneakPercent = -10,
//         ExtraSkillCriticalPercent = 4,

//}, new StaticItem {
//         dbName = "item_Cheerleader_Spankies_Zatur",
//         FriendlyName = "Cheerleader Spankies",
//         PortraitUrl = "spankmes_Meddle.png",
//         Description = "If you're going to be a cheerleader jumping around and kicking in a short skirt, you need something to cover your panties from prying eyes. Normally. St. Circe Cheerleaders seem to think that's the best part of cheerleading though, and their version of the usual cheerleader spankies are practically panties themselves. They're also emblazoned with the words 'Spank Me' on them, quite possibly as an invitation. Despite their small size, they're quite comfortable and helpful for relaxing in, but out in the field, they can be rather distracting. For both the cheerleader dealing with stares, and people trying to stare.",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         EvasionNegationPercent = 2,
//         EvasionPercent = -6,
//         MeditationExtraMana = 2,
//         CleanseExtraHealth = 2,
//         CleanseExtraTFEnergyRemovalPercent = 2,

//}, new StaticItem {
//         dbName = "item_Cherry_Sucker_Christopher",
//         FriendlyName = "Cherry Red Sucker",
//         PortraitUrl = "sucker_Meddle.png",
//         Description = "Long, red and proud, this penis shaped lollipop has an unbelievably sweet cherry flavour, followed by a soft cream filling. Due to its magical nature, it will never be consumed, but suck it for long enough and it might just reward you with a taste of its creamy filling. It is somewhat distracting however, lowering concentration of the user whilst they happily lick its surface.",
//        ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthRecoveryPerUpdate = -.75M,
//         ManaRecoveryPerUpdate = 2,
//         EvasionPercent = -9.5M,
//         SpellExtraHealthDamagePercent = 8,

//}, new StaticItem {
//         dbName = "item_Black_Leather_Shoes_Arbitrary_Hal",
//         FriendlyName = "Black Leather Shoes",
//         PortraitUrl = "Leather_Shoes_Meddle.png",
//         Description = "These fine leather shoes might once have been able to walk on their own as an independent mage, but now all they do is insulate their owner's feet from any sharp objects or extreme temperatures below.  It's a largely thankless task, performed under a fair amount of pressure, but this form is well suited to executing it while preserving both comfort and style.",
//         ItemType = PvPStatics.ItemType_Shoes,
//         Findable = false,
//         ManaBonusPercent = 4.5M,
//         SneakPercent = -1.5M,
//         SpellHealthDamageResistance = -1,
//         SpellTFEnergyDamageResistance = 3.5M,


//}, new StaticItem {
//         dbName = "animal_Pocket_Dragon_Julian_Chance",
//         FriendlyName = "Pocket Dragon",
//         PortraitUrl = "pocket_dragon_Meddle.png",
//         Description = "This is a dragon, straight out of myths and legends!  Except this one seems kind of small.  At a scant two feet in length, it's better suited for lighting cigarettes than razing villages.  It's just the right nice size for a pet though.  It's intensely magical nature makes it difficult to affect with any form of magic, so the bonding spell requires a lot of extra power.  On the other hand though, its magical nature makes it exceptional at blocking incoming spells and reducing their effect when it protects its master!",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         ManaBonusPercent = -15,
//         ManaRecoveryPerUpdate = -.5M,
//         SpellTFEnergyDamageResistance = 14,


//}, new StaticItem {
//         dbName = "item_Cheerleader_Shell_Zatur",
//         FriendlyName = "Cheerleader Shell",
//         PortraitUrl = "Cheerleader_Shell_Meddle.png",
//         Description = "In cheerleading circles, this sleeveless type of vest is known as a 'shell.' Normally a shell is rather conservative, but this is a St. Circe Cheerleader's shell, so 'conservative' is the last word one would use to describe this sweater. Somehow it also seems to work on same principle of Chainmail Bikinis and despite covering about 25% of the body, seems to provide more protection than your usual sweater. Mage-based clothing tends to be special after all.",
//         ItemType = PvPStatics.ItemType_Shirt,
//         Findable = false,
//         HealthBonusPercent = 6,
//         SneakPercent = -10,
//         SpellHealthDamageResistance = 2,
//         SpellTFEnergyDamageResistance = 2,

//}, new StaticItem {
//         dbName = "item_Playful_Pussy_Rayner",
//         FriendlyName = "Playful Pussy",
//         PortraitUrl = "Honeypot_Luxianne.png",
//         Description = "When worn this ball of flesh, which just so happens to be a pussy, merges with its owner to replace whatever boring parts they had before for a semi-aware pussy ready for action. The newly formed pussy would rather be on someone then left on the floor and so gives what help it can to stick around. ",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         ManaRecoveryPerUpdate = 1.5M,
//         SpellTFEnergyDamageResistance = 3,

//}, new StaticItem {
//         dbName = "item_High-Quality_Onahole_LexamTheGemFox",
//         FriendlyName = "High-Quality Onahole",
//         PortraitUrl = "onahole_Luxianne.png",
//         Description = "This item gives it's owner something that is almost always ready and eager to fuck without having to wait for it to recharge. It is wet, hot, and ready to help her owner nut.",
//         ItemType = PvPStatics.ItemType_Consumable_Reuseable,
//         Findable = false,
//         UseCooldown = 20,
//         ReuseableHealthRestore = 30,
//         ReuseableManaRestore = -10,

//}, new StaticItem {
//         dbName = "animal_Tentacled_Terror_Varn",
//         FriendlyName = "Tentacled Terror",
//         PortraitUrl = "tentacle_monster_Meddle.png",
//         Description = "Crikey, a Tentacle Beast!  You thought these things only existed in Japan!  Regardless, here one is, and it seems to be staring at you with its many eyes, contemplating whether you would make just as agreeable a target as its natural schoolgirl prey.  While its \"lickings\" and \"headbutts\" are quite fearsome, it lacks the native intelligence to pull off a well-timed, devastating assault; if it is being commanded by a mage with a grasp of tentacle-based tactics, it may become truly threatening.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = 30,
//         ManaBonusPercent = 10,
//         SpellHealthDamageResistance= -16.5M,


//}, new StaticItem {
//         dbName = "item_Ruby_Red_Lipstick_Greg_Mackenzie_and_Fiona_Mason",
//         FriendlyName = "Ruby Red Lipstick",
//         PortraitUrl = "Lipstick_Meddle.png",
//         Description = "This item is a Lipstick. When its owner carries it around they get an boost in their spell power when transforming their opponent. It is said that when the owner of this lipstick wears it they can dazzle everyone into submitting to their beauty although it isn't proven to be true as everyone who has seen someone wearing this lipstick has been changed into another item to be used. Although at first it just looks like a lipstick, once the lid has been taken off and roll it up, you can see the figure of a woman who seems to be smiling. At times it is said this woman will talk but that's impossible, after all. It's just a lipstick, Right?",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthBonusPercent = 6,
//         ManaBonusPercent = 4,
//         HealthRecoveryPerUpdate = -1,
//         SneakPercent= -5,
//         SpellExtraTFEnergyPercent= 2,

//}, new StaticItem {
//         dbName = "animal_Tiny_Mouse_Girl_Estyz",
//         FriendlyName = "Tiny Mouse Girl",
//         PortraitUrl = "tiny_mouse_girl_curry_senpai.png",
//         Description = "An adorable tiny mouse girl, perfectly fitted for her owner's pocket or shoulder. Her white furred body is almost entirely that of an upright walking mouse, safe for her humanoid face and hairstyle, and her clear, intelligent eyes. Make no mistake, even though she looks way more like a mouse than a human in her current form, she's every bit as witty and smart as before, probably even more so. Though shy in nature, she’s often seen scouting just ahead of her master or mistress, trying to protect them from harm by alerting them in time of attackers. Or, in case their owner brought enough nibbles, happily hiking along in their pocket and stuffing her face. ",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = -2,
//        SneakPercent= 10,
//        EvasionPercent= 7,
//        SpellTFEnergyDamageResistance= -2.3M,

//}, new StaticItem {
//         dbName = "item_Practical_Transformations_and_the_Metaphysical_Realm_Varn",
//         FriendlyName = "Practical Transformations and the Metaphysical Realm",
//         PortraitUrl = "page_turner_Meddle.png",
//         Description = "Your soul has been bound within the pages of a ninth edition copy of Practical Transformations and the Metaphysical Realm.  Thankfully, for the moment, the book is closed, and you are merely passively augmenting your new owner's magical skills.  But soon enough, they will open it again, and you will find yourself living out the scene on the page they have turned to.  Tormented by angels, demons, faeries, furries, students, teachers, and various sorts of bimbos (over four kinds!) your existence with its pages is a rough, though at times highly pleasurable one.  On rare occasions, your owner even uses the book to suction in another rival mage; though their imprisonment will only be temporary, doing so does let you play the dominant role for a time in the various scenes, leaving the rival considerably more vulnerable.  You know you will never escape these pages ... though perhaps with time, you will come to enjoy your new existence ...",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         ManaBonusPercent = -10,
//        EvasionPercent= -10,
//        SpellExtraTFEnergyPercent= 20,
//        CleanseExtraTFEnergyRemovalPercent= -2,

//}, new StaticItem {
//         dbName = "item_Disembodied_Boobs_Swogrider",
//         FriendlyName = "Disembodied Boobs",
//         PortraitUrl = "disembodied_chest_Meddle.png",
//         Description = "Once a powerful spell caster, these bouncy boobs were created by focusing their bodies down and condensing them into their best asset, or rather, assets. Still completely alive and conscious, these titties are 100% biological, merging into the flesh of whoever may wear them to give them a bigger pair, or perhaps just an extra pair! A neat perk mostly unknown to the flat-chested, you can actually keep stuff in your cleavage! These jiggly flesh-globes might seem like fun, but don't forget there's still a person in there! They'll send you telepathic compulsions to grope yourself (and them) from time to time, but don't worry, it feels just as good for you as it does for them!",
//         ItemType = PvPStatics.ItemType_Undershirt,
//         Findable = false,
//         HealthRecoveryPerUpdate = 1.5M,
//        ExtraSkillCriticalPercent= -1.5M,
//        SpellMisfireChanceReduction= -2,
//        ExtraInventorySpace= 1,

//}, new StaticItem {
//         dbName = "item_Flirty_Three-Tiered_Skirt_Martiandawn",
//         FriendlyName = "Flirty Three-Tiered Skirt",
//         PortraitUrl = "3tiered_skirt_Meddle.png",
//         Description = "Whatever this garment was before, it is now a cute, ruffled, three-tier skirt. Feminine, flirtatious, and sexy, just wearing it gives you the urge to wiggle your hips and saucily flash your assets to passersby. Your opponents are easily distracted by alluring glimpses of your derriere, helping you evade their attacks while also making them easier to hit as they stand and stare. Alas, putting yourself on public display like that does make it difficult to avoid notice, and the sheer exhilaration of such wanton exhibitionism makes it a little harder for you to concentrate on casting spells.",
//         ItemType = PvPStatics.ItemType_Pants,
//         Findable = false,
//         SneakPercent= -10,
//         EvasionPercent= 7,
//         EvasionNegationPercent= 10,
//         SpellMisfireChanceReduction= -2,

//}, new StaticItem {
//         dbName = "animal_Djinn_Slave_Danaume_Rook",
//         FriendlyName = "Djinn Slave",
//         PortraitUrl = "djinn_slave_Dauname.png",
//         Description = "This buxom and beautiful woman is dressed in light, translucent purple silks, and polished brass cuffs. Her skin is a lovely and spotless pale Arabic tan, and her long brunette hair hangs in a wild ponytail. Her deep, mystical eyes watch the world with impish wickedness that she can never voice. Lacking a nose or mouth, all she can do is watch, and respond to her master or mistress's wishes with mystical might.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         HealthBonusPercent = -15,
//        ManaBonusPercent = 18,
//        HealthRecoveryPerUpdate = -1,
//        ManaRecoveryPerUpdate = 4,

//}, new StaticItem {
//         dbName = "item_Enchanted_Tennis_Racket_themorpher606",
//         FriendlyName = "Enchanted Tennis Racket",
//         PortraitUrl = "tennis_racket_Meddle.png",
//         Description = "Once a powerful mage, this item was completely transformed into a tennis racket.  With a tough leather grip, a sturdy red handle and a blue wire net, they have become a professional piece of magic sports gear that can help its owner hit back any attack that is thrown at them.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         EvasionPercent= 3,
//         ExtraSkillCriticalPercent= .25M,
//         SpellMisfireChanceReduction= .5M,

//}, new StaticItem {
//         dbName = "item_Heart_Shaped_Sunglasses_Christopher",
//         FriendlyName = "Heart Shaped Sunglasses",
//         PortraitUrl = "heart_glasses_Meddle.png",
//         Description = "These sunglasses will not darken your day, but rather the opposite, filling it with a soothing light, converting unpleasant or unhappy scenes into far more acceptable scenarios for your eyes. They will never spoil your mood, bringing about a shining confidence which may prove helpful in these dreary times. Whether these vision altering spectacles cause you to fail to see an enemy and turned into a pair of shoes as a result, well, that's just tough luck, isn't it?",
//         ItemType = PvPStatics.ItemType_Hat,
//         Findable = false,
//         SpellExtraTFEnergyPercent= 5,
//         SpellHealthDamageResistance= 5,
//         SpellTFEnergyDamageResistance= -6,

//}, new StaticItem {
//         dbName = "item_Flirty_Tied_Crop_Top_Martiandawn",
//         FriendlyName = "Flirty Tied Crop Top",
//         PortraitUrl = "tied_croptop_Meddle.png",
//         Description = "This adorable crop top is tied in the front for convenience (since buttons can be such a bother when you're in a hurry to get naked!). Wearing it makes your feel sexy, fun and carefree, providing just enough coverage to keep your assets from being exposed, without creating too much in the way of inhibition. It's the perfect combination of girl next door and neighborhood slut! As your enemies stare at your exposed cleavage, wondering what would happen if they tugged on that bow, they are more vulnerable to willpower damage and absorb transformation energy more easily. At the same time, that flirtatiousness weakens your own will, and the nagging worry that the knot will come undone hampers your movement.",
//         ItemType = PvPStatics.ItemType_Shirt,
//         Findable = false,
//         HealthBonusPercent = -2,
//        MoveActionPointDiscount= -0.05M,
//        SpellExtraTFEnergyPercent= 3,
//        SpellExtraHealthDamagePercent= 3,

//}, new StaticItem {
//         dbName = "item_Star-Studded_Cloak_Techhead",
//         FriendlyName = "Star-Studded Cloak",
//         PortraitUrl = "star_studded_cloak_Meddle.png",
//         Description = "This beautiful cloak was once some flavor of extra-dimensional creature, bound to this world through an unwitting host, also trapped in it. It draws your eyes towards it, and as you stare into its gently pulsing form, you can see the night sky, although disconcertingly, the stars occasionally seem to shift. It can be used to pull mana from the bare ether and it boosts the wearer's magic powers noticeably, albeit unpredictably. However, in addition to its heavy bulk, its shifting form is somewhat distracting.",
//         ItemType = PvPStatics.ItemType_Shirt,
//         Findable = false,
//         HealthRecoveryPerUpdate = -1,
//         MeditationExtraMana= 3,
//         ExtraSkillCriticalPercent= 9.5M,
//         MoveActionPointDiscount= -.2M,
//         SpellMisfireChanceReduction= -10,

//}, new StaticItem {
//         dbName = "item_Knee-High_Socks_Christopher",
//         FriendlyName = "Knee-High Socks",
//         PortraitUrl = "kneehigh_socks_Meddle.png",
//         Description = "These long cotton socks are popular amongst women for their attractive appearance and practicality, breathable and comfortable no matter what the weather. You may see them on school girls and cheerleaders to match their short skirts, the energetic nature of these socks lending a youthful exuberance to the wearer, quick feet and sharp reflexes allowing them to keep up with any black widow trying to collect or attractive bimbo looking to make a new sister. However, such clothing tends to attract the wrong kind of attention, its thin surface providing little protection, lack of underwear forcing you to slow your steps somewhat, dodging more difficult as you try to cover yourself properly. ",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         EvasionPercent= -6,
//         EvasionNegationPercent= 4,
//         SpellMisfireChanceReduction= 5,
//         SpellTFEnergyDamageResistance= -2,

//}, new StaticItem {
//         dbName = "item_Fetish_Wizard's_Staff_Lily",
//         FriendlyName = "Fetish Wizard's Staff",
//         PortraitUrl = "fetish_staff_Meddle.png",
//         Description = "This item pulses with lusty fetish magic, sending lewd desires into others while drawing out their hidden kinks. Sometimes the staff even seems alive, sending it's own desires out to others, influencing them and hoping to trap them in their lusty desires.",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthBonusPercent = -7.5M,
//        HealthRecoveryPerUpdate = -.75M,
//        EvasionPercent= 6,
//        SpellExtraHealthDamagePercent= 5.5M,

//}, new StaticItem {
//         dbName = "animal_Cuddly_Pocket_Goo_Girl_GooGirl",
//         FriendlyName = "Cuddly Pocket Goo Girl",
//         PortraitUrl = "pocket_googirl_Luxianne.png",
//         Description = "Previously a mage, this cute palm-sized goo girl has found a new purpose in life as a loving pet. Whatever goals and ambitions she had seems to be of no concern to her anymore: all she wants now is an owner to cuddle up against and to keep her out of harm's way. In return, she is only all too happy to absorb any magic and fluids her owner can provide her with, causing her to drain a tiny amount of her owner's mana while making it easier to cleanse away harmful effects.  However, having a curious little goo girl constantly crawling all over can be quite distracting, making it harder for her owner to focus. ",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//         ManaRecoveryPerUpdate = -.1M,
//        MeditationExtraMana= -1,
//        CleanseExtraTFEnergyRemovalPercent= 2.7M,

//}, new StaticItem {
//         dbName = "item_Maternity_Panties_Lily",
//         FriendlyName = "Maternity Panties",
//         PortraitUrl = "maternity_panties_Meddle.png",
//         Description = "These are soft frilly panties, made for mothers to be! They are very soft, and they give off an aura of protectiveness as they cling around the owners waist. After all they need to protect the baby, and if the owner isn't pregnant, well is can grant them a little baby bump of their own.",
//         ItemType = PvPStatics.ItemType_Underpants,
//         Findable = false,
//         HealthBonusPercent = 4,
//        EvasionPercent= -15,
//        CleanseExtraHealth= 3,
//        MoveActionPointDiscount= -.1M,
//        SpellTFEnergyDamageResistance= 7.5M,

//}, new StaticItem {
//         dbName = "item_Hand_Lens_Medli",
//         FriendlyName = "Hand Lens",
//         PortraitUrl = "MagnifyingGlass_Meddle.png",
//         Description = "This item allows its owner to spot many unseen things before with much less effort. Things that used to be not-so-obvious just tend to stick out now. Enemies spells also become slightly more apparent while looking through the glass. And while your owner is escaping they can find much easier escape routes without the enemy noticing",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         SneakPercent= 4,
//        EvasionPercent= -10,
//        EvasionNegationPercent= 16,
//        SpellMisfireChanceReduction= -2,

//}, new StaticItem {
//         dbName = "item_Dog_Collar_Arnisd",
//         FriendlyName = "Dog Collar",
//         PortraitUrl = "dog_collar_Meddle.png",
//         Description = "This item is a dog collar.  When worn the wearer will feel the protection of man's best friend, increasing their protection from attackers. In turn it doesn't like its owner attacking to often, even if it would be out of self-defense.",
//         ItemType = PvPStatics.ItemType_Hat,
//         Findable = false,
//         ManaBonusPercent = -10,
//        ManaRecoveryPerUpdate = -2,
//        SpellHealthDamageResistance= 6,
//        SpellTFEnergyDamageResistance= 6,

//}, new StaticItem {
//         dbName = "item_Magical_Binding_Tattoo_Taenil_Auxifur",
//         FriendlyName = "Magical Binding Tattoo",
//         PortraitUrl = "binding_tattoo_spacedande.png",
//         Description = "This player has been turned into an ornate tattoo.  When not possessed or worn by its owner, it takes the shape of a suffering little scrap of paper, but when held against one's body the ink flows into their skin, leaving behind a fresh tattoo whose pattern can change according to its owner's emotional state.  Today they've channeled some anger and lust from its owner so their design has taken up the face of a mischievous demon girl, but who knows how they'll look tomorrow!",
//         ItemType = PvPStatics.ItemType_Accessory,
//         Findable = false,
//         HealthBonusPercent = -11,
//         ManaBonusPercent = 15,
//         ManaRecoveryPerUpdate = 1,

//}, new StaticItem {
//         dbName = "item_One-Piece_Latex_Swimsuit_Illia_Malvusin",
//         FriendlyName = "One-Piece Latex Swimsuit",
//         PortraitUrl = "onepiece_latex_swimsuit_Meddle.png",
//         Description = "This item is a smooth one-piece latex swimsuit, a single piece of latex designed for a swimmer to slip into. However, unlike a more mundane swimsuit, this one constantly exudes a smooth, damp sheen – a sign that it was once a person. It gives its wearer some amount of the magical power that the person once possessed, though the watery trail that the owner leaves behind makes them incredibly easy to track. Sometimes, it gives the phantom feeling of groping, or tightening upon the owner's body, another sign of its former status as a person...",
//         ItemType = PvPStatics.ItemType_Shirt,
//         Findable = false,
//         SneakPercent= -30,
//        ExtraSkillCriticalPercent= 2,
//        SpellExtraTFEnergyPercent= 3.6M,
//        SpellExtraHealthDamagePercent= 3.6M,

//}, new StaticItem {
//         dbName = "animal_Brainwashed_Bitch_LexamTheGemFox",
//         FriendlyName = "Brainwashed Bitch",
//         PortraitUrl = "brainwashed_bitch_LexamTheGemFox.png",
//         Description = "This player has been defeated by Donna Milton and thus has been turned into a mindless doggy.  Her body still contains traces of her transformer's magic inside her body.  Her pussy is puffed with need and her paws are useless in pleasuring herself. Her heat and lust has caused her mind to snap and now she is nothing but a doggy in need of a bone and an owner to care for her. She helplessly crawls behind her master or mistress, her tail flagged to show everyone that she challenged the great Donna Milton and lost, becoming nothing more than a slutty beast that lives only to pleasure others.",
//         ItemType = PvPStatics.ItemType_Pet,
//         Findable = false,
//        HealthBonusPercent = 2,
//        ManaBonusPercent = 2,
//        SpellExtraTFEnergyPercent= 2.5M,
//        SpellExtraHealthDamagePercent= 2.5M,
//}
     
            
//        };



//            }
      //  }

    }

}