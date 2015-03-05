function PersonsController($scope) {

    //$scope.persons = [
    //    { name: 'Dave', city: 'Memphis' }, { name: 'Sam', city: 'Troy' }, { name: 'Elijah', city: 'Hamburg' }

    //];

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

    $scope.personsPlayer = p1;

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


    $scope.personsAI = p2;

}