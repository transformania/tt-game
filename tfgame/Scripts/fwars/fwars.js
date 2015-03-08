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
};

formStatsMap['Fashion Witch'] = {
    'baseHP': 50,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 3,
    'seductionDefense': 0,
};

formStatsMap['Witchling'] = {
    'baseHP': 5,
    'strengthAttack': 1,
    'strengthDefense': 0,
    'seductionAttack': 2,
    'seductionDefense': 0,
};


// ----------------- ITEM TYPE MAP ------------------

var itemStatsMap = {};

itemStatsMap['Cotton Panties'] = {
    'bonusHP': 3,
    'value': 5,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
};

itemStatsMap['Latex Catsuit'] = {
    'bonusHP': 3,
    'value': 5,
    'strengthAttack': 0,
    'strengthDefense': 0,
    'seductionAttack': 1,
    'seductionDefense': 1,
};


// ----------------- ATTACK TEXT MAP ---------------------

var attackTextMap = {};

attackTextMap['Fashion Witch'] = {
    'texts': [
        " makes a nasty comment about their target's hair.  ",
        " pokes fun of her target's skin.  "
    ],

};

attackTextMap['Witchling'] = {
    'texts': [
        " asdfasdfasdf.  ",
        " asdfasdfadsf.  "
    ],

};

function getRandomAttackText(formName) {
    var texts = attackTextMap[formName];
  //  console.log(texts);
  
    //console.log(texts.length);
    var randIndex = Math.floor(Math.random() * texts.length);

    return texts[randIndex];
}

var spawnNumbers = {};

spawnNumbers['bystanders_easy'] = {
    'Fashion Witch': 0,
    'Witchling': 1,
    'Bystander': 5,
};