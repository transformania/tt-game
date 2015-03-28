var fashionApp = angular.module('fashionApp', ['ngSanitize', 'ngAnimate']);

logs = [];
leaderId = -1;
personIdNext = 0;
logsIdNext = 0;
itemIdNext = 0;


angular.module('fashionApp').controller('PersonsController', function ($scope, $sce) {

    $scope.page = "newPlayer";
    $scope.pageBack = "";
    $scope.randomEncounter = "";

    $scope.personsPlayer = new PersonGroup();
    $scope.personsPlayerBteam = new PersonGroup();
    $scope.personsAI = new PersonGroup();
    $scope.defeatedAI = new PersonGroup();
    $scope.defeatedPlayer = new PersonGroup();
    $scope.newItems = new ItemGroup();
    $scope.unequippedItems = new ItemGroup();
    $scope.turnsSinceCombatStart = 1;
    $scope.newfight_btn = true;
    $scope.sellItem_btn = true;
    $scope.buyItem_btn = true;
    $scope.buySellItemsView = false;
    $scope.chooseBattlegroundView = false;
    $scope.assignItemsView = false;
    $scope.showTeachSpells = false;
    $scope.defeatedView = false;
    $scope.logsView = true;

    $scope.equipNext = true;
    $scope.equipPrevious = true;

    $scope.itemsMap = itemStatsMap;
    $scope.locationsMap = locationsMap;

    $scope.leader;
    $scope.energy = 100;
    $scope.energyMax = 100;
    $scope.money = 100;
    $scope.timeOfDay = "Afternoon";
    $scope.day = 1;
    $scope.infamy = 0;

    $scope.currentlySelectedItem;
    $scope.currentlySelectedPerson;

    $scope.logs = [];

    $scope.fightInProgress = false;

    $scope.startVisible = true;

    $scope.continueFightVisible = false;
    $scope.startFightVisible = false;

    $scope.selectedAftermathPerson;


    $scope.fightSelectedPlayer;
    $scope.fightSelectedOpponent;
    $scope.fightTurn = "player";

    $scope.aftermathBtn = false;
    $scope.aftermathView = false;
    $scope.aftermathViewLevelup = false;


    $scope.fightView = false;

    var welcome = new Log("Welcome!");

    $scope.logs.push(welcome);

    // have player enter their name to kick off the game
    $scope.addPlayerAndStart = function (firstName, lastName) {
        var newbie = new Person(firstName, lastName, "Fashion Witch");
        leaderId = newbie.id;

        

        newbie.giveFinishingSpell('Cotton Panties');
        $scope.leader = newbie;

        $scope.personsPlayer.addPerson(newbie);

        var welcome = new Log("<span class='newcov'>" + newbie.fullName() + ", the young yet talented fashion witch, has decided to found her own covenant to best all her rivals and become the most powerful fashion witch in the world!  </span>");

        $scope.logs.push(welcome);

        $scope.page = "chooseBattleground";


    }

    $scope.chooseBattleground = function () {

        $scope.page = "chooseBattleground";

        $scope.aftermathBtn = false;
        $scope.timeOfDay = "Afternoon";
        $scope.defeatedPlayer.persons = [];
        $scope.defeatedAI.persons = [];
        $scope.personsAI.persons = [];
    }

    $scope.chooseBattlegroundSend = function (location, variation) {
        $scope.startVisible = false;
        $scope.fightView = true;

        $scope.continueFightVisible = false;
        $scope.startFightVisible = true;

        $scope.personsAI.spawnGroup(location, variation);
        $scope.fightTurn = "player";

        $scope.fightSelectedPlayer = $scope.personsPlayer.persons[0];
        $scope.fightSelectedOpponent = $scope.personsAI.persons[0];

        // launch fight page
        $scope.page = "fight";

    }

    // kick off a fight
    $scope.startFight = function () {

        $scope.defeatedAI.persons = [];
        $scope.defeatedPlayer.persons = [];
        $scope.fightTurn = "player"

        $scope.personsPlayer.resetFighterMarkers();

        $scope.turnsSinceCombatStart = 1;
        $scope.continueFightVisible = true;
        $scope.startFightVisible = false;
        $scope.retreatBtnVisible = false;
    }

   //  begin a round of fighting
    $scope.fight = function () {

        $scope.page = "fight";

        $scope.fightInProgress = true;

        var attacker;
        var attackerGroup;
        var victim;
        var victimGroup;
        var defeatedVictims;
       
        var fightResult;

        if ($scope.fightTurn == "player") {
            attacker = $scope.fightSelectedPlayer;
            attackerGroup = $scope.personsPlayer;

            victim = $scope.personsAI.getRandomPerson();

            victimGroup = $scope.personsAI;
            defeatedVictims = $scope.defeatedAI;
            $scope.fightTurn = "ai";
            var fightResult = attacker.fight(victim, true);
        } else {
            attacker = $scope.fightSelectedOpponent;
            attackerGroup = $scope.personsAI;

            victim = $scope.personsPlayer.getRandomPerson();

            victimGroup = $scope.personsPlayer;
            defeatedVictims = $scope.defeatedPlayer;
            $scope.fightTurn = "player";
            var fightResult = attacker.fight(victim, false);
        }

       

        if (fightResult.outcome == "win") {

            if (attacker.finishingSpells.length == 0 || attacker.id == leaderId) {
                fightResult.message += "<span class='submit'>" + victim.fullName() + ' falls to their knees and submits to their attacker!</span>';
                
                victimGroup.removePerson(victim);
                defeatedVictims.addPerson(victim);


                // target IS turned into an item, delete them and create the item
            } else {
                var newItemName = attacker.getRandomFinishingSpell();
                fightResult.message += "<span class='inanimated'>" + insertNames(getRandomDefeatText(this.type, newItemName),attacker,victim) + "</span>";

                if (newItemName == "Absorb") {
                    attacker.health += Math.floor(victim.getMaxHP() / 4);
                    if (attacker.health >attacker.getMaxHP()) {
                        attacker.health = attacker.getMaxHP();
                    }
                } else {
                    var newItem = new Item(newItemName, victim.fullName());
                    $scope.newItems.addItem(newItem);
                }
                victimGroup.removePerson($scope.fightSelectedOpponent);
            }
        }
            
        $scope.logs.push(fightResult);

        var freePlayerCombatants = $scope.personsPlayer.getAvailableFighterCount();
        var freeAICombatants = $scope.personsAI.getAvailableFighterCount();
    
        // --------
        if (freePlayerCombatants > 0) {
            $scope.fightSelectedPlayer = $scope.personsPlayer.getNextFighter();
        } else if (freePlayerCombatants == 0 && freeAICombatants > 0) {
            $scope.fightTurn = "ai";
        } else {
            $scope.personsPlayer.resetFighterMarkers();
            $scope.fightSelectedPlayer = $scope.personsPlayer.getNextFighter();
        }

        if (freeAICombatants > 0) {
            $scope.fightSelectedOpponent = $scope.personsAI.getNextFighter();
        } else if (freeAICombatants == 0 && freePlayerCombatants > 0) {
            $scope.fightTurn = "player";
        } else {
            $scope.personsAI.resetFighterMarkers();
            $scope.fightSelectedOpponent = $scope.personsPlayer.getNextFighter();
        }
       


        // give the option to retreat after the 3rd round
        //if ($scope.turnsSinceCombatStart >= 4) {
        //    $scope.retreatBtnVisible = true;
        //}

        // fight is over, one side or the other has lost!
        if ($scope.fightInProgress == true && ($scope.personsPlayer.persons.length == 0 || $scope.personsAI.persons.length == 0)) {
            $scope.continueFightVisible = false;
            $scope.retreatBtnVisible = false;
            $scope.aftermathBtn = true;
        }
    }

    $scope.retreat = function () {
        $scope.logs = [];
        var retreatMsg = new Log("Retreat!  Your covenants flee the field of battle in disarray, choosing to remain animate and fight another day.  At least those who still have feet anyway.");
        $scope.logs.push(retreatMsg);
        $scope.fightView = false;
        $scope.defeatedAI.persons = [];

        // launch aftermath page
        $scope.page = "aftermath";

    }

    $scope.aftermath = function () {

        // if victory carry on, if defeat then show end game screen
        $scope.timeOfDay = "Evening";
       
        $scope.logs = [];
        $scope.fightView = false;

        if ($scope.personsPlayer.persons.indexOf($scope.leader) < 0) {
            // launch defeated page
            $scope.page = "defeat";
        } else {
            // launch aftermath page
            $scope.page = "aftermath";
            $scope.selectedAftermathPerson = $scope.defeatedAI.persons[0];
        }
    }

    $scope.changeSelectedPerson = function (newPerson) {
        $scope.selectedAftermathPerson = newPerson;
    }

    $scope.viewUpgrades = function () {
        // launch promotions page
        $scope.page = "selfUpgrade";

    }

    $scope.upgradeSend = function (upgrade, cost) {
        $scope.leader.xp -= cost;

        if (upgrade == 'selfHeal') {
            $scope.leader.heal(99999);
        }
        else if (upgrade == 'energyPlus01') {
            $scope.energyMax += 15;
        }

    }

    $scope.recruit = function (person) {
        person.resetXP();
        $scope.personsPlayer.addPerson(person);
        $scope.defeatedAI.removePerson(person);
        $scope.energy -= person.getRecruitmentCost();


        var newlog = new Log(person.fullName() + " agrees to join your covenant for a life of adventure, danger, and sexy clothes made out rivals!");
        $scope.logs.push(newlog);


        $scope.selectedAftermathPerson = $scope.defeatedAI.persons[0];

    }

    $scope.inanimate = function (person, spell) {
        $scope.infamy += 2;
        $scope.defeatedAI.removePerson(person);
        var newItem = new Item(spell, person.fullName());
        $scope.newItems.addItem(newItem);
        $scope.energy -= person.getTransformCost(spell);

        var result = "<span class='inanimated'>" + insertNames(getRandomDefeatText(person.type, newItem.type), $scope.leader, person) + "</span>";

        var newlog = new Log(result);
        $scope.logs.push(newlog);

        $scope.selectedAftermathPerson = $scope.defeatedAI.persons[0];

    }


    $scope.absorb = function (person) {
        $scope.leader.health += Math.floor(person.getMaxHP() / 4);
        $scope.defeatedAI.removePerson(person);

        var result = "<span class='absorb'>" + insertNames(getRandomDefeatText(person.type, 'Absorb'), $scope.leader, person) + "</span>";
        var newlog = new Log(result);

        $scope.logs.push(newlog);

        $scope.selectedAftermathPerson = $scope.defeatedAI.persons[0];
    }

    $scope.amnesia = function (person) {
        $scope.infamy += 1;
        $scope.defeatedAI.removePerson(person);
        $scope.energy -= person.getBrainwashCost();
        var result = "<span class='brainwashed'>" + insertNames(getRandomDefeatText(person.type, 'brainwash'), $scope.leader, person) + "</span>";
        var newlog = new Log(result);
        $scope.logs.push(newlog);

        $scope.selectedAftermathPerson = $scope.defeatedAI.persons[0];
    }


    $scope.aftermathLevelupShow = function () {



        $scope.infamy += $scope.defeatedAI.persons.length * 5;
        $scope.personsPlayer.mergeIntoGroup($scope.defeatedPlayer);
        $scope.unequippedItems.mergeIntoGroup($scope.newItems);

        // launch promotions page
        $scope.page = "promotions";

    }

    $scope.showPromotions = function () {
        // launch promotions page
        $scope.page = "promotions";
    }

    $scope.promote = function (person, nextForm) {
        var result = insertNames(getRandomDefeatText(person.type, nextForm), $scope.leader, person);
        var newlog = new Log(result);
        $scope.logs.push(newlog);
        person.changeType(nextForm);
    }

    $scope.endDay = function () {

        $scope.timeOfDay = "Morning";
        $scope.logs = [];
        $scope.personsPlayer.healGroup(15);
        $scope.energy = $scope.energyMax;
        $scope.day++;

        $scope.personsPlayer.resetFighterMarkers();

        var roll = Math.random();

        if (roll < .5) {
            // launch random encounter page
            $scope.page = "randomEncounter";
        } else {
            // launch buy/sell page
            $scope.page = "buySell";
        }
    }

    $scope.randomEncounterContinue = function () {
        // launch buy/sell page
        $scope.page = "buySell";
    }

    $scope.showBuySell = function () {
        // launch buy/sell page
        $scope.page = "buySell";
    }

    $scope.sellItem = function (item) {
        $scope.unequippedItems.removeItem(item);
        $scope.money += Math.floor(item.getSellPrice());
        //        var newlog = new Log("You sold " + item.getVictimName() + " for " + item.getBasePrice() + " dollars.  May they find a wonderful owner who will treasure them!");
        // needs itemType
        var newlog = new Log("You sold " + item.getVictimName() + " for " + item.getSellPrice() + " dollars.  May they find a wonderful owner who will treasure them!");
        $scope.logs.push(newlog);
    }

    $scope.buyItem = function (itemType) {
        var itemInfo = itemStatsMap[itemType];
        $scope.money -= itemInfo.value;
        var newItem = new Item(itemType, '');
        $scope.unequippedItems.addItem(newItem);
        var newlog = new Log("You purchased 1 " + itemType + " for the covenant for " + itemInfo.value + " dollars.");
        $scope.logs.push(newlog);
    }

    $scope.showEquipMembersView = function () {

        // launch promotions page
        $scope.page = "assign";

        $scope.currentlySelectedItem = $scope.unequippedItems.items[0];
    }

    $scope.showPartyStatus = function () {
        // launch promotions page
        $scope.page = "partyStatus";
    }

    $scope.equipSelectedItem = function (newItem) {
        $scope.currentlySelectedItem = newItem;
    }

    /*
    $scope.equipNext = function () {
        var index = $scope.unequippedItems.items.indexOf($scope.currentlySelectedItem);
        index++;
        $scope.currentlySelectedItem = $scope.unequippedItems.items[index];

        if ($scope.currentlySelectedItem === undefined) {
            $scope.currentlySelectedItem = $scope.unequippedItems.items[0];
        }
    }

    $scope.equipPrevious = function (item) {
        var index = $scope.unequippedItems.items.indexOf($scope.currentlySelectedItem);
        index--;
        $scope.currentlySelectedItem = $scope.unequippedItems.items[index];

        if ($scope.currentlySelectedItem === undefined) {
            $scope.currentlySelectedItem = $scope.unequippedItems.items[$scope.unequippedItems.items.length - 1];
        }
    }*/

    $scope.assignTo = function (person, item) {
        person.giveItem(item);
        $scope.unequippedItems.removeItem(item);
        $scope.currentlySelectedItem = $scope.unequippedItems.items[0];
    }

    $scope.strip = function (person) {
        var numberEquipped = person.items.length;
        for (var i = 0; i < numberEquipped; i++) {
            var item = person.items[0];
            person.removeItem(item);
            $scope.unequippedItems.addItem(item);
        }
        $scope.currentlySelectedItem = $scope.unequippedItems.items[0];
    }

    $scope.moveToBTeam = function (person, toB) {
        if (toB == true) {
            $scope.personsPlayer.removePerson(person);
            $scope.personsPlayerBteam.addPerson(person);
        } else {
            $scope.personsPlayerBteam.removePerson(person);
            $scope.personsPlayer.addPerson(person);
        }
       
    }

    $scope.showTeachSpells = function () {
        
        // launch promotions page
        $scope.page = "teach";

        $scope.currentlySelectedPerson = $scope.personsPlayer.persons[0];
        $scope.logs = [];
    }

    $scope.teachSelectedPerson = function (newPerson) {
        $scope.currentlySelectedPerson = newPerson;
    }


    $scope.teachSpell = function (itemName) {
        var spellInfo = itemStatsMap[itemName];
        $scope.money -= spellInfo.spellTrainingCost;
        $scope.currentlySelectedPerson.giveFinishingSpell(itemName);
    }

    $scope.nextFight = function () {

        $scope.timeOfDay = "Afternoon";
        $scope.startFightVisible = true;
        $scope.aftermathBtn = false;
        $scope.assignItemsView = false;

        $scope.personsPlayer.healGroup(15);
        $scope.defeatedAI.persons = [];
        $scope.defeatedPlayer.persons = [];

    }

    $scope.getEnergyPercentage = function () {
        return ($scope.energy / $scope.energyMax) * 100;
    }

    $scope.showItemDetail = function (itemType) {
        // launch item detail page
        $scope.pageBack = $scope.page;
        $scope.page = "itemDetail";
        $scope.detailedItemSelection = itemStatsMap[itemType];
    }

    $scope.showFormDetail = function (formType) {
        // launch form detail page
        $scope.pageBack = $scope.page;
        $scope.page = "formDetail";
        $scope.detailedFormSelection = formStatsMap[formType];
    }

    $scope.detailBack = function () {
        // launch promotions page
        $scope.page = $scope.pageBack;
    }

    ////////////////////////// DEBUG /////////////////////////

    $scope.debug_fullXP = function () {
        for (var i = 0; i < $scope.personsPlayer.persons.length; i++) {
            $scope.personsPlayer.persons[i].xp = $scope.personsPlayer.persons[i].xpNeededForLevelup();
        }
    }

    $scope.debug_fullEnergy = function () {
        $scope.energy = $scope.energyMax;
    }


    $scope.debug_moreMoney = function () {
        $scope.money += 1000;
    }

    $scope.debug_spawnMeMembers = function () {
        $scope.personsPlayer.spawnGroup('Park:  Parking Lot', 0);
    }
});