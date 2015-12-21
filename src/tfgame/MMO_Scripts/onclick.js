$(document).ready(function () {

    $("#PersonalLog").click(function () {
        updatePersonalLog();
    });

    $("#SaveToDb").click(function () {
        AjaxSaveMeToDatabase();
    });

    $("#SceneName").click(function (event) {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/LookAtScene/',
                // data: { scenename: $("#SceneName").html() },
                type: 'POST',
                success: LookAtSceneSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_N").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_N_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_NW").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_NW_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_E").click(function () {

         if (waitingForPersonalLog == false) {
        $.ajax({
            url: 'TFWorld/Move/',
            data: { direction: $("#Connection_E_Hidden").html() },
            type: 'POST',
            success: moveSuccess,
            dataType: 'text'
        });
          }

    });

    $("#Connection_SE").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_SE_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_S").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_S_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_SW").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_SW_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_W").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_W_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#Connection_NW").click(function () {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/Move/',
                data: { direction: $("#Connection_NW_Hidden").html() },
                type: 'POST',
                success: moveSuccess,
                dataType: 'text'
            });
        }
    });

    $("#People").on("click", ".PersonListing", function (event) {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/CharacterQuery/',
                data: { character: $(this).find('.PersonListingName').html() },
                type: 'POST',
                success: characterQuerySuccess,
                dataType: 'text'
            });
        }
    });

    $("#Portrait").click(function (event) {
        if (waitingForPersonalLog == false) {
            $.ajax({
                url: 'TFWorld/SelfQuery/',
                type: 'POST',
                success: selfQuerySuccess,
                dataType: 'text'
            });
        }
    });

    // when player hits enter on keyboard, submit chat
    $("#ChatBox").keydown(function (event) {
        if (event.which == 13) {
            AjaxSendChat();
            event.preventDefault();
        }
    });

});