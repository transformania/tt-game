using System.Collections.Generic;
using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Statics
{
    public static class EffectStatics
    {

        public static List<RAMBuffBox> EffectRAMBuffBoxes;

        public static DbStaticEffect GetStaticEffect2(string dbEffectName)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.FirstOrDefault(s => s.dbName == dbEffectName);
        }


     //   List<StaticEffect> perkEffects = EffectStatics.GetStaticEffect.Where(e => e.AvailableAtLevel > 0 && e.AvailableAtLevel <= player.Level).ToList();



        public static IEnumerable<DbStaticEffect> GetEffectGainedAtLocation(string location)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.ObtainedAtLocation == location);
        }

//        public static IEnumerable<StaticEffect> GetStaticEffect
//        {



//            get
//            {
//                return new[]
//                {

//#region level up perks

//                    // keep these at around 35 points
//            new StaticEffect {
//                dbName = "perk_swift_1_lvl",
//                FriendlyName = "Swift",
//                Description = "Some people may call this cowardly, but a good mage needs to know when to run--either fleeing or pursuing.  You're a bit better than most at this, no matter what body you might have at the moment or the items burdening you.",
//                MoveActionPointDiscount = .14M,
//                AvailableAtLevel = 1,

//            },

//            new StaticEffect {
//                dbName = "perk_swift_2_lvl",
//                FriendlyName = "Seriously Swift",
//                Description = "Some people may call this cowardly, but a good mage needs to know when to run--either fleeing or pursuing.  You're quite agile on your feet and can jog for miles or break into a full on sprint that even your faster peers will have some trouble keeping up with.",
//                MoveActionPointDiscount = .14M,
//                AvailableAtLevel = 4,
//                PreRequesite = "perk_swift_1_lvl",

//            },

//             new StaticEffect {
//                dbName = "perk_mentally_resilient_1_lvl",
//                FriendlyName = "Mentally Resilient",
//                Description = "There is a lot to see and feel in this town and casting so much magic, as well as constantly finding yourself in new and strange bodies, is enough to strain anyone's mental willpower.  You are a little more mentally resilient than most, however, and will restore a bit of extra willpower every turn.",
//                HealthRecoveryPerUpdate = 3,
//                AvailableAtLevel = 1,

//            },
//            new StaticEffect {
//                dbName = "perk_mana_brewer_1_lvl",
//                FriendlyName = "Mana Brewer",
//                Description = "Some people constantly find themselves drained of mana, the essence that allows magic to flow through their body and out of their fingertips.  Your body is a natural mana brewery, however, and you regain a little bit of mana every turn without even needing to meditate.",
//                ManaRecoveryPerUpdate = 3,
//                AvailableAtLevel = 1,

//            }, new StaticEffect {
//                dbName = "perk_backpacker_1_lvl",
//                FriendlyName = "Backpacker",
//                Description = "Many of your mage friends and competitors seem to be struggling to carry all of their formerly-human clothes and equipment.  You have a sturdier back than most and can carry an extra item with ease.",
//                ExtraInventorySpace = 1,
//                AvailableAtLevel = 1,

//            }, new StaticEffect {
//                dbName = "perk_backpacker_2_lvl",
//                FriendlyName = "Serious Backpacker",
//                Description = "Many of your mage friends and competitors seem to be struggling to carry all of their formerly-human clothes and equipment.  You have a sturdier back than most and can carry an extra item with ease.  Not only that, but you have discovered a nifty little way of walking with magic in your feet and spine that allows you to carry yet another item on you at all times.",
//                ExtraInventorySpace = 1,
//                AvailableAtLevel = 4,
//                PreRequesite="perk_backpacker_1_lvl",

//            }, new StaticEffect {
//                dbName = "perk_sneaky_1_lvl",
//                FriendlyName = "Sneaky",
//                Description = "In a world of bravado and a show of prowess, you know that sometimes appearing small and invisible has its own benefits.  You know how to move more silently than most and know a thing or two about covering your tracks.",
//                SneakPercent = 8.5M,
//                AvailableAtLevel = 1,

//             }, new StaticEffect {
//                dbName = "perk_sneaky_2_lvl",
//                FriendlyName = "Silent Stalker",
//                Description = "In a world of bravado and a show of prowess, you know that sometimes appearing small and invisible has its own benefits.  You know how to move more silently than most and know a thing or two about covering your tracks.  But you've also gained the ability to not only feel but see sound waves and can dampen those near you with your magic, making you even sneakier than before.",
//                SneakPercent = 8.5M,
//                AvailableAtLevel = 4,
//                PreRequesite = "perk_sneaky_1_lvl",

//            }, new StaticEffect {
//                dbName = "perk_careful_1_lvl",
//                FriendlyName = "Careful",
//                Description = "Too many mages have found themselves as a humble animal or immobile item when a spell misfired and blew up in their own faces.  You know how to be steady and take your time, reducing the chances of having a spell that you cast misfire.",
//                SpellMisfireChanceReduction = 3.5M,
//                AvailableAtLevel = 1,

//            }, new StaticEffect {
//                dbName = "perk_intimidating_1_lvl",
//                FriendlyName = "Intimidating Glare",
//                Description = "Sometimes you don't need magic to make an opponent cower in fear and leave themselves extra vulnerable to attack.  A good glare can often wet the most brave mage's trousers, and you're not bad at giving the Evil Eye.  Your spells damage your target's willpower more than usual.",
//                SpellExtraHealthDamagePercent = 5,
//                AvailableAtLevel = 1,

