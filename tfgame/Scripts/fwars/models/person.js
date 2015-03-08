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

PersonGroup.prototype.fightGroup = function (attackingGroup, defeatedAI, defeatedPlayer, newItems) {

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
                //defeatedAI.addPerson(p2target);

                var newItem = new Item('Latex Catsuit');
                newItems.addItem(newItem);

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
                defeatedPlayer.addPerson(p2target);
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

    if (firstName === undefined || lastName === undefined) {
        this.getRandomName();
    }

    this.status = 'ok';
    this.xp = 0;

    // set type
    if (type === undefined) {
        this.type = 'Bystander';
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

    var result = "" + this.fullName() + ' [lvl ' + this.level + ' ' + this.type + '] fighting ' + opponent.fullName() + ' [lvl ' + opponent.level + ' ' + opponent.type + '].';

    // get stats from form
    var formStatsAttacker = formStatsMap[this.type];
    var formStatsDefender = formStatsMap[opponent.type];

    var damageFromForm = formStatsAttacker.seductionAttack - formStatsDefender.seductionDefense;

    var extraDamageFromGear = 0;


    // get stats from items for attacker
    for (var i = 0; i < this.items.length; i++) {
        var itemForm = itemStatsMap[this.items[i].type];
        extraDamageFromGear += itemForm.seductionAttack;
    }

    // get stats from items
      for (var i = 0; i < opponent.items.length; i++) {
       var itemForm = itemStatsMap[opponent.items[i].type];
       damage -= itemForm.seductionDefense;
    }

    var damage = damageFromForm + extraDamageFromGear;

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

Person.prototype.getRandomName = function () {
    this.firstName = randomFirstNames[Math.floor(Math.random() * randomFirstNames.length)];
    this.lastName = randomLastName[Math.floor(Math.random() * randomLastName.length)];
};

PersonGroup.prototype.spawnGroup = function (type) {

    this.persons = [];
    console.log("spawning...");


    var spawnGroup = spawnNumbers[type];


    for (var i = 0; i < spawnGroup['Bystander']; i++) {
        var spawn = new Person(undefined, undefined, 'Bystander');
        this.addPerson(spawn);
    }
    for (var i = 0; i < spawnGroup['Witchling']; i++) {
        var spawn = new Person(undefined, undefined, 'Witchling');
        this.addPerson(spawn);
    }

};

var randomFirstNames = ['Mary', 'Jane', 'Elessa', 'Jamie', 'Sarah', 'Caroline', 'Janice', 'Gloria', 'Alice',];
var randomLastName = ['Smith', 'Brown', 'White', 'Blue', 'Black', 'Green', 'Tanner', 'Shoemaker', 'Telae'];