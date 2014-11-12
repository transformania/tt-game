//@charset "UTF-8";

if (window.File && window.FileList && window.FileReader) {
    // document.getElementById('readFile').addEventListener('click', function(event) {
    // readFile();    
    //}, false);
}

var nameEnterable = false;

var newline = "&#10";

var gameDifficulty = "default";
var gameSpeed = "normal";

var questionGlobal;
var hasLost = false;
var tfsThisRoundAI = 1;
var tfsThisRoundHuman = 1;

var nameFormHack = "";

var skippingIsEnabled = false;

var gameTimerObject = document.getElementById("timerIn");

var pageJustLoaded = true;

var gameCharLoadType = "computer";
var globalDifficultyModifier = 0;
var globalDifficultyModifierDiscus = 1;
var globalDifficultyModifierDpong = 1;
var globalDifficultyModifierMemory = 1;
var globalDifficultyModifierPopup = 1;
var globalDifficultyModifierReflex = 1;
var globalDifficultyModifierRoulette = 1;
var globalDifficultyModifierSequence = 1;

var MAX_PARTS = 23;
var NUM_MINIGAMES = 7;
var miniGamesPlayed = new Array();

var waitForCharacterLoadTime = 500;

var messagesToWriteArray = new Array();
var indexMessagesToWriteArray = 0;
var indexLastWrittenMessage = 0;

var genData;

var newTfParts;

var miniGameType = 5;

var lastPartToTfSelected = 0;

var devModeOn = false;
var devModeEverOn = false;

// character loaders
var npcCharLoader;
var humanCharLoader;
var allCharLoader;
var sorter;
var humans;
var opponents;

var submittedAnswer;

var currentAnswerPos = 0;

var gameOver = false;

//setInterval("updateTimer()",10); // runs every 10 milliseconds (.1 seconds)
setInterval("innerWriteMessage()", 750); // runs every 300 milliseconds (.3 seconds)

function enterName() {

    if (humanCharLoader.characters[1].firstName != "MISSING") {

        var name = humanCharLoader.characters[1].firstName + " " + humanCharLoader.characters[1].lastName;
        //name = "<textarea>" + name + "</textarea>";
        document.getElementById("nameBox").innerHTML = name;

    }
}

function clearOutput() {
    document.getElementById("console").innerHTML = "";

}


