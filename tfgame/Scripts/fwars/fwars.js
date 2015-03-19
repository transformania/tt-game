leaderId = -1;
personIdNext = 0;
logsIdNext = 0;
itemIdNext = 0;
logs = [];

var gameStatus = "";

$(document).ready(function () {


});





// ----------------- FORM TYPE MAP ------------------
var formStatsMap = {};

formStatsMap['Female Bystander'] = {
    'baseHP': 2,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,
    'xpNeededForLevelup': 2,
    'brainwashCost': 15,
};

formStatsMap['Male Bystander'] = {
    'baseHP': 2,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToRecruit': 100,
    'xpNeededForLevelup': 2,
    'brainwashCost': 15,
};

formStatsMap['Fashion Witch'] = {
    'baseHP': 50,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 3,
    'seductionDefense': 0,
    'energyNeededToRecruit': 10000,
    'xpNeededForLevelup': 10,
    'brainwashCost': 9999,
};

formStatsMap['Witchling'] = {
    'baseHP': 4,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
    'energyNeededToRecruit': 200,
    'xpNeededForLevelup': 5,
    'brainwashCost': 35,
};

formStatsMap['Senior Witchling'] = {
    'baseHP': 8,
    'strengthAttack': 1,
    'strengthDefense': 1,
    'seductionAttack': 3,
    'seductionDefense': 3,
    'energyNeededToRecruit': 300,
    'xpNeededForLevelup': 8,
    'brainwashCost': 50,
};


// ----------------- ITEM TYPE MAP ------------------

var itemStatsMap = {};

itemStatsMap['Cotton Panties'] = {
    'name': 'Cotton Panties',
    'bonusHP': 3,
    'value': 10,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToTF': 20,
    'spellTrainingCost': 15,
    'isSpell': true,
    'isPurchaseableItem': true,
    'Description' : 'A pair of common pink panties.  Nothing too special or sexy.',
};

itemStatsMap['Latex Catsuit'] = {
    'name': 'Latex Catsuit',
    'bonusHP': 3,
    'value': 15,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToTF': 25,
    'spellTrainingCost': 25,
    'isSpell': true,
    'isPurchaseableItem': true,
    'Description': 'A tight pink catsuit.  She squeaks a lot even when slightly touched; one can only imagine how she must feel when someone squeezes inside!',
};

itemStatsMap['Comfy Sports Bra'] = {
    'name': 'Comfy Sports Bra',
    'bonusHP': 4,
    'value': 25,
    'strengthAttack': 0,
    'strengthDefense': 2,
    'seductionAttack': 0,
    'seductionDefense': 0,
    'energyNeededToTF': 35,
    'spellTrainingCost': 40,
    'isSpell': true,
    'isPurchaseableItem': true,
    'Description': 'A comfortable sports bra, perfect for bustier witches who need to stay light on their feet.',
};

itemStatsMap['Lacy Corset'] = {
    'name': 'Lacy Corset',
    'bonusHP': -2,
    'value': 40,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 5,
    'seductionDefense': 5,
    'energyNeededToTF': 50,
    'spellTrainingCost': 40,
    'isSpell': true,
    'isPurchaseableItem': true,
    'Description': 'A lacy violet corset, drawn tight around the waist with black strings.  A bit uncomfortable to wear but worth it... maybe.',
};

itemStatsMap['Ankh Necklace'] = {
    'name': 'Ankh Necklace',
    'bonusHP': 3,
    'value': 25,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToTF': 50,
    'spellTrainingCost': 200,
    'isSpell': false,
    'isPurchaseableItem': true,
    'Description': 'An enchanted little amulet, boosting the power of whichever witch or wizard wearing it.',
};


itemStatsMap['Absorb'] = {
    'name': 'Absorb',
    'bonusHP': 0,
    'value': 99999,
    'energyNeededToTF': 25,
    'spellTrainingCost': 25,
    'isSpell': false,
    'isPurchaseableItem': false,
};


// ----------------- ATTACK TEXT MAP ---------------------



var spawnNumbers = {};

spawnNumbers['park_forest_trail'] = {
    'Female Bystander': 2,
    'Fashion Witch': 0,
    'Witchling': 0,
    'Senior Witchling': 0,
   
};

spawnNumbers['park_parking_lot'] = {

     'Female Bystander': 3,
    'Fashion Witch': 0,
    'Witchling': 1,
    'Senior Witchling': 0,
   
};

spawnNumbers['park_lodge'] = {
    'Female Bystander': 5,
    'Fashion Witch': 0,
    'Witchling': 2,
    'Senior Witchling': 0,
};

spawnNumbers['coffeestore'] = {
    'Female Bystander': 7,
    'Fashion Witch': 0,
    'Witchling': 4,
    'Witchling': 1,
  
};




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