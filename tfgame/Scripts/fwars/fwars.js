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
    'xpNeededForLevelup':2,
};

formStatsMap['Fashion Witch'] = {
    'baseHP': 50,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 3,
    'seductionDefense': 0,
    'energyNeededToRecruit': 10000,
    'xpNeededForLevelup': 10,
};

formStatsMap['Witchling'] = {
    'baseHP': 5,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
    'energyNeededToRecruit': 200,
    'xpNeededForLevelup': 5,
};


// ----------------- ITEM TYPE MAP ------------------

var itemStatsMap = {};

itemStatsMap['Cotton Panties'] = {
    'name': 'Cotton Panties',
    'bonusHP': 3,
    'value': 5,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToTF': 25,
};

itemStatsMap['Latex Catsuit'] = {
    'name': 'Latex Catsuit',
    'bonusHP': 3,
    'value': 5,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
    'energyNeededToTF': 25,
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
};

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

spawnNumbers['bystanders_easy'] = {
    'Fashion Witch': 0,
    'Witchling': 1,
    'Bystander': 5,
};

spawnNumbers['bystanders_medium'] = {
    'Fashion Witch': 0,
    'Witchling': 1,
    'Bystander': 5,
};

var defeatMessage = {};

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

function getRandomDefeatText(formName) {
    var texts = defeatMessage[formName].texts;
    var randIndex = Math.floor(Math.random() * texts.length);
    return texts[randIndex];
}