//           }, new StaticEffect {
//                dbName = "perk_intimidating_2_lvl",
//                FriendlyName = "Intimidating Posture",
//                Description = "You've mastered the intimidating glare.  But you've also learned that there are other ways to intimidate your opponents--the way you walk, the way you talk, the way you brush off lesser spells like flies (or at least pretend to.)  Your spells damage your targets' willpower even more than before.  Replaces the 'Intimidating Glare' perk.",
//                SpellExtraHealthDamagePercent = 5,
//                AvailableAtLevel = 4,
//                PreRequesite = "perk_intimidating_1_lvl",

//            }, new StaticEffect {
//                dbName = "perk_self_sufficient_1_lvl",
//                FriendlyName = "Self Sufficient",
//                Description = "It's good to have friends to watch your back, but you know how to look after yourself when friends fall, fail, or betray you.  When you cleanse yourself and meditate, you recover extra willpower and mana.",
//                CleanseExtraHealth = .75M,
//                MeditationExtraMana = .75M,
//                AvailableAtLevel = 1,

//            }, new StaticEffect {
//                dbName = "perk_focused_1_lvl",
//                FriendlyName = "Focused",
//                Description = "Even in the thick of battle with friends falling inanimate and spells grazing by your head, you are pretty good at keeping your cool.  As a result, you can cast more critical spell hits than those around you.",
//                ExtraSkillCriticalPercent = 2.25M,
//                AvailableAtLevel = 1,
//            },

//            new StaticEffect {
//                dbName = "perk_focused_2_lvl",
//                FriendlyName = "Deadly Precision",
//                Description = "Even in the thick of battle with friends falling inanimate and spells grazing by your head, you are pretty good at keeping your cool.  You can completely tone out your fears and worries for the time it takes for you to deliver a spell, increasing the chance of a critical hit even greater than before.",
//                ExtraSkillCriticalPercent = 2.25M,
//                AvailableAtLevel = 4,
//                PreRequesite="perk_focused_1_lvl",
//            },

//            new StaticEffect {
//                dbName = "perk_thickskinned_1_lvl",
//                FriendlyName = "Thick Skinned",
//                Description = "Some mages have a hard time putting up a good defense, trying to dodge spells left and right.  You do this too, but you know that sometimes you will get hit no matter how agile you believe yourself to be.  Your thick, magic-resistant skin helps to less the effect of willpower loss when you do get struck, as well as keep some transformation energy from entering your bloodstream.",
//                SpellHealthDamageResistance = 2,
//                SpellTFEnergyDamageResistance = 2,
//                AvailableAtLevel = 1,
//            },

//            new StaticEffect {
//                dbName = "perk_thickskinned_2_lvl",
//                FriendlyName = "Armor-Like Skin",
//                Description = "Some mages have a hard time putting up a good defense, trying to dodge spells left and right.  You do this too, but you know that sometimes you will get hit no matter how agile you believe yourself to be.  You have conditioned your skin to be tough, almost a form of permant armor that prevents even more transformation energy from entering your body and mind than before.",
//                SpellHealthDamageResistance = 2,
//                SpellTFEnergyDamageResistance = 2,
//                AvailableAtLevel = 4,
//                PreRequesite="perk_thickskinned_1_lvl",
//            },

//             new StaticEffect {
//                dbName = "perk_naturalpurifier_1_lvl",
//                FriendlyName = "Natural Purifier",
//                Description = "You've heard of mages walking around with absolutely no idea how much transformation energy is teeming in their body, just waiting for the teensiest bit extra to tip the tide and seal its victim into a new form in even the smallest moment of weakness.  You understand the importance of stopping this energy from building too high up and can remove twice as much extra transformation energy from your body every time you cleanse yourself than most of your peers.",
//                CleanseExtraTFEnergyRemovalPercent = 1.75M,
//                AvailableAtLevel = 1,

//            },

//            new StaticEffect {
//                dbName = "perk_sharp_eye",
//                FriendlyName = "Sharp Eye",
//                Description = "You have a sharp eye for your environment.  You notice small details that your peers often miss, and when presented with some kind of new environmental experience, you can map it all out a bit more quickly and efficiently in your head, letting you get on to other things without wasting extra time and effort.  When searching your current location, you spend one less action point per search.",
//                AvailableAtLevel = 1,

//            },

//             new StaticEffect {
//                dbName = "bot_psychopathic",
//                FriendlyName = "Psychopathic",
//                Description = "Friendless but fierce, this spellslinger is powerful and can hit for a lot of willpower damage.  However, their fury keeps them from acting intelligently or persistently, and he or she is always on the hunt for new targets whether or not they've inanimated their victims or just made them angry.",
//                ExtraSkillCriticalPercent = 8,
//                SpellExtraHealthDamagePercent = 75,
//                SpellExtraTFEnergyPercent = 25,
//                HealthBonusPercent = 25,
//                ManaBonusPercent = 50,
//                AvailableAtLevel = 9999,
//                MeditationExtraMana = 15,

//            },

//            new StaticEffect {
//                dbName = "bot_psychopathic_lvl3",
//                FriendlyName = "Fiercly Psychopathic",
//                Description = "Friendless but fierce, this spellslinger is powerful and can hit for a lot of willpower damage.  However, their fury keeps them from acting intelligently or persistently, and he or she is always on the hunt for new targets whether or not they've inanimated their victims or just made them angry.  This spellslinger is a magnitude of anger higher than regular psychopathic spellslingers and fights extra fiercly.",
//                ExtraSkillCriticalPercent = 9,
//                EvasionNegationPercent = 7,
//                SpellExtraHealthDamagePercent = 125,
//                SpellExtraTFEnergyPercent = 50,
//                HealthBonusPercent = 35,
//                ManaBonusPercent = 500,
//                AvailableAtLevel = 9999,
//                MeditationExtraMana = 15,
//            },

