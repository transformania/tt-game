var roundStage = 0; // 0 is idle, 1 is warmup, 2 is question stage, 3 is running
var timeCurrent = 0.00;
var timeWarmupMax = .5;
var timeQuestionMax = 2;
var timeGameMax = 5;

var sequenceTimerFunc;
var sequenceAIcheckFunc;
var sequenceTime = 0;

var sequenceCorrectCode = "";
var sequenceGuessedCode = "";
var sequenceMaxLength = 5;
var sequenceSymbolsUsed = 15;

var sequenceRoundStage = 0;
var sequenceWarmupTime = 1;
var sequenceRevealTime = 3;

var sequencePointsAI = 0;
var sequencePointsPlayer = 0;
var sequenceWinningPointsAI = 8;

var sequenceCooldownTimeAI = .5;

function startSequenceRound() {

    $("#seqImgWrapper").show();
    $("#seqImgBtnsWrapper").show();
    $("#sequenceActionWindow").css("background-size", "100px 100px");

    sequenceRoundStage = 0;
    sequenceTime = sequenceWarmupTime;
    sequenceTimerFunc = setInterval("updateSequenceTimer()", 10); // runs every 10 milliseconds (.1 seconds)
    centerMeToWindow($("#sequenceGameMainWindow"));
    setMinigameNameplate($("#sequenceGameMainWindow"),"Memory:  Sequence");
    sequencePointsAI = 0;
    sequencePointsPlayer = 0;
    sequenceUpdateProgressBars();

    sequenceCorrectCode = "";
    sequenceGuessedCode = "";
    sequenceHideCode();

    $("#sequenceActionWindow").css("background-image", "url(../images/background2.jpg)");

    //$("#sequenceTest1").html("");
    seqAssignButtons();

    $(".seqImgBtn").click(function (e) {

        // if in guessing stage...
        if (sequenceRoundStage == 2) {
            // grab the index of what we just clicked
            var i = $(".seqImgBtn").index(this) + 1;
            seqModifyGuessed(i);
            sequenceUpdateProgressBars();
        }
    });

}

function endSequenceRoundLogic() {

    //alert(gameSpeed);
    var tfsToDo;
    if (gameSpeed == "slow") { tfsToDo = 1; }
    if (gameSpeed == "normal") { tfsToDo = 2; }
    if (gameSpeed == "fast") { tfsToDo = 3; }

    if (sequencePointsAI < sequenceMaxLength) {
        writeMessage("You have <green>WON</green> a round!<br>");
        tfOpponent(tfsToDo);
        AjaxSubmit("Win Sequence");
        updateGlobalDifficultyModifier(1.5,"sequence");
    } else {
        writeMessage("You have <red>LOST</red> a round!<br>");
        tfHuman(tfsToDo);
        AjaxSubmit("Lose Sequence");
        updateGlobalDifficultyModifier(-.75,"sequence");
    }

    sequencePointsAI = 0;

    clearInterval(sequenceTimerFunc);

    $("#sequenceGameMainWindow").hide();
    $("#grayBackground").hide();
    $("#helpButton").hide();

    // unbind our click handler
    $('.seqImgBtn').unbind('click');

    updateGlobalProgressBars();
    $("#minigameNameplate").hide();

}

