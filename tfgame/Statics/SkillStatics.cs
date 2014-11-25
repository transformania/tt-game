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
    public static class SkillStatics
    {

//        public static IEnumerable<StaticSkill> GetOldStaticSkill
//        {

//            get
//            {
//                return new[]
//                {
//            new StaticSkill {
//                dbName = "skill_busty_blonde_bimbo",
//                FriendlyName = "Blonde Bimbo Bombshell",
//                FormdbName = "form_busty_blonde_bimbo",
//                Description = "It's said that blondes have more fun, and the bigger the bust, the better.  If you don't know one and you'd care to, this spell might take care of that little dilemna.",
//                ManaCost = 5,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion="record_store",
//                DiscoveryMessage="You flip through a box of records. All you see, record after record, are middle aged guys with long-greasy hair and stoned expressions on their faces.  Suddenly you flip to a blonde woman whom you recall named her band after her hair color.  She's rather attractive and you wonder if maybe you two could have hit it off if it weren't three decades after the album's release.  Well, she may not be an option, but with all the chaos going on around town as people like you battle it out, you realize you might just be able to create a few more sexy blondes to please your eyes..."
//            },
//           new StaticSkill {
//                 dbName = "marble_statue",
//                FriendlyName = "Marble Maiden",
//                FormdbName = "marble_statue",
//                Description = "Moving can be overrated.  This spell will shrink its target and replace its skin with marble, a small statuette in the shape of an eternally beautiful woman.",
//                ManaCost = 10,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "park",
//                DiscoveryMessage = "As you walk through the peaceful brick paths of Sunnyglade park, every now and then you spot some pretty stone statues throughout the area, most of them old and suffocated with vines.  Even then they posses an ageless grace, silent yet alluring.  Maybe you can find some people around here to 'volunteer' their humanity so you can have one or two yourself to decorate the foyer back home..."
           
//           },
//           new StaticSkill {
//                 dbName = "lowerHealth",
//                FriendlyName = "Weaken",
//                FormdbName = "none",
//                Description = "While this spell will not transform your opponent on its own, it will lower's opponent's willpower, leaving them vulnerable to other aggressive forms of magic.",
//                ManaCost = 5,
//                HealthDamageAmount = 10

//        }, new StaticSkill {
//         dbName = "skill_Panty_Maker_Judoo",
//         FriendlyName = "Panty Maker",
//         FormdbName = "form_Plain_Pink_Panties_Judoo",
//         Description = "Ever too lazy to run some laundry, or just need a pair of plain old panties here and now?  This spell may solve that problem.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtRegion = "clothing",

//     }, new StaticSkill {
//                 dbName = "skill_bunnyfootsie",
//                FriendlyName = "Bunnyfootsie",
//                FormdbName = "form_bunny_slippers",
//                Description = "Cold feet are unhappy feet.  And bunnies are cute.  How can the two be combined?  Cast this spell a few times and you may just find out.",
//                ManaCost = 7,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "park",
//                DiscoveryMessage = "As you walk around the peaceful Sunnyglade Park, a couple of fuzzy young rabbits jolt out of some bushes.  Startled and worried you might be under attack, you cast a spell that misses horribly.  The rabbits ignore you as they leap around, chasing one another in a instinctive mating ritual.  One almost crashes into your leg and you hop up, afraid of crushing the other.  You avoid harming both innocent creatures, but it gets you to thinking about what a fuzzy bunny might feel like around your feet..."
//     }, new StaticSkill {
//                 dbName = "skill_skateboard",
//                FriendlyName = "Skatemaker",
//                FormdbName = "form_skateboard",
//                Description = "Two legs bad, four legs good.  Four *wheels* are even better than that.  This spell will transform a biped into a bitchin' skateboard.",
//                ManaCost = 9,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 9,
//                LearnedAtRegion = "gas_station",
//                DiscoveryMessage = "You are wandering around when you see an old, weathered skateboard lying discarded by the side of the building.  One of its wheels is missing, but other than that it looked as though it was discarded in its prime.  What a waste of a good board, you think.  If only its owner had treated it better, someone else could be enjoying it right now.  Maybe if they'd spent some time as a skateboard themself they would have.  You smile, an idea forming in your mind... "
//     }, new StaticSkill {
//                dbName = "skill_tight_blue_jeans",
//                FriendlyName = "The Loyal Hip-Hugger",
//                FormdbName = "form_tight_blue_jeans",
//                Description = "Fashion is always coming and going, and buying a new wardrobe every few months is expensive.  Human clothing is always in demand, however.  This spell transforms your opponent into a pair of hip-hugging blue jeans, fashionable but also practical--this once-human weave is incredibly loyal and tends to deflect magic cast against you.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "clothing",
//                DiscoveryMessage = "As you walk between two tall shelves stacked with a grid of cubby holes filled with blue jeans, you get to thinking about this simple but astoundingly popular garment that dates back dozens of decades.  Jeans today might be popular with factory-produced tears and stains, but only because they earned their reputation of being tough.  Loyal, almost, a loyalty found in few humans.  But what about a human turned into blue jeans?  You get to thinking..."

//    }, new StaticSkill {
//                dbName = "skill_dressmaker_jbovinne",
//                FriendlyName = "Dressmaker",
//                FormdbName = "form_dress_jbovinne",
//                Description = "Sometimes you find yourself suddenly female, and in need of a new outfit. Nothing screams ‘sexy’ like a new dress. Well, they’ve usually stopped screaming by the time you’ve put them on. This spell transforms your opponent into a fashionable pink dress.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtLocation="ranch_bedroom_teenager",
//                DiscoveryMessage = "As you look through the bedroom you see something pink moving in the closet.  You slide open the doors to discover a pink prom dress, shiny and comfy looking.  You wonder what the story behind it is, who the girl must have been dancing with when she last wore it.  With the high rate of mages in this town, for all you know, the lucky boy or girl she was with now IS the prom dress.  Maybe not so lucky after all.  No matter what the story is, a spell forms in your mind..."

// }, new StaticSkill {
//                dbName = "skill_latex_legs_jbovinne",
//                FriendlyName = "Latex Legs",
//                FormdbName = "form_latex_legs_jbovinne",
//                Description = "You’ve always envied how good some people look in latex. Skin tight to enhance the curves, while hiding the flaws. The shine catching the eye, and the noise they make as they shimmy past – delightful. Now you can create your own pair out of someone special (or anyone, really).",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtLocation="stripclub_store",
//                DiscoveryMessage = "As you wander through the crowded shelves, you come across a mannequin covered head to toes in black latex, a full-on genuine gimp suit.  It wraps around every curve of her body.  Come to think of it, you don't know for sure that there's a mannequin in there at all--maybe one of your enemies is actually inside lying in wait for you to lower your guard and hit your in the back with a spell or two.  But then again, whoever could be in there would have a hard time moving wrapped up so tightly and certainly couldn't move quietly.  It's more of a prison than a hiding place, really.  But why settle for trapping an opponent inside latex when you could turn them INTO latex?  You grin as a spell begins to form...",

// }, new StaticSkill {
//                dbName = "skill_bigger_bust",
//                FriendlyName = "Bigger Bust or Bust",
//                FormdbName = "form_bigger_bust",
//                Description = "Cast this when delicious double-Ds no longer satisfy your gaze.  Think larger.  Much larger.  Muuuuch larger.  Impossibly large, even.",
//                ManaCost = 7,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 6,
//                LearnedAtLocation="pool_shallow",
//                DiscoveryMessage="You wade into the shallow pool, spotted something red sunk underwater.  You pull it out of the water and examine it to discover that it is actually a bikini top, but the cups are so large that only the most voluptious women could fill it out--DD wouldn't even come close.  You wonder what kind of big breasted woman must have worn it and how she covered up once she lost it.  Maybe you'll come across her.  But you could also turn some of your enemies into someone with an equally large bustline...",

//}, new StaticSkill {
//                dbName = "skill_silver_flask",
//                FriendlyName = "Silver Sipper",
//                FormdbName = "form_silver_flask",
//                Description = "Casting magic is thirst work and hard on your mana.  Meditating is boring and spellweaver petals taste disgusting.  The best source of mana is distilled from the fluids of another human being.  Successfuly casting this against someone ensures you'll always have a gracious friend who will never leave you dry.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 7,
//                LearnedAtRegion="tavern",
//                DiscoveryMessage="You sit down on a bar stool and eye some half-empty glasses of beer and some abandoned shots of tequila on a table across from you.  There is also a flask on it, initials scratched into the side.  It is knocked over, its contents spilled out and drying on the table and floor.  You wonder if it's a lost family heirloom, bequeathed by a father to his sons down generations, or a close friend to another.  There's a better way to keep people you care about close to you---your enemies too, come to think of it.  You sit pondering this, a new spell starting to form into your mind...",

//}, new StaticSkill {
//                dbName = "skill_bunny_love_tfnymic",
//                FriendlyName = "Bunny Love",
//                FormdbName = "form_bunny_love_tfnymic",
//                Description = "Pesky wizard or witch bothering you? Give them something else to think about and send them hopping on their way!",
//                ManaCost = 6,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtLocation="ranch_pasture",
//                DiscoveryMessage="As you stroll through the grassy pasture you come across an opened rabbit cage holding about a dozen fuzzy white rabbits all cuddled together, chewing on the grass.  They eye you and start to hop over one another as you approach, running not away but toward you.  The smallest rabbit even leaps into your lap and thumps a paw against your thigh as if trying to send some kind of message to you.  You wonder how many of these bunnies were human once.  Maybe all, maybe none.  Eventually the bunnies lose interest and hop away again, leaving you alone again.  You feel a little lonely and think about how you could turn one of your peers into a body equally cute and affectionate, though perhaps a bit more human...",

// }, new StaticSkill {
//                dbName = "skill_leopard_bra",
//                FriendlyName = "Leopard Love",
//                FormdbName = "form_leopard_bra",
//                Description = "Feeling feline tonight and back starting to hurt from unsupported breasts?  Use this spell to turn someone into a leopard-patterned bra.",
//                ManaCost = 7,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "streets",
//                DiscoveryMessage = "You see a cat trot across the road ahead of you, a leopard-spotted bengal that moves with the grace of a near relative to a jungle predator.  You admire its agility and grace and remember an old acquaintance you knew who liked to wear clothes of a similar look.  You get to thinking about how you might design a cute little leopard-spotted garment of your own with your transformation magic, maybe send it to her as a gift some day unless you decide to keep it for yourself..."


// }, new StaticSkill {
//                dbName = "skill_99_red_balloons",
//                FriendlyName = "99 Red Balloons",
//                FormdbName = "form_99_red_balloons",
//                Description = "Ever meet an opponent who turned out to be nothing more than a bunch of hot air?  Swap out hot air for helium and this spell will make that expression a bit more literal.",
//                ManaCost = 8,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "coffee_shop",
//                  DiscoveryMessage = "You spot a bundle of yellow and green balloons bobbing around, celebrating the coffee shop's 35th anniversary.  They weave and bob from the current of a humming fan across the room, almost lifelike as though they were struggling to float away and escape.  These might be ordinary balloons, but you wonder what it would be like to feel the tugs of your opposition, turned into shiny red balloons, as they attempt in vain to slip out of your fingers...",

// }, new StaticSkill {
//                dbName = "skill_silk_tube_top",
//                FriendlyName = "Tubularly Titillating Top",
//                FormdbName = "form_silk_tube_top",
//                Description = "Magic is sweaty work and sometimes there's nothing nicer than letting your skin breath.  Use this spell to transform an opponent into a thin, breezy tube top.",
//                ManaCost = 8,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "streets",
//                  DiscoveryMessage = "The summer sun beats down on your body, drenching you with sweat the longer you stay out in it.  You wish you had some nice breezy clothes to cool down with.  A nice tight tube top would do the trick.  Now to find someone you can turn into one...",


//}, new StaticSkill {
//                dbName = "skill_black_stillettos",
//                FriendlyName = "Stilletto Sting",
//                FormdbName = "form_black_stillettos",
//                Description = "A girl can never have too many shoes, or so it is said.  Good shoes need to make a statement, to have a personality.  And while artists can make wonderful imitations, the only true way to achieve this goal is to use an actual person for the inspiration and materials.  This spell will turn your target into a pair of sexy stillettos as unique as they are--or were--human.",
//                ManaCost = 7.5M,
//                HealthDamageAmount = 4.5M,
//                TFPointsAmount = 6.5M,
//                LearnedAtRegion = "bookstore",
//                DiscoveryMessage = "As you walk through the bookstore, you notice the cover of a magazine that shows hundreds of different pairs of shoes, all shapes and types, lined up in a large walk-in closet.  You admire the photographer's effort, how he or she arranged the shoes as if they were all off doing their own little thing like denizens of a miniature metropolis.  Each shoe is unique and hints at a degree of personality.  What if each shoe actually did, the steel-toed boots and the flimsy plastic thong sandles?  You smile, realizing that with your transformation magic you could easily find out...",

// }, new StaticSkill {
//                dbName = "skill_vibrating_latex_thong",
//                FriendlyName = "Latex Underlover",
//                FormdbName = "form_vibrating_latex_thong",
//                Description = "While this spell might make sure your target will never run away or cast magic at you again, it doesn't turn them completely inanimate.  No, they can move quite a bit.  Not very far, mind you, as they will be a vibrating latex thong, but moving nonetheless.",
//                ManaCost = 7.5M,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "You are walking around the Pump-N-Dash when you spot a girlie magazine lying on the ground.  It is flipped to a page advertising sex toys, in particular vibrating panties.  The ad boasts that there is nothing more 'intimate' and 'personal' than a pair of vibrating panties for yourself or your lady love.  You beg to differ--there's nothing more personal than making a pair of vibrating panties out of an ACTUAL person, completely immobile so he or she can focus one hundred percent of his or her energy to pleasuring its master.  A new spell forms in your mind.",
//                LearnedAtRegion = "gas_station",

//                 }, new StaticSkill {
//                dbName = "skill_barmaid",
//                FriendlyName = "Booze Baroness",
//                FormdbName = "form_barmaid",
//                Description = "What's a thirsty guy or gal to do around these parts to wet their lips with a stiff drink?  There just never seems to be enough bartenders and barmaids to pour the booze.  This spell will help to alleviate the shortage.",
//                ManaCost = 8,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "tavern",
//                DiscoveryMessage = "You pace around the Smelly Sorceress Tavern.  The place is pretty quiet, no bartender, bouncers, cooks, or servers in sight.  You sit down on a bar stool and sigh, wondering where all of the employees are--probably hiding from all the transformation chaos raging through the streets.  Or perhaps they are now scattered among the items surrounding you right now--maybe the stool you're sitting on right now was a human only hours ago, enduring or possibly enjoying the feeling of your ass on their face.  Well, wherever everyone has gone off to, a tavern needs its barmaids, and there's plenty of competition you could... magically convince... to fulfill the role.",

//                 }, new StaticSkill {
//                dbName = "skill_succubus_varn1234",
//                FriendlyName = "Hellfire",
//                FormdbName = "form_succubus_varn1234",
//                Description = "This spell launches a fireball at your opponents. The flames however do not burn - judging from the expressions of outright bliss (and lewd actions upon repeated exposure), your victims seem to quite like the experience. Given the spell's source, you suspect each cast is also nibbling away at their soul, in preparation for a final infernal transformation ... and your promised reward.",
//                ManaCost = 6.66M,
//                HealthDamageAmount = 3.33M,
//                TFPointsAmount = 6.66M,
//                DiscoveryMessage="Unwisely walking within the ring of stones, a loud voice echoes within your ears. 'YES, YOU WILL DO, MAGUS. REJOICE IN YOUR GOOD FORTUNE, AS I HAVE DECIDED YOU ARE WORTHY TO BE MY DISCIPLE - I WILL NOT DEVOUR YOUR SOUL AS YOU STAND THERE HELPLESS. TODAY. GO FORTH, AND TRANSFORM OTHERS TO BE PART OF MY LUSTY INFERNAL ARMIES, AND YOUR REWARD SHALL BE GREAT. FAIL AND - LET US JUST SAY THAT YOU SHOULD NOT FAIL.'  Did you imagine it? No one else in the park seems to have heard a thing, even though the voice was loud enough that it should have been audible back on Main Street. And ... you DO seem to have knowledge of a new spell ...",
//                LearnedAtLocation="park_shrine",

// }, new StaticSkill {
//                dbName = "skill_catears_varn1234",
//                FriendlyName = "Cat Ears - Nya!",
//                FormdbName = "form_catears_varn1234",
//                Description = "Upon repeated castings, this spell will - nya! - bring out the feline in your opponents.  Though your libido could do with some sexy cat-people running around, that's not going to help you out much, is it?  So instead, you designed it to turn them into a headband, affixed with the cutest little cat ears, guaranteed to instill some cat-like grace into its wearer.",
//                ManaCost = 9,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtRegion="park",
//                DiscoveryMessage="Wandering through the park, a black cat crosses your path.  A bad omen?  Seeing it run past you, so swiftly and gracefully, you feel as if you must take its abilities for your own.  Perhaps with the 'help' of some of your fellow mages.  So inspired, you are able craft a spell which may turn your opponents into some fancy feline headgear ...",
// }, new StaticSkill {
//                dbName = "skill_mutt_girl",
//                FriendlyName = "Mutt Magic",
//                FormdbName = "form_mutt_girl",
//                Description = "Every woman has been called a bitch before in her life, but with this spell teach those bitches that your bark is a lot less scary than your bite.",
//                ManaCost = 9,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 7.5M,
//                LearnedAtRegion = "oldoak_apartments",

// }, new StaticSkill {
//                dbName = "skill_udder_delight",
//                FriendlyName = "Udder Delight",
//                FormdbName = "form_attachable_udder_varn12345",
//                Description = "You order a cup of coffee (thankfully non-transformative) - and decide that what it really needs is a bit of cream.  You take your cup over to the milk-tank, and fill it up - watching the pure white liquid flow forth in a mighty stream from the spigot.  Isn't milk great?  Don't you wish you could have some whenever you wanted?  And then it hits you - your magic (plus an unfortunate opponent) can make it happen.  With the spell you devise, soon enough you'll have a constant stream of milk - fortified with mana to do your body good.",
//                ManaCost = 6,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "Don't have a cow!  This spell can give an udderly amoosing result.  Jealous of those Blonde Buxom Babes, and those Impossibly-Big Breasted Women?  Go them one better, and craft for yourself your very own Detachable Udder.  With it, you'll never go thirsty again.",
//                LearnedAtRegion = "coffee_shop",

// }, new StaticSkill {
//                dbName = "skill_mousy_time_Elynsynos",
//                FriendlyName = "Mousy Time",
//                FormdbName = "form_catnip_mouse_Elynsynos",
//                Description = "Turns your opponent into a soft cloth mouse toy filled with catnip.",
//                ManaCost = 10,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you look around the back of the bar you notice a mouse running around. This causes you to pause for a moment, you've noticed a couple of.. cattier opponents running around. Maybe a nice catnip mouse would be just the thing to slow them down... ",
//                LearnedAtLocation = "tavern_dumpsters",

// }, new StaticSkill {
//                dbName = "skill_meow_remix_lexamthegemfox",
//                FriendlyName = "Meow Remix",
//                FormdbName = "form_catgirl_raver_lexamthegemfox",
//                Description = "Need a pick-me-up? Need something to really get you moving and grooving? Need something to turn you into a sexy party-going cat girl!? Then look no further.",
//                ManaCost = 7,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 7,
//                LearnedAtRegion = "concert_hall",

//}, new StaticSkill {
//                dbName = "skill_bovine_bounty_sunbro",
//                FriendlyName = "Bovine Bounty",
//                FormdbName = "form_cowgirl_sunbro",
//                Description = " The spell is meant to make the target more docile, friendly, and incredibly less threatening in the brains department. All while add some more beauty and flavor to the world. Who doesn't like cowgirl milk right?",
//                ManaCost = 9,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 7.5M,
//                DiscoveryMessage = "Eyeing some grassy hills outside of the town limits, you notice a herd of cattle grazing a distance away. You start thinking to yourself how even with all the chaos going on they mustn't have a care in the world. This gets you to thinking how that could be a useful trait for your enemies to have.",
//                LearnedAtRegion = "streets",
// }, new StaticSkill {
//                dbName = "skill_sqeak_lexam",
//                FriendlyName = "SQUEAK!",
//                FormdbName = "form_sexdoll_lexam",
//                Description = "Every guy wishes they had a girl that would put our whenever they got horny, now with this wonderful spell make a willing sex doll anytime anywhere......even if the victim wasn't willing to begin with.",
//                ManaCost = 11,
//                HealthDamageAmount = 3.75M,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "You notice a pile of latex on the ground. Upon closer inspection you comes to realize that the flesh colored latex belongs to a deflated sex doll. You grin and lift the pile, finding the nozzle and beginning to blow the doll back up to it's fully inflated size. After you're done you notice the sex toy's face looks rather frightened, almost as if were human at some ponint... which gives you an idea on how to eliminate your opponent and help get rid of your pent up lust.",
//                LearnedAtRegion = "oldoak_apartments",
// }, 
// new StaticSkill {
//                dbName = "skill_brief_blaster_PsychoticPie",
//                FriendlyName = "Brief Blaster",
//                FormdbName = "form_boxer_briefs_PsychoticPie",
//                Description = "Tidy widies can be nice, but nothing feels better than some nice full coverage. This spell transforms it's target into a pair of boxer briefs.",
//                ManaCost = 7,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8.5M,
//                DiscoveryMessage = "As you walk through the store you notice that here, just like most everywhere, the womens' clothes section is usually at least fifty percent larger than the men's.  What a shame that the men don't have as much of a selection to choose from.  You can change that by turning some of your peers into comfy men's apparel.  But what?  Hm... why not start with the basics, a simple pair of briefs...",
//                LearnedAtLocation = "clothing_mens",
// }, new StaticSkill {
//                dbName = "skill_pirates_curse_scipio_africanus",
//                FriendlyName = "Pirate's Curse",
//                FormdbName = "form_pirates_curse_scipio_africanus",
//                Description = "Inspired by the magic of the trapped coin you found, this spell will turn your opponent into a delightfully saucy pirate wench. Ahoy!",
//                ManaCost = 8,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you search around the private room, you find evidence of a rather dramatic brawl at some point earlier.  Among the knife gouges, black-powder stains, blood-spattered playing cards, and scraps of torn clothing, you find a single golden coin left behind in the scuffle.  Examining the coin, you're shocked to feel a current of magic suddenly rush over you, trying to transform you into a buxom pirate wench!  As an experienced Mage, you're able to cut off the effect before much damage is done, but not before getting a glimpse of the structure of the spell involved.  With a little tweaking, you think you might just be able to adapt this booby-trap to your own offensive purposes!",
//                LearnedAtLocation = "tavern_private_room",

// }, new StaticSkill {
//                dbName = "skill_phallus_equus_varn",
//                FriendlyName = "Morpheus Phallus Equus",
//                FormdbName = "form_phallus_equus_varn",
//                Description = "What's a horny, sex-crazed, magically altered girl to do in a city with so few men?  Simple - play for the other team!  And this spell will give her the 'equipment' to do so - turning its unlucky recipient into a fully-functional strapon, shaped like a horsecock attached to two enormous balls overflowing with seed.  You suppose a man looking for a little 'boost' downstairs might be interested as well...",
//                ManaCost = 7,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "A poster adorns the theater walls:  ;War Horse!  Fresh off Broadway, Touring Company Coming to the Oldoak This Summer!  Don't Miss It!;  Hmm, horses, horses... having one to gallop around the city on would sure be nice.  Or you could make a nice horse-morphed stud, or split the difference with a centaur.  And those are just the thoughts you have *before* the words 'Twilight Sparkle' float across your consciousness.  But no - recent events have caused you to be almost entirely focused on sex.  And thinking of equines, that means one word, and one word only:  'Horsecock!'",
//                LearnedAtRegion = "concert_hall",

// }, new StaticSkill {
//                dbName = "skill_Atexlay_Abray_Arrhae",
//                FriendlyName = "Atexlay Abray",
//                FormdbName = "form_black_latex_bra_Arrhae",
//                Description = "You learned this spell out of a lingerie catalog. It allows you to transform someone into a tight-fitting black latex bra.",
//                ManaCost = 7.5M,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "While searching through the lingerie store, you find a magazine cataloging a variety of latex lingerie. Sifting through the pages, you come across the picture of a sexy latex bra. After studying the picture for a few minutes, devise a spell to transform somebody into a duplicate of the bra from the picture.",
//                LearnedAtLocation = "clothing_intimate",

// }, new StaticSkill {
//                dbName = "skill_hanging_around_CCWS",
//                FriendlyName = "Hanging Around",
//                FormdbName = "form_straight_tie_CCWS",
//                Description = "This spell will transform its target into a red silk tie - Perfect for a more professional (or perhaps, slutty) look!",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "As you look around the store, you notice a bored looking sales assistant in a collared shirt and tie. Chuckling, you remember how uncomfortable they tend to be, even if they do spruce up a person's image. That's when the thought hits - What if instead of a tight and constricting usual tie, you owned a tie with a little... Life? Perhaps you could find an opponent to 'volunteer' for your experiment...",
//                LearnedAtRegion = "record_store",

// }, new StaticSkill {
//                dbName = "skill_sneaker_shot_PsychoticPie",
//                FriendlyName = "Sneaker Shot",
//                FormdbName = "form_athletic_sneakers_PsychoticPie",
//                Description = "The problem with footwear today is that it's all too flashy and cumbersome. This spell transforms its target into a practical pair of sneakers.",
//                ManaCost = 7,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you're scrounging around the Sweater Girls Gym for goodies, you notice your feet have become sore. Whatever happened to the days when footwear was simple? 'I know just the thing to fix this problem,' you think to yourself, as you craft a new spell in your head.",
//                LearnedAtRegion = "gym",

// }, new StaticSkill {
//                dbName = "skill_tip_of_the_hat_Zatur",
//                FriendlyName = "Tip of the Hat",
//                FormdbName = "form_black_silk_top_hat_Zatur",
//                Description = "Whether you need to put on the Ritz or perhaps perform some stage magic, you couldn't pull it off without style. And what's more stylish than a black silk top hat? Good thing you have this spell to make for you the perfect accessory!",
//                ManaCost = 7,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 7,
//                DiscoveryMessage = "Among the old, faded posters is one advertising a show to be performed by a renowned Magician. Well, with everything going on around you lately, stage magic doesn't really sound that mystical to you. Or does it? There's a flair in the way a magician holds the audience attention, as they misdirect them to pull off a sleight of hand that leaves others speechless. Maybe that's something you could use. Some style. Some panache. A really cool hat. And besides, with all these bunny girls walking around, having a top hat might be a great conversation starter...",
//                LearnedAtRegion = "concert_hall",

// }, new StaticSkill {
//                dbName = "skill_willen_dorfus_varn",
//                FriendlyName = "Willen Dorfus",
//                FormdbName = "form_fertility_idol_varn",
//                Description = "The joy of pregnancy - this spell can bring it to those around you.  Your initial victim will get to experience it perpetually - reduced to a stone idol of a pregnant woman, frozen in time mere days away from delivery.  However, they will be able to spread the joy, passing on the physical appearance of pregnancy to any who hold them... perhaps even to you.",
//                ManaCost = 8,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "An illustrated book on archaeology catches your eye - it claims to illustrate life in a Stone Age village.  You imagine what you might find within its pages.  Maybe a brawny cavewoman strong enough to take down all around her, while still looking sexy in a fur bikini?   Hmm, no... no pictures of people to inspire you.  You see cave paintings.  Stone tools.  Foundations where primitive buildings once stood.  You are about to throw the book down in disgust, when you see it - a stone idol, carved in the shape of a voluptuous and VERY pregnant woman.  You imagine how it must feel, to be so swollen with new life... no, better, to be frozen that way, perpetually over nine months pregnant, frozen in time... and resolve to bring that joy to those around you.",
//                LearnedAtRegion = "bookstore",


// }, new StaticSkill {
//                dbName = "skill_take_root_Sherry_Gray",
//                FriendlyName = "Take Root",
//                FormdbName = "form_enchanted_tree_Sherry_Gray",
//                Description = "Strong roots will dig into the ground, leaving your opponent completely immobile.  They will remain a sentient mighty oak, a living defender of nature.",
//                ManaCost = 7,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "In your search of the park, you come upon an ancient willow.  Drawn in by whispers in the wind, you find yourself at its base.  You feel a mental connection with the tree as flashes of runewords form on its trunk, spelling out an ancient forest spell.",
//                LearnedAtRegion = "park",

//  }, new StaticSkill {
//                dbName = "skill_schooltop_bop_PsychoticPie",
//                FriendlyName = "Schooltop Bop",
//                FormdbName = "form_schoolgirl_top_PsychoticPie",
//                Description = "There's something special about the schooldays of youth. This spell transforms its target into a cute little button up schoolgirl top.",
//                ManaCost = 10,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you're searching Saint Circe Community College, you begin to reminisce about your schooldays. A time of innocent, youth, freedom... wouldn't it be lovely if you could recapture those feelings? You conjure a new spell that will do just that.",
//                LearnedAtRegion = "college",

//  }, new StaticSkill {
//                dbName = "skill_schoolbottom_bop_PsychoticPie",
//                FriendlyName = "Schoolbottom Bop",
//                FormdbName = "form_schoolbottom_bop_PsychoticPie",
//                Description = "Do you wish you could've been a more popular girl in your schooldays? Do you wish you could've gotten more chicks back then?  This spell transforms its target into a cute, pleated plaid, red, short schoolgirl skirt. ",
//                ManaCost = 10,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you're searching Saint Circe's Community College, you notice a girl walk by in a schoolgirl outfit, with a particularly short skirt. You could find many uses for one of those, and you know the perfect way to acquire one.",
//                LearnedAtRegion = "college",

//  }, new StaticSkill {
//                dbName = "skill_studious_sucker_christyd",
//                FriendlyName = "Studious Sucker",
//                FormdbName = "form_naughty_schoolgirl_christyd",
//                Description = "Everyone deserves a higher education and now you can help your fellow spell-casters obtain it - whether they want it or not. This spell will turn your target into a voluptuous red-haired schoolgirl, eager to get her grades (and other things) up. Though, she may not be very good at studying there are other ways of 'sucking up' to her professors...",
//                ManaCost = 8,
//                HealthDamageAmount = 3.5M,
//                TFPointsAmount = 8,
//                LearnedAtLocation = "college_humanities",

//  }, new StaticSkill {
//                dbName = "skill_Vibrating_vex_PsychoticPie",
//                FriendlyName = "Vibrating Vex",
//                FormdbName = "form_the_pink_pulsar_PsychoticPie",
//                Description = "Regardless of your sex, a vibrator is a useful thing to have around. This spell transforms its target into a soul powered vibrator.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtLocation = "clothing_intimate",

//  }, new StaticSkill {
//                dbName = "skill_liquify_Arrhae",
//                FriendlyName = "Liquify",
//                FormdbName = "form_liquify_Arrhae",
//                Description = "This spell will turn its target into a more fluid form.",
//                ManaCost = 8,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 8,
//                LearnedAtLocation = "lab_secret_laboratory",

//  }, new StaticSkill {
//                dbName = "skill_glass_cannon_psychoticpie",
//                FriendlyName = "Glass Cannon",
//                FormdbName = "form_cateye_glasses_psychoticpie",
//                Description = "Some people have mediocre eyesight and use glasses to enhance their vision. Other people are just turned on by glasses! No matter the need, this spell transforms its target into a pair of Cat Eye Glasses.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                LearnedAtRegion = "lab",

//  }, new StaticSkill {
//                dbName = "skill_fox_fire_Elynsynos",
//                FriendlyName = "Fox Fire",
//                FormdbName = "form_kitsune_Elynsynos",
//                Description = "The spirit, a will-o-wisp you think it's called, has given you this spell to help restore the shrine. However, you are a magi, one of the wise, and in this city you need your targets to be a little more... feisty.",
//                ManaCost = 12,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 10,
//                DiscoveryMessage = "As you walk around the strange shrine in the park the world grows quiet for a moment. A wave passes over you from behind and everything around you changes; the statues are no longer broken and decaying, cherry blossoms surround you filling the air with their beautiful flowers, and a spirit hangs before you. 'Mage, you are looking at what once was, but this shrine is long forgotten. We need new maidens to restore its former glory and for this reason I shall grant you a spell,' and with that everything returns to normal. Taking a moment though you realize you do indeed have knowledge of a spell you didn't know before.",
//                LearnedAtLocation = "park_shrine",

//  }, new StaticSkill {
//                dbName = "skill_foryouramusements_WhiteflameK",
//                FriendlyName = "For Your Amusement...Park",
//                FormdbName = "form_miniature_horse_whiteflameK",
//                Description = "Inspired by Sunnyglade's old carousel, this spell is one of the darker and more devious of your creations, but it promises to be very useful for removing a contestant from their 'high horse' and giving you the opportunity to ride your way to victory. You can sense that the final form have some particularly useful benefits for yourself as well.",
//                ManaCost = 10,
//                HealthDamageAmount = 4.5M,
//                TFPointsAmount = 8.5M,
//                DiscoveryMessage = "As you wander about the park, brainstorming new devious ways to best your opponents, you think that some animal transformation would be viable - knock out their ability to compete mentally, and they won't be able to resist the changes. Off in the distance, you see Sunnyglade's old carousel, and a brilliant idea strikes you. Why not kill two birds with one stone...erm, spell? An animal transformation might be mentally draining, but why not benefit in the process by turning them into a useful object while they are distracted and defenseless? They might even enjoy it, until they realize what is happening to them. It gives you chills, thinking of their panic when they realize how you've beaten them.",
//                LearnedAtLocation = "park_merrygoround",

//  }, new StaticSkill {
//                dbName = "skill_cheerus_leaderius_Haretia",
//                FriendlyName = "Cheerus Leaderious",
//                FormdbName = "form_Cheerleader_Haretia",
//                Description = "Feeling down lately? Wanna have something to cheer you up? Maybe your own personal Cheerleader? Then this is the spell for you! *Warning cheerleader my not be pleased/grateful. Use on your own risk.",
//                ManaCost = 7,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 7.5M,
//                DiscoveryMessage = "Seeing those beautiful girls train makes you wish you could have your own. But if one of them would be missing that might be suspicious. So what to do? Just make your own! How hard can that be?",
//                LearnedAtLocation = "college_track",

//  }, new StaticSkill {
//                dbName = "skill_lingerie_inveigh_PsychoticPie",
//                FriendlyName = "Lingerie Inveigh",
//                FormdbName = "form_living_lingerie_PsychoticPie",
//                Description = "Need some stockings to keep your legs warm, or just want to look fabulous and sexy for the right guy? This spell transforms its target into a slightly transparent pair of panties with garter straps and back beam stockings.",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "As you're searching inside the freezer, your legs get chilly and before long your skin starts to feel a little numb.  'I wish I had something to keep them warm' you think to yourself.  Wait, you're a magician, right? Why don't you just make something to keep them warm?  Using a humanoid soul, preferably.",
//                LearnedAtLocation = "lab_freezers",

//    }, new StaticSkill {
//                dbName = "skill_hay_fever_lexam_hachik",
//                FriendlyName = "Hay Fever",
//                FormdbName = "form_hung_stallion_lexam_hachik",
//                Description = "Tired of your noisy old skateboard? Know someone who would appreciate running free without a human care in the world? Then use this spell on them and soon you will have a beautiful stallion to carry you around.",
//                ManaCost = 10,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 5,
//                DiscoveryMessage = "You can't help but admire the powerful horses in the fields pictured in a poster on the wall of the clinic. You smile at all the mares, their bellies round with foals. Their master was a proud black stallion cantering inside his harem and you get to thinking. If you make that master your pet, how much more powerful would you appear to others?",
//                LearnedAtLocation = "college_vet",

//  }, new StaticSkill {
//                dbName = "hey_listed_Varn",
//                FriendlyName = "HEY! LISTEN!",
//                FormdbName = "form_fairy_familiar_Varn",
//                Description = "This spell will transform its victim into a 3-inch high, flying woman - a fairy.  Make no mistake though; this will end their career as a magic duelist - the spell will bind them forever more to you (or any who defeat you) as a familiar.  You can forsee no downside to gaining such an ally...",
//                ManaCost = 8,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 8,
//                DiscoveryMessage = "Walking along the wooded area, you see any number of small, winged creatures.  Dragonflies flitting about.  Hummingbirds seeking nectar.  Ouch! Mosquitoes.  You consider... having a small, flying familiar might be just the thing to give you the edge in this transformation battle. You just know your new pet will surely be useful, and in no way highly annoying...",
//                LearnedAtLocation = "park_boardwalk",

//  }, new StaticSkill {
//                dbName = "skill_Big_Burly_Brute_Varn",
//                FriendlyName = "Brutemaker",
//                FormdbName = "form_big_burly_brute_Varn",
//                Description = "The city is full of hot, horny, magically transformed women - most of them out on the prowl looking for a man.  Who are you to deny them?  This rare spell will produce a MAN - a musclebound study to satisfy the lusty female hordes.  And while you technically could cast it on a dude, that would be *such* a waste, now wouldn't it?",
//                ManaCost = 6,
//                HealthDamageAmount = 4,
//                TFPointsAmount = 10,
//                DiscoveryMessage = "As you walk around, glancing at all the transformation chaos filling the streets, you suddenly dodge a stray spell that you were sure would have turned you female.  It seems the city is crawling with hot, horny, magically affected women - nearly all of them looking for a man - though the masculine gender seems to be in very short supply around here.  It suddenly hits you - you can help change that! - and you quickly devise a spell to create a hyper-masculine, musclebound stud.  One that would empower the scrawniest male nerd into a tower of testosterone, capable of satisfying a whole horde of Busty Blonde Bimbos.  But of course... you don't have to cast it on a man...",
//                LearnedAtRegion = "streets",
//},

//  new StaticSkill {
//                dbName = "wail_wereleopard__alessandro",
//                FriendlyName = "Wail of the Were-Leopard",
//                FormdbName = "form_were_leopard_alessandro",
//                Description = "The spell begin the transition of the Female Were-leopard upon all who are hit by it and the desire to in return change others.",
//                ManaCost = 9.25M,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 10,
//                 DiscoveryMessage = "You shuffle around the science classroom and by chance open up a book on biology, specifically zoology.  As you flip through the pages, an almost mystic force makes one page stand out with the image of a majestic leopard.  You gaze at it, your mind slipping into some kind of delusion in which you are walking through a thick jungle.  As you stumble through, sweating in the humidity, you come across a leopard resting in the branches of a tree in front of you.  She stares at you and you hear in your mind 'Mortal... you shall help rekindle my dying species, the were-leopard.'  She leaps away and the illusion is gone, but a new spell lingers in your mind...",
//                LearnedAtLocation = "college_sciences",

//  }, new StaticSkill {
//                dbName = "skill_enmaiden_Ellipsis",
//                FriendlyName = "Enmaiden",
//                FormdbName = "form_maid_Ellipsis",
//                Description = "There's just too much initiative in the world, isn't there? There's an overflowing surplus of deep thinking and independent choices. Doesn't it just brittle your kneecaps? Well, worry no more! This pretty little spell will take away one critically-minded individual and replace them with a meek, submissive maid. And hey, you'll also cut down on the mess around here!",
//                ManaCost = 9,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 7.5M,
//                DiscoveryMessage = "Trailing your finger along the banister, you frown as your notice the steady building of gray on the tip. Surely, they can hire someone to take care of all this dust... can't they?  Maybe you can help. In fact, while your at it, you've thought of the perfect way to cut down both on messes...and on competition.",
//                LearnedAtLocation = "apartment_rental_office",

//  }, new StaticSkill {
//                dbName = "skill_Masked_Soul_Haretia",
//                FriendlyName = "Masked Soul",
//                FormdbName = "form_ball_mask_Haretia",
//                Description = "You recreated the mask from your youth. But now that you wield magic a simple mask won't do. This spell helps you to bind a soul to your mask to create a magical sparkly masquerade ball mask.",
//                ManaCost = 6.5M,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 8.5M,
//                DiscoveryMessage = "As you enter the old concert hall you notice some old posters for different events are hung up. Many different plays or orchestra had their home here. However it is a poster for a masquerade ball that catches your eyes. It reminds you of a masquerade dance you had in high-school. Somehow it appeals to you more than the prom-dance you had later that year. You especially liked the mask you had: a red one with red and black feathers at one side and small gemstones all over it. But who knows where your old mask is now, so you decide to make a new one, this time with more 'personality'.",
//                LearnedAtRegion = "concert_hall",
  
//  }, new StaticSkill {
//                dbName = "skill_Haute_Couture_Budugu2004",
//                FriendlyName = "Haute Couture",
//                FormdbName = "form_maid_dress_Budugu2004",
//                Description = "This pompous spell helps you turn your opponent into a masterpiece of French couture, a maid dress! It will use your opponent pure essence to weave it into the most delicate jet black satin and sew it into a sexy and not so functional maid dress. Once the dress is don, its wearer will be amazed how comfortable it is, but being on maid's duty is not a menial task. It requires dedication, even to unwanted guests this dress seems to attract!",
//                ManaCost = 11,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "You spot an old dusty oak chest in the room. You open it and find a huge pile of clothes all mixed up. As you start searching for magical items, a black piece of cloth catch your attention. You pull on it and uncover long black dress, stiff, with long sleeves a white collar. It looks like one of those old English maid dress. 'Too bad it's not a French maid dress, those are the real deal!' You think. Almost instantly, your twisted mind come out with a spell to make a cute French maid dress out of your next target.",
//                LearnedAtRegion = "bookstore",

//  }, new StaticSkill {
//                dbName = "skill_maidus_bindus_Haretia",
//                FriendlyName = "Maidus Bindus",
//                FormdbName = "form_bondage_kitten_magazine_Haretia",
//                Description = "If you would love to have your own slave girl dedicated only to you, then this is the spell for you.  Inspired by a sexy redhead on a magazine cover you saw, you are fairly certain that you can create an equally beautiful and dedicated creature.  Well fairly certain. What can go wrong right? Maybe you shouldn't try this spell on a friend after all.",
//                ManaCost = 7,
//                HealthDamageAmount = 4.2M,
//                TFPointsAmount = 7.5M,
//                DiscoveryMessage = "As you browse the magazines in the adult section of the bookstore you find a fetish model that pleases your eyes: a twenty-something with bright red hair and green eyes. You just have to have her.  Maybe your magic can help you with that.",
//                LearnedAtLocation = "bookstore_back",

//  }, new StaticSkill {
//                dbName = "skill_reflecius_fabricos_Haretia",
//                FriendlyName = "Reflecius Fabricos",
//                FormdbName = "form_sparkly_cocktail_dress_Haretia",
//                Description = "Every girl needs that one dress. The one that not only highlights every curve you've got. A dress that makes you stand out in a crowd, mezmerizeing everyone. Not just a pece of cloth, but something with a real soul sewn into it. If you still missing such a fine dress then this spell is for you.",
//                ManaCost = 12,
//                HealthDamageAmount = 4.5M,
//                TFPointsAmount = 7.5M,
//                DiscoveryMessage = "Looking through old props you find a battered discoball.  While you clean it up you think about the disco era and the many strange clothes, dance moves and dresses. Suddenly and idea forms in your mind. The same mezmerizing property of the discoball with the provoking look of a sinful cocktail dress.  Shouldn't be hard to create a spell for it.  Smirking, you start the search for your new dress.",
//                LearnedAtLocation = "concert_hall_backstage",

//  }, new StaticSkill {
//                dbName = "skill_mouse_trap_Danaume",
//                FriendlyName = "Adorable Mouse Trap",
//                FormdbName = "form_mouse_boy_Danaume",
//                Description = "Are you a man, or are you a mouse... it doesn't really matter, with this spell, your opponent can be both! Although let's be honest, he won't be much of a man once you are done with him.",
//                ManaCost = 10,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "You poke around the dumpster looking for anything useful, but all you find is a nest of mice living off the discarded food from the tavern's patrons. There is a sprung mouse trap under the dumpster, which appears to have failed to catch anything. What's the saying, build a better mouse trap, and the world will beat a path to your door? Hmm... a mouse trap...",
//                LearnedAtLocation = "tavern_dumpsters",

//  }, new StaticSkill {
//                dbName = "skill_Lloths_Caress_XanKitsu",
//                FriendlyName = "Lloth's Caress",
//                FormdbName = "form_Drow_Priestess_XanKitsu",
//                Description = "This spell infuses your target with the deadly grace of the Dark Elves. Draping them in the sensual raiments of the Priestesshood of Lloth, you turn them into sensual beings of lust and dominance. You only hope that after they decide to leave you be, accepting your offering to the Spider Queen.",
//                ManaCost = 12,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "Standing in the shed you spy a spider web built in the corner. A large Spider sits in the center, and you can swear it was staring at you. As you lock your eyes to its eight, you hear the sensual voice of a woman in your head. 'Spread the faith, bring my people back into power on the surface.' You feel the words of power flow into your mind, and you know how to bring her Priestesses into being.",
//                LearnedAtLocation = "park_toolshed",

//  }, new StaticSkill {
//                dbName = "skill_mon_lupe_Alessandro",
//                FriendlyName = "Mon Lupe",
//                FormdbName = "form_wolf_cub_Alessandro",
//                Description = "Casting this spell will change your foes into a  wolf cub that follows you faithfully into battle and beyond. Loyal to his master or mistress until inanimation do they part.",
//                ManaCost = 12,
//                HealthDamageAmount = 3,
//                TFPointsAmount = 7.75M,
//                DiscoveryMessage = "Leafing through a science magazine lying in the corner of the room halfway fallen behind a shelf, you see a page where wolves tear a deer apart. This sparks inspiration to have a cub of your own. Thinking that you won't be able to just take one from the wild why not make one out of the countless horde of other mages and witches around you.",
//                LearnedAtLocation = "college_vet",

//  }, new StaticSkill {
//                dbName = "skill_its_a_trap_lexam",
//                FriendlyName = "It's a Trap!",
//                FormdbName = "form_feminine_slut_boy_Lexam",
//                Description = "Getting tired of that over abundance of slutty women, furries, and other creatures? Well lets add some diversity and make cute little submissive femmy boys as well!",
//                ManaCost = 9,
//                HealthDamageAmount = 5,
//                TFPointsAmount = 9,
//                DiscoveryMessage = "As you walk through the pool, thinking of how bikini-clad women must swarm the area on regular days, you can't help but realize that there's quite an abundance of women around the city.  You think that it is time for a change. You imagine cute boys running around, looking girly and much less threatening to you because they would be so horny and submissive.",
//                LearnedAtRegion = "pool",

//}, new StaticSkill {
	
//         dbName = "skill_Jungle_Fever_Lexam",
//         FriendlyName = "Jungle Fever",
//         FormdbName = "form_Tribal_Girl_Lexam",
//         Description = "Delve deep into the deepest parts of the jungle and discover a cute girl from a long lost tribe, but mind your manners or she just might make a tribe sister out of you with her trusty blow dart gun.",
//         ManaCost = 10,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 9,
//         DiscoveryMessage = "You look around for inspiration, yearning to learn a new spell. You begin to notice some old props from what looks like a deep jungle themed movie set and you get to thinking about the cute, scantily clad  girls that run around in the jungle away from civilization. Mabye it would be cute to see some of them running around the city....",
//         LearnedAtLocation = "pool_concessions",


//}, new StaticSkill {
//         dbName = "skill_Feather_Flight_Medli",
//         FriendlyName = "Feather Flight",
//         FormdbName = "form_Dainty_Birdgirl_Medli",
//         Description = "Caution - Casting this spell may cause your target to: Lay eggs, Squawk, Grow Feathers, and Crave bread crumbs, seeds, and worms. Typically should be used if you'd like to see your enemy transformed into a helpless bird...of course there's always the off-chance that they'll peck your eyes out.",
//         ManaCost = 8,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "You're walking on the grass around the playground when suddenly a pile of white bird crap falls inches from you. Noticing a flock of birds flying overhead, gives you an idea...",
//         LearnedAtLocation = "pool_playground",

//}, new StaticSkill {
//         dbName = "skill_Sea_of_frills_Budugu2004",
//         FriendlyName = "Sea of Frills",
//         FormdbName = "form_Frilly_Petticoat_Budugu2004",
//         Description = "Want to get in touch with your girly side? Why not use frills? A lot of frills? Well, this spell will fulfill all your desires and turn your opponent into a white frilly petticoat! You'll feel rejuvenated! But be cautious, this petticoat tends to defy gravity and will flashes the panties you are wearing, or not wearing, to anyone crossing your path. ",
//         ManaCost = 7,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "While walking between the aisles, you hear a girly giggle behind you. You quickly jump around, ready to fight, but you see nothing. You decide to go back your way, but brushes past a petticoat hanging on a rack while turning back. Once again, you hear a faint giggle, realizing it's coming from the petticoat. It's then you notice a small piece of paper on the ground.",
//         LearnedAtLocation = "clothing_womens",

//}, new StaticSkill {
//         dbName = "skill_Sgnikcots_Tenhsif_Zatur",
//         FriendlyName = "Sgnikcots Tenhsif",
//         FormdbName = "form_Fishnet_Tights_Zatur",
//         Description = "What's better than legs clad in tights? When those tights are full of holes like a net!  Disclaimer: Do not use as a fish net.",
//         ManaCost = 7.00M,
//         HealthDamageAmount = 3.00M,
//         TFPointsAmount = 7.00M,
//         DiscoveryMessage = "While perusing the shelves, your eyes light up on seeing some particular comics. Upon one of the covers you spot a certain well known magical heroine. Well, marginally well known. Still, you can't help recall how not only interesting she was, but how sexy her costume was. The modified magician's outfit with those fishnet tights... Hrm, speaking of which, that gives you an idea for a spell.",
//         LearnedAtLocation = "comicstore_comics",

//}, new StaticSkill {
//         dbName = "skill_Witch's_Friend_Blood_Knight",
//         FriendlyName = "Witch's Friend",
//         FormdbName = "form_Familiar_Feline_Blood_Knight",
//         Description = "Turns the target into a witch's familiar.",
//         ManaCost = 10,
//         HealthDamageAmount = 3.1M,
//         TFPointsAmount = 7.75M,
//         DiscoveryMessage = "Wandering the abandoned Estate you come across a black cat. It hisses at you before clawing your leg. As you bend down to investigate your wound you see images of the moon, and cat's eyes. As you do the workings of a spell weave themselves into your mind.",
//         LearnedAtRegion = "mansion",

//}, new StaticSkill {
//         dbName = "skill_Curse_of_Straw_Rust",
//         FriendlyName = "Curse of Straw",
//         FormdbName = "form_Cursed_Doll_Rust",
//         Description = "What kind of person would even think of a spell like this? To use it one would have to be even less than human. Drain your foes life essence from their body leaving nothing but a cursed doll behind.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Searching through the dusty shelves, you come across an older looking book sitting out of place with the rest. Curiosity gets the better of you and pulling it down you start to read through it. To your surprise it is actually a book of spells though the effects of them disturb you. Each spell becomes worse than the last as you continue until finally you shut the book in disgust seeing fit to hide it so no one else will find it. You try to lock away memories of what you read but one still remains in your mind...",
//         LearnedAtLocation = "mansion_study",

//}, new StaticSkill {
//         dbName = "skill_Whipcrack_Christy_D",
//         FriendlyName = "Whipcrack",
//         FormdbName = "form_Leather_Whip_Christy_D",
//         Description = "This spell turns it's victim into the age-old tool of dominatrixes everywhere- a sturdy leather whip.",
//         ManaCost = 7,
//         HealthDamageAmount = 4.5M,
//         TFPointsAmount = 7M,
//         DiscoveryMessage = "While searching through the master bedroom, you come across an old leather whip halfway under the bed, creeping through the sheets like an asp.  You can't tell if it was meant to be real or ornamental, but either way it sure looks vicious.  What kind of pervaded fun did the master have with his wife or his mistresses?  You'll never know, but a spell forms in your mind...",
//         LearnedAtLocation = "mansion_bedroom",

//}, new StaticSkill {
//         dbName = "skill_Mirror,_mirror_Budugu2004",
//         FriendlyName = "Mirror, Mirror",
//         FormdbName = "form_Magic_choker_Budugu2004",
//         Description = "Unlike the famous fairy tale, this powerful arcane spell will trap the soul of your enemy in a small, very small glass prison, small enough to be tied around your neck with a small ribbon. The greatest wizard will be able to subdue the soul of the victim into servitude, turning it into a loyal companion and adviser. So, tell me, choker, choker, on the neck, who will be my next target?",
//         ManaCost = 9M,
//         HealthDamageAmount = 4M,
//         TFPointsAmount = 8M,
//         DiscoveryMessage = "While searching through all the dusty clutter in the servants' quarters, you stumble upon a very old mirror on the floor. You take a second to examine it. It's a round wall mirror with a cherry wood frame. The woodwork in the frame is very detailed. You don't know why, but it reminds you of the evil sorceress' mirror in the Snow White fairy tale. If only you could trap the soul of someone into a mirror, or maybe something more convenient, you would also have a magical mirror to answer all your questions...",
//         LearnedAtLocation = "mansion_servant_quarters",

//}, new StaticSkill {
//         dbName = "skill_Crown_of_order_Budugu2004",
//         FriendlyName = "Crown of Order",
//         FormdbName = "form_Maid_Headband_Budugu2004",
//         Description = "This curse will turn your opponent in a frilly maid garment, a headband. The poor soul will be forever caught in a never-ending spiral of torment, disgusted by the tinniest wrinkle on a bed sheet or dust on the highest shelf. If you dare to put that headband on your head, it will spot for you all these unacceptable messes, including your filthy opponents. On the downside, it will slowly damage your will.",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Sighing, you sit down on a dining room chair and feel a little notebook underneath you.  Curious, you pick it up and are delighted to see that it is filed with spells, though you quickly realize they are mostly useless. Near the end of the book, you stumble upon a strange spell you never hear about, Crown of order, which turns a target into a frilly maid headband. It is written this forbidden spell was created by a powerful mistress exasperated by her bimbo maid. She transformed a long-life rival into a mind-controlling device to help her maid remember doing all her daily chores. The pour soul trapped into the headband sought revenge and one day, the mistress asked the maid for a haircut. The headband convinced the maid it was misplaced and belonged onto mistress' head. The second it was put on the mistress' head, the headband infiltrated her mind, breaking her will instantly and turning her into a mindless cleaning slave. Legend says the immortal witch still travels the world in a maid uniform, trying to clean even the tiniest rock on her path. Moral of the story, choose and handle your target carefully. ",
//         LearnedAtLocation = "mansion_dining",

//}, new StaticSkill {
//         dbName = "skill_Sarmoti_Zatur",
//         FriendlyName = "Sarmoti",
//         FormdbName = "form_White_Tiger_Zatur",
//         Description = "If you're going to perform magic, than you're going to need something to dazzle the audience. Something worthy of Vegas. And nothing screams Vegas and Magic like a White Tiger to help you perform tricks! Well, more of a roar, but you get the picture.",
//         ManaCost = 7,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7M,
//         DiscoveryMessage = "Perusing the albums of the record store, you come across an odd find. The debut album for a glam metal band called White Tiger. The album, in a stroke of creativity, is simply called White Tiger, with a roaring tiger in the middle of the letters. Looking at the pictures of the band members, you can't help but wonder what was wrong with the 80s, but at least this gives you an idea. How great would it be to have your own white tiger as a pet? And if things don't pan out here, you could always take them to Vegas with you...",
//         LearnedAtLocation = "record_store_front",

//}, new StaticSkill {
//         dbName = "skill_Bimborb_Crystalis_Varn",
//         FriendlyName = "Bimborb Crystalis",
//         FormdbName = "form_Magic_Slut_Ball_Varn",
//         Description = "What, you may ask, is a Magic Slut Ball?  Certainly, a crystal prison for one of your deadly foes, from which they can NEVER escape - but isn't it so much more?  Not only is a tiny, constantly horny bimbo forever encased in a crystal orb a fine conversation piece - but you suspect she could grow to be quite a fine pet.  The outlook is hazy, but you can even imagine her whispering the secrets of the universe to you as you shake her ball ... ",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "You see an odd shelf in the store, filed with vintage novelty toys.  X-ray specs, plastic vampire fangs, a slinky, a Rubik's cube, silly putty - and sitting right in the middle, an over-sized black pool ball, with a small window carved out of its back.  You pick it up and shake it vigorously, asking 'Oh, Magic 8-ball - will I ever find the perfect spell to incapacitate my enemies, yet keep them around for further humiliation?  A spell to create a perfect little useful pet, who may never escape my control, one who I could just hold ... in my hands ...'  Even before the 20-sided die inside settles on 'All Signs Point to Yes' - the idea is born, and you have half-composed the spell...",
//         LearnedAtLocation = "comicstore_gaming_room",

//}, new StaticSkill {
//         dbName = "skill_Pans_Pipes_Martiandawn",
//         FriendlyName = "Pan's Pipes",
//         FormdbName = "form_Randy_Satyr_Martiandawn",
//         Description = "Wine, women, and song, afford such easy distractions in a troubled world. A rival obsessed with the pursuit of such pleasant diversions would surely pose less of a threat, no?",
//         ManaCost = 7,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "From somewhere in the distance, the sound of someone playing a flute reaches your ears, a merry, lilting tune carried upon a gentle breeze. No, not a flute… it sounds more like pipes, the preferred instrument of the ancient goat-god Pan. For a moment your imagination takes you back to a simpler time. Pan cavorts across a grassy meadow upon cloven feet, playing his pipes as naked women and randy satyrs engage in a drunken orgy with lustful abandon. The air is heavy with the smell of sex and spilled wine, arousing the lust any who venture too near. Wait – that gives you an idea for a spell!",
//         LearnedAtLocation = "park_duck_pond",

//}, new StaticSkill {
//         dbName = "skill_Spoogebringer_Blood_Knight",
//         FriendlyName = "Spoogebringer",
//         FormdbName = "form_Runic_Dildo_Blood_Knight",
//         Description = "Calling upon powers of Chaos from before the current universe you twist your target into the form of a handheld demonic dildo.",
//         ManaCost = 9,
//         HealthDamageAmount = 3.14M,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "As you walk amongst the graves of old, you see one different than the rest, a sarcophagus laying above the ground. Drawn to it as if by some untold force you touch it and see for a moment a hero of old, peer to the king, beset on all sides by demons. His form shifts until he is nothing more than one of their weapons, and you smile realizing it shouldn't be difficult to do the same to your foes.",
//         LearnedAtLocation = "mansion_mausoleum",

//}, new StaticSkill {
//         dbName = "skill_Vulpes_Zerda_Elynsynos",
//         FriendlyName = "Vulpes Zerda",
//         FormdbName = "form_Fennec_Fox_Elynsynos",
//         Description = "Tired of opponents running and hiding from you? This spell has just what you need! An adorable fox pet with a nose that rivals the best hunting dogs and ears to hear the smallest of sounds! This loyal guardian will help you not only hunt down your foes, but also keep you out of danger by alerting you their presence before they can get the drop on you.",
//         ManaCost = 8,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Walking around the park you smile at all the animals wandering around. You notice a rabbit jolt through the brush and off further into the woods. Curious you hang around to see what spooked it, and laugh as a fox slowly trots along behind it. Sniffing the air and following its trail back to the den. That gives you an idea though. Your opponents sometimes sneak off and hide on you, and a wonderful hunting pet might do just the trick against them.",
//         LearnedAtRegion = "park",

//}, new StaticSkill {
//         dbName = "skill_Kiss_of_the_Spider_Woman_JBovinne",
//         FriendlyName = "Kiss of the Spider Woman",
//         FormdbName = "form_Female_Dryder_JBovinne",
//         Description = "Some people in this town are creeps.  And some could do with being even more 'creepy'.  Bring out your opponent's inner monster and watch them unleash chaos on... well, hopefully someone other than you.",
//         ManaCost = 8.5M,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 6.5M,
//         DiscoveryMessage = "You move aside an old rusted spade (or was that a shovel?) and a tiny spider crawls across your hand.  You don't think anyone heard your shriek, but it gets you thinking.  There are plenty of people in this town with great legs... perhaps they could do with a few more...",
//         LearnedAtLocation = "mansion_westgarden",

// }, new StaticSkill {
//         dbName = "skill_Slave_to_The_Rhythm_redneckdemon",
//         FriendlyName = "Slave to The Rhythm",
//         FormdbName = "form_Gyrating_Temptress_redneckdemon",
//         Description = "Designed by a particularly lascivious demon, this spell not only transforms the body of your victim into one pleasing to the eye, but transforms the mind to ensure that function follows form.  In other words: it turns your target into a stripper, inside and out.",
//         ManaCost = 5,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "You soon grow weary of your fruitless hunt / For more diverse ways of shaping a cunt. / You take a break while a fetching young lass / Plies her trade: shaking her ass. / It strikes you then, entranced by her show: / How delightful it'd be if her titties would grow! / If the music would move her to dance and to strip, / While her bosom did grow with every new tip! / Your reverie is broken the moment you perceiv. / A small red imp, grinning at your sleeve! / He's been whispering all the while, right into your ear, / Your cravings and daydreams, trying to steer! / He vanishes forthwith, frightfully fast! / But you have a new spell, now, all ready to cast...",
//         LearnedAtRegion = "stripclub",

//}, new StaticSkill {
//         dbName = "skill_Ass_Master_LexamTheGemFox_&_Hachik0048",
//         FriendlyName = "Ass Master",
//         FormdbName = "form_Donkey_Dominatrix_LexamTheGemFox_&_Hachik0048",
//         Description = "Feeling down or timid now that your a sissy mouse boy? Feeling horny or needing someone to take care of you because your a Air Headed Bimbo or a Bitch in Heat? Well a few casts of this handy spell will create a sexy anthro jenny just dying to dominate you",
//         ManaCost = 8,
//         HealthDamageAmount = 2,
//         TFPointsAmount = 9,
//         DiscoveryMessage = "You stroll along, noticing how full the town. is full of horny girls, timid boi toys, and other such creatures that used to be human. You think that these poor lost souls need someone to take care of them. You know they need to be stubborn and firm as you look around for ideas. As you pass through the hallway, you glance at a hanging picture of a donkey grazing in a meadow as well as a riding crop hanging on the wall.  This gives you an idea on how to mix a stubborn ass with a harsh leather mistress",
//         LearnedAtLocation = "ranch_hallway",

//}, new StaticSkill {
//         dbName = "skill_Dat_Ass_LexamTheGemFox_&_Hachik0048",
//         FriendlyName = "Dat Ass",
//         FormdbName = "form_Adorable_Donkey_Foal_LexamTheGemFox_&_Hachik0048",
//         Description = "Look over there! It's not a wolf! It's not panties! It's not a human.....anymore! It's.....it's......an Adorable Donkey Foal!",
//         ManaCost = 9,
//         HealthDamageAmount = 4.5M,
//         TFPointsAmount = 10,
//         DiscoveryMessage = "You spot a jennet out in the fields. Her belly is large and swollen, obviously she is pregnant. You grumble in annoyance, really wanting to see such a cute little animal being born. But that though gives you an idea, why not make cute little donkey foals out of the vast amount of enemy spellcasters?",
//         LearnedAtLocation = "ranch_pasture",

//}, new StaticSkill {
//         dbName = "skill_Transformus_Wandus_themorpher606",
//         FriendlyName = "Transformus Wandus",
//         FormdbName = "form_Wizard's_Wand_themorpher606",
//         Description = "Turns the victim into a powerful magic wand",
//         ManaCost = 12,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 6,
//         DiscoveryMessage = "You walk slowly through the master bedroom, heart thumping a little more loudly than you thought it would.  You come to a bookshelf and you find a rather old book.  Most of the pages are old and yellow and written in an undecipherable language, but a small sticky note slips out with a spell for turning something into a powerful magic wand.  You are about to take it when you remember the woman who lives here may not be so happy to find you snooping, so you memorize it instead and leave everything where you found it.",
//         LearnedAtLocation = "ranch_bedroom",

//}, new StaticSkill {
//         dbName = "skill_Captured_souls_Alessandro_Stamegna",
//         FriendlyName = "Captured Souls",
//         FormdbName = "form_Captured_Souls_Alessandro_Stamegna",
//         Description = "Ever wanted to feel owned without ever having to belong to someone? Try this spell today to create your own living latex collar. You will be the biggest hit at the club while wearing them; unfortunately for you, people are not included with this spell, so you will have to find your own to transform.",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 6,
//         DiscoveryMessage = "You enter the club and realize that it is not so much the strip club that you expected it to be. Yes there are poles in the corners but what you see slightly turns you on. Watching as not just a single person but multiple witches punish a lone beefcake strapped to the pole. You notice the collar as it reflects the black-lights which are holding him in place. You think why not mass produce this by getting rid of some of the bums in the streets by changing them into your new line of collars.",
//         LearnedAtLocation = "stripclub_bar_seats",

//}, new StaticSkill {
//         dbName = "skill_Dark_Baptism_Blood_Knight",
//         FriendlyName = "Dark Baptism",
//         FormdbName = "form_Vampire_Lord_Blood_Knight",
//         Description = "This spell calls upon the ancient powers of darkness to convert its target into a lord of the night, a vampire.",
//         ManaCost = 6.16M,
//         HealthDamageAmount = 4.25M,
//         TFPointsAmount = 6.16M,
//         DiscoveryMessage = "As you wander through the lord's room, your hand touches some dried blood, blacker than what you are used to, and visions dance through your mind. Darkness, filled with dancers, beautiful and obediently. Candles flicker, painting the scene a red-orange. In the center a man kneels before you and, with a lengthened nail, you open your wrist and hold it to his mouth making him drink your dark, deep red blood. You watch as they drink, and change, becoming your thrall. Returning from the vision it strikes you, the power is in the blood. The power to change and reshape them.",
//         LearnedAtLocation = "castle_lordroom",

// }, new StaticSkill {
//         dbName = "skill_Mistress_of_the_night_Foxpower93",
//         FriendlyName = "Mistress of the Night",
//         FormdbName = "form_Seductive_vampire_Foxpower93",
//         Description = "This spell is perfect to dark up parties, to help someone that have high pressure or simply if you like being... kissed in the neck. Anyway this spell will turn your opponent into a beautiful creature of the night that medieval people feared, but as dangerous they can be as long they are enjoyable to the eye it is good... right? ",
//         ManaCost = 8.5M,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 10,
//         DiscoveryMessage = "You walk in the throne room and notice a big dusty paint on the wall. Getting closed you notice it was a fine art work of a woman wearing a big Victorian dress with a snow white skin. You remember hearing that paler people were sexier at those time. Starring at the paint you surprise yourself thinking about how she would look like in underwear and if she was truly human... after all with everything you saw here and the sinister ambiance of this place this woman just remember you... a vampire! Something click in your mind and you learn Mistress of the night spell.",
//         LearnedAtLocation = "castle_throneroom",

//}, new StaticSkill {
//         dbName = "skill_Dusty_Budugu2004",
//         FriendlyName = "Dusty",
//         FormdbName = "form_Feather_duster_Budugu2004",
//         Description = "This spell turns your enemy into an indispensable but simple cleaning tool, a feather duster. Use it to remove some magical dust covering your skin, to tickle your friends or simply for your daily chores. ",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "As you walk around the pond, you find a pristine duck feather lying on the ground. You pick it up and admire it, but then spot another one. As you bend over to grab this second feather, you spot a third one, then a fourth, then a fifth. Quickly, you end up with your hand full of duck feathers. You ponder for a while, looking at your feathers, and realize you could make a cute duster out of them.  But it would be so much more interesting to turn one of your competitors into one instead...",
//         LearnedAtLocation = "park_duck_pond",

//}, new StaticSkill {
//         dbName = "skill_Purification_Adeviant",
//         FriendlyName = "Purification",
//         FormdbName = "form_Austere_Angel_Adeviant",
//         Description = "This spell is fueled by the insecurities and regrets of the target, making them turn against themselves. If effective, the target will end up purifying itself, eventually becoming worthy of serving as soldiers in heavens army. All earthly desires and human defects are ripped out of the target, leaving behind the pure husk of an angel, happy to purify other sinners...",
//         ManaCost = 7,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "As you wander by the chapel, you are suddenly struck by a strange feeling of nausea. This world is corrupt.  Lust, greed, rock music, unhealthy PvP fetish games have weakened the faith of the people, and now foul (but oh so sexy) demons are invading the Earth!  The moment passes, and you come back to your senses staring at the strange shrine. You feel as if something has been implanted into your mind, and as you probe your spell-memory, you realize that you know a new spell! The memory of the spell seems to connect to a vague memory of a beautiful chorus, and an important task.  You now know how to turn people, sinners or otherwise, into angels! ",
//         LearnedAtLocation = "mansion_chapel",

//}, new StaticSkill {
//         dbName = "skill_Clothes_Make_the_Man_Arbitrary_Hal",
//         FriendlyName = "Clothes Make the Man",
//         FormdbName = "form_Black_Suit_Jacket_Arbitrary_Hal",
//         Description = "It is well known that clothes make the man--naked people tend to have little or no impact on society.  To that end, this spell should help you with getting a snazzy suit jacket.",
//         ManaCost = 9.99M,
//         HealthDamageAmount = 5M,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "After looking through Ophalia's varied selection of mens' clothing, you decide you want to be able to present a more professional front--after all, magic is serious business.  While you could just buy a suit off the rack, it wouldn't be nearly as good as one you made specifically tailored to fit.",
//         LearnedAtLocation = "clothing_mens",

//}, new StaticSkill {
//         dbName = "skill_A_Mother's_Touch_Purple_Autumn",
//         FriendlyName = "A Mother's Touch",
//         FormdbName = "form_Magic_Nursing_Bra_Purple_Autumn",
//         Description = "This spell will slowly change the target into a magic nursing bra that, when worn, will cause the user to lactate. A blessing or a curse, depending on your perspective.",
//         ManaCost = 9,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "While rummaging through, you come across several different bras. You idly wonder in awe, thinking they must have every kind of bra here. Wait, they don't. But, that gives you an idea for a spell...",
//         LearnedAtLocation = "clothing_womens",

// }, new StaticSkill {
//         dbName = "skill_Cat(boi)_Scratch_Fever_Aingelproject667",
//         FriendlyName = "Cat(boi) Scratch Fever",
//         FormdbName = "form_Kitten_Boi_Aingelproject667",
//         Description = "A kitten is the best way to brighten up even the dreariest days. Don't you wish that there were more kittens in this town? With this spell, you can make that possible! Just a few applications will turn anyone you wish into an adorable kitty boy!",
//         ManaCost = 10,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "While looking around the gas station's crash, you hear a small chorus of meows coming from some tall grasses next to the building. You follow the sound, and happen upon a family of cats; leading the procession is the mother cat, with her small army of kittens, all stumbling over each other and mewing cutely. One of the kittens seems to have lost his way, and looks up at you with big eyes as if for help. You point the kitten back towards his brethren, and watch as the family disappears around the corner. The scene is endearing, though it also gives you a more...devious idea to use against your opponents. A few experiments and tweaks later, and you successfully make a spell that will turn your opponent into an adorable kitten boy.",
//         LearnedAtLocation = "gas_station_pumps",

//}, new StaticSkill {
//         dbName = "skill_Heavy_Artillery_Swog",
//         FriendlyName = "Heavy Artillery",
//         FormdbName = "form_Unimaginably_huge-breasted_woman_Swog",
//         Description = "Think someone would look better with some bigger breasts? Some ridiculously sized breasts? Some seriously off the wall 'I can't even walk with these monsters attached to my chest' breasts? This spell will do just the trick.",
//         ManaCost = 8,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Browsing through some of the kinkier Japanese video games delegated to part of a small hard-to-find shelf, you see a woman with large breasts. At second glance you realize her breasts are enormous! In real life she would have to carry them around in her arms just to be able to walk! An idea starts bouncing around in your mind as you watch her bounce around. You think you could improve on her 'design'. Why should she be able to walk?",
//         LearnedAtLocation = "comicstore_videogames",

//}, new StaticSkill {
//         dbName = "skill_Who_Wears_the_Pants_Arbitrary_Hal",
//         FriendlyName = "Who Wears the Pants",
//         FormdbName = "form_Black_Silk_Pants_Arbitrary_Hal",
//         Description = "Sometimes it's not clear just who wears the pants around here.  This spell will make someone into a proper pair of dress pants (a.k.a. suit trousers) so that you can show the world that the answer to that question is you.",
//         ManaCost = 9,
//         HealthDamageAmount = 4.5M,
//         TFPointsAmount = 7.5M,
//         DiscoveryMessage = "While most of the clothes around town seem either casual or revealing, you feel you'd rather display a little class.  Unfortunately, most of Ophalia's merchandise seems to be catering to what is apparently the local taste, so it looks like you'll have to make it yourself.  A nice pair of silk pants should do the trick.",
//         LearnedAtLocation = "clothing_mens",

//}, new StaticSkill {
//         dbName = "skill_Bondage_is_Magic!_LexamTheGemFox",
//         FriendlyName = "Bondage is Magic!",
//         FormdbName = "form_BDSM_Pony_Girl_LexamTheGemFox",
//         Description = "Animals and fairies might be nice, but sometimes you just need a sexy old fashioned human bondage pet to make yourself feel great. This spell can turn anyone into a submissive Pony Girl.",
//         ManaCost = 12,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "As you poke your head into each and every stall, extra careful to make sure that you don't accidentally swing an iron gate behind yourself and lock yourself in a cell, you come across a pair of shackles, some bizarre torture instruments, and a horse saddle.  Or... are they torture instruments?  One looks suspicious like a dildo... maybe some of the prisoners here weren't exactly here against their own free will.  You start to wonder what it would be like if you could recreate whatever strange scene must have happend here by turning one of your enemies into a girl who gets soaked at the thought of hardcore BDSM...",
//         LearnedAtLocation = "castle_dungeon",


//}, new StaticSkill {
//         dbName = "skill_Heavy_Metal_Faremann",
//         FriendlyName = "Heavy Metal",
//         FormdbName = "form_Shiny_Robot_Girl_Faremann",
//         Description = "Always wanted to Build a Robot? Now this is you chance you found some pretty good blueprints and where able to make a spell out of them. now you just have to find some suitable targets...",
//         ManaCost = 13.37M,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "While searching the area you stumble upon some blueprints for what it seems some kind of robot. you are reading them carefully and decide this could make a great spell. after a few minutes you memorised the most important parts and finally got a new spell.",
//         LearnedAtLocation = "college_sciences",


//}, new StaticSkill {
//         dbName = "skill_Schoolyard_Strut_Christopher",
//         FriendlyName = "Schoolyard Strut",
//         FormdbName = "form_Mary_Janes_Christopher",
//         Description = "To a school girl, her uniform is everything, her shoes especially. A pair of black patented Mary Janes are the only acceptable shoes with a uniform such as this. Your chosen target will become presentable, feminine, and if they happen to possess a five inch heel, well, a girl can't have too much!",
//         ManaCost = 7,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "A picture on the wall catches your eye as you walk past. It is a leaver portrait for some successful students a number of years ago, showing the different years together. You smirk briefly as your eyes run over their forms, the front lines holding no interest to you as you gaze at the finishing students, fully developed and posing sexily in their short uniforms. You notice they stand quite a bit taller than the younger girls, realising that they are each supported by five inch heels, matched by cute lacy socks, as if they were a concession to the innocent appearance. The idea of a cute pair of Mary Janes isn't bad, not bad at all.....",
//         LearnedAtRegion = "college",

//}, new StaticSkill {
//         dbName = "skill_Portent_Aidyn_Bright",
//         FriendlyName = "Portent",
//         FormdbName = "form_Ioun_Stone_Aidyn_Bright",
//         Description = "Need help with your mana/willpower regen woes? This spell will create an ioun stone of your target that floats around your body glowing and siphoning their soul energy to you! ",
//         ManaCost = 18,
//         HealthDamageAmount = 2,
//         TFPointsAmount = 6,
//         DiscoveryMessage = "As you searching the solar you come across a pile of leather-bound books and an old dusty red box.  Inside is an ancient tome with a picture of a star system on the front.  Most of the pages are missing but one spell remains about halfway through it.  You run your finger along the text, reading a memorizing the magic words...",
//         LearnedAtLocation = "castle_solar",


//}, new StaticSkill {
//         dbName = "skill_Hairspray_Kirshwasser",
//         FriendlyName = "Hairspray",
//         FormdbName = "form_Red_Wavy_Wig_Kirshwasser",
//         Description = "It's so hard to keep your hair looking good in this crazy town. This spell makes sure you don't need to worry about that anymore.",
//         ManaCost = 7,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "While looking around for anything helpful, you catch a glimpse of yourself on a bathroom mirror. You walk a bit closer to have a better look and notice your hair is kinda greasy, and well, a bit of a mess. You can't exactly visit a hairdresser, but then it strikes you.  Why not just turn someone else into your hair, you're sure their inner magic would keep them looking good forever, not to mention natural.",
//         LearnedAtLocation = "castle_baths",
         

//}, new StaticSkill {
//         dbName = "skill_Valkyrja_Run_Blood_Knight",
//         FriendlyName = "Valkyrja Run",
//         FormdbName = "form_Valiant_Valkyrie_Blood_Knight",
//         Description = "Transforms the target into a statuesque raven haired valkyrie, with moon pale skin and ice blue eyes.",
//         ManaCost = 9,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 9,
//         DiscoveryMessage = "Looking about the armory you see a handful of small stones lined neatly against the wall underneath some rusty battleaxes. On each of them is carved a runic symbol and knowing that this is a sorcerous realm you begin to put them into a variety of new configurations. After a few moments you find a combination of runes that seem to speak to you, 'Valkyrja'...",
//         LearnedAtLocation="castle_armory",

//}, new StaticSkill {
//         dbName = "skill_Blue_feather_Draggony",
//         FriendlyName = "Blue Feather",
//         FormdbName = "form_Blue_Chaffinch_Draggony",
//         Description = "Need your opponent to value something other than bursting you into the next fashionable dress? Or are you just tired of hot babes trying to seduce you into submission?  Why not kill two birds with one spell by turning your opponent into a bird?",
//         ManaCost = 7.50M,
//         HealthDamageAmount = 3.00M,
//         TFPointsAmount = 8.00M,
//         DiscoveryMessage = "Wandering through the Enchanted garden area you spot a little blue chaffinch pecking at some seeds. As you watch him dance around the seeds and peck them up a thought crosses your mind. Hey, if all your opponents are birds they should be less inclined to compete against you. You take note and make sure to test this theory soon!",
//         LearnedAtLocation = "castle_garden",

//}, new StaticSkill {
//         dbName = "skill_Dairy_Factory_Sampleguy",
//         FriendlyName = "Dairy Factory",
//         FormdbName = "form_Milk_Cow_Sampleguy",
//         Description = "This spell turns its unwitting victim into a dumb, sluggish Milk Cow with a fat, over-engorged udder. ",
//         ManaCost = 5,
//         HealthDamageAmount = 2,
//         TFPointsAmount = 10,
//         DiscoveryMessage = "Walking through the fields you see the cows peacefully grazing, blissfully unaware of the magical war happening around them. You think to a moment, wouldn't it be nice of you to turn your victim into a stupid yet content animal. You then laugh to yourself. Nah why not add a gigantic unrealistic udder just to be mean. Maybe you can even corner the entire dairy market after this conflict is over.",
//         LearnedAtLocation = "ranch_barn",

//}, new StaticSkill {
//         dbName = "skill_It's_the_Thought_that_Counts_Berrie_Valentine",
//         FriendlyName = "It's the Thought that Counts",
//         FormdbName = "form_Count_Cuddles_Berrie_Valentine",
//         Description = "Nothing helps to calm the mind more then a loving embrace, whether it's with one's lover or just a beloved stuffed animal they hold dear. With this spell you might just be able to kill two birds with one stone. ",
//         ManaCost = 10,
//         HealthDamageAmount = 6,
//         TFPointsAmount = 6,
//         DiscoveryMessage = "As you walk by the Lord's bed your gaze falls upon a portrait on the opposite wall above a drawer. Curious you move closer to take a better look at it. The frame holds a brilliantly painted canvas; the man portrayed upon it standing with a rather apathetic demeanor. His lustrous silver mane falls across his pale face down to his shoulders. Striking red irises peer between the strands, accenting an elegant rose button-up beneath an onyx cloak. Longingly you gaze into his crimson eyes and sigh, oh how wonderful it would be to embraced by this beautiful regal man, to be showered with his love and affection, and share the nights in his skillful hands... You shake your head and come out of your revere, surely you haven't been swooning over a mere painting have you? You are not a teenage girl so that thought is just silly. Yet your hands seem to have been holding something. As that realization dawns upon you, you look down only to find a white teddy bear clutched tightly in your hands. A polar bear from the looks of it. You have no idea where it came from but you assume it was sitting on the drawer and you picked it up during your day dreaming. You quickly set it down not wanting to invoke the wrath of its owner who seems to think of it quite fondly. It's been adorned with a red ribbon around its neck with a red crystal rose. A pang of loneliness washes over you as you set down the stuffed toy, wishing you had one of your own. Inspiration hits you like a lightning bolt as your gaze drifts back up to the majestic painting. Surely you could easily create a little lord to call your own to embrace and cuddle with as long as you find a volunteer... willing or not.",
//         LearnedAtLocation="castle_lordroom",


//}, new StaticSkill {
//         dbName = "skill_Help_in_high_places_Christopher",
//         FriendlyName = "Help in high places",
//         FormdbName = "form_Platform_heels_Christopher",
//         Description = "This sexy spell will persuade your target to take up your burden, so to speak, in the form of a pair of strappy platform high heels. You aren't sure where it came from, but you don't mind, and they don't matter!",
//         ManaCost = 8,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         //DiscoveryMessage = "You wander along, occasionally glancing at the objects left strewn about the mansion. It's not as if it is boring, there just simply isn't anything in here worth taking, seemingly consisting of only old rubbish and dust. A dresser catches your eye, or more specifically, a small book lying atop it. The pages are yellowed with age, but it is still legible. It appears to be a pamphlet for new maids starting at the mansion. There is an image of the full uniform, your eyes darting directly to the five inch platform heels. Did they really have to wear these?  You feel mildly jealous of their uniform, and a spell quickly forms in your mind, unbidden but not lacking for desire...",
//         LearnedAtLocation = "mansion_servant_quarters",

//}, new StaticSkill {
//         dbName = "skill_Doll_'Em_plzTryMySpell",
//         FriendlyName = "Doll 'Em",
//         FormdbName = "form_Magical_Mannequin_plzTryMySpell",
//         Description = "This spell transforms its target into a mannequin-like golem. Golems were traditionally employed as manual laborers of the better-off mage families. Perhaps things would run smoother around here if you were to turn a few of your opponents into golems?",
//         ManaCost = 13,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "You find an older looking book almost hidden behind one of the dusty shelves. Figuring that someone might have put it there to keep you from learning about its content, you make the effort needed to pull it out from there.  You open the book somewhere in the middle... only to be greeted by a spread-sized rune staring straight at you. According to some accompanying text, this rune supposedly has the power to compel even dead objects to serve, and was traditionally used to create golems. You smile to yourself as your mind runs wild with ideas on how to use this knowledge against your enemies. Figuring that you may want to create a golem yourself someday soon, you begin to copy the rune into your spellbook.  You find it surprisingly relaxing to just let your pen trace out the intricate symbol... you eventually start to drift off, losing yourself in fantasies about serving your Masters.  Realizing that the rune must be affecting you, you steel yourself and quickly finish copying the rune. The deed done, you quickly put the book back where you found it, not wanting anyone else to find it.",
//         LearnedAtLocation = "mansion_study",


//}, new StaticSkill {
//         dbName = "skill_Spank'Me_Zatur",
//         FriendlyName = "Spank'Me",
//         FormdbName = "form_Cheerleader_Spankies_Zatur",
//         Description = "If you're going to be a cheerleader jumping around and kicking in a short skirt, you need something to cover your panties from prying eyes, so most wear an article of clothing called spankies! But where's the fun in that? St. Circe Cheerleaders much rather just wear the 'Spank'Me's this spell can make for them!",
//         ManaCost = 7,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "Searching around the office for new ways to have fun with other mages, you catch a glimpse of some discarded clothing under a desk. Seems some others have beat you to the punch. One of them being a cheerleader, judging by the brightly colored articles of clothing. Hrrm... That one piece of clothing don't seem to be normal panties. No, those look like spankies. But that makes no sense. Why would St. Circe Cheerleaders care about a little extra modesty? Well, you're sure you can provide them something a little more their style...",
//         LearnedAtLocation = "college_foyer",

//}, new StaticSkill {
//         dbName = "skill_Sucker_born_every_minute_Christopher",
//         FriendlyName = "Sucker Born Every Minute",
//         FormdbName = "form_Cherry_Sucker_Christopher",
//         Description = "For the mage with a sweet tooth or just an oral fixation, this delicious spell will transform your opponent into a cherry red delight, complete with gum balls and a tasty inner filling.",
//         ManaCost = 8,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "As you walk through the candy store into the kitchen, you observe how the walls are covered with shelves, each containing a glass jar filled with colourful flavours, tantalisingly out of reach of any children, filling their minds with desire. The store is empty and you walk through a back door into the kitchens. Vats and pulleys, pots and jars. These all lie empty, their purpose beyond you. One of the vats still has a mixture within it and you walk over, dipping a finger in and drawing out a cherry red appendage. As you suck on it briefly, its flavour terribly strong in its state, a thought rises in your mind upon the shape of the lollipop you are eating, long and hard, but sweet in its taste. Maybe you could find a way to make another one of these with a willing participant... Or unwilling, it doesn’t matter at the end.",
//         LearnedAtLocation = "candystore_kitchen",

//}, new StaticSkill {
//         dbName = "skill_Eye_for_Ra_Pharlynx",
//         FriendlyName = "Eye for Ra",
//         FormdbName = "form_Egyptian_Priestess_Pharlynx",
//         Description = "Ra is in need of new Priestesses. He tires of the same drab old Priests and desires something youthful and beautiful to grace his sight. Using his power you can provide him with these and gain his favor!",
//         ManaCost = 5,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "As you look around the old Courtyard, you look up at the sun feeling the sheer power that the sun has. As if on cue, a voice tells you to go out and gather subjects for your ruler, Ra. You feel the words enter into your mind and form a coherent thought. ",
//         LearnedAtLocation = "mansion_courtyard",


//}, new StaticSkill {
//         dbName = "skill_A_Goody_Two_Shoes_Arbitrary_Hal",
//         FriendlyName = "A Goody Two Shoes",
//         FormdbName = "form_Black_Leather_Shoes_Arbitrary_Hal",
//         Description = "Some people are so sickeningly good, that you just know the world is going to walk all over them.  This spell will let you help that process along.",
//         ManaCost = 8,
//         HealthDamageAmount = 2,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Looking around the concert hall, you feel like you need some proper footwear in the event of a special event there.  Unfortunately, it seems this down doesn't have a dedicated shoe store, so it looks like you're stuck making your own.",
//         LearnedAtLocation = "concert_hall_seats",


//}, new StaticSkill {
//         dbName = "skill_Dawn_of_the_Ditz_Varn",
//         FriendlyName = "Dawn of the Ditz",
//         FormdbName = "form_Bimbonic_Plague-Bearer_Varn",
//         Description = "Ever since your encounter at the Treasure Chest, you have seen them around the city - an identical horde of vacant-eyed, giggling, bimbos, strutting around their skimpy outfits, propositioning everyone they see.  Victims of the Bimbonic Plague.  Supremely infectious, as you well know: a mere touch of their fluids spreads their condition to any who come into contact with them; though it seems to take repeated exposure to overwhelm a strong mage.  Exposure that this spell can grant - casting it will stimulate the virus in your system (making you - temporarily - into one of them, if you aren't already) - and direct your overcharged libido at the target in question.  The rest will ... take care of itself.",
//         ManaCost = 5,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 10,
//         DiscoveryMessage = "While walking through the private booths, a young woman calls you over, giggling.  She looks - familiar somehow, like... like you've seen her around the city?  You shake your head - for some reason it's difficult to think clearly in her presence.  You seem to remember - hundreds of women who look identical to her... and... a sense of incredible danger?  That... that doesn't make any sense - and god, she looks just beautiful.  'Like, come over here, sexy!' she says, giggling - all thoughts of her true nature melt away as you approach. 'You'd, like, totes be perfect to join us!'  The scantily-clad bimbo opens her arms wide, as if for a hug, puckers her lips as to offer a kiss - without conscious thought, the two of you are soon engaged in a passionate bout of tongue wrestling.  Her saliva tastes... off... and your body feels oddly warm - but your eyes are closed, and you aren't ready to break the make-out session up just yet.  But finally, you do.  Your eyes wander over to the mirror - odd, you see two of the bimbo that just seduced you... but... why are the eyes of one of them following your own?  And why does your chest feel so...   You gasp, as the bimbo in the mirror brings her own hands up to cup her enormous breasts.  It... it can't be... thoughts of the virus, the so called 'Bimbonic Plague' rush back into you, as you realize where you have seen this woman - seen your OWN reflection before.  'Like, ohmigawd, you, uh, like did this to me?' you ask the girl, in a high-pitched voice.  'Oh yeah!!', she says in the same sing-song voice, 'ain't it, like, so totally kewl?'  'Yeah, so hawt too!' you find yourself agreeing... but as you look at yourself in the mirror, you see your body slowly shifting, returning to its prior shape.  'Oh noes!' she pouts, 'Like, you're totally turning back!' - but before she can do anything else to you, before you even finish changing, you're hightailing it out of there.  And yet... the Plague is still inside of you.  You've heard that there is no cure, mundane or magical - and the rumors may well be true, as your magic is not up to removing the virus, only suppressing it.  For now.  In fact... about the only thing you could do magically would be to temporarily give in, become that sexy bimbo - and spread her curse around the city.  But you wouldn't want to do that... like, would ya?",
//         LearnedAtLocation = "stripclub_booths",

//}, new StaticSkill {
//         dbName = "skill_Bookwyrm_Julian_Chance",
//         FriendlyName = "Bookwyrm",
//         FormdbName = "form_Pocket_Dragon_Julian_Chance",
//         Description = "While magic takes many shapes and forms, there is one ironclad rule that holds true across all variations of sorcery:  Dragons. Are. Cool.  This spell lets you turn someone into one, so you can have them as a pet, and be cool by association.",
//         ManaCost = 15,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 12,
//         DiscoveryMessage = "Your eye falls upon a rulebook for role playing.  On the cover, a majestic looking wizard and knight do battle with a gigantic, fire-spewing dragon.  While you chortle at just how wrong those sorts of books always are about magic, it occurs to you that they might have a point about dragons.  They ARE pretty awesome.  Maybe you should try to make one yourself.  Certainly couldn't hurt your image, having one as a pet...",
//         LearnedAtLocation = "comicstore_gaming_room",


//}, new StaticSkill {
//         dbName = "skill_Shell_Out_Zatur",
//         FriendlyName = "Shell Out",
//         FormdbName = "form_Cheerleader_Shell_Zatur",
//         Description = "The sleeveless vests worn by cheerleaders are typically called 'shells.' Not only does this spell create a nice looking one that is typically used by St. Circe Cheerleaders, but it also makes it a rather protective piece of clothing as well!",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Glancing around, it seems that this store doesn't just provide clothing, but uniforms. There's one for a nurse or a maid even. At least normal uniforms for them, not the kind you've been seeing around here. And look, there's even the uniform for St. Circe Cheerleaders here. Maybe they provide the school with them in bulk? There's the skirt and the vest. Except the tag calls it a 'shell'? Huh. Must be a term used by the cheering squad or something. Why are those things called shells? Do they provide some kind of protection? Hrm... Well if the real ones don't, you're sure you can make some that do....",
//         LearnedAtLocation = "clothing_womens",

//}, new StaticSkill {
//         dbName = "skill_Honey_Pot_Rayner",
//         FriendlyName = "Honey Pot",
//         FormdbName = "form_Playful_Pussy_Rayner",
//         Description = "Clothing is just not intimate enough for you, with this spell you can get even closer to your target, or rather your target can get even closer to you. ",
//         ManaCost = 8,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "Wandering into the secret laboratory of Dr. Hadkin and searching around a bit too long you come across some interesting research. It seems as though some lab rats were rather intimately combined with each other and as you peer over the notes you wonder if this might be the fact of the Doctor himself. Whatever the case the idea is born, the magic is weaved, you've got a new spell to test out...",
//         LearnedAtLocation = "lab_secret_laboratory",

//}, new StaticSkill {
//         dbName = "skill_Pussy_Power_LexamTheGemFox",
//         FriendlyName = "Pussy Power",
//         FormdbName = "form_High-Quality_Onahole_LexamTheGemFox",
//         Description = "Need something more realistic than a sex doll's plastic pouch pussy? Well then use this to change an opponent into a finely crafted onahole for your pleasure! Use them to stick your dick in, or practice your oral sex skills.",
//         ManaCost = 8,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 9.5M,
//         DiscoveryMessage = "You browse around the many onaholes in this sex shop, sighing softly to yourself and unable to find something that catches your eyes. Why can't they make a sex toy with a bit more love and care, mabye even a bit more soul? With that thought you smile to yourself and leave the store, the idea of a spell to make an opponent into exactly that forming in your mind.",
//         LearnedAtLocation = "stripclub_store",

//}, new StaticSkill {
//         dbName = "skill_Anthro_Porcinus_themorpher606",
//         FriendlyName = "Anthro Porcinus",
//         FormdbName = "form_Pig_Girl_themorpher606",
//         Description = "This mischievous spell frees one's inner swine, transforming its victim into a squealing pig girl.",
//         ManaCost = 6,
//         HealthDamageAmount = 6,
//         TFPointsAmount = 10,
//         DiscoveryMessage = "As you walk through the teenager's bedroom you see the heap of clothes strewn in, but mostly around, a hamper.  While most of the bedroom is clean and pristine, you can't help but notice this one small mess.  You wonder if whoever lives here is forced to keep her bedroom clean against her will, forced by her guardians to make sure her bedroom doesn't become a pigstyle.  But what if she didn't have to?  What if there were a way you could release her... or someone else's... inner swine?",
//         LearnedAtLocation = "ranch_bedroom_teenager",

//}, new StaticSkill {
//         dbName = "skill_Robotomy_Christine_Winters_(Arrhae,_ToniV_and_Meddle_for_proofing)",
//         FriendlyName = "Robotomy",
//         FormdbName = "form_Hedonistic_Fembot_Christine_Winters_(Arrhae,_ToniV_and_Meddle_for_proofing)",
//         Description = "The future is here! Utilizing the latest technology and magic, this spell transforms your target into a sexy fembot. Ready to act out any hedonistic tendency you might have!",
//         ManaCost = 7,
//         HealthDamageAmount = 5.5M,
//         TFPointsAmount = 9,
//         DiscoveryMessage = "As you browse the store, you are delighted find a sleek high-end looking media player with a set of headphones amongst the used instruments. Touching the display panel turns it on and you decide to give it a try. As you pull the snug pair of headphones over your ear and tap the play button, you gasp as a small static shock zaps your finger and a loud high-pitched ping resounds in your ear. Pulling the headphone off your head in a hurry, you’re suddenly flooded with the knowledge on how to operate the device. Smiling to yourself, you pocket the device and leave the store eager to try out the special functions as you will the device to stream your favorite track list right into your cerebral core.",
//         LearnedAtLocation = "record_store_usedInstruments",


//}, new StaticSkill {
//         dbName = "skill_Bull_by_the_horns_Christopher",
//         FriendlyName = "Bull by the Horns",
//         FormdbName = "form_Muscular_Minotaur_Christopher",
//         Description = "This somewhat dangerous spell will create a mythical minotaur from Greek legend, with your own added twist. It transforms your opponent into a muscular behemoth, with surprisingly sharp mind included. It will be so subsumed by his new found desires however, you may find he poses even less of a risk than before, at least in terms of magic.",
//         ManaCost = 9,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "Whilst perusing the solar, the evidence of a previous reader on one of the tables catches your eye. It is pile of books on Greek mythology, two of which are currently open. The first book appears to display the Minotaur of Crete, slain by Theseus. The other book displays a small list of minor Greek deities, one of which is especially interesting; Priapus, a minor god of livestock and more importantly, fertility. All of the other information is lost to you when you catch site of his member, a good foot long. Your eyes unfocus as your mind wanders, the two images blending together, forming a spell in your mind.",
//         LearnedAtLocation = "castle_solar",

//}, new StaticSkill {
//         dbName = "skill_Day_of_the_Tentacle_Varn",
//         FriendlyName = "Day of the Tentacle",
//         FormdbName = "form_Tentacled_Terror_Varn",
//         Description = "A little chaos can be a good thing.  You need a way to counter the threat of all these schoolgirls running around; why not take a page from Japanese culture and create their natural enemy, the tentacle monster?  While your victim will lose all rational thought - becoming a mere sex-obsessed beast - they may be still be very effective at 'licking' their victims.  And, who knows - it might be fun (if a bit distracting) to have a tentacled monstrosity as a pet of your very own ...",
//         ManaCost = 8,
//         HealthDamageAmount = 2,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Odd - this section of the lab is dominated by photos of young women, most of whom look like students at the local college - former tech assistants, perhaps? Though judging from the angle of the shots - quite a few looking down shirts or up skirts - the primary researcher in this part of the lab - a Doctor F. Edison judging from the nameplate on the main desk - must be just a bit of a pervert.  Rifling through his desk (a little casual vandalism never hurt anyone, right?) your suspicions are confirmed; Dr. Edison has snuck quite a collection of hentai comics into his workplace.  Though he seems to have been taking notes in the margins - not just alongside the attractive schoolgirls, but also alongside pictures of the monsters that invariably seem to assault them.  Hmmm...",
//         LearnedAtLocation = "lab_offices",

//}, new StaticSkill {
//         dbName = "skill_Winter_is_Coming_Alessandro_Stamegna",
//         FriendlyName = "Winter is Coming",
//         FormdbName = "form_Ice_Queen_Alessandro_Stamegna",
//         Description = "Ever wanted to freeze that annoying person at work or make it so people can't annoy you with singing off-key. You can do this now with your very own Ice Queen. (Warning you may have an excessive heating bill trying to keep warm with her around.)",
//         ManaCost = 11,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "You are hit by a cold chill up your spine in the heat of summer. You think it would be nice to have a slightly colder weather around here. Why not make a few people into ice queens to cool off your part of town or maybe even use them to just be an air conditioning unit for your location. ",
//         LearnedAtLocation = "college_track",

//}, new StaticSkill {
//         dbName = "skill_Flower_Power_Foxpower93",
//         FriendlyName = "Flower Power",
//         FormdbName = "form_Alraune_Foxpower93",
//         Description = "Flowers are known to have good look, good smell and some even have medical virtues. And now you know how to make a flower to go, relax, take a deep breath and enjoy the sight of some new kind of pretty flowers.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 7.5M,
//         LearnedAtLocation = "park_rose_garden",

//}, new StaticSkill {
//          dbName = "skill_Lucky_Lips_Greg_Mackenzie_and_Fiona_Mason",
//         FriendlyName = "Lucky Lips",
//         FormdbName = "form_Ruby_Red_Lipstick_Greg_Mackenzie_and_Fiona_Mason",
//         Description = "Whilst out at the shops, a bit of paper blows and lands next to your foot. Upon closer inspection it appears to be a spell to turn its user into a dark ruby red lipstick. It will make the user stronger in will and mana but reduce the rate of recovery",
//         ManaCost = 6.75M,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 9,
//         DiscoveryMessage = "Whilst walking inside the salon, a page from what looks like a make-up catalogue lands on your foot. Picking it up you see it is a spell to turn someone into a tube of ruby red lipstick.  It is a dark colour that can seduce even the most hardy wizards.",
//         LearnedAtRegion = "salon",

//}, new StaticSkill {
//         dbName = "skill_Hickory_Dickory_Dyke_Estyz",
//         FriendlyName = "Hickory Dickory Dyke",
//         FormdbName = "form_Tiny_Mouse_Girl_Estyz",
//         Description = "This spell turns your opponent into an adorable tiny mouse girl that just about fits in the palm of your hand. It definitely wouldn't hurt to have some hard cheeses or some sunflower seeds ready before you cast it.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "As you sneak your way through the creepy secret laboratory, you are suddenly distracted and scared by high pitched chattering coming from a cage which has a labcoat draped over it. Wary of magical traps and competing casters (not to mention the weird equipment and chemicals around here) you look around carefully before you sneak towards the cage and lift off the labcoat… 'Peep?' A pair of bright blue eyes in a cute white face look up at you from between the bars, instantly melting your fears away. It’s just a lab mouse. Were you really afraid of this? Dropping your guard you open the cage with a smile, and after a bit of persuasion from your side the the little guy runs up and down your arm, until it gets tired of playing and falls asleep on your shoulder. You are tempted to take the happy critter with you, but you have an even better idea. While the mouse is deep asleep you point your finger at it, touch its nose, and whisper a spell. The mouse’s fur lights up for a second with magic, and as you put the sleeping beauty back in its cage you see little arches and sparkles of static energy crackling between your fingers. You reckon it’s time to make your own mouse pet.",
//         LearnedAtLocation = "lab_secret_laboratory",

//}, new StaticSkill {
//         dbName = "skill_Message_in_a_Bottle_Zatur",
//         FriendlyName = "Message in a Bottle",
//         FormdbName = "form_Willing_Wish-giver_Zatur",
//         Description = "You have maids and you have slaves, but what if you want someone to make your every wish come true? The wishes that require a bit of magic? Well, you could just take a cue from Aladdin, rub something, and POOF, ask a magical genie to grant your wish. You just need to find yourself a genie to rub. Genie's lamp! A genie's lamp to rub. Though, I guess with this spell you could tweak it so they don't mind WHAT you rub.",
//         ManaCost = 3,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "As you sit at the bar having a drink, a familiar tune wafts to your ears from the nearby TV. It's an old show about an astronaut finding a bumbling, but well-meaning genie and her bottle of a home. The theme is pretty catchy, however you've always preferred that show about the guy who married a witch who twitched her nose. Anytime you talk about that though, you end up arguing whether Dick York or Dick Sargent was the better Darrin with someone and then comes the shouting and yelling and... Wait, what were you thinking about? Oh yeah. Genies. You should make some of those.",
//         LearnedAtLocation = "tavern_counter",
//}, new StaticSkill {
//         dbName = "skill_Page_Turner_Varn",
//         FriendlyName = "Page Turner",
//         FormdbName = "form_Practical_Transformations_and_the_Metaphysical_Realm_Varn",
//         Description = "You have  come into possession of a ninth edition copy of 'Practical Transformations and the Metaphysical Realm', a book capable of granting you enormous transformative powers - provided, of course, that the book is first empowered by the soul of another mage.  The spell for entrapping one of your competitors is quite simple; though you may have to use it repeatedly to permanently trap them between the book's pages.",
//         ManaCost = 15,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 6,
//         DiscoveryMessage = "You seem to be drawn to Words of Wisdom's used textbook section - and your eyes can hardly believe what you find there - a pristine, unopened, ninth edition copy of 'Practical Transformations and the Metaphysical Realm'!  Currently in its fifteenth printing, the text is still standard fare at all sorcerer's colleges.  But if the legends are true, the ninth edition had to be banned after a number of 'accidents' involving missing apprentice mages, and copies of the book turning up several inches thicker.  Grinning from ear to ear, you rush to the front counter before anyone else spots the prize; ultimate transformative power will soon be yours, for only a few Arpeyjis.  Well, the cash and ... the soul of another, bound into the copy of your new book.  A small price to pay, especially as it will be another paying it, yes?",
//         LearnedAtLocation = "bookstore_front",

//}, new StaticSkill {
//         dbName = "skill_Your_Highness_JBovinne",
//         FriendlyName = "Your Highness",
//         FormdbName = "form_Elf_Princess_JBovinne",
//         Description = "Some of the casters in town always arrogantly act like everything should go their way; complaining when things aren't ‘just right’ or to their standard of perfection.  If they’re going to act like princesses then you are going to transform them into one.",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Looking at the dozens of exotic cocktail mixes and small bottles of liquor costing in the triple digits you realize how precious people are becoming.  It almost a trend with some of the other spell casters in town, always complaining that things aren't going their way.  Perhaps if everyone is going to act like an exotic princess you should go and give them a more fitting form. ",
//         LearnedAtLocation = "sorority_kitchen",

//}, new StaticSkill {
//         dbName = "skill_Boobify_Swogrider",
//         FriendlyName = "Boobify",
//         FormdbName = "form_Disembodied_Boobs_Swogrider",
//         Description = "Sometimes making your target into a bra just doesn't keep them close enough. This spell transforms the target into a pair of big bouncy titties! Getting rid of an opponent and giving yourself some bigger assets? Win/Win!",
//         ManaCost = 8,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 9,
//          DiscoveryMessage = "As you explore the sorority bedroom you come across a poster that shows a rather large assortment of hunks with big-breasted chicks around their arms, the men striking muscular poses next to 60s American muscle cars with their lady companions bent over to give you, the viewer, the best possible look down their V-neck shirts.  You wonder if any of the girls in the poster haven't been under the plastic surgeon's knife at least twice to get that look.  Surgery is painful and expensive, and nobody seems to have quite gotten the hang of absorption spells yet.  But what's to stop you from enhancing your own chest with a pair of bouncy disembodied breasts that you can put on and off at will while feeling real and making sure that specific opponent won't bother you again, except in the case they decide to make their... your new... nipples hypersensitive to your shirt...",
//         LearnedAtLocation = "sorority_bedrooms",

//}, new StaticSkill {
//         dbName = "skill_Cyber_Superiority_themorpher606_(Kevin_Gates)",
//         FriendlyName = "Cyber Superiority",
//         FormdbName = "form_Mechanical_Man_themorpher606_(Kevin_Gates)",
//         Description = "This spell turns its victim into a metal machine man, just like in the corniest of B scifi flicks.",
//         ManaCost = 10,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 7,
//         DiscoveryMessage = "As you walking along the pathway you come across an old rusted truck overgrown with vines and vegetation.  You peer inside, wondering how many decades it's been resting there abandoned.  When you pull your head out, your heart skips a beat as you see the shadow of a man beside you.  Thumping your head on the frame and getting some rust in your hear, you stumble back to discover that you've been startled by a mere trick of the light.  Some twisted metal pipes hanging out of the bed of the truck cast the strangely humanlike shadow across the path, nothing more.  It even kind of vaguely reminds you of a robot from a British television show that span a few decades, the actors' costumes improving not even the tiniest bit over time.  Well, you have your magic.  Using a new spell you actually could turn an opponent into a walking, calculating heap of metal and circuits for real...",
//         LearnedAtLocation = "pathway_sorority",


//}, new StaticSkill {
//         dbName = "skill_Hot_for_Teacher_Varn",
//         FriendlyName = "Hot for Teacher",
//         FormdbName = "form_Tempting_Teacher_Varn",
//         Description = "Schoolgirls are nice, but what happens to them when they grow up, get a real job, get some \"experience\" as it were?  This spell will let you find out ... and the faculty at SCCC will be ever so grateful for you adding to their numbers.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "Walking the halls of St. Circe's, a thought occurs.  You've seen schoolgirls, cheerleaders, all kinds of cute young things around town - but where are the people running this place?  The caretakers, the administrative staff, the professors ... most of all, the young lecturers.  You can just imagine the sorts of young women a school with this reputation might attract; if nothing else, maybe some of their recent graduates might be sticking around, training up a new generation of young girls.  Now that you think about it ... the school REALLY could use a few of these young professionals ... and you think you know just how to help them acquire one.",
//         LearnedAtRegion = "college",

//}, new StaticSkill {
//         dbName = "skill_It's_Got_Ruffles!_Martiandawn",
//         FriendlyName = "It's Got Ruffles!",
//         FormdbName = "form_Flirty_Three-Tiered_Skirt_Martiandawn",
//         Description = "Planning to dance away the evening club-hopping? Nothing beats a sexy, tiered miniskirt for a night of dancing on the town. Everyone on the dance floor will focus their attention on your derriere as it bounces to the beat beneath those flirty ruffles. And with this spell, you won't even have to go our shopping to find one!",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         DiscoveryMessage = "A schoolgirl walks by, her skirt flouncing to reveal her panties with each roll of her hips. As cute as her plaid miniskirt is, you can't help but think it would be nice to have a few more fashion options to choose from. Then it strikes you. Magic can provide a solution to that!",
//         LearnedAtRegion = "sorority",

//}, new StaticSkill {
//         dbName = "skill_Show_Me_Where_it_Hurts_Ashley_Maid",
//         FriendlyName = "Show Me Where it Hurts",
//         FormdbName = "form_Slutty_Nurse_Ashley_Maid",
//         Description = "With the ongoing chaos in the streets of this town, injuries are unavoidable. This spell will satisfy your needs in medical attention, transforming your opponent into a hospital nurse. Of course, a regular nurse would be boring, so...",
//         ManaCost = 8,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 10,
//         LearnedAtLocation = "college_vet",

//}, new StaticSkill {
//         dbName = "skill_Happily_Ever_After_BlackTGKitty",
//         FriendlyName = "Happily Ever After",
//         FormdbName = "form_Sexy_Princess_BlackTGKitty",
//         Description = "With this spells you can make all those happy stories you read when you were young, come true.",
//         ManaCost = 9.5M,
//         HealthDamageAmount = 3.25M,
//         TFPointsAmount = 8.5M,
//         LearnedAtLocation = "sorority_bedrooms",

//},


//new StaticSkill {
//         dbName = "skill_Make_Love_Not_War_Varn",
//         FriendlyName = "Make Love Not War",
//         FormdbName = "form_New_Age_Retro_Hippy_Varn",
//         Description = "War - what is it good for?  Putting you at risk of being turned into clothing, that's what!  And other than perpetuating the cycle of violence, what can you do?  Some have tried creating pets, or immobilizing their attackers in some way, but you have a revolutionary idea: free their minds instead.  A real solution!  Once they won't want to make anyone into anything anyhow, you know that when you talk about destruction, you just know that you can count the victim of this spell out.  Yeah - it'll be all right indeed.",
//         ManaCost = 12,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "campground_makeout",

//}, new StaticSkill {
//         dbName = "skill_Make_a_Wish_Danaume_Rook",
//         FriendlyName = "Make a Wish",
//         FormdbName = "form_Djinn_Slave_Danaume_Rook",
//         Description = "There are few forces out there more powerful than a sorcerer.  This spell will turn your opponent into one such being, but at the price of enslaving them for eternity.",
//         ManaCost = 6,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "sorority_backyard",

//}, new StaticSkill {
//         dbName = "skill_Vg'f_Lbhe_Freir!__themorpher606",
//         FriendlyName = "Vg'f Lbhe Freir! ",
//         FormdbName = "form_Enchanted_Tennis_Racket_themorpher606",
//         Description = "This spell transforms its victim into an enchanted professional tennis racket.",
//         ManaCost = 10,
//         HealthDamageAmount = 3.14M,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "park_tennis",


//}, new StaticSkill {
//         dbName = "skill_Ratticus_Buckthievious_Draggony",
//         FriendlyName = "Ratticus Buckthievious",
//         FormdbName = "form_Rat_Thief_Draggony",
//         Description = "Looking to cause some havoc? Fill your coin purse perhaps? Whatever the reason this spell will score you a loyal (Or so you hope) thief that will pocket valuables and do so with glee!",
//         ManaCost = 8.5M,
//         HealthDamageAmount = 2.75M,
//         TFPointsAmount = 8.5M,
//         LearnedAtLocation = "castle_treasury",

//}, new StaticSkill {
//         dbName = "skill_Love_Struck_Christopher",
//         FriendlyName = "Love Struck",
//         FormdbName = "form_Heart_Shaped_Sunglasses_Christopher",
//         Description = "The world can be harsh, its appearance grim and future sights unpleasant to contemplate, especially if you end up on some bimbo's feet or stuck in a set of drawers. What you need are some rose tinted glasses, a sunnier view of pleasant people and the wonderful world. Such spectacles are a wonder indeed, and your victim may even thank you for their new optimistic views.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "salon_front_desk",

//}, new StaticSkill {
//         dbName = "skill_Fit_to_Be_Tied!_Martiandawn",
//         FriendlyName = "Fit to Be Tied!",
//         FormdbName = "form_Flirty_Tied_Crop_Top_Martiandawn",
//         Description = "Sometimes it can be such a bother to have to deal with buttons, especially if you need to get undressed in a hurry. It's so much simpler to tie off your top and be done with it, and when you need to strip all it takes is a quick tug and you're in business. This spell was designed to address that issue!",
//         ManaCost = 7.5M,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtRegion = "salon",

//}, new StaticSkill {
//         dbName = "skill_Let's_Make_a_Deal_Rayner",
//         FriendlyName = "Let's Make a Deal",
//         FormdbName = "form_Dapper_Charmer_Rayner",
//         Description = "You stare at the black Ace of Spades in your hand, your prize from the dapper gentlemen you met in the streets earlier. A black magic emanates from it, the ink on the card runny, wavy as you look. You can feel the power within ready to have your magic caste through it, to summon a man like him to make another deal. ",
//         ManaCost = 8,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 6,
//         LearnedAtRegion = "streets",

//}, new StaticSkill {
//         dbName = "skill_Shadow's_Embrace_Techhead",
//         FriendlyName = "Shadow's Embrace",
//         FormdbName = "form_Star-Studded_Cloak_Techhead",
//         Description = "A summoning spell that pulls some extra-dimensional something or other into this realm, giving you a useful cloak or something. But it needs a host, willing or otherwise, and you're damn sure it isn't going to be you.",
//         ManaCost = 11,
//         HealthDamageAmount = 6,
//         TFPointsAmount = 7,
//         LearnedAtLocation = "sorority_rooftop",


//}, new StaticSkill {
//         dbName = "skill_Put_a_Sock_In_It_Christopher",
//         FriendlyName = "Put a Sock In It",
//         FormdbName = "form_Knee-High_Socks_Christopher",
//         Description = "Are you looking for something to cover your long legs? Something nice and warm, thicker than tights but longer than those bobby socks; a perfect middle ground, crisp white knee high socks, comfortable and breathable, perfect for wearing whilst exercising or just to compliment a nice red skirt. Now, all you need is a volunteer, ready to cling to your legs and tickle your toes, whether they like it or not.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "college_track",

//}, new StaticSkill {
//         dbName = "skill_Sly_Fox_Rayner",
//         FriendlyName = "Sly Fox",
//         FormdbName = "form_Fox_Thief_Rayner",
//         Description = "Give your foe a taste for the shadows and a life of furry crime. ",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 7,
//         LearnedAtLocation = "pool_concessions",

//}, new StaticSkill {
//         dbName = "skill_Staff_Fetish_Lily",
//         FriendlyName = "Staff Fetish",
//         FormdbName = "form_Fetish_Wizard's_Staff_Lily",
//         Description = "This spell turns the victim into a sexy, smooth staff, full of lust filled desires. The staff unravels the secret fetishes of others and lock them in wonderful sexual prison of delight.",
//         ManaCost = 8,
//         HealthDamageAmount = 4.5M,
//         TFPointsAmount = 8,
//         LearnedAtRegion = "comicstore",

//}, new StaticSkill {
//         dbName = "skill_Maiden's_Armor_Keyne_Vangsten",
//         FriendlyName = "Maiden's Armor",
//         FormdbName = "form_Female_Possesed_Armor_Keyne_Vangsten",
//         Description = "You've always thought that normal clothing wasn't that protective. Now to give those barely clothed women proper protection!",
//         ManaCost = 9,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 9,
//         LearnedAtLocation = "castle_armory",

//}, new StaticSkill {
//         dbName = "skill_Liquefy_V2.0_GooGirl",
//         FriendlyName = "Liquefy V2.0",
//         FormdbName = "form_Cuddly_Pocket_Goo_Girl_GooGirl",
//         Description = "Not to be used interchangeably with older versions of Liquefy! The mental component of the transformation has been ramped up, while the spell now shrinks the target in addition to liquefying them. The downside is a somewhat increased mana cost and a negative impact on the environment, as the goo that disappears in the shrinking process must end up somewhere else.",
//         ManaCost = 10,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "lab_offices",

//}, new StaticSkill {
//         dbName = "skill_Shadow_Ditz_Great_Daeo",
//         FriendlyName = "Shadow Ditz",
//         FormdbName = "form_Gimbo_Stripper_Great_Daeo",
//         Description = "Created by the hand of a Goth girl who was sick of been laugh at for being different. Unfortunately for her, she misspelled some of the words and it had some nasty, or some should say interesting, side effects. The spell as being passed on since them. It is now used to turn people into eager Goth Bimbo who have an aversion to clothing's and a insatiable libido.",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 10,
//         LearnedAtLocation = "stripclub_bar_seats",

//}, new StaticSkill {
//         dbName = "skill_Mommy_Needs_Panties!_Lily",
//         FriendlyName = "Mommy Needs Panties!",
//         FormdbName = "form_Maternity_Panties_Lily",
//         Description = "Pregnant mommies need nice fitting panties like everyone else but some times it is hard to find some that fit just right, especially when you want one that has a soul attached so that they can rearrange and change shape as you progress. ",
//         ManaCost = 9,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 7.5M,
//         LearnedAtLocation = "candystore_shelves",

//}, new StaticSkill {
//         dbName = "skill_That's_No_Cat_Nyx",
//         FriendlyName = "That's No Cat",
//         FormdbName = "form_Femboi_Skunk_Nyx",
//         Description = "Skunks are cute, but they also stink. Luckily you may know of a way to make one without the stench. ",
//         ManaCost = 7,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 7.5M,
//         LearnedAtLocation = "pathway_campground",

//}, new StaticSkill {
//         dbName = "skill_Exeunt_Omnes_Varn",
//         FriendlyName = "Exeunt Omnes",
//         FormdbName = "form_Iniquitous_Omnibus_Varn",
//         Description = "\"Exeunt omnes!\" - read the incantation you glimpsed in the collected spellbooks - \"Exeunt omnes spiritae corpus!  Possidete est formae de semimarem duodecim daemonum!\"  The letters of the spell seem to blaze within your mind, urging you to speak them ... though given their source, doing so may be quite dangerous.",
//         ManaCost = 6.66M,
//         HealthDamageAmount = 3.33M,
//         TFPointsAmount = 6.66M,
//         LearnedAtLocation = "stripclub_office",

//}, new StaticSkill {
//         dbName = "skill_Quiver_and_Shaft_Aingelproject667",
//         FriendlyName = "Quiver and Shaft",
//         FormdbName = "form_Elven_Femboy_Aingelproject667",
//         Description = "It is no surprise that the population of elves in these parts has dwindled so, especially ever since you casters came around and started turning all our males into females, and then into panties, shoes, animals and other things that..aren't elves. With this spell, you can remedy that! And...make them a little cuter while you're at it. ",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 6,
//         LearnedAtLocation = "pathway_campground",

//}, new StaticSkill {
//         dbName = "skill_Return_of_the_King_Varn",
//         FriendlyName = "Return of the King",
//         FormdbName = "form_Elvish_Bard_Varn",
//         Description = "Casting this spell will create a beautiful elvish woman with a superb singing voice, and a powerful stage presence.  Any similarity to persons living or (presumed) dead is entirely coincidental.",
//         ManaCost = 7,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "concert_hall_stage",

//}, new StaticSkill {
//         dbName = "skill_Circine's_Compassion_LexamTheGemFox",
//         FriendlyName = "Circine's Compassion",
//         FormdbName = "form_Unicorn_Mare_LexamTheGemFox",
//         Description = "The dreams of peace and kindness from the heart of a special young witch. This spell was given birth by the tears she shed from the cruelty of the spell casters in this town and how many lost their free will just for the chance at glory and power. This spell changes the one it is cast on into a female creature known for it's kindness, love, healing, and compassion.",
//         ManaCost = 10,
//         HealthDamageAmount = 7.25M,
//         TFPointsAmount = 9,
//         LearnedAtLocation = "ranch_bedroom_teenager",

//}, new StaticSkill {
//         dbName = "skill_Identify_Medli",
//         FriendlyName = "Identify",
//         FormdbName = "form_Hand_Lens_Medli",
//         Description = "Transforms your target into a magnifying glass to help you seek out your targets a little more efficiently.",
//         ManaCost = 10,
//         HealthDamageAmount = 5,
//         TFPointsAmount = 8,
//         LearnedAtLocation = "mansion_study",

//}, new StaticSkill {
//         dbName = "skill_Sunny_Happiness_Great_Daeo",
//         FriendlyName = "Sunny Happiness",
//         FormdbName = "form_Beach_Babe_Enthusiast_Great_Daeo",
//         Description = "Ever wonder why women seems to enjoy sunbathing so much. Well you can find out on your own with this spell, turn any unaware targets into a blonde beach bombshell that just begs to show her well toned body to everyone... and the sun of course.",
//         ManaCost = 8,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 9,
//// YOU MUST DO THIS YOURSELF
//         LearnedAtLocation = "pool_shallow",

//}, new StaticSkill {
//         dbName = "skill_Doggy_Owner_Arnisd",
//         FriendlyName = "Doggy Owner",
//         FormdbName = "form_Dog_Collar_Arnisd",
//         Description = "What's a dog lover without having at least one collar to put around their beloved pet?  This spell changes something, or someone, into a pink leather belt of love and compassion... or just to put a leash on for walks",
//         ManaCost = 6,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 9,
//         LearnedAtLocation = "apartment_dog_park",

//}, new StaticSkill {
//         dbName = "skill_Writ_of_Sealing_Taenil_Auxifur",
//         FriendlyName = "Writ of Sealing",
//         FormdbName = "form_Magical_Binding_Tattoo_Taenil_Auxifur",
//         Description = "This unique spell, when cast, transforms its victim into a little slip of paper with a designed inked in.  When held against one's skin the ink will transfer in the form of a tattoo.",
//         ManaCost = 10,
//         HealthDamageAmount = 3,
//         TFPointsAmount = 9,
//         LearnedAtLocation = "gym_weights",

//}, new StaticSkill {
//         dbName = "skill_Slippery_Swimsuit_Illia_Malvusin",
//         FriendlyName = "Slippery Swimsuit",
//         FormdbName = "form_One-Piece_Latex_Swimsuit_Illia_Malvusin",
//         Description = "This spell turns the target into a one-piece latex swimsuit, perfect for a nice dip in the local lake, a relaxing swim in the pool, or for engaging in transformative battles with your fellow mages! ",
//         ManaCost = 8,
//         HealthDamageAmount = 3.5M,
//         TFPointsAmount = 8.5M,
//         LearnedAtLocation = "pool_deep",
//},



//#region specific to forms

//new StaticSkill {
//         dbName = "skill_bimbo_kiss",
//         FriendlyName = "Bimbo Kiss Bliss",
//         Description = "When you plant this big, lust-saturated kiss against one of your peers, a bit of bimbo magic will transfer over to them turning them ditzy, just like you!  Teehee!  ",
//         ManaCost = 3,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         GivesEffect="curse_bimbo_kiss",
//         ExclusiveToForm = "form_busty_blonde_bimbo",
//},

//new StaticSkill {
//         dbName = "skill_your_ass_is_mine",
//         FriendlyName = "Your Ass Is Mine",
//         Description = "You're a sexy donkey dominatrix, master of humiliation and coercer into submission.  A powerful slap on the ass is always a good start to breaking your subject's willpower, and this body of yours makes you quite adept at it. ",
//         ManaCost = 4,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         GivesEffect="curse_dominatrix_ass_slap",
//         ExclusiveToForm = "form_Donkey_Dominatrix_LexamTheGemFox_&_Hachik0048",
//},

//new StaticSkill {
//         dbName = "skill_joining_Gaia",
//         FriendlyName = "One With the Earth",
//         Description = "You're a beautiful tree dryad, strong, sturdy, and living as 'green' a lifestyle as possible.  There are many benefits to this form, though many landwalkers are too busy or blind to understand.  Help other mages lay down some foundations by ensnaring them with roots until they see the error of their ways...",
//         ManaCost = 8,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         GivesEffect="curse_rooted_to_the_ground",
//         ExclusiveToForm = "form_enchanted_tree_Sherry_Gray",
//},

//new StaticSkill {
//         dbName = "skill_birdcaller",
//         FriendlyName = "Bird Caller",
//         Description = "As humanoids go, you're rather unique as a red 4-foot-tall bird girl.  As any ornithologist knows, the secret for a male bird to woo a female is by his birdsong, and you're ever so shy and lonely... perhaps you can teach some of your peers how to sing sexy mating calls to you.",
//         ManaCost = 7,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         GivesEffect="curse_singing_like_an_idiot",
//         ExclusiveToForm = "form_Dainty_Birdgirl_Medli",
//},

//new StaticSkill {
//         dbName = "skill_straight_from_the_teat",
//         FriendlyName = "Straight from the Teat",
//         Description = "Many of your peers have great magical powers coming from their new mythic bodies but all you have are impossiblly heavy breasts.  Your lactating nipples dampen everything you wear unless you give them a squeeze every now and then to alleviate some of the pressure inside.  Running away from danger really isn't an option in this body, but a squirt of milk in an enemy's face may help even the odds.",
//         ManaCost = 7,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         GivesEffect="curse_milk_moustache",
//         ExclusiveToForm = "form_bigger_bust",

//}, new StaticSkill {
//         dbName = "skill_Sanguify_Izz_Valentine",
//         FriendlyName = "Sanguify",
//         Description = "That thy foot may be dipped in the blood of thine enemies, and thy tongue in the same.",
//         ManaCost = 10,
//         HealthDamageAmount = 0,
//         TFPointsAmount = 0,
//         ExclusiveToForm = "form_Vampire_Lord_Blood_Knight",
//         GivesEffect = "effect_Hemorrhage_Izz_Valentine",


//}, new StaticSkill {
//         dbName = "skill_Donna's_Bitch_LexamTheGemFox",
//         FriendlyName = "Donna's Bitch",
//         FormdbName = "form_Brainwashed_Bitch_LexamTheGemFox",
//         Description = "Transforms a victim into Donna's canine pet.",
//         ManaCost = 3,
//         HealthDamageAmount = 4,
//         TFPointsAmount = 8,
//         ExclusiveToForm = "form_Donna_LexamTheGemFox",
//         LearnedAtLocation = "nowhere",
//},

//#endregion

////new StaticSkill {
////         dbName = "",
////         FriendlyName = "",
////         Description = "",
////         ManaCost = 3,
////         HealthDamageAmount = 0,
////         TFPointsAmount = 0,
////         GivesEffect="",
////         ExclusiveToForm = "",
////},


            
//        };
//            }

//        }

        public static DbStaticSkill GetStaticSkill(string name)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == name);
        }

        public static IEnumerable<DbStaticSkill> GetAllStaticSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills;
        }

        public static IEnumerable<DbStaticSkill> GetLearnablePsychopathSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.dbName != "" && s.dbName != "lowerHealth" && s.ExclusiveToForm == null && s.GivesEffect == null && (s.LearnedAtLocation != null || s.LearnedAtLocation != null) && s.MobilityType != "full" && s.IsLive == "live");
        }

        public static IEnumerable<DbStaticSkill> GetFormSpecificSkills(string formdbName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToForm == formdbName);
        }

        public static IEnumerable<DbStaticSkill> GetItemSpecificSkills(string itemdbaName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToItem == itemdbaName);
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtLocation(string locationName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtLocation == locationName && s.GivesEffect == null && s.IsLive == "live").ToList();
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtRegion(string regionName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtRegion == regionName && s.GivesEffect == null && s.IsLive == "live").ToList();
        }

       // public static StaticSkills

    }
}