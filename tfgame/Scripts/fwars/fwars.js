personIdNext = 0;
logsIdNext = 0;
logs = [];

var gameStatus = "";

$(document).ready(function () {


});

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
    this.health = 5;

    // set type
    if (type === undefined) {
        this.type = 'Male Civilian';
    } else {
        this.type = type;
    }

    // set health based on form
    var formStats = formStatsMap[this.type];
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

Person.prototype.fight = function (opponent) {

    var result = "" + this.fullName() + ' [lvl ' + this.level + ' ' + this.type +  '] fighting ' + opponent.fullName() + ' [lvl ' + opponent.level + ' ' + opponent.type + '].';

    var formStatsAttacker = formStatsMap[this.type];
    var formStatsDefender = formStatsMap[opponent.type];

    console.log(formStatsAttacker);
    console.log(formStatsDefender);

    var damage = formStatsAttacker.seductionAttack - formStatsDefender.seductionDefense;

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
    'seductionDefense': 3,
};