//            new StaticEffect {
//                dbName = "bot_psychopathic_lvl5",
//                FriendlyName = "Wrathfully Psychopathic",
//                Description = "Friendless but fierce, this spellslinger is powerful and can hit for a lot of willpower damage.  However, their fury keeps them from acting intelligently or persistently, and he or she is always on the hunt for new targets whether or not they've inanimated their victims or just made them angry.  Having been wronged many times in life, this psychopathic spellslinger has shed their mercy for their peers.",
//                ExtraSkillCriticalPercent = 11,
//                EvasionNegationPercent = 10,
//                SpellExtraHealthDamagePercent = 200,
//                SpellExtraTFEnergyPercent = 75,
//                HealthBonusPercent = 50,
//                ManaBonusPercent = 500,
//                AvailableAtLevel = 9999,
//                MeditationExtraMana = 15,
//                CleanseExtraHealth = 5,
//            },

//            new StaticEffect {
//                dbName = "bot_psychopathic_lvl7",
//                FriendlyName = "Loathfully Psychopathic",
//                Description = "Friendless but fierce, this spellslinger is powerful and can hit for a lot of willpower damage.  However, their fury keeps them from acting intelligently or persistently, and he or she is always on the hunt for new targets whether or not they've inanimated their victims or just made them angry.  This psychopathic spellslinger loathes all forms of life and will seek to dominate any human it encounters, no matter the cost.",
//                ExtraSkillCriticalPercent = 13,
//                EvasionNegationPercent = 20,
//                SpellExtraHealthDamagePercent = 150,
//                SpellExtraTFEnergyPercent = 100,
//                HealthBonusPercent = 75,
//                ManaBonusPercent = 500,
//                AvailableAtLevel = 9999,
//                MeditationExtraMana = 15,
//                CleanseExtraHealth = 10,
//            },

//            new StaticEffect {
//                dbName = "bot_psychopathic_lvl9",
//                FriendlyName = "Soulessly Psychopathic",
//                Description = "Friendless but fierce, this spellslinger is powerful and can hit for a lot of willpower damage.  However, their fury keeps them from acting intelligently or persistently, and he or she is always on the hunt for new targets whether or not they've inanimated their victims or just made them angry.  This psychopathic spellslinger is unordinarily strong and and is said to be wholly devoid of any humanity, little more than an embodiment of hate.",
//                ExtraSkillCriticalPercent = 15,
//                EvasionNegationPercent = 30,
//                SpellExtraHealthDamagePercent = 50,
//                SpellExtraTFEnergyPercent = 125,
//                HealthBonusPercent = 150,
//                ManaBonusPercent = 500,
//                AvailableAtLevel = 9999,
//                MeditationExtraMana = 30,
//                CleanseExtraHealth = 15,
//            },

//#endregion

//#region temporary from spells

//            new StaticEffect {
//                dbName = "curse_bimbo_kiss",
//                FriendlyName = "Bimbo Kiss Bliss",
//                Description = "You have recently been kissed by a busty blonde bimbo.  The moment your tongue met with hers, you began to feel a little bit strange... you find that your gaze lingers on the fine features of your friends and enemies, filling your mind with lustful urges.  Your hazy mind is more prone to misfiring spells--maybe some part of you WANTS to have your willpower crushed and your body ravished by a lover or two or ten...",
//                SpellMisfireChanceReduction = -5,
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 24,
//                MessageWhenHit = "One of the busty blonde bimbo suddenly grabs your wrist and pulls you into her body, squeezing you against her voluptious breasts with a giggle.  Before you can further react, she turns her head sideways and gives you a long fifteen second French Kiss, deep throating you in a way you never knew possible.  Your knees go weak as her curse spreads through your body, and while you'll be able to recover from her magic in due time it's hard to keep pangs of lust and random girly giggles from slipping out of your throat...",
//                AttackerWhenHit = "",

//            },

//             new StaticEffect {
//                dbName = "curse_dominatrix_ass_slap",
//                FriendlyName = "Donkey Dominatrix Ass Slap",
//                Description = "You have had your rear end slapped, hard, by a Donkey Dominatrix not that long ago.  Your bottom still tingles not just from the force of the impact, but from a lingering dominatrix magic that you can feel trying to creep up your spine and into your mind and the rest of your body.  You can fight the infiltrating submissive urges, but not without leaving you a little weaker and more exposed until the curse goes away...",
//                AvailableAtLevel = 0,
//                Duration = 8,
//                Cooldown = 24,
//                MessageWhenHit = "A Donkey Dominatrix slips behind you and delivers a hoofed kick to one of your calf muscles.  You stumble forward, hands on the ground with your ass stuck exposed up in the air.  Ow!  Somehow she has slipped down your pants and underwear and delivered a powerful slap to your ass, stinging your butt cheeks and causing your whole body to tense up.  Embarassed, you stumble forward and pull your garments back on, but despite the sting and humiliation some aggressive magic is trying to force your mind to plead for another hit, and another, and another...  You'd better be careful until you can take control of these submissive urges.",
//                AttackerWhenHit = "",
//                SpellHealthDamageResistance = -7.5M,
//                SpellTFEnergyDamageResistance = -7.5M,
//            },

