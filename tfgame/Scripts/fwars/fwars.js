personIdNext = 0;
logsIdNext = 0;
itemIdNext = 0;
logs = [];

var gameStatus = "";

$(document).ready(function () {


});

// --------------------- ITEM --------------------

var Item = function (type) {
    this.type = type;
    this.id = itemIdNext;
    itemIdNext++;
}


// --------------------- LOG --------------------

var Log = function (message, outcome) {

    if (typeof outcome === undefined) {
        outcome = "";
    }

    this.id = logsIdNext;
    logsIdNext++;
    this.message = message;
    this.outcome = outcome;
}

// --------------------- PERSON GROUP --------------------

var PersonGroup = function () {
    this.persons = [];
}

PersonGroup.prototype.addPerson = function (newperson) {
    this.persons.push(newperson);
};

PersonGroup.prototype.removePerson = function (personToRemove) {

    var removalIndex = this.persons.indexOf(personToRemove);
        this.persons.splice(removalIndex, 1);
};

PersonGroup.prototype.getRandomPerson = function () {
    return this.persons[Math.floor(Math.random() * this.persons.length)];
}

PersonGroup.prototype.fightGroup = function (attackingGroup) {

    var max = this.persons.length;

    if (attackingGroup.persons.length > max) {
        max = attackingGroup.persons.length;
    }

    var logMessages = [];

    for (var i = 0; i < max; i++) {

        try {
            // group 1 attacks
            var p2target = attackingGroup.getRandomPerson();
            var fightResult = this.persons[i].fight(p2target);

            if (fightResult.outcome == "win") {
                attackingGroup.removePerson(p2target);
            }

            logMessages.push(fightResult);

        } catch (err) {

        }

        try {
            // group 2 responds
            var p1target = this.getRandomPerson();
            var fightResult2 = attackingGroup.persons[i].fight(p1target);

          //  logs.push(fightResult);

            if (fightResult2.outcome == "win") {
                this.removePerson(p1target);
            }

            logMessages.push(fightResult2);

        } catch (err) {

        }


    }

  
    return logMessages;

};

// --------------------- PERSON ----------------------

// constructor and model
var Person = function (firstName, lastName, type) {

    this.id = personIdNext;
    this.firstName = firstName;
    this.lastName = lastName;
    this.level = 1;
  
    this.status = 'ok';
    this.xp = 0;

    // set type
    if (type === undefined) {
        this.type = 'Male Civilian';
    } else {
        this.type = type;
    }

    // set health based on form
    var formStats = formStatsMap[this.type];
    this.health = formStats.baseHP;

    this.items = [];
    this.finishingSpells = [];

    this.health = formStats.baseHP;

    personIdNext++;

};

// methods
Person.prototype.fullName = function () {
    return this.firstName + ' ' + this.lastName;
};

Person.prototype.levelUp = function () {
    this.level++;
};

Person.prototype.giveItem = function (item) {
    this.items.push(item);
};


Person.prototype.getMaxHP = function () {
    var formStats = formStatsMap[this.type];

    var maxHP = formStats.baseHP;

    // get stats from items
    for (var i = 0; i < this.items.length; i++) {
        var itemForm = itemStatsMap[this.items[i].type];
        maxHP += itemForm.bonusHP;
    }

    return maxHP;
};

Person.prototype.fight = function (opponent) {

    var result = "" + this.fullName() + ' [lvl ' + this.level + ' ' + this.type +  '] fighting ' + opponent.fullName() + ' [lvl ' + opponent.level + ' ' + opponent.type + '].';

    // get stats from form
    var formStatsAttacker = formStatsMap[this.type];
    var formStatsDefender = formStatsMap[opponent.type];

    var damageFromForm = formStatsAttacker.seductionAttack - formStatsDefender.seductionDefense;

    var extraDamageFromGear = 0;


    // get stats from items
    for (var i = 0; i < this.items.length; i++) {
        var itemForm = itemStatsMap[this.items[i].type];
        extraDamageFromGear += itemForm.seductionAttack;
    }

    // get stats from items
  //  for (var i = 0; i < opponent.items.length; i++) {
     //   var itemForm = itemStatsMap[opponent.items[i].type];
     //   damage -= itemForm.seductionDefense;
    //}

  //  console.log("damage from items:");
  //  console.log(extraDamageFromGear);



    var damage = damageFromForm + extraDamageFromGear;

      console.log("damage from form:");
      console.log(damageFromForm);

      console.log("damage from items:");
      console.log(extraDamageFromGear);

   console.log("damage total:");
    console.log(damage);


    if (damage < 1) {
        damage = 1;
    }

    result += '  ' + this.fullName() + ' dealt ' + damage + ' damage.';
    opponent.health -= damage;

    //  rollNeeded = levelDiff * .2 + .05;

    if (opponent.health <= 0) {
        opponent.health = 0;
        outcome = "win";
        opponent.status = 'defeated';
        this.xp++;
        result += "  VICTORY!  " + opponent.fullName() + " was turned into a pair of panties.";
    } else {
      
        outcome = "";
    }

    var resultLog = new Log(result, outcome);

    return resultLog;

};

// ----------------- FORM TYPE STATICS ------------------
    

// ----------------- FORM TYPE MAP ------------------
var formStatsMap = {};

formStatsMap['Male Civilian'] = {
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
    console.log(texts);
  
    //console.log(texts.length);
    var randIndex = Math.floor(Math.random() * texts.length);

    return texts[randIndex];
}