function setupNextRound() {

    if (gameOver == true) {
        alert("The gameshow is over.  Perhaps you'd care for another game?  There are plenty of other contestants...");

    } else {

        //alert(miniGamesPlayed[0] + "    " + miniGamesPlayed[1] + "    " + miniGamesPlayed[2] + "    " + miniGamesPlayed[3] + "    " + miniGamesPlayed[4] + "    " + miniGamesPlayed[5]);

        miniGameType = Math.floor(Math.random() * NUM_MINIGAMES);
        while (miniGamesPlayed[miniGameType] == "played") {
            miniGameType = Math.floor(Math.random() * NUM_MINIGAMES);

            if (allMiniGamesHaveBeenPlayed()) {
                for (var z = 0; z < NUM_MINIGAMES; z++) {
                    miniGamesPlayed[miniGameType] = "undefined";
                }
            }

        }



        //if (miniGamesPlayed[miniGameType]=="played") {
        //miniGameType = Math.floor(Math.random()*NUM_MINIGAMES);
        //} else {
        miniGamesPlayed[miniGameType] = "played";

        //}

        if (allMiniGamesHaveBeenPlayed()) {
            for (var z = 0; z < NUM_MINIGAMES; z++) {
                miniGamesPlayed[z] = "undefined";
            }
        }

        //miniGameType = 4;

        if (miniGameType == 0) {
            $("#sequenceGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startSequenceRound();

        } else if (miniGameType == 1) {
            $("#reflexGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startReflexRound();

        } else if (miniGameType == 2) {
            $("#memoryGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startMemoryRound();

        } else if (miniGameType == 3) {
            $("#popupGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startPopupRound();

        } else if (miniGameType == 4) {
            $("#roulGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startRouletteRound();

        } else if (miniGameType == 5) {
            $("#discusGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startDiscusRound();

        } else if (miniGameType == 6) {
            $("#dpongGameMainWindow").show();
            $("#grayBackground").show();
            $("#helpButton").show();
            startdpongRound();

        }

    }



}

function resetAll() {

    if (confirm("Are you sure you want to restart?")) {

        dropdownOpp = document.getElementById('loadedOpponentsList');
        dropdownPlayer = document.getElementById('loadedPlayersList');

        var playerIndex = dropdownPlayer.selectedIndex;
        var computerIndex = dropdownOpp.selectedIndex;

        gameOver = false;
        messagesToWriteArray = new Array();
        indexMessagesToWriteArray = 0;
        indexLastWrittenMessage = 0;

        //if (( humans.characters[playerIndex].firstName!="MISSING" ) && (opponents.characters[computerIndex].firstName!="MISSING" )) {

        try {
            //clearAnswerBox();
            document.getElementById("allCharSubmitField").value = "";
            nameFormHack = "";
            clearOutput();
            submittedAnswer = "";

            //document.questionForm.questionConsole.value = "";
            allCharLoader = new CharacterLoader();
            npcCharLoader = new CharacterLoader();
            humanCharLoader = new CharacterLoader();
            submitAIData();
            submitHumanData();

            setCharacters();

            writeGameIntroductionMessage();
            openNewGameMenu();
            document.body.scrollTop = document.documentElement.scrollTop = 0;
            enterName();

            newTfParts = npcCharLoader.characters[1].getTransformableParts();
            setTransformableParts(newTfParts);

            setDifficulty($('#difficultyChoice input:checked').val());
            setGameSpeed($('#gameSpeedChoice input:checked').val());

            closeNewGameMenu();
            updateGlobalProgressBars();

            reflexResetVariables();
            globalDifficultyModifier = 0;
            globalDifficultyModifierDiscus = 1;
            globalDifficultyModifierDpong = 1;
            globalDifficultyModifierMemory = 1;
            globalDifficultyModifierPopup = 1;
            globalDifficultyModifierReflex = 1;
            globalDifficultyModifierRoulette = 1;
            globalDifficultyModifierSequence = 1;


            miniGameType = 0;

        } catch (err) {
            alert("You must select characters before starting a new game!" + err);
        }
    }
}

function firstPageLoad() {

    document.getElementById("debugCharacterButton").style.visibility = "hidden";
    document.getElementById("debugHumanButton").style.visibility = "hidden";

    $("#grayBackground").hide();
    $("#newGameWindow").hide();
    $("#helpWindow").hide();
    $("#reflexGameMainWindow").hide();
    $("#sequenceGameMainWindow").hide();
    $("#reflexGameMainWindow").hide();
    $("#memoryGameMainWindow").hide();
    $("#popupGameMainWindow").hide();

    resizeDivs();

    // load up our two generic characters
    sorter = new CharacterLoader();
    allCharLoader = new CharacterLoader();
    npcCharLoader = new CharacterLoader();
    humanCharLoader = new CharacterLoader();
    humans = new CharacterLoader();
    opponents = new CharacterLoader();

    document.getElementById("allCharSubmitField").value == "";


    //document.getElementById("enterNameButton").disabled = false;
    nameFormHack = "";
    clearOutput();
    submittedAnswer = "";
    //document.timerForm.timerConsole.value = "--.--";


    writeMessage("<i>If you want to play as or against a different character,  click 'New Game' and load the character scripts located in the Characters folder.</i>");

    if (document.getElementById("allCharSubmitField").value == "") {
        var genData = new GenericCharData();
        var genData2 = new GenericHumanData();
        var data = genData.getGenericCharacterText();
        data += genData2.getGenericHumanText();
        sorter.loadData(data);
        listLoadedCharacters();
        setCharacters();

    }

    writeGameIntroductionMessage();
    enterName();

    var newTfParts = npcCharLoader.characters[1].getTransformableParts();
    setTransformableParts(newTfParts);

    updateGlobalProgressBars();

    try {
        loadAllCharactersWithAjax();
    } catch (exception) {

    }

    //preload round selection
    for (var w = 0; w < NUM_MINIGAMES; w++) {
        miniGamesPlayed[w] = "undefined";
    }


}

function setDifficulty(choice) {
    gameDifficulty = choice;
}

function setGameSpeed(choice) {
    gameSpeed = choice;
}

function setTransformableParts(newTfParts) {

    var menu = document.getElementById('tfablePartsSelect');

    menu.length = 0;

    var option = document.createElement("option");
    option.text = "random";
    menu.add(option, menu.options[null]);

    for (var s in newTfParts) {
        //alert(newTfParts[s]);
        option = document.createElement("option");
        option.text = newTfParts[s];



        menu.add(option, menu.options[null]);
    }

    var index = -1;
    for (var i = 0; i < menu.length; i++) {

        if (menu[i].value == lastPartToTfSelected) {
            index = i;
        }

    }

    if (index != -1) {
        menu.selectedIndex = index;
    }

    // swap custom variables names for their aliases
    $('select option:contains("custom1")').text(npcCharLoader.characters[1].aliases[1]);
    $('select option:contains("custom2")').text(npcCharLoader.characters[1].aliases[2]);
    $('select option:contains("custom3")').text(npcCharLoader.characters[1].aliases[3]);
    $('select option:contains("custom4")').text(npcCharLoader.characters[1].aliases[4]);
    $('select option:contains("custom5")').text(npcCharLoader.characters[1].aliases[5]);

}

// FUNCTION:  transforms an opponent 'oount' times.
function tfOpponent(count) {

    var starvationDetectorCount = 0; // if we loop too many times in the for loop there is probably starvation in the script via [WaitFor]

    for (var i = 0; i < count; i++) {

        if (starvationDetectorCount > 2000) {
            alert("Starvation probable.  Canceling transformation.  Check the AI script for [WaitFor] cycles causing this.");
            break;
        }

        // if the character is already %100 transformed, break out of the loop
        if (npcCharLoader.characters[1].charIsFullyTransformed() == true) {
            writeMessage(npcCharLoader.characters[1].losedescription + "<br><br>");
            alert("This character is fully transformed!  You win!");
            AjaxSubmit("AI full TF");
            gameOver = true;
            break;
        }

        var seed = Math.floor(Math.random() * MAX_PARTS)

        var type = "XXX";

        var menu = document.getElementById('tfablePartsSelect');
        if (menu.value != "random") {
            type = menu.value;
            lastPartToTfSelected = type;
        } else {
            if (seed == 0) {
                type = "hair";
            } else if (seed == 1) {
                type = "face";
            } else if (seed == 2) {
                type = "voice";
            } else if (seed == 3) {
                type = "breasts";
            } else if (seed == 4) {
                type = "body";
            } else if (seed == 5) {
                type = "shoulders";
            } else if (seed == 6) {
                type = "waist";
            } else if (seed == 7) {
                type = "butt";
            } else if (seed == 8) {
                type = "legs";
            } else if (seed == 9) {
                type = "feet";
            } else if (seed == 10) {
                type = "hands";
            } else if (seed == 11) {
                type = "arms";
            } else if (seed == 12) {
                type = "groin";
            } else if (seed == 13) {
                type = "mind";
            } else if (seed == 14) {
                type = "shirt";
            } else if (seed == 15) {
                type = "pants";
            } else if (seed == 16) {
                type = "shoes";
            } else if (seed == 17) {
                type = "accessory";
            } else if (seed == 18) {
                type = "custom1";
            } else if (seed == 19) {
                type = "custom2";
            } else if (seed == 20) {
                type = "custom3";
            } else if (seed == 21) {
                type = "custom4";
            } else if (seed == 22) {
                type = "custom5";
            } else {
                alert("bad seed.");
            }
        }

        var doChange = npcCharLoader.characters[1].partIsDone(type);

        if (!doChange) {

            var okay = npcCharLoader.characters[1].incrementStage(type);

            if (okay == 1) {
                var out = npcCharLoader.characters[1].writeCurrentTF(type);

                if (out == "") {
                    alert("failed to change. " + type);
                }


                checkAndSwapGender(npcCharLoader.characters[1]);

                writeMessage("&#9830; " + out);

            } else if (okay == -1) {

                i = i - 1;
            }

        } else {

            i = i - 1;
        }

        starvationDetectorCount++;
        newTfParts = npcCharLoader.characters[1].getTransformableParts();
        setTransformableParts(newTfParts);
    }

}

function tfHuman(count) {

    

    var starvationDetectorCount = 0; // if we loop too many times in the for loop there is probably starvation in the script via [WaitFor]

    for (var i = 0; i < count; i++) {

        if (starvationDetectorCount > 2000) {
            alert("Starvation probable.  Canceling transformation.  Check the AI script for [WaitFor] causing this.");
            break;
        }

        // if the human is already %100 transformed, break out of the loop
        if (humanCharLoader.characters[1].charIsFullyTransformed() == true) {
            writeMessage(humanCharLoader.characters[1].losedescription + "<br><br>");
            alert("You are already fully transformed!  You lose!");
            AjaxSubmit("Human full TF");
            gameOver = true;
            break;
        }

        var seed = Math.floor(Math.random() * MAX_PARTS);

        var type = "XXX";

        if (seed == 0) {
            type = "hair";
        } else if (seed == 1) {
            type = "face";
        } else if (seed == 2) {
            type = "voice";
        } else if (seed == 3) {
            type = "breasts";
        } else if (seed == 4) {
            type = "body";
        } else if (seed == 5) {
            type = "shoulders";
        } else if (seed == 6) {
            type = "waist";
        } else if (seed == 7) {
            type = "butt";
        } else if (seed == 8) {
            type = "legs";
        } else if (seed == 9) {
            type = "feet";
        } else if (seed == 10) {
            type = "hands";
        } else if (seed == 11) {
            type = "arms";
        } else if (seed == 12) {
            type = "groin";
        } else if (seed == 13) {
            type = "mind";
        } else if (seed == 14) {
            type = "shirt";
        } else if (seed == 15) {
            type = "pants";
        } else if (seed == 16) {
            type = "shoes";
        } else if (seed == 17) {
            type = "accessory";
        } else if (seed == 18) {
            type = "custom1";
        } else if (seed == 19) {
            type = "custom2";
        } else if (seed == 20) {
            type = "custom3";
        } else if (seed == 21) {
            type = "custom4";
        } else if (seed == 22) {
            type = "custom5";
        } else {
            alert("bad seed.");
        }

        var doChange = humanCharLoader.characters[1].partIsDone(type);

        if (!doChange) {
            var okay = humanCharLoader.characters[1].incrementStage(type);

            if (okay == 1) {
                var out = humanCharLoader.characters[1].writeCurrentTF(type);

                if (out == "undefined<br>") {
                    //alert("failed to change. " + type);
                }


                checkAndSwapGender(humanCharLoader.characters[1]);

                writeMessage("&#9824; " + out);

            } else if (okay == -1) {

                i = i - 1;
            }

        } else {
            // adding on to i for a redo cycle
            i = i - 1;
        }

        starvationDetectorCount++;

        

    }
}




// FUNCTION:  Writes a message to the console in increments of .5 seconds
function writeMessage(text) {
    text = "<p>" + text + "</p>";
    messagesToWriteArray[indexMessagesToWriteArray] = text;
    indexMessagesToWriteArray++;

}

// FUNCTION:  Writes a message to the console all at once
function writeMessageImmediate(text, noScroll) {
    var oldheight = document.getElementById("console").scrollTop;

    document.getElementById("console").innerHTML = document.getElementById("console").innerHTML + text;

    if (noScroll == "noScroll") {

        var myDiv = document.getElementById('console');
        var h = myDiv.style.height + "px";

        // jump down to the bottom of the scroll box.  Later, jump back up to last top of last block.
        document.getElementById("console").scrollTop = document.getElementById("console").scrollHeight;
    } else {
        // jump down to the bottom of the scroll box
        document.getElementById("console").scrollTop = document.getElementById("console").scrollHeight;
    }
}
// FUNCTION:  prints out a piece of the queue to write one chunk per interval
function innerWriteMessage() {

    //indexLastWrittenMessage

    if (messagesToWriteArray[indexLastWrittenMessage] != null) {
        var fin = "<fade>";
        var fout = "</fade>";

        document.getElementById("console").innerHTML = document.getElementById("console").innerHTML + fin + messagesToWriteArray[indexLastWrittenMessage] + fout;

        //$("#console").fadeIn();
        //$("fade").fadeOut(.01,function() {
        //$(this).text((this).text).fadeIn();
        //document.getElementById("console").innerHTML = document.getElementById("console").innerHTML
        //});

        // jump down to the bottom of the scroll box
        document.getElementById("console").scrollTop = document.getElementById("console").scrollHeight;
        indexLastWrittenMessage++;

        $("#console p").css("opacity", "1.0");
        $("#console p:last").css("opacity", "0.0");
        $("#console p:last").animate({ "opacity": "1.0" }, "slow");
        //$("#console :not(p:last)").css({backgroundColor: 'yellow'});
    }

}

// FUNCTION:  Returns the transformation status of each part of the opponent
function lookAtAI() {

    var message = npcCharLoader.characters[1].getOverallStatus();
    writeMessageImmediate(message, "noScroll");

}

// FUNCTION:  Returns the transformation status of each part of the player	
function lookAtHuman() {

    var message = humanCharLoader.characters[1].getOverallStatus();
    writeMessageImmediate(message, "noScroll");

}

function submitAIData() {

    var data = document.getElementById("charSubmitField").value;
    npcCharLoader.loadData(data);
    allCharLoader.loadData(data);

    //npcCharLoader.calculateMaxTFPoints(1);

}

function submitCharData() {

    var data = document.getElementById("charSubmitField").value;
    allCharLoader.loadData(data);

    //allCharLoader.calculateMaxTFPoints(1);	

}

function submitHumanData() {

    var data = document.getElementById("playerSubmitField").value;
    humanCharLoader.loadData(data);
    allCharLoader.loadData(data);

    //humanCharLoader.calculateMaxTFPoints(1);


    nameFormHack = humanCharLoader.characters[1].firstName + " " + humanCharLoader.characters[1].lastName;

}

function debugAI() {
    var debugData = npcCharLoader.characters[1].debug();
    writeMessageImmediate(debugData);

}

function debugHuman() {
    var debugData = humanCharLoader.characters[1].debug();
    writeMessageImmediate(debugData);

}

function writeGameIntroductionMessage() {
    //write an introduction message
    writeMessage("Welcome to Transformania Time(TM)!  Today we have two eager contestents competing against each other for a hefty cash prize!  What's the catch?  Well, let me tell you this:  this experience CHANGES you!  I can guarantee that neither of our contestants will step out this door the same person that they stepped in as!  And I don't mean in some kind of metaphysical or emotional way like other game shows.  What do I mean?  Well, it won't take you long to find out, I can guarantee you that!  Neither can I say how either will be when they leave--it's different for absolutely everyone!  So... let the games BEGIN!!!!!");
    writeMessage(humanCharLoader.characters[1].description);
    writeMessage(npcCharLoader.characters[1].description);
}



window.onresize = function () {

    resizeDivs();


}

function resizeDivs() {
    var windowWidth = window.innerWidth;
    //var buttonsWidth = 300;
    //var difficultyWidth = 250;

    var leftBoxWidth = $("#leftColumnDiv").width();
    var rightBoxWidth = $("#rightColumnDiv").width();

    // resize gameplay box
    var mydiv = document.getElementById("gameplayBox");
    mydiv.style.width = windowWidth - leftBoxWidth - rightBoxWidth - 90 + "px";

    //resize question/answer wrapper
    //var timerWidth = document.getElementById("timer").offsetWidth;
    //var wrapperWidth = document.getElementById("questionTimerWrapper").offsetWidth;
    //var resizeMe = document.getElementById("question");
    //resizeMe.style.width = wrapperWidth - timerWidth - 10 + "px";

    //var outputBoxWidth = document.getElementById("outputBox");
    //var consoleBox = document.getElementById("outputBox");
    //consoleBox.style.width = outputBoxWidth.offsetWidth - outputBoxWidth.style.

    //alert(outputBoxWidth);

}

function closeNewGameMenu() {
    $("#newGameWindow").hide();
    $("#grayBackground").hide();
}

function openNewGameMenu() {
    $("#newGameWindow").show();
    $("#grayBackground").show();

    $("#newGameWindow").scrollTop(0);

    centerNewGameWindow();

}

function showDoneLoading() {
    document.getElementById("loadingInfo").innerHTML = "Characters loaded!";
}

// FUNCTION:  Using newly loaded characters, add them to the character selection dropdown menu
function listLoadedCharacters() {


    // update load status text after designated wait time
    setTimeout("showDoneLoading()", waitForCharacterLoadTime);

    dropdownOpp = document.getElementById('loadedOpponentsList');
    dropdownPlayer = document.getElementById('loadedPlayersList');

    // reset the options in our list of loaded characters
    document.getElementById('loadedOpponentsList').options.length = 0;
    document.getElementById('loadedPlayersList').options.length = 0;

    var humanSlot = humans.nextEmptyCharSlot(humans);
    var aiSlot = humans.nextEmptyCharSlot(opponents);

    var data = document.getElementById("allCharSubmitField").value;
    sorter.loadData(data);

    document.getElementById("allCharSubmitField").value = "";


    //humans.characters = preloadCharacters(charactersToLoad);
    //opponents.characters = preloadCharacters(charactersToLoad);



    // for each character in our file, add the character to the appropriate category
    for (var i = 0; i <= charactersToLoad; i++) {

        //populate each array with a blank character if needed
        try {
            var throwaway = sorter.characters[i].firstName;
        } catch (e) { sorter.characters[i] = new Character(); }

        try {
            var throwaway = humans.characters[i].firstName;
        } catch (e) { humans.characters[i] = new Character(); }

        try {
            var throwaway = opponents.characters[i].firstName;
        } catch (e) { opponents.characters[i] = new Character(); }

        if (sorter.characters[i].firstName != "MISSING") {

            var name = sorter.characters[i].firstName + " " + sorter.characters[i].lastName;
            var desc = sorter.characters[i].description;

            humanSlot = humans.nextEmptyCharSlot(humans);
            aiSlot = opponents.nextEmptyCharSlot(opponents);

            var option = document.createElement("option");
            option.text = name;

            //alert(i + " : " + name + ".  Next slot:  " + aiSlot);

            if (sorter.characters[i].useableTypes == "human") {
                if (!charIsLoaded(name, desc, humans)) {

                    humans.characters[humanSlot] = sorter.characters[i];
                    //alert("human loaded");
                }
            } else if (sorter.characters[i].useableTypes == "computer") {
                if (!charIsLoaded(name, desc, opponents)) {
                    opponents.characters[aiSlot] = sorter.characters[i];
                    //alert("opponent loaded");
                }
            } else if (sorter.characters[i].useableTypes == "computer") {

            }
        }
    }

    // populate the options in the player/ai dropdown list
    for (var i = 0; i < humans.characters.length; i++) {

        if ((humans.characters[i] instanceof Character) && (humans.characters[i].firstName != "MISSING")) {

            var name = humans.characters[i].firstName + " " + humans.characters[i].lastName;
            var option = document.createElement("option");
            option.text = name;
            dropdownPlayer.add(option, dropdownOpp.options[null]);
        }
    }

    for (var i = 0; i < opponents.characters.length; i++) {

        if ((opponents.characters[i] instanceof Character) && (opponents.characters[i].firstName != "MISSING")) {

            var name = opponents.characters[i].firstName + " " + opponents.characters[i].lastName;
            var option = document.createElement("option");
            option.text = name;
            dropdownOpp.add(option, dropdownOpp.options[null]);
        }
    }

    sorter = new CharacterLoader();
    charactersToLoad = 0;


    showCharacterDescriptions();

}

function setCharacters() {

    dropdownOpp = document.getElementById('loadedOpponentsList');
    dropdownPlayer = document.getElementById('loadedPlayersList');

    var playerIndex = dropdownPlayer.selectedIndex;
    var computerIndex = dropdownOpp.selectedIndex;

    humanCharLoader.characters[1] = humans.characters[playerIndex];
    npcCharLoader.characters[1] = opponents.characters[computerIndex];

    resetPartTFs(humanCharLoader.characters[1]);
    resetPartTFs(npcCharLoader.characters[1]);

    humanCharLoader.calculateMaxTFPoints(1);
    npcCharLoader.calculateMaxTFPoints(1);


}

function resetPartTFs(character) {

    for (var p in character.parts) {
        var thisPart = character.parts[p];
        thisPart.currentStage = 0;
       // alert(thisPart.currentStage);
    }

}

function showCharacterDescriptions() {
    dropdownOpp = document.getElementById('loadedOpponentsList');
    dropdownPlayer = document.getElementById('loadedPlayersList');

    var playerIndex = dropdownPlayer.selectedIndex;
    var computerIndex = dropdownOpp.selectedIndex;

    humans.calculateMaxTFPoints(playerIndex);
    opponents.calculateMaxTFPoints(computerIndex);

    var p = humans.characters[playerIndex];
    var c = opponents.characters[computerIndex];

    if (p.startGender == "male") {
        document.getElementById("selectedPlayerInfo").innerHTML = "<blue>" + p.firstName + " " + p.lastName + "</blue> : " + p.maxTFPoints + " stages.";
    } else {
        document.getElementById("selectedPlayerInfo").innerHTML = "<pink>" + p.firstName + " " + p.lastName + "</pink> : " + p.maxTFPoints + " stages.";
    }
    document.getElementById("selectedPlayerDesc").innerHTML = p.description + "<br><br>Intelligence:  " + p.intelligence + "<br>Reflex:  " + p.reflex + "<br>Prediction:  " + p.prediction + "<br><br>";
    //$("#selectedPlayerDesc").html()
    document.getElementById("selectedPlayerAuthor").innerHTML = 'Character created by <a href="' + p.authorHome + '"target="_blank">' + p.author + '</a>. ~~~ Tags:  ' + p.tags;

    if (c.startGender == "male") {
        document.getElementById("selectedOpponentInfo").innerHTML = "<blue>" + c.firstName + " " + c.lastName + "</blue> : " + c.maxTFPoints + " stages.";
    } else {
        document.getElementById("selectedOpponentInfo").innerHTML = "<pink>" + c.firstName + " " + c.lastName + "</pink> : " + c.maxTFPoints + " stages.";
    }
    document.getElementById("selectedOpponentDesc").innerHTML = c.description + "<br><br>Intelligence:  " + c.intelligence + "<br>Reflex:  " + c.reflex + "<br>Prediction:  " + c.prediction + "<br><br>";
    document.getElementById("selectedOpponentAuthor").innerHTML = 'Character created by <a href="' + c.authorHome + '"target="_blank">' + c.author + '</a>. ~~~ Tags:  ' + c.tags;

}

function unloadCharacters() {
    humans = new CharacterLoader();
    opponents = new CharacterLoader();
    sorter = new CharacterLoader();

    dropdownOpp = document.getElementById('loadedOpponentsList');
    dropdownPlayer = document.getElementById('loadedPlayersList');
    dropdownOpp.length = 0;
    dropdownPlayer.length = 0;

    document.getElementById("selectedPlayerInfo").innerHTML = "";
    document.getElementById("selectedPlayerDesc").innerHTML = "";
    document.getElementById("selectedPlayerAuthor").innerHTML = "";
    document.getElementById("selectedOpponentInfo").innerHTML = "";
    document.getElementById("selectedOpponentDesc").innerHTML = "";
    document.getElementById("selectedPlayerAuthor").innerHTML = "";

    charactersToLoad = 0;

}

function charIsLoaded(name, desc, chars) {
    for (var i = 0; i < chars.characters.length; i++) {
        var pullName = chars.characters[i].firstName + " " + chars.characters[i].lastName;
        if ((pullName != "MISSING") && (name != "MISSING") && (name == pullName)) {
            if (desc == chars.characters[i].description) {
                return true;
            } else {

            }

        }
    }
    return false;
}

// FUNCTION:  Checks for keyboard keys that are being pressed and runs the appropriate logic
document.onkeydown = function (evt) {
    evt = evt || window.event;

    devModeEverOn = true;

    if (evt.keyCode == 112) {
        if (devModeOn == false) {
            document.getElementById("debugCharacterButton").style.visibility = "visible";
            document.getElementById("debugHumanButton").style.visibility = "visible";
            alert("Character test mode enabled.");
            
            devModeOn = !devModeOn;
        } else {
            document.getElementById("debugCharacterButton").style.visibility = "hidden";
            document.getElementById("debugHumanButton").style.visibility = "hidden";
            alert("Character test mode disabled.");
            devModeOn = !devModeOn;
        }
    }

    if (devModeOn) {
        if (evt.keyCode == 113) {
            writeMessage("<b>TEST MODE:  OPPONENT TRANSFORMS 10 TIMES</B><br>")
            tfOpponent(10);
        }
        if (evt.keyCode == 114) {
            writeMessage("<b>TEST MODE:  PLAYER TRANSFORMS 10 TIMES</B><br>")
            tfHuman(10);
        }
        if (evt.keyCode == 115) {
            alert("ajax...");




            //$.get('Characters/test.txt', function(data) {
            //$('.result').html(data);

            //});

            //$.ajax({
            //url: "Characters/test.txt",
            //context: document.body
            //}).done(function() {
            //$(this).addClass("done");
            //alert("done!");
            //});

        }
    }

};


function updateGlobalProgressBars() {

    var numerator = parseFloat(npcCharLoader.characters[1].currentTFPoints);
    var denominator = parseFloat(npcCharLoader.characters[1].maxTFPoints);
    var newHeightO = parseFloat(parseFloat(numerator) / parseFloat(denominator)) * 100;
    newHeightO = newHeightO.toString() + "%";

    var numerator = parseFloat(humanCharLoader.characters[1].currentTFPoints);
    var denominator = parseFloat(humanCharLoader.characters[1].maxTFPoints);
    var newHeightP = parseFloat(parseFloat(numerator) / parseFloat(denominator)) * 100;
    newHeightP = newHeightP.toString() + "%";

    $("#totalOpponentTFProgressBar").animate({ "height": newHeightO }, 1200);
    $("#totalPlayerTFProgressBar").animate({ "height": newHeightP }, 1200);

    //alert(newHeightO + "," + newHeightP);
}

function centerNewGameWindow() {

    var windowWidth = $(window).width();
    var gameAreaWidth = $("#newGameWindow").width();
    var bufferEachSide = ((windowWidth - gameAreaWidth) / 2 - 25) + "px";

    $("#newGameWindow").css({ "margin-left": bufferEachSide });
    $("#newGameWindow").css({ "margin-right": bufferEachSide });

}

function updateGlobalDifficultyModifier(num, type) {

    if (type == "discus") {
        globalDifficultyModifierDiscus += num;
        if (globalDifficultyModifierDiscus < 0) {
            globalDifficultyModifierDiscus = 0;
        }
    } else if (type == "dpong") {
        globalDifficultyModifierDpong += num;
        if (globalDifficultyModifierDpong < 0) {
            globalDifficultyModifierDpong = 0;
        }
    } else if (type == "memory") {
        globalDifficultyModifierMemory += num;
        if (globalDifficultyModifierMemory < 0) {
            globalDifficultyModifierMemory = 0;
        }
    } else if (type == "popup") {
        globalDifficultyModifierPopup += num;
        if (globalDifficultyModifierPopup < 0) {
            globalDifficultyModifierPopup = 0;
        }
    } else if (type == "reflex") {
        globalDifficultyModifierReflex += num;
        if (globalDifficultyModifierReflex < 0) {
            globalDifficultyModifierReflex = 0;
        }
    } else if (type == "roulette") {
        globalDifficultyModifierRoulette += num;
        if (globalDifficultyModifierRoulette < 0) {
            globalDifficultyModifierRoulette = 0;
        }
    } else if (type == "sequence") {
        globalDifficultyModifierSequence += num;
        if (globalDifficultyModifierSequence < 0) {
            globalDifficultyModifierSequence = 0;
        }
    } else {
        alert("ERROR WILL ROBINSON!");
    }

    if (globalDifficultyModifier < 0) {
        globalDifficultyModifier = 0;
    }

}

function centerMeToWindow(div) {

    var windowWidth = $(window).width();
    var gameAreaWidth = div.width();
    var bufferEachSideHoriz = ((windowWidth - gameAreaWidth) / 2 - 25) + "px";

    var windowHeight = $(window).height();
    var gameAreaHeight = div.height();
    var bufferEachSideVert = ((windowHeight - gameAreaHeight) / 2 - 25) + "px";


    div.css({ "margin-left": bufferEachSideHoriz });
    div.css({ "margin-right": bufferEachSideHoriz });
    div.css({ "margin-top": bufferEachSideVert });
    div.css({ "margin-bottom": bufferEachSideVert });

}

function showHelp() {
    $("#helpWindow").toggle();
    $("#grayBackground").toggle();
    $("#helpWindow").scrollTop(0);
    centerMeToWindow($("#helpWindow"));
}

function loadAllCharactersWithAjax() {
    $.ajax({
        url: "Characters/All Characters (23).txt",
        dataType: "text",
        success: function (data) {
            sorter.loadData(data);
            listLoadedCharacters();

        }
    });
}

function allMiniGamesHaveBeenPlayed() {
    for (var i = 0; i < NUM_MINIGAMES; i++) {
        //alert(miniGamesPlayed[i]);
        if (miniGamesPlayed[i] == "undefined") {
            return false;
        }
    }
    return true;


}

function setMinigameNameplate(window, gameName) {

    $("#minigameNameplate").show();

    var itsOffset = window.offset();
    itsOffset.top = itsOffset.top - 40;

    $("#minigameNameplate").offset(itsOffset);
    var itsWidth = parseInt(window.css("width"));
    itsWidth += 18;

    $("#minigameNameplate").css("width", itsWidth + "px");

    $("#minigameNameplate").html(gameName);

    //alert("done");
}


function showHelpAlert() {
    if (miniGameType == 0) {
        alert("Remember the sequence shown at the start of the round.  When it vanishes, enter it back in.");
    } else if (miniGameType == 1) {
        alert("Click on the moving target.");
    } else if (miniGameType == 2) {
        alert("Remember the locations of the symbols in the grid.  When they vanish, uncover each pair.");
    } else if (miniGameType == 3) {
        alert("Click on the boxes that pop up.  Smaller boxes give you more points, but if you miss, you may lose some points instead.");
    } else if (miniGameType == 4) {
        alert("Click on one of the four colored boxes to place a bet on which color you think the spinner will land on.  Each bet is worth less than the previous, and each bet is worth less as the spinner slows down.");
    } else if (miniGameType == 5) {
        alert("Click in the game window to shoot at a disc.  Hitting smaller discs will earn you more points. Caution: each bullet you fire may cost you a few of your points!");
    } else if (miniGameType == 6) {
        alert("Keep all of the discs in the game window by moving your paddle to defect it.  The discs will leave the game window at both the TOP and BOTTOM!  HINT:  You can use the arrow keys on your keyboard for this game!");
    }
}

function loadSuccess() {
   // alert("ding");
}

function AjaxSubmit(input) {

   // alert(input);

    if (devModeEverOn == false) {

        $.ajax({
           // url: 'TransformaniaTime/PostStat/',
            url: '../TransformaniaTime/PostStat/',
            type: 'POST',
            data: {message : input},
            // beforeSend: loadWaiting,
            success: loadSuccess,
            dataType: 'text'
        });



    } else {
        
    }


}