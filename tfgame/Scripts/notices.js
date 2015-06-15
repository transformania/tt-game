var attackPulse = 0;


$(document).ready(function () {

    if ($("#notificationBox").html().length > 10) {
        $("#notificationBox").show();
        $("#dismissNotficationBox").show();
    }
    else {
        $("#notificationBox").hide();
        $("#dismissNotficationBox").hide();
    }

    if (donatorTier >= 0) {

        $(function () {
            var link = $.connection.noticeHub;
            link.client.receiveNotice = function (data) {

                //alert(data);
                //   console.log(data);

                var notice = JSON.parse(data);

                $("#notificationBox").show();
                $("#dismissNotficationBox").show();
                $("#notificationBox").prepend(notice.jMessage + "</br></br>");

                if (notice.type == "attack") {
                    var wpWidth = notice.wp / notice.maxwp * 100;
                    var mnWidth = notice.mana / notice.maxmana * 100;
                    $("#wp_amt1").css("width", wpWidth + "%");
                    $("#mn_amt1").css("width", mnWidth + "%");
                    $("#wp_amt2").css("width", wpWidth + "%");
                    $("#mn_amt2").css("width", mnWidth + "%");


                    $("#wp_num1").html(notice.wp);
                    $("#wp_num2").html(notice.wp);

                    // $("body").css("background-color", "darkred");
                    attackPulse = 1;
                    backgroundPulse();



                }


            };


            $.connection.hub.start().done(function () {
                link.server.connect();

                $("#liveConnectionNotice").addClass("noticeOn");
                $("#liveConnectionNotice").removeClass("noticeOff");

            });

            $.connection.hub.disconnected(function () {
                $("#liveConnectionNotice").addClass("noticeOff");
                $("#liveConnectionNotice").removeClass("noticeOn");
            });

            // load whether or not to play audio notifications
            var playUpdateSoundLoad = JSON.parse(localStorage.getItem("play_updateSoundEnabled"));
            if (playUpdateSoundLoad == undefined) {
                localStorage.setItem("chat_IgnoreList", "false");
            } else if (localStorage.getItem("play_updateSoundEnabled") == "true") {
                playUpdateSound = true;
            }

            var attackedAudio = new Audio('../Sounds/attack.wav');
            audio.play();

        });
    }

});

function backgroundPulse() {
    if (attackPulse == 1) {
        $("body").animate({ backgroundColor: "red" }, 1000).animate({ backgroundColor: "black" }, 1000, backgroundPulse);
    }
}

function dismissNotificationsSuccess() {
    attackPulse = 0;
    $("#notificationBox").html("");
    $("#notificationBox").hide();
    $("#dismissNotficationBox").hide();
}

function dismissNotificationsFail() {
    alert("Something went wrong.  Unable to dismiss notifications.");
}