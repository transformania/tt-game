var startingForms = {};

$(document).ready(function () {

    $.ajax({
        url: "/Info/StartingForms"
    }).done(function (response) {
        startingForms = shuffle(response);

        $("#FormName").val(startingForms[0].dbName);

        var selectionsBox = $('#selections');

        // load form dropdown
        for (var i = 0; i < startingForms.length; i++) {
            var form = startingForms[i];
            selectionsBox.append("<img src='../Images/PvP/portraits/Thumbnails/100/" + form.PortraitUrl + "' class='selectable' onclick='setForm(\"" + form.dbName + "\")'></img>");
        }


        if ($("#oldFirstName").val() !== "") {
            $("#FirstName").val($("#oldFirstName").val());
            $("#LastName").val($("#oldLastName").val());
            $("#FormName").val($("#oldForm").val());
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

    $("#FormName").change(function () {
        renderPortrait();
    });



    $("#MigrateLetters").prop('checked', true);
});

function setForm(formName) {
    $("#FormName").val(formName);
    renderPortrait();
}


function renderPortrait() {
    var x = $("#FormName").val();

    var form = startingForms.find(function (element) {
        return element.dbName === x;
    });

    $("#portrait").attr("src", "../Images/PvP/portraits/" + form.PortraitUrl);

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
    setForm(startingForms[Math.floor(Math.random() * startingForms.length)].dbName);

    if ($("#FormName").val().indexOf("woman_") !== -1) {
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