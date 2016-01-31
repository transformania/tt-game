function startdpongRound() {

    var tfsToDo;

    var dpongTimerFunc;
    var dpongAICheckFunc;

    var dpongRoundType = 0;

    var dpongTime = 1.5;
    var dpongNewBallTime = 5;
    var dpongNewBallReset = 5;
    
    var AIMissInterval = 30;

    var pScore = 100;
    var cScore = 100;
    var winPoints = 100;
    
    var NUM_BALLS = 8;
    var baseVelocity = 1;
    var lossPenalty = 20;
    
    var posX = new Array(0,0,0,0,0,0);
    var posY = new Array(0,0,0,0,0,0);
    var velX = new Array(0,0,0,0,0,0);
    var velY = new Array(0,0,0,0,0,0);
    var isActive = new Array(false, false, false, false, false, false);
    
    var paddleX = 250;
    var paddleY = 490;
    var paddleSide = "bottom";
    var paddleWidth = 125;
    
    var clickhandler;
    

    
    $("#dpongHideMe").show();

    var start = function () {
        
        
        centerMeToWindow($("#dpongGameMainWindow"));
        setMinigameNameplate($("#dpongGameMainWindow"),"Reflex:  Double Pong");
        $("#dpongActionWindow").css("background-image", "url(../images/background2.jpg)");
        dpongTimerFunc = setInterval("updatedpongTimer()", 10); // runs every 10 milliseconds (.1 seconds)
        dpongUpdateProgressBars();
        
        $("#dpongPaddle").css("left",paddleX-paddleWidth/2);
        $("#dpongPaddle").css("top",paddleY);
        $("#dpongPaddle").css("width",paddleWidth);
        

        if (gameSpeed == "slow") { tfsToDo = 1; }
        if (gameSpeed == "normal") { tfsToDo = 2; }
        if (gameSpeed == "fast") { tfsToDo = 3; }
        
        $(".dpongBall").hide();
        setDifficulty();
        

    }

    var endRoundLogic = function () {
        
        
        if (pScore > cScore) {
            writeMessage("You have <green>WON</green> a round!<br>");
            tfOpponent(tfsToDo);
            AjaxSubmit("Win Dpong");
            updateGlobalDifficultyModifier(1.35,"dpong");
            updateGlobalProgressBars();
        } else {
            writeMessage("You have <red>LOST</red> a round!<br>");
            tfHuman(tfsToDo);
            AjaxSubmit("Lose Dpong");
            updateGlobalDifficultyModifier(-.7,"dpong");
            updateGlobalProgressBars();
        }
        
        $('#dpongPaddleLeft').unbind('click');
        $('#dpongPaddleRight').unbind('click');
        $('#dpongPaddleAcross').unbind('click');
        
        clickhandler = null;
        
   

        $("#dpongGameMainWindow").hide();
        $("#grayBackground").hide();
        $("#helpButton").hide();
        $("#minigameNameplate").hide();
          
    }
    
    this.setDifficulty = function() {
        if (gameDifficulty=="easy") {
            baseVelocity = 1.0;
            dpongNewBallReset = 7;
            AIMissInterval = 40;
        } else if (gameDifficulty=="medium") {
            baseVelocity = 1.5;
            dpongNewBallReset = 5;
            AIMissInterval = 30;
        } else if (gameDifficulty=="hard") {
            baseVelocity = 2;
            dpongNewBallReset = 3.5;
            AIMissInterval = 25;
        } else if (gameDifficulty=="insane") {
            baseVelocity = 2.8;
            dpongNewBallReset = 2.5;
            AIMissInterval = 20;
        } else if (gameDifficulty=="default") {
            baseVelocity = 1 + .4*globalDifficultyModifierDpong;
            dpongNewBallReset = 5-.5*globalDifficultyModifierDpong;
            AIMissInterval = 50 - .28*parseFloat(npcCharLoader.characters[1].reflex);
            paddleWidth = 100 + .25*parseFloat(humanCharLoader.characters[1].reflex);
        }
    }

    this.updatedpongTimer = function () {
        
        dpongTime-= .01;
        dpongNewBallTime-=.01;

        if (dpongRoundType == 0) { // warmup stage

            if (dpongTime < 0) {
                dpongRoundType = 1;
               dpongTime = 25;
               dpongAICheckFunc = setInterval("dpongAICheck()", 250);
               //for (var i = 0; i < NUM_BALLS; i++) {placeBalls(i);}
               
               registerButtons();
               placeBalls(0);
               
               
            }

        } else if (dpongRoundType == 1) { // action stage
          
          moveBalls();
          
          if (dpongNewBallTime < 0) {
                // place a ball
                for (var z = 0; z < NUM_BALLS; z++) {
                    if (!isActive[z]) {
                        placeBalls(z);
                        break;
                    }
                }
                dpongNewBallTime = dpongNewBallReset;
          }
          
            if ((pScore <= 0) || (cScore <= 0)) {
                dpongRoundType = 2;
                dpongTime = 2;
                clearInterval(dpongAICheckFunc);
                
                $("#dpongHideMe").fadeOut(500);
        
                if (pScore > cScore) {
                    $("#dpongActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
                } else {
                    $("#dpongActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
                }
                
            }
        

        } else if (dpongRoundType == 2) { // cooldown stage
            if (dpongTime < 0) {
                
                clearInterval(dpongTimerFunc);
                
                endRoundLogic();

            }
        }

    }
    

    this.dpongAICheck = function () {
        if (Math.random()*1000 < AIMissInterval) {
            cScore-=lossPenalty;
            flashBorder("green");
            dpongUpdateProgressBars();
        }
        

    }
    
    this.registerButtons = function() {
        $("#dpongPaddleLeft").click(function(e) {
            movePaddleLeft();
        });
        $("#dpongPaddleRight").click(function(e) {
            movePaddleRight();
        });
        $("#dpongPaddleAcross").click(function(e) {
            movePaddleAcross();
        });
        

        clickhandler = document.addEventListener('keydown', function(event) {
            if(event.keyCode == 37) { movePaddleLeft(); }
            else if(event.keyCode == 38) { movePaddleAcross(); }
            else if(event.keyCode == 39) { movePaddleRight(); }
            else if(event.keyCode == 40) { movePaddleAcross(); }
            
            if ((pScore <=0) || (cScore <= 0)) {
                this.removeEventListener('keydown',arguments.callee,false);
            }
            
        });
        
        
    }
    
    this.movePaddleLeft = function() {
        if (paddleX > (paddleWidth/2+12)) {
                paddleX -= paddleWidth/2;
                $("#dpongPaddle").animate({"left" : paddleX-paddleWidth/2},50);
        }
        
    }
    
    this.movePaddleRight = function() {
        if (paddleX < (500-paddleWidth/2-18)) {
                paddleX += paddleWidth/2;
                $("#dpongPaddle").animate({"left" : paddleX-paddleWidth/2},50);
            }
    }
    
    this.movePaddleAcross = function() {
        if (paddleY == 490) {
              paddleY = 0;
            } else {
              paddleY = 490;
            }
            $("#dpongPaddle").animate({"top" : paddleY}, 50);
    }
    
    this.placeBalls = function(i) {
        
            isActive[i] = true;
            $(".dpongBall:eq(" + i + ")").show();
            
            posX[i] = 150 + Math.random()*150;
            posY[i] = 150 + Math.random()*150;
            velX[i] = .5-Math.random();
            velY[i] = 3*(.5-Math.random());
            
            //normalize x, y
            var length = Math.sqrt(velX[i]*velX[i] + velY[i]*velY[i]);
            velX[i] = velX[i] / length;
            velY[i] = velY[i] / length;
            
            velX[i]*=baseVelocity;
            velY[i]*=baseVelocity;
             
    }
    
    this.moveBalls = function() {
        
        for (var i = 0; i < NUM_BALLS; i++) {
            
            if (isActive[i]) {
            
                posX[i] = parseFloat(posX[i]) + parseFloat(velX[i]);
                posY[i] = parseFloat(posY[i]) + parseFloat(velY[i]);
            
                var newX = (posX[i] + "px").toString();
                var newY = (posY[i] + "px").toString();
                
                $(".dpongBall:eq(" + i + ")").css("left",newX);
                $(".dpongBall:eq(" + i + ")").css("top",newY);
                
                // out of bounds HORIZONTALLY... just bounce
                if ((posX[i]>485) || (posX[i]<0)) {
                    velX[i] = velX[i]*-1;
                    
                }
                
                // out of bounds VERTICALLY
                if ((posY[i]>480) || (posY[i]<10)) {
                    
                    var bCenterX = posX[i] + 7.5;
                    
                    var heightMatch = false;
                    if (((paddleY > 400) && (posY[i] > 400)) || ((paddleY < 100) && (posY[i] < 100))) {
                        heightMatch = true;
                    }
                    
                    var pLeftBound = paddleX - paddleWidth;
                    var pRightBound = paddleX + paddleWidth;
                    if ((bCenterX>pLeftBound) && (bCenterX<pRightBound) && heightMatch) {
                        // hit the paddle, so just bounce
                        velY[i] = velY[i]*-1;
                    } else {
                        // ball goes out of bounds
                        isActive[i] = false;
                        pScore -= lossPenalty;
                        $(".dpongBall:eq(" + i + ")").hide();
                        dpongUpdateProgressBars();
                        flashBorder("red");
                    }
                    
                }
            
            }
            
        }
       
      
        
    }
    
    this.dpongUpdateProgressBars = function () {
        
        var newWidthO = Math.floor(cScore/winPoints * 100);
        var newWidthP = Math.floor(pScore/winPoints * 100);

        if (newWidthO < 0 ) { newWidthO = 0; }
        if (newWidthP < 0 ) { newWidthP = 0; }

        newWidthO += "%";
        newWidthP += "%";

        newWidthO = newWidthO.toString();
        newWidthP = newWidthP.toString();

        $("#dpongOpponentScoreProgressBar").animate({ "width": newWidthO }, "fast");
        $("#dpongPlayerScoreProgressBar").animate({ "width": newWidthP }, "fast");


    this.flashBorder = function(color) {
    
        var value;
        if (color=="green") {
            value = "#00FF00";
        } else if (color=="red") {
            value = "#FF0000";
        }
        
        $('#dpongActionWindow').animate({borderColor: value}, 100, flashBorderBack);
        
        function flashBorderBack() {
            $('#dpongActionWindow').animate({borderColor: '#000000'}, 100);
        }
    
    }
   

}

 start();

}