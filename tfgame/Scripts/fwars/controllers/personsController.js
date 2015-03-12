function PersonsController($scope) {

    $scope.personsPlayer = new PersonGroup();
    $scope.personsAI = new PersonGroup();
    $scope.defeatedAI = new PersonGroup();
    $scope.defeatedPlayer = new PersonGroup();
    $scope.newItems = new ItemGroup();
    $scope.unequippedItems = new ItemGroup();
    $scope.turnsSinceCombatStart = 1;
    $scope.newfight_btn = true;
    $scope.endDay_btn = true;
    $scope.sellItem_btn = true;
    $scope.buyItem_btn = true;
    $scope.buySellItemsView = false;
    $scope.chooseBattlegroundView = false;

    $scope.itemsMap = itemStatsMap;

    $scope.leader;
    $scope.energy = 100;
    $scope.energyMax = 100;
    $scope.money = 0;
    $scope.day = 1;

    $scope.logs = [];

    $scope.fightInProgress = false;

    $scope.startVisible = true;

    $scope.continueFightVisible = false;
    $scope.startFightVisible = false;

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
         newbie.giveFinishingSpell('Latex Catsuit');
         $scope.leader = newbie;

         $scope.personsPlayer.addPerson(newbie);

         var welcome = new Log(newbie.fullName() + " <b>has</b> founded a new covenant!");

         $scope.logs.push(welcome);

         $scope.chooseBattlegroundView = true;

     }

     $scope.chooseBattleground = function () {
         $scope.chooseBattlegroundView = true;
         $scope.buySellItemsView = false;
     }

     $scope.chooseBattlegroundSend = function (location, variation) {
         $scope.startVisible = false;
         $scope.fightView = true;

         $scope.continueFightVisible = false;
         $scope.startFightVisible = true;

         $scope.chooseBattlegroundView = false;
         $scope.personsAI.spawnGroup(location, variation);

     }

    // kick off a fight
     $scope.startFight = function () {

         $scope.defeatedAI.persons = [];
         $scope.defeatedPlayer.persons = [];

         $scope.turnsSinceCombatStart = 1;
         $scope.continueFightVisible = true;
         $scope.startFightVisible = false;
         $scope.retreatBtnVisible = false;
     }

    // begin a round of fighting
    $scope.fight = function () {

        $scope.logs = [];
        $scope.fightInProgress = true;
        var fightMsg = new Log("Fight started!");
        $scope.logs.push(fightMsg);

        var newLogs = $scope.personsPlayer.fightGroup($scope.personsAI, $scope.defeatedAI, $scope.defeatedPlayer, $scope.newItems);
        $scope.turnsSinceCombatStart++;

        for (var i = 0; i < newLogs.length; i++) {
            $scope.logs.push(newLogs[i]);
           
        }

        // give the option to retreat after the 3rd round
        if ($scope.turnsSinceCombatStart >= 4) {
            $scope.retreatBtnVisible = true;
        }

        // fight is over, one side or the other has lost!
        if ($scope.fightInProgress == true && ($scope.personsPlayer.persons.length == 0 || $scope.personsAI.persons.length == 0)) {
            $scope.continueFightVisible = false;
            $scope.retreatBtnVisible = false;
            $scope.aftermathBtn = true;
        }
    }

    $scope.retreat = function () {
        $scope.logs = [];
        var retreatMsg = new Log("Retreat!  The aggressors flee the field of battle in disarray, choosing to remain animate and fight another day.");
        $scope.logs.push(retreatMsg);
        $scope.fightView = false;
        $scope.aftermathView = true;
        $scope.aftermathViewLevelup_btn = true;
        $scope.defeatedAI.persons = [];
    }

    $scope.aftermath = function () {

        // if victory carry on, if defeat then show end game screen

        $scope.energy = $scope.energyMax;
        $scope.logs = [];
        $scope.fightView = false;
        $scope.aftermathView = true;
        $scope.aftermathViewLevelup_btn = true;
    }

    $scope.recruit = function (person) {
        person.resetXP();
        $scope.personsPlayer.addPerson(person);
        $scope.defeatedAI.removePerson(person);
        $scope.energy -= person.getRecruitmentCost();

        var newlog = new Log(person.fullName() + " agrees to join your covenant for a life of adventure, danger, and sexy clothes made out rivals!");
        $scope.logs.push(newlog);
    }
   
    $scope.inanimate = function (person, spell) {
        $scope.defeatedAI.removePerson(person);
        var newItem = new Item(spell, person.fullName());
        $scope.newItems.addItem(newItem);
        $scope.energy -= person.getTransformCost(spell);
        var newlog = new Log(person.fullName() + " " + getRandomDefeatText(newItem.type));
        $scope.logs.push(newlog);
    }


    $scope.aftermathLevelupShow = function () {
        $scope.aftermathViewLevelup = true;
        $scope.aftermathView = false;
        $scope.personsPlayer.mergeIntoGroup($scope.defeatedPlayer);
        $scope.unequippedItems.mergeIntoGroup($scope.newItems);
       
    }

    $scope.promote = function (person, nextForm) {
        var newlog = new Log(person.fullName() + " " + getRandomDefeatText(nextForm));
        $scope.logs.push(newlog);
        person.changeType(nextForm);
    }

    $scope.endDay = function () {
        $scope.aftermathViewLevelup = false;
        $scope.buySellItemsView = true;
        $scope.logs = [];
        $scope.personsPlayer.healGroup(15);
        $scope.day++;
    }

    $scope.sellItem = function (item) {
        $scope.unequippedItems.removeItem(item);
        $scope.money += Math.floor(item.getSellPrice());
        var newlog = new Log("You sold " + item.getVictimName() + " for " + item.getBasePrice() + " dollars.  May they find a wonderful owner who will treasure them!");
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

    $scope.nextFight = function () {
       
     //   $scope.personsAI.spawnGroup('bystanders_easy');
        $scope.startFightVisible = true;
        $scope.aftermathView = false;
        $scope.fightView = true;
        $scope.aftermathBtn = false;
        
        $scope.personsPlayer.healGroup(15);
        $scope.defeatedAI.persons = [];
        $scope.defeatedPlayer.persons = [];
       
    }




}