//            new StaticEffect {
//                dbName = "curse_rooted_to_the_ground",
//                FriendlyName = "Rooted to the Earth",
//                Description = "An Enchanted Tree has cursed you causing small black vines to start growing around your feet.  Try as you might you can't break free and while a quick counterspell is causing the vines to whither and die, you'd better be prepared to stand your ground until you can uproot yourself.  At least you're able to draw some nourishment from the ground beneath you.",
//                AvailableAtLevel = 0,
//                Duration = 2,
//                Cooldown = 25,
//                MessageWhenHit = "You are passing through when an Enchanted Tree, a tree tryad, taps you on the shoulder with a twig-like finger.  Before you know it she has placed an aromatic wreath of flowers around your scalp.  You feel more annoyed than scared and try to tear it off, but when you try to step you away you discover that you a bundle of slowly growing vines has captured your foot and is slowly creeping up along your shins and knees.  You feel strange, suddenly able to taste with the soles of your feet--the dryad is trying to turn you into a tree!  You cast a counter spell that causes the vines to start to whither, but you're still going to have to struggle a bit to break free...",
//                AttackerWhenHit = "",
//                MoveActionPointDiscount = -999,
//                HealthRecoveryPerUpdate = 5,
//                ManaRecoveryPerUpdate = 5,

//            },

//        new StaticEffect {
//                dbName = "curse_singing_like_an_idiot",
//                FriendlyName = "Squawking Like an Idiot",
//                Description = "A Dainty Birdgirl sang a mating song to you and now the damn thing is stuck in your head on repeat.  Worse, some magic compells you to try and repeat it yourself, causing you to squawk out the melody like an idiot.  With all this racket you'll find it far harder to sneak around unnoticed until you push the song out of your head again.",
//                AvailableAtLevel = 0,
//                Duration = 12,
//                Cooldown = 24,
//                MessageWhenHit = "A Dainty Birdgirl approaches you with a lonely look in her eyes.  You back up, ready to sling some spells, but your ears start to ring when she begins to squawk out a pretty but very loud and shrill bird song.  She ruffles her feathers and prances around you with sharp birdlike steps, and while you know some might find this sexy, you aren't in the right body or mood for it.  Even when she gives up, however, her song gets stuck in your head, and a curse compells you to start screeching the melody out loud  against your own will.",
//                AttackerWhenHit = "",
//                SneakPercent = -32,
//            },

//          new StaticEffect {
//                dbName = "curse_milk_moustache",
//                FriendlyName = "Milk Moustache",
//                Description = "An Impossibly Big Breasted Lady squirted you in the face with her own breast milk.  You've wiped the liquid off but some got in your mouth and you fight to keep your mind from regressing to infantile behavior, sapping your mana and slowing you down as you fight the urge to resort back to crawling as a primary means of travel.",
//                AvailableAtLevel = 0,
//                Duration = 10,
//                Cooldown = 25,
//                MessageWhenHit = "You eye an Impossibly Big Breasted Lady across from you, stepping slowly and holding up her massive jostling mammaries in her hands.  You take a step forward, anticipating an easy takedown and a lovely new garment with her soul lingering inside, but to your surprise she slips out a nipple and aims it at your face.  You barely have time to blink before a stream of warm, wet breast milk douses your face, getting in your eyes, nose, and mouth.  And the taste isn't at all bad... your minds begins to regress, longing for a time long past when it was just you, your blankie, and your mommy...  You wipe the milk off your face and clear your mind, but part of you remains distracted while the taste of it lingers on your tongue...",
//                AttackerWhenHit = "",
//                ManaRecoveryPerUpdate = -5,
//                MoveActionPointDiscount = -.2M,




//      }, new StaticEffect {
//         dbName = "effect_Hemorrhage_Izz_Valentine",
//         FriendlyName = "Hemorrhage",
//         Description = "You have been clearly bitten by a vampire, or some lunatic who thought it was Halloween, either way you are currently bleeding (which is rather distracting when one is trying to preserve their willpower) and should seek immediate medical assistance.",
//         Duration = 6,
//         Cooldown = 12,
//         AvailableAtLevel = 0,
//         HealthRecoveryPerUpdate = -2,
//         CleanseExtraTFEnergyRemovalPercent= -1.42M,


//      },  new StaticEffect {
//                dbName = "skill_a_bloody_kiss_Lilith",
//                FriendlyName = "A Bloody Kiss",
//                Description = "You are dizzy with lust for the powerful Lord Valentine.  Your willpower is draining sharply as you fight your desire to give your body and soul in to him utterly.",
//                AvailableAtLevel = 0,
//                Duration = 3,
//                Cooldown = 5,
//                MessageWhenHit = "You find the intoxicating scent of cinnamon and cloves fill your lungs. You stumble backwards as your heart begins to irregularly thump in your chest to send waves of dizziness throughout your body. Pleasureable fuzziness course through your veins as your fears are confirmed with a sudden grip on your shoulders. Israel has appeared in front of you, and with a grip on you he tilts your neck as his chilling fingers run across your neck. The bright lustful eyes watch over you as the screams of horror inside your head seems to please him. He leans over you as time stands still, and a sharp pain pierces your neck. Within seconds you feel the warmth drain from your body as the pain disappears into a blinding pleasure. A bright light begins to consume your vision and you find your body tremble in absolute bliss. Arousal floods your thoughts as the dominanting Lord Valentine drinks your very life away in greedy swallows. Then, as suddenly as the pleasure began, it halts and you find yourself on your knees in a daze. His laughter fills the room as his thoughts fill your head. 'Enjoy just a sliver of what I have to offer...'  As the lust disappears and you stand, his final thought whispers into your ear. 'Perhaps when we are finished you can find me later to sate your... desires.'",
//                AttackerWhenHit = "",
//                HealthRecoveryPerUpdate = -15,
//                SneakPercent = -15,
//}, 

