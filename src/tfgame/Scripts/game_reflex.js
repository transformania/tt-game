// TODO:  add PLAYER and OPPONENT progress bars and remove the timer so it's more like player is playing
// TODO:  keep a "true" x and y position to enable non-integer movements
// TODO:  set random direction to be normalized
// again a realtime opponent


var reflexRoundType = 0;
var reflexTime = 10;
var reflexCooldownTime = 0;
var reflexCooldownTimeAI = 0;
var reflexPoints = 0;
var reflexPointsAI = 0;

var reflexSpeedBoost = 1;

var truex;
var truey;
var dirx = 1;
var diry = 1;

var reflexTimerFunc;
var reflexAIcheckFunc;

var reflexRequiredWinPoints = 5;

function startReflexRound() {
    
    reflexTimerFunc = setInterval("updateReflexTimer()",10); // runs every 10 milliseconds (.1 seconds)
    $("#reflexActionWindow").css("background-image" , "url(../images/background2.jpg)");
    
    reflexTime = 1;
    reflexPoints = 0;
    reflexPointsAI = 0;
    updateProgressBars();
    setRandomTargetPosition();
    centerMeToWindow($("#reflexGameMainWindow"));
    setMinigameNameplate($("#reflexGameMainWindow"),"Reflex:  Moving Target");
    
    // hide the target until round actually starts
    $(".reflexTarget").hide();
    
        $(".reflexTarget").click(function(e) {
            
            // player hits target within valid cooldown time
            if (reflexCooldownTime < 0) {
            
                reflexPoints++;
                reflexCooldownTime = 1;
                
                // get new position / direction
                setRandomTargetDirection();
                setRandomTargetPosition();
                
                updateProgressBars();
                
                flashBorder("green");

            }
        });
     
}

function endReflexRoundLogic() {
    
    var tfsToDo;
    if (gameSpeed=="slow") {tfsToDo = 1;}
    if (gameSpeed=="normal") {tfsToDo = 2;}
    if (gameSpeed=="fast") {tfsToDo = 3;}
    
    if (reflexPoints >= reflexRequiredWinPoints) {
        writeMessage("You have <green>WON</green> a round!<br>");
        reflexSpeedBoost = reflexSpeedBoost + .25;
        tfOpponent(tfsToDo);
        AjaxSubmit("Win Target");
       updateGlobalDifficultyModifier(1.25,"reflex");
    } else {
        writeMessage("You have <red>LOST</red> a round!<br>");
        if (reflexSpeedBoost > 1) { reflexSpeedBoost = reflexSpeedBoost - .25; }
        tfHuman(tfsToDo);
        AjaxSubmit("Lose Target");
        updateGlobalDifficultyModifier(-.6,"reflex");
    }
    
    $("#reflexGameMainWindow").hide();
    $("#grayBackground").hide();
    $("#helpButton").hide();
    clearInterval(reflexTimerFunc);
    
    updateGlobalProgressBars();
    $("#minigameNameplate").hide();
    
}

function updateReflexTimer() {
    
    reflexTime = reflexTime - .01;
    reflexCooldownTime = reflexCooldownTime - .01;
    reflexCooldownTimeAI = reflexCooldownTimeAI - .01;
    
    if (reflexRoundType==0) {
        if (reflexTime < 0 ) {
            reflexTime = 10;
            reflexRoundType = 1;
            $(".reflexTarget").show();
            setRandomTargetDirection();
            setRandomTargetPosition();
            reflexAIcheckFunc = setInterval("reflexAIscoreCheck()",250); // runs every 10 milliseconds (.1 seconds)
            
        }
    } else if (reflexRoundType==1) {
        
        var target = $(".reflexTarget");
        
        truex = truex + dirx;
        truey = truey + diry;
        
        target.css('left', truex);
        target.css('top', truey);
        
        //updateProgressBars();
        
        if (targetIsOutOfBounds()) {
            setRandomTargetDirection();
            setRandomTargetPosition();
        }
        
        // if the round is out of time OR the player has enough points, begin end round logic
        if ((reflexPointsAI >= reflexRequiredWinPoints) || (reflexPoints >= reflexRequiredWinPoints)) {
            reflexRoundType++;
            reflexTime = 1.5;
            target.hide();
            clearInterval(reflexAIcheckFunc);
            
            // set the background image for the game window to match win/loss
            if (reflexPoints > reflexPointsAI) {
                $("#reflexActionWindow").css("background-image" , "url(../images/reflex_win.jpg)");
            } else {
                $("#reflexActionWindow").css("background-image" , "url(../images/reflex_lose.jpg)");
            }
            
        }        
        
    } else if (reflexRoundType==2) {
        
        if (reflexTime < 0) {
            endReflexRoundLogic();
            reflexRoundType = 0;
        }
    }
}

