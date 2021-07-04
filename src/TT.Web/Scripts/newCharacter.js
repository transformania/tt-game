var startingForms = {};

$(document).ready(function () {

    $.ajax({
        url: "/Info/StartingForms"
    }).done(function (response) {
        startingForms = shuffle(response);

        $("#FormSourceId").val(startingForms[0].Id);

        var selectionsBox = $('#selections');

        // load form dropdown
        for (var i = 0; i < startingForms.length; i++) {
            var form = startingForms[i];
            selectionsBox.append("<img src='https://images.transformaniatime.com/portraits/Thumbnails/100/" + form.PortraitUrl + "' class='selectable' onclick='setForm(\"" + form.Id + "\")'></img>");
        }


        if ($("#oldFirstName").val() !== "") {
            $("#FirstName").val($("#oldFirstName").val());
            $("#LastName").val($("#oldLastName").val());
            $("#FormSourceId").val($("#oldFormSourceId").val());
        } else {
            randomize();
        }

        renderPortrait();

    });

    $('#itemBox').change(function () {
        if ($(this).is(':checked')) {
            $('#itemForm').prop('disabled', false);
            $('#itemForm').prop('class', 'enable');
        } else {
            $('#itemForm').prop('disabled', 'disabled');
            $('#itemForm').prop('class', 'disabled');
        }
    });

    $("#randomize").click(function () {
        randomize();
        renderPortrait();
    });

    $("#FormSourceId").change(function () {
        renderPortrait();
    });



    $("#MigrateLetters").prop('checked', true);
});

function setForm(formSourceId) {
    $("#FormSourceId").val(formSourceId);
    renderPortrait();
}


function renderPortrait() {
    var x = $("#FormSourceId").val();

    var form = startingForms.find(function (element) {
        return element.Id == x;
    });

    $("#portrait").attr("src", "https://images.transformaniatime.com/portraits/" + form.PortraitUrl);

}

function shuffle(array) {
    var newArray = [];
    while (array.length > 0) {
        var index = Math.floor(Math.random() * array.length);
        newArray.push(array[index]);
        array.splice(index, 1);

    }
    return newArray;
}

function randomize() {

    var gender;
    setForm(startingForms[Math.floor(Math.random() * startingForms.length)].Id);

    var form = startingForms.find(function (element) {
        return element.Id == $("#FormSourceId").val();
    });

    if (form.FriendlyName == "Regular Girl") {
        gender = "female";
    } else {
        gender = "male";
    }

    if (gender === "male") {
        var randMale = Math.floor(Math.random() * namesBox.maleNames.length);
        $("#FirstName").val(namesBox.maleNames[randMale]);
    } else {
        var randFemale = Math.floor(Math.random() * namesBox.femaleNames.length);
        $("#FirstName").val(namesBox.femaleNames[randFemale]);
    }

    var randLast = Math.floor(Math.random() * namesBox.lastNames.length);
    $("#LastName").val(namesBox.lastNames[randLast]);
}