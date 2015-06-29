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

                var notice = JSON.parse(data);

                $("#notificationBox").show();
                $("#dismissNotficationBox").show();
                $("#notificationBox").prepend(notice.jMessage + "</br></br>");

                if (notificationsEnabled == true) {
                    var message = notice.jMessage;
                    message = message.replace(/<(?:.|\n)*?>/gm, '');
                    sendAlertBox(message);
                }
               

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

                    if (playUpdateSound == true) {
                        var attackedAudio = new Audio('../Sounds/attack.wav');
                        attackedAudio.play();
                    }
                    

                } else if (notice.type == "message") {
                    if (playUpdateSound == true) {
                        var attackedAudio = new Audio('../Sounds/paper.wav');
                        attackedAudio.play();
                    }
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
            var playUpdateSoundLoad = localStorage.getItem("play_updateSoundEnabled");
            if (playUpdateSoundLoad == undefined) {
                localStorage.setItem("chat_IgnoreList", "false");
            } else if (localStorage.getItem("play_updateSoundEnabled") == "true") {
                playUpdateSound = true;
            }

            // load whether or not to use html5 notifications
            var notificationsEnabledLoad = localStorage.getItem("play_html5PushEnabled");
            if (notificationsEnabledLoad == undefined) {
                localStorage.setItem("play_html5PushEnabled", "false");
            } else if (localStorage.getItem("play_html5PushEnabled") == "true") {
                notificationsEnabled = true;
            }

           

        });
    }

});

function backgroundPulse() {
    if (attackPulse == 1) {
        $("body").animate({ backgroundColor: "#A16969" }, 1500).animate({ backgroundColor: "#475374" }, 1500, backgroundPulse);
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


function titleToggle() {
    if (blinkEnabled == 1) {
        if (togglestate == 0) {
            $('title').text('Transformania Time!');
            togglestate = 1;
        } else if (togglestate == 1) {
            $('title').text('[UPDATED--reload page!]');
            togglestate = 0;
        }
        if (playUpdateSound == true && (secondsToUpdate + 3) % 7 == 0) {
            var audio = new Audio('../Sounds/turnUpdate1.wav');
            audio.play();
        }
    }
}

function sendAlertBox(noticemessage) {
    if (window.Notification && Notification.permission !== "granted") {
        Notification.requestPermission(function (status) {
            if (Notification.permission !== status) {
                Notification.permission = status;
            }
        });
    }
    if (window.Notification && Notification.permission === "granted") {
        var n = new Notification("Alert!", { body: noticemessage, icon: "/Images/PvP/Icons/tt_logo.ico" });
    }
    else if (window.Notification && Notification.permission !== "denied") {
        Notification.requestPermission(function (status) {
            if (Notification.permission !== status) {
                Notification.permission = status;
            }
            if (status === "granted") {
                var n = new Notification("Alert!", { body: noticemessage, icon: "/Images/PvP/Icons/tt_logo.ico" });
            }
        });
    }
}