function PersonsController($scope) {

    $scope.personsPlayer = new PersonGroup();
    $scope.personsAI = new PersonGroup();

    $scope.startVisible = true;
    $scope.fightVisible = false;
  
    var person11 = new Person('Leah', 'Logarts');
 //   person11.levelUp();
  //  person11.levelUp();
    var person22 = new Person('Gary', 'Fadi');
    var person33 = new Person('Goliath', 'Bads');
  //  var person44 = new Person('Sassa', 'Fraut', 'Witchling');
    var person55 = new Person('Berrie', 'Vasta');
    var person66 = new Person('Holas', 'Shoes');
  //  person22.levelUp();
  //  person22.levelUp();
   // person22.levelUp();

    var item1 = new Item('Cotton Panties');
    var item2 = new Item('Cotton Panties');

    person11.giveItem(item1);
    person11.giveItem(item2);

    $scope.personsAI.addPerson(person11);
     $scope.personsAI.addPerson(person22);
     $scope.personsAI.addPerson(person33);
    // $scope.personsAI.addPerson(person44);
     $scope.personsAI.addPerson(person55);
     $scope.personsAI.addPerson(person66);

     var welcome = new Log("Welcome!");

     $scope.logs = [];

     $scope.logs.push(welcome);

     var test = getRandomAttackText('Fashion Witch');
     console.log(test);


    $scope.fight = function () {

        var fightMsg = new Log("Fight started!");
        $scope.logs.push(fightMsg);

        var newLogs = $scope.personsPlayer.fightGroup($scope.personsAI);

        for (var i = 0; i < newLogs.length; i++) {
            $scope.logs.push(newLogs[i]);
        }

    }

    $scope.addPlayerAndStart = function (firstName, lastName) {
        // var newbie = new Person(firstName, lastName, "Fashion Witch");
        var newbie = new Person(firstName, lastName, "Fashion Witch");
        $scope.personsPlayer.addPerson(newbie);

        var welcome = new Log(newbie.fullName() + " has founded a new covenant!");

        $scope.logs.push(welcome);

        $scope.startVisible = false;
        $scope.fightVisible = true;
    }



}