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
    public static class FormStatics
    {

//        public static IEnumerable<Form> GetForm2
//        {

//            get
//            {
//                return new[]
//                {

//#region starting out forms

//             new Form {
//                dbName = "man_01",
//                FriendlyName = "Regular Guy",
//                Description = "A normal-looking guy, 25 years old.  But he's also a young, eager witch, inexperienced but determined to rise above his peers and become the most powerful sorcerer in the world.",
//                Gender = "male",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Man_01_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//             new Form {
//                dbName = "man_02",
//                FriendlyName = "Regular Guy",
//                Description = "A normal-looking guy, 25 years old.  But he's also a young, eager witch, inexperienced but determined to rise above his peers and become the most powerful sorcerer in the world.",
//                Gender = "male",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Man_02_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//             new Form {
//                dbName = "man_03",
//                FriendlyName = "Regular Guy",
//                Description = "A normal-looking guy, 25 years old.  But he's also a young, eager witch, inexperienced but determined to rise above his peers and become the most powerful sorcerer in the world.",
//                Gender = "male",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Man_03_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//             new Form {
//                dbName = "man_04",
//                FriendlyName = "Regular Guy",
//                Description = "A normal-looking guy, 25 years old.  But he's also a young, eager witch, inexperienced but determined to rise above his peers and become the most powerful sorcerer in the world.",
//                Gender = "male",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Man_04_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//             new Form {
//                dbName = "man_05",
//                FriendlyName = "Regular Guy",
//                Description = "A normal-looking guy, 25 years old.  But he's also a young, eager witch, inexperienced but determined to rise above his peers and become the most powerful sorcerer in the world.",
//                Gender = "male",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Man_05_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },
            
//            new Form {
//                dbName = "woman_01",
//                FriendlyName = "Regular Girl",
//                Description = "A normal-looking girl, 25 years old.  But she's also a young, eager witch, inexperienced but determined to rise above her peers and become the most powerful sorceress in the world.",
//                Gender = "female",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Woman_01_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//              new Form {
//                dbName = "woman_02",
//                FriendlyName = "Regular Girl",
//                Description = "A normal-looking girl, 25 years old.  But she's also a young, eager witch, inexperienced but determined to rise above her peers and become the most powerful sorceress in the world.",
//                Gender = "female",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Woman_02_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//              new Form {
//                dbName = "woman_03",
//                FriendlyName = "Regular Girl",
//                Description = "A normal-looking girl, 25 years old.  But she's also a young, eager witch, inexperienced but determined to rise above her peers and become the most powerful sorceress in the world.",
//                Gender = "female",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Woman_03_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//              new Form {
//                dbName = "woman_04",
//                FriendlyName = "Regular Girl",
//                Description = "A normal-looking girl, 25 years old.  But she's also a young, eager witch, inexperienced but determined to rise above her peers and become the most powerful sorceress in the world.",
//                Gender = "female",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Woman_04_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//              new Form {
//                dbName = "woman_05",
//                FriendlyName = "Regular Girl",
//                Description = "A normal-looking girl, 25 years old.  But she's also a young, eager witch, inexperienced but determined to rise above her peers and become the most powerful sorceress in the world.",
//                Gender = "female",
//                TFEnergyRequired = 9999,
//                MobilityType = "full",
//                PortraitUrl = "Woman_05_Meddle.png",
//                FormBuffs = new BuffBox{},
//            },

//            new Form {
//                dbName = "female_bartender",
//                FriendlyName = "Cute bartender",
//                Description = "a female bartender, 25 years old with black hair and a lean body",
//                Gender = "female",
//                TFEnergyRequired = 30,
//                MobilityType = "full",
//                PortraitUrl = "",
//                FormBuffs = new BuffBox{},

//#endregion

//#region bot forms


//        }, new Form {
//                dbName = "botform_psychopathic_spellslinger_male",
//                FriendlyName = "Psychopathic Spellslinger",
//                Description = "This mage can best be described as a psychopath or perhaps simply insane.  He cares nothing for friends or alliances and has no regard to who or what he attacks, no matter the opposition.  It's all or nothing for this spellslinger, and while he can be very dangerous on his own he is also relatively easy to avoid or take down with a friend or two.",
//                Gender = "male",
//                TFEnergyRequired = 30,
//                MobilityType = "full",
//                PortraitUrl = "psycho_male.png",
//                FormBuffs = new BuffBox{

//                },

//       }, new Form {
//                dbName = "botform_psychopathic_spellslinger_female",
//                FriendlyName = "Psychopathic Spellslinger",
//                Description = "This mage can best be described as a psychopath or perhaps simply insane.  She cares nothing for friends or alliances and has no regard to who or what she attacks, no matter the opposition.  It's all or nothing for this spellslinger, and while she can be very dangerous on her own she is also relatively easy to avoid or take down with a friend or two.",
//                Gender = "male",
//                TFEnergyRequired = 30,
//                MobilityType = "full",
//                PortraitUrl = "psycho_female.png",
//                FormBuffs = new BuffBox{

//                },

//         }, new Form {
//                dbName = "botform_clothes_merchant",
//                FriendlyName = "Soul Item Vendor",
//                Description = "This clever witch appears as a plain human pushing around a wooden cart filled with clothes of all sizes and shapes.  Despite her ordinary looks she found a way to make a nice profit out of transformation-related combat--she makes her living by buying and selling souls who have been trapped as inanimate articles of clothing to new masters for a decent profit.  These so-called 'Soul Items' are often in high demand as they often feel extra warm, cool, or arousing when worn, something that no regular pieces of fabric can do.  Not wanting to miss out on other financial opportunities, she also carries other more mundane items.  She'll gladly trade with you, but beware that you don't attack her or she may reveal how much power she keeps concealed.",
//                Gender = "female",
//                TFEnergyRequired = 500,
//                MobilityType = "full",
//                PortraitUrl = "Lindella_Luxianne.png",
//                FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 1000,
//                    FromForm_ManaBonusPercent = 1000,
//                    FromForm_SpellExtraHealthDamagePercent = 500,
//                    FromForm_SpellExtraTFEnergyPercent = 250,
//                    FromForm_HealthRecoveryPerUpdate = 25,
//                    FromForm_ManaRecoveryPerUpdate = 25,
//                    FromForm_CleanseExtraHealth=100,
//                    FromForm_MeditationExtraMana = 100,
//                    FromForm_SpellTFEnergyDamageResistance = 75,
//                },

//           }, new Form {
//                dbName = "form_Donna_LexamTheGemFox",
//                FriendlyName = "Mythical Sorceress",
//                Description = "Before you is a woman garbed in green with a deep magic eminating from her person. She smile and introduces herself as Donna Milton, the true owner of the Milton Ranch. She is the stuff of legends and even rumored to come from stories from ancient mythology, surviving and propsering to this day with centuries of accumulated wisdom and power. With a smile she stares back at you, here eyes glimmering at all the possible animals to change you into.",
//                Gender = "female",
//                TFEnergyRequired = 500,
//                MobilityType = "full",
//                PortraitUrl = "Donna_Wrenzephyr.png",
//                FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 750,
//                    FromForm_ManaBonusPercent = 500,
//                   // FromForm_HealthRecoveryPerUpdate = 25,
//                   // FromForm_ManaRecoveryPerUpdate = 25,
//                    FromForm_ExtraSkillCriticalPercent = 25,
//                    FromForm_SpellExtraHealthDamagePercent = 100,
//                    FromForm_SpellExtraTFEnergyPercent = 100,
//                    FromForm_EvasionPercent = 33,
//                    FromForm_MeditationExtraMana = 250,
//                    FromForm_CleanseExtraHealth = 25,
//                    FromForm_ExtraInventorySpace = 24,
//                },



//#endregion

//            }, new Form {
//                dbName = "marble_statue",
//                FriendlyName = "Glossy Marble Statuette",
//                Description = "You are a marble statuette in the shape of a voluptuous young woman, extremely lifelike in its pose and detail.  That's because you actually were a person some time ago, though what it feels like to have been able to move your limbs, breathe, talk out loud are fading like distant memories.  Not that you particularly miss them--you inwardly shutter in eroticism whenever you feel human fingers wrapped around your cool marble body.  Forever beautiful.",
//                Gender = "female",
//                TFEnergyRequired = 50,
//                MobilityType = "inanimate",
//                BecomesItemDbName = "marble_statue",
//                PortraitUrl = "",
//                FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "For a moment your fingers feel cold and numb, your fingernails smooth yet doubly heavy.  You manage to resist the spell and reverse the subtle changes.",
//                TFMessage_40_Percent_1st = "You stumble a bit as your feet grow numb, a cold stiffness that creeps up your shins.  Luckily you are able to stop the changes.",
//                TFMessage_60_Percent_1st = "You gasp as your feet grow numb, a cold stifness that creeps up your shins and to your waist.  Luckily, you are able to reverse this and continue on.",
//                TFMessage_80_Percent_1st = "You gasp as your feet grow numb, a cold stifness that creeps up your shins and up to your shoulders.  Luckily, you are able to reverse this and continue on.",
//                TFMessage_100_Percent_1st = "Your entire body turns stiff, your skin smooth and glossy with a crystalline, marble texture replacing pink flesh.  You are barely able to undo these effects, but next you the energy built up inside you may overwhelm your resistance...",
//                TFMessage_Completed_1st = "You inhale one last time before your entire body stiffens, locked into a sexy pose as a wave of magic replaces your skin with smooth, glossy marble.  Your muscles fight to the last, but when the last of your body has changed you can do nothing but watch, forever sealed into your new form as a marble statue.",
                        
//                TFMessage_20_Percent_3rd = "Your target glances at their fingers, the tips a glossy gray where stone has replaced flesh.  However, they are able to resist your magic and their fingers change back to before.",
//                TFMessage_40_Percent_3rd = "Your target stumble a bit as the marble texture creeps up along their toes up to their shins.  However, they are able to resist your magic and their body changes back to before.",
//                TFMessage_60_Percent_3rd = "Your target gasps as the marble texture flows from their toes all the way up to their waist.  However, they are able to resist your magic and their body changes back to before.",
//                TFMessage_80_Percent_3rd = "Your target gasps as the marble texture flows from their toes all the way up to their shoulders.  However, they are able to resist your magic and their body changes back to before.",
//                TFMessage_100_Percent_3rd = "Your target's entire body moves stiffly, skin replaced by marble, locked into a sexy pose.  Just barely are they able to break free and reverse their transformation, but who knows if they can do it again...",
//                TFMessage_Completed_3rd = "Your target inhales one last time before their entire body stiffens, locked into a sexy pose as a wave of magic replaces their skin with smooth, glossy marble.  Their last remnants of independent motion cease, their body locked into that of a marble statue forever.",
           
//            //}, new Form {
//            //     dbName = "panty_plain_1",
//            //    FriendlyName = "Plain Pink Panties",
//            //    Description = "You are a pair of plain pink panties made out of a smooth, silky fabric.  Your design is plain, designed for comfort instead of sexiness.  When you are being worn you feel your owner's crotch up against you, sweet sweat and the occasional sexual fluid--not always belonging to your wearer!--delicious against your taste buds.",
//            //    Gender = "female",
//            //    TFEnergyRequired = 80,
//            //    MobilityType = "inanimate",
//            //    BecomesItemDbName = "panty_plain_1",
//            //    PortraitUrl = "",
//            //    FormBuffs = new BuffBox{},

//            }, new Form {
//         dbName = "form_Plain_Pink_Panties_Judoo",
//         FriendlyName = "Plain Pink Panties",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Plain_Pink_Panties_Judoo",
//         FormBuffs = new BuffBox{}


//            }, new Form {
//                 dbName = "form_busty_blonde_bimbo",
//                FriendlyName = "Busty Blonde Bimbo",
//                Description = "A sexy blonde woman sporting at least D-cups, a wasplike waist with hips and ass that catches one's eye regardless of their sexual preferences.  Hips sway and breasts bounce with every small step and a stream of fragrance lingers behind, enticing others to follow.",
//                Gender = "female",
//                TFEnergyRequired = 50,
//                MobilityType = "full",
//                PortraitUrl = "form_busty_blonde_bimbo_nymic-tf.jpg",
//                FormBuffs = new BuffBox {
//                    FromForm_ManaBonusPercent = -4,
//                    FromForm_HealthBonusPercent = -4,
//                    FromForm_SpellExtraHealthDamagePercent = 7,
//                },
               
//            }, new Form {
//                 dbName = "form_bunny_slippers",
//                FriendlyName = "Fuzzy Pink Bunny Slippers",
//                Description = "A pair of fuzzy pink bunny slippers, extremely adorable and even more so comfortable to wear.  Walking in these keep your feet quite warm, leaving your feetsies warm and nimble.  Plus they don't make much noise when they hit the ground.",
//                Gender = "female",
//                TFEnergyRequired = 65,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_bunny_slippers",
//                 FormBuffs = new BuffBox{},

//#region skateboard
//            }, new Form {
//                 dbName = "form_skateboard",
//                FriendlyName = "Sleek, Fiery Skateboard",
//                Description = "A wooden skateboard absolutely plastered in flame decals, sleek black wheels perfectly oiled for maximum speed, paint fresh and unscratched--until its owner gets some good rail grinds in, anyway.",
//                Gender = "male",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_skateboard",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You feel a slight pressure pushing against your chest and back as if you were being wedged between two surfaces.  The feeling is disconcerting and you wonder if the attack is some kind of flattening spell. You are able to reverse the effects with no difficulty.",
//                TFMessage_40_Percent_1st = "You feel a slight pressure pushing against your chest and back as if you were being wedged between two surfaces.  This time the pressure is a lot stronger and you feel your bones and muscles start to reshape in the process.  In addition, your fingertips begin to turn glossy and black.  You reverse the changes before your body transforms any farther.",
//                TFMessage_60_Percent_1st = "The flattening feeling returns, stronger thane ever before.  But you also begin to shrink vertically and find your arms and legs locking up, squeezing against your sides as if joining into one large, flat mass.  Your fingers turn black again and begin to fuse together, the very tips rounding out into wheel-like shapes.  You are able to reverse the changes with some difficulty.",
//                TFMessage_80_Percent_1st = "Your body compresses sharply, your armgs merging into your sides and your legs together into one flat mass.  Your entire body grows stiff, skin turning to finely sanded wood, shrinking down to only three feet tall.  You are turning into some kind of board with small black wheels at your fingers and knees--a skateboard, probably.  Luckily, you are able to reverse the changes but it takes a considerable amount of effort.",
//                TFMessage_100_Percent_1st = "Once again your arms merge against your sides, your legs and torso reshaping into a single, oval, flat board.  Your neck crunches down to nothing and your vision starts to fade as your face dissolves into the wooden board that your body has become.  Four black, plastic wheels jut out of you and you topple forward, your skateboard-shaped body rolling several feet from the momentum of the fall.  Flame-shaped stickers and decals pop out all over your skin.  With every last ounce of effort you can must muster, you are finally able to undo the transformation and restore your old body, but the activity leaves you feeling drained.  You won't be able to resist much longer...",
//                TFMessage_Completed_1st = "On last time your arms merge against your sides, your legs and torso reshaping into a single, oval, flat wooden board.  Your neck crunches down to nothing and your vision starts to fade as your face dissolves into the wooden board that your body has become.  Four black, plastic wheels jut out of you and you topple forward, your skateboard-shaped body rolling several feet from the momentum of the fall.  Flame-shaped stickers and decals pop out all over your skin.  You can't fight it anymore.  You feel a weight press aginst your back as your attacker steps on top of you, riding you around in a few circles triumphantly.  You love the feel of the ground against your wheels, the massaging vibrations traveling through your body as you roll, your paint and flame-themed decals sporty.  You could really enjoy a full life like this... which you will.",
                        
//                TFMessage_20_Percent_3rd = "Your target pats their chest, responding to an invisible pressure pushing against their chest and back as if you they were being wedged between two surfaces.  Your target is able to reverse the effects with no difficulty.",
//                TFMessage_40_Percent_3rd = "Your target gasps as an invisible pressure pushes against their chest and back as if they were being wedged between two surfaces.  This time the pressure seems to be a lot stronger and you watch as your target's bones and muscles start to reshape to fit the flattening..  In addition, their fingertips begin to turn glossy and black.  They are able to reverse the changes before their body transforms any farther.",
//                TFMessage_60_Percent_3rd = "Your target's invisible flattening force returns, stronger than ever before.  Your target also begins to shrink vertically and flail a little until they find that their  arms and legs locking up, squeezing against their  sides as if joining into one large, flat mass.  Your target's fingers turn black again and begin to fuse together, the very tips rounding out into wheel-like shapes.  They are able to reverse the changes with some difficulty.",
//                TFMessage_80_Percent_3rd = "Your target's body compresses greatly, their armgs merging into their sides and their legs into one another forming one flat mass.  Their entire body grows stiff, skin turning to finely sanded wood, shrinking down to only three feet tall.  Your target is clearly turning into a skateboard with small black wheels at their former fingers and knees.  Luckily for your target, they are able to reverse the changes but it takes a considerable amount of effort.",
//                TFMessage_100_Percent_3rd = "Once again your target's arms merge against their sides, legs and torso reshaping into a single, oval, flat wooden board.  Their face vanishes into the wood as their head and neck flatten and reshape into the board's front end.  Four black, plastic wheels jut out of your target and they  topple forward, their  skateboard-shaped body rolling several feet from the momentum of the fall.  Flame-shaped stickers and decals pop out all over their skin.  With every last ounce of effort your target can must muster, they are finally able to undo the transformation and restore their old body, but the activity leaves them looking absolutely drained.  They won't be able to resist much longer...",
//                TFMessage_Completed_3rd = "Once final time your target's arms merge against their sides, legs and torso reshaping into a single, oval, flat wooden board.  Their face vanishes into the wood as their head and neck flatten and reshape into the board's front end.  Four black, plastic wheels jut out of your opponent and they topple forward, their skateboard-shaped body rolling several feet from the momentum of the fall.  Flame-shaped stickers and decals pop out all over their skin.  You watch, waiting to see if your target can change back.  Nothing happens. You take a step on the board and ride it around in a few circles, admiring its sleekness and flame-themed sportiness.  You could swear that the skateboard is vibrating in ecstasy as it bumps against the ground, rearing for more.",

//#endregion

//#region blue jeans
//             }, new Form {
//                 dbName = "form_tight_blue_jeans",
//                FriendlyName = "Loyal Hip-Hugging Blue Jeans",
//                Description = "",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_tight_blue_jeans",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You feel your opponent land their spell squarely on your crotch.  You feel a slight tingle that spreads away from your genitals, your skin turning slightly rough with a blue tint.  You reverse the changes with little difficulty.",
//                TFMessage_40_Percent_1st = "Your opponent's spell once again strikes you squarely on the crotch.  This time the changes are a little more dramatic--you feel your legs begin to weaken as a blue weave begins to replace your skin, shooting down from your sex nearly down to your ankles.  Your torso begins to shrink a bit as well, your waist condensing a couple inches, before you manage to resist the spell.",
//                TFMessage_60_Percent_1st = "Another spell strikes you at your waist.  This time you fall flat on your ass as the skin on your legs is replaced by the weave of blue jeans.  You feel your feet and ankles begin to hollow and watching as your cloth-legs flatten under the force of gravity, you legs vanishing nearly up to your knees.  Simultaneously your spine shrinks six inches and a silver button grows out of your skin a bit below your belly button.  With some difficulty, you manage to reverse the spell.",
//                TFMessage_80_Percent_1st = "You are struck again, falling to the ground nearly instantly as the skin of your legs turn into a dark blue jeans weave.  Your feet vanish entirely, your bones and muscles vanishing into nothing, leaving behind only the skin as fabric settling to the ground.  Your torso contracts, a silver button and zipper poking through your body below your belly button.  You have jeans for legs and a short, stumpy mineature torso sinking into your own hipline when finally you are able to reverse the spell and return to before, but the effort is starting to grow exhausting...",
//                TFMessage_100_Percent_1st = "You shiver as another spell strikes you.  Within moments your legs have turned into hollow blue jeans draped across the ground, complete with a silver button and zipper.  You watch, fascinated and horrified as pink threads begin to lace their way through the seams, creating flowery embroderies in the fabric that was once your skin.  You vision darkens, your torso being absorbed into your hipline.  You scratch at the floor around you, trying to resist your shrinking.  Only at the last second with an almost miraculous burst of energy are you able to reverse the changes and return to your old body.  But once more, twice if you are lucky, there's no way you'll be able to fight it off again...",
//                TFMessage_Completed_1st = "You shiver as another spell strikes you.  Within moments your legs have turned into hollow blue jeans draped across the ground, complete with a silver button and zipper.  You watch, fascinated and horrified as pink threads begin to lace their way through the seams, creating flowery embroderies in the fabric that was once your skin.  Your vision darkens, your torso being absorbed into your hipline.  You scratch at the floor around you, trying to resist your shrinking.  You try to muster the energy to resist, but by now the cause is hopelss.  You feel the last of your arms, neck, and head pull into your hips.  Soon they too hollow out, leaving you a pair of crumpled, feminine blue jeans.  You feel your attacker pick you up and examine you, chucking trimphantly.  Their touch feels magical and you start to crave the sensation of being wrapped around their entire lower body...",
                        
//                TFMessage_20_Percent_3rd = "You land your spell squarely on your target's crotch.  Embarassed and a little confused, they slip a finger down there to investigate as a tiny hint of blue begins to spread from the point of impact.  They reverse the changes with little difficulty.",
//                TFMessage_40_Percent_3rd = "Your spell once again strikes your target squarely on the crotch.  This time the changes are a little more dramatic--your target stumbles a bit as their legs begin to weaken, a blue weave beginning to replace to replace your skin, shooting down from their sex nearly down to their ankles.  Their torso begins to shrink a bit as well, their waist condensing a couple inches, before they manage to resist the spell and change back.",
//                TFMessage_60_Percent_3rd = "Another spell strikes your opponent around the waist.  This time your target falls flat on their ass as the skin on their legs is replaced by the weave of blue jeans.  You watch as your opponent's feet and ankles begin to hollow and watch as their cloth-legs flatten under the force of gravity, legs vanishing nearly up to the knees.  Simultaneously their spine shrinks six inches and a silver button grows out of your target's skin a bit below their belly button.  With some difficulty, your opponent manages to reverse the spell.",
//                TFMessage_80_Percent_3rd = "You strike your target again.  They fall to the ground nearly instantly as the skin of their legs turn into a dark blue jeans weave.  Their feet vanish entirely, bones and muscles vanishing into nothing, leaving behind only the skin as fabric settling to the ground.  Your target's torso contracts, a silver button and zipper poking through their body below the belly button.  They soon have jeans for legs and a short, stumpy mineature torso sinking into their own hipline when they are able to reverse the spell and return to before.  The encounter leaves them looking rather drained, however.",
//                TFMessage_100_Percent_3rd = "Your target shiver as another spell strikes them.  Within moments their legs have turned into hollow blue jeans draped across the ground, complete with a silver button and zipper.  You watch, fascinated and horrified as pink threads begin to lace their way through the seams, creating flowery embroderies in the fabric that was once your opponent's skin.  As the transformation reaches into its final stages, your opponent's torso has been all but absorbed into their hipline.  They scratch at the floor around you with what's left of their arms, trying to resist the shrinking.  Only at the last second are they able to reverse the changes and return to their old body.  But one more attack, two if they are lucky, is sure to leave them transformed into a pair of blue jeans for good.",
//                TFMessage_Completed_3rd = "Your target shiver as another spell strikes them.  Within moments their legs have turned into hollow blue jeans draped across the ground, complete with a silver button and zipper.  You watch, fascinated and horrified as pink threads begin to lace their way through the seams, creating flowery embroderies in the fabric that was once your opponent's skin.  As the transformation reaches into its final stages, your opponent's torso has been all but absorbed into their hipline.  They scratch at the floor around you with what's left of their arms, trying to resist the shrinking.  Despite your target's final flailing, they have no energy to resist anymore.  Soon there is no trace of their former humanity, a pair of feminine, tight blue jeans lying crumpled on the ground in their place.  You pick them up, feeling the human-fabric tingle in excitement at your touch, mentally succombed to the changes, desiring nothing more to be worn...",

//#endregion

//#region pink dress jbovinne

//             }, new Form {
//                 dbName = "form_dress_jbovinne",
//                FriendlyName = "Fashionable Pink Dress",
//                Description = "",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "dress_jbovinne.png",
//                 BecomesItemDbName = "item_dress_jbovinne",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "A cloud of pink gas appears around you. It is magical, but you dissipate it with a wave of your hand.",
//                TFMessage_40_Percent_1st = "Another cloud of pink gas appears around you, swirling with mystical energy. You have to concentrate, but you again wave it away. Your hand felt funny, though.",
//                TFMessage_60_Percent_1st = "The cloud of pink smoke returns. You wave at it and stare in horror as your hand begins to evaporate. It has mostly joined the cloud when you get control and return it to normal.",
//                TFMessage_80_Percent_1st = "You feel a slight tingle before your entire body starts evaporating. Your legs lose their strength and turn to pink gas, sending you falling backwards. You watch as your legs disappear with the wind. You start to collect your energy to restore yourself when a pleasant tightening occurs across your arms and torso. You just start to see your skin change to pink when you return yourself to normal. ",
//                TFMessage_100_Percent_1st = "A spell hits you before you have the chance to duck, knocking you back. You try to correct your fall, but realize you have lost the strength in your limbs. You just have time to see your hands and legs blow away before even your eyes start changing to pink smoke. Your body is drifting away from you, leaving you with an incredible feeling of tightness across your arms and torso. You barely hold onto your identity as the rest of your body turns to gas, but find enough of ‘you’ left to slowly rebuild. It takes time, but you eventually coalesce into a human form. You think it resembles what you used to look like.",
//                TFMessage_Completed_1st = "The spell hits you, causing a shiver to run up your body. The effect is not as dramatic as previous hits, and you open your mouth to laugh. As gas starts billowing out of your mouth, you realize that the spell has already fully converted your insides to smoke. You try to take a step, but your muscles, bones, everything inside you is just gas. It only takes a moment for the rest of you to follow. What is left of you tightens into a beautiful pink dress, your dissipating body creating wondrous patterns across it as it tries to hold itself together. This time there is not enough of ‘you’ left to return, only the dress you have become. You hope your sense of time blew away… forever is a long time.",
                        
//                TFMessage_20_Percent_3rd = "Your spell creates a cloud of pink energy around your target. They wave at it and it dissipates. ",
//                TFMessage_40_Percent_3rd = "Again your spell creates a cloud of pink energy around your target. This time it is noticeably harder for them to remove. And did something happen to their skin?",
//                TFMessage_60_Percent_3rd = "Your spell creates a cloud of pink energy that swirls and confuses your target. They wave at it, trying to disperse it, and you notice their hands start to evaporate. They stare in shock before they are able to reverse the changes and return it to a solid form.",
//                TFMessage_80_Percent_3rd = "Your opponent looks shocked for a second before their body starts turning to smoke. They struggle to control it as their legs collapse under them and then simply blow away. Just before they regain control of their body, you notice that the skin around their torso was still noticeably solid. And pink.",
//                TFMessage_100_Percent_3rd = "Your spell knocks your target back as if they weighed next to nothing. Their body turns to smoke before your eyes, their legs and hands blowing away with the wind. The skin around their torso and arms solidifies, forming a pink material. The smoke seems to create patterns across it as their body swirls and flows out of the sleeves, neck, and opening at the base. They have very clearly become a beautiful pink dress, when they very slowly manage to piece themselves back together again. You can’t believe it as they return to their human form.",
//                TFMessage_Completed_3rd = "Your target looks at you, their mouth moving as if they were trying to speak. Only pink smoke emerges. Their body seems to lose all strength as they evaporate on the spot. The skin of their torso and hands tightens and solidifies into their new form; the rest of their body simply drifts away. Within seconds they no longer have enough of themselves left to stay upright, and the new pink dress floats to the ground. You watch as the smoke blows away, certain that this time they won’t be coming back.",

//#endregion

//#region latex legs jbovinne
  

//              }, new Form {
//                 dbName = "form_latex_legs_jbovinne",
//                FriendlyName = "Black Latex Pants",
//                Description = "",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_black_latex_pants_jbovinne",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "The spell hits you in the chest, but you dispel it quickly. Your skin seems slightly smoother than usual, though.",
//                TFMessage_40_Percent_1st = "Another hit and another burst of energy through you. This one doesn’t seem to do much except darken your skin a littler. Woo! Free tan!",
//                TFMessage_60_Percent_1st = "As the energy of the spell starts working you a spot of black appears on your neck. You rub it and feel how smooth and shiny your skin has become. More patches are appearing on your arms and face. With some effort you gather the energy to stop them.",
//                TFMessage_80_Percent_1st = "This time your skin rapidly changes. Black and shiny, you realize your skin is become latex. You start to gather the energy to stop the changes when something pushes through your lip, sealing your mouth for a moment. You feel it with hands that are barely recognisable and find a metal button. Opening it you muster enough will to slowly return to normal. ",
//                TFMessage_100_Percent_1st = "Metal buttons push through your lips, sealing your mouth and pulling it together into a vertical line. Your vision blurs, then disappears as your eyes change into similar buttons and start pulling towards each other. You can’t see, but you can feel as your body folds in on itself, sliding over your new shiny skin. It’s with a herculean effort that you manage to find enough willpower to return to normal. You stand up, visibly shaken.",
//                TFMessage_Completed_1st = "The spell hits you and you realize you’re done. You look down as your skin changes to black latex, any clothes you are wearing simply sliding off. Your arms flow into your sides as you collapse down, your legs losing the strength to hold you up without being worn. You simply don’t have the will to stop this. Again you feel your eyes change to metal as two more buttons push out from behind your lips. This time they have no resistance as they pull your face together before even that flows away. You lie on the ground, now simply a pair of latex pants, waiting to be added to someone’s wardrobe. ",
                        
//                TFMessage_20_Percent_3rd = "Your spell successfully hits your target. They brush it off without a fuss, but the light seems to reflect off their skin a little more. ",
//                TFMessage_40_Percent_3rd = "Your spell hits your opponent on the arm. They don’t seem to notice much change, but you saw the black patches start to appear on their face. You smile and prepare another spell.",
//                TFMessage_60_Percent_3rd = "The spell hits your target and flows across them. Black patches appear on their skin and you realize they are starting to transform. Disappointingly, they stop and revert to normal.",
//                TFMessage_80_Percent_3rd = "This time your spell’s energy noticeably affects them. Their skin changes to latex in waves, tightening and pulling their body out of shape. Just before they regain control of their body you see a metal button push out from their mouth, sealing it shut. They then revert to normal. ",
//                TFMessage_100_Percent_3rd = "Your spell hits your target square in the face. They have so much built up energy within them that the change is almost instantaneous. Their eyes change to buttons, and their mouth seals. They silently fumble about their head as their arms start to flow into their body. You smile, knowing that they will finish transforming soon. Miraculously they manage to stop the transformation. As they slowly stand back up you ready another spell. ",
//                TFMessage_Completed_3rd = "This time it’s permanent. Your target doesn’t even bother fighting the change as their clothes slide off their new shiny black skin. Their arms flow into their sides as their body collapses to the ground. You walk towards them as their eyes cloud over and become metal buttons. You can’t recognise them at all as two more buttons seal their mouth and pull into a vertical line. Within moments there is nothing left except a new pair of shiny black latex pants. They look just your size. ",

//#endregion
       
//#region impossibly bb lady
//               }, new Form {
//                 dbName = "form_bigger_bust",
//                FriendlyName = "Impossibly Big-Breasted Lady",
//                Description = "An extremely well-endowed woman in the chest department.  Almost implausibly so.  Bearing mamarries beyond ordinary measure, this woman has considerable difficulty moving about and won't be able to move between locations without using twice as much energy.  At least on the plus side, her oversized breasts have hyperactive magical milk glands which are able to restore some willpower over time.",
//                Gender = "female",
//                TFEnergyRequired = 95,
//                MobilityType = "full",
//                PortraitUrl = "ibbl.jpg",
//                 FormBuffs = new BuffBox{
//                     FromForm_MoveActionPointDiscount = -.6M,
//                     FromForm_HealthRecoveryPerUpdate = 2.6M,
//                 },

//                TFMessage_20_Percent_1st = "You feel the spell hit you right at the sternum.  Your nipples tingle and begin to swell, but you are able to reverse the effects without any difficulty.",
//                TFMessage_40_Percent_1st = "Your opponent's spell slams right between your breasts.  You stagger, feeling your nipples through your shirt as your breasts inflate a couple cup sizes, each nipple swelling to the size of a marble.  You are able to reverse the effects, but when you draw your fingers away you feel a small trickle running down your abs. ",
//                TFMessage_60_Percent_1st = "Your adversary launches the spell at you twice in quick succession.  One lands at your belly button and you feel your body first shift to an androgynous shape.  Then, in sharp contrast, your hips crack wider and you feel your ass puff up.  The second spell hits you on the neck and almost immediately you find yourself staring down at a pair of DD-cup tits straining underneath your top.  You mash your fleshy mounds together, trying to squeeze them smaller as you focus your magic to dispell the effects on your body.  You manage to do so, but it's definitely becoming harder. ",
//                TFMessage_80_Percent_1st = "Your adversary launches the spell at you one, two, three times.  All hit you directly.  Their first lands at your belly button and you feel your body first shift to an androgynous shape.  Then, in sharp contrast, your hips crack wider and you feel your ass puff up.  The second spell hits you on the neck and almost immediately you find yourself staring down at a pair of DD-cup tits straining underneath your top.  You mash your fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  You feel the weight of them pulling you down.  The third spell hits your feet and you collapse on your padded feminine ass, breasts lurching like gelatin bowling balls.  Your thinner, feminine arms have a hard time pushing you back up, but you muster the fortitude to dispell and reverse your opponent's magic before the transformation can finished.",
//                TFMessage_100_Percent_1st = "Your adversary launches the spell at you one, two, three times.  All hit you directly.  Their first lands at your belly button and you feel your body first shift to an androgynous shape.  Then, in sharp contrast, your hips crack wider and you feel your ass puff up.  The second spell hits you on the neck and almost immediately you find yourself staring down at a pair of DD-cup tits straining underneath your top.  You mash your fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  You feel the weight of them pulling you down.  The third spell hits your feet and you collapse on your padded feminine ass, breasts lurching like gelatin bowling balls.  Even then they keep on growing, the weight of them pinning you down.  Your thinner, feminine arms try but fail to push you back up to your feet.  There you are, an impossibly large breasted woman helpless on the ground, your opponent peering down at you.  With every last ounce of willpower you shrink your boobs back to a size where you can pull yourself to your feet and reverse the rest of the changes, but any more and you may find yourself stuck this way for a while.",
//                TFMessage_Completed_1st = "Your adversary launches the spell at you one, two, three times.  All hit you directly.  Their first lands at your belly button and you feel your body first shift to an androgynous shape.  Then, in sharp contrast, your hips crack wider and you feel your ass puff up.  The second spell hits you on the neck and almost immediately you find yourself staring down at a pair of DD-cup tits straining underneath your top.  You mash your fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  You feel the weight of them pulling you down.  The third spell hits your feet and you collapse on your padded feminine ass, breasts lurching like gelatin bowling balls.  Even then they keep on growing, the weight of them pinning you down.  Your thinner, feminine arms try but fail to push you back up to your feet.  There you are, an impossibly large breasted woman helpless on the ground, your opponent peering down at you.  With every last ounce of willpower you shrink your boobs back to a size where you can pull yourself to your feet and reverse the rest of the changes, but halfway through your energy dwindles down.  Sweating, you sink back to the ground, panting as your tits return to their impossible, burdensome size.  Worse, with every heave a trickle of milk starts to flow out of your nipples, streaming down your narrow waist and pooling around your crotch.  Your attacker grins and finally eases off, leaving you to your own devices.  Safe for the time being, you learn how to wiggle your way back to your feet even if every step takes the effort of several.  The milk that squirts out has a way of refreshing you, restoring a bit of willpower... at least one good thing out of all of this.",
                        
//                TFMessage_20_Percent_3rd = "Your spell hits your target right on the sternum.  Their nipples begin to swell, poking through their top, but they are able to reverse the effects without any difficulty.",
//                TFMessage_40_Percent_3rd = "Your spell slams into your opponent right between their breasts.  They stagger, feeling up their chest as their breasts inflate a couple cup sizes, each nipple swelling to the size of a marble.  They are able to reverse the effects, but you swear that there is a damp spot on their top right at their nips.",
//                TFMessage_60_Percent_3rd = "You launch the spell at your opponent twice in quick succession.  One lands at their belly button and their body reshapes into a fairly androgynous form.  Then, in sharp contrast, their hips crack wider and you watch as  your target's ass puff up.  The second spell hits your target on the neck and almost immediately they find themself staring down at a pair of DD-cup tits straining underneath their top.  They mash their fleshy mounds together, trying to squeeze them smaller as they focus their magic to dispell the effects on their body.  Your target manages to do so, but it's definitely becoming harder. ",
//                TFMessage_80_Percent_3rd = "You launch the spell at your opponent one, two, three times.  All hit your target directly.  Your first spell lands at their belly button and they shiver as their body first shifts to an androgynous shape.  Then, in sharp contrast, their hips crack wider and you watch as their ass puffs up.  The second spell hits your target on the neck and almost immediately they find themself staring down at a pair of DD-cup tits straining underneath their top.  They mash their fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  Your target sways as they feel the weight of them pulling them down.  The third spell hits their feet and they collapse on their padded feminine ass, breasts lurching like gelatin bowling balls.  Your adversary's thinner, feminine arms have a hard time pushing them back up, but they manage to muster the fortitude to dispell and reverse your magic before the transformation can finished.",
//                TFMessage_100_Percent_3rd = "You launch the spell at your target one, two, three times.  All hit your target directly.  Your first spell lands at their belly button and they shiver as their body first shifts to an androgynous shape.  Then, in sharp contrast, their hips crack wider and you watch as their ass puffs up.  The second spell hits your target on the neck and almost immediately they find themself staring down at a pair of DD-cup tits straining underneath their top.  They mash their fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  Your target sways as they feel the weight of them pulling them down.  The third spell hits their feet and they collapse on their padded feminine ass, breasts lurching like gelatin bowling balls.  Their thinner, feminine arms try but fail to prop them back up on their feet.  There your target sits, an impossibly large breasted woman lying helpless on the ground.  You peer down at them, watching, waiting.  With what must be every last ounce of willpower, your target does manage to shrink their boobs back to a size where they can push themselves to their feet and reverse the rest of the changes, but any more and you are sure they will be stuck this way for some time.",
//                TFMessage_Completed_3rd = "You launch the spell at your target one, two, three times.  All hit your target directly.  Your first spell lands at their belly button and they shiver as their body first shifts to an androgynous shape.  Then, in sharp contrast, their hips crack wider and you watch as their ass puffs up.  The second spell hits your target on the neck and almost immediately they find themself staring down at a pair of DD-cup tits straining underneath their top.  They mash their fleshy mounds together, trying to squeeze them smaller, but they keep on growing to E, F, who knows what size.  Your target sways as they feel the weight of them pulling them down.  The third spell hits their feet and they collapse on their padded feminine ass, breasts lurching like gelatin bowling balls.  Their thinner, feminine arms try but fail to prop them back up on their feet.  There your target sits, an impossibly large breasted woman lying helpless on the ground.  You peer down at them, watching, waiting.  With what must be every last ounce of willpower, your target starts to shrink their boobs back to a size where they can push themselves to their feet and reverse the rest of the changes, but halfway through their willpower fails them and they collapse, sweating, to the ground in a puddle of milk trickling our of their nipples.  Satisfied, you leave your target to their ridicuously mega-chested fate--for now.",
//#endregion         

//#region silver whiskey flask
//                    }, new Form {
//                 dbName = "form_silver_flask",
//                FriendlyName = "Self-Refilling Silver Whiskey Flask",
//                Description = "a small, silver flask originally shaped to discreetly hold whiskey.  Previously human, this flask taps from the consciousness it retains and slowly fills itself with a drinkable liquid seeped in mana.  Sometimes when the owner drinks from it, the flask will softly moan in pleasure...",
//                Gender = "female",
//                TFEnergyRequired = 85,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_mana_flask",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your attacker's spell strikes you on the elbow.  You rub your skin, feeling a smooth, cold, metallic texture about the size of a quarter, slowly growing wider.  Without much of a thought you reverse the effects.",
//                TFMessage_40_Percent_1st = "You are struck by your attacker's spell again, this time right on the nose.  You flinch as a shine appears on the tip, bright and polished like untarnished silver.  Your nose begins to melt into your face and you find that you can no longer breath through your nostrils.  Feeling a little disconcerted, you quickly rub at it with your fingers, coaxing the effects to reverse until you are your back to your previous shape.",
//                TFMessage_60_Percent_1st = "Your opponents launches another attack at you, their spell landing against your ribs.  You squeak as you feel your ribs shifting, your torso stretching out of your sides.  The cold, metallic feeling of the polished silver creeps up your body up to your sternum and down to your thighs, smoothing out your bones and muscule into a round, even surface.  Meanwhile you feel your neck begin to thicken and stretch taller, vertebrae being replaced by a solid inflexible bone.  By the time you are able to fight back the aggressive magic, your stiff body sort of resembles the shape of a flattened bottle.  With a bit of trouble you are able to revert back to your old form.",
//                TFMessage_80_Percent_1st = "Your opponents launches another attack at you, their spell landing against your ribs at the same place as before.  You squeak as you feel your ribs shifting, your torso stretching out of your sides.  The cold, metallic feeling of the polished silver creeps up your body up to your sternum and down to your thighs, smoothing out your bones and muscule into a round, even surface.  Meanwhile you feel your neck begin to thicken and stretch taller, vertebrae being replaced by a solid inflexible bone, the silver metal flowing up to your chin.  Your legs merge together, silver and smooth, turning from two distinct shapes into a cone shape that follows the contours of your bottle-like torso.  With this your entire body begins to shrink down, but before you lose your balance you muster the strength to reverse the effects and pull yourself back up to your feet.",
//                TFMessage_100_Percent_1st = "Your opponents launches another attack at you.  Immediately your entire body begins to shrink faster than you can fall, your torso and legs merging into a single, polished silver flattened cylinder.  Hollow, it feels like it.  You clatter as your strike the ground, an eight inch tall polished silver flask, your neck and shoulders turned into the narrow portion of the top.  You feel the scalp on what's left of your head tingle as it reshapes into a black screw-on cap, fluid sloshing around in your body.  Your face soon vanishes, absorbed into your featureless metal body.  At the very last moment, just before your vision fades for good, you do manage to fight the transformation enough to get a slight foothold against its effects.  With monumental effort you manage to coax your body back to its previous form, leaving you exhausted and feeling quite vulnerable should your attacker persist much longer.",
//                TFMessage_Completed_1st = "Your opponents launches another attack at you.  Immediately your entire body begins to shrink faster than you can fall, your torso and legs merging into a single, polished silver flattened cylinder.  Hollow, it feels like it.  You clatter as your strike the ground, an eight inch tall polished silver flask, your neck and shoulders turned into the narrow portion of the top.  You feel the scalp on what's left of your head tingle as it reshapes into a black screw-on cap, fluid sloshing around in your body.  Your face soon vanishes, absorbed into your featureless metal body.  At the very last moment, just before your vision fades for good, you do manage to fight the transformation enough to get a slight foothold against its effects.  You try to fight the magic but it is too late.  Your vision goes dark and you find yourself unable to move, wobbling slightly side to side on your silver metal back, totally inanimate.  A liquid sloshes around inside you, throwing off your center of gravity.  The movement excites you. making you feel horny in this strange new body. You feel your metallic insides ever so slowly secrete more inside yourself.  You'd love nothing more than to have someone pick you up and slip their tongue along your sensitive opening, slowly sipping out the liquid building up inside, the feeling of being drank from orgasmic as you offer your body and magic to your owner...",
                        
//                TFMessage_20_Percent_3rd = "Your spell strikes your target on the elbow.  They rub their skin where a smooth, cold metallic texture has replacing their skin in a patch about the size of a quarter, slowly growing wider.  They shrug it off and reverse the effects.",
//                TFMessage_40_Percent_3rd = "Your spell strikes your opponent again, thise time right on the nose.  They flinch as a shine appears on the tip, bright and polished like untarnished silver.  Your target's nose begins to melt into their face and they wheeze a little as they lost their ability to breath through their nostrils.  Looking disconverted, your opponent rubs at their nose, coaxing the effects until they have vanished.",
//                TFMessage_60_Percent_3rd = "You launch another attack againsg your opponent, your spell landing against their ribs.  Your opponent squeaks, their ribs shifting as their torso stretches out from side to side.  The shiny, silver gleam spreads out from underneath their clothes up to their sternum and down to their thighs, smoothing out their bones and muscule into a round, even surface.  Meanswhile their neck begins to thicken and stretch taller, vertebrae replaced by a single solid bone.  By the time they are able to fight back against the aggressive magic, the move stiffly with a body someone resembling a flattened bottle.",
//                TFMessage_80_Percent_3rd = "You launch another spell at your target, striking their ribs at the same place as before.  Your opponent squeaks, their ribs shifting as their torso stretches out from side to side.  The shiny, silver gleam spreads out from underneath their clothes up to their sternum and down to their thighs, smoothing out their bones and muscule into a round, even surface.  Meanswhile their neck begins to thicken and stretch taller, vertebrae replaced by a single solid bone, the silver metal flowing up to their chin.  Your opponent's legs merge together into one continuous, cylindrical curve that follows the contouyrs of your bottle-like torso.  With this their entire begins begins to shrink down, but before they lose their balance your target musters the strength to reverse the effects and pull themself back to their feet.",
//                TFMessage_100_Percent_3rd = "You launch another spell against your opponent.  Immediately their entire body begins to shrink faster than you they can fall, torso and legs merging into a single, polished silver flattened cylinder.  Hollow, it looks like.  Your target clatters as they strike the ground, an eight inch tall polished silver flask,  neck and shoulders turned into the narrow portion of the top.  What's left of your target's human head reshapes into a black screw-on cap, fluid sloshing around in your body.  Their face soon vanishes, absorbed into their new featureless metal body.  But somehow at the last moment they do manage to fight the transformation enough to get a slight foothold against its effects.  With monumental effort they manage to coax theor body back to its previous form, leaving them looking exhausted and vulnerable should your attacks persist much longer.",
//                TFMessage_Completed_3rd = "You launch another spell against your opponent.  Immediately their entire body begins to shrink faster than you they can fall, torso and legs merging into a single, polished silver flattened cylinder.  Hollow, it looks like.  Your target clatters as they strike the ground, an eight inch tall polished silver flask,  neck and shoulders turned into the narrow portion of the top.  What's left of your target's human head reshapes into a black screw-on cap, fluid sloshing around in your body.  Their face soon vanishes, absorbed into their new featureless metal body.  But somehow at the last moment they do manage to fight the transformation enough to get a slight foothold against its effects.  With monumental effort they manage to coax theor body back to its previous form, leaving them looking exhausted and vulnerable should your attacks persist much longer.  Your opponent tries to fight the magic but it is far too late.  The last humanoid features vanish into their flask body, wobbling back and forth ever so slightly as some internal fluid swishes about inside of them.  You sense a strong desire eminating from the flask, desiring for a master to rub their tongue along their top and drink deeply.  Maybe you'll oblige...",
//            },
//#endregion
                
//#region bunny babe
//            new Form {
//                 dbName = "form_bunny_love_tfnymic",
//                FriendlyName = "Bunny Babe",
//                Description = "A beautiful bouncy bunny babe with long white fur covered ears, large blue eyes, and big sparkling front teeth that protrude over red full lips, all framed by long red curls. Large breasts press against her clothing and bounce happily as she hops around. A small white tail flicks about above her very round butt and wide hips.",
//                Gender = "female",
//                TFEnergyRequired = 74,
//                MobilityType = "full",
//                PortraitUrl = "bunnygirl_nymictf_fixed.png",
//                FormBuffs = new BuffBox{
//                    FromForm_ManaRecoveryPerUpdate = -.25M,
//                    FromForm_HealthRecoveryPerUpdate = .25M,
//                    FromForm_MoveActionPointDiscount = .15M
//                 },

//                TFMessage_20_Percent_1st_M = "A cold trickle like ice flows down your spine as you feel the spell hit you. You watch as the hair along your arms thickens slightly and turns white. You also notice your fingernails as they darken and grow more pointed. With a shudder you shake off the transformations with no problem.",
//                TFMessage_40_Percent_1st_M = "When the spell hits you this time the cold is stronger and you feel your bones and teeth start to ache. The white hair returns to your arms and you can feel it on your legs as well. What appear to be small black claws appear on your fingertips. You feel your front teeth push down on your lower lip pressing it outward. A long red lock of hair falls in front of your eyes and you blow it away. Right before you wash away the spell you feel the tips of your ears start to tingle ominously.",
//                TFMessage_60_Percent_1st_M = "You were unable to block the spell and the cold returns with a vengeance. The familiar white fur returns to your skin and your front teeth burst from your lips. You feel your ears and find them much too long and pointed. Your nose twitches rapidly in your agitation. You hear a small pop behind you and turn your head to see a white tufted tail poking out above your butt. You turn back to your attacker, cheeks flushing red. You hope your embarrassment passes as anger. You cast the counter spell but it is becoming harder.",
//                TFMessage_80_Percent_1st_M = "Once again you are hit and your bones freeze in your body. All of your previous changes occur again, this time more rapidly, you have to brush back long red locks so you can see. The cold settles deep in your chest and you feel your skin roil. You stare in shock as large mounds of flesh push outward. Fur covered breasts stretch out your clothing revealing a long canyon of cleavage and your much larger, clearly female, nipples make themselves known as they grow hard. As you feel the cold move lower to your groin panic overtakes you and you thrust  your hand between your legs. You feel your manhood start to shrink and pull inside. In utter horror you focus your mind and slowly force the changes away.",
//                TFMessage_100_Percent_1st_M = "You almost fall to your knees with the force of the spell. Fur spreads like wildfire across your body. The breasts reappear,larger and more sensitive than before. The cold returns to your groin and you feel a sharp pain. You know before you reach to feel what has happened, your clawed fingers find only bare skin with no sign of your manhood. Almost as soon as you touch it the skin softens and opens up into you. The first warmth you've felt since being hit by the spell seeps into your body starting between your legs. Your fingers feel as you grow wet and you have to fight an urge to push them inside. A far too long foot starts thumping the ground and your heart races. Finally you rip your hand away from yourself and using your last bit of willpower you reverse the spell. You don't know if you can take much more.",
//                TFMessage_Completed_1st_M = "As you are hit one last time you have almost no strength to fight it. Your once male body twists and shifts into that of a very curvy, busty, woman. You glance around for any reflective surface and stare at what you see. Long red hair frames your perfectly feminine face. Your buck teeth poke out from lush red lips and brilliant blue eyes peer back at you blinking dark full lashes. Your cute bunny nose twitches and one of your long fur covered ears flicks at the sound of your attackers laughter. Perky breasts press out of your clothing and you can clearly see the nipples through the cloth. Your ass has grown round and full as well and you see the by now familiar white tail when you turn to look. Lower your feet stick out long and pointed. The warmth between your legs has grown more intense, the odd feeling of emptiness between your legs is almost overwhelmed by the new sensation of heat and moisture. You have an almost undeniable urge to satisfy your new body. ",
                        
//                TFMessage_20_Percent_3rd_M = "Your spell hits perfectly and you see your target stiffen. Soft white hair spreads across his arms and legs and you see him staring at dark nails on his hands. He brushes off the spell easily and glares at you.",
//                TFMessage_40_Percent_3rd_M = "You land the spell once again and watch his eyes grow wide. The fur grows thicker and his hair turns a deep red, growing slightly and falling into his eyes. You notice his ears have started to grow right before he manages to counter your spell.",
//                TFMessage_60_Percent_3rd_M = "Your targets skin is covered by a flurry of white fur this time, far quicker than the previous tries. Two white teeth burst from his mouth over a slightly swelling lower lip. The red hair returns, longer and slightly curly. His ears have shot up a good couple inches and fur starts to spread across them. Your target turns to see something behind him and you have to hold back a sharp laugh as you see his new bunny tail twitch above his ass. He turns back to you flushing red and casts the counter spell. ",
//                TFMessage_80_Percent_3rd_M = "The spell speeds up once again, a good sign your target's will is weakening. The first sign of the second part of your spell shows itself as two fur covered breasts swell up on his chest. You smile at the panic in his now large blue eyes as he reaches down between his legs to what must already be a noticeably smaller penis judging by his reaction. He reverses the changes once again but you know it must be growing difficult.",
//                TFMessage_100_Percent_3rd_M = "Once again the changes speed up and you see all the familiar changes take hold almost immediately. His breasts grow even larger than last time, fat nipples visible through his clothing. His now thick red locks tumble down past his shoulders and his face if far more feminine. His hands drop immediately to his crotch and it is clear this time he is too late. He as far as any onlooker is concerned is now a she and a very curvy she at that. His hand hesitates between his legs and you know he must be fighting his new sexual urges. Disappointingly he manages to pull his noticeably wet hand free and with a mighty struggle undo your spell.",
//                TFMessage_Completed_3rd_M = "When your final spell slams into your opponent it is clear that there is no fight left in him. The changes burst over his body and soon his is fully changed and the chance for him to undo the spell passes. You look over your handy work and laugh. Long red hair frames her perfectly feminine face. Straight buck teeth poke out from lush red lips and brilliant blue eyes peer back at you blinking dark full lashes. Her cute bunny nose twitches and one of her long fur covered ears flicks toward you at the sound of your laughter. Perky breasts press out of her clothing and you can clearly see the nipples through the cloth. Her ass has grown round and full as well and you see the by now familiar white tail when she turns to look. Lower down her feet stick out long and pointed. Her face reddens once again and you know she will soon lose to her new bunny like urge for sex.",

//                TFMessage_20_Percent_1st_F = "A cold trickle like ice flows down your spine as you feel the spell hit you. You watch as the hair along your arms thickens slightly and turns white. You also notice your fingernails as they darken and grow more pointed. With a shudder you shake off the transformations with no problem.",
//                TFMessage_40_Percent_1st_F = "When the spell hits you this time the cold is stronger and you feel your bones and teeth start to ache. The white hair returns to your arms and you can feel it on your legs as well. What appear to be small black claws appear on your fingertips. You feel your front teeth push down on your lower lip pressing it outward. A long red lock of hair falls in front of your eyes and you blow it away. Right before you wash away the spell you feel the tips of your ears start to tingle ominously.",
//                TFMessage_60_Percent_1st_F = "You were unable to block the spell and the cold returns with a vengeance. The familiar white fur returns to your skin and your front teeth burst from your lips. You feel your ears and find them much too long and pointed. Your nose twitches rapidly in your agitation. You hear a small pop behind you and turn your head to see a white tufted tail poking out above your butt. You turn back to your attacker, cheeks flushing red. You hope your embarrassment passes as anger. You stamp your foot and cast the counter spell but it is becoming harder.",
//                TFMessage_80_Percent_1st_F = "Once again you are hit and your bones freeze in your body. All of your previous changes occur again, this time more rapidly, you have to brush back long red locks so you can see. The cold settles deep in your chest and you feel your skin roil. Fur covers your breasts as they press themselves against your clothing growing more sensitive and your nipples make themselves known as they grow hard. As you feel the cold move lower to your groin panic overtakes you and you thrust your hand between your legs. You feel the fur start to cover your womanhood and an unmistakable wetness as you begin to grow horny. In utter horror you focus your mind and slowly force the changes away.",
//                TFMessage_100_Percent_1st_F = "You almost fall to your knees with the force of the spell. Fur spreads like wildfire across your body. Your breasts grow even more sensitive than before, your nipples as hard as ice. The cold returns to your groin but is erased by an almost burning heat. As soon as you touch it the skin softens and opens up to you. The warmth begins to spread across your body starting between your legs. Your fingers feel as you grow wetter and wetter and you have to fight an urge to push them inside. A far too long foot starts thumping the ground and your heart races. Finally you rip your hand away from yourself and using your last bit of willpower you reverse the spell. You don't know if you can take much more.",
//                TFMessage_Completed_1st_F = "As you are hit one last time you have almost no strength to fight it. Your body twists and shifts into that of a very curvy, busty, bunny girl. You glance around for any reflective surface and stare at what you see. Long red hair frames your perfectly feminine face. Your buck teeth poke out from lush red lips and brilliant blue eyes peer back at you blinking dark full lashes. Your cute bunny nose twitches and one of your long fur covered ears flicks at the sound of your attackers laughter. Perky breasts press out of your clothing showing off a long canyon of white fur covered cleavage. Your ass has grown round and full as well and you see the by now familiar white tail when you turn to look. Lower your feet stick out long and pointed. The warmth between your legs has grown more intense, heat pulses as you grow more and more wet. You have an almost undeniable urge to satisfy your new body. ",
                        
//                TFMessage_20_Percent_3rd_F = "Your spell hits perfectly and you see your target stiffen. Soft white hair spreads across her arms and legs and you see her staring at dark nails on her hands. She brushes off the spell easily and glares at you.",
//                TFMessage_40_Percent_3rd_F = "You land the spell once again and watch her eyes grow wide and a bit larger. The fur grows thicker and her hair turns a deep red. You notice her ears have started to grow right before she manages to counter your spell.",
//                TFMessage_60_Percent_3rd_F = "Your targets skin is covered by a flurry of white fur this time, far quicker than the previous tries. Two white teeth burst from her mouth over a slightly swelling lower lip. The red hair returns, longer and slightly curly. Her ears have shot up a good couple inches and fur starts to spread across them. Your target turns to see something behind her and you have to hold back a sharp laugh as you see her new bunny tail twitch above her ass. She turns back to you flushing red and casts the counter spell with a stomp of a bit larger than normal foot.",
//                TFMessage_80_Percent_3rd_F = "The spell speeds up once again, a good sign your target's will is weakening. The second part of your spell starts to take hold and you smile at the panic in her now very large blue eyes as she reaches down between her legs to what must already be a noticeably wet pussy judging by her reaction. She reverses the changes once again but you know it must be growing difficult.",
//                TFMessage_100_Percent_3rd_F = "Once again the changes speed up and you see all the familiar changes take hold almost immediately. Her breasts press hard into her clothing, fat nipples clearly visible. Her now thick red locks tumble down past her shoulders and her face had grown round and cute, large lips red and swollen. Her hands drop immediately to her crotch and it is clear the urge is nearly overwhelming. Her hand hesitates between her legs and you know he must be fighting her new sexual urges very hard. Disappointingly she manages to pull her noticeably glistening hand free and with a mighty struggle undo your spell.",
//                TFMessage_Completed_3rd_F = "When your final spell slams into your opponent it is clear that there is no fight left in her. The changes burst over her body and soon she is fully changed and the chance for her to undo the spell passes. You look over your handy work and laugh. Long red hair frames her perfectly feminine face. Straight buck teeth poke out from lush red lips and brilliant blue eyes peer back at you blinking dark full lashes. Her cute bunny nose twitches and one of her long fur covered ears flicks toward you at the sound of your laughter. Perky breasts press out of her clothing and you can clearly see the nipples through the cloth. Her ass has grown round and full as well and you see the by now familiar white tail when she turns to look. Lower down her feet stick out long and pointed. Her face reddens once again and you know she will soon lose to her new bunny like urge for sex.",
//#endregion            

//#region leopard bra
//               }, new Form {
//                 dbName = "form_leopard_bra",
//                FriendlyName = "Leopard Patterned Bra",
//                Description = "You are a leopard patterned bra.",
//                Gender = "female",
//                TFEnergyRequired = 76,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_leopard_bra",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your opponent casts their spell at you.  It nearly misses and hits you on your left foot.  You feel a tingling sensation but without even looking you reverse the changes.",
//                TFMessage_40_Percent_1st = "Your agressor casts their spell again.  This time it hits you a little higher up on the leg.  Your leg itches horribly and you take a look this time to discover orange, yellow, and black fuzz expanding form the point of impact, patterned like leopard skin.  You reverse the changes just as it finishes extending from your ankle to your thighs.",
//                TFMessage_60_Percent_1st = "Your opponent knocks you off balance with another cast of the spell.  This time it hits you on the waist and the same leopard fuzz begins to grow from there. You aren't sure what the spell is supposed to do to your body--turn you into a catgirl?  But no, while fuzzy, your skin is more like fabric than fur.  Additionally, your body begins to weaken and shrink where the fabric has taken over.  With a bit of effort you are able to reverse the changes hopeing you won't have to find out what happens next. ",

//                TFMessage_80_Percent_1st_M = "This time your attacker's spell hits you straight on the chest between your nipples.  You gasp, feeling two orbs swell out of your chest--clearly feminine breasts with a decent cup size, a rather interesting sensation on your otherwise male body.  But these aren't regular breasts, just empty curves as your inside evaporate out of small holes permeating your torso.  Suddenly you fold foward as the hollow in your body spreads to your waist.  With a great amount of effort you are able to reverse the magic and restore your previous male body.",
//                TFMessage_80_Percent_1st_F = "This time your attacker's spell hits you straight on the chest between your nipples.  Your nipples tingle as the magic spreads through your breasts.  As the magic flows through your skin, your insides begin to evaporate out of small holes permeating your boobs as all of the fleshy volume to them vanish, leaving them an empty bra still attached to your body.  Suddenly you fold forward as the hollowness reaches your waist.  With a great amount of effort you are able to reverse the magic and restore your previous female body.",

//                TFMessage_100_Percent_1st = "Once again you grunt as your opponent's spell slams into your body.  Your breasts swell and harden and you feel wires sliding through your body to give your bra-chest some strength.  Almost instantly your arms and legs evaporate, your torso quickly shriveling up into two leapord-spotted cups and a few dangling straps coming out of the sides and back.  You are little more than a bundle of still-shrinking fabric as your find your thoughts drifting, longing the sweet warmth of a woman's warm breasts to fill you out and fill out your form... However, you focus and are able to squeeze out the last of your willpower and ever so slowly rematerialize your previous form.  Any more, though, and you may find yourself holding up some breasts for the rest of your material existance.",
//                TFMessage_Completed_1st = "Once again you grunt as your opponent's spell slams into your body.  Your breasts swell and harden and you feel wires sliding through your body to give your bra-chest some strength.  Almost instantly your arms and legs evaporate, your torso quickly shriveling up into two leapord-spotted cups and a few dangling straps coming out of the sides and back.  You are little more than a bundle of still-shrinking fabric as your find your thoughts drifting, longing the sweet warmth of a woman's warm breasts to fill you out and fill out your form.  The fine fibers on both the inside and outside of your cups quiver in lust and you feel your old mind slipping away.  You are a bra now, a sexy leapord-spot garment, and the sooner you can please your master or mistress the better.  Your lust surges as your attacker picks you up and examines you.  All you can think is 'Put me on!  Put me on!  We both know you'll look so sexy with me around your bosom... Purrrrrrrrrrr...'",
                        
//                TFMessage_20_Percent_3rd = "You cast your spell at your target.  It nearly misses and hits then on their left foot.  They shake their foot a little but without even looking they reverse the changes.",
//                TFMessage_40_Percent_3rd = "You cast your spell at your victim once more.  This time it hits your target a little higher up on the leg.  Your target looks horrible uncomfortable as they scratch their skin near the impact, discovering orange, yellow, and black fuzz expanding form the point of impact, patterned like leopard skin.  They reverse the changes just as it finishes extending from their ankle to their thighs.",
//                TFMessage_60_Percent_3rd = "Your spell knocks your target balance.  This time it hits them on the waist and the same leopard fuzz begins to grow from there.  Your target begins to stumble as the portions of their body affected by the magic begins to shrink down and hollow.  With a bit of effort they are able to reverse the changes.",

//                TFMessage_80_Percent_3rd_M = "This time your spell hits your target straight on the chest between the nipples.  He gasps, feeling two orbs swell out of his chest--clearly feminine breasts with a decent cup size, a rather interesting look on his otherwise male body.  But these aren't regular breasts, just empty cloth cups as his insides evaporate out of small holes permeating his torso.  Suddenly they fold foward as the hollowness in his body spreads to his waist.  With a great amount of effort he is able to reverse the magic and restore his previous male body.",
//              TFMessage_80_Percent_3rd_F = "This time your attacker's spell hits your target straight on the chest between her nipples.  She rubs her nipples as the magic spreads through her breasts.  As the magic flows through her skin, her insides begin to evaporate out of small holes permeating her boobs as all of the fleshy volume to them vanish, leaving them as the rigid cups of an empty bra still attached to her body.  Suddenly she folds forward as the hollowness reaches her waist.  With a great amount of effort she is able to reverse the magic and restore her previous female body.",

//                TFMessage_100_Percent_3rd = "Once again your target grunts as your  spell slams into your opponent's body.  Their breasts swell and harden and you can vaguely see wires sliding through your body to give their bra-chest some strength.  Almost instantly their arms and legs evaporate, their torso quickly shriveling up into two leapord-spotted cups and a few dangling straps coming out of the sides and back.  Your target is little more than a bundle of still-shrinking fabric.  You watch as the transformation slows, your target's consciousness struggling to resist the mental effects of the change.  You sense that the bra wants you more than you want it, but the human mind seems to be winning.  Finally the human mind breaks through and your target's previous form rematerializes, though you doubt they can take much more...",

//                TFMessage_Completed_3rd = "Once again your target grunts as your spell slams into your opponent's body.  Their breasts swell and harden and you can vaguely see wires sliding through your body to give their bra-chest some strength.  Almost instantly their arms and legs evaporate, their torso quickly shriveling up into two leapord-spotted cups and a few dangling straps coming out of the sides and back.  Your target is little more than a bundle of still-shrinking fabric.  You watch as the transformation slows, your target's consciousness struggling to resist the mental effects of the change.  You sense that the bra wants you more than you want it and every second the human strength is failing.  Soon it is gone and all you can feel as you pick the sexy new bra up is its silent pleading, 'Put me on!  Put me on!  We both know you'll be so sexy with my on.  Purrrrrrrrrrr...'  It'd be a shame not to oblige this new simple-minded but loving undergarment.",
//            },
//#endregion

//#region red balloons

//                new Form {
//                 dbName = "form_99_red_balloons",
//                FriendlyName = "Bundle of Red Balloons",
//                Description = "You are a bundle of bright red, helium filled balloons constantly pulling against a plastic counterweight keeping you from floating away into the sky.",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_99_red_balloons",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your opponent's spell just slightly grazes you, just enough to leave your skin tingling a bit on your arm where the spell passed by you.  You reverse the effects without missing a beat.",
//                TFMessage_40_Percent_1st = "Your opponent grazes you again, but this time more of the spell has splashed against your skin.  You rub your arm, feeling your skin forming a lump of air underneath your skin.  You reverse the changes just as a second and third bubble begin to grow nearby.",
//                TFMessage_60_Percent_1st = "You aren't able to dodge your opponent's spell this time and hit hits you directly on your shoulder.  Immediately a big bubble inflates out of your skin, its surface very thin with a translucent pink tint.  It grows to the size of a baseball and a dozen more bubbles start popping out up along your arm.  Within five seconds your entire arm has been replaced by a series of these pink bubbles you find that you can no longer control the limb at all.  It lifts by itself, pulled upward by a gas lighter than air.  With a little strain you manage to reverse the effects.",
//                TFMessage_80_Percent_1st = "Your opponent strikes you with their spell once more.  This time your attempt to dodge fails miserably and you find yourself impacted by the spell directly on your chest.  For a second it feels like you are growing taller, but in reality your entire torso has almost instantaneously been replaced by pink balloon-bubbles, held together only through some gooey, sticky  strings stretching like rubber bands.  Several balloons do manage to break free and drift away and you feel the weight on your feet all but vanish as your body's net volume teeters at the outside air ressure.  You focus your mind, the sheer force of your willpower condensing your balloon-torso into your previous, human one.  Not all of you can be retrieved, and you feel a bit disconcerted as a few balloons that came from your half-transformed body vanish into the sky.",
//                TFMessage_100_Percent_1st = "Exhausted, there's little more you can do than clench your eyes shut as your opponent's spell strikes you dead on your chest again.  In less time than it takes for you to blink, you feel your body tear apart into two or three dozen red helium-filled balloons, each rushing higher, lifting your collective body several feet into the air.  You kick, struggling as your legs mold together into a bundle of white ribbons fastening the balloons that make up your body to a small plastic block, all that remains of your feet.  Your vision splits from human binocular visision to a chaotic, kaleidascopic field of view that allows you to observe every direction from many different angles all at the same time.  That is, in effect, your new essence--many small balloons that collectively make up one whole consciousness.  Between this and an almost euphoric delight in overcoming gravity, your old human mind nearly slips away.  But with every last ounce of concentration you are just barely able to pull your many balloon-bodies bakc into one, compressing them down into your old body.  You struggle to your feet, exhausted and feeling as though you weigh a thousand pounds.",
//                TFMessage_Completed_1st = "Exhausted, there's little more you can do than clench your eyes shut as your opponent's spell strikes you dead on your chest again.  In less time than it takes for you to blink, you feel your body tear apart into two or three dozen red helium-filled balloons, each rushing higher, lifting your collective body several feet into the air.  You kick, struggling as your legs mold together into a bundle of white ribbons fastening the balloons that make up your body to a small plastic block, all that remains of your feet.  Your vision splits from human binocular visision to a chaotic, kaleidascopic field of view that allows you to observe every direction from many different angles all at the same time.  That is, in effect, your new essence--many small balloons that collectively make up one whole consciousness.  You try to concentrate, to envision your previous form and tease it back into existence, but the urge to float up and defy gravity is too strong.  How could you ever be so heavy and pathetic again, now that you've tasted the sweet freedom of the open air and broke the shackles of petty gravity?  All you hope for--all you desire--is to please your new master enough some day so that he or she may cut you free from the plastic block weighing you down and release you into the warm, infinite sky.",
                        
//                TFMessage_20_Percent_3rd = "Your spell just slightly grazes your target.  They reverse the effects without missing a beat.",
//                TFMessage_40_Percent_3rd = "Your spell grazes your target again, but this time more of the spell has splashed against their skin.  Your targets rub their arm, staring in mild surprise as their skin begins to form some strange lumps.  They reverse the changes just as a second and third bubble begin to grow nearby.",
//                TFMessage_60_Percent_3rd = "Yout target isn't able to dodge your spell this time and it hits them directly on their shoulder.  Immediately a big bubble inflates out of their skin, its surface very thin with a translucent pink tint.  It grows to the size of a baseball and a dozen more bubbles start popping out up along your target's arm.  Within five seconds the entire arm has been replaced by a series of these pink bubbles, the limb bobbing up to the ceiling as your target discovers that they can no longer control the limb at all.  It lifts by itself, pulled upward by a gas lighter than air.  With some visible strain your target manages to reverse the effects.",
//                TFMessage_80_Percent_3rd = "You strike your target with your spell once more.  This time their attempt to dodge fails miserably and they find themself impacted by the spell directly on the chest.  Almost instantly you watch as your victim's torso is replaced by pink balloon-bubbles, held together only through some gooey, sticky  strings stretching like rubber bands.  Several balloons do manage to break free and drift away and you watch as your target's toes start to leave contact with the ground, pulled upward by their torso's new low density.  Your target clenches their eyes shut, focusing their mind. The sheer force of their willpower condensing their balloon-torso into their previous body.  Not all of your target can be retrieved and a few balloons that managed to break free drift up out of sight.",
//                TFMessage_100_Percent_3rd = "Exhausted, there's little more your target can do than clench their eyes shut as your spell strikes them dead on your chest again.  In less time than it takes for you to blink, your target has only millisecodns to gasp, feeling their body tear apart into two or three dozen red helium-filled balloons, each rushing higher, lifting their collective body several feet into the air.  Your target's kick before they too are transformed, struggling as the legs mold together into a bundle of white ribbons fastening the balloons that make up their body to a small plastic block, all that remains of their feet.  You detect an almost euphoric delight eminating from your target as their old human mind starts to slips away.  But with every last ounce of concentration they are just barely able to pull their many balloon-bodies back into one, compressing them down into their old body.  Your target struggles to their feet slowly and painstakingly as if they had not felt the effects of gravity in years.",
//                TFMessage_Completed_3rd = "Exhausted, there's little more your target can do than clench their eyes shut as your spell strikes them dead on your chest again.  In less time than it takes for you to blink, your target has only millisecodns to gasp, feeling their body tear apart into two or three dozen red helium-filled balloons, each rushing higher, lifting their collective body several feet into the air.  Your target's kick before they too are transformed, struggling as the legs mold together into a bundle of white ribbons fastening the balloons that make up their body to a small plastic block, all that remains of their feet.  You detect an almost euphoric delight eminating from your target as their old human mind starts to slips away.  Your target's body consolidates a little back into a vaguely human shape, but it is too little too late.  The last sticky strands holding the balloons into a single body evaporate, leaving your target nothing more than a bundle of shiny red balloons swaying in the breeze, struggling to break free and float into the atmosphere.",
          

//#endregion

//#region silk tube top
//               }, new Form {
//                 dbName = "form_silk_tube_top",
//                FriendlyName = "Silk Tight Tube Top",
//                Description = "You are a sky blue tube top made out of silk, your fabric thin but amazingly soft.  You cling to your wearer, their skin like a kiss against your entire body at once.  You lap up their sweet sweat and let the breeze pass through you to cool your master or mistress down.  It's a good life, simple but fulfilling.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_silk_tube_top",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your attacker's spell strikes your body.  Other than a little bit of tingling, however, you don't feel any different.",
//                TFMessage_40_Percent_1st = "Your attacker's spell strikes you once more.  It hits you on the hip and you feel your muscles and bones begin to dissolve, transforming into a thin, glossy fabric.  Only a small portion of your hip and waist is effected before you reverse the effects.",
//                TFMessage_60_Percent_1st = "Your attacker slings another spell at you, hitting you near your belly button.  The spell creeps up your chest, focusing its effects around your breasts.  Your skin in the targeted areas dissolves into several thin layers of a sky blue silk.  Meanwhile the extremities of your body--your toes, fingers, and hair begin to evaporate like dust being whisked away into the breeze.  You reverse the effects just as your knuckles vanish, before you lose your balance and fall over.",
//                TFMessage_80_Percent_1st = "Your attacker slings another spell at you, hitting you near your belly button.  The spell creeps up your chest, focusing its effects around your breasts.  Your skin in the targeted areas dissolves into several thin layers of a sky blue silk that extends in a ring around that entire part of your body.  Meanwhile your limbs begin to evaporate, your fingers, palms and wrists whisked away like dust.  Your legs do the same and you slip backwards, unable to feel the feet that you no longer actually have.  The impact when you hit the ground is shockingly small, your body mass maybe only half of what it was before.  On your back, you shut your eyes and manage to reverse the effects, but you start to wonder how many more times you can pull it off.",
//                TFMessage_100_Percent_1st = "You are struck by the spell full impact and you can no little more than gasp and stare as your torso dissolves like steam, nearly all of its mass whisked away leaving behind only a few thin layers of a sky blue, silk garment that seems to defy gravity.  Your vision swims and then fades to blue as your head is pulled down into the fabric, your five senses being distributed amongst the silk that makes up your body.  Your limbs are also all gone and you flutter to the ground, crumpling gracefully into a neat little pile.  You can feel every slight breeze against your silk-skin, every vibration from the ground.  You smell a human's scent nearby, aromatic and sweet, and all you can think about is being wrapped around his or her torso, pampering their skin as you drink in the sweetness of their sweat and oils.  However, a small part of your human mind resists, and with a titanic amount of willpower you manage reassemble your old body and pull yourself back to your feet.",
//                TFMessage_Completed_1st = "You are struck by the spell full impact and you can no little more than gasp and stare as your torso dissolves like steam, nearly all of its mass whisked away leaving behind only a few thin layers of a sky blue, silk garment that seems to defy gravity.  Your vision swims and then fades to blue as your head is pulled down into the fabric, your five senses being distributed amongst the silk that makes up your body.  Your limbs are also all gone and you flutter to the ground, crumpling gracefully into a neat little pile.  You can feel every slight breeze against your silk-skin, every vibration from the ground.  You smell a human's scent nearby, aromatic and sweet, and all you can think about is being wrapped around his or her torso, pampering their skin as you drink in the sweetness of their sweat and oils.  A small part of your old mind resists, but it is a losing battle.  Before long your old existence is little more than an old memory and you eagerly await the first special young lady--or guy--who first slides you, a sexy sky blue tube top, on.",
                        
//                TFMessage_20_Percent_3rd = "Your spell strikes your target's body.  However, there is not enough effect for you to notice any visible changes to their body.",
//                TFMessage_40_Percent_3rd = "Your spell strikes your target once more.  It hits them on the hip and they stumble as their muscles and bones begin to dissolve, transforming into a thin, glossy fabric.  Only a small portion of their hip and waist is effected before they reverse the effects.",
//                TFMessage_60_Percent_3rd = "You sling another spell at your target, hitting them near their belly button.  The spell creeps up your target's chest, focusing its effects around their breasts.  The skin in the targeted areas dissolves into several thin layers of a sky blue silk.  Meanwhile the extremities of your target's body--the toes, fingers, and hair--begin to evaporate like dust being whisked away into the breeze.  Your target manages to reverse the effects just as their knuckles start to vanish.",
//                TFMessage_80_Percent_3rd = "You sling another spell at your target, hitting them near the belly button.  The spell creeps up their chest, focusing its effects around the breasts.  The skin in the targeted areas dissolves into several thin layers of a sky blue silk that extends in a ring around that entire part of your target's body.  Meanwhile your target's limbs begin to evaporate, their fingers, palms and wrists whisked away like dust.  Your victim's legs do the same and they slip backwards, unable to stand on the feet that they no longer actually have.  Lying on their back, your target shuts their eyes and manage to reverse the effects, but clearly it is taking some significant effort now.",
//                TFMessage_100_Percent_3rd = "Your target is struck by the spell's full impact and they can no little more than gasp and stare as their torso dissolves like steam, nearly all of its mass whisked away leaving behind only a few thin layers of a sky blue, silk garment that seems to defy gravity.  Your target's limbs evaporate shortly after and what is rest of their body flutters to the ground, crumpling gracefully into a neat little pile.  You watch, waiting to see if your opponent is able to restore their old body.  Looking at the thin tube tube lying on the ground you are more than a little surprised when some part of your target's mind resists, and with a visibly titanic amount of willpower your target manage reassemble their old body and pull themself back to their feet.",
//                TFMessage_Completed_3rd = "Your target is struck by the spell's full impact and they can no little more than gasp and stare as their torso dissolves like steam, nearly all of its mass whisked away leaving behind only a few thin layers of a sky blue, silk garment that seems to defy gravity.  Your target's limbs evaporate shortly after and what is rest of their body flutters to the ground, crumpling gracefully into a neat little pile.  You watch, waiting to see if your opponent is able to restore their old body.  Looking at the thin tube tube lying on the ground you are more than a little surprised when some part of your target's mind resists and begin to reform their old body out of the thin air from whence it vanished.  But your target is too far gone and the effort peters out, leaving them lying on the ground, a thin sky-blue silk tube top waiting for you to introduce it into your wardrobe.",
            

//#endregion

//#region black stilettos
//               }, new Form {
//                 dbName = "form_black_stillettos",
//                FriendlyName = "Black 3-Inch Stillettos",
//                Description = "A pair of stilettos with 3-inch heels.  Its curves are sleek and feminine, expertly crafted with its leather exterior smartly polished.  Tiny gemstones line the the rims, glittering in the light and beaming small facets of light around its surroundings.  There's a deep sense of pride around these heels, a sense of confidence that it is above all other forms of footwear.  While wearing these stilettos are a little noisy and slow the wearer's walking speed, they enhance the ability wearer's self-confidence, increasing their willpower.",
//                Gender = "female",
//                TFEnergyRequired = 78,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_black_stillettos",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your attacker's spell hits you on your shoulder.  You stumble back a little, feeling the energy streak down along your veins to about your waist where it seems to run out of energy and dissipates, leaving you without any real changes to your body.",
//                TFMessage_40_Percent_1st = "Your opponent's spell strikes you on your hip.  Your leg tingles as the transformative magic slows down to your shins, gathering around your feet.  You begin to feel yourself lean forward just the slightest amount, both of your heels aching a bit as something seems to be trying to force their way out.  Before long the magic dissipates and your old form is restored.",
//                TFMessage_60_Percent_1st = "Once again your attacker's spell hits you.  As before the magic courses straight down to your feet, completely ignoring anything higher up than your shins.  But the magic is stronger and more centralized now and you feet your toes begin to merge into a narrow point.  You flail as two protrusions grow out of the bottom of each of your heels, sharp black columns that lift the back of your feet an inch higher into the air.  You notice that the soles and sides of your feet are turning black and leathery and it's at this time you realize that your feet are slowly turning into black stilettos while still attached to the rest of your body.  Before your form changes any further, you summon your strength and reverse the changes.",
//                TFMessage_80_Percent_1st = "Once again your attacker's spell hits you.  As before the magic courses straight down to your feet, completely ignoring anything higher up than your shins.  But the magic is stronger and more centralized now and you feet your toes begin to merge into a narrow point.  You flail as two protrusions grow out of the bottom of each of your heels, sharp black columns that lift the back of your feet one, two, then three inches higher into the air.  But while this happens, the rest of your body begins to shrink and you watch as your opponent and surroundings start to loom over you.  Your stiletto-feet start to hollow and fall to your knees, unable to support the weight of your torso.  Before you lose the majority of your human shape, however, you muster the willpower to halt and reverse the effects on your body, though it takes a fair amount of effort.",
//                TFMessage_100_Percent_1st = "Once again your attacker's spell hits you.  As before the magic courses straight down to your feet, completely ignoring anything higher up than your shins.  But the magic is stronger and more centralized now and you feet your toes begin to merge into a narrow point.  You flail as two protrusions grow out of the bottom of each of your heels, sharp black columns that lift the back of your feet one, two, then three inches higher into the air.  But while this happens, the rest of your body begins to shrink and you watch as your opponent and surroundings start to loom over you.  Your stiletto-feet start to hollow and fall to your knees, unable to support the weight of your torso.  What remains of your legs and torso are absorbed into your the pair of stilettos, each fully separate from the other.  As your vision goes dark, a sense of pride starts to flood your mind.  You might be a pair of shoes, but not just ANY pair of shoes--you are the BEST and SEXIEST pair, well worth being fought over.  But a part of your old mind remembers and resists, slowly restoring bringing back your old body with virtually every whisp of willpower you have left.",
//                TFMessage_Completed_1st = "Once again your attacker's spell hits you.  As before the magic courses straight down to your feet, completely ignoring anything higher up than your shins.  But the magic is stronger and more centralized now and you feet your toes begin to merge into a narrow point.  You flail as two protrusions grow out of the bottom of each of your heels, sharp black columns that lift the back of your feet one, two, then three inches higher into the air.  But while this happens, the rest of your body begins to shrink and you watch as your opponent and surroundings start to loom over you.  Your stiletto-feet start to hollow and fall to your knees, unable to support the weight of your torso.  What remains of your legs and torso are absorbed into your the pair of stilettos, each fully separate from the other.  As your vision goes dark, a sense of pride starts to flood your mind.  You might be a pair of shoes, but not just ANY pair of shoes--you are the BEST and SEXIEST pair, well worth being fought over.  Some small part of your old mind remembers and resists, but your new identity fights back, sprouting gemstones along your ridges, sweeping any doubt in your mind that you are just simply the most elegant, unique, and stylish pair of stilettos this world has ever seen!",
                        
//                TFMessage_20_Percent_3rd = "Your spell hits your target on their shoulder.  They stumble back a little, feeling the energy streak down along their veins to about your their waist where it seems to run out of energy and dissipates, leaving your target without any lasting changes to their body.",
//                TFMessage_40_Percent_3rd = "Your spell strikes your target on the hip.  They clutch their legs, the transformative magic slinks down to their shins, gathering around the feet.  They begin to lean forward just the slightest amount, both of their heels growing a bit taller.  Before long the magic dissipates and their old form is restored.",
//                TFMessage_60_Percent_3rd = "Once again you spell strikes your target.  As before the magic courses straight down to your opponent's feet, completely ignoring anything higher up than their shins.  But the magic is stronger and more centralized now and their toes begin to merge into a narrow point.  Your target flails as two protrusions grow out of the bottom of each of their heels, sharp black columns that lift the back of their feet an inch higher into the air.  You notice that the soles and sides of your target's feet are turning black and leathery, slowly turning into black stilettos while still attached to the rest of the body.  Before their form changes any further, your target summons their strength and reverse the changes.",
//                TFMessage_80_Percent_3rd = "Once again your spell hits its mark.  As before the magic courses straight down to your target's feet, completely ignoring anything higher up than their shins.  But the magic is stronger and more centralized now and you target stumbles as their toes begin to merge into a narrow point.  They flail as two protrusions grow out of the bottom of each of their heels, sharp black columns that lift the back of their feet one, two, then three inches higher into the air.  But while this happens, the rest of your target's body begins to shrink and you watch as your opponent shortens down to just a couple feet tall.  Their stiletto-feet start to hollow they fall to their knees, unable to support the weight of their torso.  However, before the transformation can be completed, let alone made irreversible, your target gathers the willpower to restore their old form with a considerable amount of effort.",
//                TFMessage_100_Percent_3rd = "Once again your spell hits its mark.  As before the magic courses straight down to your target's feet, completely ignoring anything higher up than their shins.  But the magic is stronger and more centralized now and you target stumbles as their toes begin to merge into a narrow point.  They flail as two protrusions grow out of the bottom of each of their heels, sharp black columns that lift the back of their feet one, two, then three inches higher into the air.  But while this happens, the rest of your target's body begins to shrink and you watch as your opponent shortens down to just a couple feet tall.  Their stiletto-feet start to hollow they fall to their knees, unable to support the weight of their torso.   What remains of your target's legs and torso are absorbed into the pair of stilettos, each fully individual from the other.  Your target's transformation is complete, but some part of their old mind is able to fight back, slowly reversing the changes until they are back to their old form.  The effort leaves them looking exhausted, unable to fight back much longer...",
//                TFMessage_Completed_3rd = "Once again your spell hits its mark.  As before the magic courses straight down to your target's feet, completely ignoring anything higher up than their shins.  But the magic is stronger and more centralized now and you target stumbles as their toes begin to merge into a narrow point.  They flail as two protrusions grow out of the bottom of each of their heels, sharp black columns that lift the back of their feet one, two, then three inches higher into the air.  But while this happens, the rest of your target's body begins to shrink and you watch as your opponent shortens down to just a couple feet tall.  Their stiletto-feet start to hollow they fall to their knees, unable to support the weight of their torso.   What remains of your target's legs and torso are absorbed into the pair of stilettos, each fully individual from the other.  You wait a bit, trying to see if your target can pull off some kind of miraculous reversal again.  As you watch gemstones begin to sprout from the linings of the stilettos as if to confirm that your target has fully accepted their fate, and not only that but is proud to now be simply the most elegant, unique, and stylish pair of stilettos this world has ever seen!",
//            },

//#endregion

//#region vibrating th
//                  new Form {
//                 dbName = "form_vibrating_latex_thong",
//                FriendlyName = "Vibrating Purple Latex Thong",
//                Description = "",
//                Gender = "female",
//                TFEnergyRequired = 84,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_vibrating_latex_thong",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your opponent's spell strikes you.  Other than some tingling, you don't feel any different.",
//                TFMessage_40_Percent_1st = "Your opponent lands another spell against your body.  You raise a hand up to defend, but the spell strikes you squarely on your palm.  Your skin darkens and takes on a glossy, rubbery texture, but before it spreads much you reverse the changes.",
//                TFMessage_60_Percent_1st = "Your attacker slings another spell at you.  This time it hits your forearm and immediately you feel the muscle and bone vanish, leaving only your skin to sink down to your side as it turns to thick, oily latex.  So your opponent has something latex in mind, but it's too early to tell what.  You focus your mind and reverse the changes.",
//                TFMessage_80_Percent_1st = "Your opponent casts another spell at you, striking you on your left knee.  You topple over as the entire limb turns into latex and then starts to shrink, vanishing up to your hips.  You gasp as a strange new sensation begins, a vibration that seems to be coming from your groin, exciting your genitals and threatening to steal your concentration from the duel at hand.  Your other leg and part of your left arm also fall victim to the magic, shrinking all the way to your hip where your entire groin is replaced with latex.  You have to struggle a bit to gather the willpower to reverse the changes, but at last you succeed and return to your previous form.",
//                TFMessage_100_Percent_1st = "Your opponent's spell strikes you straight on your crotch and immediately you feel a small plastic cylinder form underneath your skin, vibrating so intensely that you are too busy drinking in its erotic pleasure to notice that all of your limbs have turned to latex and shrunk to your hips.  You don't even remember the point at which your torso and head shrunk and vanished, leaving your body as nothing more than a latex thong with only a few inches of surface area with a built in vibrator.  It is only when you can no longer draw pleasure from the vibration by itself, replaced by an instinctive knowledge that you will only be satisfied when your wearer is, that a piece of your old mind is able to focus enough to slowly grow back your old form and 'live' to fight another few minutes.",
//                TFMessage_Completed_1st = "Your opponent's spell strikes you straight on your crotch and immediately you feel a small plastic cylinder form underneath your skin, vibrating so intensely that you are too busy drinking in its erotic pleasure to notice that all of your limbs have turned to latex and shrunk to your hips.  You don't even remember the point at which your torso and head faded away, leaving your body as nothing more than a oily, dark purple latex thong, no more than a few inches of surface area, with a built in vibrator.  Your old mind slips away as you orgasm one last time, the vibration dying down.  You know that the only way you can experience such pleasure again will be to satisfy your owner instead.  You vibrate a little in anticipation as your once-enemy eyes you.  What's a better way to put your past life and its animosities behind you and show your new friendship and devotion with a some good, good, good vibrations tucked right against the most sensual bits of their body?",
                        
//                TFMessage_20_Percent_3rd = "Your spell strikes your opponent, but it's too soon to tell what effects the magic has on their body.",
//                TFMessage_40_Percent_3rd = "You land another spell against your target's body.  They raise a hand up to defend but the spell strikes them squarely on their palm.  Your target's skin darkens and takes on a glossy, rubbery texture, but before it spreads much they are able to reverse the changes.",
//                TFMessage_60_Percent_3rd = "You sling another spell at your target.  This time it hits their forearm and immediately the muscle and bone vanishe, leaving only your target's skin to sink down to their side as it turns to thick, oily latex.  Before it spreads past their elbow, your opponent is able to reverse the changes.",
//                TFMessage_80_Percent_3rd = "You cast another spell at your opponent, striking them on their left knee.  Your target topples over as the entire limb turns into latex and then starts to retract up to to hips.  Your target gasps and clutches their groin, a muffled vibrating noise originating from somewhere in that area.  Your target's other leg and part of their left arm also fall victim to your magic, shrinking all the way to the hip as the entire groin is replaced with a dark purple latex.  Your target struggles a bit but manages to succeed in reversing the changes, returning to their previous form.",
//                TFMessage_100_Percent_3rd = "Your spell strikes your target straight on the crotch.  Your opponent's eyes shoot wide as something begins to vibrate so intensely that they are soon too busy drinking in its erotic pleasure to notice that all of their limbs have turned to latex and shrunk to the hips.  They fail to notice or care even at the point in which their torso and head have shrunk away as well, leaving their body as nothing more than a latex thong with a built in vibrator.  You are surprised when the thong slowly but surely grows back to your opponent's previous form, but they look exhausted as they clamber back to their feet.",
//                TFMessage_Completed_3rd = "You hit yourr target straight on the crotch.  Your opponent's eyes shoot wide asas something begins to vibrate so intensely that they are soon too busy drinking in its erotic pleasure to notice that all of their limbs have turned to latex and shrunk to the hips.  They fail to notice or care even at point in which their torso and head have shrunk away as well, leaving their body as nothing more than a latex thong with a built in vibrator, no more than just a few inches of surface area.  You watch, waiting to see if your target is able to reverse the changes, but instead you begin to hear your opponent's sewn-in vibrator begin to buzz, quick happy little pulses that seem to plead for you to pay attention, to slip the new once-human garment up your legs and let it bring you to orgasm over and over again, no matter the time or place.",
//            },

//#endregion

//#region barmaid
//             new Form {
//                 dbName = "form_barmaid",
//                FriendlyName = "Flirty Olive-Skinned Barmaid",
//                Description = "A tall, attractive woman with charcoal black hair, dark green eyes, and a tan Mediterranean skin complexion.  Tattoos cover her arms and shoulders, continuing to flow underneath her clothes--as if anyone needed a reason to try and remove them in the first place.  No matter what kind of person she once was, she has the memory and experiences of being a professional barmaid and knows how to bend over just right to show off the best angle of her sexy ass, or lean over a beer-dampened counter-top for a good glimpse down her shirt.  While very fit and agile, always on her feet, she's never quite focused on her magic due to a combination of alcohol in her bloodstream and a raging libido that tends to pay the favor back to her best tipping customers.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "boozebaroness_by_nymic_tf-d7ekv5d.jpg",
     
//                 FormBuffs = new BuffBox{
//                 FromForm_ManaBonusPercent = -6,
//                 FromForm_SpellExtraTFEnergyPercent = -15,
//                 FromForm_EvasionPercent = 20},

//                TFMessage_20_Percent_1st = "Your opponent's spell grazes by your scalp.  You see your hair begin to darken, but you reverse the effects almost immediately.",
//                TFMessage_40_Percent_1st = "Your opponent's spell strikes your arm.  You watch as the magic spreads through your skin, coloring in a tan Mediterranean complexion.  The affected skin is smooth and hairless and you can just start to see faint markings before you reverse the effects.",

//                TFMessage_60_Percent_1st_M = "You gulp as your opponent's spell hits you straight in your gaping mouth.  Your mouth starts to tingle and you mildly detect the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn smooth and charcoal black, growing down to your collarbone, thick and finely conditioned.  Any hairs or stubble on your chin and neck vanish, leaving your skin smooth and soft, rather feminine.  The tingle of the transformation is just starting to reach your abs when you reverse the changes.",
//                TFMessage_80_Percent_1st_M = "You gulp as your opponent's spell hits you straight in your gaping mouth.  Your mouth starts to tingle and you mildly detect the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and finely conditioned.  Any hairs or stubble on your chin and neck vanish, leaving your skin smooth, rather feminine.  You tremble a little in anticipation as the magic forces out two fleshy mounds from your chest, two small but clearly feminine breasts with perky dark nipples, somewhere in the range of A-cups.  Your waist narrows as the magic continues to move down your body and you feel your hips just starting to widen when you reverse the effects and return to your previous form.",
//                TFMessage_100_Percent_1st_M = "You gulp as your opponent's spell hits you straight in your gaping mouth.  Your mouth starts to tingle and you mildly detect the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and finely conditioned.  Any hairs or stubble on your chin and neck vanish, leaving your skin smooth, rather feminine.  You tremble a little in anticipation as the magic forces out two fleshy mounds from your chest, two clearly feminine breasts with perky dark nipples, somewhere in the range of B-cups.  Your waist narrows as the magic continues to move down your body, your hips widening and ass plumping up mere seconds later, your manhood withdrawing into a pair of moist, swollen labia.  The magic finally flows down to your toes, leaving your legs curvy and hairless with crimson manicured nails.  You are one hundred percent woman and a hundred ten percent sexy.  Your mind begins to wonder how much you'll make in tips tonight and how many guys, or girls, will slip you their number on the back of a ten dollar bill before the night's end.  However, a gnawing doubt grows in your mind--this isn't who you are!--and with a surprisingly high amount of effort you manage to return to your old male body.  Next time, however, you may find yourself serving drinks to drunken and horny patrons for much longer...",
//                TFMessage_Completed_1st_M = "You gulp as your opponent's spell hits you straight in your gaping mouth.  Your mouth starts to tingle and you mildly detect the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and finely conditioned.  Any hairs or stubble on your chin and neck vanish, leaving your skin smooth, rather feminine.  You tremble a little in anticipation as the magic forces out two fleshy mounds from your chest, two clearly feminine breasts with perky dark nipples, somewhere in the range of C-cups.  Your waist narrows as the magic continues to move down your body, your hips widening and ass plumping up mere seconds later, your manhood withdrawing into a pair of moist, swollen labia.  The magic finally flows down to your toes, leaving your legs curvy and hairless with crimson manicured nails.  You are one hundred percent woman and a hundred ten percent sexy.  Your mind begins to wonder how much you'll make in tips tonight and how many guys, or girls, will slip you their number on the back of a ten dollar bill before the night's end.  There's a small gnawing doubt in the back of your mind--you're not a bartender, nor even a woman--but as you rub your soft legs together and feel your breasts shift with even that subtle motion, your hesitance fades and you no longer question your identity.  Almost as if in agreement, distinctive tattoos appear all over your arms and shoulders, each with a story or two you could tell about, a job or a lover you met on a Saturday night midnight shift...",

//                TFMessage_60_Percent_1st_F = "You gulp as your opponent's spell hits you straight in your open mouth.  Your mouth starts to tingle and you mildly sense the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and shiny.  You rub your finger along your cheek, feeling how smooth and feminine it feels.  The tingle of the transformation is just starting to reach your breasts when you reverse the changes.",
//                TFMessage_80_Percent_1st_F = "You gulp as your opponent's spell hits you straight in your open mouth.  Your mouth starts to tingle and you mildly sense the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and shiny.   You feel your boobs growing and shrinking, adjusting to a new cup size--somewhere around a C with dark, perky nipples.  Your waist and hips readjust a bit leaving you with a clear hourglass shape.  Before the magic reaches all the way down to your feet, however, you fight the magic and return to your previous form.",
//                TFMessage_100_Percent_1st_F = "You gulp as your opponent's spell hits you straight in your open mouth.  Your mouth starts to tingle and you mildly sense the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and shiny.   You feel your boobs growing and shrinking, adjusting to a new cup size--somewhere around a C with dark, perky nipples.  Your waist and hips readjust a bit leaving you with a clear hourglass shape.  The magic finishes off by reaching down to the tip of your toes, your body fully transformed into that a tan-skin, black haired Mediterranean bartender.  However, a gnawing doubt grows in your mind--this isn't who you are!--and with a surprisingly high amount of effort you manage to return to your old body.  Next time, however, you may find yourself with the impulse to serve drinks to drunken patrons for much longer...",
//                TFMessage_Completed_1st_F = "You gulp as your opponent's spell hits you straight in your open mouth.  Your mouth starts to tingle and you mildly sense the taste of tequila on your tongue as if you'd downed a shot or two some time ago.  The bangs in front of your eyes turn charcoal black, extending down to your collarbone, thick and shiny.   You feel your boobs growing and shrinking, adjusting to a new cup size--somewhere around a C with dark, perky nipples.  Your waist and hips readjust a bit leaving you with a clear hourglass shape.  The magic finishes off by reaching down to the tip of your toes, your body fully transformed into that a tan-skin, black haired Mediterranean bartender.  However, a gnawing doubt grows in your mind--this isn't who you are!  You are a witch engaging in ultimate combat with your peers--you don't have time to be running around serving with drunkards and wondering how much you can earn in tips after a few sensual minutes with a horny patron in the back room.  But as your old identity loses the battle, tattoos appear on your arms and shoulders, each with a story of a job or a particular lover you met on a Saturday night midnight shift...",
                        
//                TFMessage_20_Percent_3rd = "Your spell just barely grazes your target's scalp.  You see their hair begin to darken, but they reverse the effects almost immediately.",
//                TFMessage_40_Percent_3rd = "Your spell strikes your target's arm.  You watch as the magic spreads through their skin, coloring in a tan Mediterranean complexion.  The affected skin is smooth and hairless and you can just start to see faint markings appearing before they reverse the effects.",

//                TFMessage_60_Percent_3rd = "Your target gulps as your spell hits them straight in their open mouth.   The bangs in front of their eyes turn charcoal black, extending down to their collarbone, thick, shiny, and finely brushed.  Your target rubs their fingers along their cheek, feeling how smooth and feminine it feels.  The effects of the transformation is just starting to reach your target's chest when they manage to reverse the effects.",

//                TFMessage_80_Percent_3rd_M = "Your target gulps as your spell hits him straight in his open mouth.  You very faintly smell tequila on his breath.  The bangs in front of your target's eyes turn charcoal black, extending down to their collarbone, thick and finely conditioned.  Any hairs or stubble on your target's face vanish, leaving his skin smooth and soft, rather feminine.  Your target trembles a little in anticipation as the magic forces out two fleshy mounds from his chest, two small but clearly feminine breasts in the A-cup range with perky dark nipples.  Your target's waist narrows as the magic continues to move down his body and you watch as his hips just starting to widen when he reverses the effects and returns to his previous form.",
//                TFMessage_100_Percent_3rd_M = "Your target gulps as your spell hits him straight in his open mouth.  You very faintly smell tequila on his breath.  The bangs in front of your target's eyes turn charcoal black, extending down to their collarbone, thick and finely conditioned.  Any hairs or stubble on your target's face vanish, leaving his skin smooth and soft, rather feminine.  Your target trembles a little in anticipation as the magic forces out two fleshy mounds from his chest, two small but clearly feminine breasts in the B-cup range with perky dark nipples. His waist narrows as the magic continues to move down his body, hips widening and ass plumping up into round, sexy curves.  The magic flows down to his toes, leaving his legs curvy and hairless with crimson manicured nails.  Your target is now one hundred percent woman and a hundred ten percent sexy.  He starts to look distracted and smiles at you, blowing you a kiss and striking up a pose, inviting your lustful gaze.  But his expression quickly changes and he looks pained, a mental battle raging in his mind whether or not to accept his new identity.  To your surprise, he is able to resist and steps back, squeezing his eyes shut as he purges the new thoughts of being a barmaid out of his mind, reverting his body to its previous form.",
//                TFMessage_Completed_3rd_M = "Your target gulps as your spell hits him straight in his open mouth.  You very faintly smell tequila on his breath.  The bangs in front of your target's eyes turn charcoal black, extending down to their collarbone, thick and finely conditioned.  Any hairs or stubble on your target's face vanish, leaving his skin smooth and soft, rather feminine.  Your target trembles a little in anticipation as the magic forces out two fleshy mounds from his chest, two small but clearly feminine breasts in the B-cup range with perky dark nipples. His waist narrows as the magic continues to move down his body, hips widening and ass plumping up into round, sexy curves.  The magic flows down to his toes, leaving his legs curvy and hairless with crimson manicured nails.  Your target is now one hundred percent woman and a hundred ten percent sexy.  He starts to look distracted and smiles at you, blowing you a kiss and striking up a pose, inviting your lustful gaze.  But his expression quickly changes and he looks pained, a mental battle raging in his mind whether or not to accept his new identity.  To your surprise, he is able to resist and steps back, squeezing his eyes shut.  You watch his eyelids flutter, but his--now her--flirtatious smile returns and she no longer seems to resist the magic.",

          
         
//                TFMessage_80_Percent_3rd_F = "Your target gulps as your spell hits her straight in her open mouth.  You very faintly smell tequila on her breath.  The bangs in front of your target's eyes turn charcoal black, extending down to her collarbone, thick and finely conditioned.  Her skin flushes to a tan Mediterranean, olive shade.  Her breasts grow and shrink until they settle in a B-cup range, her waist similarly ending up thin and narrow.  The magic has just about reached her feet when she recovers and reverses the effects.",
//                TFMessage_100_Percent_3rd_F = "Your target gulps as your spell hits her straight in her open mouth.  You very faintly smell tequila on her breath.  The bangs in front of your target's eyes turn charcoal black, extending down to her collarbone, thick and finely conditioned.  Her skin flushes to a tan Mediterranean, olive shade.  Her breasts grow and shrink until they settle in a C-cup range, her waist similarly ending up thin and narrow.  The magic creeps down to her feet, painting her nails with a dark crimson polish.  Now fully transformed, she starts to look distracted and smiles at you, blowing you a kiss and striking up a pose, inviting your lustful gaze.  But her  expression quickly changes and she looks pained, a mental battle raging in her  mind whether or not to accept this new identity.  To your surprise, she is able to resist and steps back, squeezing her eyes shut.  You watch her eyelids flutter.  Soon her flirtacious smile turns to a scowl.  Slowly but surely she transforms back to her old form, not defeated quite just yet.",
//                TFMessage_Completed_3rd_F = "Your target gulps as your spell hits her straight in her open mouth.  You very faintly smell tequila on her breath.  The bangs in front of your target's eyes turn charcoal black, extending down to her collarbone, thick and finely conditioned.  Her skin flushes to a tan Mediterranean, olive shade.  Her breasts grow and shrink until they settle in a C-cup range, her waist similarly ending up thin and narrow.  The magic creeps down to her feet, painting her nails with a dark crimson polish.  Now fully transformed, she starts to look distracted and smiles at you, blowing you a kiss and striking up a pose, inviting your lustful gaze.  But her  expression quickly changes and she looks pained, a mental battle raging in her  mind whether or not to accept this new identity.  She is able to resist and steps back, squeezing her eyes shut.  You watch her eyelids flutter.  Her flirtacious smile starts to turn to a scowl, but all of a sudden she snaps her eyes open and giggles.  'Care for a drink?' she asks you.",

//            },

//#endregion

//#region mutt girl

//                 new Form {
//                 dbName = "form_mutt_girl",
//                FriendlyName = "Bitch in Heat",
//                Gender = "female",
//                TFEnergyRequired = 80,
//                MobilityType = "full",
//                PortraitUrl = "mutt_girl_lexam_the_gem_fox.jpg",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -5,
//                    FromForm_ManaBonusPercent = -5,
//                    FromForm_EvasionPercent = 20,
//                 },

//            },



//#endregion

//#region succubus by varn
                 

//             new Form {
//                 dbName = "form_succubus_varn1234",
//                FriendlyName = "Sexy Succubus",
//                Description = "She looks like the most attractive woman you've ever seen - except for the blood-red skin - the curled ram's horns sprouting from her forehead - the large bat-like wings sprouting from her back - her sinuous barbed tail - the claws that pass for her hands - the cloven hooves for her feet - the hellish rune of pubic hair surrounding her exposed, steaming snatch - the pools of hellfire that pass for her eyes... OK, maybe not so much like an attractive woman then. The transformation seems to have greatly enhanced her offensive abilities - but judging from her overwhelming lust, you suspect her willpower has been greatly diminished. Though pinning her down may be difficult, as it looks like she can move pretty quickly with those wings.",
//                Gender = "female",
//                TFEnergyRequired = 66.6M,
//                MobilityType = "full",
//                PortraitUrl = "succubus_by_nymic_tf-d7euc8y.jpg",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -7.5M,
//                    FromForm_ManaBonusPercent = 6,
//                    FromForm_ManaRecoveryPerUpdate = 2,
//                    FromForm_MoveActionPointDiscount = .25M,
//                    FromForm_ExtraSkillCriticalPercent = 11,
//                    FromForm_EvasionPercent = -35,
//                 },
//            },

//#endregion

//#region cat ears by varn1234

//             new Form {
//                 dbName = "form_catears_varn1234",
//                FriendlyName = "Cat Ear Headband",
//                Description = "You have been transformed - nya! - into a simple headband, atop which sit a set the cuttest widdle set of cat-like ears.  Changing in this way has cost you your ninth life - though you are purrfectly satisfied to grant your feline essence to your new owner.  You've even given them the gift of a lovely, fluffy tail!  Though you worry that the paws you've turned their hands into may interfere with their spellcasting...",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_catears_varn1234",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You feel a number of changes to your body - mostly around your butt, where it feels like something is exploding out your spine, and your ears, which seem to be migrating up your head.  You make note of a long fluffy tail, and triangular ears sitting atop your forehead - clearly a spell for turning you into a cat-person.  You are quickly able to banish the changes.",
//                TFMessage_40_Percent_1st = "Again, the cat-spell hits you.  This time, the change is more complete, as fur covers your entire body.  You also note that you are rather definitely a cat-GIRL, as you run your hands over your six breasts, letting out a startled 'Nya!' at the pleasurable sensation.  You are quickly able to banish the incantation - but did your fingers feel a bit stiff as you did it?",
//                TFMessage_60_Percent_1st = "Another hit from the Catgirl Spell - although, as you quickly shrink to a height of what you'd guess to be 3 feet, and find yourself standing more comfortably on your four paws, you begin to worry that, while you still appear mostly human, it may be a regular Cat Spell instead.  Though the stiffness which lingers in your limbs after you cancel it worries you - shouldn't cats be more graceful than this?",
//                TFMessage_80_Percent_1st = "You are again hit with the spell, and realize that it is trying to turn you into a full cat after all - standing there on all fours, less than a foot long with your back arched, covered in black fur - it's only the human features lingering in your face that suggests you aren't a common housecat.  You also have extreme difficulty moving any of your muscles - it is only through considerable perseverance that you are able to summon the motions that return you to your old form.",
//                TFMessage_100_Percent_1st = "You brace to again be turned into a strangely immobile cat - and this time, find motion nearly impossible.  Your arched back is exaggerated, forming a semi-circle against the ground - moreover, your front and back pairs of legs merge together to be indistinguishable from your torso.  Your head seems to shrink back into the bent torso as well - only your ears clearly remaining, migrating up to the furthest point from the ground.  Even the fur from everywhere but your ears seems to vanish, your skin turning into a hard plastic.  It seems you're turning into some sort of headband, festooned with your feline ears.  Somehow, you are able to break free - but you are in no way confident that you will be able to do so again.",
//                TFMessage_Completed_1st = "This time, the process runs to full completion - you shrink downwards, turning first to catgirl, then to housecat, then to headband.  Your body made of hard plastic forming a semi-circle roughly the size of a person's head, the only organic-looking features your feline ears.  Your game is over, as you prepare to settle in for a long catnap, granting your feline essence to your new owner...",
                        
//                TFMessage_20_Percent_3rd = "Your victim gasps as they take on some feline features - they seem more astonished by the long fluffy tail, than what you know to be the far more significant pair of ears migrating to the top of their forehead.  They are easily able to banish the changes. ",
//                TFMessage_40_Percent_3rd = "This time, the feline transformation is much more complete, as fur covers your victim's body from head to toe, the 6 full breasts making it clear that she is now a Catgirl.  A startled 'nya!' issues from her mouth as she takes in her changes.  Unfortunately for your libido, she chooses to quickly revert the changes rather than explore her new body.",
//                TFMessage_60_Percent_3rd = "You are again able to transform your victim into a Catgirl - though this time she is much shorter, and significantly more feline-looking.  It begins to dawn on her that she may be getting turned into a Cat, not a Catgirl.  She is wrong, however - though she does not seem to notice the stiffness in her limbs as she slowly reverses your spell.",
//                TFMessage_80_Percent_3rd = "This time, you are able to transform your opponent almost fully into a black-furred, female housecat - only a bit of human features lingering on her face suggest that she was ever anything other than feline.  She looks as if she wants to pounce on you, perhaps to claw your eyes out - but is having significant difficulties moving.  She instead settles for reverting the spell, though it takes her much longer than before.",
//                TFMessage_100_Percent_3rd = "After taking on the form of a motionless housecat, your opponent begins to change further.  Legs, head and torso merge, forming only a semi-circle of shiny black plastic - the only organic features that remain are her ears, which you saw sliding up her back as it plasticized, eventually coming to rest at the summit of the arch.  Somehow, someway, your opponent is able to recover from this - though you doubt they will be able to do so again.",
//                TFMessage_Completed_3rd = "You have fully transformed your opponent into a black plastic hairband, upon which sit a pair of realistic-looking cat ears.  You strongly contemplate wearing them - knowing that they will give you a significant amount of feline grace and agility.  Though they may well tamper with your spellcasting, nya!",

//             },

//#endregion

//#region attachable udder by Varn

//                    new Form {
//                 dbName = "form_attachable_udder_varn12345",
//                FriendlyName = "Detachable Udder",
//                Description = "Don't have a cow, but your adventure in magic has ended in udder failure.  You've been reduced to the form of a detachable, magic udder - wearable by your new opponent, presumably for the purpose of generating fresh restorative milk for them (unless they just happen to like being a walking milk fountain.)   If it's any consolation, every time you are milked feels udderly bovine (er, divine) - and having your teats suckled is more pleasurable than you could possibly have ever imagined.  This isn't going to be so bad, really...  Well, adios moochacho.  Better luck next time.",
//                Gender = "female",
//                TFEnergyRequired = 76,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_attachable_udder_varn12345",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Upon the cast, your chest swells to bimbo-like proportions - well over a DD-cup, though exactly how much is hard to say.  Especially as they appear to be overly full of milk.  Some mage with a lactation fetish?  Great, that's just what you need today.  Sighing, you quickly cancel out the spell.",
//                TFMessage_40_Percent_1st = "Your chest swells again - having had time to consider measurements since the last assault, you'd estimate that, based on the difference from DD, this pair is an H-cup.  And it is VERY easy to judge the difference, as a smaller set of DD-breasts grow in under them.  The mammary tissue dominates your torso to a huge extent - in fact, are you shorter?  No - you think, it's just your imagination.  What ISN'T your imagination is the milk leaking from your four swollen breasts - 'that mage must be a real pervert', you think, as you neutralize the effects.",
//                TFMessage_60_Percent_1st = "Again, you are assaulted by the mage with the fetish for enormous lactating breasts.  This time, the four are of equal size - and larger than before, stopping around the upper limits of bra technology, a K or L cup.  Your nipples seem both larger and longer, and touching each in turn leads to large spurts of milk.  And you are DEFINITELY shorter, having lost nearly a foot of height...  Thankfully though, no part of the perverse transformation is able to prevent your counterspell, and you soon return to your old height and proportions.",
//                TFMessage_80_Percent_1st = "Again, you are hit by the breast-focused mage.  Your twin racks seem even larger to you this time - but they may well be smaller in truth, as you find yourself risen to a height of only two feet tall!  Playing with them, with your arms that (just like your legs) seem even shorter than they should be proportionately (and damn, does that feel good) - you find that the four orbs are sticking together, even moreso than you would expect from the dried lactose runoff.  Pondering what it means, you manage to revert the changes - though the mammary magic is starting to take hold, causing you to expend a great deal of effort to do so.",
//                TFMessage_100_Percent_1st = "Yet another assault by the breast-centric magic.  This time, while you first form four large, distinct breasts in the L-cup range, as you shrink down - the rest of your body only inches high, smaller than the massive milk-spewing mounds - they fuse together into one large breast, the flesh around them turning significantly pink.  In fact, the only evidence that they were ever separate entities are the four huge, long nipples.  Or, you realize, the four teats - as you have become an inches-tall human attached to a large, over-producing udder.  You can almost feel your consciousness shifting to lie within the large, fleshy milk-sack, ending your career as a mage... but you are somehow able to counter the magic.  Barely.  As you regain your old height, the ridiculous wobbling protrusions shrinking back away, you realize that if you are hit again, you may not be able to escape again...",
//                TFMessage_Completed_1st = "As before, the 4 enormous breasts form on your chest - and again they fuse together as the remainder of your body shrinks.  This time, you do not stop as an inches-high human attached to a mighty udder - your body continues to shrivel away, until only the udder remains, your mind now somehow taking up residence in the giant pink milk-sack.  You realize you are wearable - detachable even - and feel an enormous desire to be attached to an owner, even if it is the mage that so rudely ended your life.  Your teats stand erect, in anticipation of being milked - you know that every spurt will feel like a delicious orgasm to you, even moreso if someone attempts to suckle.  Perhaps this will not be so bad after all...",
                        
//                TFMessage_20_Percent_3rd = "Your spell causes your victim's chest to swell into a set of impressive globes, large enough to put those Busty Blonde Bimbos to shame, yet still not quite outside the realm of plausible natural human anatomy.  Your victim quickly realizes what you already know - that they look larger than their true size due to being stuffed full of milk.  You'll have to try again though - you don't even get to see any lactation before they quickly revert the changes, scowling at you all the while.",
//                TFMessage_40_Percent_3rd = "Your spell again hits your chosen victim, this time inflating their breasts in a way that cannot possibly be natural - the large H-cup tits riding high on their chest might (theoretically) be natural, though the pair of DD-cups beneath them certainly cannot.  They look even a bit larger on your victim, as you notice they have also shrunk several inches in height.  They, however, are too busy scowling at the drops of white fluid forming at their four engorged nipples to have noticed their loss in altitude.  All too quickly, your victim is able to restore their former figure.  They look none too pleased with you.",
//                TFMessage_60_Percent_3rd = "Rather than two uneven pairs of breasts, this time your spell produces a foursome of K-cups, equally sized and quivering on your victim's shrunken torso as they leak copious amounts of milk.  Moreover, the loss in height is much greater this time - well over a foot has vanished from your victim's frame, causing the large breasts to completely dominate their shrunken torso.  Your victim runs their hands over the fleshy orbs - milk spurting out as each nipple is reached.  They at first seem to smile, but then their mind returns and they realize what is happening - they quickly reverse the breasts, regaining their old height and renewing their vow to hurt the one afflicting them.",
//                TFMessage_80_Percent_3rd = "This time, the four orbs do not grow any larger than before - in fact, after your victim finishes shrinking down, they may actually be smaller.  She (for regardless of preference or anatomy, you can't possibly think of anyone with breasts nearly bigger than the rest of their body as anything but 'she') is now only two feet tall.  Moreover, her arms, legs, torso and head are all out of proportion - seemingly all shrunken, flowing into the giant mammaries.  Mammaries which, you note, seem to be sticking together as she plays with them...  Nevertheless, she is able to recover, and soon stands before you unchanged.  For now.",
//                TFMessage_100_Percent_3rd = "On this assault, your victim shrinks even further down, attaining a final height of around six inches.  While her glorious boobies do shrink as well, they do not do so in proportion - each would still strain the limit of bra technology on a normal-sized woman (making them something like a ZZZZZZZZ cup on your tiny victim.)  You watch, fascinated, as the four orbs seem to fuse together, the quartet of nip - no, teats - being the only evidence of their prior separation.  As the flesh around it turns pink, you realize you are looking at a large udder - constantly spewing milk onto the ground - attached to a half-foot human.  Who, almost unbelievably, is able to overcome your magic - though you seriously doubt your victim will be able to do so again.",
//                TFMessage_Completed_3rd = "This time, the change is absolute.  The quad-breasts form, then merge into a milk-leaking udder, as the mage attached to them rapidly diminishes in height - finally vanishing altogether.  Though there is no physical sign, you know their mind is now trapped in the large, fleshy sack.  Picking it up, you squeeze three nipples in turn, imagining you can hear your former opponent crying out in ecstasy - and suckling the fourth seems to leave the milk-filled mass quivering in delight.  You feel revived after the drink - and are saddened to see the udder dry up.  Perhaps contact with human flesh will restore its life-giving lactation?  There is only one way to find out... ",

//                    },

//#endregion

//#region catnip toy
//         new Form {
//                 dbName = "form_catnip_mouse_Elynsynos",
//                FriendlyName = "Catnip Mouse Toy",
//                Description = "While you no longer feel anything while being carried around in your new form you can't help but feel extremely happy when someones batting you around and playing with you.",
//                Gender = "female",
//                TFEnergyRequired = 82,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_catnip_mouse_Elynsynos",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Your nose pushes forward slowly from your face forming a narrow muzzle. As you reach up to examine it you realize its made up of a soft brown cloth, but you are easily able to reverse the changes.",
//                TFMessage_40_Percent_1st = "The muzzle forms more quickly this time and a pair of thin whiskers explode out of the tip this time. A tingling sensation begins to spread through your extremities but you counter the changes before the spell takes much more effect.",
//                TFMessage_60_Percent_1st = "The cloth muzzle isn't much of a surprise anymore but you're caught off guard when your arms and legs shrink down and turn into cloth appendages as well. In a panic you hurriedly work on a counter spell and get back on your feet.",
//                TFMessage_80_Percent_1st = "Your entire body shrinks at once cause you to yell out in surprise. Taking quick stock of yourself you notice the soft brown cloth has taken over your entire body except for your head. It's even in a well made cross-stitched pattern like you would find on some cat toys. Worrying about staying this way for too long you quickly counter the spell and return to normal, but its a lot harder then it was before.",
//                TFMessage_100_Percent_1st = "Your used to the experience by now so you were prepared for the spells sudden shrinking effect. However the spells progressing even faster this time and the cloth overtakes the only thing it hadn't before, your head. As you begin to counter the spell a lethargic feeling overtakes you. Your body is hollowing out... wouldn't it be wonderful to have something nice put inside you and to be batted around and played with like a good toy mouse? You manage to shake off the feeling and counter the spell, but as you return to normal you realize you probably won't be able to do that for much longer.",
//                TFMessage_Completed_1st = "As the spell comes upon you again you almost welcome the change. Your tired of resisting... preferring the life of a plaything to this endless hounding. As your mouse body reforms and hollows out you notice something filling in the missing space this time. Catnip is forming inside your cloth body and as your waking mind fades a sense of pleasure fills you in the fact your going to be the perfect toy for some cat to play with. No longer resisting you fall into a half-sleep as pleasure and fulfillment dominate whats left of your mind as you await your new master to take you with him.",
                        
//                TFMessage_20_Percent_3rd = "Your opponents eyes open wide as a muzzle made from cloth pushes out from their face. After reaching up and touching it for a bit they quickly counter the spell and return to normal.",
//                TFMessage_40_Percent_3rd = "This time the spell take effect more quickly and the muzzle comes with a set of thin whiskers. As your victim works on their counter spell you see their hands start to turn, but before it can go much further they successfully finish their spell and return to normal.",
//                TFMessage_60_Percent_3rd = "Your victim isn't even surprised by the muzzle this time, but you take some satisfaction in their look of surprise as they fall to the ground when their arms and legs turn into cloth and take a more mouse appropriate shape. Surprised as they are they manage to quickly work out the counter spell as the transformation made them panic instead of looking at what was happening to them.",
//                TFMessage_80_Percent_3rd = "Your victim anticipates your spell this time and gets close to the ground as your spell takes effect. They still yell out in surprise however when your spell quickly shrinks them along with all of the previous changes. The cloth begins climbing their body and moving towards their head when the changes are finally stopped. You watch as your victim slowly turns back to normal from their baby sized cloth body.",
//                TFMessage_100_Percent_3rd = "As you let loose your spell once more your victim gives up on trying to stay standing and gets on the ground before it hits them. They quickly shrink down to the size of a normal mouse and the cloth begins to overtake their last human feature, their head. As you lean down to see if they'll succeed in countering your spell again you notice their eyes starting to fade to a pure black and push forward into a button shape, but before it can fully happen the changes stop and the fully cloth mouse in front of you shudders before slowly returning to normal. You're sure you'll have a new toy in just a couple more casts, one or two should be enough.",
//                TFMessage_Completed_3rd = "As you begin to cast your spell your victim almost seems to bend down before you, welcoming the change. All attempts at resisting the spell are gone from their actions and the soft brown cloth quickly sweeps across their body. As they reach the size of a normal mouse you bend over a pick them up noticing that this time the button eyes have fully formed. In just a few moments more you notice their body puff up as the hollow inside of the cloth fills with catnip. Smiling you bounce them up and down in your hand a few times as you know they've accepted their life as a plaything.",

//            },

//#endregion

//#region raver cat

//               new Form {
//                 dbName = "form_catgirl_raver_lexamthegemfox",
//                FriendlyName = "Raver Cat Girl",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "full",
//                PortraitUrl = "tt_technokitty_by_wrenzephyr2-d7eozjf.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -7,
//                    FromForm_ManaBonusPercent = -7,
//                    FromForm_HealthRecoveryPerUpdate = 3.25M,
//                    FromForm_ManaRecoveryPerUpdate = 3.25M,
//                    FromForm_ExtraSkillCriticalPercent = 5,
//                 },

               
//            },

//#endregion

//#region cowgirl 

//              new Form {
//                 dbName = "form_cowgirl_sunbro",
//                FriendlyName = "Curvaceous Cowgirl",
//                Description = " A beautiful Cowgirl with surprisingly soft white fur featuring the occasional black splotch, 3 digit hard clicking hoof nails on her hands and large sturdy hooves for feet, all pulled together by a slight bovine muzzle to her face and frizzy dirty blonde hair that gives her that 'farm girl' look. She has four very large breasts kept wrapped up under hir shirt, most just hanging free. Speaking with a slight southern twang she may not seem that smart, but her cheerful demeanor and sheer work ethic seems to just bring up the room alone, and just to wrap it all up is a cow tail swinging lightly off her generous rear.",
//                Gender = "female",
//                TFEnergyRequired = 73,
//                MobilityType = "full",
//                PortraitUrl = "tt_cowgirl_by_wrenzephyr2-d7evem4.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 10,
//                    FromForm_ManaBonusPercent = -12,
//                    FromForm_HealthRecoveryPerUpdate = 1.5M,
//                    FromForm_ManaRecoveryPerUpdate = -2,
//                    FromForm_SpellExtraTFEnergyPercent = 4,
//                 },

//                TFMessage_20_Percent_1st = "The enemy casts their spell successfully striking you right on your chest, fearing the worst you look down but notice no change until you feel a cramping in your hands, shaking them out you notice black spots and start to hear clicking before you dispel the effects and glare at the enemy.",
//                TFMessage_40_Percent_1st = "They land the spell once again and your hands cramp up faster this time, looking down you gasp in shock at your new 3 digit hands marveling at the tough exterior they have. Almost like hooves?! Hearing more clacking below you look down realizing you didn't even notice your feet changing fully into hooves! Shaking your head you focus and within moments your hands turn back to normal.",
//                TFMessage_60_Percent_1st = "Upon getting hit by the spell again you already hear the now familiar clacking of your hands and feet, before you can respond though more odd feelings crop up from the where the spell hit and shortly afterward you feel your chest pushing out, breasts expanding further and further until their hefty weight sticks out proudly but then just below that another set comes in just barely dwarfed by the first pair. Feeling them a sense of calm comes over you for a moment as you start to rub them gently and relax before you come to your senses and undo the spell.",
//                TFMessage_80_Percent_1st = "As the spell hits your large breasts bounce before you even land on your flat on your bum, which you immediately notice your landing was much comfier then before due to your greatly expanded rear. Rising back to your hooves you look to the other spell caster in bewilderment, how can you be letting this city folk get you so bad? Wait... There's something off about that but you can't quite point out what, it doesn't help your frizzy hair is getting in your eyes. Except you don't have frizzy hair...do you? Shaking your head swishing your new hair with it you focus your mind and undo the changes once again. This is starting to effect your mind badly too now, your not sure how much more you can take!",
//                TFMessage_100_Percent_1st = "As the spell hits your large breasts bounce before you even land on your falling back on your large cushiony bottom. Rising back to your hooves you huff in annoyance. 'You ain't much one for violence but this junebug's asking for it.' You shake your head out for a moment? What was that? 'I ain't got no accent, and why the heck would I want to go and hurt that city folk? They may hap be a bit lazy but that's just-' WHAT ARE YOU THINKING?! Looking down you see your skin is now covered in white fur with occasional black spots distinguishing it. Your hair too must have grown out more while you were off in southern belle land and no has a frizzy look to it with a dirty blonde look. You have to focus real hard and take longer due to slipping into southern twang but you finally manage to undo the spell. You doubt you can come back from that again though.",
//                TFMessage_Completed_1st = "As the spell hits your large breasts bounce before you fall backwards feeling everything about your body changing! Rising back to your hooves you huff in annoyance at your own clumsiness. Shucks you can't believe you went and bumped into this nice city folk here, that's just right plum embarrassing! You start to give an apology before they cut you of saying it's no problem with a bright smile, his kind attitude brings a smile to your faces as well at the kind action but still decide it probably best to be on your way to keep some of your dignity. 'Well shoot he didn't seem to mind none so I shouldn't either, I still have work to do on the farm anyhow!' Tightening the knot of your shirt covering your breasts you take a deep breathe in of the fresh air ready to not let a good day like this go to waste.",
                        
//                TFMessage_20_Percent_3rd = "Your spell hits perfectly and your target looks at you in confusion as nothing seems to happen. Their hands seem to visibly cramp up as they pull them up to examine them, noticing traces of black they shake their hands out and dispel the effects before glaring at you.",
//                TFMessage_40_Percent_3rd = "You land your spell once again and watch as their hands rapidly turn black and shiny converging into three digits. You almost snort in laughter as they finally notice the changes you also inflicted on their feet before with a little disappointment you see them undo the changes once again.",
//                TFMessage_60_Percent_3rd = "Upon hitting them with the spell again you already hear the now satisfying clacking of their new hooves, with greater interest you watch their chest expand out into four large, bouncing breasts as they lean forward a bit to accommodate the added weight. Looking at their face aside from slight changes in appearance making them more attractive they get a calm look to their eyes giving you hope for a moment that you've got them before they shake their head and seem to come out of it undoing the transformation. Drat!",
//                TFMessage_80_Percent_3rd = "You hit them harder with the spell this time knocking them right down, smirking as they land on an already expanding rear. Rising back to their hooves they huff to themselves glaring at you, as their hair frizzes up and starts growing out their expression of anger turns into on of confusion as they look curiously at their new hair failing to notice the fur also coming in before they once again shake their head and start undoing the transformation. But this time you feel more confident seeing the doubt that flashed across their eyes even as they undid the spell. Your getting close!",
//                TFMessage_100_Percent_3rd = "Hitting them easily with the spell you send them flying even further back onto their large rear. Rising back up they put they're hooved hands on their hips looking at you giving a snort of disapproval. You can't help but grin as more and more changes until they are a complete cowgirl minus the clothes and they don't even seem to notice it! Sadly they're train of thought seems to break at that and looking around in a slight panic they seem to focus once again on undoing the changes making you want to scream in frustration. That is until you hear them muttering in some kind of southern twang which also has the effect of slowing their progress. Just one or two more and she will be all done and ready for the fields.",
//                TFMessage_Completed_3rd = "Focusing all your energy into this last shot you push the energy forward right at them determined to end this once and for all, the power of which sends them flying back causing all the previous changes in a matter of moments. Coming to a landing they seem dazed for a few moments rising back to their hooves and you watch in delight seeing their clothes finally changed to a checkered tie across shirt and jean shorts. They look down at you making you notice the good 5 or 6 inches they now have on you and seem to be giving some apology for bumping into you. Playing along you brush it off as no big deal getting the desired result as the relief on her face seems to drown out any lingering memories of their previous form. Watching with satisfaction you watch her walk off after a quick excuse and seeing they large rear shake back and forth reminds You of how good a day it is!",

//            },

//#endregion

//#region adult doll
//              new Form {
//                 dbName = "form_sexdoll_lexam",
//                FriendlyName = "Inflatable Sex Doll",
//                Description = "A willing and eager sex doll, with nice round O's for it's mouth, cunny, and butt hole. Once upon a time it used to be a human, but now they are a toy who only exists to pleasure it's owner. The toy's mind has become very simple and vacant, only wishing to be used like a good dolly.",
//                Gender = "female",
//                TFEnergyRequired = 80,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_sexdoll_lexam",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You begin to feel lightheaded from the sudden rush of magic into your body. Your skin seems smother and shiny and you feel lighter than normal. With a faint blush you revert yourself back to normal with a simple thought, feeling oddly confused by the spell.",
//                TFMessage_40_Percent_1st = "You gasp as the spell takes hold of your body more this time. Once more your skin becomes smooth and shiny, you run your hand across your skin and are greeted with the squeak of latex on latex. You wobble a little as your hips and butt plump up as if air was being blown into them, gaining a more exaggerated feminine shape. With a blush you bring yourself to change back once again, missing the feeling of being so light.",
//                TFMessage_60_Percent_1st = "You open your mouth to moan, but a weak squeak is all that exits your lips. You to your ass, but oddly it is a very soft landing. You feel so light headed and empty, as if your insides are becoming nothing but air. Your skin squeaks as you move and look down at your extremely feminine shaped body. You smile to yourself and think how happy you will make someone when they get to cum inside you. Your eyes widen with surprise at that random thought and you quickly change yourself back to normal, although you begin to regret not pleasuring at least one person before you did.",
//                TFMessage_80_Percent_1st = "You squeak happily as the changes return, your breasts inflating and your nipples turning into erect latex bumps. Your hips and butt swell up nice and wide as you again drop to your backside, legs spread apart and your arms held at your sides in a classic sex doll position. Your mouth begins to form into a wide O shape and you can feel your new pussy and butt hole doing the same. The strange sensations prove to be to sudden as you muster your mind and force your magics to change you back again. You sit on the ground for a while in the sex doll position before you realize you had changed yourself back and you get sheepishly to your feet, feeling so heavy now.",
//                TFMessage_100_Percent_1st = "You eagerly drop to your ass with a squeak, spreading your legs and getting into the proper pose for a slutty little sex doll like yourself. Your huge inflated breasts stand proudly, your hips and ass eager to be caressed and touched. Your O shaped mouth begging to suck, your O shaped pussy begging to fuck. Your eyes gloss over and become painted on dolly eyes and let you look out through them with a dolly mindset. 'Dolly wanna fuck.....Dolly horny.....Dolly......not.....dolly....human?' You question yourself in your head as the little remaining human thoughts you had fought back and eventually gained control again. Your grunt and squeak as you slowly change back and look at your opponent, almost wanting to beg them to change you back into a happy and horny little fuck toy again.",
//                TFMessage_Completed_1st = "You eagerly drop to your ass with a squeak, spreading your legs and getting into the proper pose for a slutty little sex doll like yourself for the last time. Your huge inflated breasts stand proudly, your hips and ass eager to be caressed and touched. Your O shaped mouth begging to suck, your O shaped pussy begging to fuck. Your eyes gloss over and become painted on dolly eyes and let you look out through them with a dolly mindset. 'Dolly happy to be Dolly! Dolly ready to fucky fuck!' You chant happily in your little air filled head as your belly button pops out and becomes an inflation nuzzle for your owner to blow air into your and let the same air out. You don't mind though as you just sit there in the slutty sex doll position, awaiting someone to stick a nice big cock into one of your holes.",
                        
//                TFMessage_20_Percent_3rd = "Your opponent looks a little confused as your spell hits them. Their skin appears to be much smoother and shiny but that doesn't last long as they quickly change themselves back to normal.",
//                TFMessage_40_Percent_3rd = "Your opponent gasp as the spell takes hold of their body more this time. Once more their skin becomes smooth and shiny, they run a hand across their skin and are greeted with the squeak of latex on latex. They wobble a little as their hips and butt plump up as if air was being blown into them, gaining a more exaggerated feminine shape. With a blush they bring themselves to change back once again, slouching some as they appear to be feeling heavier.",
//                TFMessage_60_Percent_3rd = "Their mouth opens to moan, but a weak squeak is all that exits their lips. They fall to their ass, but oddly it is a very soft landing. They look light headed and dazed. Their skin squeaks as they move and look down at their extremely feminine shaped body. They smile and seem to think how happy they will make someone when they get to cum inside them. Their eyes widen with surprise and  quickly change themselves back to normal, although you can see they begin to regret not pleasuring at least one person before they did.",
//                TFMessage_80_Percent_3rd = "They squeak happily as the changes return, their breasts inflating and their nipples turning into erect latex bumps. Their hips and butt swell up nice and wide as they again drop to your backside, legs spread apart and their arms held at their sides in a classic sex doll position. Their mouth begins to form into a wide O shape and you can see their new pussy and butt hole doing the same. The strange sensations prove to be to sudden for them as they muster their mind and force your magics out to change themselves back again. They sit on the ground for a while in the sex doll position before they realize they had changed back and  get sheepishly to their feet, looking a little depressed.",
//                TFMessage_100_Percent_3rd = "They eagerly drop to their ass with a squeak, spreading their legs and getting into the proper pose for a slutty little sex doll like they are quickly becoming. Their huge inflated breasts stand proudly, their hips and ass eager to be caressed and touched. Their O shaped mouth begging to suck, their O shaped pussy begging to fuck. Their eyes gloss over and become painted on dolly eyes and they gaze ahead through them as their mind begins to change as well. They sit motionless for a few minutes before the little remaining human thoughts they had fought back and eventually gained control again. They grunt and squeak as they slowly change back and look at you, looking as if they almost want to beg you to change them  back into a happy and horny little fuck toy again.",
//                TFMessage_Completed_3rd = "They eagerly drop to their ass with a squeak, spreading their legs and getting into the proper pose for a slutty little sex doll like they are now going to spent forever as. Their huge inflated breasts stand proudly, their hips and ass eager to be caressed and touched. Their O shaped mouth begging to suck, their O shaped pussy begging to fuck. Their eyes gloss over and become painted on dolly eyes and they gaze ahead through them as their mind begins to change as well. They sit there for a few minutes again, but remain a doll this time. As you pick them up they shiver in your grasp, seeming to orgasm as you begin to deflate them so they can be used later.",

//            },

//#endregion

//#region boxer briefs

//              new Form {
//                 dbName = "form_boxer_briefs_PsychoticPie",
//                FriendlyName = "Boxer Briefs",
//                Description = "You're a pair of boxer briefs. Your purpose in life is to give your owner's genitals a nice, comfortable place to rest. All of your senses are filled with the warm, musky, tight, and sometimes sweaty sensation of genitals. Not to mention that the drippage is really disgusting. There's nothing that you can do about it, so you might as well settle in and learn to enjoy it.",
//                Gender = "male",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_boxer_briefs_PsychoticPie",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "The spell connects You feel... loose, like your insides are disappearing. You easily reverse the effects before things progress any further.",
//                TFMessage_40_Percent_1st = "The spell connects. You feel loose again, but this time your arms start to retract. As some strange material begins to appear on your stomach, you manage to reverse the changes.",
//                TFMessage_60_Percent_1st = "The spell connects, hitting you square in the chest. Your insides disappear as usual, but this time your legs retract along with your arms, leaving you flat on your back. The strange material begins to appear again, covering your entire midsection. It feels so soft like... cotton! You somehow manage to reverse the changes.",
//                TFMessage_80_Percent_1st = "You sigh as the spell connects yet again. You find yourself in the same position, flat on your back with cotton spreading across your body. This time, however, your head is also constricted by some sort of elastic band as the cotton begins to crawl up your neck. Also, you haven't noticed it up until now, but you're shrinking! 'Wouldn't it be nice if my face was buried in someone's genitals?' you think to yourself. Uh... where did that come from? You shake the thought from you head as you struggle to reverse the changes.",
//                TFMessage_100_Percent_1st = "Once again the spell connects. Your limbs retract, your insides melt, and you're covered in cotton again. You scream in pain as your skin begins to mold itself. Two cylinders made of your skin sprout from the bottom of your stomach, and two rough lines of cotton grow from the top of your head to your chest. 'I'd love to smell someone's testicles all day! Why is there no cock in my face?' You think to yourself. You shake your head to clear your thoughts, and somehow, miraculously, you're able to reverse the changes. You can't keep this up much longer...",
//                TFMessage_Completed_1st = "The spell connects, and you collapse from exhaustion as your limbs retract, your insides melt, cotton covers your body, and you shrink once more. The cylinders that sprouted previously keep growing, until they expand wide enough so that legs could fit through them. The rough lines of cotton regrow, and you scream as the left side of your face is ripped open. An even larger line of cotton appears, and curves along the center of your face, creating a snug, convenient passage to your insides that a nice hard cock could easy slip through. 'Mmmm . . . cock . . .' you think to yourself, as your new entrance connects to the cylinders that sprouted earlier. Cotton covers you completely, and you almost look like underwear now. The top of your head rips open, and the new hole created continued expanding outwards, until it's so large that someone's waist could easily slip inside. The elastic band that was formerly constricting your forehead now sits perfectly along the rim of this new hole. You sigh as your new shrunken, cotton form falls gently to the ground, and you're picked up by your new owner. That's right, you're a pair of boxer briefs now, so I hope you enjoy the taste of testicles!",
                        
//                TFMessage_20_Percent_3rd = "The spell connects with it's target. They look a little wobbly, but they reverse the changes effortlessly.",
//                TFMessage_40_Percent_3rd = "The spell connects with it's target. A look of shock spreads across your victim's face, as cotton begins materializing on their stomach, and they lose a few inches in height. They reverse the changes without much difficulty.",
//                TFMessage_60_Percent_3rd = "The spell connects with it's target again, this time hitting them square in the chest. You watch with glee as their legs and arms retract, and they wind up laying flat on their back, as they shrink to the size of a small child. They manage to reverse the changes just as the cotton begins materializing.",
//                TFMessage_80_Percent_3rd = "Your victim sighs as the spell connects once more. You watch as they suffer the same effects as before; they wind up flat on their back, covered in cotton, and shrunken. As an elastic waistband begins to materialize around their head, they reverse the changes with great difficulty.",
//                TFMessage_100_Percent_3rd = "The spell connects. You grin as your victim's limbs retract, their insides melt, and they're covered in cotton again. They scream in pain as their skin begins to mold itself. Two cylinders made of your skin sprout from the bottom of their stomach, and two rough lines of cotton grow from the top of their head to their chest. You notice that their expression has changed, and they don't seem to be resisting as much as before. They can't keep this up much longer.",
//                TFMessage_Completed_3rd = "The spell connects, and your victim collapses as their limbs retract, their insides melt, cotton covers their body, and they shrink once more. The cylinders that sprouted previously keep growing, until they expand wide enough so that legs could fit through them. The rough lines of cotton regrow, and your victim screams as the left side of their face is ripped open. An even larger line of cotton appears, and curves along the center of their face, creating a snug, convenient passage to their insides that a nice hard cock could easy slip through. The new entrance connects to the cylinders that sprouted earlier, as cotton covers them completely. They almost look like underwear now. The top of their head rips open, and the new hole created continued expanding outwards, until it's so large that someone's waist could easily slip inside. The elastic band that was formerly constricting their forehead now sits perfectly along the rim of this new hole. You give a satisfied smirk as their new shrunken, cotton form falls gently to the ground, and you walk over to claim your newest pal.",

//            },

//#endregion

//#region pirate wench
//             new Form {
//                 dbName = "form_pirates_curse_scipio_africanus",
//                FriendlyName = "Pirate Wench",
//                Description = "The pirate wench is a gorgeous woman with wild, untamed red hair and the glint of plunder in her eyes. Despite her fierceness, she's extremely attractive and her remarkable figure is hidden only partially by a daringly skimpy outfit designed for the waves and beaches of the Caribbean sea.  As a pirate, she's well-prepared for the rigors of combat with extra reserves of both mana and will, but she's also clearly driven a little crazy by greed such that she'll probably have a hard time staying ahead without taking in a steady stream of loot and plunder.  This wild little firecracker may not have time for precise marksmanship, but her wild emotions mean a full broadside from her arsenal packs an extra-powerful punch!",
//                Gender = "female",
//                TFEnergyRequired = 72,
//                MobilityType = "full",
//                PortraitUrl = "scipio_pirate.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 4,
//                    FromForm_ManaBonusPercent = 4,
//                    FromForm_CleanseExtraHealth = -3,
//                    FromForm_MeditationExtraMana = -3,
//                    FromForm_EvasionPercent = -10,
//                    FromForm_SpellExtraTFEnergyPercent = 25,
//                    FromForm_ExtraSkillCriticalPercent = -5,
//                 },

//                TFMessage_20_Percent_1st_M = "You duck out of the way of the oncoming strike, but you're not quite fast enough, and a bolt grazes your shoulder.  You feel a ripple of feminizing magic wash over your body, but it's a small hit, and easy enough to counter.  So far.",
//                TFMessage_40_Percent_1st_M = "You stumble a bit as you're hit with another bolt of the feminizing magic.  This time, the changes gain more of a foothold, and you can feel the beginnings of breasts start to push outward from your chest before you're able to put together a coherent counter-spell.  And is your hair getting longer?",
//                TFMessage_60_Percent_1st_M = "You take another direct hit, and the effects of the spell are more dramatic than ever.  This time, you're almost completely female by the time you're able to push back the magic, and your hair erupts into a fiery red as your breasts continue to grow.  Finally, you're able to reverse the changes, but not before you're seriously shaken.",
//                TFMessage_80_Percent_1st_M = "As you take another hit, you're knocked to the floor by the force of the magic. Your hair explodes outward into a long, flame-red tangle,  and your waist pulls inward with gusto even as a pair of sizable breasts erupts onto your chest.  You can feel a layer of makeup rising from your skin, but you manage to keep enough control to fight the transformation back down.  You're back to normal, but you're not sure how much longer you can resist the force of the change.",
//                TFMessage_100_Percent_1st_M = "Another hit slams into you, and you're almost completely wench-ified.  Your manhood disappears as your figure develops to the most dramatic proportions so far, and you can feel the wanton lust for booty both literal and metaphorical worming its way into your brain.  With a tremendous effort of will you're able to reverse the changes, but you know that if this keeps up it won't be long now before you're completely trapped as a pirate wench.",
//                TFMessage_Completed_1st_M = "One final hit slams into your body, and it's all over.  You don't have the energy left to resist as the transformation magic twists your body, stripping away the last vestiges of your masculinity and leaving behind only the lusty, wild and reckless pirate wench the spell was designed to create.  Your very thoughts twist in surrender, as fantasies of plunder, rum, and filthy pirate orgies wrestle for deckspace in your newly-molded mind.  There's no question about it: you're a pirate wench, through and through.",
                        
//                TFMessage_20_Percent_3rd_M = "Focusing on the spell structure you uncovered from the coin, you point at your victim and let him have it!  It's a solid hit, and though he easily dismisses the minor effects, you can tell the spell is working exactly as you'd hoped.",
//                TFMessage_40_Percent_3rd_M = "Another blast slams into your target, and he reels visibly from the impact.  You can see he's having a harder time resisting the effects now, as a pair of small breasts appears briefly on his chest before he's able to will them away.  You grin; it's working!",
//                TFMessage_60_Percent_3rd_M = "You focus hard on another bolt, and let him have it!  A torrent of energy slams into your adversary, and you watch wish pleasure as he struggles against the growing changes.  He manages to fight off the changes again, but not before he's more curvy redhead than man, at least briefly!",
//                TFMessage_80_Percent_3rd_M = "You let fly another barrage of magic against your foe, and score several satisfying hits.  By now, the redheaded form is coming easily, and you can see the panic on his face as he struggles with the new impulses and desires washing over him.  He manages again to revert the magic, but you can tell he's shaken now.",
//                TFMessage_100_Percent_3rd_M = "Your next hit is barely a graze, but at this point that's more than enough to send your victim almost completely into the Pirate Wench form, and from the anguish on his (or should that be her, now?) face makes it very obvious the mental changes are almost complete.  One more good hit, and your foe should be no more than another wanton pirate wench.",
//                TFMessage_Completed_3rd_M = "Another solid hit, and it's done.  Your victim struggles briefly, but he's no match for the onslaught, and the changes rip over his body like a feminizing cascade.  Where once stood a man, now lounges a svelte, buxom pirate wench, her eyes glinting with undisguised lust.  You smile.  Exactly as planned.",

//                TFMessage_20_Percent_1st_F = "You duck out of the way of the oncoming strike, but you're not quite fast enough, and a bolt grazes your shoulder.  You feel a ripple of transformation magic wash over your body, but it's a small hit, and easy enough to counter.  So far.",
//                TFMessage_40_Percent_1st_F = "You stumble a bit as you're hit with another bolt of the agressive magic.  This time, the changes gain more of a foothold, and you can feel the contours of your body shifting to align themselves with the design of the spell's target form.  You're able to reverse the changes, but for how long?",
//                TFMessage_60_Percent_1st_F = "You take another direct hit, and the effects of the spell are more dramatic than ever, and you begin to see the first indications of your final form--a curvy redheaded woman, from what you can tell--but you doubt it's as simple as a hair dye job. Finally, you're able to reverse the changes, but not before you're seriously shaken.",
//                TFMessage_80_Percent_1st_F = "As you take another hit, you're knocked to the floor by the force of the magic. Your hair explodes outward into a long, flame-red tangle,  and your waist pulls inward with gusto even as your breasts reshape themselves into bouncy, firm D-cups. You can feel a layer of makeup rising from your skin, but you manage to keep enough control to fight the transformation back down.  You're back to normal, but you're not sure how much longer you can resist the force of the change.",
//                TFMessage_100_Percent_1st_F = "Another hit slams into you, and you're almost completely wench-ified.  Your figure develops almost completely into that of the curvy redhead, and you can feel the wanton lust for booty both literal and metaphorical worming its way into your brain.  With a tremendous effort of will you're able to reverse the changes, but you know that if this keeps up it won't be long now before you're completely trapped as a pirate wench.",
//                TFMessage_Completed_1st_F = "One final hit slams into your body, and it's all over.  You don't have the energy left to resist as the transformation magic twists your body, stripping away the last vestiges of your old self and leaving behind only the lusty, wild and reckless pirate wench the spell was designed to create.  Your very thoughts twist in surrender, as fantasies of plunder, rum, and filthy pirate orgies wrestle for deckspace in your newly-molded mind.  There's no question about it: you're a pirate wench, through and through.",
                        
//                TFMessage_20_Percent_3rd_F = "Focusing on the spell structure you uncovered from the coin, you point at your victim and let her have it!  It's a solid hit, and though she easily dismisses the minor effects, you can tell the spell is working exactly as you'd hoped.",
//                TFMessage_40_Percent_3rd_F = "Another blast slams into your target, and she reels visibly from the impact.  You can see she's having a harder time resisting the effects now, as her figure begins to twist and reshape itself under the effects of your magic.  It's working!",
//                TFMessage_60_Percent_3rd_F = "You focus hard on another bolt, and let her have it!  A torrent of energy slams into your adversary, and you watch wish pleasure as she struggles against the growing changes.  She manages to fight off the changes again, but not before the new, redheaded form is clearly dominant, at least briefly.",
//                TFMessage_80_Percent_3rd_F = "You let fly another barrage of magic against your foe, and score several satisfying hits.  By now, the redheaded form is coming easily, and you can see the panic on her face as she struggles with the new impulses and desires washing over her.  She manages again to revert the magic in the end, but you can tell she's shaken.",
//                TFMessage_100_Percent_3rd_F = "Your next hit is barely a graze, but at this point that's more than enough to send your victim almost completely into the Pirate Wench form, and from the anguish on her face makes it very obvious the mental changes are almost complete.  One more good hit, and your foe should be no more than another wanton pirate wench.",
//                TFMessage_Completed_3rd_F = "Another solid hit, and it's done.  Your victim struggles briefly, but she's no match for the onslaught, and the changes rip over her body like a cascade.  Where once stood a noble adversary, now lounges only another svelte, buxom pirate wench, her eyes glinting with undisguised lust.  You smile.  Exactly as planned.",


//            },

//#endregion

//#region horse

//             new Form {
//                 dbName = "form_phallus_equus_varn",
//                FriendlyName = "Prosthetic Horsecock",
//                Description = "It's all over now but the sex - as you have been transformed into an enormous artificial horsecock.  Eighteen inches long, with a set of heavy, sloshing balls six inches in diameter - you stand proud and erect, mottled pink skin extending from a black, leathery sheath.  You are hollow on the inside - allowing a male owner to insert himself into you, feeling each of your sensations in what will for him be vastly improved equipment.  Moreover, small tendrils extruding from your base allow yourself to be attached to a female owner as well, granting her many new sexual options.  As an inanimate object, you don't get to decide - but if it were up to you, you would never stop fucking, ever... and given the way the city stood when last you were able to gaze upon it with human eyes, you may just get your wish.  You pulse, throbbing with anticipation...",
//                Gender = "male",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_phallus_equus_varn",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Upon being hit by this spell, all strength leaves your arms as they fall to your side - it's not as if you cannot move them, but it just feels so hard... as if they are sticking to your torso.  You notice your stance has changed as well, pressing your legs together tightly - realizing your seeming inability to spread them may cause some immense 'social difficulties'.  You are just noticing that you seem to have shrunk a bit all over - except for your two feet, which seem to be swelling up to fill a pair of clown shoes - when the trance is broken, and you are able to reverse the spell.",
//                TFMessage_40_Percent_1st = "Hit by the spell again, you find you aren't imagining things - your limbs instantly cleave to your sides, and fuse together - your body resembling one large tube.  A sssnake, perhapsss?  Your neck seems to lengthen, while your head changes shape, joining it - seemingly confirming your thought.  But no - you aren't entirely cylindrical.  Your feet are insanely swollen - the look like spheres, nearly a foot in diameter.  Something about your overall shape is really bothering you, but as you cancel the effects,  you just can't quite grasp the the worry in your fingers and tug on it...",
//                TFMessage_60_Percent_1st = "Why did it have to not be snakes?  The spell hits again, and it is quite obvious that you are turning into a bit of a dick.  A huge one - though smaller than you were at a current height around 3 feet 6 inches.  From the neck down, you are entirely a long, fleshy tube - with your feet having become balls about in proportion to a typical human male's at your base.  Your head has lost nearly all human definition - the only visible feature remains your mouth, which loses lips, teeth and tongue before sliding to the top of your former skull.  While it should be horrifying, your mind can only think of thrusting, thrusting, always thrusting, the routine broken only by occasionally being sucked upon...   A small corner of your mind sees what is happening, and - horrified - is able to perform the counterspell.",
//                TFMessage_80_Percent_1st = "Again, you become the giant cock.  Smaller - though at 24 inches, a bit ridiculous for even the most Brutish of men.  And as thick, black, leathery skin surrounds you from your former navel downwards, the skin above turning bright pink, mottled with dark splotches - your feet/balls growing far out of proportion to the most virile human's - your head becoming not rounded, but flattened - it is clear that you are not becoming a set of human genitalia.  Rather - you stand erect, proud to be a mighty horsecock, ready to penetrate any and all local orifices... slowly thrumming to yourself, liquid sloshing within your feet, the fantasy continues.  It is only with great reluctance that a small part of you is able to restore your normal form.  However, the burning lust within, feeling ready to build to a full-body orgasm, yet remains...",
//                TFMessage_100_Percent_1st = "Is good to be horsecock.  Good to be 18 inches long, good to have 6 inch testicles.  Good to throb, throb, throb, waiting for release that seem to never come.  Good to feel hollowed out, ready to be worn atop and replace lesser cock.  Good to feel sticky tendrils on bottom, ready to attach to cock-less human, share joy of cock with.  Good to throb.  Good to be horsecock.  Good to throb.  Good to penetrate.  Good to be worn...  these thoughts continue for quite some time, and it is only through sheer luck, a lifetime of arcane training, and your iron will that you are able to escape.  Though you find yourself exceedingly robbed of focus - mind consumed with lust, hands constantly running up and down your torso, tensing, releasing, tensing releasing... ",
//                TFMessage_Completed_1st = "Is good to be worn.  Good to be horsecock, yes, better to be used.  Good to plow succubus cunt.  Good to take bitch from behind.  Good to be sucked on by bimbo.  Good for owner to stroke all alone.  Good to throb.  Good to throb.  Good to throb.  Better to cum.  Best to be worn, share gift of horsecock with owner.  Good to be horsecock... good to be hard... good to throb... good to be horsecock.",
                        
//                TFMessage_20_Percent_3rd = "A single word, a single thought - it conveys such power, such beauty, such dominance, such sexual potency that you can hardly restrain yourself from shouting it from the rooftops.  'Horsecock!'  You need one, and you need one now!  Perhaps you shall grant it to another, soon feeling its mottled flesh thrusting deep within you... but no.  It is far better to give than to receive - and for that, you need a horsecock of your very own.  You pick a likely victim and loose the spell.  The changes at first are subtle - arms and legs tending to stick together, a loss in height, swelling of the two feet... but soon.  Soon your victim shall be your mighty tool, with which you can go forth and conquer...  but for now, they seem to have returned to normalcy.",
//                TFMessage_40_Percent_3rd = "The purpose of your spell is becoming clearer - but your victim does not quite seem to grasp the full situation.  Their body is starting to become cylindrical - limbs vanishing into flesh, neck lengthening as the face reshapes.  This could almost passs asss a sssnake transssformation... if it were not for their feet, each hugely swollen, nearly spherical looking, a full foot in diameter.  A shock of realization crosses their face, as they realize what the massive dick assaulting them has in mind - and they change back to their normal, boring form.",
//                TFMessage_60_Percent_3rd = "Most of the humanity is gone.  Your victim stands tall at 3 feet and 6 inches, most of their body a large, fleshy tube, geometry only broken by feet which now resemble proportionate-looking balls.  Even their face as largely vanished - the only remaining feature the mouth, and that has lost lips, teeth and tongue to migrate up to the top of the former skull.  Your victim seems - well, as they stand there, vibrating, you suspect they may be quite happy.  But it is not long before they manage to regain their humanity, thwarting your designs for the moment...",
//                TFMessage_80_Percent_3rd = "No human trace remains.  Sitting before you is a 24-inch cock - a bit large even for your tastes - twitching happily on the ground, liquid sloshing through its disproportionately large ballsack.  Twitching even further as black, leathery skin forms around its lower half, while its upper portions turn bright pink, broken with a grey mottled pattern.  Even the head loses it's definition, becoming flat, flared.  You are looking at a glorious horsecock.  You reach out, eager to claim your prize - and see that no, your victim was still able to escape.  But their eyes... by all that is unholy, you have never seen such lust-filled eyes...",
//                TFMessage_100_Percent_3rd = "You swear you hear a cry of joy as your victim is again transformed into the horsecock - equine features forming much more rapidly.  At 18 inches, it is still far too lar... you know what? No, it sure as hell isn't too large, it is just the perfect size for you.  You see its insides hollow out, perfect for inserting a lesser member, even as small, writhing tendrils extend from its base, looking ready to attach to a groin regardless of gender.  You reach for your new pleasure tool as it continues to throb, thinking of how much better life will be after its attachment - it spasms repeatedly at your touch, seeming to agree.  But just as you pick it up, you are forced to drop it as it rapidly increases in weight and size, resolving finally in the form of your opponent.  But given the way they are constantly rubbing their hands up and down their torso, constantly flexing and relaxing their entire body - you suspect you will not be horsecockless for long.",
//                TFMessage_Completed_3rd = "Such joy, such wonder, such magnificence.  A giant horsecock, all for you!  And not just any horsecock - it lies there, throbbing, and you are just aching to attach it to your own skin.  'Go ahead, try me on', the horsecock seems to say - 'We are going to have such FUN together... shall we start with a human bimbo, or maybe find a nice catgirl?'  And given its quivering as you reach out for it, and the intense throbbing as your fingers close around it, these may not be far at all from its own thoughts.  Though you suppose those thoughts might just as well be something like 'Must Fuck Must Fuck Must Fuck Must Fuck Must Fuck'... the mighty horsecock is inscrutable in its myriad ways.  Well... what are you waiting for?  Try it on already!",

//            },

//#endregion

//#region latex bra arrhae

//            new Form {
//                 dbName = "form_black_latex_bra_Arrhae",
//                FriendlyName = "Black Latex Bra",
//                Description = "You are now a tight fitting latex bra, ready to support your new owner's breasts. Every movement of their body sends a jolt of pure ecstasy through you.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_black_latex_bra_Arrhae",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Out of the corner of your eye, you see your enemy cast a spell at you. You attempt to dodge, and they only manage a glancing blow. You notice that your skin turns a little bit darker. However, you quickly shrug it off and return back to normal.",
//                TFMessage_40_Percent_1st = "Your opponent casts a spell at you, which turns your skin completely black. With little effort you manage to reverse the spell.",
//                TFMessage_60_Percent_1st = "Your opponent hits you with another spell, and once again your skin turns black. Shortly afterwards, you touch your chest in shock to find out that it's beginning to turn to latex! You cast a reversal spell, but it worryingly takes a few seconds to take effect.",
//                TFMessage_80_Percent_1st = "Once again your opponent lands a hit a hit on you and your skin turns into black latex. In horror, you notice that your entire lower body is being absorbed into your chest. In a panic you cast a reversal spell, but afterwards your skin still retains a slight black latex sheen for a few seconds longer than you expect.",
//                TFMessage_100_Percent_1st = "Distracted by some of the previous transformations attempted on you, you don't notice your opponent casting a spell. It's a direct hit, and your skin rapidly returns to being black latex. In addition, your lower body is quickly absorbed into your chest, and despite your mental protests, your arms are forced to wrap around your back and hold hands. In horror, you feel your head lowering towards your chest. Just in time, you manage to fire off a reversal spell. However even though your body reforms, your skin retains its black latex sheen for about a minute afterwards. You're not sure if you can survive another hit like that.",
//                TFMessage_Completed_1st = "Still distracted by your thoughts, your opponent lands a direct hit. Your skin rapidly returns to being black latex. In addition, your lower body is quickly absorbed into your chest, and their arms are once again forced to wrap around your back and hold hands. As your head starts lowering towards your chest you prepare to fire off a reversal spell. A quick moment of wondering what it would feel like to hold up someone else's breasts distracts you for too long, however! A few short seconds later, you feel your chest hollowing out, finishing your transformation into sexy black latex bra. As you fall to the ground, you realize that you have a craving to do nothing but support someone's breasts.",

//                TFMessage_80_Percent_1st_M = "Once again your opponent lands a hit a hit on you and your skin turns into black latex. In horror, you notice that your entire lower body is being absorbed into your chest. Suddenly you feel two mounds forming on your chest. You have breasts! Cupping your new breasts in a panic you cast a reversal spell, but afterwards your skin still retains a slight black latex sheen for a few seconds longer than you expect.",
//                TFMessage_100_Percent_1st_M = "Distracted by some of the previous transformations attempted on you, you don't notice your opponent casting a spell. It's a direct hit, and your skin rapidly returns to being black latex. In addition, your lower body is quickly absorbed into your chest quickly reforming into two large breasts. Before you can worry about your breasts however, your arms are forced to wrap around your back and hold hands. In horror, you feel your head lowering towards your breasts. Just in time, you manage to fire off a reversal spell. However even though your body reforms, your skin retains its black latex sheen for about a minute afterwards. You're not sure if you can survive another hit like that.",
//                TFMessage_Completed_1st_M = "Still distracted by your thoughts, your opponent lands a direct hit. Your skin rapidly returns to being black latex. In addition, your lower body is quickly absorbed into your chest rapidly reforming into two large breasts. Once again, your arms are forced to wrap around your back and hold hands. As your head starts lowering towards your expansive breasts you prepare to fire off a reversal spell. A quick moment of wondering what it would feel like to hold up breasts like these delays you for too long! A few short seconds later, you feel your chest hollowing out, finishing your transformation into sexy black latex bra. As you fall to the ground, you realize that you have a craving to do nothing but support someone's breasts.",
                        
//                TFMessage_20_Percent_3rd = "You cast your spell at your victim with a glancing blow, and their skin turns a little bit darker. However, they quickly shrug it off and return back to normal.",
//                TFMessage_40_Percent_3rd = "You cast your spell at your victim, and their skin turns completely black. With little effort however, they reverse the spell.",
//                TFMessage_60_Percent_3rd = "You land a hit on your victim, and their skin turns black. Shortly afterwards, they touch their chest in shock to find out that it's beginning to turn to latex! With a worried look on their face, they cast a reversal spell, but it takes a few seconds to take effect.",
//                TFMessage_80_Percent_3rd = "You land a hit on your victim, and their skin turns into black latex. Their entire lower body starts getting absorbed into their chest. In a panic, they cast a reversal spell, but afterwards their skin still retains a slight black latex sheen for a few seconds longer than expected.",
//                TFMessage_100_Percent_3rd = "You land a direct hit on your victim, and their skin rapidly returns to being black latex. In addition, their lower body is quickly absorbed into their chest, and their arms are forced to wrap around their back and hold hands. As their head starts lowering towards their chest in horror, they just manage to fire off a reversal spell. However even though their body reforms, their skin retains its black latex sheen for about a minute afterwards. One more cast should do it, you think to yourself.",
//                TFMessage_Completed_3rd = "You land a direct hit on your victim, and their skin rapidly returns to being black latex. In addition, their lower body is quickly absorbed into their chest, and their arms are once again forced to wrap around their back and hold hands. As their head starts lowering towards their chest in horror, they attempt to fire off a reversal spell, but they are too slow!  A few short seconds later, their chest hollows out, leaving behind sexy black latex bra ready to support someone's breasts.",

//                TFMessage_80_Percent_3rd_M = "You land a hit on your victim, and their skin turns into black latex. Their entire lower body starts getting absorbed into their chest, out of which two breasts start forming. Cupping their new breasts in a panic, they cast a reversal spell, but afterwards their skin still retains a slight latex sheen for a few seconds longer than expected.",
//                TFMessage_100_Percent_3rd_M = "You land a direct hit on your victim, and their skin rapidly returns to being black latex. In addition, their lower body is quickly absorbed into their chest, rapidly growing into two large breasts. Before they can do anything else, their arms are forced to wrap around their back and hold hands. As their head starts lowering towards their chest in horror, they just manage to fire off a reversal spell. However, their skin retains its latex sheen for about a minute, and their breasts slowly deflate over that same time period. One more shot should do it, you think to yourself.",
//                TFMessage_Completed_3rd_M = "You land a direct hit on your victim, and their skin rapidly returns to being black latex. In addition, their lower body is quickly absorbed into their chest, rapidly reforming into two large breasts. Once again, their arms are once again forced to wrap around their back and hold hands. As their head starts lowering towards their chest in horror, they attempt to fire off a reversal spell, but they are too slow!  A few short seconds later, their chest hollows out, leaving behind sexy black latex bra ready to support someone's breasts.",

//            },

//#endregion

//#region straight tie

//              new Form {
//                 dbName = "form_straight_tie_CCWS",
//                FriendlyName = "Smart looking Tie",
//                Description = "Long, silky and soft. That's your form now. Made to wrap around your owner, and (if your lucky!) even hang between the mounds on their chest... You draw looks, and it makes you feel warm. Filled with glorious purpose! But better still is when your owner touches you - The spell it seems, had the appealing side effect of increasing your sensitivity ten fold. Every stroke and touch sending a fire of sensation through you!  As a tie, your know that you will draw eyes to your owner - Giving them an air of authority if they wear a shirt, and a more... Distracting allure if they don't. As such, your owner will find it harder to escape from their foes; though their foes may also find it more difficult to avoid your owners attacks.",
//                Gender = "female",
//                TFEnergyRequired = 76,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_straight_tie_CCWS",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "The spell strikes you and you feel a strange wave of lightheadedness. Your body feeling strangely insubstantial... Shaking your head, you dismiss the effects and refocus. It'll take more than that to knock you down!",
//                TFMessage_40_Percent_1st = "The spell strikes again, and once more that feeling of lightness creeps through you. Your skin starts to shimmer a little, a strange sensitivity building as a red pigment spreads from the point of impact. You watch it, mesmerized for a second before once more refocusing and reversing the changes. Whatever this was, you had to end it quickly!",
//                TFMessage_60_Percent_1st = "As the spell strikes you once more, you feel your whole body shudder pleasurably. The red pigment has now completely covered your skin, the texture changing to that of... Silky fabric? So... They were trying to make you an item of clothing then. The thought striking you just as you feel yourself sinking to the ground, your legs seemingly failing to keep you upright as they fold underneath you - Bones and flesh now a soft, malleable fabric. The changes are quickly shed, with a little effort, but the fearful knowledge that it's getting worse is evident.",
//                TFMessage_80_Percent_1st = "Once again the spell strikes, once again your sinking to the ground. Your legs merge seamlessly with each other as they flatten - an almost rectangular ribbon of red silk extending from your waist. You barely notice the fact that you seem to have lost a few feet in height, shaking with the pleasure the movement of your sensitive fabric-like skin brings, a silly smile spreading as you contemplate simply collapsing... With a groan, however, you realize you can't give in just yet - Reversing the changes once more and standing to your feet shakily. ",
//                TFMessage_100_Percent_1st = "As the spell comes your way once more, you can't even bring yourself to try and dodge. Welcoming the bliss of your legs merging, shrinking down as you start to flatten to the floor. The change is moving up now, racing through your torso as your arms shrink away. As your head begins to flatten, you can feel it reshaping to a triangular 'tongue', seamlessly joining your shoulders. The pleasure wracking your form is intense - And it's only just before your mouth is sealed away that your manage to mutter a counterspell. Even so, you lie there for a while, adrift on a sea of pleasure before you can return to the fight. Knowing you might not be able to resist next time...",
//                TFMessage_Completed_1st = "An idle part of your mind knows you should be worried as the spell strikes once more. You should be disappointed, or dismayed, or something that you've lost. But that part is soon quashed by the rapid flow of the pleasurable sensations once more. Your whole form rippling as you shrink down to collapse into yourself, finally lying there as the perfect, red silk tie. You only hope your owner will wear you soon, a thrill of excitement filling you at the thought of the touch of your skin against theirs!",
                        
//                TFMessage_20_Percent_3rd = "The first hit you score is perhaps a little disappointing. Your opponent simply shaking a little, a far off look in their eyes before they shake off the effects and come for you again. ",
//                TFMessage_40_Percent_3rd = "You strike again, and your opponent once more seems in a far off daze. Slowly looking at the point of impact, and the spreading red silk your spell creates. Alas, before it gets too far they manage to regain their senses, but you note a look of worry in their gaze that wasn't there before.",
//                TFMessage_60_Percent_3rd = "At your next strike, your opponent moans a little in pleasure! Already, they're shrinking down - Their legs losing substance and folding away underneath them as the silk fabric spreads across their form. The worry in their features as they return to normal is more pronounced now - But return they do.",
//                TFMessage_80_Percent_3rd = "Again you strike, and once more they start to collapse down as their legs begin to merge into a single long ribbon... Or rather, the tail of the tie you'll soon have hanging down your chest! A silly smile spreads across their face, the mental changes obviously kicking in - And their counterspell seems to take a toll on them, fatigue and distress evident",
//                TFMessage_100_Percent_3rd = "A thrill of excitement fills you as you score another hit, knowing it could easily be the last. The change takes them quickly, your foe moaning in pleasure as they sink to the ground. Arms, legs, torso all lost into the silky red ribbon - Even their facial features begin to sink away! Alas, before they succumb entirely, their vanishing mouth mutters the counterspell and they return to normal. But it's clear from the way they simply lie there for a while that they can only last for one or two more hits...",
//                TFMessage_Completed_3rd = "A grin of triumph spreads across your face as you land the final blow, your opponent at last giving in to their new desires as they collapse into their new form. You walk over and run them through your hands with a soft laugh - Seems you were right! A living tie is infinitely better than any 'normal' substitute...",

//            },

//#endregion

//#region athletic sneakers
//             new Form {
//                 dbName = "form_athletic_sneakers_PsychoticPie",
//                FriendlyName = "Athletic Sneakers",
//                Description = "You're a pair of sneakers. The purpose of your existence is to provide a comfortable place for a person's feet to rest. Unfortunately, since you used to be human, all of your senses are still intact; better hope your owner is hygienic.",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_athletic_sneakers_PsychoticPie",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You're suddenly hit by the urge to open your mouth wide, and look up towards the sky. 'Strange...' you think to yourself, as you easily reverse the effects of the spell.",
//                TFMessage_40_Percent_1st = "You open your mouth wide once again, and you begin to bloat. You reverse the effects of the spell without a second thought.",
//                TFMessage_60_Percent_1st = "You find yourself with your mouth open, staring at the sky, and with a bloated body yet again. You panic as the world begins to shrink around you, your limbs start becoming thin and stringy, and your skin becomes leathery. 'What I wouldn't give to have someone's sweaty foot in mouth right now...' you think to yourself. It takes a bit of effort, but you manage to reverse the effects of spell once more.",
//                TFMessage_80_Percent_1st = "Again, you transform into a shrunken, bloated, open mouthed, leathery... thing. This time, your limbs fully become strings: or rather, shoelaces! You're being transformed into shoes! 'I'd love for someone to ram me into the ground all day with their feet!' you think to yourself as you feel your back being transformed into an outsole. With a great deal of effort and struggle, you manage to stop and reverse the effects before things get out of hand.",
//                TFMessage_100_Percent_1st = "You're transformed yet again; you look almost identical to sneakers now. You scream in agony as your chest is torn open, becoming the throatline, as your inner organs shape themselves to become to the tongue. 'Feet... sweat... feet... feet...' you think to yourself. Your body pierces itself several times, creating convenient eyelets for your new shoelace limbs to go through. Through a combination of skill, willpower, and dumb luck, you're able to just barely reverse the effects of the spell. You doubt that you could do so again...",
//                TFMessage_Completed_1st = "You collapse as the spell connects, and the previous changes occur yet again. Your shoelace limbs slip nicely through your new eyelets, and tie themselves into a nice knot, much to your suffering. Your mouth expands outwards greatly, allowing ample room for a foot to slip inside. The bottom of your body is curved and expanded outwards as well. You black out for a split second, and suddenly, there's another shoe next you! You quickly realize that you can sense everything the other shoe senses. It feels like you have two eyes, two noses, two mouths... you're two shoes; you were duplicated. Your insides are coated with some nice, comfy material, leaving you helplessly trapped in this state, as your new owner comes to collect you. Looks like you'll be spending the rest of your days with feet in your mouth...",
                        
//                TFMessage_20_Percent_3rd = "Your victim opens their mouth wide, and looks up towards the sky. You chuckle as they reverse the effects of the spell.",
//                TFMessage_40_Percent_3rd = "Your victim opens their mouth wide once again, and they begin to bloat. They reverse the effects of the spell fairly easily.",
//                TFMessage_60_Percent_3rd = "Your victim opens their mouth, stares at the sky, and bloats yet again. A look of panic spreads across their face as they begin to shrink. Their limbs start transforming into shoelaces, and their skin becomes leathery. After a bit of struggling, they manage to reverse the effects of the spell.",
//                TFMessage_80_Percent_3rd = "Again, your victim begins to transform into a shoe. You laugh as their limbs fully become shoelaces, and their back begins the transformation into an outsole. With a great deal of effort and struggle, they manage to stop and reverse the effects before things get out of hand.",
//                TFMessage_100_Percent_3rd = "Your victim is transformed yet again; they look almost identical to sneakers now. They scream in agony as their chest is torn open, becoming the throatline, and their inner organs shape themselves to become to the tongue.  Their body pierces itself several times, creating convenient eyelets for their new shoelace limbs to go through. You watch, amazed and dumbfounded, as they're able to just barely reverse the effects of the spell. You doubt that they could pull that off again.",
//                TFMessage_Completed_3rd = "Your victim collapse as the spell connects, and the previous changes occur yet again. Their shoelace limbs slip nicely through their new eyelets, and tie themselves into a nice knot. Their mouth expands outwards greatly, allowing ample room for a foot to slip inside. The bottom of their body is curved and expanded outwards as well, as they duplicate their form, creating another identical shoe for their soul to inhabit. You smile as the spell finalizes, and walk over to collect your new sneakers. Looks like the days of your feet being sore and tired are over!",

//#endregion

//#region magic silk hat

//            },  new Form {
//                 dbName = "form_black_silk_top_hat_Zatur",
//                FriendlyName = "Black Silk Top Hat",
//                Description = "You are a stylish silk top hat. A posh accessory that any magician would love to have. With but a few magic words and the wave of a wand by someone, you can let loose doves, rabbits, or other stage magic paraphernalia. Or you could simply be worn to the most classy of venues, to hobnob with society's upper-crust as you are simply dapper. Anyone who possesses you can't help but be showy and dramatic as you make sure they, and you, are noticed.",
//                Gender = "female",
//                TFEnergyRequired = 65,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_black_silk_top_hat_Zatur",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "Falling for the old 'nothing up my sleeve' magician's patter, you aren't quick enough to dodge out of the way of the spell. A tingle runs through you as you fight the urge to ball up into a sitting position. You're not sure what they were trying to pull off, but it was rather showy. You almost feel like they should be on a stage with a beautiful assistant...",
//                TFMessage_40_Percent_1st = "Distracted again, the spell hits you. You curl up on the ground and tilt your head back, mouth opened wide as you feel that tingle once more. Everything seems larger. Did you shrink a little? You quickly reverse the spell and stand back up, slightly unnerved and unsure where that transformation is going...",
//                TFMessage_60_Percent_1st = "Entranced with the caster's hand movements, you don't even notice when they hit you with the spell once more. The tingle returns, stronger than before, as you sit down with your knees up to your chest and your arms curled around them, encircling you almost like a band. Your head tilts up again as your mouth opens wide, wider than you think possible, and you start to shrink once more. Shocked, you stand still for a moment before you reverse the transformation. Feeling a little triumphant, you almost shout 'Ta-da!' at your attacker...",
//                TFMessage_80_Percent_1st = "Again caught off guard, the spell washes over you. You go through the previous transformation as before, only this time you shrink to about the size of someone's head, and when you open your mouth, it feels like you're opening up completely inside as well, as if you're becoming hollow. You feel sadly empty at that and soon imagine what it would be like to be shown to an audience as empty and ordinary, only to suddenly have an adept hand pull a bouquet of flowers or a rabbit from the space. You could probably hold all manner of things as...a....hat? Wait, what?! This sudden realization snaps you out of your daydream and you quickly reverse the spell. That was too close.",
//                TFMessage_100_Percent_1st = "The spell hits you again, a sense of lightness and smallness envelop you as you once again shrink into a classy looking top hat. And once again you feel empty inside. If only you were up on stage, letting doves fly out of you. Or only if someone would wear you along with a fancy looking cane to a party. You would look wonderful. Dignified. Posh, even. Just like a silk top hat should look like. But a voice in your head suddenly shouts how wrong that is, and you reflexively counteract the spell. Returning you to your boring human body that wouldn't be in style in any season...",
//                TFMessage_Completed_1st = "The razzle dazzle of the spell is just too much to ignore and you take it full blast, feeling your body compact and shrink. Soon you're entirely a stylish looking top hat, ready for a rabbit or two to leap out of you or a night on the town. Your fashionable black silk is contrasted nicely with a white hatband near your brim. You can't wait for someone to pick you up and wear you. And if they're blue and don't know where to go, you know just where they can go where fashion sits...",
                        
//                TFMessage_20_Percent_3rd = "You quickly say 'There's nothing up my sleeve' to get the target to look at your one arm as you use your other hand to hit them with this showy spell. They stand there a moment as the magic passes over them. With all the glitz and glamour, they're suitable impressed, but unconcerned as it seems to do little else to them.",
//                TFMessage_40_Percent_3rd = "Using more misdirection, you hit them with the spell once more. They curl up into a sitting position, head back and mouth opened wide. They seem to shrink a little before they notice what's happening and counter the spell, looking at you nervously...",
//                TFMessage_60_Percent_3rd = "You have them eating out of your hands, so to speak, and hit them with the spell before they even realize what you're up to. They sit down once more, with their knees up to their chest and their arms curled around them, encircling themselves almost like a band. They tilt their head up again as it stretches wide open and this time they even shrink smaller than before. Just as their features start to fade into silk, they reverse the spell and pop back up as a human, with a slight smirk on their face. Almost like they are showing off a new trick with a flair...",
//                TFMessage_80_Percent_3rd = "Enthralled with your hand movements, it isn't too hard to hit your target with the spell again. They go through the previous transformation as before, shrinking to about the size of your head. Opening their mouth this time, you see it stretches down inside them, leaving an empty space just perfect for pulling out a bouquet of flowers or a rabbit from. They remain still for a moment as their skin turns to silk before suddenly reversing the spell and turning back to normal. They look extremely worried...",
//                TFMessage_100_Percent_3rd = "Hit with the spell once more, the target quickly shrinks and reshapes into a rather classy looking top hat. The hat sits rather forlornly on the ground, almost like it yearns for you to pick it up and have a stylish night on the town with it, a black cane, and a suit coat. But then it suddenly transforms back into a human. A human that seems to find their current form unfashionable and boring...",
//                TFMessage_Completed_3rd = "With a grand flair and panache of hand gestures, you hit them with the spell once more. This time they quickly, almost with an audible 'pop,' turn into the posh looking top hat as they settle on the ground. You pick them up as you hum a jaunty tune once sung by the likes of Fred Astaire, Gene Wilder, and even some Dutch singer. The black silk is very fashionable looking and the hat will most likely compliment any outfit. Especially one that makes you look like Gary Cooper...",

//            },

//#endregion

//#region fertility idol

//              new Form {
//                 dbName = "form_fertility_idol_varn",
//                FriendlyName = "Fertility Idol",
//                Description = "You have been transformed into a stone idol, resembling a very large, very pregnant woman, mere days away from giving birth.  Though you yourself are motionless, the essence of motherhood infuses you... you feel you may be able to share it with any who own you, making them look and feel more and more pregnant as time passes.  You are never able to move under your own power again... which does not bother you in the slightest, except for an occasional intense, unfulfillable desire to rub your swollen belly, feeling the life within you... ",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_fertility_idol_varn",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "The spell causes you to take on the form of a slightly chubby woman nearing middle-age.  You feel odd sensations from your vagina - it feels almost as if something is spurting into it?  It feels... right... to be full down there.  You bring your hands to your B-cup breasts, luxuriating in the sensations.  As soon as they stop... you realize what must be happening, and take steps to revert to your normal self.",
//                TFMessage_40_Percent_1st = "Again, you become her.  She looks... more voluptuous, this time.  Dark, tanned skin.  A bit chunkier all around.  Much wider hips.  A bloated belly.  Significantly shorter, under 5 feet.  Larger breasts, at least in the DD range, with large, brown nipples.  These last, you feel compelled to bring your arms to - even though you are more concerned about the belly - something seems... off, about its growth, as if it were not merely cellulite... for some reason, you feel sad as it vanishes as you cast the counterspell.",
//                TFMessage_60_Percent_1st = "Mmmm... you are her again.  Much shorter - barely four feet, though your overabundant curves will leave no one thinking you are only a girl.  Unnatural looking pale-yellow skin is on display everywhere, your clothes having vanished - your hands again seem able to do naught else but caress your expanding bosom, but your chunky legs, broad hips, and enormous ass seem to quiver below you.  Your belly has grown even further - you look truly obese.  But then... you think you feel movement coming from within it.  Oh no... you quickly cancel the effects.",
//                TFMessage_80_Percent_1st = "Ooooh... it feels good to be her again.  You question why you ever wanted to avoid motherhood - your stomach is truly massive now, you are surely ready to give birth any day.  Though... you think you feel quite a bit of independent motion in there - twins? triplets? more?  You sigh happily at the thought, before looking at the rest of your body.  You look even more... fertile - hips and ass impossibly extended, swollen breasts, larger than your head, with huge brown nipples set off on your red-orange skin.  The ground seems closer than ever - your head perhaps three feet away.  You try to feel up the rest of your body, and find that not only are your arms stuck to the top of your bosom, but your entire form seems almost immovable - the stiffness lingers even after you return.",
//                TFMessage_100_Percent_1st = "Aaaaah... you can't move at all anymore, but why would you when it feels this good?  You were right before - you are pregnant with many children - at least 5, maybe more judging from the sight of your belly - grown so far out, so full of life.  Your orange skin seems to be cracking open, though it does not hurt - an observer might take you for an aged, two-foot high statue.  Your hands cannot even feel your breasts - which have swollen beyond all feasible anatomy, engorged with milk they may never be able to express.  Your mind is nearly lost, luxuriating in the feeling of being so hugely pregnant... but you somehow are just barely able to return from the brink...",
//                TFMessage_Completed_1st = "You are again reduced to the fertility idol.  This time, you feel it - there is no escape.  But why would you want to escape?  Surely, losing your free will, your ability to move - oh, it is so worth it to feel this full of life, this complete.  There must be 8 children in there, all full-term - the only sensation you feel are their heartbeats, their occasional kicking and squirming.  Despite all appearances, you have never felt so alive!  Not only that, you feel you may be able to... share... your state with any holding you, spreading your pregnant nature to those who come into contact with you - letting others experience the gravid joy you now eternally feel... ",
                        
//                TFMessage_20_Percent_3rd = "Seeking a target for your spell, you come across a likely looking mage - you barely get a glance at them, before your magic has them changed into a pudgy-looking older woman.  She seems... occupied... as you realize the spell is making her feel the conception that will kick off her magical pregnancy.  But soon, your opponent recovers, regaining their prior form.",
//                TFMessage_40_Percent_3rd = "You barely get a glance at your target before your magic has them changed into a pudgy-looking older woman.  Again, your victim shifts towards your desired form.  She looks more... voluptuous, breasts in the double D range, hips, thighs, and ass all swelling... she has also lost quite a bit of height, and her skin is darkening, taking on the earth tones of the statue she will become.  She is most likely concerned with the 'weight-gain' around her belly - yet seems unable to move her arms from the top of her breasts to investigate.  It does not take too long for the spell to be broken... for now.",
//                TFMessage_60_Percent_3rd = "You barely get a glance at your target before your magic has them changed into a pudgy-looking older woman.  Her movements become more stiff, her unnatural-looking pale yellow skin starting to mark her fate.  She is now the height of a young girl... though her over-abundant curves make it clear she is very much a woman.  Other than the height, her whole body has expanded further - her breasts surpassing all but a few of her rivals, her belly swelling in a way that can no longer be mistaken for mere cellulite.  She seems to notice it too - she stiffly flinches, as if feeling a kick from there - and sharply breaks off your enchantment.",
//                TFMessage_80_Percent_3rd = "You barely get a glance at your target before your magic has them changed into a pudgy-looking older woman.  Her curves expand once again, as she continues to shrink, becoming less animate.  If she were carrying only a single child, one might think she was near full-term - but you know there are more, many more in there, and her gravid belly has quite a way to go.  Her mind seems to be slipping away... you see only a vacant grin on her face, as her immobile hands rub the upper bosom they seem attached to.  She seems... happy... so much so you are momentarily surprised when you see your old opponent standing before you again.",
//                TFMessage_100_Percent_3rd = "You barely get a glance at your target before your magic has them changed into a pudgy-looking older woman.  It is almost complete.  A two-foot tall neoltihic fertility idol stands before you, only a hint of flesh onto the orange-ish clay skin proclaiming an incomplete transformation.  The expression on the motionless face is one of pure bliss at feeling so alive, so full of life.  Her curves are beyond possibility - breasts so large you doubt she could stand, if not balanced by comically-wide hips, trunk-like thighs and ginormous rear.  Her belly is preposterously swollen - there are easily eight children in there, if not more.  You resolve to hit her again... make her complete... as your opponent claws their way back to mobility.  Though they stand there, rubbing their stomach, feeling the loss within...",
//                TFMessage_Completed_3rd = "You barely get a glance at your target before your magic has them changed into a fertility idol.  Carefully, you approach, picking it up - despite its size, it is not heavy.  You can feel the spirit of your opponent within.  It feels ecstatic to be overflowing with life... but also... giving?  In the mood to share.  You feel a strong tingling in your stomach, and a weaker force in your genitals, your breasts, your hips and rear.  If the feeling of pregnancy is not your goal, you suspect that keeping your opponent lying around may not be such a good idea.  But considering how happy she seems... perhaps you will keep her after all.",

//            },

//#endregion

//#region enchanted tree

//                new Form {
//                 dbName = "form_enchanted_tree_Sherry_Gray",
//                FriendlyName = "Enchanted Tree",
//                Description = "A young tree shades the area with a wide canopy of branches and lush foliage.  Its root are locked deep into the ground, here to remain.  The human form is still visible on the surface of its trunk, covered in bark but awake.",
//                Gender = "female",
//                TFEnergyRequired = 150,
//                MobilityType = "full",
//                PortraitUrl = "enchanted_tree.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_MoveActionPointDiscount = -999,
//                    FromForm_EvasionPercent = -40,
//                    FromForm_HealthRecoveryPerUpdate = 8,
//                    FromForm_ManaRecoveryPerUpdate = 8,
//                    FromForm_EvasionNegationPercent = 30,
//                 },

//                TFMessage_20_Percent_1st = "Your skin tingles with energy and your movement slows.  You feel a slow buildup of transformative energy that you easily brush off.",
//                TFMessage_40_Percent_1st = "Your skin hardens and your joints stiffen.  You feel your feet become stuck to the floor but you manage to fight off the magic before transformative effects manifest.",
//                TFMessage_60_Percent_1st = "Your skin hardens, thickening into a solid dark layer of bark.  Tendrils sprout from your legs, digging into the ground and locking you in place.  You feel trapped as you struggle against the encasement.  You concentrate your will and the bark cracks and falls from your skin.",
//                TFMessage_80_Percent_1st = "Your skin hardens, thickening into a solid dark layer of bark.  Tendrils sprout from your legs, digging into the ground and growing into mighty roots that hold you in place.  The trunk grows around you, partially engulfing your body in its mighty girth.  Branches begin to spread out over your head and form a patch of shade.  You use a mix of strength and will to break yourself out of the trunk.  The growing tree collapses under its own weight and the magic dissolves.",
//                TFMessage_100_Percent_1st = "Your skin hardens, thickening into a solid dark layer of bark.  Tendrils sprout from your legs, digging into the ground and growing into mighty roots that hold you in place.  The trunk grows around you, partially engulfing your body in its mighty girth.   Your body is lifted up off the ground mostly merged with the trunk of the tree.  You become aware of the branches that grow out into a full canopy over you and feel the breeze rustling against your leaves.  With one last focus you try to break free and with a loud crack you fall forward, free from the trunk.  The growing tree withers and dissolves into nothingness without a focus for its magic.",
//                TFMessage_Completed_1st = "Your skin hardens, thickening into a solid dark layer of bark.  Tendrils sprout from your legs, digging into the ground and growing into mighty roots that hold you in place.  The trunk grows around you, partially engulfing your body in its mighty girth.   Your body is lifted up off the ground nearly completely merged with the trunk of the tree.  You become aware of the branches that grow out into a full canopy over you and feel the breeze rustling against your leaves.  The light feels pleasant as it falls on your majestic canopy.  You stand tall and proud, knowing how strong you've become, here to remain eternally.",
                        
//                TFMessage_20_Percent_3rd = "Your spell strikes your opponent but they shake it off even before it begins to take effect.",
//                TFMessage_40_Percent_3rd = "Your spell manages to slow your opponent for a few moments.  They remain stuck in place until they fight off the effect.",
//                TFMessage_60_Percent_3rd = "Your opponent grows a layer of bark over their skin, stopping their movement.  Thick roots grow from their legs and dig into the ground, locking them in place.  But the magic fails and the bark shatters, falling from their skin.",
//                TFMessage_80_Percent_3rd = "Your opponent grows a layer of bark over their skin, stopping their movement.  Mighty roots grow from their legs and dig into the ground, locking them in place.  Tendrils grow upwards, widening into a think trunk that partially envelopes your victim's body.  As the tendrils continue upwards they spread out into a canopy of branches.  The tree collapses under its own weight as your opponent breaks free of the trunk and the magic dissolves.",
//                TFMessage_100_Percent_3rd = "Your opponent grows a layer of bark over their skin and mighty roots grow from their legs and dig into the ground, locking them in place.  A widening trunk partially envelopes your victim's body and lifts them up off the ground.  Tendrils snake upwards and spread out into a thick canopy of branches with lush leaves that rustle in the breeze.  Before their body fully merges into the trunk, they manage to struggle free, falling roughly to the ground.  The tree withers and dissolves after the loss of focus for the magic.",
//                TFMessage_Completed_3rd = "Your opponent grows a layer of bark over their skin and mighty roots grow from their legs and dig into the ground, locking them in place.  A widening trunk nearly completely envelopes your victim's body and lifts them up off the ground.  Tendrils snake upwards and spread out into a thick canopy of branches with lush leaves that rustle in the breeze.  Your opponent's body remains on the face of the mighty trunk, awake but trapped.  They will remain in this spot, casting a pleasant patch of shade.",

//            },

//#endregion


//#region schoolgirl top
//              new Form {
//                 dbName = "form_schoolgirl_top_PsychoticPie",
//                FriendlyName = "Schoolgirl Top",
//                Description = "You're a button up schoolgirl top. You exist to protect your owner's torso, and make them look conservative, yet feminine. For some reason, you just don't feel complete without a tie. If you're lucky, you won't spend too much time at the dry cleaners.",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_schoolgirl_top_PsychoticPie",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You feel stalwart and... soft? You reverse the effects of the spell.",
//                TFMessage_40_Percent_1st = "The strange feelings return, and some white material begins sprouting from the top of your head. Fortunately, you're easily able to reverse the effects of spell.",
//                TFMessage_60_Percent_1st = "Again, you're struck by feelings of soft bravery, and white material sprouts from the top of your head.  In addition, your insides begin to feel 'puffy'. You reverse the changes just as buttons begin forming on your stomach.  Are you being transformed into a shirt?",
//                TFMessage_80_Percent_1st = "The changes return, and this time they come on much more quickly.  The white material on the top of your head fully sprouts into a collar, and buttons appear from the top of your chest to the bottom of your stomach. You yelp in surprise as your head begins melting into your torso, and your skin starts to turn white (literally). It takes some doing, but you reverse the effects of the spell before things get out of hand. ",
//                TFMessage_100_Percent_1st = "The spell sweeps through you're body, and you end up in the same position as previously. You're beginning to resemble a shirt.  This time your head melts completely, and the collar that had sprouted previously melds itself nicely on top of your well buttoned torso. Your arms retract and shrink, molding themselves into a cylinder like shape, becoming nice short sleeves. 'I wish I could make someone look pretty and innocent. I hope my owner has a nice silk tie to go with me...' you think to yourself, as you realize what you're becoming: a schoolgirl top!  Just as your legs begin to retract, you muster up all of your willpower, and manage to reverse the effects of the spell. You know that you won't be human much longer if this keeps up...",
//                TFMessage_Completed_1st = "You submit to the magical assault as the spell connects to your chest, and your body begins to mold itself into a schoolgirl top once more. This time your legs fully retract into your torso, and the bottom of your remaining body is torn open, creating a nice opening for anyone who wishes to wear you. The last of your insides fuse into the shirt, and your nervous system is implanted into your new being.  With a few finishing touches, the spell finalizes, leaving you trapped as a cute, innocent, conservative little schoolgirl top. ",
                        
//                TFMessage_20_Percent_3rd = "Your victim wears an odd expression after the spell connects. They effortlessly reverse the effects.",
//                TFMessage_40_Percent_3rd = "The odd expression returns to their face, and shirt collar begins sprouting from the top of their head. Unfortunately, they're're easily able to reverse the effects of spell.",
//                TFMessage_60_Percent_3rd = "The collar sprouts once more upon your victim's head, and their expression becomes even more strange. They reverse the changes just as buttons begin forming on their stomach.  ",
//                TFMessage_80_Percent_3rd = "The changes to your victim return, and this time they come on much more quickly.  The white material on the top of their head fully sprouts into a collar, and buttons appear from the top of their chest to the bottom of their stomach. They yelp in surprise as their head begins melting into their torso, and their skin starts to turn white (literally). They manage to reverse the effects of the spell after struggling quite a bit.",
//                TFMessage_100_Percent_3rd = "Your victim finds themselves in the same position as before. They're beginning to resemble a shirt. This time their head melts completely, and the collar that had sprouted previously melds itself nicely on top of their well buttoned torso. Their arms retract and shrink, molding themselves into a cylinder like shape, becoming nice short sleeves. Just as their legs begin to retract, they muster up all of their energy, and manage to reverse the effects of the spell. You know that they won't be human much longer at this rate...",
//                TFMessage_Completed_3rd = "Your victim submits to the magical assault as the spell connects to their chest, and their body begins to mold itself into a schoolgirl top once more. This time their legs fully retract into their torso, and the bottom of their remaining body is torn open, creating a nice opening for anyone who wishes to wear them. The last of their insides fuse into the shirt, and their nervous system implants itself into their new being. With a few finishing touches, the spell finalizes, leaving them trapped as a cute, innocent, conservative little button up schoolgirl top.",

//            },

//#endregion

//#region schoolgirl bottom

//               new Form {
//                 dbName = "form_schoolbottom_bop_PsychoticPie",
//                FriendlyName = "Schoolgirl Skirt",
//                Description = "You're trapped in the form of a little pleated plaid schoolgirl skirt. Your job is to make your owner look cute and feminine while protecting their lower coxal regions. Given the fact that you're very short, the former task should be quite simple. You don't have a choice in the matter, so you should probably get used to this existence.",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_schoolbottom_bop_PsychoticPie",
//                 FormBuffs = new BuffBox{},

//                TFMessage_20_Percent_1st = "You're struck by the urge to tightly hug a young girl's hips. You shake your head as you reverse the effects of the spell.",
//                TFMessage_40_Percent_1st = "You're once again struck by the urge to embrace young feminine hips. Your skin begins to turn red, and diamond patterns start appearing all over your body. You reverse the effects of the spell just as you begin to shrink.",
//                TFMessage_60_Percent_1st = "Along with the usual hip hugging urges, you're also struck by a new urge to . . . enhance a young girl's appearance, and you feel your insides hollowing out. By this point you've shrunken significantly, and your skin is completely red, covered in a diamond pattern with stripes. 'What type of spell is this? Am I being transformed into clothing?' you think to yourself as you manage to reverse the effects of the spell with some difficulty.",
//                TFMessage_80_Percent_1st = "The spell strikes you once more. In addition to the earlier changes, your sides rip open, creating nice slits that go from your hips to your chest. You panic as you feel your limbs begin to retract, and your insides hollow themselves out again. Your retracted limbs shrink as they transform into lace, and you scream in pain as they puncture your body, lacing up the slits that were created in your sides. Your attacker must be trying to transform you into a schoolgirl skirt! Your head begins to retract inside of your torso just as you reverse the effects of the spell, with great difficulty.",
//                TFMessage_100_Percent_1st = "You curse yourself as the spell connects yet again. You're forced into your skirt-like form once more, and you scream as the agonizing changes sweep over you. Your head shrinks completely, and transforms itself into an elastic waistband. Your upper torso opens itself and expands outwards, and your new elastic waistband head melds itself onto the formation. This creates an opening that a thin young schoolgirl could easily slip her lower body into. You aren't sure how, but you're able to just barely reverse the effects of the spell. You know that your luck will run out very soon...",
//                TFMessage_Completed_1st = "You mentally collapse as your attacker connects with the spell. The changes reoccur, and this time they happen in nearly an instant. Your lower torso rips itself open and expands outwards, and your insides disappear completely. There's nothing left of your body now but skirt, and you look identical to one. Your thoughts are filled with nothing but how you can enhance your wearer's physical appeal, and keep their lower body safe from harm. Your owner walks over and picks you up, your new skirt body flowing freely in the air.",
                        
//                TFMessage_20_Percent_3rd = "Your victim stops moving and looks to the ground as they appear to contemplate something. They shake their head as they reverse the effects of the spell.",
//                TFMessage_40_Percent_3rd = "Your victim's skin begins to turn red, and diamond patterns start appearing all over their body. They reverse the effects of the spell just as they begin to shrink.",
//                TFMessage_60_Percent_3rd = "Your victim goes pale as their insides begin hollowing out. By this point they've shrunken significantly, and their skin is completely red, covered in a diamond pattern with stripes.  They manage to reverse the effects of the spell with some difficulty.",
//                TFMessage_80_Percent_3rd = "The spell strikes your victim once more. In addition to the earlier changes, their sides rip open, creating two nice slits that go from their hips to their chest. They panic as they feel their limbs begin to retract, and their insides hollow themselves out again. Their retracted limbs shrink as they transform into black lace, and your victim screams in pain as they're punctured by their new limbs, lacing up the slits that were created in their sides. Their head begins to retract inside of their torso just as they reverse the effects of the spell.",
//                TFMessage_100_Percent_3rd = "Your victim curses themselves as the spell connects yet again. They're forced into their skirt-like form once more, and they scream as the agonizing changes sweep over them. Their head shrinks completely, and transforms itself into an elastic waistband. Their upper torso opens itself and expands outwards, and their new elastic waistband head melds itself onto the formation. This creates an opening that a thin young schoolgirl could easily slip her lower body into. You aren't sure how, but they're able to just barely reverse the effects of the spell. You know that their luck will run out very soon...",
//                TFMessage_Completed_3rd = "Your victim mentally collapse as you connect with the spell. The changes reoccur, and this time they happen in nearly an instant. Their lower torso rips itself open and expands outwards, and their insides disappear completely. There's nothing left of their body now but skirt, and they look identical to one. Their thoughts are filled with nothing but how they can enhance their wearer's physical appeal, and keep their wearer's lower body safe from harm.  You walk over and grab your new item, watching as their skirt body flows freely in the air.",

//            },

//#endregion

//#region schoolgirl animate

//         new Form {
//                 dbName = "form_naughty_schoolgirl_christyd",
//                FriendlyName = "Naughty Schoolgirl",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "naughty_schoolgirl_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 6,
//                    FromForm_ManaBonusPercent = 2,
//                    FromForm_EvasionNegationPercent = 1,
//                    FromForm_SpellExtraTFEnergyPercent = 4,
//                    FromForm_SpellExtraHealthDamagePercent = 4,
//                    FromForm_SpellMisfireChanceReduction = -9,
//                 },
//            },

//#endregion

//               new Form {
//                 dbName = "form_the_pink_pulsar_PsychoticPie",
//                FriendlyName = "The Pink Pulsar",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_the_pink_pulsar_PsychoticPie",
//                 FormBuffs = new BuffBox{},
//            },

//              new Form {
//                 dbName = "form_liquify_Arrhae",
//                FriendlyName = "Goo Girl",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "goo_girl_b_meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_SneakPercent = -30,
//                    FromForm_MoveActionPointDiscount = -.35M,
//                    FromForm_HealthBonusPercent = -6,
//                    FromForm_HealthRecoveryPerUpdate = -.25M,
//                    FromForm_SpellExtraHealthDamagePercent = 8,
//                    FromForm_SpellExtraTFEnergyPercent = 8,
//                    FromForm_ExtraSkillCriticalPercent = 6,
//                    FromForm_CleanseExtraTFEnergyRemovalPercent = 1,
//                    FromForm_EvasionPercent = 10,
//                 },

//            }, new Form {
//                 dbName = "form_cateye_glasses_psychoticpie",
//                FriendlyName = "Cat Eye Glasses",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_cateye_glasses_psychoticpie",
//                 FormBuffs = new BuffBox{},
//            },

//            new Form {
//                 dbName = "form_kitsune_Elynsynos",
//                FriendlyName = "Kitsune Shrine Maiden",
//                Description = "A 3-tailed kitsune with silver fur, short silver hair, and blue eyes that glow slightly. This fox-like creature stands around 5' 6\" tall and has 4 pairs of breast, the top pair a perky D with each pair underneath decreasing in cup size. She wears her Shrine Maiden uniform partially open towards the top hinting at her sensual nature, but you also know to be wary lest you fall for one of her tricks.",
//                Gender = "female",
//                TFEnergyRequired = 90,
//                MobilityType = "full",
//                PortraitUrl = "kitsune_shrine_maiden_lux.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -8,
//                    FromForm_ManaBonusPercent = -7,
//                    FromForm_CleanseExtraTFEnergyRemovalPercent = 1.25M,
//                    FromForm_CleanseExtraHealth = -3,
//                    FromForm_EvasionPercent = -10,
//                    FromForm_SneakPercent = 7,
//                    FromForm_EvasionNegationPercent = 4,
//                    FromForm_SpellExtraTFEnergyPercent = 10,
//                 },
//            },

//               new Form {
//                 dbName = "form_miniature_horse_whiteflameK",
//                FriendlyName = "Miniature Carousel Horse",
//                Description = "You are a miniature orphaned carousel horse. Your body is a beautiful caricature of a real horse with a disconnected shining pole running through your torso. Your days spent thinking of new spells and your next victims are over. In fact, your days of thinking in general are over. At least, if you were a real horse, you'd be able to spend the rest of your existence in relative peace, grazing or mating.  Some element of the magic inside you makes you almost feel like a real horse and you have the urge to canter and neigh.  Even if you can't, at least, in your last conscious moments, you felt so majestic in form...even if immobile. Some vengeful part of your spirit remains within, longing to become animate, or to at least be a part of a carousel, but it is just a lifeless echo of what you once were.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_miniature_horse_whiteflameK",
//                 FormBuffs = new BuffBox{},
//            },

//            new Form {
//                 dbName = "form_Cheerleader_Haretia",
//                FriendlyName = "Chatty Cheerleader",
//                Description = "It's hard to be around this person for long.  With her chatty and cheerful attitude she clearly stands out in every crowd. But don't be a grump. That is like being honey for bees. She just can't help but to try cheering you up.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "cheerleader_meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 4,
//                    FromForm_ManaBonusPercent = -6,
//                    FromForm_HealthRecoveryPerUpdate = .7M,
//                    FromForm_SneakPercent = -30,
//                    FromForm_MoveActionPointDiscount = .15M,
//                    FromForm_EvasionPercent = 8,
//                    FromForm_SpellExtraHealthDamagePercent = -20,
//                 },

//#region living lingerie

//            },  new Form {
//                 dbName = "form_living_lingerie_PsychoticPie",
//                FriendlyName = "Living Lingerie",
//                Description = "You're a slightly transparent pair of panties with garter straps and back seam stockings. If you weren't feminine before, you're sure as hell feminine now. You exist to keep your wearer's legs nice and warm, and generate tons of lustful attention from the opposite sex. Being worn is usually better than the torturous boredom of sitting in a dark drawer for days on end, at least. Your sensation of arousal has been transferred to your stocking feet, so you're flooded with intense arousal whenever your wearer takes a step.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_living_lingerie_PsychoticPie",
//                 FormBuffs = new BuffBox{},
//            },

//#endregion

//#region stallion pet

//                 new Form {
//                 dbName = "form_hung_stallion_lexam_hachik",
//                FriendlyName = "Hung Stallion",
//                Description = "A beautiful stallion with a dark chocolate brown coat and a flowing black mane. He used to be human, you can tell by his essence, but now he is way happier as an animal, free of responsibility. Between his legs hangs a proud horse cock, the even puts that prosthetic horse cock to shame. He snorts and tosses his mane, he is powerful, horny, and ready to gallop!",
//                Gender = "male",
//                TFEnergyRequired = 75,
//                MobilityType = "animal",
//                PortraitUrl = "",
//                 BecomesItemDbName = "animal_hung_stallion_lexam_hachik",
//                 FormBuffs = new BuffBox{},
//            },

//#endregion
            
            
//            new Form {
//                 dbName = "form_fairy_familiar_Varn",
//                FriendlyName = "Fairy Familiar",
//                Description = "You have been fully transformed into a fairy - a 3-inch high, winged, glowing woman, bonded forever to your owner (or whoever claims you by rite of conquest.)  You don't mind - you are happy to serve.  Excited, even.  You flit around happily, pointing out the landscape to your master - 'Look! A stick! A tree!  A bimbo!  Hey! Listen!  A second stick!'  You'll never understand why master keeps putting you in the bottle after you've been so helpful... ",
//                Gender = "female",
//                TFEnergyRequired = 80,
//                MobilityType = "animal",
//                PortraitUrl = "fairy_familiar_Meddle.png",
//                 BecomesItemDbName = "animal_fairy_familiar_Varn",
//                 FormBuffs = new BuffBox{},
//            },

//#region big burly brute
//                new Form {
//                 dbName = "form_big_burly_brute_Varn",
//                FriendlyName = "Big Burly Brute",
//                Description = "Unlike the vast majority of the magically afflicted, this person is a man.  And what a man!  He stands at nearly seven feet tall, every inch of him bulging with muscle:  in any contest where physical strength mattered a whit, he would be a truly formidable opponent.  But he will be easy prey, as his face betrays not the slightest hint of intelligence.  What it does show is a look of overwhelming lust - and a glance down below at the enormous, inhumanly large bulge in his boxer shorts (the only article of clothing he is wearing) - suggests there is only a single thing on his mind.  Luckily for him, many of his afflicted opponents are all too willing to oblige.",
//                Gender = "male",
//                TFEnergyRequired = 70,
//                MobilityType = "full",
//                PortraitUrl = "manly_man_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -4,
//                    FromForm_ManaBonusPercent = -4,
//                    FromForm_SpellExtraHealthDamagePercent = 20,
//                 },

//#endregion

//            },  new Form {
//                 dbName = "form_were_leopard_alessandro",
//                FriendlyName = "Were-Leopardess",
//                Description = "This were-leopardess' multiple breasts jiggle ever so slightly as she prowls these city streets waiting for prey to enter walk into her excellent eyesight. The call of the hunt runs deep within her veins.  When this burning urge takes a hold of her, she heeds it beckon and follows it no matter where ever it may lead her.  This desire directs her actions and with her matching lust for a pack to call her own. Beware that you are not her next victim. ",
//                Gender = "female",
//                TFEnergyRequired = 74,
//                MobilityType = "full",
//                PortraitUrl = "wereleopard_luxianne.png",
//                 FormBuffs = new BuffBox{
//                     FromForm_MoveActionPointDiscount = .4M,
//                     FromForm_ManaBonusPercent = -15,
//                     FromForm_SpellExtraTFEnergyPercent = 6.5M,
//                 },

//            },

//          new Form {
//                 dbName = "form_maid_Ellipsis",
//                FriendlyName = "Mincing Maid",
//                Description = "There's no mistaking the woman before you as anything but a servant. Even without her short black skirt, tight, ruffled white apron, or feather duster, or balanced on those five inch platform heels, fishnet-wrapped legs pushing her high as they can but only managing 5'4', or puffy, short black sleeves, or any of he other hundred signifiers that might make a maid a maid, there's just something in the way she stands, the way she looks, the way she talks. An hour of carefully thought-out planning, for this girl, can be shattered by one firm order. Or a resolute suggestion, even. In fact, her willpower seems to drain itself, though she's very good at keeping herself from not being noticed when she doesn't, or others, do not wish her to be.",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "full",
//                PortraitUrl = "french_maid_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -6,
//                    FromForm_HealthRecoveryPerUpdate = -.1M,
//                    FromForm_EvasionPercent = -15,
//                     FromForm_SneakPercent = 50,
//                 },
//            },

//                  new Form {
//                 dbName = "form_ball_mask_Haretia",
//                FriendlyName = "Sparkly Masquerade Ball Mask",
//                Description = "You barely remember being human. The feeling of being this gorgeous mask muffled those memories a long time ago. You enjoy it when people gaze at the beautiful gems covering your masquerade ball style body of a mask. Some black and red feathers to your right show your playful side. And finally there is the magic you retain and use to protect your owner. You are perfect and you know it.",
//                Gender = "female",
//                TFEnergyRequired = 68,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_ball_mask_Haretia",
//                 FormBuffs = new BuffBox{},


//            },

//       new Form {
//                 dbName = "form_maid_dress_Budugu2004",
//                FriendlyName = "Maid Dress",
//                Description = "Once human, this fallen player met an opponent who was able to channel his naughtiest fantasies and turn him into a satin french maid dress. Tainted by the strong spell, this dress lust for cleaning and will make its owner a good industrious maid. But, as a servant of the manor, the maid knows she's at the bottom of the chain and suffers a strong willpower penalty.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_maid_dress_Budugu2004",
//                 FormBuffs = new BuffBox{},
//            },

//      new Form {
//                 dbName = "form_bondage_kitten_magazine_Haretia",
//                FriendlyName = "Bondage Kitten Magazine",
//                Description = "The bondage kitten magazine is unlike any other publication. If you look at the cover you see one of your defeated foes. Trapped in the picture, but not in time like other models. Your enemy is very much alive and looking at you with a pleading expression. How you would like to play with her, a petite twenty something with red hair and green eyes, covered in the most exquisite bondage equipment you could imagine. But the strangest thing happens when you open up the magazine. Before you can even look at the first page a wave of magic flows through you. When you then open the page you are astonished anew seeing and reading about a bondage session with you and your “bondage kitten” from your submissive foe's perspective. Even pictures are provided, for every highlight of your “session”. Reading about it makes you wish you could play with your “bondage kitten” yourself. Yet every time you read the stories inside you have a slight déjà vu as if as you already know what happens. At the end, when you close the magazine, you are greeted by your bondage kitten who seems now to be quite satisfied. It saddens you that the magic of the magazine takes some time to recharge but reading about these sessions manages to lift your spirit every time.",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_bondage_kitten_magazine_Haretia",
//                 FormBuffs = new BuffBox{},

//      },

//           new Form {
//                 dbName = "form_sparkly_cocktail_dress_Haretia",
//                FriendlyName = "Sparkly Cocktail Dress",
//                Description = "You are a sparkly cocktail-dress, designed to catch everyone’s eyes with your decorative gems and tailored to highlight your owner's figure with your pinkish-red velvet form.  If dresses had a queen, it would be you.",
//                Gender = "female",
//                TFEnergyRequired = 85,
//                MobilityType = "inanimate",
//                PortraitUrl = "",
//                 BecomesItemDbName = "item_sparkly_cocktail_dress_Haretia",
//                 FormBuffs = new BuffBox{},

//           },

//        new Form {
//                 dbName = "form_mouse_boy_Danaume",
//                FriendlyName = "Adorable Mouse Boy",
//                Description = "Quiet, adorable, and easy to miss, this little mouse is often mistaken for a girl when people do see him. Maybe because he tends to dress up like a girl, or maybe because he just looks so soft and sexy. It doesn't matter either way, this is one cute mouse, the perfect trap just waiting to strike.",
//                Gender = "male",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "mouse_boy_Danaume.jpg",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -10,
//                    FromForm_SneakPercent = 32,
//                    FromForm_EvasionPercent = 15,
//                    FromForm_SpellMisfireChanceReduction = 5,
//                 },

//        },

//       new Form {
//                 dbName = "form_Drow_Priestess_XanKitsu",
//                FriendlyName = "Drow Priestess",
//                Description = "A dark elf, a drow of lore. A wry smile adorns her lush ruby lips, long flowing silver hair framing her face, her dusky skin helping to hide her in the shadows. Her red eyes lock to yours as he runs her hand over her ample chest, and over her hips as if to beckon you to her sensual curves. Clad only samite spider silks and spider jewelry, she moves closer to you with almost a predatory gait. Pulling a thin whip from around her waist, she cracks it in your direction, as if to warn you of the painful pleasure she's about to whip from your body. ",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "full",
//                PortraitUrl = "lilath_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = 4,
//                    FromForm_ManaBonusPercent = -10,
//                    FromForm_HealthRecoveryPerUpdate = 1.55M,
//                    FromForm_ManaRecoveryPerUpdate = -2M,
//                    FromForm_EvasionPercent = 5,
//                    FromForm_CleanseExtraHealth = 2,
//                    FromForm_MoveActionPointDiscount = -.4M,
//                    FromForm_CleanseExtraTFEnergyRemovalPercent = 1.25M,
//                 },

//      },

//          new Form {
//                 dbName = "form_wolf_cub_Alessandro",
//                FriendlyName = "Wolf Cub",
//                Description = "You are a wolf cub.  You ferociously attack opponents for your owner.  Your speed and stealth make you a great ally for any mage's or witches' menagerie.  You follow your owner around lovingly and loyally.  Anytime you serve, you do so without any hesitation.  You can occasionally be seen carrying an item in your mouth when not full of an enemy.",
//                Gender = "female",
//                TFEnergyRequired = 70,
//                MobilityType = "animal",
//                PortraitUrl = "",
//                 BecomesItemDbName = "animal_wolf_cub_Alessandro",
//                 FormBuffs = new BuffBox{},
//},

//           new Form {
//                 dbName = "form_feminine_slut_boy_Lexam",
//                FriendlyName = "Feminine Slut Boy",
//                Description = "A shy looking boy, no older than 18. He looks timidly at everyone and refers to wear very girly clothes. He has widened hips and a girly bubble butt on his slender and hairless body. His short hair tops his head cutely as he keeps looking around, trying to decide whether he wants to keep fighting other magic caster, or find a master or mistress to treat him like a proper little pet.",
//                Gender = "male",
//                TFEnergyRequired = 68,
//                MobilityType = "full",
//                PortraitUrl = "femboy_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_SneakPercent = 30,
//                    FromForm_CleanseExtraHealth = 7,
//                    FromForm_SpellExtraHealthDamagePercent = -15,
//                    FromForm_SpellExtraTFEnergyPercent = -15,
//                 },

//           },

//           new Form {
//                 dbName = "form_Tribal_Girl_Lexam",
//                FriendlyName = "Tribal Girl",
//                Description = "An adorable, dark skinned girl with curly black hair and deep brown eyes. She looks around the city in wonder, speaking in a foreign and unknown language. She is dressed in a skimpy top wrapped around her perky breasts and a grass skirt, showing off the very bottom of her rounded bubble butt. She also carries various pouches of powder and darts for her blow gun, showing that although she is out of place and alone that she is still very confident in her shamanic arts.",
//                Gender = "female",
//                TFEnergyRequired = 75,
//                MobilityType = "full",
//                PortraitUrl = "jungle_girl_Meddle.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_HealthBonusPercent = -10,
//                    FromForm_ManaBonusPercent = 10,
//                    FromForm_ManaRecoveryPerUpdate = 2,
//                    FromForm_SpellExtraTFEnergyPercent = 10,
//                    FromForm_SpellExtraHealthDamagePercent = -10,
//                    FromForm_ExtraSkillCriticalPercent = 5,
                    
//                 },

//           },

//           new Form {
//                 dbName = "form_Dainty_Birdgirl_Medli",
//                FriendlyName = "Dainty Birdgirl",
//                Gender = "female",
//                TFEnergyRequired = 72,
//                MobilityType = "full",
//                PortraitUrl = "birdie_luxianne.png",
//                 FormBuffs = new BuffBox{
//                    FromForm_MoveActionPointDiscount = .25M,
//                    FromForm_HealthBonusPercent = -7,
//                    FromForm_ManaBonusPercent = -7,
//                    FromForm_ExtraSkillCriticalPercent = -2.5M,
//                    FromForm_SpellMisfireChanceReduction = -5,
//                    FromForm_EvasionNegationPercent = 50,
                 
//                 },


//  }, new Form {
//         dbName = "form_Frilly_Petticoat_Budugu2004",
//         FriendlyName = "Frilly Petticoat",
//         Description = "You are a white frilly petticoat, made to add some fluff to a dress or can simply be worn as a tutu. You make your owner feel rejuvenated, restoring a good amount of will each turn. But your owner should watch their step--the slightest bend will show some of their undies to anyone crossing their path. Your childish soul trapped into the frills might also distract your owner in the most critical of situations.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Frilly_Petticoat_Budugu2004",
//         FormBuffs = new BuffBox{},


//}, new Form {
//         dbName = "form_Fishnet_Tights_Zatur",
//         FriendlyName = "Fishnet Tights",
//         Description = "You find yourself changed into a sexy pair of fishnet tights made to encase the entire lower body of your owner. Despite your design being full of holes, when worn you can't help but feel every inch of the wearer's legs and lower torso. With every step they make, you stretch and slide along their legs, making your owner simply ooze sensuality. On top of that, you still feel a bit connected to your magical abilities and can provide them as an aid to your wearer.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Fishnet_Tights_Zatur",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Familiar_Feline_Blood_Knight",
//         FriendlyName = "Familiar Feline",
//         Description = "You are a sleek, svelte cat with glossy black fur and bright green eyes which almost seem to glow in the dark. Around your neck is a red collar, complete with a little bell which never seems to ring. You can feel their owner's will and desire, always nestled in the back of your head, and something in you compels you to serve these desires. Even that, though, has trouble curbing the feline mischievousness and curiosity you constantly feel.",
//         Gender = "female",
//         TFEnergyRequired = 72,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Familiar_Feline_Blood_Knight",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Cursed_Doll_Rust",
//         FriendlyName = "Cursed Doll",
//         Description = "Just being around this straw doll makes you feel sick to your stomach, you can't help but feel a bit of pity for whoever this object was originally but anything that was left of them is long gone. The object drains away all life around it and is near painful to touch but at the same time gives the one holding it a feeling of immense power.  Only a fool would hold this doll lest they be consumed by it.",
//         Gender = "male",
//         TFEnergyRequired = 72,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Cursed_Doll_Rust",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Leather_Whip_Christy_D",
//         FriendlyName = "Leather Whip",
//         Description = "You are a fearsome leather whip, often found in the hands of domineering women in leather. You wouldn't want to be on the wrong end of this toy... but you hope your victims don't find this out before you can streak against their soft exposed ass!",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Leather_Whip_Christy_D",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Magic_choker_Budugu2004",
//         FriendlyName = "Magic Choker",
//         Description = "You are a magical chocker, a delicate piece of jewelry that rests tightly on your owner's warm neck.  If someone were to try to take a closer look at the cameo on your front, they might see a Victorian lady moving and reacting to their presence--that lady being you!  You a powerful wizard trapped in this glass prison and are now worn as a magical artifact to protect your owner from spells.  Although you are sealed in with no means of escape of vengeance, you are loyal to your owner, and should a friend or foe of theirs lean in close, they might just catch an image of you winking at them one last time before a spell inanimates them too...",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Magic_choker_Budugu2004",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Maid_Headband_Budugu2004",
//         FriendlyName = "Maid Headband",
//         Description = "Although you have been turned into a cute little maid cap, you are far more than an accessory. Your poor soul trapped in this form is cursed with a strong obsessive-compulsive disorder for cleaning. Like a radar, you point out to your owner every single chores that need to be attended every room crossed. Some people might go crazy wearing you, staying a mindless maid forever even after removing you from their scalp.  If they want to benefit from the extra awareness you can procure,they must also accept their will slowly draining away.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Maid_Headband_Budugu2004",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_White_Tiger_Zatur",
//         FriendlyName = "White Tiger",
//         Description = "You find yourself no longer a member of Homo sapiens, but of the genus of Panthera, species of tigris. A tiger. And not just any tiger, but one of the rare breeds of the animal: your fur is colored white instead of the typical orange. While this might be a hindrance to hiding anywhere anytime besides winter, being a 500 pound giant cat with sharp teeth means you don't exactly have to worry about being unable to defend yourself.",
//         Gender = "male",
//         TFEnergyRequired = 60,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_White_Tiger_Zatur",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Magic_Slut_Ball_Varn",
//         FriendlyName = "Magic Slut Ball",
//         Description = "So, it has come to this - despite mostly keeping your mind, your body has been reduced to that of a 6-inch tall, overly sexed bimbo, trapped forever in a crystal sphere barely big enough for you to stand in.  Technically, if your master ignores you, you might be able to escape from their grasp, if not the sphere - your current best plan is to roll around the city, using your prison like a little hamster ball - but that seems unlikely at best.  Not only is your master taking great pains to hold onto you - but you can feel your will to escape - your will to do ANYTHING but lie here, either jilling off or milking your swollen breasts - slowly being siphoned off into your owner ...  You ask yourself - 'Can I ever escape?', but the outlook is hazy - perhaps you should ask again later.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Magic_Slut_Ball_Varn",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Randy_Satyr_Martiandawn",
//         FriendlyName = "Randy Satyr",
//         Description = "The classic goat-man of myth, a Satyr loves to sing, dance, and drink wine. He also loves to fuck. A lot. He is happiest when he is able to combine all of his favorite pastimes into a single event. Stubborn, in the manner of all goats, he has considerable willpower and can be very persistent when he sees something he wants… especially if it is something he can fuck. Yet the Satyr's unending quest for self-gratification interferes with the concentration required for proper magic. His spellcasting suffers as a consequence. Though nimble and quick on his cloven feet, his heady musk lingers long after he has left, limiting any stealthy inclinations he might have. Yet the distinctive aroma of his arousal also proves quite distracting for his prey.",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "satyr_martiandawn.jpg",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = 8,
//            FromForm_ManaBonusPercent = -8,
//            FromForm_HealthRecoveryPerUpdate = .4M,
//            FromForm_ManaRecoveryPerUpdate = -.6M,
//            FromForm_SneakPercent = -10,
//            FromForm_EvasionNegationPercent = 15,
//            FromForm_MoveActionPointDiscount = .2M,
//            FromForm_SpellMisfireChanceReduction = -10,
//         }

//}, new Form {
//         dbName = "form_Runic_Dildo_Blood_Knight",
//         FriendlyName = "Runic Dildo",
//         Description = "14 inches long, not counting its sword like handle, this dildo is made of no material you recognize. A strange black substance neither rubber, plastic, stone, nor steel, the dildo's shaft is covered in strangely glowing runes. Within the dildo you can feel the power of your new form, explosive, malevolent, and hungry. Draining resolve from any who dares to try and wield you, you channel it forth against those around you with hellish glee.",
//         Gender = "male",
//         TFEnergyRequired = 73,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Runic_Dildo_Blood_Knight",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Fennec_Fox_Elynsynos",
//         FriendlyName = "Fennec Fox",
//         Description = "Your huge batlike ears swivel around searching out for sounds of any potential threats. Nose tracing the ground you detect many of the different scents of the city and follow the ones you find interesting. Your body is small, but you feel full of energy and not at all weak. The black tipped tail gently swaying as you continue to search the city for someone to care for you.",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Fennec_Fox_Elynsynos",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Female_Dryder_JBovinne",
//         FriendlyName = "Web Widow",
//         Description = "You are a dryder, a creature of nightmares.  You are half beautiful seductress and half giant spider.  You feed off the fear and ecstacy of those around you... and this town has so much food.  You have the speed to chase down quarry, or the stealth to be the perfect ambush predator, but your added size makes you an easy target when cornered.  Your constant feeding keeps you resupplied, but your need to hunt allows little concentration on anything else.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "dryder_jbovinne.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -4,
//            FromForm_SneakPercent = 10,
//            FromForm_EvasionPercent = -5,
//            FromForm_CleanseExtraHealth = -1,
//            FromForm_ExtraSkillCriticalPercent = 5,
//            FromForm_SpellExtraTFEnergyPercent = 10,
//         }

//}, new Form {
//         dbName = "form_Gyrating_Temptress_redneckdemon",
//         FriendlyName = "Gyrating Temptress",
//         Description = "This stunning creature may seem to have been made for sex, but it would be more accurate to say that she was made for ornamentation.  Looking and acting sexy turns her on, but nothing revs her motor quite like getting naked.  She just can't help herself whenever she hears the music play and sees hands go up with money in them; she has to dance, to titillate and arouse.  She has to strip off her clothes and show off her sexy body in any way she can, for her own pleasure as much as for others.  Now, that's not to say she never gets so carried away with her performance that she forgets sex isn't actually part of the deal...",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "stripper_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_ExtraSkillCriticalPercent = 16,
//            FromForm_HealthBonusPercent = -4,
//            FromForm_ManaBonusPercent = -4,
//            FromForm_EvasionPercent = -6,
//            FromForm_MoveActionPointDiscount = .15M,
//            FromForm_SpellExtraHealthDamagePercent = -6,
//            FromForm_SpellExtraTFEnergyPercent = -6,
//            FromForm_SpellMisfireChanceReduction = -3,
//         }

//}, new Form {
//         dbName = "form_Donkey_Dominatrix_LexamTheGemFox_&_Hachik0048",
//         FriendlyName = "Donkey Dominatrix",
//         Description = "Before you is a busty anthro jenny, her tight leather corset making her breasts jut forwards, her nipples barely showing over the hem of her corset. She is wearing little else besides a black thong and modified thigh boots that let her walk around on her digitigrade hooved legs. Her ears are are pierced along with a stud through her bottom lip. She stares at you as you look upon her and smacks her riding crop against the leather on her leg, making a loud crack. You know you better avoid such a stern mistress or you might just end up her well trained BDSM pet.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "donkey_dominatrix_danaume.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = 4,
//            FromForm_ManaBonusPercent = -8,
//            FromForm_ManaRecoveryPerUpdate = -1,
//            FromForm_SpellExtraHealthDamagePercent = 8.5M,
//            FromForm_SpellExtraTFEnergyPercent = 8.5M,
//            FromForm_ExtraSkillCriticalPercent = -10,
//         }

//}, new Form {
//         dbName = "form_Adorable_Donkey_Foal_LexamTheGemFox_&_Hachik0048",
//         FriendlyName = "Adorable Donkey Foal",
//         Description = "You are now an adorable little donkey foal. Your mind is filled with innocent curiosity and you want nothing more that to prance around and be adored by others!",
//         Gender = "female",
//         TFEnergyRequired = 90,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Adorable_Donkey_Foal_LexamTheGemFox_&_Hachik0048",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Wizard's_Wand_themorpher606",
//         FriendlyName = "Wizard's Wand",
//         Description = "You are a magic wand, a thin black stick with a mesmerizing pink, glowing crystal.  If one were to look closely inside the crystal, they might still see your spirit drifting around inside staring back at them.  This wand increases its owner's spell effectivenss.",
//         Gender = "male",
//         TFEnergyRequired = 90,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Wizard's_Wand_themorpher606",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Captured_Souls_Alessandro_Stamegna",
//         FriendlyName = "Captured Souls",
//         Description = " You are a latex collar and you sit around your owners neck quite snugly. You refract the lights from the clubs strobes as well as the black-lights that surround some of the rooms. You have D-rings that encompass you from all sides allowing for leashes and chains to be run through their openings. A clasp lies on the back of your form with a lock hanging from it. You drain a small amount of will from your wearer allowing yourself to live in their pleasure eternally.",
//         Gender = "female",
//         TFEnergyRequired = 73,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Captured_Souls_Alessandro_Stamegna",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Vampire_Lord_Blood_Knight",
//         FriendlyName = "Vampire Lord",
//         Description = "This vampire lord stands tall and with an aristocratic bearing. His brow is heavy, over red eyes, and an aquiline nose, with high cheekbones betraying a long lineage of noble counts. His skin is pale, but his lips are oddly red, and they can feel long, sharp fangs hidden behind them.  He finds himself looking on the world with a new outlook, the gaze of a predator, and rejoicing in the idea of having others bent to his will as pets, playthings, and prey.",
//         Gender = "male",
//         TFEnergyRequired = 66.16M,
//         MobilityType = "full",
//         PortraitUrl = "vampire_Lux.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -5,
//            FromForm_ManaBonusPercent = -25,
//            FromForm_ManaRecoveryPerUpdate = -2.5M,
//            FromForm_SneakPercent= 15, 
//            FromForm_MoveActionPointDiscount= 0.1M,
//            FromForm_SpellExtraHealthDamagePercent= 15,
         
//         }

//}, new Form {
//         dbName = "form_Seductive_vampire_Foxpower93",
//         FriendlyName = "Seductive Vampire",
//         Description = "She have snow white skin that also have the temperature of it. Her curves are suave and full of sensuality able to hypnotize any human looking at it for too long. She is a perfect example of immortal beauty and she know it. She can't help, but grin showing her long canine when man and woman come at her, but it is only natural her supernatural charisma and beauty do this effect on mortals. But you can see in her eyes she is more curious how her followers taste then how they are good to bed... or maybe both.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "vampire_female_Lux.png",
//    FormBuffs = new BuffBox {
//            FromForm_HealthBonusPercent = -5,
//            FromForm_ManaBonusPercent = -25,
//            FromForm_ManaRecoveryPerUpdate = -2.5M,
//            FromForm_SneakPercent= 15, 
//            FromForm_MoveActionPointDiscount= 0.1M,
//            FromForm_SpellExtraHealthDamagePercent= 15,
//    },

//}, new Form {
//         dbName = "form_Feather_duster_Budugu2004",
//         FriendlyName = "Feather duster",
//         Description = "You are a feather duster. Indispensable to mincing maids, you can also be used to remove some magical transformation dust covering your owner. Each time the tip of your feathers touches a shelf, furniture or some delicate skin, it sends waves of pleasure through your handle. Some of your bliss may even be magically transferred to your owner, as they will develop a taste for dusting chores.",
//         Gender = "female",
//         TFEnergyRequired = 65,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Feather_duster_Budugu2004",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Austere_Angel_Adeviant",
//         FriendlyName = "Austere Angel",
//         Description = "This quiet girl is wearing a serene expression. With her unnaturally white skin, it almost looks like she would glow in the dark. All attempts at subtleness are blown out of the water by her wings and halo; an angel is walking among us. Most traces of who she once was seems to have been erased in the purification process, leaving a holy husk with little sense of self. ",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "angel_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -4,
//            FromForm_ManaBonusPercent = 7,
//            FromForm_SneakPercent = -15,
//            FromForm_MoveActionPointDiscount = .2M,
//            FromForm_ExtraSkillCriticalPercent = 12.5M,
//            FromForm_EvasionPercent = -20,
//         }

//}, new Form {
//         dbName = "form_Black_Suit_Jacket_Arbitrary_Hal",
//         FriendlyName = "Black Suit Jacket",
//         Description = "Being a well tailored suit, you can feel the precise contour of your owner's upper body, as you are flat but not tight against their form.  Your sole function, at this point, is to make your owner look and feel professional even when they are running around like a headless chicken.  The magic of your creation will protect you from mundane dangers like getting stained if someone spills a drink on you or if your owner gets too sweaty wearing you.",
//         Gender = "male",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Black_Suit_Jacket_Arbitrary_Hal",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Magic_Nursing_Bra_Purple_Autumn",
//         FriendlyName = "Magic Nursing Bra",
//         Description = "This new form feels totally alien. Not being even vaguely hominid, you can't really relate the sensation to anything. You are shaped and supportive fabric. Your body is now cups, your arms have become straps, your legs are bands that clasp together at what was once your feet. Your feel powerful still; able to make your owner lactate, but not much more.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Magic_Nursing_Bra_Purple_Autumn",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Kitten_Boi_Aingelproject667",
//         FriendlyName = "Kitten Boi",
//         Description = "While once human, this person has been turned into an adorable kitten boi. He has a slim, feminine body with a flat chest, widely flared hips, and a perky bubble butt perfect for groping or giving a light spank, topped with a long, furry cat tail, which sways with its owner, helping him keep perfect balance. His face has soft curves, and, combined with his body, he could be easily mistaken for a girl, if not for the small, round bulge on his tight bottom clothing, right between his smooth thighs. On his face is a pair of feline eyes, which track a nearby moth intently, batting at it lightly with his small, padded paw-like hands. Atop his head, sprouting from his short hair, sit a pair of cute, pointed cat ears, which rotate at sudden noises and also serve to indicate the cat boi's mood-at the moment, they are standing up, indicating that the catboi is happy, but if threatened or frightened, they will lay back against his head. Upon noticing you, the cat boi tilts his head inquisitively an mews cutely. Apparently, while the cat boi is capable of speech, meowing and making kitty sounds is his preferred method of communication...",
//         Gender = "male",
//         TFEnergyRequired = 65,
//         MobilityType = "full",
//         PortraitUrl = "adorable_kitty_boy_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_ManaBonusPercent = -6,
//            FromForm_SneakPercent = 5,
//            FromForm_SpellMisfireChanceReduction = -3,
//            FromForm_MoveActionPointDiscount = .25M,

//         }

//}, new Form {
//         dbName = "form_Unimaginably_huge-breasted_woman_Swog",
//         FriendlyName = "Unimaginably Huge-Breasted Woman",
//         Description = "At first you think you're looking at a giant pair of disembodied breasts, but you soon see that there is a woman behind them, leaning on top of them. Her mammoth mammaries are so large they pin her to the ground, completely unable to move! When her body moves you can see the waves of momentum move out in ripples across her massive tits. You would think someone in her situation would be a little miffed at their immobility, but she just seems content to lay on top of her gargantuan chest and hug them, a wide smile on her face.",
//         Gender = "female",
//         TFEnergyRequired = 124,
//         MobilityType = "full",
//         PortraitUrl = "unimagineably_bbl_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_ManaRecoveryPerUpdate = 10,
//            FromForm_EvasionPercent = -100,
//            FromForm_ExtraSkillCriticalPercent = 8,
//            FromForm_SpellExtraTFEnergyPercent = 30,
//            FromForm_MoveActionPointDiscount = -999,

//         }

//}, new Form {
//         dbName = "form_Black_Silk_Pants_Arbitrary_Hal",
//         FriendlyName = "Black Silk Pants",
//         Description = "As a pair of black silk pants, you are obviously a garment of class.  Your understated elegance supplies any who would wear you with nice crisp lines while both preserving modesty and allowing the skin beneath you to breath.  A garment such as the one you have become is suitable for any of a variety of formal occasions including, but not limited to, weddings, funerals, and magical transformation conventions.",
//         Gender = "male",
//         TFEnergyRequired = 69,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Black_Silk_Pants_Arbitrary_Hal",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_BDSM_Pony_Girl_LexamTheGemFox",
//         FriendlyName = "BDSM Pony Girl",
//         Description = "Before stands a bound woman, wearing modified horse equipment. Her arms are tied behind her back and her feet are in special boots made to look like hooves. She shakes her tail, which is actually just a butt plug with a tail attached to it, and neighs through the bit in her mouth. Clearly she was once human but the way she tosses her mane and head, acting just like a normal horse would, it is clear her mind has been warped and twisted to where she is nothing more now than a human animal.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_BDSM_Pony_Girl_LexamTheGemFox",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Shiny_Robot_Girl_Faremann",
//         FriendlyName = "Shiny Robot Girl",
//         Description = "A shiny cute robot girl. She is rather pretty, but looking at her feels a little bit uncanny.  She has big breasts wich allow her to store more data.  Her shiny metal skin works pretty well on deflecting spells, but also due too the high amount of metal her own spells are weakened as well.  She runs on mana and is using it up rather quick instead of producing it.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "robotgirl_thurosis.png",
//         FormBuffs = new BuffBox{
//             FromForm_ManaRecoveryPerUpdate = -2.5M,
//            FromForm_SpellExtraHealthDamagePercent = -8,
//            FromForm_SpellExtraTFEnergyPercent = -8,
//            FromForm_SpellHealthDamageResistance = 9.5M,
//            FromForm_SpellTFEnergyDamageResistance = 9.5M,
//         }

//}, new Form {
//         dbName = "form_Mary_Janes_Christopher",
//         FriendlyName = "Mary Janes",
//         Description = "As a simple pair of black Mary Janes and bobby socks, you can't help but rub against your owner's feet. You're so sleek and feminine now, whoever you once were is gone, replaced by your new sexy outlook on life; making that plaid skirt practically worthless in covering your owner's rear.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Mary_Janes_Christopher",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Ioun_Stone_Aidyn_Bright",
//         FriendlyName = "Ioun Stone",
//         Description = "A small crystal light as a feather orbiting your new owner. Purple in color you feel energy surging through your body rather frequently, though when called upon by the concentration of your master, you'd find that energy being siphoned off to add to their own. The process leaves you a bit tingly but you are happy to fuel the owners needs!",
//         Gender = "male",
//         TFEnergyRequired = 100,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Ioun_Stone_Aidyn_Bright",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Red_Wavy_Wig_Kirshwasser",
//         FriendlyName = "Red Wavy Wig",
//         Description = "You feel beautiful and glamorous, and will be so eternally. Your own magic energy keeps you from getting dirty or losing form, assuring your owner is the fairest of them all.",
//         Gender = "female",
//         TFEnergyRequired = 70.00M,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Red_Wavy_Wig_Kirshwasser",
//         FormBuffs = new BuffBox{}

//},new Form {
//         dbName = "form_Valiant_Valkyrie_Blood_Knight",
//         FriendlyName = "Valiant Valkyrie",
//         Description = "You stand a statuesque woman, over six feet tall with raven-feather black hair falling down almost to your ass in luxurious ebony braids. Your pale skin and piercing blue eyes are almost like ice. Looking down you can see a finely toned body with firm, full breasts, and a plank stomach, above womanly hips. Those that think your feminine form shows a physical weakness would be fools, though, as you can feel a strength like fine-forged steel in every inch of your body. You find yourself striding forward confidently, fearlessly even, looking for the bravest warriors to dance steel, or spell, with.",
//         Gender = "female",
//         TFEnergyRequired = 72,
//         MobilityType = "full",
//         PortraitUrl = "Valiant_Valkyrie_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = 6,
//            FromForm_ManaBonusPercent = -5,
//            FromForm_EvasionPercent = -15,
//            FromForm_SneakPercent = -30,
//            FromForm_SpellExtraHealthDamagePercent = 8,
//            FromForm_SpellHealthDamageResistance = 8,
//            FromForm_SpellTFEnergyDamageResistance = 8,

//         }

//}, new Form {
//         dbName = "form_Blue_Chaffinch_Draggony",
//         FriendlyName = "Blue Chaffinch",
//         Description = "An anthropomorphic avian covered with a prominent blue-grey plumage though not including down his front which shades into a white going down his chest. His wings and tail feathers holding the same blue color and shaded black at the tip. Not only his feathers hold a blue tint but so do his feet, hands and beak. At his back rests two large wings which seem very capable of flight but being much heavier than a normal bird he relies more on his agile legs to get around. This doesn't seem to hinder him as his muscle mass seems a bit above average, especially for a bird.",
//         Gender = "male",
//         TFEnergyRequired = 74,
//         MobilityType = "full",
//         PortraitUrl = "blue_chaffinch_Lux.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -7,
//            FromForm_ManaBonusPercent = -5,
//            FromForm_SneakPercent = -18,
//            FromForm_EvasionPercent = 25,
//            FromForm_MoveActionPointDiscount = .15M,
//         }

//}, new Form {
//         dbName = "form_Milk_Cow_Sampleguy",
//         FriendlyName = "Milk Cow",
//         Description = "You feel... Content? Your're not sure. Your mind is so foggy and clouded that you can't really focus on anything for very long. In fact you find it impossible to think any complex thought. You know three things. You want to eat, your udders need to be drained of the countless gallons if milk every few hours, and you feel the urge to moo constantly. You vaguely remember that your were once human, but all you feel now is a satisfaction that comes from living a much simpler life.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Milk_Cow_Sampleguy",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Count_Cuddles_Berrie_Valentine",
//         FriendlyName = "Count Cuddles Plush Bear",
//         Description = "You have been turned into an adorable plush toy. Your mind is mostly intact, but you can't help but think of your captor as your beloved who has been taking care of you since you've been changed by a terrible curse. You feel nothing but affection for them and even in this form you are willing to lend them as much of your strength as you can; if that means you'd be together again one day.",
//         Gender = "male",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Count_Cuddles_Berrie_Valentine",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Platform_heels_Christopher",
//         FriendlyName = "Platform heels",
//         Description = "You have become a pair of five inch platform heels. You cannot move, but you still retain each of your senses, to be used whenever you wish, so it isn't all bad. The sheer pleasure of being worn outweighs any of the disadvantages however. Simply the act of existing as such a sexy pair of high heels is a state of permanent bliss, unceasing, removing any thought of becoming human again. ",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Platform_heels_Christopher",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Magical_Mannequin_plzTryMySpell",
//         FriendlyName = "Magical Mannequin",
//         Description = "This doll could easily be mistaken for an inanimate object, were it not for the magical rune glowing where the upper part of its face should be. Simply staring at the rune for too long is enough to make even the most willful mage fall into submission. Supposedly, it is this rune that compels the doll to move, eager to serve those around it.  Of course, one could always inscribe new instructions into the doll, although it is somewhat difficult to do so by magic, as its wooden body provides it with quite formidable magic resistance.  Whoever crafted this doll was thoughtful enough to provide it with an artificial vagina, providing it with yet more ways to please its masters.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "wood_mannequin_Lux.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -64,
//FromForm_SpellExtraTFEnergyPercent= 23,
//FromForm_SpellExtraHealthDamagePercent= 23,

//         }

//}, new Form {
//         dbName = "form_Cheerleader_Spankies_Zatur",
//         FriendlyName = "Cheerleader Spankies",
//         Description = "Normal spankies for cheerleaders are used to hide their panties from prying eyes while they kick and flip. You're not normal spankies. Small enough to be panties yourself, most St. Circe Cheerleaders sometimes don't even bother wearing anything else under their skirts, if they remember at all. When they do remember you get to hug their body with your tight pink curves and the words 'Spank Me' across their rears. An invitation that you can't just wait to happen.",
//         Gender = "female",
//         TFEnergyRequired = 69,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Cheerleader_Spankies_Zatur",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Cherry_Sucker_Christopher",
//         FriendlyName = "Cherry Red Sucker",
//         Description = "The average lollipop is quite small and only lasts a short while, but you are no ordinary lollipop. Long, red and proud, a penis shaped sucker for girls, or guys, who need to satisfy their oral cravings but no one's around to help them. You are eternal, your cherry flavour never fading and always ready to give your owner a tasty treat with your inner filling.",
//         Gender = "male",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Cherry_Sucker_Christopher",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Egyptian_Priestess_Pharlynx",
//         FriendlyName = "Egyptian Priestess",
//         Description = "This player is a subject of Ra, working to bring in fellow worshipers and to gain favor in his sight. She finds that throughout her time serving Ra, she is empowered by the powerful god. This woman is also graciously blessed with incredible beauty mixed with an underlying sense of power. Her mind is always in the moment and surveys each situation carefully, relying on the power of Ra to get her through each struggle.",
//         Gender = "female",
//         TFEnergyRequired = 65,
//         MobilityType = "full",
//         PortraitUrl = "egyptian_priestess_Lux.png",
//         FormBuffs = new BuffBox{
//            FromForm_ManaBonusPercent = 1.7M,
//            FromForm_HealthRecoveryPerUpdate = 1,
//            FromForm_SpellExtraHealthDamagePercent = -4,
//            FromForm_SpellExtraTFEnergyPercent = -4,
//            FromForm_CleanseExtraTFEnergyRemovalPercent = 2,
//         }

//}, new Form {
//         dbName = "form_Black_Leather_Shoes_Arbitrary_Hal",
//         FriendlyName = "Black Leather Shoes",
//         Description = "While not exactly enthusiastic at first about serving time as a pair of shoes for a crime you didn't know you were committing (that, in fact, you didn't know was illegal in the first place), you have certainly risen to the challenge.  You cushion your warden's every step, in the hope that you will be released either in consideration of your appeal, or else on good behavior.",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Black_Leather_Shoes_Arbitrary_Hal",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Bimbonic_Plague-Bearer_Varn",
//         FriendlyName = "Bimbonic Plague-Bearer",
//         Description = "You have seen this woman before - dozens, hundreds of her identical sisters are swarming the city, victims of the so called 'Bimbonic Plague'.  Each of them a giggling, strutting bimbo - most dressed in the skimpiest little outfits - all of them the very picture of femininity, with highly improbable curves.  Every single one flirting outrageously with those around her, offering up every kind of carnal delight.  You - and all but a few of the most foolish - know accepting her gift, becoming infected, would be a horrible idea - that she and her sisters are highly contagious, a mere drop of their fluids enough to transform those around her into yet another identical slutty bimbo.  And yet... some small part of you cries out to run to her, kiss her, join her, BECOME her...",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "bimbo_plague_bearer_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -20,
//            FromForm_SneakPercent = -35,
//            FromForm_EvasionPercent = -20,
//            FromForm_ExtraSkillCriticalPercent = 11,
//            FromForm_SpellExtraTFEnergyPercent = 50,
//            FromForm_SpellHealthDamageResistance = -10,
//         }

//}, new Form {
//         dbName = "form_Pocket_Dragon_Julian_Chance",
//         FriendlyName = "Pocket Dragon",
//         Description = "You are nothing less than a scale-clad, firebreathing dragon, straight out of myths and legend!  A symbol of strength and magical power, you are capable of inspiring awe in all who behold you!  Although, you would probably be a bit more awe-inspiring if you were a bit bigger.  Measuring in at 24\" from snout to tail, you're not really as imposing as you feel you ought to be.  On the other hand, you're the perfect size for sitting on someone's shoulder once they've tamed you.  Which, you're forced to admit, is probably closer to what they had in mind when they transformed you, anyway.",
//         Gender = "male",
//         TFEnergyRequired = 100,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Pocket_Dragon_Julian_Chance",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Cheerleader_Shell_Zatur",
//         FriendlyName = "Cheerleader Shell",
//         Description = "Full of team spirit and a cheerleader's chest, that's the wonderful life of a cheerleader shell and now it's all yours! You're bright and pink and stand out from a mile away, but that's the point after all. Cheerleaders are meant to be seen by all, and you love it when you're seen by all. Not only do you have the ability to protect the modesty of your owner, but you seem to be able to defend them a bit from spells others toss at them. After all, if they're no longer a cheerleader, they might stop wearing you!",
//         Gender = "female",
//         TFEnergyRequired = 64,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Cheerleader_Shell_Zatur",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Playful_Pussy_Rayner",
//         FriendlyName = "Playful Pussy",
//         Description = "Until used you reside in the form of a fleshy ball with a pussy vaguely aware of your surroundings, unable to move, but certainly able to feel. When worn your new body is alive, you are dimly aware of your fate and surroundings but most importantly of your owners needs. You can feel your folds, everything that enters you fills you entirely, and you know you are nestled between a warm pair of legs. You'd like to think you control them but their urges control you, you entire being is hot when they are, wet when excited, but mostly you find yourself eager to be filled... by anything. You twitch a muscle or two now and then to send a shiver of pleasure through yourself and your owner, reminding them not to ignore you. It's not a terrible fate, you will be well cared for and fed often. ",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Playful_Pussy_Rayner",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_High-Quality_Onahole_LexamTheGemFox",
//         FriendlyName = "High-Quality Onahole",
//         Description = "This player has been changed into a beautifully crafted pussy sex toy. They are very realistic both inside an out, even remaining warm and wet so she can be used at any time her owner wishes. Her transformation was a bit humbling, but now since she is only a pussy made to fuck the only thing on her mind is exactly that.",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_High-Quality_Onahole_LexamTheGemFox",
//         FormBuffs = new BuffBox{}


//}, new Form {
//         dbName = "form_Pig_Girl_themorpher606",
//         FriendlyName = "Pig Girl",
//         Description = "You are now a sexy pig girl, slightly pudgy with a barrel shaped body.  You have six sensitive breasts, a tail, ears, and a snout, but for the most part; you're still mostly human with a strange desire to eat slop off of the ground and roll in a puddle of mud.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "pig_girl_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_ExtraSkillCriticalPercent = 6,
//            FromForm_MoveActionPointDiscount = -.25M,
//            FromForm_SpellExtraTFEnergyPercent = 5,
//            FromForm_SpellMisfireChanceReduction = -7,
//         }
//}, new Form {
//         dbName = "form_Hedonistic_Fembot_Christine_Winters_(Arrhae,_ToniV_and_Meddle_for_proofing)",
//         FriendlyName = "Hedonistic Fembot",
//         Description = "She is a sleek and sexy technological marvel. Equipped with quantum gates and powered by magic she is capable of so much more than most of those two-bit calculators out there, rendering her capable of simulating true emotions that you would have trouble thinking her human if not for her purposely artificial exterior. Her curves and contours are perfection. Every panel crafted with machine grade precision down to the very pico meter, and painstakingly assembled. Arranged in such a fashion it expands an contract to mimic human muscle movements.  Her limbs are powered by latest state of the art servos and lightweight carbon nano-tube cables.  Every single motion produced by this unit is calculated and coordinated for maximum seduction. Whoever she may be before no longer matters as her new programming demands to be satisfied, whether to please or be pleased. You would do well not to catch the attention of those glowing orbs of hers.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "Fembot_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -5,
//            FromForm_ManaBonusPercent = -5,
//            FromForm_SneakPercent= -5,
//            FromForm_EvasionNegationPercent= 40,
//            FromForm_SpellHealthDamageResistance= -10,
//         }


//}, new Form {
//         dbName = "form_Muscular_Minotaur_Christopher",
//         FriendlyName = "Muscular Minotaur",
//         Description = "This mage has become something akin to the Minotaur of legend, part man and part bull. A towering pillar of masculinity and musculature, at first glance he may be considered stupid, merely an animal. This couldn't be further from the truth however, his thoughts slow and methodical, but deep and philosophically bent upon his situation. This is somewhat detracted however, as the sight of a woman will turn his mind to a single focus, as direct and impassable as the size of his member.",
//         Gender = "male",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "Minotaur_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = 8,
//            FromForm_HealthRecoveryPerUpdate = 1,
//            FromForm_SneakPercent= -10,
//            FromForm_EvasionPercent= -15,
//            FromForm_CleanseExtraHealth= 5,
//         }

//}, new Form {
//         dbName = "form_Tentacled_Terror_Varn",
//         FriendlyName = "Tentacled Terror",
//         Description = "Though no longer doing so through arcane arts, you are still able to happily assault the rival wizards of this town; a simple wave of a pseudopod from you causes many of your former opponents to run.  Though it must be said that a few are running TOWARDS you, more than eager to be \"licked\".  You may be free for a time, to wander the streets, hunting the elusive schoolgirl as your natural prey; but soon enough, one of your former enemies is going to tame you.  It won't be so bad; you'll be able to concentrate your attentions on your new owner.  And if you are commanded, perhaps on another special mage ...",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Tentacled_Terror_Varn",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Ice_Queen_Alessandro_Stamegna",
//         FriendlyName = "Ice Queen",
//         Description = "This person is an ice queen, they constantly chill the air around them  as they move around. Their multi-colored eyes make them get second glances often.  That, combined with their pale complexion and blue hair with it's green streaks in it, attracts quite a few people to follow their lithe frame.  Just be careful not to anger her or you will be trapped in her icy grip, giving her a nice ice sculpture for her home.",
//         Gender = "female",
//         TFEnergyRequired = 77,
//         MobilityType = "full",
//         PortraitUrl = "Ice_Queen_Meddle.png",
//         FormBuffs = new BuffBox{
//             FromForm_ManaRecoveryPerUpdate = 2.5M,
//            FromForm_ExtraSkillCriticalPercent= 15,
//            FromForm_MoveActionPointDiscount= -.17M,
//            FromForm_SpellHealthDamageResistance= -10,
//            FromForm_SpellTFEnergyDamageResistance= -10,

//            }

//}, new Form {
//         dbName = "form_Alraune_Foxpower93",
//         FriendlyName = "Alraune",
//         Description = "This person (if you can still call it a person) have green skin, vines as feet, a beautiful pink flower that is smelling sweet ornamenting her head and let not hide it she have a beautiful body able to lure all men close with B-cups breasts and hourglass curves, she is clearly what you would classify as an Alraune.  However you look at her she feel quite ditzy, close of nature and without a worry in the world. Why should she worry anyway? As long she have a sun over her head and water here and there everything will be alright, but if they feel danger their mesmerizing smell and thorns will surely help them get out of trouble. ",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "alraune_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -16,
//            FromForm_ManaBonusPercent = 20,
//            FromForm_ManaRecoveryPerUpdate = 5,
//            FromForm_SneakPercent= -25,
//            FromForm_MoveActionPointDiscount= -.7M,
//            FromForm_SpellExtraHealthDamagePercent= 10,

//         }

//}, new Form {
//         dbName = "form_Ruby_Red_Lipstick_Greg_Mackenzie_and_Fiona_Mason",
//         FriendlyName = "Ruby Red Lipstick",
//         Gender = "female",
//         TFEnergyRequired = 67,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Ruby_Red_Lipstick_Greg_Mackenzie_and_Fiona_Mason",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Tiny_Mouse_Girl_Estyz",
//         FriendlyName = "Tiny mouse girl",
//         Description = "You are an adorable tiny mouse girl, perfectly fitted for a regular human's pocket or shoulder. Your white furred body is almost entirely that of an upright walking mouse, safe for your humanoid face and hairstyle, and your clear, intelligent eyes. Even though you look way more like a mouse than a human in your current form, you're every bit as witty and smart as before, probably even more so. You're now somewhat shy in nature, though, and find a strange craving for cheese, sunflower seeds, and various other tidbits of food to stuff in your face.",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Tiny_Mouse_Girl_Estyz",
//         FormBuffs = new BuffBox{
         
//         }

//}, new Form {
//         dbName = "form_Willing_Wish-giver_Zatur",
//         FriendlyName = "Willing Wish-giver",
//         Description = "Under any other circumstances, you might consider the woman before you dressed for a costume party as some harem girl or slave. They seem to act the part with a hint of submissiveness in their posture and actions. That idea is broken by the strong magic you feel radiating off of them, along with the devilish smile they wear. Legends tell of the highly powerful magic of the djinn, of how they have no limits, but they need someone to command them to do anything. That isn't to say they can't be imaginative at times though. After all, it takes a creative mind to interpret commands in ways that might benefit them more than the one issuing them...",
//         Gender = "female",
//         TFEnergyRequired = 60,
//         MobilityType = "full",
//         PortraitUrl = "Willing_wish_granter_Meddle.png",
//         FormBuffs = new BuffBox{
//               FromForm_HealthBonusPercent = -10,
//                FromForm_ManaBonusPercent = 12,
//                FromForm_MoveActionPointDiscount= -.45M,
//                FromForm_SpellExtraTFEnergyPercent= 5,
//                FromForm_ExtraInventorySpace= 1,

//         }
//}, new Form {
//         dbName = "form_Practical_Transformations_and_the_Metaphysical_Realm_Varn",
//         FriendlyName = "Practical Transformations and the Metaphysical Realm",
//         Description = "This copy of Practical Transformations and the Metaphysical Realm is significantly thicker than when it arrived off the magical printing presses, signifying that it is host to the bound spirit of a rival mage.  Every flip of the page leads to a new illustrated image of what appears to be the same mage (or at least the same face; bodies differ considerably) trapped in a variety of sexually-charged situations.  You catch glimpses of angels, demons, faeries, furries, students, teachers, and various sorts of bimbos (over four kinds!) as you turn the pages; screams unidentifiable as either pleasure or pain issuing forth with each flip.  Not only that, but apart from the pleasure of tormenting your former rival, each page is inscribed with runes describing methods for making such transformations a reality. The knowledge of which may greatly enhance your magical skills; making it well worth the cost of infusing the book with a bit of your own mana to maintain its enchantment, and becoming a bit more vulnerable to changes yourself.  And finally, the book can be used to temporarily suck in a second rival, letting your captive pet have their way with them, making them more vulnerable to further transformations ...",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Practical_Transformations_and_the_Metaphysical_Realm_Varn",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Elf_Princess_JBovinne",
//         FriendlyName = "Elf Princess",
//         Description = "This player is an Elf Princess.  They have been raised from birth to be dignified and refined.  Always dressing in the most exclusive clothes, and their servants ensure they are never seen in public unless they are beautifully made up.  Of course, this life of obligation and responsibility has left them with a want inside that no amount of royal dinners can satisfy.  They want sex and will use their athletic and well-schooled body to get it any way they can.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "elf_princess_jbovinne.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -4,
//            FromForm_ManaBonusPercent = 5,
//            FromForm_SneakPercent= 5,
//            FromForm_EvasionPercent= 4,
//            FromForm_SpellMisfireChanceReduction= -4,

//         }

//}, new Form {
//         dbName = "form_Disembodied_Boobs_Swogrider",
//         FriendlyName = "Disembodied Boobs",
//         Description = "You've been turned into a big wobbly set of boobs, still made of flesh and everything! Sure, you don't have quite as much autonomy as when you had a body, but anyone lucky enough to wear you will find out how good your new life is! You meld perfectly into the flesh of whoever puts you on, becoming their new breasts, swaying and bouncing with every step they take. Any arousal either of you feel is transferred straight to the other, causing big, hard, sensitive nipples either way! And with your level of sensitivity, it's kind of hard for anyone not to want to touch you. You might be giving them telepathic compulsions to grab you every once and a while, but who doesn't get a morale boost after squeezing a nice pair of titties!",
//         Gender = "female",
//         TFEnergyRequired = 85,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Disembodied_Boobs_Swogrider",
//         FormBuffs = new BuffBox{},

//}, new Form {
//         dbName = "form_Mechanical_Man_themorpher606_(Kevin_Gates)",
//         FriendlyName = "Mechanical Man",
//         Description = "This spellcaster is now a terrifying robot, lacking all emotions, although it has a human mind. They are now very noisy, and can easily be noticed when approaching. It reminds you of a robot from the movies. ",
//         Gender = "male",
//         TFEnergyRequired = 90,
//         MobilityType = "full",
//         PortraitUrl = "mechanical_man_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -10,
//            FromForm_HealthRecoveryPerUpdate = .5M,
//            FromForm_SneakPercent= -50,
//            FromForm_EvasionPercent= -8,
//            FromForm_ExtraSkillCriticalPercent= 7.5M,
//            FromForm_MoveActionPointDiscount= -.1M,
//            FromForm_SpellExtraHealthDamagePercent= 8.5M,

//         }
//}, new Form {
//         dbName = "form_Tempting_Teacher_Varn",
//         FriendlyName = "Tempting Teacher",
//         Description = "A young teacher - the subject of schoolgirl fantasy.  Or at least, those schoolgirls who go in for other women; which to be fair, appears to be nearly all of them in this town.  Somewhere in her early 30s, this woman - a recent graduate of Saint Circe's herself - has returned as a lecturer.  Though technically in the biology department, her passion is teaching courses on sexual education - of which the college has a surprising number.  From the 101 course required for all students, to a multitude with names like \"Bondage through the Ages\", to her 'Independent Study' courses with waiting lists a mile long - her schedule is full, though she always seems to be able to make time to help her students with 'personalized instruction'; her popularity among her students is unmatched, even though most have to really work for those As.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "teacher_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_SpellExtraTFEnergyPercent= 3,
//            FromForm_SpellMisfireChanceReduction= 2,
//            FromForm_SpellHealthDamageResistance= 3,
//            FromForm_SpellTFEnergyDamageResistance= -7,
//         }

//}, new Form {
//         dbName = "form_Flirty_Three-Tiered_Skirt_Martiandawn",
//         FriendlyName = "Flirty Three-Tiered Skirt",
//         Description = "Whatever you were before, you are now a cute, ruffled, three-tier skirt. Your main body is black, with a middle tier in a lovely shade of pink and a matching stripe right below the waistband! Feminine, flirtatious, and sexy, you spend your days riding around on your owner's hips, saucily flashing their assets to passersby with every flip of your ruffles as their derriere bounces beneath you. Could anyone ask for a better existence?",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Flirty_Three-Tiered_Skirt_Martiandawn",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Djinn_Slave_Danaume_Rook",
//         FriendlyName = "Djinn Slave",
//         Description = "You are a beautiful and buxom woman, dressed in light, translucent purple silks and polished brass wrist cuffs. Your skin is a lovely and spotless pale Arabic tan, and your long brunette hair hangs in a wild ponytail. Your eyes burn with the knowledge of the universe, and a sense of wicked mischievousness, but with your face lacking any other real features, all you can do is watch silently and plot against your owner.",
//         Gender = "female",
//         TFEnergyRequired = 68,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Djinn_Slave_Danaume_Rook",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_New_Age_Retro_Hippy_Varn",
//         FriendlyName = "New Age Retro Hippy",
//         Description = "All she is saying is give peace a chance.  This young girl - probably a student at the local campus - is dressed quite oddly for this era.  A skimpy tie-dyed outfit; flowers in her hair, flowers everywhere.  Given that she's college-aged today, she either got the look from history class or she is wearing hand-me-downs from her grandmother.  Nevertheless, she does seem to fit in around here, particularly when she's taking about 'free love,' though her friends tend to roll their eyes when she's on about almost any other one of her many causes.  Most notably, she is staunchly anti-war - you've seen her around town holding signs that say things like \"Make Love, Not Horsecock!\" - and is nearly incapable of fighting herself; moreover, she will passively resist attempts to change her idealistic ways.",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "new_age_retro_hippy_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_SneakPercent= 10,
//            FromForm_EvasionPercent= 30,
//            FromForm_SpellExtraTFEnergyPercent= -50,
//            FromForm_SpellExtraHealthDamagePercent= -50,
//            FromForm_SpellHealthDamageResistance= 25,
//            FromForm_SpellTFEnergyDamageResistance= 25,
//         }


//}, new Form {
//         dbName = "form_Slutty_Nurse_Ashley_Maid",
//         FriendlyName = "Slutty Nurse",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "slutty_nurse_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_EvasionNegationPercent= 6,
//            FromForm_CleanseExtraHealth= 6,
//            FromForm_SpellExtraTFEnergyPercent= -2,
//            FromForm_SpellExtraHealthDamagePercent= -4,
//            FromForm_CleanseExtraTFEnergyRemovalPercent= 2,
//            FromForm_ExtraInventorySpace= -1,
//         }

//}, new Form {
//         dbName = "form_Sexy_Princess_BlackTGKitty",
//         FriendlyName = "Sexy Princess",
//         Gender = "female",
//         TFEnergyRequired = 72,
//         MobilityType = "full",
//         PortraitUrl = "sexy_princess_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_SneakPercent= 27.5M,
//            FromForm_SpellExtraTFEnergyPercent= 5,
//            FromForm_CleanseExtraTFEnergyRemovalPercent= -5,
//            FromForm_SpellTFEnergyDamageResistance= -5,
//         }

//}, new Form {
//         dbName = "form_Enchanted_Tennis_Racket_themorpher606",
//         FriendlyName = "Enchanted Tennis Racket",
//         Gender = "male",
//         TFEnergyRequired = 72,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Enchanted_Tennis_Racket_themorpher606",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Rat_Thief_Draggony",
//         FriendlyName = "Rat Thief",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "rat_thief_m_Thrax.png",
//         FormBuffs = new BuffBox{
//            FromForm_SneakPercent= 25,
//            FromForm_EvasionPercent= 21,
//            FromForm_ExtraSkillCriticalPercent= 5,
//            FromForm_SpellHealthDamageResistance= -22,
//            FromForm_SpellTFEnergyDamageResistance= -20,
//            FromForm_ExtraInventorySpace= 1,

//         }

//}, new Form {
//         dbName = "form_Heart_Shaped_Sunglasses_Christopher",
//         FriendlyName = "Heart Shaped Sunglasses",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Heart_Shaped_Sunglasses_Christopher",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Flirty_Tied_Crop_Top_Martiandawn",
//         FriendlyName = "Flirty Tied Crop Top",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Flirty_Tied_Crop_Top_Martiandawn",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Dapper_Charmer_Rayner",
//         FriendlyName = "Dapper Charmer",
//         Gender = "male",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "dapper_Luxianne.png",
//         FormBuffs = new BuffBox{
//          FromForm_SneakPercent= 15,
//            FromForm_ExtraSkillCriticalPercent= 3,
//            FromForm_CleanseExtraTFEnergyRemovalPercent= -3,
//            FromForm_SpellMisfireChanceReduction= -4.5M,
// }

//}, new Form {
//         dbName = "form_Star-Studded_Cloak_Techhead",
//         FriendlyName = "Star-Studded Cloak",
//         Gender = "male",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Star-Studded_Cloak_Techhead",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Knee-High_Socks_Christopher",
//         FriendlyName = "Knee-High Socks",
//         Gender = "female",
//         TFEnergyRequired = 60,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Knee-High_Socks_Christopher",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Fox_Thief_Rayner",
//         FriendlyName = "Fox Thief",
//         Gender = "male",
//         TFEnergyRequired = 75,
//         MobilityType = "full",
//         PortraitUrl = "foxtheif_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_ManaBonusPercent = 8,
//            FromForm_SneakPercent= 10,
//            FromForm_MoveActionPointDiscount= .10M,
//            FromForm_SpellExtraTFEnergyPercent= -20,
//            FromForm_SpellExtraHealthDamagePercent= 8,
//         }
//}, new Form {
//         dbName = "form_Fetish_Wizard's_Staff_Lily",
//         FriendlyName = "Fetish Wizard's Staff",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Fetish_Wizard's_Staff_Lily",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Female_Possesed_Armor_Keyne_Vangsten",
//         FriendlyName = "Female Possesed Armor",
//         Gender = "female",
//         TFEnergyRequired = 72,
//         MobilityType = "full",
//         PortraitUrl = "female_armor_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = 5,
//            FromForm_SneakPercent= -20,
//            FromForm_EvasionNegationPercent= -10,
//            FromForm_MoveActionPointDiscount= -.4M,
//            FromForm_SpellHealthDamageResistance= 6,
//            FromForm_SpellTFEnergyDamageResistance= 6,
//         }

//}, new Form {
//         dbName = "form_Cuddly_Pocket_Goo_Girl_GooGirl",
//         FriendlyName = "Cuddly Pocket Goo Girl",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Cuddly_Pocket_Goo_Girl_GooGirl",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Gimbo_Stripper_Great_Daeo",
//         FriendlyName = "Gimbo Stripper",
//         Gender = "female",
//         TFEnergyRequired = 90,
//         MobilityType = "full",
//         PortraitUrl = "gimbo_stripper_thegreatdaeo.jpg",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -15,
//            FromForm_ManaBonusPercent = 10,
//            FromForm_SneakPercent= -20,
//            FromForm_EvasionPercent= -15,
//            FromForm_ExtraSkillCriticalPercent= 5,
//            FromForm_SpellExtraTFEnergyPercent= 6,
//         }

//}, new Form {
//         dbName = "form_Maternity_Panties_Lily",
//         FriendlyName = "Maternity Panties",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Maternity_Panties_Lily",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Femboi_Skunk_Nyx",
//         FriendlyName = "Femboi Skunk",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "femboi_skunk_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -8,
//            FromForm_SneakPercent= -10,
//            FromForm_SpellExtraTFEnergyPercent= 9,
//         }

//}, new Form {
//         dbName = "form_Iniquitous_Omnibus_Varn",
//         FriendlyName = "Iniquitous Omnibus",
//         Gender = "female",
//         TFEnergyRequired = 66.66M,
//         MobilityType = "full",
//         PortraitUrl = "Omnibus_Meddle.png",
//         FormBuffs = new BuffBox{
//            FromForm_ManaBonusPercent = 6.66M,
//FromForm_SpellExtraTFEnergyPercent= 6.66M,
//FromForm_SpellExtraHealthDamagePercent= 6.66M,
//FromForm_SpellHealthDamageResistance= -6.66M,
//FromForm_SpellTFEnergyDamageResistance= -6.66M,

//         }

//}, new Form {
//         dbName = "form_Elven_Femboy_Aingelproject667",
//         FriendlyName = "Elven Femboy",
//         Gender = "male",
//         TFEnergyRequired = 65,
//         MobilityType = "full",
//         PortraitUrl = "elven_femboi_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -25,
//            FromForm_HealthRecoveryPerUpdate = .4M,
//            FromForm_ExtraSkillCriticalPercent= 10,
//            FromForm_MoveActionPointDiscount= .2M,
//            FromForm_SpellHealthDamageResistance= -10,
//         }

//}, new Form {
//         dbName = "form_Elvish_Bard_Varn",
//         FriendlyName = "Elvish Bard",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "full",
//         PortraitUrl = "elvish_bard_BabblingFaces.png",
//         FormBuffs = new BuffBox{
//             FromForm_HealthBonusPercent = 7,
//             FromForm_HealthRecoveryPerUpdate = -5,
//             FromForm_SneakPercent= -20,
//             FromForm_CleanseExtraHealth= 10,
//             FromForm_SpellExtraTFEnergyPercent= 16,
//             FromForm_SpellHealthDamageResistance= -4,
//         }

//}, new Form {
//         dbName = "form_Unicorn_Mare_LexamTheGemFox",
//         FriendlyName = "Unicorn Mare",
//         Gender = "female",
//         TFEnergyRequired = 67.50M,
//         MobilityType = "full",
//         PortraitUrl = "unicorn_mare_Luxianne.png",
//         FormBuffs = new BuffBox{
//            FromForm_SpellExtraTFEnergyPercent= -30,
//            FromForm_SpellExtraHealthDamagePercent= -30,
//            FromForm_SpellHealthDamageResistance= 23.5M,
//            FromForm_SpellTFEnergyDamageResistance= 23.5M,
//         }

//}, new Form {
//         dbName = "form_Hand_Lens_Medli",
//         FriendlyName = "Hand Lens",
//         Gender = "male",
//         TFEnergyRequired = 70,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Hand_Lens_Medli",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Beach_Babe_Enthusiast_Great_Daeo",
//         FriendlyName = "Beach Babe Enthusiast",
//         Gender = "female",
//         TFEnergyRequired = 80,
//         MobilityType = "full",
//         PortraitUrl = "beach_babe_enthusiast_TheGreatDaeo.jpg",
//         FormBuffs = new BuffBox{
//            FromForm_HealthBonusPercent = -15,
//            FromForm_SneakPercent= 12,
//            FromForm_EvasionPercent= 5,
//            FromForm_MoveActionPointDiscount= -.5M,
//            FromForm_SpellExtraTFEnergyPercent= 5,
//            FromForm_SpellExtraHealthDamagePercent= 5,
//            FromForm_CleanseExtraTFEnergyRemovalPercent= 5,
//            FromForm_SpellHealthDamageResistance= -5,
//            FromForm_SpellTFEnergyDamageResistance= -5,
//         }

//}, new Form {
//         dbName = "form_Dog_Collar_Arnisd",
//         FriendlyName = "Dog Collar",
//         Gender = "male",
//         TFEnergyRequired = 80,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Dog_Collar_Arnisd",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Magical_Binding_Tattoo_Taenil_Auxifur",
//         FriendlyName = "Magical Binding Tattoo",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_Magical_Binding_Tattoo_Taenil_Auxifur",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_One-Piece_Latex_Swimsuit_Illia_Malvusin",
//         FriendlyName = "One-Piece Latex Swimsuit",
//         Gender = "female",
//         TFEnergyRequired = 75,
//         MobilityType = "inanimate",
//         PortraitUrl = "",
//         BecomesItemDbName = "item_One-Piece_Latex_Swimsuit_Illia_Malvusin",
//         FormBuffs = new BuffBox{}

//}, new Form {
//         dbName = "form_Brainwashed_Bitch_LexamTheGemFox",
//         FriendlyName = "Brainwashed Bitch",
//         Gender = "female",
//         TFEnergyRequired = 70,
//         MobilityType = "animal",
//         PortraitUrl = "",
//         BecomesItemDbName = "animal_Brainwashed_Bitch_LexamTheGemFox",
//         FormBuffs = new BuffBox{}
//}



 
            
//        };


//            }

//        }

        public static DbStaticForm GetForm(string dbFormName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.FirstOrDefault(s => s.dbName == dbFormName);
        }

        public static IEnumerable<DbStaticForm> GetAllAnimateForms()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.Where(s => s.MobilityType == "full");
        }

        public static List<RAMBuffBox> FormRAMBuffBoxes;

    }

}