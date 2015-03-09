function PersonsController($scope) {

    $scope.personsPlayer = new PersonGroup();
    $scope.personsAI = new PersonGroup();
    $scope.defeatedAI = new PersonGroup();
    $scope.defeatedPlayer = new PersonGroup();
    $scope.newItems = new ItemGroup();
    $scope.unequippedItems = new ItemGroup();
    $scope.turnsSinceCombatStart = 1;

    $scope.logs = [];

    $scope.fightInProgress = false;

    $scope.startVisible = true;

    $scope.continueFightVisible = false;
    $scope.startFightVisible = false;

    $scope.fightAftermathVisible = false;

    $scope.mainGame = false;

    $scope.personsAI.spawnGroup('bystanders_easy');

     var welcome = new Log("Welcome!");

     $scope.logs.push(welcome);

    // have player enter their name to kick off the game
     $scope.addPlayerAndStart = function (firstName, lastName) {
         var newbie = new Person(firstName, lastName, "Fashion Witch");

         // newbie.giveFinishingSpell('Cotton Panties');

         $scope.personsPlayer.addPerson(newbie);

         var welcome = new Log(newbie.fullName() + " <b>has</b> founded a new covenant!");

         $scope.logs.push(welcome);

         $scope.startVisible = false;
         $scope.mainGame = true;

         $scope.continueFightVisible = false;
         $scope.startFightVisible = true;

     }

    // kick off a fight
     $scope.startFight = function () {
         $scope.turnsSinceCombatStart = 1;
         $scope.continueFightVisible = true;
         $scope.startFightVisible = false;
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

        if ($scope.fightInProgress == true && ($scope.personsPlayer.persons.length == 0 || $scope.personsAI.persons.length == 0)) {
            $scope.continueFightVisible = false;
            $scope.fightAftermathVisible = true;
        }
    }

    $scope.retreat = function () {

    }

    $scope.aftermath = function () {
        $scope.mainGame = false;
    }

   



}