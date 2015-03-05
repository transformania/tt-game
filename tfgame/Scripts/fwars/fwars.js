$(document).ready(function () {

    var p1 = new PersonGroup();

    var person1 = new Person('Alice', 'Magerson');
    person1.levelUp();
    person1.levelUp();
    var person2 = new Person('Bob', 'Marley');
    var person3 = new Person('Argoyle', 'Mauler');
    var person4 = new Person('Ruby', 'Lilia');
    person4.levelUp();
    person4.levelUp();
    person4.levelUp();

    p1.addPerson(person1);
    p1.addPerson(person2);
    p1.addPerson(person3);
    p1.addPerson(person4);


    var p2 = new PersonGroup();

    var person11 = new Person('Leah', 'Logarts');
    person11.levelUp();
    person11.levelUp();
    var person22 = new Person('Gary', 'Fadi');
    var person33 = new Person('Goliath', 'Bads');
    var person44 = new Person('Sassa', 'Fraut');
    var person55 = new Person('Berrie', 'Vasta');
    var person66 = new Person('Holas', 'Shoes');
    person22.levelUp();
    person22.levelUp();
    person22.levelUp();

    p2.addPerson(person11);
    p2.addPerson(person22);
    p2.addPerson(person33);
    p2.addPerson(person44);
    p2.addPerson(person55);
    p2.addPerson(person66);

    p1.fightGroup(p2);

    console.log(p1);
    console.log(p2);


    // var fightResult = person1.fight(person2);



});

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

    if (attackingGroup.length > max) {
        max = attackingGroup.length;
    }

    for (var i = 0; i < max; i++) {

        try {
            // group 1 attacks
            var p2target = attackingGroup.getRandomPerson();
            var fightResult = this.persons[i].fight(p2target);

            console.log("g1 attacking");
            console.log(fightResult);

            if (fightResult == "win") {
                attackingGroup.removePerson(p2target);
            }

        } catch (err) {

        }

        try {
            // group 2 responds
            var p1target = this.getRandomPerson();
            var fightResult2 = attackingGroup.persons[i].fight(p1target);

            console.log("g2 attacking");
            console.log(fightResult2);

            if (fightResult2 == "win") {
                this.removePerson(p1target);
            }
        } catch (err) {

        }


    }



};

// --------------------- PERSON ----------------------

// constructor and model
var Person = function (firstName, lastName) {
    this.firstName = firstName;
    this.lastName = lastName;
    this.level = 1;
    this.type = 'Male Civilian';
    this.status = 'alive';
    this.xp = 0;
};

// methods
Person.prototype.fullName = function () {
    return this.firstName + ' ' + this.lastName;
};

Person.prototype.levelUp = function () {
    this.level++;
};

Person.prototype.fight = function (opponent) {

    var roll = Math.random();
    var levelDiff = this.level - opponent.level;

    if (levelDiff <= 0) {
        levelDiff = 0;
    }

    rollNeeded = levelDiff * .2 + .05;

    console.log(this.fullName() + '(' + this.level + ') fighting ' + opponent.fullName() + '(' + opponent.level + ') with roll ' + roll + '(needs < ' + rollNeeded + ' )');

    if (roll < rollNeeded) {
        opponent.status = 'dead';
        this.xp++;
        return "win";
    } else {
        return "No result.";
    }

};

// ---------------------------------------------

