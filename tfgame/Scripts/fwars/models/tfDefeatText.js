/* 
    defeatMessage[NEW FORM][OLD FORM] = {

    New Form is what the victim is turning into
    Old Form is what the victim is before their transformation

*/

var defeatMessage = {};

defeatMessage['brainwash'] = {};
defeatMessage['brainwash']['Generic'] = {
    'texts': [
    '[VICTIM] stands blankly as their memory of the past hour slowly vanishes.  "Like... what was I doing?  Where am I?" they ask as [ATTACKER] smiles hospitably.  "Oh, you were just on your way to your job at the club," [ATTACKER] explains, placing a finger on [VICTIM]\'s forehead as their breasts morph to ample D-cups with a killer body and tight yellow dress to match."  "Oh right... well... thank you!" [VICTIM] replies, jogging away with an ample amount of jiggling.',
    ],
};

defeatMessage['Cotton Panties'] = {};
defeatMessage['Cotton Panties']['Generic'] = {
    'texts': [
    '[VICTIM]  gasps and shrinks into a pair of pink panties.',
    ],
};

defeatMessage['Latex Catsuit'] = {};
defeatMessage['Latex Catsuit']['Generic'] = {
    'texts': [
    '[VICTIM] gasps as the skin on their arms and legs turns pink and shiny, squeaking as they rub together.  "What\s happening to me?" [VICTIM] gasps as their legs give out and their torso collapses into an empty latex glove.  In seconds their head vanishes with one last, long shriek that changes into a rubbery squeak, leaving behind a tight pink latex catsuit on the floor where [VICTIM] had stood moments before.  "Lovely!", [ATTACKER] purrs, stroking its latex counters and inducing a few more orgasmic-sounding squeaks out of [VICTIM]\'s new form!',
    ],
};

defeatMessage['Comfy Sports Bra'] = {};
defeatMessage['Comfy Sports Bra']['Generic'] = {
    'texts': [
    '[VICTIM] gasps as their chest erupts into two wire-supported cups pushing out, arms and legs vanishing as the torso shrinks down to a few straps and buckle.  [VICTIM] clatters on the ground, the metal hook clinking on the ground.',
    ],
};

defeatMessage['Lacy Corset'] = {};
defeatMessage['Lacy Corset']['Generic'] = {
    'texts': [
    'A pair of black strings appear out of [VICTIM]\s hips, coiling around them like a pair of twin serpants until their arms are bound tightly to their sides.  "I... can\t... breath"... [VICTIM] coughs moments before their limbs shrink into lacy frills running along the counters of their chest, head sucked in to a now-hollow torso defined by a violet garment, a corset tightly bound by the settling black strings.  It flops on the ground, empty and motionless, though one can\'t help but feel it beckins for someone to wrap around and squeeze tight.',
    ],
};

defeatMessage['Witchling'] = {};
defeatMessage['Witchling']['Generic'] = {
    'texts': [
    '[VICTIM] moans as a bolt of lighting pierces through their chest, setting their veins aglow with an influx of magic as they transform into a cute young witchling, eager to serve the covenant until inanimation do her part!',
    ],
};

defeatMessage['Senior Witchling'] = {};
defeatMessage['Senior Witchling']['Generic'] = {
    'texts': [
    '[VICTIM] giggles elatedly as the magic fills her body, expanding her breasts and hips a bit larger, her skin a little clearer and the predatorial glint in her eyes just a bit stronger and ruthless.',
    ],
};

defeatMessage['Apprentice'] = {};
defeatMessage['Apprentice']['Generic'] = {
    'texts': [
    '[VICTIM] apprentice text',
    ],
};

defeatMessage['Mage'] = {};
defeatMessage['Mage']['Generic'] = {
    'texts': [
    '[VICTIM] mage text',
    ],
};

defeatMessage['Absorb'] = {};
defeatMessage['Absorb']['Generic'] = {
    'texts': [
    '"Shhhhhh..." [ATTACKER] whispers, placing [VICTIM]\'s lips to her breast.  Both moan as [VICTIM] is slowly sucked into their [ATTACKER]\'s breasts until the last few toes have vanished through thei nipples, inflating [ATTACKER]\'s boobs up a cup size and stealing [VICTIM]\s life force into their merged body.',

     '"I\'m going to send you somewhere where you\'ll always feel welcome..." [ATTACKER] whispers, sitting down on [VICTIM]\'s back as if riding a pony.  [VICTIM]\'s eyes shoot wide open as the skin on their back starts to get sucked into [ATTACKER]\'s clit, head and limbs shrinking and dissapearing before the last of [VICTIM]\'s torso vanishes between [ATTACKER]\'s trembling thighs.  [ATTACKER] continues to finger her new hypersensitive, enchanced pussy for a while before returning to full attention.',

    ],
};


function getRandomDefeatText(victimForm, defeatType) {
    try {
        var texts = defeatMessage[defeatType][victimForm].texts;
        var randIndex = Math.floor(Math.random() * texts.length);
        var result = texts[randIndex];
        return result;
    } catch (e) {
        var texts = defeatMessage[defeatType]['Generic'].texts;
        var randIndex = Math.floor(Math.random() * texts.length);
        var result = texts[randIndex];
        return result;
    }
}