function updateSequenceTimer() {

    sequenceTime = sequenceTime - .01;
    sequenceCooldownTimeAI = sequenceCooldownTimeAI - .01;
    $("#sequenceTest1").html(sequenceTime);
    $("#sequenceTest2").html(sequenceGuessedCode + " , " + sequenceCorrectCode);

    // warmup stage
    if (sequenceRoundStage == 0) {
        if (sequenceTime < 0) {
            sequenceRoundStage = 1;
            sequenceTime = sequenceRevealTime;
            sequenceGenerateCode();
            sequenceShowCode();
        }
    }

        // reveal stage
    else if (sequenceRoundStage == 1) {
        if (sequenceTime < 0) {
            sequenceHideCode();
            sequenceRoundStage = 2;
            sequenceAIcheckFunc = setInterval("sequenceAIscoreCheck()", 125);
        }
    }

        // guessing stage
    else if (sequenceRoundStage == 2) {

        // ai has won
        if (sequencePointsAI >= sequenceMaxLength) {
            clearInterval(sequenceAIcheckFunc);
            sequenceRoundStage = 3;
            sequenceTime = 1.5;
            $("#sequenceActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
            $("#sequenceActionWindow").css("background-size", "700px 500px");
            $("#seqImgWrapper").hide();
            $("#seqImgBtnsWrapper").hide();
            // player has won	
        } else if (seqPlayerHasCorrectAnswer()) {
            clearInterval(sequenceAIcheckFunc);
            sequenceRoundStage = 3;
            sequenceTime = 1.5;
            $("#sequenceActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
            $("#sequenceActionWindow").css("background-size", "700px 500px");
            $("#seqImgWrapper").hide();
            $("#seqImgBtnsWrapper").hide();
        }

        // results stage
    } else if (sequenceRoundStage == 3) {
        if (sequenceTime < 0) {
            endSequenceRoundLogic();
        }
    }

}

function seqPlayerHasCorrectAnswer() {
    if (sequenceCorrectCode == sequenceGuessedCode) {
        return true;
    } else {
        return false;
    }
}

function sequenceAIscoreCheck() {
    var num = Math.random();

    var sequenceAIGuessSlowdown = .7 * (sequencePointsAI / sequenceMaxLength);

    if ((num < .05) && (sequencePointsAI > 0)) {
        sequencePointsAI--;
    } else if (num < .8) {
        if (sequenceCooldownTimeAI < 0) {
            sequencePointsAI++;
            sequenceCooldownTimeAI = .8 + sequenceAIGuessSlowdown;
            sequenceUpdateProgressBars();
        }
    }
}

function sequenceSetImage(position, imgName) {
    $(".seqImg:nth-child(" + (position + 1) + ")").css({ 'background-image': 'url(../images/symb_' + imgName + '.png)' });
}

function sequenceGenerateCode() {

    sequenceMaxLength = seqCalculateDifficulty();

    for (var i = 0; i < sequenceMaxLength; i++) {
        var random = Math.floor(Math.random() * sequenceSymbolsUsed);

        var letter;

        if (random == 0) {
            letter = "A";
        } else if (random == 1) {
            letter = "B";
        } else if (random == 2) {
            letter = "C";
        } else if (random == 3) {
            letter = "D";
        } else if (random == 4) {
            letter = "E";
        } else if (random == 5) {
            letter = "F";
        } else if (random == 6) {
            letter = "G";
        } else if (random == 7) {
            letter = "H";
        } else if (random == 8) {
            letter = "I";
        } else if (random == 9) {
            letter = "J";
        } else if (random == 10) {
            letter = "K";
        } else if (random == 11) {
            letter = "L";
        } else if (random == 12) {
            letter = "M";
        } else if (random == 13) {
            letter = "N";
        } else if (random == 14) {
            letter = "O";
        }

        sequenceCorrectCode = sequenceCorrectCode + letter;
    }
    return sequenceCorrectCode;
}

function sequenceShowCode() {
    for (var i = 0; i < sequenceMaxLength; i++) {
        if (sequenceCorrectCode[i] == "A") {
            sequenceSetImage(i, "star");
        } else if (sequenceCorrectCode[i] == "B") {
            sequenceSetImage(i, "4pointstar");
        } else if (sequenceCorrectCode[i] == "C") {
            sequenceSetImage(i, "blueThing");
        } else if (sequenceCorrectCode[i] == "D") {
            sequenceSetImage(i, "energy");
        } else if (sequenceCorrectCode[i] == "E") {
            sequenceSetImage(i, "sparks");
        } else if (sequenceCorrectCode[i] == "F") {
            sequenceSetImage(i, "cross");
        } else if (sequenceCorrectCode[i] == "G") {
            sequenceSetImage(i, "city");
        } else if (sequenceCorrectCode[i] == "H") {
            sequenceSetImage(i, "greenStar");
        } else if (sequenceCorrectCode[i] == "I") {
            sequenceSetImage(i, "tree");
        } else if (sequenceCorrectCode[i] == "J") {
            sequenceSetImage(i, "pinkDrops");
        } else if (sequenceCorrectCode[i] == "K") {
            sequenceSetImage(i, "waterfall");
        } else if (sequenceCorrectCode[i] == "L") {
            sequenceSetImage(i, "cell");
        } else if (sequenceCorrectCode[i] == "M") {
            sequenceSetImage(i, "vines");
        } else if (sequenceCorrectCode[i] == "N") {
            sequenceSetImage(i, "blueTarget");
        } else if (sequenceCorrectCode[i] == "O") {
            sequenceSetImage(i, "3circs");
        }
    }
}

// FUNCTION:  resets all the revealed boxes to blank
function sequenceHideCode() {
    // reset all the boxes back to the original setup
    for (var i = 1; i < 9; i++) {
        $(".seqImg:nth-child(" + i + ")").css({ 'background-image': 'url(../images/symb_blank.png)' });

    }
}

function sequenceUpdateProgressBars() {
    var newWidthO = (((parseInt(sequencePointsAI, 10) / (sequenceMaxLength)) * 100)) + "%";
    newWidthO = newWidthO.toString();

    sequencePointsPlayer = 0;

    if (sequenceRoundStage == 2) {
        for (var i = 0; i < sequenceMaxLength; i++) {
            if (sequenceGuessedCode[i] == sequenceCorrectCode[i]) {
                sequencePointsPlayer++;
            }
        }
    }

    var newWidthP = (((parseInt(sequencePointsPlayer, 10) / (sequenceMaxLength)) * 100)) + "%";
    newWidthP = newWidthP.toString();



    $("#sequenceOpponentScoreProgressBar").animate({ "width": newWidthO }, "fast");
    $("#sequencePlayerScoreProgressBar").animate({ "width": newWidthP }, "fast");
}



function seqAssignButtons() {
    $(".seqImgBtn:nth-child(1)").css("background-image", "url(../images/symb_star.png)");
    $(".seqImgBtn:nth-child(2)").css("background-image", "url(../images/symb_4pointstar.png)");
    $(".seqImgBtn:nth-child(3)").css("background-image", "url(../images/symb_blueThing.png)");
    $(".seqImgBtn:nth-child(4)").css("background-image", "url(../images/symb_energy.png)");
    $(".seqImgBtn:nth-child(5)").css("background-image", "url(../images/symb_sparks.png)");
    $(".seqImgBtn:nth-child(6)").css("background-image", "url(../images/symb_cross.png)");
    $(".seqImgBtn:nth-child(7)").css("background-image", "url(../images/symb_city.png)");
    $(".seqImgBtn:nth-child(8)").css("background-image", "url(../images/symb_greenStar.png)");
    $(".seqImgBtn:nth-child(9)").css("background-image", "url(../images/symb_tree.png)");
    $(".seqImgBtn:nth-child(10)").css("background-image", "url(../images/symb_pinkDrops.png)");
    $(".seqImgBtn:nth-child(11)").css("background-image", "url(../images/symb_waterfall.png)");
    $(".seqImgBtn:nth-child(12)").css("background-image", "url(../images/symb_cell.png)");
    $(".seqImgBtn:nth-child(13)").css("background-image", "url(../images/symb_vines.png)");
    $(".seqImgBtn:nth-child(14)").css("background-image", "url(../images/symb_blueTarget.png)");
    $(".seqImgBtn:nth-child(15)").css("background-image", "url(../images/symb_3circs.png)");
    $(".seqImgBtn:nth-child(16)").css("background-image", "url(../images/back.png)");
}

function seqModifyGuessed(i) {

    var pos = sequenceGuessedCode.length;
    if (pos < 8) {
        if (i == 1) {
            sequenceGuessedCode += "A";
            sequenceSetImage(pos, "star");
        } else if (i == 2) {
            sequenceGuessedCode += "B";
            sequenceSetImage(pos, "4pointstar");
        } else if (i == 3) {
            sequenceGuessedCode += "C";
            sequenceSetImage(pos, "blueThing");
        } else if (i == 4) {
            sequenceGuessedCode += "D";
            sequenceSetImage(pos, "energy");
        } else if (i == 5) {
            sequenceGuessedCode += "E";
            sequenceSetImage(pos, "sparks");
        } else if (i == 6) {
            sequenceGuessedCode += "F";
            sequenceSetImage(pos, "cross");
        } else if (i == 7) {
            sequenceGuessedCode += "G";
            sequenceSetImage(pos, "city");
        } else if (i == 8) {
            sequenceGuessedCode += "H";
            sequenceSetImage(pos, "greenStar");
        } else if (i == 9) {
            sequenceGuessedCode += "I";
            sequenceSetImage(pos, "tree");
        } else if (i == 10) {
            sequenceGuessedCode += "J";
            sequenceSetImage(pos, "pinkDrops");
        } else if (i == 11) {
            sequenceGuessedCode += "K";
            sequenceSetImage(pos, "waterfall");
        } else if (i == 12) {
            sequenceGuessedCode += "L";
            sequenceSetImage(pos, "cell");
        } else if (i == 13) {
            sequenceGuessedCode += "M";
            sequenceSetImage(pos, "vines");
        } else if (i == 14) {
            sequenceGuessedCode += "N";
            sequenceSetImage(pos, "blueTarget");
        } else if (i == 15) {
            sequenceGuessedCode += "O";
            sequenceSetImage(pos, "3circs");
        }
    }

    if ((pos > 0) && (i == 16)) {
        sequenceGuessedCode = sequenceGuessedCode.substring(0, sequenceGuessedCode.length - 1);
        sequenceSetImage(pos - 1, "blank");
    }
}

function centerSequenceGameArea() {

    var windowWidth = $(window).width();
    var gameAreaWidth = $("#sequenceGameMainWindow").width();
    var bufferEachSide = ((windowWidth - gameAreaWidth) / 2 - 25) + "px";

    $("#sequenceGameMainWindow").css({ "margin-left": bufferEachSide });
    $("#sequenceGameMainWindow").css({ "margin-right": bufferEachSide });
}

function seqCalculateDifficulty() {
    if (gameDifficulty == "default") {

        var length = 3;

        // % of opponent transformed
        var numerator = parseFloat(npcCharLoader.characters[1].currentTFPoints);
        var denominator = parseFloat(npcCharLoader.characters[1].maxTFPoints);
        //var percent = 1-parseFloat(parseFloat(numerator)/parseFloat(denominator));

        // factor in opponent's intelligence
        intelBonus = parseFloat(npcCharLoader.characters[1].intelligence) * 3 / 100;

        // factor in player's intelligence (makes game easier/more netural), but to a lesser degree
        intelBonus -= parseFloat(humanCharLoader.characters[1].intelligence) * 2 / 100;


        length = Math.floor(3 + intelBonus + .5 * parseFloat(globalDifficultyModifierSequence));

        if (length > 8) { length = 8; }
        if (length < 3) { length = 3; }

        return length;

    } else if (gameDifficulty == "easy") {
        return 3;
    } else if (gameDifficulty == "medium") {
        return 4;
    } else if (gameDifficulty == "hard") {
        return 6;
    } else if (gameDifficulty == "insane") {
        return 8;
    } else if (gameDifficulty == "incremental") {
        return 5;
    }

    return 1;
}
