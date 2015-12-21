function startRouletteRound() {

    var tfsToDo;

    var roulTimerFunc;
    var roulAICheckFunc;

    var roulRoundType = 0;

    var roulTime = 1.5;

    var pScore = 0;
    var winPoints = 100;
    
    var betsMax = 3;
    var remainingBets = 3;
    var betValue = "---";
    var cScore = "?";
    

    var currentSpeed = 0;
    var currentSpeedDecay = .0025;
    var currentRotation = 0;
    
   // $("#roulSpinnerFrame").rotate(-currentRotation);
    
    $("#roulHideMe").show();

    var start = function () {
        centerMeToWindow($("#roulGameMainWindow"));
        setMinigameNameplate($("#roulGameMainWindow"), "Prediction:  Roulette");
        $("#roulActionWindow").css("background-image", "url(../images/background2.jpg)");
        roulTimerFunc = setInterval("updateRoulTimer()", 10); // runs every 10 milliseconds (.1 seconds)
        
       

        if (gameSpeed == "slow") { tfsToDo = 1; }
        if (gameSpeed == "normal") { tfsToDo = 2; }
        if (gameSpeed == "fast") { tfsToDo = 3; }
        
        $("#roulBtnRed").html(0);
        $("#roulBtnGreen").html(0);
        $("#roulBtnBlue").html(0);
        $("#roulBtnYellow").html(0);
        
         //$("#roulSpinnerFrame").css("width", $("#roulSpinnerWheel".css("width")));
         //$("#roulSpinnerFrame").css("height", $("#roulSpinnerWheel".css("height")));
       // $("#roulSpinnerFrame").css("left", $("#roulSpinnerWheel".css("left")));
       // $("#roulSpinnerFrame").css("right", $("#roulSpinnerWheel".css("right")));
       
       var abc = $("#roulHideMe");
       var offset = abc.position();
       
       //alert(offset.top);
       
     
       $("#roulSpinnerFrame").css("top",offset.top);
       
       //var xoff = abc.offset().left;
       //var yoff = abc.offset().top;
       //
       // $("#roulSpinnerFrame").css("left", xoff);
       //$("#roulSpinnerFrame").css("top", yoff);
       
        currentRotation = Math.floor(Math.random()*360);
        $("#roulSpinnerWheel").rotate(currentRotation);
       

    }
    


    var roulSetDifficulty = function () {

        if (gameDifficulty == "easy") {
             cScore = Math.floor(Math.random()*40);
        } else if (gameDifficulty == "medium") {
             cScore = 10+Math.floor(Math.random()*60);
        } else if (gameDifficulty == "hard") {
            cScore = 20+Math.floor(Math.random()*80);
        } else if (gameDifficulty == "insane") {
           cScore = 30+Math.floor(Math.random()*90);
        } else if (gameDifficulty == "default") {
            
            cScore = Math.floor(Math.random()*75 + 8*globalDifficultyModifierRoulette);
            
            var skillMod = Math.floor(.3*(npcCharLoader.characters[1].prediction) - Math.floor(humanCharLoader.characters[1].prediction));
            
            if (skillMod > 0) {cScore += skillMod;}
            
            if (Math.random()*1000 < (50-globalDifficultyModifierRoulette*4)) {
                cScore = 0;
            }
            
                // clamp max CPU score at 115, which is about what human con attain by blind luck
            if (cScore > 115) {
                cScore = 115;
            }
        }

    }

    var endRoundLogic = function () {
        if (pScore > cScore) {
            writeMessage("You have <green>WON</green> a round!<br>");
            tfOpponent(tfsToDo);
            AjaxSubmit("Win Roulette");
            updateGlobalDifficultyModifier(1.00,"roulette");
            updateGlobalProgressBars();
        } else {
            writeMessage("You have <red>LOST</red> a round!<br>");
            tfHuman(tfsToDo);
            AjaxSubmit("Lose Roulette");
            updateGlobalDifficultyModifier(-.5,"roulette");
            updateGlobalProgressBars();
        }

        $("#roulGameMainWindow").hide();
        $("#grayBackground").hide();
        $("#helpButton").hide();
        $("#minigameNameplate").hide();
    }

    this.updateRoulTimer = function () {
        
        
        $("#roulDebug1").html(currentRotation);
        
        $("#roulInfo").html("Bets left: " + remainingBets + "<br>Current bet value:  " + betValue);
        $("#roulOpponentScore").html("Opponent's score to beat: " + cScore);

        roulTime -= .01;

        if (roulRoundType == 0) { // warmup stage

            if (roulTime < 0) {
                roulRoundType = 1;
               
               
               // set our spinning speed and begin rotation
               currentSpeed = 1+.3*globalDifficultyModifierRoulette + Math.random()*.05;
               
               $("#roulSpinnerWheel").rotate(currentRotation);
               
               // enable votes
               $("#roulBtnRed").click(function (e) {
                if (remainingBets > 0) {
                    var oldval = parseInt($("#roulBtnRed").html());
                    $("#roulBtnRed").html(oldval + betValue);
                    remainingBets--;
                }
                });
               $("#roulBtnGreen").click(function (e) {
                  if (remainingBets > 0) {
                    var oldval = parseInt($("#roulBtnGreen").html());
                    $("#roulBtnGreen").html(oldval + betValue);
                    remainingBets--;
                }
                });
               $("#roulBtnBlue").click(function (e) {
                   if (remainingBets > 0) {
                    var oldval = parseInt($("#roulBtnBlue").html());
                    $("#roulBtnBlue").html(oldval + betValue);
                    remainingBets--;
                }
                });
               $("#roulBtnYellow").click(function (e) {
                    if (remainingBets > 0) {
                    var oldval = parseInt($("#roulBtnYellow").html());
                    $("#roulBtnYellow").html(oldval + betValue);
                    remainingBets--;
                }
                });
               
               roulTime = 8;
               roulAICheckFunc = setInterval("roulAICheck()", 250); 
            }


        } else if (roulRoundType == 1) { // action stage
          
            //betValue = Math.floor(10*roulTime*(remainingBets/betsMax));
            betValue = Math.floor((15+Math.floor(30*(roulTime/8)))-3*(betsMax-remainingBets));
            
            currentRotation += currentSpeed;
            $("#roulSpinnerWheel").rotate(currentRotation);
           // $("#roulSpinnerFrame").rotate(-currentRotation); // keeps at 0...
        
            if (currentSpeed > 0) {
                currentSpeed -= currentSpeedDecay;
                if (currentSpeed < 0) {
                    currentSpeed = 0;
                   roulRoundType = 2;
                   roulTime = 1.5;
                   calculatePoints();
                }
            
            // no more voting time; disable betting buttons
            if (roulTime < 0) {
                $('#roulBtnRed').unbind('click');
                $('#roulBtnBlue').unbind('click');
                $('#roulBtnGreen').unbind('click');
                $('#roulBtnYellow').unbind('click');
                betValue = "---";
            }
        }

        } else if (roulRoundType == 2) { // cooldown stage
            betValue = "---";
            if (roulTime < 0) {
               
                clearInterval(roulTimerFunc);
                clearInterval(roulAICheckFunc);
                endRoundLogic();

            }
        }

    }
    
    

    this.roulAICheck = function () {

        if (((Math.random() < .05) || (currentSpeed < .2)) && (cScore=="?")) {
             roulSetDifficulty();
        }

    }
    
    
    var calculatePoints = function () {
        color = currentRotation%360;
        if (color <= 90) {
            pScore = $('#roulBtnGreen').html();
        } else if (color <= 180) {
           pScore = $('#roulBtnRed').html();
        } else if (color <= 270) {
           pScore = $("#roulBtnYellow").html();
        } else if (color <= 360) {
           pScore = $("#roulBtnBlue").html();
        }
        
        $("#roulHideMe").fadeOut("500");
        
        if (pScore > cScore) {
            $("#roulActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
        } else {
            $("#roulActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
        }
    }
    
    


    start();

}