//new StaticEffect {
//                dbName = "Valentine's_Presence_Lilith",
//                FriendlyName = "Valentine’s Presence",
//                Description = "You are so mesmerized by Lord Valentine’s presence that you find yourself immobilized, unable to look away from his alluring visage. His charming smile and passionate gaze send overwhelming sensations all over your body. You feel burning lust run through your veins like an intoxicating drug. As a small moan escapes through your lips, you know that you have already fallen for his trap...",
//                AvailableAtLevel = 0,
//                Duration = 1,
//                Cooldown = 5,
//                MessageWhenHit = "You are so mesmerized by Lord Valentine’s presence that you find yourself immobilized, unable to look away from his alluring visage. His charming smile and passionate gaze send overwhelming sensations all over your body. You feel burning lust run through your veins like an intoxicating drug. As a small moan escapes through your lips, you know that you have already fallen for his trap...",
//                AttackerWhenHit = "",
//                MoveActionPointDiscount = -999,
//},

//new StaticEffect {
//                dbName = "curse_bimboboss_kiss",
//                FriendlyName = "Bimbonic Virus Infectee",
//                Description = "You have been infected with the bimbonic virus raging through the town.  You find it hard to concentrate on what you are doing, and every man and woman you eye leaves you so deeply aroused that you have to fight your hardest to run up and kiss them, strip off their clothes then and there and indulge yourself in whatever manner of genitalia they possess, transferring your virus on to them in the process.  Not only that, but while your body and magic is trying to stave off the infection, there's always a chance that it will fail and you'll find yourself in the body of a bimbo if you haven't already...",
//                AvailableAtLevel = 0,
//                Duration = 99999,
//                Cooldown = 99999,
//                MessageWhenHit = "An infected blonde bimbo approaches you, smelling of sweet perfume and staring at you in lust, giggling all the while.  You back up and get ready to cast your magic against them when they suddenly leap forward, knocking you to the ground and slobbering you with kisses.  You roll them off just as they begin to dry hump you, wiping their saliva off your face though some has already slipped into your mouth along with the bimbo virus inside it.  You begin to sweat knowing that at any moment your body might spontaneously transform into the bimbo just like her...",
//                AttackerWhenHit = "You eye your target, a silly mage who has not yet embraced your Mother's gift.  You take a few steps forward, watching as they glance to the side to check for an escape route or if it's better to try and turn you into a statue or a pair of panties.  Just as they raise their hand to send a spell your way, you leap toward them and knock them to the ground, slobbering their face with kisses.  They clench up, closing their eyes and sealing their lips to keep the virus from entering their body, but just the slightest drop manages to slip in.  Your victim manages to push you off and break free from your grip, but you giggle knowing it's just a matter of time before the virus overwhelms them and transforms their body into a sexy one just like yours...",
//                HealthRecoveryPerUpdate = -3,
//                CleanseExtraHealth = -2,
//},

//new StaticEffect {
//                dbName = "blessing_bimboboss_cure",
//                FriendlyName = "Bimbonic Virus Free!",
//                Description = "You have injected yourself with the antivirus, protecting you against the bimbo virus.  However, it won't last forever as the virus is constantly evolving, but for now you won't have to worry about getting reinfected for a few hours.",
//                AvailableAtLevel = 0,
//                Duration = 48,
//                Cooldown = 48,
//                MessageWhenHit = "",
//                AttackerWhenHit = "",
//},

//                   //new StaticEffect {
//            //    dbName = "",
//            //    FriendlyName = "",
//            //    Description = "",
//            //    AvailableAtLevel = 0,
//            //    Duration = 0,
//            //    Cooldown = 0,
//            //    ObtainedAtLocation = "",
//            //    MessageWhenHit = ""
//            //},

//#endregion

//  new StaticEffect {
//                dbName = "help_animate_recovery",
//                FriendlyName = "Back On Your Feet",
//                Description = "You have recently recovered from being an item or an animal and some of your magic has been dampened while other bits enhanced.  You'll be a bit safer from attackers for a while as you get back on your feet, but you won't be so good at attacking either.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 7,
//                MessageWhenHit = "You feel a strange tingling sensation as you return to a human body.  It's distracting, but it also seems to repel magic from hitting you as strongly.  The sensation is fading but very slowly, so it'll last with you for at least an hour.",
//                SpellExtraTFEnergyPercent = -75,
//                SpellExtraHealthDamagePercent = -75,
//                SpellHealthDamageResistance = 75,
//                SpellTFEnergyDamageResistance = 75,
//  },

//  new StaticEffect {
//                dbName = "help_entered_PvP",
//                FriendlyName = "Rising to the Challenge",
//                Description = "You have recently decided to fight the most powerful mages in this town, no spells out of the question no matter what kind of item or beast they leave you in.  You are still mentally preparing to deal with your numerous opponents and are acting very defensively while you get used to it at the cost of your own ability to attack.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 7,
//                MessageWhenHit = "You take a deep breath, ready to face your opponents with any magic you are your attackers can muster from their perverted minds.",
//                SpellExtraTFEnergyPercent = -75,
//                SpellExtraHealthDamagePercent = -75,
//                SpellHealthDamageResistance = 75,
//                SpellTFEnergyDamageResistance = 75,
//  },

//#region temporary from searches

//            new StaticEffect {
//                dbName = "curse_shrunken",
//                FriendlyName = "Shrunken!",
//                Description = "You have been shrunken down to just a few inches tall by one of the strange machines at Dr. Hadkin's Research Clinic.  The world looms above you--walls are steep vertical cliffs, grasshoppers are fierce beasts, and the giants around you makes the ground tremble no matter how lightly they step.  Your tiny legs make you much slower, but at least you're also a much smaller target and can dodge spells a bit more easily.",
//                MoveActionPointDiscount = -3,
//                EvasionPercent = 15,
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 12,
//                ObtainedAtLocation = "lab_laboratory",
//                MessageWhenHit = "You are walking around the laboratory when you come across a strange ray-looking machine with a glowing blue rod.  It catches your eye, or at least its simplicity does.  There's just one red button with an arrow pointing down.  Your curiosity overtakes you and you press it.  You feel dizzy and your vision begins to blur.  Dark shapes whiz by from the ground up into the sky and you feel as though you are falling.  A few seconds later your senses return to normal and you pat down your body to see if anything about you has changed.  It isn't until you look up and discover the world looming above you--you, as well as all of your equipment, have shrunken down to just a few inches tall!  This isn't good... you hope someone doesn't accidentally step on you."

