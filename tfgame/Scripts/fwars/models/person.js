﻿// --------------------- PERSON GROUP --------------------

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

                // target is not turned into any item, have them submit
                if (this.persons[i].finishingSpells.length == 0 || this.persons[i].id == leaderId) {
                    fightResult.message += p2target.fullName() + ' falls to their knees and submits to your overwhelming power!';
                    attackingGroup.removePerson(p2target);
                    defeatedAI.addPerson(p2target);


                    // target IS turned into an item, delete them and create the item
                } else {
                    var newItemName = this.persons[i].getRandomFinishingSpell();
                    fightResult.message += insertNames(getRandomDefeatText(this.type, newItemName),this.persons[i], p2target);

                    if (newItemName == "Absorb") {
                        this.persons[i].health += Math.floor(p2target.getMaxHP() / 4);
                        if (this.persons[i].health > this.persons[i].getMaxHP()) {
                            this.persons[i].health - this.persons[i].getMaxHP();
                        }
                    } else {
                        var newItem = new Item(newItemName, p2target.fullName());
                        newItems.addItem(newItem);
                    }
                    attackingGroup.removePerson(p2target);
                }
            }

            logMessages.push(fightResult);

        } catch (err) {

        }

        try {
            // group 2 responds
            var p1target = this.getRandomPerson();
            var fightResult2 = attackingGroup.persons[i].fight(p1target);

            if (fightResult2.outcome == "win") {

                // target is not turned into any item, have them submit
                if (attackingGroup.persons[i].finishingSpells.length == 0) {
                    fightResult2.message += p1target.fullName() + ' falls to their knees and submits to your opponent\'s overwhelming power!';
                    this.removePerson(p1target);
                    defeatedPlayer.addPerson(p1target);


                // target IS turned into an item, delete them and create the item
                } else {
                    var newItemName = attackingGroup.persons[i].getRandomFinishingSpell();
                   // fightResult2.message += p1target.fullName() + ' ' + getRandomDefeatText(newItemName);
                    fightResult2.message += insertNames(getRandomDefeatText(this.type, newItemName), attackingGroup.persons[i], p1target);
                    if (newItemName == "Absorb") {
                        this.persons[i].health += Math.floor(p1target.getMaxHP() / 4);
                        if (this.persons[i].health > this.persons[i].getMaxHP()) {
                            this.persons[i].health - this.persons[i].getMaxHP();
                        }
                    } else {
                        var newItem = new Item(newItemName, p1target.fullName());
                        newItems.addItem(newItem);
                    }


                    this.removePerson(p1target);
                }
            }

            logMessages.push(fightResult2);

        } catch (err) {

        }


    }


    return logMessages;

};

PersonGroup.prototype.spawnGroup = function (type, variation) {

    this.persons = [];
    var spawnGroup = spawnNumbers[type];

    var extra = Math.floor(Math.random() * variation) - (Math.floor(variation / 2));
    for (var i = 0; i < spawnGroup['Female Bystander'] + extra; i++) {
        
        var gender = Math.random();

        if (gender < .5) {
            var spawn = new Person(undefined, undefined, 'Female Bystander');
            spawn.setRandomNameFemale();
            this.addPerson(spawn);
        } else {
            var spawn = new Person(undefined, undefined, 'Male Bystander');
            spawn.setRandomNameMale();
            this.addPerson(spawn);
        }
    }

    extra = Math.floor(Math.random() * variation) - (Math.floor(variation / 2));
    for (var i = 0; i < spawnGroup['Witchling'] + extra; i++) {
        var spawn = new Person(undefined, undefined, 'Witchling');
        this.addPerson(spawn);
    }

};

PersonGroup.prototype.healGroup = function (amount) {
    for (var i = 0; i < this.persons.length; i++) {
        this.persons[i].health += amount;

        if (this.persons[i].health > this.persons[i].getMaxHP()) {
            this.persons[i].health = this.persons[i].getMaxHP();
        }

    }
};

