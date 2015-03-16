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

formStatsMap['Bystander'] = {
    'baseHP': 1,
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
    'baseHP': 5,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
    'energyNeededToRecruit': 200,
    'xpNeededForLevelup': 5,
    'brainwashCost': 35,
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
    'energyNeededToTF': 25,
    'spellTrainingCost': 15,
    'isSpell': true,
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
};


itemStatsMap['Absorb'] = {
    'name': 'Absorb',
    'bonusHP': 0,
    'value': 99999,
    'energyNeededToTF': 25,
    'spellTrainingCost': 25,
    'isSpell': false,
};

//function itemIsSpell(item) {
//    console.log("asdf");
//    return itemStatsMap[item].itemIsSpell;
//}

// ----------------- ATTACK TEXT MAP ---------------------

var attackTextMap = {};

attackTextMap['Fashion Witch'] = {
    'texts': [
        " makes a nasty comment about their target's hair,",
        " pokes fun of her target's skin,"
    ],

};

attackTextMap['Witchling'] = {
    'texts': [
        " zapped her target with a pale blue bolt of lightning,",
        " whispered some words into her target's ear,"
    ],

};

attackTextMap['Bystander'] = {
    'texts': [
        " throws a desperate punch at their target,",
        " shrieks loudly in fright,"
    ],

};

function getRandomAttackText(formName) {
    var texts = attackTextMap[formName].texts;
    var randIndex = Math.floor(Math.random() * texts.length);
    return texts[randIndex];
}

var spawnNumbers = {};

spawnNumbers['park_forest_trail'] = {
    'Fashion Witch': 0,
    'Witchling': 0,
    'Bystander': 2
};

spawnNumbers['park_parking_lot'] = {
    'Fashion Witch': 0,
    'Witchling': 1,
    'Bystander': 3
};

spawnNumbers['park_lodge'] = {
    'Fashion Witch': 0,
    'Witchling': 2,
    'Bystander': 5
};

spawnNumbers['coffeestore'] = {
    'Fashion Witch': 0,
    'Witchling': 4,
    'Bystander': 7
};




var defeatMessage = {};


defeatMessage['brainwash'] = {
    'texts': [
          " stands blankly as their memory of the past hour slowly vanishes from their mind and walks off.",
    ],
};

defeatMessage['Cotton Panties'] = {
    'texts': [
          " gasps and shrinks into a pair of pink panties.",
    ],
};

defeatMessage['Latex Catsuit'] = {
    'texts': [
          " gasps as their skin turns pink and shiny like latex, shrinking down into a tight latex catsuit!",
    ],
};

defeatMessage['Witchling'] = {
    'texts': [
          " moans as a bolt of lighting pierces through their chest, setting their veins aglow with an influx of magic as they transform into a cute young witchling, eager to serve the covenant until inanimation do her part!",
    ],
};

defeatMessage['Absorb'] = {
    'texts': [
          " moans as they are sucked into their attacker's breasts, pushing them up a cup size!",
    ],
};

function getRandomDefeatText(formName) {
    var texts = defeatMessage[formName].texts;
    var randIndex = Math.floor(Math.random() * texts.length);
    return texts[randIndex];
}