//            },

//             new StaticEffect {
//                dbName = "blessing_life_changes_fast",
//                FriendlyName = "Life Changing Experiences",
//                Description = "After experiencing a strange vision of being many different students at Saint Circe's Community College, you have gained the inspiration to change other peoples' lives.  More practically, however, you have an enhanced ability to change their bodies, greatly increasing the amount of transformation energy you deal to your target when casting spells.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 30,
//                ObtainedAtLocation = "college_foyer",
//                MessageWhenHit = "As you search the admission office of Saint Circe's Community College, you see a large poster on the wall showing images of smiling students hard at work studying or partying.  The images progress from showing young, bewildered freshmen to black gown-clad graduating series, a little older and a lot wiser.  The poster's caption simply states 'Life changes quickly at Saint Circe's.  You won't be the same person coming out as when you came in.'  Suddenly you feel dizzy and pass out on the ground.  You imagine yourself as a student--not just one, many different students all at the same time, male and female--human, anthromorphic, or totally alien.  Emotions surge through you for a minute until the vision fades.  When you stand up, you are back in your old form, but you are invigorated with a desire for the ability to change lives.  You may not be able to changes lives directly, exactly, but you have gained a temporarily enhanced ability to change other's bodies far more easily... though watch out, they can also do the same to you!",
//                SpellExtraTFEnergyPercent = 30,
//                SpellTFEnergyDamageResistance = -30,


//            },

//               new StaticEffect {
//                dbName = "blessing_tipsy",
//                FriendlyName = "Tipsy",
//                Description = "You've recently had a couple shots of a stiff liquor at the Smelly Sorceress Tavern.  You are tipsy from the alcohol and you find it a bit more difficult to walk in a straight line and cast magic.  On the plus side, you feel a little more relaxed and will regenerate extra willpower for a short while, as well as have your cleansing be more effective in both restoring willpower and removing transformation energy in your body.",
//                AvailableAtLevel = 0,
//                Duration = 8,
//                Cooldown = 12,
//                ObtainedAtLocation = "tavern_counter",
//                MessageWhenHit = "You sit down at the bar stool and spin around a few times, letting your weight off of your feet for a bit.  You eye a bottle of whiskey behind the counter.  Seeing no one around who would mind if you took a bit, you leap over the counter and nab the bottle and a shot glass, downing two shots to help ease your mind and relax a bit while you still have a human--or humanoid--body to enjoy booze with.  A few minutes later you feel a bit tipsy, your vision swimming a little as you stand up.  It may not be a good idea to try and duel anyone this way until the alcohol wears off, but at least you feel a bit more relaxed.",
//                SpellMisfireChanceReduction = -15,
//                EvasionPercent = -30,
//                EvasionNegationPercent = -30,
//                MoveActionPointDiscount = -.25M,
//                HealthRecoveryPerUpdate = 3,
//                CleanseExtraHealth = 6,
//                CleanseExtraTFEnergyRemovalPercent = 1
//            },

//           new StaticEffect {
//                dbName = "blessing_coffee_jitters",
//                FriendlyName = "Espresso Jitters",
//                Description = "You recently drank a coffee with a shot of what must be enchanted espresso.  You feel the magic and caffeine--you aren't sure which is which--coarse through your bloodstream, giving you the jitters are raising your heartbeat up a few dozens beats per minute.  You find that you can move faster and, both casting and doging spells a bit easier, but on the downside you aren't very focused and there's a good chance your spells may blow up in your own face.  Also, you might as well kiss your ability to meditate goodbye--you'll have quite a hard time keeping your body still enough for it to have any effect.",
//                AvailableAtLevel = 0,
//                Duration = 8,
//                Cooldown = 16,
//                ObtainedAtLocation = "coffee_shop_patio",
//                MessageWhenHit = "You sit down at one of the two-person tables in the cafe, hoping to see a barista come by and take your order or at least get behind the damn cash register.  Nobody comes.  You do, however, spot a little silver cup next to an espresso machine behind the counter.  You slip over to it and a minute later have a small cup of espresso steaming in your fingers.  You take only the smallest sip when you feel energy--magic, it feels like, surge through your mouth and shooting through your veins like liquid lightning.  You feel your body begin to jitter and expect to find yourself turning into something, but after half a minute you realize that what you have is nothing more than caffeine jitters.  Magically enhanced espresso jitters.",
//                MoveActionPointDiscount = .2M,
//                EvasionPercent = 15,
//                EvasionNegationPercent = 15,
//                SpellMisfireChanceReduction = -12,
//                MeditationExtraMana = -15,
//            },

