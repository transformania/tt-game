function PersonsController($scope) {

    $scope.personsPlayer = new PersonGroup();
    $scope.personsAI = new PersonGroup();
    $scope.defeatedAI = new PersonGroup();
    $scope.defeatedPlayer = new PersonGroup();
    $scope.newItems = new ItemGroup();
    $scope.unequippedItems = new ItemGroup();

    $scope.logs = [];

    $scope.fightInProgress = false;

    $scope.startVisible = true;
    $scope.fightVisible = false;

    $scope.personsAI.spawnGroup('bystanders_easy');
  
   // var person11 = new Person();
    //var person22 = new Person(undefined, undefined, 'Fashion Witch');
  //  var person33 = new Person();
  //  var person55 = new Person();
  //  var person66 = new Person();

   // console.log($scope.newItems);

   // $scope.personsAI.addPerson(person11);
   //  $scope.personsAI.addPerson(person22);
   //  $scope.personsAI.addPerson(person33);
    // $scope.personsAI.addPerson(person44);
    // $scope.personsAI.addPerson(person55);
   //  $scope.personsAI.addPerson(person66);

     var welcome = new Log("Welcome!");

    

     $scope.logs.push(welcome);

     var test = getRandomAttackText('Fashion Witch');

    $scope.fight = function () {

        $scope.logs = [];
        $scope.fightInProgress = true;
        var fightMsg = new Log("Fight started!");
        $scope.logs.push(fightMsg);

        var newLogs = $scope.personsPlayer.fightGroup($scope.personsAI, $scope.defeatedAI, $scope.defeatedPlayer, $scope.newItems);

        for (var i = 0; i < newLogs.length; i++) {
            $scope.logs.push(newLogs[i]);
        }

        if ($scope.fightInProgress == true && ($scope.personsPlayer.persons.length == 0 || $scope.personsAI.persons.length == 0)) {
            $scope.fightVisible = false;
        }



    }

    $scope.addPlayerAndStart = function (firstName, lastName) {
        var newbie = new Person(firstName, lastName, "Fashion Witch");
        $scope.personsPlayer.addPerson(newbie);

        var welcome = new Log(newbie.fullName() + " has founded a new covenant!");

        $scope.logs.push(welcome);

        $scope.startVisible = false;
        $scope.fightVisible = true;
    }

    $scope.retreat = function () {

    }

}