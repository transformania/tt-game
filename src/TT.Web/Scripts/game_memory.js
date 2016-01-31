

function startMemoryRound() {

    var memoryRoundType = 0;
    var memoryTime = 1.5;
    var memoryTimerFunc;
    var memoryAIcheckFunc;

    var lastIndexClicked = -1;
    var lastIndexClicked2 = -1;

    var clickOneOrTwo = 1;

    var pScore = 0;
    var cScore = 0;
    var difficulty;

    var code = new Array();
    var tfsToDo;



    var start = function () {
        centerMeToWindow($("#memoryGameMainWindow"));
        setMinigameNameplate($("#memoryGameMainWindow"), "Memory:  Match Two");
        $("#memImgWrapper").show();
        $("#memoryActionWindow").css("background-image", "url(../images/background2.jpg)");
        memoryTimerFunc = setInterval("updateMemoryTimer()", 10); // runs every 10 milliseconds (.1 seconds)
        fillArrayWithCrap();
        generateCode();
        hideImages();

        memUpdateProgressBars();


        if (gameSpeed == "slow") { tfsToDo = 1; }
        if (gameSpeed == "normal") { tfsToDo = 2; }
        if (gameSpeed == "fast") { tfsToDo = 3; }




    }

    // FUNCTION:  update memory timer and run ingame logic
    this.updateMemoryTimer = function updateMemoryTimer() {

        memoryTime -= .01;

        if (memoryRoundType == 0) { // warmup stage

            if (memoryTime < 0) {
                difficulty = calculateDifficulty();
                memoryRoundType = 1;
                memoryTime = 3 - 5 * difficulty;
                if (memoryTime < 1) { memoryTime = 1; }
                showImages();

            }


        } else if (memoryRoundType == 1) { // reveal stage
            if (memoryTime < 0) {
                memoryRoundType = 2;
                memoryTime = 10;
                hideImages();

                // register click for buttons
                $(".memImgBtn").click(function (e) {
                    var i = $(".memImgBtn").index(this) + 1;
                    showThisImage(i);
                });

                memoryAIcheckFunc = setInterval("memAICheck()", 250); // runs every 10 milliseconds (.1 seconds)


            }
        } else if (memoryRoundType == 2) { // guess stage
            if ((pScore >= 8) || (cScore >= 8)) {
                memoryRoundType = 3;
                memoryTime = 1.5;

                // unbind our click handler
                $('.memImgBtn').unbind('click');
                clearInterval(memoryAIcheckFunc);

                $("#memImgWrapper").hide();
                if (pScore > cScore) {
                    $("#memoryActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
                } else {
                    $("#memoryActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
                }

            }
        } else if (memoryRoundType == 3) { // result stage

            if (memoryTime < 0) {
                endRoundLogic();
            }
        }

    }

    var endRoundLogic = function () {
        clearInterval(memoryTimerFunc);

        if (pScore > cScore) {
            writeMessage("You have <green>WON</green> a round!<br>");
            tfOpponent(tfsToDo);
            AjaxSubmit("Win Match2");
            updateGlobalDifficultyModifier(1.5,"sequence");
            updateGlobalProgressBars();
        } else {
            writeMessage("You have <red>LOST</red> a round!<br>");
            tfHuman(tfsToDo);
            AjaxSubmit("Lose Match2");
            updateGlobalDifficultyModifier(-.75,"sequence");
            updateGlobalProgressBars();
        }

        $("#memoryGameMainWindow").hide();
        $("#grayBackground").hide();
        $("#helpButton").hide();
        $("#minigameNameplate").hide();
    }

    var hideImages = function () {
        for (var i = 1; i <= 16; i++) {
            $(".memImgBtn:nth-child(" + i + ")").css({ 'background-image': 'url(../images/symb_blank.png)' });
        }
    }

    this.memAICheck = function () {

        if (Math.random() < difficulty) {
            cScore++;
            memUpdateProgressBars();
            flashBorder("red");
        }
    }

    var showImages = function () {
        for (var i = 1; i <= 16; i++) {

            if (code[i - 1] == 0) {
                memSetImage(i, "star");
            } else if (code[i - 1] == 1) {
                memSetImage(i, "4pointstar");
            } else if (code[i - 1] == 2) {
                memSetImage(i, "blueThing");
            } else if (code[i - 1] == 3) {
                memSetImage(i, "energy");
            } else if (code[i - 1] == 4) {
                memSetImage(i, "sparks");
            } else if (code[i - 1] == 5) {
                memSetImage(i, "cross");
            } else if (code[i - 1] == 6) {
                memSetImage(i, "city");
            } else if (code[i - 1] == 7) {
                memSetImage(i, "greenStar");
            } else if (code[i - 1] == 8) {
                memSetImage(i, "tree");
            } else if (code[i - 1] == 9) {
                memSetImage(i, "pinkDrops");
            } else if (code[i - 1] == 10) {
                memSetImage(i, "waterfall");
            } else if (code[i - 1] == 11) {
                memSetImage(i, "cell");
            } else if (code[i - 1] == 12) {
                memSetImage(i, "vines");
            } else if (code[i - 1] == 13) {
                memSetImage(i, "blueTarget");
            } else if (code[i - 1] == 14) {
                memSetImage(i, "3circs");
            }

        }
    }

    var showThisImage = function (i) {

        if (clickOneOrTwo != 3) {

            if (code[i - 1] == 0) {
                memSetImage(i, "star");
            } else if (code[i - 1] == 1) {
                memSetImage(i, "4pointstar");
            } else if (code[i - 1] == 2) {
                memSetImage(i, "blueThing");
            } else if (code[i - 1] == 3) {
                memSetImage(i, "energy");
            } else if (code[i - 1] == 4) {
                memSetImage(i, "sparks");
            } else if (code[i - 1] == 5) {
                memSetImage(i, "cross");
            } else if (code[i - 1] == 6) {
                memSetImage(i, "city");
            } else if (code[i - 1] == 7) {
                memSetImage(i, "greenStar");
            } else if (code[i - 1] == 8) {
                memSetImage(i, "tree");
            } else if (code[i - 1] == 9) {
                memSetImage(i, "pinkDrops");
            } else if (code[i - 1] == 10) {
                memSetImage(i, "waterfall");
            } else if (code[i - 1] == 11) {
                memSetImage(i, "cell");
            } else if (code[i - 1] == 12) {
                memSetImage(i, "vines");
            } else if (code[i - 1] == 13) {
                memSetImage(i, "blueTarget");
            } else if (code[i - 1] == 14) {
                memSetImage(i, "3circs");
            }



            //we are on our first click; do not check for victory logic
            if (clickOneOrTwo == 1) {
                clickOneOrTwo = 2;
                lastIndexClicked = i;

                // we are on our second click; check for a match
            } else if (clickOneOrTwo == 2) {

                if (lastIndexClicked != i) {

                    if (code[lastIndexClicked - 1] == code[i - 1]) {
                        $(".memImgBtn:nth-child(" + i + ")").unbind('click');
                        $(".memImgBtn:nth-child(" + lastIndexClicked + ")").unbind('click');
                        clickOneOrTwo = 1;
                        lastIndexClicked = -1;
                        pScore++;
                        memUpdateProgressBars();
                        flashBorder("green");
                    } else {
                        //window.setTimeout("javascript function",250);

                        clickOneOrTwo = 3;
                        lastIndexClicked2 = i;
                    }

                }
            }

        } else if (clickOneOrTwo == 3) {
            memSetImage(lastIndexClicked2, "blank");
            memSetImage(lastIndexClicked, "blank");
            clickOneOrTwo = 1;
        }



        //}




    }

    var memSetImage = function (position, imgName) {
        $(".memImgBtn:nth-child(" + (position) + ")").css({ 'background-image': 'url(../images/symb_' + imgName + '.png)' });
    }

    var findEmptySlots = function (value) {
        var location = Math.floor(Math.random() * 16);
        var location2 = Math.floor(Math.random() * 16);

        var i = 0;

        var old0 = code[location];
        var old1 = code[location2];



        while ((code[location] != "_") && (i < 1000)) {
            location = (location + Math.floor(Math.random() * 3)) % 16;
            i++;
        }

        code[location] = value;

        while ((code[location2] != "_") && (i < 1000)) {
            location2 = (location2 + Math.floor(Math.random() * 3)) % 16;
            i++;
        }

        if (i > 1000) {
            //alert("crap.");
        }
        code[location2] = value;


    }

    var generateCode = function () {

        // for all 8 sets of 2
        for (var i = 0; i < 8; i++) {
            var randNum = Math.floor(Math.random() * 15);

            while (jQuery.inArray(randNum, code) != -1) {
                randNum = Math.floor(Math.random() * 15);
            }
            findEmptySlots(randNum);
        }


    }


    var flashBorder = function (color) {

        var value;
        if (color == "green") {
            value = "#00FF00";
        } else if (color == "red") {
            value = "#FF0000";
        }

        $('#memoryActionWindow').animate({ borderColor: value }, 100, flashBorderBack);

        function flashBorderBack() {
            $('#memoryActionWindow').animate({ borderColor: '#000000' }, 100);
        }

    }

    var fillArrayWithCrap = function () {
        for (var i = 0; i < 16; i++) {
            code[i] = "_";
        }
    }

    var memUpdateProgressBars = function () {
        var newWidthO = (((parseInt(cScore, 10) / 8) * 100)) + "%";
        var newWidthP = (((parseInt(pScore, 10) / 8) * 100)) + "%";

        newWidthO = newWidthO.toString();
        newWidthP = newWidthP.toString();

        $("#memoryOpponentScoreProgressBar").animate({ "width": newWidthO }, "fast");
        $("#memoryPlayerScoreProgressBar").animate({ "width": newWidthP }, "fast");
    }

    start();

    var calculateDifficulty = function () {

        if (gameDifficulty == "default") {
            difficulty = .04 + .04 * globalDifficultyModifierMemory + .004 * npcCharLoader.characters[1].intelligence - .0032 * humanCharLoader.characters[1].intelligence;
        } else if (gameDifficulty == "easy") {
            difficulty = .06;
        } else if (gameDifficulty == "medium") {
            difficulty = .1;
        } else if (gameDifficulty == "hard") {
            difficulty = .14;
        } else if (gameDifficulty == "insane") {
            difficulty = .2;
        }
        return difficulty;
    }
}