//            new StaticEffect {
//                dbName = "curse_stoneass",
//                FriendlyName = "Petrified Ass",
//                Description = "Your ass has turned into marble where it touched one of the cement benches at the Sunnyglade Park Memorial Fountain.  Luckily you have use of all of your limbs and you are confident that you'll be able to reverse the changes given enough time, but your petrified ass will keep you from moving anywhere until the curse is reversed.  The only good news is that the parts of your body that are marble are immune to spells cast against you.",
//                MoveActionPointDiscount = -999,
//                AvailableAtLevel = 0,
//                Duration = 3,
//                Cooldown = 30,
//                ObtainedAtLocation = "park_fountain",
//                MessageWhenHit = "You sit down on one of the stone benches surrounding the memorial fountain to rest your feet a bit.  You sit forward, facing the water and gaze down into the gurgling blue bottom.  The reflection of the stone girl catches your eye and you look up.  That's strange... she's looking at you, but you could have sworn she was posed differently when you first arrived.  Her overly innocent grin starts to creep you out and you attempt to sit up, but as your feet push against the ground you discover you can't lift yourself up at all.  You gasp and redouble your efforts to no avail. You look down and are horrified to discover that your ass is turning to marble where it touches the cement bench, slowing creeping up your body.  A disembodied giggle comes from the direction of the statue.  You curse and begin to focus your mind on reversing on freeing your petrifying body, but it may take some time for you to reverse the magic enough to escape.  At which point you're immobile and at the mercy of any opportunistic mage who passes your way...",
//                EvasionPercent = -25,
//                SpellTFEnergyDamageResistance = 25,
//                SpellHealthDamageResistance = 25,
//            },

//             new StaticEffect {
//                dbName = "blessing_packmule",
//                FriendlyName = "Pack Mule's Burden",
//                Description = "You freed a burdened mule from Milton's Ranch.  It knocked you down at first but gave you a magical lick that somehow makes your legs and back feel stronger, increasing the number of items you can carry for a while.  Unfortunately your stride is a bit slower, your body trained toward endurance rather than speed or agility, making you overall a bit less nimble.",
//                AvailableAtLevel = 0,
//                Duration = 12,
//                Cooldown = 32,
//                ObtainedAtLocation = "ranch_pens",
//                MessageWhenHit = "You approach a small stable with a couple horses inside.  As you peer inside, you stand corrected--one horse and one mule.  The mule is still wearing a harness with a heavy-looking burlap sack tied on each end, dangling down on both sides of the poor creature with a thick yellowed rope.  'Poor creature,' you whisper, swinging open the latch to enter the stall with the intention of helping the beast of burden out by taking the load off of its back.  You start to release the harness but the mule panicks and charges you, knocking you flat on your back.  When the diziness goes away you see the mule inches from your face, licking your cheek a few times almost... apologetically?  Did it used to be a mage like you?  As you rise to your feet, you feel a new strength to your legs and back as the mule trots off to graze, free of its burden.",
//                ExtraInventorySpace = 2,
//                EvasionPercent = -20,
//                MoveActionPointDiscount = -.2M,
//            },

//             new StaticEffect {
//                dbName = "blessing_washerman",
//                FriendlyName = "Laundered by the Old Washerman",
//                Description = "You were recently turned into a gym towel and thrown into the washers and dryers in the Sweater Girls Laundry Room by an old half-blind washerman.  Luckily he had no intention of keeping you inanimate and all you had to endure was an actually rather massaging spin cycle and dryer sauna.  A towel named Bubba even taught you a few things about staying clean, enhancing your ability to remove transformation energy from your body for a short while.  The only downside is that your legs still feel a bit wobbly as you just grew them back not too long ago.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 24,
//                ObtainedAtLocation = "gym_laundry",
//                MessageWhenHit = "As you search the laundry room, you hear the door shut behind you.  You twirl around, ready to blast back at any peer who dares cast a spell at you, but lower your guard when you see an wrinkled old man carrying a heap of towels toss them into a washing machine.  At first you think he may be blind or visually impaired as he seems to ignore you entirely, but when you begin to step toward him he suddenly turns to face you, wrinkling his nose.  'Dirty!  So dirty!' He grumbles and raises a finger at you.  Before you can respond, you feel your body flutter to the ground with all the clothes you are wearing draped underneath and on top you you.  Is this it?  Inanimated by a half blind old man???  He tosses you and all of your equipment into one of the washing machines.  You are churned around for a few minutes before being tossed into a drier, humiliated by your sudden demise.  But to your surprise, once the last bit of moisture evaporates from your cloth body, you suddenly grow back to your last animate body, none the worse for wear.  On the plus side, you even learned a thing about being clean in the washer from a towel who called itself Bubba.  Now might be a good time to use your strange new knowledge before you get too used to being animate again...",
//                CleanseExtraTFEnergyRemovalPercent = 5,
//                MoveActionPointDiscount = -.05M,
//                SneakPercent = -10,

//            }, new StaticEffect {
//         dbName = "effect_Circine's_Hypnosis_LexamTheGemFox",
//         FriendlyName = "Circine's Hypnosis",
//         Description = "You were poking around in the bedroom of a young witch when you came across a silver doggy pendant.  You couldn't stop staring at it no matter how hard you tried.  Now you are crawling around on all fours and whimpering like a doggy.  You can no longer seem to focus on attacking others; you really want to roll onto your back and let people rub your belly instead!  But you have to fight this mind control, or else you just might wind up mentally stuck as a doggy forever.",
//         Duration = 5,
//         Cooldown = 10,
//         ObtainedAtLocation = "ranch_bedroom_teenager",
//         AvailableAtLevel = 0,
//         HealthBonusPercent = -25,
//         SpellMisfireChanceReduction= -25,
//},

     

//#endregion

//#region temporary from consumeables

//              new StaticEffect {
//                dbName = "blessing_health_jawbreaker_small",
//                FriendlyName = "Sucking on a Small Willflower Jawbreaker",
//                Description = "You are sucking on a small willflower jawbreaker.  Its magical juices are released slowly as your mouth dissolves away the sugar, recovering your willpower over time.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 9,
//                MessageWhenHit = "You pop the jawbreaker into your mouth and begin to suck away at its layers.  Its magical juices will slowly recover your willpower over time even when the candy itself has dissolved away in your mouth.",
//                HealthRecoveryPerUpdate = 5,
//            },

