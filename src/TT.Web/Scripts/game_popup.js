function startPopupRound() {

    var tfsToDo;

    var popupTimerFunc;
    var popupAICheckFunc;
    var popupSpawnTargetFunc;

    var popupRoundType = 0;

    var popupTime = 1.5;

    var missPenalty = 0;
    var scoreValueDecay = 1;

    var pScore = 0;
    var cScore = 0;
    var winPoints = 100;

    var playerCooldown = .1;

    var start = function () {
        centerMeToWindow($("#popupGameMainWindow"));
         setMinigameNameplate($("#popupGameMainWindow"), "Reflex:  Popups");
        $("#popupActionWindow").css("background-image", "url(../images/background2.jpg)");
        popupTimerFunc = setInterval("updatePopupTimer()", 10); // runs every 10 milliseconds (.1 seconds)

        hideAllTargets();

        popupSetDifficulty();


        popupUpdateProgressBars();

        $(".popupTargetLarge").click(function (e) {
            playerCooldown = .1;
            pScore += 2 * scoreValueDecay;
            $(this).hide();
            popupUpdateProgressBars();
        });
        $(".popupTargetMedium").click(function (e) {
            playerCooldown = .1;
            pScore += 4 * scoreValueDecay;
            $(this).hide();
            popupUpdateProgressBars();
        });
        $(".popupTargetSmall").click(function (e) {
            playerCooldown = .1;
            pScore += 8 * scoreValueDecay;
            $(this).hide();
            popupUpdateProgressBars();
        });
        $(".popupTargetSuperSmall").click(function (e) {
            playerCooldown = .1;
            pScore += 16 * scoreValueDecay;
            $(this).hide();
            popupUpdateProgressBars();
        });




        if (gameSpeed == "slow") { tfsToDo = 1; }
        if (gameSpeed == "normal") { tfsToDo = 2; }
        if (gameSpeed == "fast") { tfsToDo = 3; }

    }

    var popupSetDifficulty = function () {


        if (gameDifficulty == "easy") {
            missPenalty = 0;
            scoreValueDecay = 1.1;
        } else if (gameDifficulty == "medium") {
            missPenalty = 2;
            scoreValueDecay = 1;
        } else if (gameDifficulty == "hard") {
            missPenalty = 5;
            scoreValueDecay = .8;
        } else if (gameDifficulty == "insane") {
            missPenalty = 10;
            scoreValueDecay = .6;
        } else if (gameDifficulty == "default") {
            missPenalty = 1.2 * globalDifficultyModifierPopup;
            scoreValueDecay = 1 - .04 * globalDifficultyModifierPopup;
            if (scoreValueDecay < .1) { scoreValueDecay = .1; }

        }

        $("#popupMissPenalty").html("Miss penalty:  " + missPenalty.toFixed(3));
    }

    var endRoundLogic = function () {
        if (pScore > cScore) {
            writeMessage("You have <green>WON</green> a round!<br>");
            tfOpponent(tfsToDo);
            AjaxSubmit("Win Popup");
            updateGlobalDifficultyModifier(1.5,"popup");
            updateGlobalProgressBars();
        } else {
            writeMessage("You have <red>LOST</red> a round!<br>");
            tfHuman(tfsToDo);
            AjaxSubmit("Lose Popup");
            updateGlobalDifficultyModifier(-.7,"popup");
            updateGlobalProgressBars();
        }

        $("#popupGameMainWindow").hide();
        $("#grayBackground").hide();
        $("#helpButton").hide();
        $("#minigameNameplate").hide();
    }




    this.updatePopupTimer = function () {

        popupTime -= .01;
        playerCooldown -= .01;

        if (popupRoundType == 0) { // warmup stage

            if (popupTime < 0) {
                difficulty = calculateDifficulty();
                popupRoundType = 1;
                popupAICheckFunc = setInterval("popupAICheck()", 250); // runs every 10 milliseconds (.1 seconds)
                popupSpawnTargetFunc = setInterval("popupSpawnSchedule()", 200); // runs every 10 milliseconds (.1 seconds)
                popupTime = 15;

                $("#popupActionWindow").click(function (e) {
                    if (playerCooldown < 0) {
                        pScore -= missPenalty;
                        popupUpdateProgressBars();
                    }

                });

            }


        } else if (popupRoundType == 1) { // action stage
            if ((pScore > winPoints) || (cScore > winPoints)) {
                popupRoundType = 2;
                clearInterval(popupAICheckFunc);

                clearInterval(popupSpawnTargetFunc);
                hideAllTargets();
                popupTime = 2;

                if (pScore > cScore) {
                    $("#popupActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
                } else {
                    $("#popupActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
                }

                //unbind click events
                $('#popupActionWindow').unbind('click');
                $('.popupTargetLarge').unbind('click');
                $('.popupTargetMedium').unbind('click');
                $('.popupTargetSmall').unbind('click');
                $('.popupTargetSuperSmall').unbind('click');
            }


        } else if (popupRoundType == 2) { // cooldown stage
            if (popupTime < 0) {
                clearInterval(popupTimerFunc);
                endRoundLogic();

            }
        }

    }

    this.popupAICheck = function () {

        var num = Math.random() * 2.9;
        if (num < .65) {
            cScore += 2;
            popupUpdateProgressBars();
        } else if (num < .7) {
            cScore += 4;
            popupUpdateProgressBars();
        } else if (num < .95) {
            cScore += 8;
            popupUpdateProgressBars();
        } else if (num < 1.02) {
            cScore += 16;
            popupUpdateProgressBars();
        }

    }

    this.popupSpawnSchedule = function () {
        if (Math.random() < .9) {
            spawnTarget();
        }
    }

    var popupUpdateProgressBars = function () {
        var newWidthO = Math.floor(parseInt(cScore));
        var newWidthP = Math.floor(parseInt(pScore));

        if (newWidthO > 100) { newWidthO = 100; }
        if (newWidthP > 100) { newWidthP = 100; }

        newWidthO += "%";
        newWidthP += "%";

        newWidthO = newWidthO.toString();
        newWidthP = newWidthP.toString();

        $("#popupDebug1").html(newWidthO + " , " + cScore);
        $("#popupDebug2").html(newWidthP + " , " + pScore);



        $("#popupOpponentScoreProgressBar").animate({ "width": newWidthO }, 50);
        $("#popupPlayerScoreProgressBar").animate({ "width": newWidthP }, 50);


    }

    var calculateDifficulty = function () {

    }

    var spawnTarget = function (size) {

        var maxTargets = 15;

        var randx = Math.random() * 400;
        var randy = Math.random() * 400;

        var num = Math.floor(Math.random() * maxTargets + 1);

        if (num <= 8) {
            $(".popupTargetLarge:nth-child(" + num + ")").show();
            $(".popupTargetLarge:nth-child(" + num + ")").css('left', randx);
            $(".popupTargetLarge:nth-child(" + num + ")").css('top', randy);
            $(".popupTargetLarge:nth-child(" + num + ")").css('z-index', num + 10);
        } else if (num <= 12) {
            $(".popupTargetMedium:nth-child(" + num + ")").show();
            $(".popupTargetMedium:nth-child(" + num + ")").css('left', randx);
            $(".popupTargetMedium:nth-child(" + num + ")").css('top', randy);
            $(".popupTargetMedium:nth-child(" + num + ")").css('z-index', num + 10);
        } else if (num <= 14) {
            $(".popupTargetSmall:nth-child(" + num + ")").show();
            $(".popupTargetSmall:nth-child(" + num + ")").css('left', randx);
            $(".popupTargetSmall:nth-child(" + num + ")").css('top', randy);
            $(".popupTargetSmall:nth-child(" + num + ")").css('z-index', num + 10);
        } else if (num <= 15) {
            $(".popupTargetSuperSmall:nth-child(" + num + ")").show();
            $(".popupTargetSuperSmall:nth-child(" + num + ")").css('left', randx);
            $(".popupTargetSuperSmall:nth-child(" + num + ")").css('top', randy);
            $(".popupTargetSuperSmall:nth-child(" + num + ")").css('z-index', num + 10);
        }

    }

    var hideAllTargets = function () {
        var kids = $("#popupActionWindow").children();
        kids.hide();
    }

    var showAllTargets = function () {
        var kids = $("#popupActionWindow").children();
        kids.show();
    }

    start();

}