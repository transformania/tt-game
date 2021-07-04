var attackPulse = 0;
var nextRoundCountdownInitialValue;


$(document).ready(function () {

    nextRoundCountdownInitialValue = $("#nextRound").html();
    updateRoundCountdownTimer();

    if ($("#notificationBox").length) {
        initNotificationBox();
    }

    if ($("#turn_countdown").length) {
        initClock($('title'));
    }
});

function initNotificationBox() {
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

                    if (playAttackSound == true) {
                        var attackedAudio = new Audio('../Sounds/attack.wav');
                        attackedAudio.play();
                    }
                    

                } else if (notice.type == "message") {
                    if (playMessageSound == true) {
                        var attackedAudio = new Audio('../Sounds/paper.wav');
                        attackedAudio.play();
                    }
                }


            };


            $.connection.hub.start().done(function () {
                link.server.connect();

                $("#liveConnectionNotice").addClass("noticeOn");
                $("#liveConnectionNotice").removeClass("noticeOff");
                $("#liveConnectionIcon").addClass("glyphicon-ok-circle");
                $("#liveConnectionIcon").removeClass("glyphicon-remove-circle");
                $("#liveConnectionNotice").children(".sr-only").text("Connected:");

            });

            $.connection.hub.disconnected(function () {
                $("#liveConnectionNotice").addClass("noticeOff");
                $("#liveConnectionNotice").removeClass("noticeOn");
                $("#liveConnectionIcon").addClass("glyphicon-remove-circle");
                $("#liveConnectionIcon").removeClass("glyphicon-ok-circle");
                $("#liveConnectionNotice").children(".sr-only").text("Disconnected:");
            });

            // load whether or not to play audio notifications for updates
            var playUpdateSoundLoad = localStorage.getItem("play_updateSoundEnabled");
            if (playUpdateSoundLoad == undefined) {
                localStorage.setItem("play_updateSoundEnabled", "false");
            } else if (localStorage.getItem("play_updateSoundEnabled") == "true") {
                playUpdateSound = true;
            }

            // load whether or not to play audio notifications for updates
            var playAttackSoundLoad = localStorage.getItem("play_AttackSoundEnabled");
            if (playAttackSoundLoad == undefined) {
                localStorage.setItem("play_AttackSoundEnabled", "false");
            } else if (localStorage.getItem("play_AttackSoundEnabled") == "true") {
                playAttackSound = true;
            }

            // load whether or not to play message notifications 
            var playMessageSoundLoad = localStorage.getItem("play_MessageSoundEnabled");
            if (playMessageSoundLoad == undefined) {
                localStorage.setItem("play_MessageSoundEnabled", "false");
            } else if (localStorage.getItem("play_MessageSoundEnabled") == "true") {
                playMessageSound = true;
            }

            // load whether or not to use html5 notifications
            var notificationsEnabledLoad = localStorage.getItem("play_html5PushEnabled");
            if (notificationsEnabledLoad == undefined) {
                localStorage.setItem("play_html5PushEnabled", "false");
            } else if (localStorage.getItem("play_html5PushEnabled") == "true") {
                notificationsEnabled = true;
            }

           

        });

        if (localStorage.getItem("play_keyMovement") !== null && localStorage.getItem("play_keyMovement") !== "false") {
            setupWASDKeys();
        }
    }
}

function initClock(tabTitle) {
    var titleText = tabTitle.text();

    // javascript for ticking down clocks
    setInterval(function () {
        secondsToUpdate--;
        timer_minutes = Math.floor(secondsToUpdate / 60);
        timer_seconds = String(secondsToUpdate % 60);

        if (timer_seconds.length == 1) {
            timer_seconds = "0" + timer_seconds;
        }

        if (timer_minutes < 0) {
            timer_minutes = "0";
            timer_seconds = "00";
            $("#turn_countdown").addClass("good");
            blinkEnabled = 1;
        } else {
            tabTitle.text(titleText + ' [' + timer_minutes + ":" + timer_seconds + "]");
        }

        $("#turn_countdown").html(timer_minutes + ":" + timer_seconds);

        updateRoundCountdownTimer();


    }, 1000);

    setInterval(function () { titleToggle(titleText) }, 1000);

}

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


