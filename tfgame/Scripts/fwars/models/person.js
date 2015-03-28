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

PersonGroup.prototype.getNextFighter = function () {
    for (var i = 0; i < this.persons.length; i++) {
        if (this.persons[i].hasAttacked == false) {
            return this.persons[i];
        }
    }
    return -1;
}

PersonGroup.prototype.resetFighterMarkers = function () {
    for (var i = 0; i < this.persons.length; i++) {
        this.persons[i].hasAttacked = false;
    }
}

PersonGroup.prototype.getAvailableFighterCount = function ()
{
    var freeCount = 0;
    for (var i = 0; i < this.persons.length; i++) {
        if (this.persons[i].hasAttacked == false) {
            freeCount++;
        }
    }
    return freeCount;
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
                    fightResult.message += "<span class='submit'>" + p2target.fullName() + ' falls to their knees and submits to your overwhelming power!</span>';
                    attackingGroup.removePerson(p2target);
                    defeatedAI.addPerson(p2target);


                    // target IS turned into an item, delete them and create the item
                } else {
                    var newItemName = this.persons[i].getRandomFinishingSpell();
                    fightResult.message += "<span class='inanimated'>" + insertNames(getRandomDefeatText(this.type, newItemName),this.persons[i], p2target) + "</span>";

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
                    fightResult2.message += "<span class='submit'>" + p1target.fullName() + ' falls to their knees and submits to your opponent\'s overwhelming power!</span>';
                    this.removePerson(p1target);
                    defeatedPlayer.addPerson(p1target);


                // target IS turned into an item, delete them and create the item
                } else {
                    var newItemName = attackingGroup.persons[i].getRandomFinishingSpell();
                   // fightResult2.message += p1target.fullName() + ' ' + getRandomDefeatText(newItemName);
                    fightResult2.message += "<span class='inanimated'>" + insertNames(getRandomDefeatText(this.type, newItemName), attackingGroup.persons[i], p1target) + "</span>";
                    if (newItemName == "Absorb") {
                        attackingGroup.persons[i].health += Math.floor(p1target.getMaxHP() / 4);
                        if (attackingGroup.persons[i].health > attackingGroup.persons[i].getMaxHP()) {
                            attackingGroup.persons[i].health - attackingGroup.persons[i].getMaxHP();
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

   // this.persons = [];
    var spawnGroup = locationsMap[type];
    var extra = Math.floor(Math.random() * variation);
 
    for (var i = 0; i < spawnGroup['Female Bystander'][0] + spawnGroup['Female Bystander'][1]; i++) {
        
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

    extra = Math.floor(Math.random() * variation);
    for (var i = 0; i < spawnGroup['Witchling'][0] + spawnGroup['Witchling'][1]; i++) {
        var spawn = new Person(undefined, undefined, 'Witchling');
        spawn.giveFinishingSpell('Cotton Panties');
        this.addPerson(spawn);
    }

    extra = Math.floor(Math.random() * variation);
    for (var i = 0; i < spawnGroup['Senior Witchling'][0] + spawnGroup['Senior Witchling'][1]; i++) {
        var spawn = new Person(undefined, undefined, 'Senior Witchling');
        spawn.giveFinishingSpell('Cotton Panties');
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

    this.hasAttacked = false;

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

Person.prototype.heal = function (healAmount) {
    this.health += healAmount;

    if (this.health > this.getMaxHP()) {
        this.health = this.getMaxHP();
    }
};

Person.prototype.fight = function (opponent) {

    this.hasAttacked = true;

    var result = "<span class='fight'>" + this.fullName() + ' [lvl ' + this.level + ' ' + this.type + '] fighting ' + opponent.fullName() + ' [lvl ' + opponent.level + ' ' + opponent.type + '].</span>';

    // get stats from form
    var formStatsAttacker = formStatsMap[this.type];
    var formStatsDefender = formStatsMap[opponent.type];

    var levelRequiredForMyPromotion = this.levelRequiredForPromotion();

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

        

        // assign some XP if this person is not stuck on waiting for upgrade
        if (this.level < levelRequiredForMyPromotion) {
            this.xp++;
        }

        if (this.xp >= this.xpNeededForLevelup()) {
            this.level++;
            this.xp = 0;
            result += " LEVEL UP!  ";
        }

        result += "  VICTORY!  ";
    } else {
        if (this.level < levelRequiredForMyPromotion) {
            this.xp += .25;;
        }
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

Person.prototype.levelRequiredForPromotion = function () {
    return formStatsMap[this.type].levelRequiredForPromotion;
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

Person.prototype.getPromotionForms = function () {
    return formStatsMap[this.type].promotesTo;
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

Person.prototype.canLearnSpells = function () {
    if (this.type == 'Male Bystander' || this.type == 'Female Bystander') {
        return false;
    } else {
        return true;
    }
}

Person.prototype.getHealthAsPercentage = function () {
    return (this.health / this.getMaxHP()) * 100;
}

Person.prototype.getGraphic = function () {
    return formStatsMap[this.type].graphic;
}

var randomFirstNamesFemale = ['Mary', 'Jane', 'Elessa', 'Jamie', 'Sarah', 'Caroline', 'Janice', 'Gloria', 'Alice', 'Janice', 'Ellen', 'Amy', 'Bella', 'Crissy', 'Elizabeth', 'Laura', 'Eileen','Lily','Debby', 'Darla', 'Janice', 'Greta'];

var randomFirstNamesMale = ['Aden', 'Alden', 'Alexis', 'Alfredo', 'Allen', 'Angel', 'Bart', 'Beckett', 'Bernard', 'Blake', 'Bo', 'Bob', 'Braedon', 'Braylen', 'Braylon', 'Brendan', 'Brodie', 'Byron', 'Cade', 'Cassius', 'Chris', 'Cohen', 'Corey', 'Cortez', 'Cruz', 'Cullen', 'Curtis', 'Dakota', 'Damari', 'Dangelo', 'Dante', 'Darius', 'Darrell', 'Darren', 'Declan', 'Demarcus', 'Demarion', 'Derrick', 'Deryl', 'Devyn', 'Dexter', 'Drew', 'Easton', 'Emerson', 'Emiliano', 'Esteban', 'Evan', 'Everett', 'Gary', 'Gary', 'George', 'Gianni', 'Grady', 'Hamza', 'Harrison', 'Hayden', 'Henry', 'Izaiah', 'Jaime', 'Jamarcus', 'Jamarion', 'James', 'Jamir', 'Jason', 'Jaxon', 'Jayden', 'Jaylen', 'Jaylin', 'Jesse', 'Jesus', 'Jett', 'Joey', 'Jonathon', 'Jorge', 'Joshua', 'Julio', 'Julius', 'Kale', 'Kamari', 'Kamron', 'Kane', 'Keaton', 'Kellen', 'Kendrick', 'Kody', 'Kristopher', 'Kylan', 'Landen', 'Lennon', 'Leonel', 'Lorenzo', 'Luke', 'Marcus', 'Mario', 'Mason', 'Mathew', 'Mathias', 'Matteo', 'Matthew', 'Max', 'Maximilian', 'Mike', 'Muhammad', 'Nick', 'Nigel', 'Niko', 'Nikolas', 'Patrick', 'Pierre', 'Prince', 'Quinn', 'Raymond', 'Remington', 'Roberto', 'Roger', 'Ronnie', 'Roy', 'Seth', 'Silas', 'Simon', 'Slade', 'Theodore', 'Tim', 'Tobias', 'Trevon', 'Tripp', 'Troy', 'Trystan', 'Willie', 'Wyatt', 'Yahir', 'Zackary', 'Zain'];

var randomLastName = ['Abbott', 'Acevedo', 'Acosta', 'Adams', 'Adkins', 'Aguilar', 'Aguirre', 'Alexander', 'Ali', 'Allen', 'Allison', 'Alvarado', 'Alvarez', 'Andersen', 'Anderson', 'Andrade', 'Andrews', 'Anthony', 'Archer', 'Arellano', 'Arias', 'Armstrong', 'Arnold', 'Arroyo', 'Ashley', 'Atkins', 'Atkinson', 'Austin', 'Avery', 'Avila', 'Ayala', 'Ayers', 'Bailey', 'Baird', 'Baker', 'Baldwin', 'Ball', 'Ballard', 'Banks', 'Barajas', 'Barber', 'Barker', 'Barnes', 'Barnett', 'Barr', 'Barrera', 'Barrett', 'Barron', 'Barry', 'Bartlett', 'Barton', 'Bass', 'Bates', 'Bauer', 'Bautista', 'Baxter', 'Bean', 'Beard', 'Beasley', 'Beck', 'Becker', 'Bell', 'Beltran', 'Bender', 'Benitez', 'Benjamin', 'Bennett', 'Benson', 'Bentley', 'Benton', 'Berg', 'Berger', 'Bernard', 'Berry', 'Best', 'Bird', 'Bishop', 'Black', 'Blackburn', 'Blackwell', 'Blair', 'Blake', 'Blanchard', 'Blankenship', 'Blevins', 'Bolton', 'Bond', 'Bonilla', 'Booker', 'Boone', 'Booth', 'Bowen', 'Bowers', 'Bowman', 'Boyd', 'Boyer', 'Boyle', 'Bradford', 'Bradley', 'Bradshaw', 'Brady', 'Branch', 'Brandt', 'Braun', 'Bray', 'Brennan', 'Brewer', 'Bridges', 'Briggs', 'Bright', 'Brock', 'Brooks', 'Brown', 'Browning', 'Bruce', 'Bryan', 'Bryant', 'Buchanan', 'Buck', 'Buckley', 'Bullock', 'Burch', 'Burgess', 'Burke', 'Burnett', 'Burns', 'Burton', 'Bush', 'Butler', 'Byrd', 'Cabrera', 'Cain', 'Calderon', 'Caldwell', 'Calhoun', 'Callahan', 'Camacho', 'Cameron', 'Campbell', 'Campos', 'Cannon', 'Cantrell', 'Cantu', 'Cardenas', 'Carey', 'Carlson', 'Carney', 'Carpenter', 'Carr', 'Carrillo', 'Carroll', 'Carson', 'Carter', 'Case', 'Casey', 'Castaneda', 'Castillo', 'Castro', 'Cervantes', 'Chambers', 'Chan', 'Chandler', 'Chaney', 'Chang', 'Chapman', 'Charles', 'Chase', 'Chavez', 'Chen', 'Cherry', 'Choi', 'Christensen', 'Christian', 'Chung', 'Church', 'Cisneros', 'Clark', 'Clarke', 'Clay', 'Clayton', 'Clements', 'Cline', 'Cobb', 'Cochran', 'Coffey', 'Cohen', 'Cole', 'Coleman', 'Collier', 'Collins', 'Colon', 'Combs', 'Compton', 'Conley', 'Conner', 'Conrad', 'Contreras', 'Conway', 'Cook', 'Cooke', 'Cooley', 'Cooper', 'Copeland', 'Cordova', 'Cortez', 'Costa', 'Cowan', 'Cox', 'Craig', 'Crane', 'Crawford', 'Crosby', 'Cross', 'Cruz', 'Cuevas', 'Cummings', 'Cunningham', 'Curry', 'Curtis', 'Dalton', 'Daniel', 'Daniels', 'Daugherty', 'Davenport', 'David', 'Davidson', 'Davies', 'Davila', 'Davis', 'Dawson', 'Day', 'Dean', 'Decker', 'Delacruz', 'Deleon', 'Delgado', 'Dennis', 'Diaz', 'Dickerson', 'Dickson', 'Dillon', 'Dixon', 'Dodson', 'Dominguez', 'Donaldson', 'Donovan', 'Dorsey', 'Dougherty', 'Douglas', 'Downs', 'Doyle', 'Drake', 'Duarte', 'Dudley', 'Duffy', 'Duke', 'Duncan', 'Dunlap', 'Dunn', 'Duran', 'Durham', 'Dyer', 'Eaton', 'Edwards', 'Elliott', 'Ellis', 'Ellison', 'English', 'Erickson', 'Escobar', 'Esparza', 'Espinoza', 'Estes', 'Estrada', 'Evans', 'Everett', 'Ewing', 'Farley', 'Farmer', 'Farrell', 'Faulkner', 'Ferguson', 'Fernandez', 'Ferrell', 'Fields', 'Figueroa', 'Finley', 'Fischer', 'Fisher', 'Fitzgerald', 'Fitzpatrick', 'Fleming', 'Fletcher', 'Flores', 'Flowers', 'Floyd', 'Flynn', 'Foley', 'Forbes', 'Ford', 'Foster', 'Fowler', 'Fox', 'Francis', 'Franco', 'Frank', 'Franklin', 'Frazier', 'Frederick', 'Freeman', 'French', 'Frey', 'Friedman', 'Fritz', 'Frost', 'Fry', 'Frye', 'Fuentes', 'Fuller', 'Gaines', 'Gallagher', 'Gallegos', 'Galloway', 'Galvan', 'Gamble', 'Garcia', 'Gardner', 'Garner', 'Garrett', 'Garrison', 'Garza', 'Gates', 'Gay', 'Gentry', 'George', 'Gibbs', 'Gibson', 'Gilbert', 'Giles', 'Gill', 'Gillespie', 'Gilmore', 'Glass', 'Glenn', 'Glover', 'Golden', 'Gomez', 'Gonzales', 'Gonzalez', 'Good', 'Goodman', 'Goodwin', 'Gordon', 'Gould', 'Graham', 'Grant', 'Graves', 'Gray', 'Green', 'Greene', 'Greer', 'Gregory', 'Griffin', 'Griffith', 'Grimes', 'Gross', 'Guerra', 'Guerrero', 'Gutierrez', 'Guzman', 'Haas', 'Hahn', 'Hale', 'Haley', 'Hall', 'Hamilton', 'Hammond', 'Hampton', 'Hancock', 'Haney', 'Hanna', 'Hansen', 'Hanson', 'Hardin', 'Harding', 'Hardy', 'Harmon', 'Harper', 'Harrell', 'Harrington', 'Harris', 'Harrison', 'Hart', 'Hartman', 'Harvey', 'Hatfield', 'Hawkins', 'Hayden', 'Hayes', 'Haynes', 'Hays', 'Heath', 'Hebert', 'Henderson', 'Hendricks', 'Hendrix', 'Henry', 'Hensley', 'Henson', 'Herman', 'Hernandez', 'Herrera', 'Herring', 'Hess', 'Hester', 'Hickman', 'Hicks', 'Higgins', 'Hill', 'Hines', 'Hinton', 'Ho', 'Hobbs', 'Hodge', 'Hodges', 'Hoffman', 'Hogan', 'Holden', 'Holder', 'Holland', 'Holloway', 'Holmes', 'Holt', 'Hood', 'Hooper', 'Hoover', 'Hopkins', 'Horn', 'Horne', 'Horton', 'House', 'Houston', 'Howard', 'Howe', 'Howell', 'Huang', 'Hubbard', 'Huber', 'Hudson', 'Huerta', 'Huff', 'Huffman', 'Hughes', 'Hull', 'Humphrey', 'Hunt', 'Hunter', 'Hurley', 'Hurst', 'Hutchinson', 'Huynh', 'Ibarra', 'Ingram', 'Irwin', 'Jackson', 'Jacobs', 'Jacobson', 'James', 'Jarvis', 'Jefferson', 'Jenkins', 'Jennings', 'Jensen', 'Jimenez', 'Johns', 'Johnston', 'Jones', 'Jordan', 'Joseph', 'Joyce', 'Juarez', 'Kaiser', 'Kane', 'Kaufman', 'Keith', 'Keller', 'Kelley', 'Kelly', 'Kemp', 'Kennedy', 'Kent', 'Kerr', 'Key', 'Khan', 'Kidd', 'Kim', 'King', 'Kirby', 'Kirk', 'Klein', 'Kline', 'Knapp', 'Knight', 'Knox', 'Koch', 'Kramer', 'Krause', 'Krueger', 'Lam', 'Lamb', 'Lambert', 'Landry', 'Lane', 'Lang', 'Lara', 'Larsen', 'Larson', 'Lawrence', 'Lawson', 'Le', 'Leach', 'Leblanc', 'Lee', 'Leon', 'Leonard', 'Lester', 'Levine', 'Levy', 'Lewis', 'Li', 'Lin', 'Lindsey', 'Little', 'Liu', 'Livingston', 'Lloyd', 'Logan', 'Long', 'Lopez', 'Love', 'Lowe', 'Lowery', 'Lozano', 'Lucas', 'Lucero', 'Luna', 'Lutz', 'Lynch', 'Lynn', 'Lyons', 'Macdonald', 'Macias', 'Mack', 'Madden', 'Maddox', 'Mahoney', 'Maldonado', 'Malone', 'Mann', 'Manning', 'Marks', 'Marquez', 'Marsh', 'Marshall', 'Martin', 'Martinez', 'Mason', 'Massey', 'Mata', 'Mathews', 'Mathis', 'Matthews', 'Maxwell', 'May', 'Mayer', 'Maynard', 'Mayo', 'Mays', 'Mcbride', 'Mccall', 'Mccann', 'Mccarthy', 'Mccarty', 'Mcclain', 'Mcclure', 'Mcconnell', 'Mccormick', 'Mccoy', 'Mccullough', 'Mcdaniel', 'Mcdonald', 'Mcdowell', 'Mcfarland', 'Mcgee', 'Mcgrath', 'Mcguire', 'Mcintosh', 'Mcintyre', 'Mckay', 'Mckee', 'Mckenzie', 'Mckinney', 'Mcknight', 'Mclaughlin', 'Mclean', 'Mcmahon', 'Mcmillan', 'Mcneil', 'Mcpherson', 'Meadows', 'Medina', 'Mejia', 'Melendez', 'Melton', 'Mendez', 'Mendoza', 'Mercado', 'Mercer', 'Merritt', 'Meyer', 'Meyers', 'Meza', 'Michael', 'Middleton', 'Miles', 'Miller', 'Mills', 'Miranda', 'Mitchell', 'Molina', 'Monroe', 'Montes', 'Montgomery', 'Montoya', 'Moody', 'Moon', 'Mooney', 'Moore', 'Mora', 'Morales', 'Moran', 'Moreno', 'Morgan', 'Morris', 'Morrison', 'Morrow', 'Morse', 'Morton', 'Moses', 'Mosley', 'Moss', 'Moyer', 'Mueller', 'Mullen', 'Mullins', 'Munoz', 'Murillo', 'Murphy', 'Murray', 'Myers', 'Nash', 'Navarro', 'Neal', 'Nelson', 'Newman', 'Newton', 'Nguyen', 'Nichols', 'Nicholson', 'Nielsen', 'Nixon', 'Noble', 'Nolan', 'Norman', 'Norris', 'Norton', 'Novak', 'Nunez', 'Obrien', 'Ochoa', 'Oconnell', 'Oconnor', 'Odom', 'Odonnell', 'Oliver', 'Olsen', 'Olson', 'Oneal', 'Oneill', 'Orozco', 'Orr', 'Ortega', 'Ortiz', 'Osborn', 'Osborne', 'Owen', 'Owens', 'Pace', 'Pacheco', 'Padilla', 'Page', 'Palmer', 'Park', 'Parker', 'Parks', 'Parrish', 'Parsons', 'Patel', 'Patrick', 'Patterson', 'Patton', 'Paul', 'Payne', 'Pearson', 'Peck', 'Pena', 'Pennington', 'Perez', 'Perkins', 'Perry', 'Peters', 'Petersen', 'Peterson', 'Petty', 'Pham', 'Phelps', 'Phillips', 'Pierce', 'Pineda', 'Pittman', 'Pitts', 'Pollard', 'Ponce', 'Poole', 'Pope', 'Porter', 'Potter', 'Potts', 'Powell', 'Powers', 'Pratt', 'Preston', 'Price', 'Prince', 'Proctor', 'Pruitt', 'Pugh', 'Quinn', 'Ramirez', 'Ramos', 'Ramsey', 'Randall', 'Randolph', 'Rangel', 'Rasmussen', 'Ray', 'Raymond', 'Reed', 'Reese', 'Reeves', 'Reid', 'Reilly', 'Reyes', 'Reynolds', 'Rhodes', 'Rice', 'Rich', 'Richard', 'Richards', 'Richardson', 'Richmond', 'Riddle', 'Riggs', 'Riley', 'Rios', 'Ritter', 'Rivas', 'Rivera', 'Rivers', 'Roach', 'Robbins', 'Roberson', 'Roberts', 'Robertson', 'Robinson', 'Robles', 'Rocha', 'Rodgers', 'Rodriguez', 'Rogers', 'Rojas', 'Rollins', 'Roman', 'Romero', 'Rosales', 'Rosario', 'Rose', 'Ross', 'Roth', 'Rowe', 'Rowland', 'Roy', 'Rubio', 'Ruiz', 'Rush', 'Russell', 'Russo', 'Ryan', 'Salas', 'Salazar', 'Salinas', 'Sampson', 'Sanchez', 'Sanders', 'Sandoval', 'Sanford', 'Santana', 'Santiago', 'Santos', 'Saunders', 'Savage', 'Sawyer', 'Schaefer', 'Schmidt', 'Schmitt', 'Schneider', 'Schroeder', 'Schultz', 'Schwartz', 'Scott', 'Sellers', 'Serrano', 'Sexton', 'Shaffer', 'Shah', 'Shannon', 'Sharp', 'Shaw', 'Shea', 'Shelton', 'Shepard', 'Shepherd', 'Sheppard', 'Sherman', 'Shields', 'Short', 'Silva', 'Simmons', 'Simon', 'Simpson', 'Sims', 'Singh', 'Singleton', 'Skinner', 'Sloan', 'Small', 'Smith', 'Snow', 'Snyder', 'Solis', 'Solomon', 'Sosa', 'Soto', 'Sparks', 'Spears', 'Spence', 'Spencer', 'Stafford', 'Stanley', 'Stanton', 'Stark', 'Steele', 'Stein', 'Stephens', 'Stephenson', 'Stevens', 'Stevenson', 'Stewart', 'Stokes', 'Stone', 'Stout', 'Strickland', 'Strong', 'Stuart', 'Suarez', 'Sullivan', 'Summers', 'Sutton', 'Swanson', 'Sweeney', 'Tanner', 'Tapia', 'Tate', 'Taylor', 'Terrell', 'Terry', 'Thomas', 'Thompson', 'Thornton', 'Todd', 'Torres', 'Townsend', 'Tran', 'Travis', 'Trevino', 'Trujillo', 'Tucker', 'Turner', 'Tyler', 'Underwood', 'Valdez', 'Valencia', 'Valentine', 'Valenzuela', 'Vance', 'Vang', 'Vargas', 'Vasquez', 'Vaughan', 'Vaughn', 'Vazquez', 'Vega', 'Velasquez', 'Velazquez', 'Velez', 'Villa', 'Villanueva', 'Villarreal', 'Villegas', 'Vincent', 'Wade', 'Wagner', 'Walker', 'Wall', 'Wallace', 'Waller', 'Walls', 'Walsh', 'Walter', 'Walters', 'Walton', 'Wang', 'Ward', 'Ware', 'Warner', 'Warren', 'Washington', 'Waters', 'Watkins', 'Watson', 'Watts', 'Weaver', 'Webb', 'Weber', 'Webster', 'Weeks', 'Weiss', 'Welch', 'Wells', 'Werner', 'West', 'Wheeler', 'Whitaker', 'White', 'Whitehead', 'Whitney', 'Wiggins', 'Wilcox', 'Wiley', 'Wilkerson', 'Wilkins', 'Wilkinson', 'Williams', 'Williamson', 'Willis', 'Wilson', 'Winters', 'Wise', 'Wolf', 'Wolfe', 'Wong', 'Wood', 'Woodard', 'Woods', 'Woodward', 'Wright', 'Wu', 'Wyatt', 'Yang', 'Yates', 'Yoder', 'York', 'Young', 'Yu', 'Zamora', 'Zavala', 'Zhang', 'Zimmerman', 'Zuniga'];