function setRandomTargetPosition() {
    var target = $(".reflexTarget");
    
    // find somewhere NOT within 100 pixels of a border.  This is why the numbers do
    // NOT add up to 500.
    truex = Math.random()*300+100;
    truey = Math.random()*300+100;
    
    target.css('left', truex);
    target.css('top', truey);
    
}

// calculate and set a random direction for the target to move
function setRandomTargetDirection() {
    
    dirx = Math.random()*2-1;
    diry = Math.random()*2-1;
    
    //normalize x, y
    var length = Math.sqrt(dirx*dirx + diry*diry);
    dirx = dirx / length;
    diry = diry / length;
    
    if ((gameDifficulty=="default") || (gameDifficulty=="incremental")) {
        dirx = dirx*(1+.25*globalDifficultyModifierReflex);
        diry = diry*(1+.25*globalDifficultyModifierReflex);
    } else if (gameDifficulty=="easy") {
        dirx = dirx*.7;
        diry = diry*.7;
    } else if (gameDifficulty=="medium") {
        dirx = dirx*1.0;
        diry = diry*1.0;
    } else if (gameDifficulty=="hard") {
        dirx = dirx*1.5;
        diry = diry*1.5;
    } else if (gameDifficulty=="insane") {
        dirx = dirx*2.25;
        diry = diry*2.25;
    }
    
}

function targetIsOutOfBounds() {
    
    var modifier = 15;
    
    var target = $(".reflexTarget");
    var x = $(".reflexTarget").css("left");
    var y = $(".reflexTarget").css("top");
    var newx = parseInt(x, 10);
    var newy = parseInt(y, 10);
    
    if (( newx < 0) || (newx > 500-modifier) || (newy < 0) || (newy > 500-modifier)) {
        return true;
    } else {
        return false;
    }
    
}

// FUNCTION:  have the player/AI progress bars stretch to show points toward win
function updateProgressBars() {
    
    var newWidthO = (((parseInt(reflexPointsAI, 10) / reflexRequiredWinPoints)*100)) + "%";
    var newWidthP = (((parseInt(reflexPoints, 10) / reflexRequiredWinPoints)*100)) + "%";
    
    newWidthO = newWidthO.toString();
    newWidthP = newWidthP.toString();
    
    $("#reflexOpponentScoreProgressBar").animate( {"width" : newWidthO }, "fast" );
    $("#reflexPlayerScoreProgressBar").animate( {"width" : newWidthP }, "fast" );
    
}

function reflexAIscoreCheck() {
    
    if (Math.random() < .25) {
        if (reflexCooldownTimeAI < 0) {
            reflexPointsAI++;
            reflexCooldownTimeAI = 1;
            updateProgressBars();
            flashBorder("red");
        }
    }
}

function flashBorder(color) {
    
    var value;
    if (color=="green") {
        value = "#00FF00";
    } else if (color=="red") {
        value = "#FF0000";
    }
    
    $('#reflexActionWindow').animate({borderColor: value}, 100, flashBorderBack);
    
    function flashBorderBack() {
        $('#reflexActionWindow').animate({borderColor: '#000000'}, 100);
    }
    
}



function centerReflexGameArea() {
    
    var windowWidth = $(window).width();
    var gameAreaWidth = $("#reflexGameMainWindow").width();
    var bufferEachSide = ((windowWidth - gameAreaWidth)/2 - 25) + "px";
    
    $("#reflexGameMainWindow").css({"margin-left" : bufferEachSide});
    $("#reflexGameMainWindow").css({"margin-right" : bufferEachSide});
    
}

function setReflexDifficulty() {
    if (gameDifficulty=="Default") {
        
    }
}

function reflexResetVariables() {
    reflexSpeedBoost = 1;
}