//            new StaticEffect {
//                dbName = "blessing_health_jawbreaker_large",
//                FriendlyName = "Sucking on a Large Willflower Jawbreaker",
//                Description = "You are sucking on a large willflower jawbreaker.  Its magical juices are released slowly as your mouth dissolves away the sugar, recovering your willpower over time.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 9,
//                MessageWhenHit = "You pop the jawbreaker into your mouth and begin to suck away at its layers.  Its magical juices will slowly recover your willpower over time even when the candy itself has dissolved away in your mouth.",
//                HealthRecoveryPerUpdate = 9,
//            },

//             new StaticEffect {
//                dbName = "blessing_mana_jawbreaker_small",
//                FriendlyName = "Sucking on a Small Spellweaver Jawbreaker",
//                Description = "You are sucking on a small spellweaver jawbreaker.  Its magical juices are released slowly as your mouth dissolves away the sugar, recovering your mana over time.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 9,
//                MessageWhenHit = "You pop the jawbreaker into your mouth and begin to suck away at its layers.  Its magical juices will slowly recover your mana over time even when the candy itself has dissolved away in your mouth.",
//                ManaRecoveryPerUpdate = 5,


//          },

//             new StaticEffect {
//                dbName = "blessing_mana_jawbreaker_large",
//                FriendlyName = "Sucking on a Large Spellweaver Jawbreaker",
//                Description = "You are sucking on a large spellweaver jawbreaker.  Its magical juices are released slowly as your mouth dissolves away the sugar, recovering your mana over time.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 9,
//                MessageWhenHit = "You pop the jawbreaker into your mouth and begin to suck away at its layers.  Its magical juices will slowly recover your mana over time even when the candy itself has dissolved away in your mouth.",
//                ManaRecoveryPerUpdate = 5,

//            },  new StaticEffect {
//                dbName = "blessing_concealment_cookie",
//                FriendlyName = "Concealment Cookie",
//                Description = "You recently ate a Concealment Cookie, a tasty treat with its flour mixed in with a special secret powder that can make whoever eats it shimmer in and out of the visible spectrum of light, making them harder to track and dampening any sounds they make for a short period of time.  Don't eat too many or you may end up like the recipe's inventor, who nobody has seen or heard from since she ate a dozen at her own celebration party and vanished into the air.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 9,
//                MessageWhenHit = "You take a big bite of the Concealment Cookie.  Every now and then your body shimmers, your skin turning translucent for brief periods of time, making it harder for others to see and hear you.",
//                SneakPercent = 15,
//            },

//            new StaticEffect {
//                dbName = "blessing_fire_fritter",
//                FriendlyName = "Fire Fritter",
//                Description = "You've recently eaten a Fire Fritter, a spicy apple-flavored donut that will make your spells a bit more powerful for a while if you can stand the burning sensation on your tongue.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 10,
//                MessageWhenHit = "You take a bite out of your Fire Fritter, wincing as the spice reaches your taste buds and begins to make you sweat.  Channeling this energy will allow you deal more damage with your spells until the burn dissipates.",
//                SpellExtraHealthDamagePercent = 12,
//                SpellExtraTFEnergyPercent = 12,
//            },

//            //new StaticEffect {
//            //    dbName = "",
//            //    FriendlyName = "WillyNilly Will Worms",
//            //    Description = "A small bag of candy with a handful of red gummy worms inside.  ",
//            //    AvailableAtLevel = 0,
//            //    Duration = 6,
//            //    Cooldown = 12,
//            //    MessageWhenHit = "",
//            //    SpellExtraHealthDamagePercent = 12,
//            //    SpellExtraTFEnergyPercent = 12,
//            //},

//             new StaticEffect {
//                dbName = "blessing_barricade_brownie",
//                FriendlyName = "Barricade Brownie Bites",
//                Description = "You have eaten a few brownie bites froma Barricade Brownie Bites box.  Its magical sprinkes, now in your stomach, will make you a little more resistant to incoming magic.",
//                AvailableAtLevel = 0,
//                Duration = 6,
//                Cooldown = 10,
//                MessageWhenHit = "You quickly chomp down the brownie bites, your stomach gurgling a little bit in protest but nothing you can't handle.  The sprinkles dissolving into your veins will prevent some magic cast against you from being able to enter your body for a while.",
//                SpellHealthDamageResistance = 10,
//                SpellTFEnergyDamageResistance = 10,
//            },

//            // new StaticEffect {
//            //    dbName = "blessing_",
//            //    FriendlyName = "Barricade Brownie Bites",
//            //    Description = "A small ",
//            //    AvailableAtLevel = 0,
//            //    Duration = 6,
//            //    Cooldown = 9,
//            //    ObtainedAtLocation = "",
//            //    MessageWhenHit = ""
//            //},

//#endregion



//            //new StaticEffect {
//            //    dbName = "",
//            //    FriendlyName = "",
//            //    Description = "",
//            //    AvailableAtLevel = 0,
//            //    Duration = 0,
//            //    Cooldown = 0,
//            //    ObtainedAtLocation = "",
//            //    MessageWhenHit = ""
//            //},
       
       
                
//                };



//            }
//        }

        public static IEnumerable<DbStaticEffect> GetAllStaticEffects()
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName != null && e.dbName != "");
        }

        public static DbStaticEffect GetEffect(string dbFormName)
        {
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return effectRepo.DbStaticEffects.FirstOrDefault(s => s.dbName == dbFormName);
        }

    }

}