PersonGroup.prototype.mergeIntoGroup = function (otherGroup) {
    for (var i = 0; i < otherGroup.persons.length; i++) {
        this.addPerson(otherGroup.persons[i]);
    }

    otherGroup.persons = [];

};


// --------------------- PERSON ----------------------

// constructor and model
var Person = function (firstName, lastName, type) {

    this.id = personIdNext;
    this.firstName = firstName;
    this.lastName = lastName;
    this.level = 1;

    if (firstName === undefined || lastName === undefined) {
        this.getRandomNameFemale();
    }

    this.status = 'ok';
    this.xp = 0;

    // set type
    if (type === undefined) {
        this.type = 'Female Bystander';
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

Person.prototype.changeType = function (newType) {
    this.type = newType;
    this.xp = 0;
}

Person.prototype.giveItem = function (item) {
    this.items.push(item);
};


Person.prototype.removeItem = function (itemToRemove) {
    var removalIndex = this.items.indexOf(itemToRemove);
    this.items.splice(removalIndex, 1);
}

Person.prototype.giveFinishingSpell = function (spell) {
    this.finishingSpells.push(spell);
};

Person.prototype.knowsSpell = function (spell) {
    var index = this.finishingSpells.indexOf(spell);
    if (index >= 0) {
        return true;
    } else {
        return false;
    }
}


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

    result += '  ' + getRandomAttackText(this.type, opponent.type);
    result = insertNames(result, this, opponent);

    result += '  (' + damage + ' damage)';
    opponent.health -= damage;

    //  rollNeeded = levelDiff * .2 + .05;

    if (opponent.health <= 0) {
        opponent.health = 0;
        outcome = "win";
        opponent.status = 'defeated';
        this.xp++;
        result += "  VICTORY!  ";
    } else {

        outcome = "";
    }

    var resultLog = new Log(result, outcome);

    return resultLog;

};

Person.prototype.getRandomNameFemale = function () {
    this.firstName = randomFirstNamesFemale[Math.floor(Math.random() * randomFirstNamesFemale.length)];
    this.lastName = randomLastName[Math.floor(Math.random() * randomLastName.length)];
};

Person.prototype.setRandomNameFemale = function () {
    this.firstName = randomFirstNamesFemale[Math.floor(Math.random() * randomFirstNamesFemale.length)];
};


Person.prototype.setRandomNameMale = function () {
    this.firstName = randomFirstNamesMale[Math.floor(Math.random() * randomFirstNamesMale.length)];
};

Person.prototype.getRandomFinishingSpell = function () {
    return this.finishingSpells[Math.floor(Math.random() * this.finishingSpells.length)];
}

Person.prototype.resetXP = function () {
    this.xp = 0;
}

Person.prototype.xpNeededForLevelup = function () {
    return formStatsMap[this.type].xpNeededForLevelup + (this.level - 1) * 3;
}

Person.prototype.getRecruitmentCost = function () {
    return formStatsMap[this.type].energyNeededToRecruit;
}

Person.prototype.getBrainwashCost = function () {
    return formStatsMap[this.type].brainwashCost;
}

Person.prototype.getTransformCost = function (itemName) {
    return itemStatsMap[itemName].energyNeededToTF;
}

Person.prototype.itemIsSpell = function (item) {
    return itemStatsMap[item].isSpell;
}

Person.prototype.knowsAbsorb = function () {
    var index = this.finishingSpells.indexOf('Absorb');
    if (index >= 0) {
        return true;
    } else {
        return false;
    }
}

var randomFirstNamesFemale = ['Mary', 'Jane', 'Elessa', 'Jamie', 'Sarah', 'Caroline', 'Janice', 'Gloria', 'Alice', ];
var randomFirstNamesMale = ['James', 'Tim', 'Jason', 'Bob', 'Gary', 'Chris', 'Max', 'Mike', 'Derrick', ];
var randomLastName = ['Smith', 'Brown', 'White', 'Blue', 'Black', 'Green', 'Tanner', 'Shoemaker', 'Telae'];