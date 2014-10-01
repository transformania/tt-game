function startDiscusRound() {

    var tfsToDo;

    var discusTimerFunc;
    var discusAICheckFunc;
      var discusLaunchFunc;

    var discusRoundType = 0;

    var discusTime = .1;

    var pScore = 0;
    var cScore = 0;
    var cScoreCooldownInterval = 3;
    var winPoints = 70;
    var aiBonusProbability = 0;
    
    var pointsLarge = 10;
    var pointsMedium = 20;
    var pointsSmall = 35;
    
    
    var posX = new Array(0,0,0,0,0,0);
    var posY = new Array(0,0,0,0,0,0);
    var velX = new Array(0,0,0,0,0,0);
    var velY = new Array(0,0,0,0,0,0);
    var tWidth = new Array(0,0,0,0,0,0);
    var tHeight = new Array(0,0,0,0,0,0);
    var tValue = new Array(pointsMedium,pointsLarge,pointsLarge,pointsMedium,pointsLarge,pointsSmall);
    var isActive = new Array("false", "false", "false", "false", "false", "false", "false");
    
    var bposX;
    var bposY;
    var bvelX;
    var bvelY;
    var bisActive = "false";
    var bWidth;
    var bHeight;
    var bPrice = 2;
    
    var bSpeed = 5;

    var nextDiscToLaunch = 0;
    
     $(".discusTarget:eq(0)").css('width', '25px');
     $(".discusTarget:eq(0)").css('height', '25px');
     $(".discusTarget:eq(0)").css("background-image", "url(../images/discus_target2.png)");
     
      $(".discusTarget:eq(3)").css('width', '25px');
      $(".discusTarget:eq(3)").css('height', '25px');
      $(".discusTarget:eq(3)").css("background-image", "url(../images/discus_target2.png)");
       
       $(".discusTarget:eq(5)").css('width', '15px');
       $(".discusTarget:eq(5)").css('height', '15px');
       $(".discusTarget:eq(5)").css("background-image", "url(../images/discus_target3.png)");
    
     $(".discusTarget").hide();
     $(".discusBullet").hide();
     
    $("#discusHideMe").show();

    var start = function () {
        
        centerMeToWindow($("#discusGameMainWindow"));
         setMinigameNameplate($("#discusGameMainWindow"),"Prediction:  Discus");
         $("#discusActionWindow").css("background-image", "url(../images/background2.jpg)");
        discusTimerFunc = setInterval("updateDiscusTimer()", 10); // runs every 10 milliseconds (.1 seconds)
        
        
        // assign some variables once...
        for (var i = 0; i < 6; i ++) {
            tWidth[i] = parseFloat($(".discusTarget:eq(" + i + ")").css("width"));
            tHeight[i] = parseFloat($(".discusTarget:eq(" + i + ")").css("height"));
        }
        bWidth = parseFloat($(".discusBullet").css("width"));
        bHeight = parseFloat($(".discusBullet").css("height"));
        
        discusUpdateProgressBars();
        
        if (gameSpeed == "slow") { tfsToDo = 1; }
        if (gameSpeed == "normal") { tfsToDo = 2; }
        if (gameSpeed == "fast") { tfsToDo = 3; }
        calculateDifficulty();
        
        $("#discusMissPenalty").html("Bullet price:  " + bPrice.toFixed(3));
       

    }

    var endRoundLogic = function () {
        
        
        if (pScore > cScore) {
            writeMessage("You have <green>WON</green> a round!<br>");
            tfOpponent(tfsToDo);
            AjaxSubmit("Win Discus");
            updateGlobalDifficultyModifier(1.5,"discus");
            updateGlobalProgressBars();
        } else {
            writeMessage("You have <red>LOST</red> a round!<br>");
            tfHuman(tfsToDo);
            AjaxSubmit("Lose Discus");
            updateGlobalDifficultyModifier(-.7,"discus");
            updateGlobalProgressBars();
        }

        $("#discusGameMainWindow").hide();
        $("#grayBackground").hide();
        $("#helpButton").hide();
        $("#minigameNameplate").hide();
        
       
    }

    this.updateDiscusTimer = function () {
        
        discusTime-=.01;
        cScoreCooldownInterval -=.01;
    

        if (discusRoundType == 0) { // warmup stage

            if (discusTime < 0) {
                discusRoundType = 1;
               discusTime = 25;
               discusAICheckFunc = setInterval("discusAICheck()", 250);
               discusLaunchFunc = setInterval("discusLaunch()", 2500);
               
               $("#discusActionWindow").click(function(e) {
                     
                    var offset = $("#discusActionWindow").offset();
                    var x = e.pageX - offset.left;
                    var y = e.pageY - offset.top;
                    
                    fireBullet(x, y);
                    
                });
               
            }

        } else if (discusRoundType == 1) { // action stage
            
            moveDiscsAndBullet();
          
            if ((pScore >= winPoints) || (cScore >= winPoints)) {
                 $('#discusActionWindow').unbind('click');
                discusRoundType = 2;
                discusTime = 2;
                
                $("#discusHideMe").fadeOut("500");
        
                if (pScore > cScore) {
                    $("#discusActionWindow").css("background-image", "url(../images/reflex_win.jpg)");
                } else {
                    $("#discusActionWindow").css("background-image", "url(../images/reflex_lose.jpg)");
                }
                
                clearInterval(discusAICheckFunc);
            }
        

        } else if (discusRoundType == 2) { // cooldown stage
            if (discusTime < 0) {
                
                clearInterval(discusTimerFunc);
                
                clearInterval(discusLaunchFunc);
                endRoundLogic();

            }
        }

    }
    
    var moveDiscsAndBullet = function() {
        
         
        
        for (var i = 0; i < 6; i++ ) {
            
            if (isActive[i]=="true") {
             
            
            posX[i] = parseFloat(posX[i]) + parseFloat(velX[i]);
            posY[i] = parseFloat(posY[i]) + parseFloat(velY[i]);
            
            velY[i] += .009; // "gravity"
            
            var newX = (posX[i] + "px").toString();
            var newY = (posY[i] + "px").toString();
            
            if (outOfFrame(i)) {
                
            } else {
                // move target as normal
                $(".discusTarget:eq(" + i + ")").css('left', newX);
                $(".discusTarget:eq(" + i + ")").css('top', newY);
            
            }
            
            } 
           
        }
        
        // move our bullet...
        
        if (bisActive=="true") {
            bposX = parseFloat(bposX) + parseFloat(bvelX);
            bposY = parseFloat(bposY) + parseFloat(bvelY);
            var newX = (bposX + "px").toString();
            var newY = (bposY + "px").toString();
            $(".discusBullet").css('left', newX);
            $(".discusBullet").css('top', newY);
            
            bulletDestroy();
        } 
        
          
    }
    
    
    

    this.discusAICheck = function () {

        if (cScoreCooldownInterval < 0) {
            var roll = Math.random()*1000;
            
            if ((roll > 850) && (cScore > 0)){
                cScore -= 3;
                discusUpdateProgressBars();
                cScoreCooldownInterval = 3;
            }
            
            if (roll < 45 + aiBonusProbability) {
                if (roll < 9 + aiBonusProbability) {
                    cScore += pointsSmall;
                } else if (roll < 22 + aiBonusProbability){
                    cScore += pointsMedium;
                } else if (roll < 45 + aiBonusProbability){
                    cScore += pointsLarge;
                }
             
                discusUpdateProgressBars();
                flashBorder("red");
                cScoreCooldownInterval = 3;
            }
        }
        
        

    }
    
    this.fireBullet = function(x,y) {
        
        
       
       if (bisActive=="false") {
            pScore -= bPrice;
            if (pScore < 0) {pScore = 0;}
            discusUpdateProgressBars();
            bposX = 250;
            bposY = 500;
            // normalize 
            bvelX = bposX - x;
            bvelY = bposY - y;
            bvelY += .009; // "gravity"
            var length = Math.sqrt(bvelX*bvelX + bvelY*bvelY);
            bvelX = -bSpeed * (bvelX / length);
            bvelY = -bSpeed * (bvelY / length);
            bisActive = "true";
            $(".discusBullet").show();
       }
    }
    
    this.discusLaunch = function () {
        
        var i = nextDiscToLaunch;
    
        if (isActive[i]=="false") {
            
            isActive[i]="true";
            
            
            posX[i] = 0;
            posY[i] = 250;
            
            velX[i] = 1+Math.random()*1;
            velY[i] = -1 - Math.random()*1;
            
            $(".discusTarget:eq(" + i + ")").show();
            
        }
        
        nextDiscToLaunch = (nextDiscToLaunch + 1) % 6;
    }
    
    this.outOfFrame = function(i) {
        if ((posX[i] > 500) ||  (posY[i] > 500)) {
            isActive[i] = "false";
            
            $(".discusTarget:eq(" + i + ")").hide();
           return true;
        } else {
            return false;
        }
    }
    
    this.bulletDestroy = function() {
        if ((bposX < 0) || (bposX > 500) ||  (bposY < 0)) {
            
            $(".discusBullet").hide();
            bisActive = "false";
        }
        
        for (var i = 0 ; i  < 6; i++) {
            
            if (isActive[i]) {
            
            var bcenterX = bposX + .5*bWidth;
            var bcenterY = bposY + .5*bHeight;
            var tcenterX = posX[i] + .5*tWidth[i];
            var tcenterY = posY[i] + .5*tHeight[i];
            var distance = Math.sqrt((bcenterX - tcenterX)*(bcenterX - tcenterX) + (bcenterY - tcenterY)*(bcenterY - tcenterY));
            var hitDistance = (bWidth + tWidth[i]) / 2;
            
            // target hit; destroy it and bullet, add score
            if (distance < hitDistance) {
               $(".discusBullet").hide();
                bisActive = "false";
                
                $(".discusTarget:eq(" + i + ")").hide();
                isActive[i] = "false";
                posX[i] = -50;
                posY[i] = 250;
                pScore+= tValue[i] + bPrice*.5;
                flashBorder("green");
                discusUpdateProgressBars();
            }
            
          
            
            }
            
        }
        

        
        
        
    }
    
    var flashBorder = function(color) {
    
        var value;
        if (color=="green") {
            value = "#00FF00";
        } else if (color=="red") {
            value = "#FF0000";
        }
        
        $('#discusActionWindow').animate({borderColor: value}, 100, flashBorderBack);
        
        function flashBorderBack() {
            $('#discusActionWindow').animate({borderColor: '#000000'}, 100);
        }
    
    }
    
    var discusUpdateProgressBars = function () {
        var newWidthO = Math.floor(cScore/winPoints * 100);
        var newWidthP = Math.floor(pScore/winPoints * 100);

        if (newWidthO > 100) { newWidthO = 100; }
        if (newWidthP > 100) { newWidthP = 100; }

        newWidthO += "%";
        newWidthP += "%";

        newWidthO = newWidthO.toString();
        newWidthP = newWidthP.toString();



        $("#discusOpponentScoreProgressBar").animate({ "width": newWidthO }, "fast");
        $("#discusPlayerScoreProgressBar").animate({ "width": newWidthP }, "fast");


    }
    
    var calculateDifficulty = function() {
        
        
        if (gameDifficulty=="easy") {
            bSpeed = 4;
            aiBonusProbability = 0;
            bPrice = 0;
        } else if (gameDifficulty=="medium") {
            bSpeed = 3.5;
            aiBonusProbability = 8;
            bPrice = 4;
        } else if (gameDifficulty=="hard") {
            bSpeed = 3;
            aiBonusProbability = 16;
            bPrice = 8;
        } else if (gameDifficulty=="insane") {
            bSpeed = 2.5;
            aiBonusProbability = 26;
            bPrice = 12;
        } else if (gameDifficulty=="default") {
            bSpeed = 3 + parseFloat(humanCharLoader.characters[1].prediction) / 33;
            aiBonusProbability = (parseFloat(npcCharLoader.characters[1].prediction) / 4.66) + globalDifficultyModifierDiscus*2.5;
            bPrice = globalDifficultyModifierDiscus * 1.5;
        }
    }
    
    start();

}