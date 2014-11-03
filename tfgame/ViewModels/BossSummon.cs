using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public static class BossSummonDictionary
    {
        public static Dictionary<string, BossSummon> GlobalBossSummonDictionary = new Dictionary<string, BossSummon> {
    { "ranch_bedroom", new BossSummon { BossName="Donna", MinimumTurn = 1000, ActivationText = "As you search around in the master bedroom a strange jewel catches your eye. You grin and reach out, your fingertips brushing the jewel as you intend to steal for yourself. There is a flash of light and a nude woman is now standing in the center of the bedroom. \"Circine you better have a good reason for......\" She cuts her own statements off and looks at you with a grin. \"Oh my looks like I will get to have some fun tonight.\" She grins and snaps, green robes materializing around her body. \"But before we get to the fun you shall be punished for summoning me.\" She grins as a wave of powerful magic starts to bear down around you." }},

     { "castle_armory", new BossSummon { BossName="Valentine", MinimumTurn = 750, ActivationText = "You reach the door to the armory of the Valentine Castle; with the intention of looking for anything   interesting. However subtle details of the building seem oddly different. For one, the crimson door is now coal black. You enter and realize it is not the armory you were expecting to see. Perhaps Queen Valentine has renovated the castle. Why wouldn’t vampires care about interior design like everyone else?\n\nStepping into an oval room lit by lanterns along the walls you now see a vast space without a hint of furniture. This room is undoubtedly some kind of an arena. Opposite the door from which you entered you hear footsteps echoing acrossing the room. The begins to air broil with dreadful energy. A man enters your range of vision, the light flickering wildly, frames his dark silhouette. He stops at what you believe to be the center of the arena. The wavering lantern wicks settle, and you discern him more clearly. He wears a silk dress shirt, and black dress pants cling to his skin allowing agile movement. Rose tinted black boots grace his feet, and silver tresses of hair fall over his shoulders and down his back in a dashing ponytail. Finally a black cloak that drapes down his frame with the Valentine crest, a beautifully crafted silver rose, emblazoned upon it.\n\nHe grins as though he’s found his prey, and a cold chill prickles down your spine and making you take a step back.\n\n'Welcome to my castle, so glad you decided to visit.' His voice, heavy with bass, rumbles throughout the room. While his eyes watch your every breathe.\n\n'Very well then, I am in need of a sparring partner as my Childe is very busy, but first...' He leans his chest over into a graceful bow.\n\n'Israel Victis Valentine, First Lord of the Valentine Castle, and the founder and creator of this beautiful estate.' He straightens himself out and stretches his shoulders as though to loosen himself up. The Lord takes up a combat stance as magic swirls through the air into his hands forming two masterfuly crafted swords. \n\n'Now with no further hesitation I believe it is time for us to duel.' He lets a rich laugh fill the arena as a hint of bloodlust shines in his eyes." }},

     { "stripclub_bar_seats", new BossSummon { BossName="BimboBoss", MinimumTurn = 250, ActivationText = "" }},


    };

    }

    public class BossSummon
    {
        public string BossName { get; set; }
        public int MinimumTurn { get; set; }
        public string ActivationText { get; set; }
    }
}