function titleToggle(titleText) {
    if (blinkEnabled == 1) {
        if (togglestate == 0) {
            $('title').text(titleText);
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
        var n = new Notification("Alert!", { body: noticemessage, icon: "https://images.transformaniatime.com/Icons/tt_logo.ico" });
        n.onclick = function () {
            window.focus();
            this.close();
        };
    }
    else if (window.Notification && Notification.permission !== "denied") {
        Notification.requestPermission(function (status) {
            if (Notification.permission !== status) {
                Notification.permission = status;
            }
            if (status === "granted") {
                var n = new Notification("Alert!", { body: noticemessage, icon: "https://images.transformaniatime.com/Icons/tt_logo.ico" });
                n.onclick = function () {
                    window.focus();
                    this.close();
                };
            }
        });
    }
}

function parseAttackLinks() {

    var lastAttackSpan = $("#lastAttack");
    var latestAttacksString = localStorage.getItem("play_lastAttack");
    var latestAttacks = [];

    // check if typeof is string, if so, clear it out as it'll break the parsing
    if (latestAttacksString !== null) {

        try {
            latestAttacks = JSON.parse(latestAttacksString);
            if (typeof latestAttacks === "string") {
                latestAttacks = [];
            }
        } catch (e) {
            latestAttacks = [];
        }

    }


    if (latestAttacks.length > 0) {
        latestAttacks = latestAttacks.slice(0, localStorage.getItem("play_lastAttackCount"));
        lastAttackSpan.html("<b>Latest:</b>  " + lastAttackSpan.html());
        lastAttackSpan.css("background-color", "#996699");
    }
    

    if (attacksMade >= attackCap) {

        $(".action_attack").each(function () {
            $(this).attr("href", "#");
            $(this).addClass("disabled");
        });

    } else {

        $(".action_attack").each(function () {

            var thisSpellText = $(this).html();

            $(this).click(function () {

                if (!latestAttacks.includes(thisSpellText)) {
                    latestAttacks.unshift(thisSpellText);
                }

                localStorage.setItem("play_lastAttack", JSON.stringify(latestAttacks));
            });

            // if this spell appears in the latest attacks, render it in the latest bar
            for (var i = 0; i < latestAttacks.length; i++) {
                if (thisSpellText === latestAttacks[i]) {
                    $(this).clone().appendTo(lastAttackSpan);
                }
            }


            var cost = $(this).attr("manacost");

            // disable if too costly
            if (cost > playerMana) {
                $(this).attr("href", "#");
                $(this).addClass("disabled");
            }

        });

    }

}


function fail() {
    // alert("fail");
}

function showAttackModal() {
    $("#attackModal").show();
    $("#modalBackdrop").show();
    parseAttackLinks();
}

function closeAttackModal() {
    $("#attackModal").hide();
    $("#modalBackdrop").hide();
}

function attack_Success() {
    showAttackModal();
}

function attack_Wait() {
    $("#attackModal").show();
    $("#attackModal").html("Loading...");
    $("#modalBackdrop").show();
}

function attack_Failure() {
    alert("ERROR:  Failed to launch attack modal window.  Try again shortly.  If this persists, you have either been logged out or the server is momentarily having problems.");
}

function closeAttackModal() {
    $("#attackModal").hide();
    $("#modalBackdrop").hide();
}

function showHidePlayers() {    
    $(".onlinePlayersWrapperBG").toggle();
}

function updateRoundCountdownTimer() {
    if ($("#nextRound").length) {
        var timestampLocal = new Date(nextRoundCountdownInitialValue);
        var timestampUtc = timestampLocal.getTime() - timestampLocal.getTimezoneOffset() * 60 * 1000;
        var now = Date.now();
        var timeAwayInSeconds = Math.floor((timestampUtc - now) / 1000);
        var displayTime = `<b>${Math.floor(timeAwayInSeconds / 86400)}</b> days, <b>${Math.floor(timeAwayInSeconds / 3600) % 24}</b> hours, and <b>${Math.floor(timeAwayInSeconds / 60) % 60}</b> minutes`;
        $("#nextRound").html(displayTime);
    }
}

function setupWASDKeys() {

    var codes = {
        "w": "north",
        "a": "west",
        "s": "south",
        "d": "east"
    }

    document.onkeypress = function (e) {
        if (document.getElementById('attack-search-box') !== document.activeElement && codes[e.key]) {
            var cell = document.getElementById(codes[e.key]);
            if (cell) {
                var link = cell.getAttribute("href");
                window.location.replace(link);
            }
        }
    }
}