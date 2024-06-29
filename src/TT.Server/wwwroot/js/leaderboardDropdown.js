var roundData;

$(document).ready(function () {

    $.ajax({
        url: "/Scripts/pastRounds.json",
    }).done(function (data) {
        roundData = data;
        buildDropdown();
        renderButtons();
    });
});

function buildDropdown() {
    var dropdown = $("#list");
    for (var i = 0; i < roundData.rounds.length; i++) {
        var round = roundData.rounds[i];
        dropdown.append($("<option />").val(round.number).text(round.name));
    }

    if (selectedRound) {
        dropdown.val(selectedRound)
    }
}

function renderButtons() {

    var roundNumber = parseInt($("#list").val());

    var round = roundData.rounds.find(function (round) {
        return round.number === roundNumber;
    });

    if (round.pvp === true) {
        $("#pvp").show();
        $("#pvp").click(function () {
            window.location.href = "/Leaderboard/OldLeaderboards?round=" + round.number;
        });
    } else {
        $("#pvp").hide();
    }

    if (round.xp === true) {
        $("#xp").show();
        $("#xp").click(function () {
            window.location.href = "/Leaderboard/OldLeaderboards_XP?round=" + round.number;
        });
    } else {
        $("#xp").hide();
    }

    if (round.item === true) {
        $("#item").show();
        $("#item").click(function () {
            window.location.href = "/Leaderboard/OldLeaderboards_Item?round=" + round.number;
        });
    } else {
        $("#item").hide();
    }

    if (round.achievement === true) {
        $("#achievements").show();
        $("#achievements").click(function () {
            window.location.href = "/Leaderboard/OldLeaderboards_Achievements?round=" + round.number;
        });
    } else {
        $("#achievements").hide();
    }

}