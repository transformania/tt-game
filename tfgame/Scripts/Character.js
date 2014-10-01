
function Character() {

    this.useableTypes = "";

    this.firstName = "MISSING";
    this.lastName = "MISSING";
    this.firstNameNew = "MISSING";
    this.lastNameNew = "MISSING";
    this.startGender = "MISSING";
    this.description = "MISSING";
    this.losedescription = "MISSING";
    this.intelligence = 50;
    this.reflex = 50;
    this.prediction = 50;
    this.desire = "MISSING";
    this.gender;
    this.author;
    this.authorHome;
    this.tags;

    this.aliases = new Array(5);



    this.parts = {
        hair: new Part(),
        face: new Part(),
        voice: new Part(),
        breasts: new Part(),
        body: new Part(),
        shoulders: new Part(),
        waist: new Part(),
        butt: new Part(),
        legs: new Part(),
        feet: new Part(),
        hands: new Part(),
        arms: new Part(),
        groin: new Part(),
        mind: new Part(),
        shirt: new Part(),
        pants: new Part(),
        shoes: new Part(),
        accessory: new Part(),
        custom1: new Part(),
        custom2: new Part(),
        custom3: new Part(),
        custom4: new Part(),
        custom5: new Part(),
    };

    this.maxTFPoints = new Number(0);
    this.currentTFPoints = new Number(0);

    // FUNCTION:  Returns string with information about character's current status
    this.getOverallStatus = getOverallStatus;
    function getOverallStatus() {

        var genderTagOpen = "";
        var genderTagClose = "";
        if (isMale(this.gender)) {
            genderTagOpen = "<blue>";
            genderTagClose = "</blue>";
        } else if (isFemale(this.gender)) {
            genderTagOpen = "<pink>";
            genderTagClose = "</pink>";
        } else {

        }

        var output = "";

        // check if we are referring to a player or computer for the sake of "you look at him" or "you look at yourself"
        if (this.useableTypes == "computer") {
            output = "<br>You take a glance at " + genderTagOpen + this.firstName + " " + this.lastName + genderTagClose + ".<br>";
        } else if (this.useableTypes == "human") {
            output = "<br>You take a glance at yourself, " + genderTagOpen + this.firstName + " " + this.lastName + genderTagClose + ".<br>";
        }

        for (var p in this.parts) {

            if (this.parts[p].maxStage > 0) {

                if (this.parts[p].name != "accessory") {

			var st = this.parts[p].currentStage;
			// if the status is not hidden...
			if (this.parts[p].stages[st].status!=undefined) {
				output += "&#9674; " + this.writeCurrentStatus(p);
			}
		    
		    
                } else if (this.parts[p].name == "accessory") {

                    if (this.parts[p].stages[0].status != null) {
                        output += "&#9674; " + this.writeStageStatus(p, 0);
                    }

                    for (var i = 0; i < this.parts[p].currentStage; i++) {

                        output += "&#9674; " + this.writeStageStatus(p, i);
                    }


                }
            }

        }

        output += "<br>";

        return output;
    }

    // FUNCTION:  check if gender is male
    this.isMale = isMale;
    function isMale(gender) {
        if (gender == "male") {
            return true;
        } else {
            return false;
        }
    }

    // FUNCTION:  check if gender is female
    this.isFemale = isFemale;
    function isFemale(gender) {
        if (gender == "female") {
            return true;
        } else {
            return false;
        }
    }

    this.debug = debug;
    function debug() {
        var output = "<debug> <b>GENERAL<br></b>";
        output += "<b>First name:  </b>" + this.firstName + "<br>";
        output += "<b>Last name:  </b>" + this.lastName + "<br>";
        output += "<b>New first name:  </b>" + this.firstNameNew + "<br>";
        output += "<b>New Last name:  </b>" + this.lastNameNew + "<br>";
        output += "<b>Character type:  </b>" + this.useableTypes + "<br>";
        output += "<b>Starting gender:  </b>" + this.startGender + "<br>";
        output += "<b>description:  </b>" + this.description + "<br>";
        output += "<b>loss description:  </b>" + this.losedescription + "<br>";
        output += "<b>Author:  </b>" + this.author + "<br>";
        output += "<b>Author Homepage:  </b>" + this.authorHome + "<br>";
        output += "<b>Tags:  </b>" + this.tags + "<br>";
        output += "<b>intelligence:  </b>" + this.intelligence + "<br>";
        output += "<b>reflex:  </b>" + this.reflex + "<br>";
	output += "<b>prediction:  </b>" + this.prediction + "<br>";
        output += "<b>distractibility:  </b>" + this.desire + "<br>";
        output += "<b>current gender:  </b>" + this.gender + "<br>";

        for (var p in this.parts) {
            //if ()
            if (this.parts[p].maxStage > 0) {
                output += "**********" + this.parts[p].name.toUpperCase() + "**********<br>";
                output += "current stage:  " + this.parts[p].currentStage + "<br>";
                output += "max stage:  " + this.parts[p].maxStage + "<br>";
                output += "tgPoint:  " + this.parts[p].tgPoint + "<br>";

                var cycles = this.parts[p].maxStage;

                for (var i = 0; i <= cycles; i++) {
                    output += "[tf] " + i + ":  ";
                    output += this.parts[p].stages[i].tf + "<br>";
                }
                output += "-----------<br>";
                for (var i = 0; i <= cycles; i++) {
                    output += "[status] " + i + ":  ";
                    output += this.parts[p].stages[i].status + "<br>";
                }
                output += "-----------<br>";
                for (var i = 0; i <= cycles; i++) {
                    output += "[short] " + i + ":  ";
                    output += this.parts[p].stages[i].short + "<br>";
                }
                output += "-----------<br>";
                for (var i = 0; i <= cycles; i++) {
                    // another for loops...
                    if (this.parts[p].waitfor[i] != null) {
                        output += "-----------<br>";
                        output += "Part at stage " + i + " waits for " + this.parts[p].waitfor[i] + "<br>";
                    }
                }
                output += "-----------<br>";


                for (var i = 0; i <= cycles; i++) {
                    if (this.parts[p].tgShade[i] != null) {
                        output += "tgShade at " + i + ": " + this.parts[p].tgShade[i] + "<br>";
                    }

                }

            }
        }


        output += this.currentTFPoints + " / " + this.maxTFPoints + " stages remaining.<br>";

        output += "</debug>";
        return output;
    }

    // returns the color-coded text for a character's latest transformation
    this.writeCurrentTF = writeCurrentTF;
    function writeCurrentTF(type) {

        //type is just a string, name of part

        var output = "";

        //output += this.stages[this.currentStage].tf + "<br>";
        cur = this.parts[type].currentStage;
        output += this.parts[type].stages[cur].tf + "<br>";



        output = parseForGenderComponents(this, output);
        output = parseForVariables(this, output);
        return output;
    }

    // returns the color-coded text for a character's latest status
    this.writeCurrentStatus = writeCurrentStatus;
    function writeCurrentStatus(type) {
        var output = "";
	
	

        stage = this.parts[type].currentStage;
	

	
	if (this.parts[type].stages[stage].status!="") {
	
		output += this.parts[type].stages[stage].status + "<br>";
	
	}
	



        output = parseForGenderComponents(this, output);
        output = parseForVariables(this, output);
        return output;
    }

    // FUNCTION : returns the color-code  d text for a part's status at a specified stage, used only for accessories so far.
    this.writeStageStatus = writeStageStatus;
    function writeStageStatus(type, stage) {
        var output = "";



        output += this.parts[type].stages[stage].status + "<br>";



        output = parseForGenderComponents(this, output);
        output = parseForVariables(this, output);



        return output;
    }

    // returns the color-coded text for a character's latest short
    // NOTE:  This lacks a breakline at the end.  Since it is used
    // as a slice of something else, we don't want it.
    this.writeCurrentShort = writeCurrentShort;
    function writeCurrentShort(type) {

        var output = "";


        stage = this.parts[type].currentStage;
        output += this.parts[type].stages[stage].short;

        var gender = getPartGender(this, this.parts[type]);
        var colIn;
        var colOut;


        // no tg shade for this part so we'll go use defaults
        if (this.parts[type].tgShade[stage] == null) {
            if (gender == "male") {
                var colIn = "<blue>";
                var colOut = "</blue>";
            } else if (gender == "female") {
                var colIn = "<pink>";
                var colOut = "</pink>";
            }
        }

        // there is a TG shade so we will calculate a color gradient
        if (this.parts[type].tgShade[stage] != null) {

            var redShade = (parseInt(this.parts[type].tgShade[stage]) / 100) * 255;
            var redHex = redShade.toString(16).substring(0, 2);
            colIn = '<font color="' + redHex + '00FF">';
            colOut = '</font>';

        }

        // apply color tags, parse for name/gender, then parse for variables
        output = colIn + output + colOut;
        output = parseForGenderComponents(this, output);
        output = parseForVariables(this, output);

        return output;
    }

    this.parseForGenderComponents = parseForGenderComponents;
    function parseForGenderComponents(me, text) {

        var isSwapped = genderIsSwapped(me);
        var iterations = 10;

        // character name/gender pronouns are based strictly on the groin
        var groin = me.parts["groin"];
        var stage = groin.currentStage;



        // no tg shade for this part so we'll go use defaults
        if (groin.tgShade[stage] == null) {
            if (me.gender == "male") {
                var colIn = "<blue>";
                var colOut = "</blue>";
            } else if (me.gender == "female") {
                var colIn = "<pink>";
                var colOut = "</pink>";
            }
        }

        else {
            // grab the color value we need to print the name in
            var redShade = (parseInt(groin.tgShade[stage]) / 100) * 255;
            var redHex = redShade.toString(16).substring(0, 2);
            var colIn = '<font color="' + redHex + '00FF">';
            var colOut = '</font>';
        }



        //how many times to look through the string so multiple words can be recolored
        for (var i = 0; i < iterations; i++) {

            if (me.useableTypes == "computer") {

                // changes the character's name, as well as he/his she/her to the appropriate color,
                // currently based on overall sex
                if (me.gender == "male") {
                    if (isSwapped == true) {

                        // a male who was female (female tags to male)
                        var name = me.firstName;
                        var nameWithTags = colIn + name + colOut;
                        text = text.replace(name, nameWithTags);
                        text = text.replace(" she ", " " + colIn + "he" + colOut + " ");
                        text = text.replace(" She ", " " + colIn + "He" + colOut + " ");
                        text = text.replace(" her ", " " + colIn + "his" + colOut + " ");
                        text = text.replace(" Her ", " " + colIn + "His" + colOut + " ");
                        text = text.replace(" She's ", " " + colIn + "He's" + colOut + " ");
                        text = text.replace(" she's ", " " + colIn + "he's" + colOut + " ");
                        text = text.replace(" her ", " " + colIn + "his" + colOut + " ");
                        text = text.replace(" Her ", " " + colIn + "His" + colOut + " ");
                        text = text.replace(" herself ", " " + colIn + "himself" + colOut + " ");
                        text = text.replace(" Herself ", " " + colIn + "Himself" + colOut + " ");


                    } else { // a male who is still male

                        var name = me.firstName;
                        var nameWithTags = colIn + name + colOut;
                        text = text.replace(name, nameWithTags);
                        text = text.replace(" he ", " " + colIn + "he" + colOut + " ");
                        text = text.replace(" He ", " " + colIn + "He" + colOut + " ");
                        text = text.replace(" his ", " " + colIn + "his" + colOut + " ");
                        text = text.replace(" His ", " " + colIn + "His" + colOut + " ");
                        text = text.replace(" he's ", " " + colIn + "he's" + colOut + " ");
                        text = text.replace(" He's ", " " + colIn + "He's" + colOut + " ");
                        text = text.replace(" his ", " " + colIn + "his" + colOut + " ");
                        text = text.replace(" His ", " " + colIn + "His" + colOut + " ");
                        text = text.replace(" Him ", " " + colIn + "Him" + colOut + " ");
                        text = text.replace(" him ", " " + colIn + "him" + colOut + " ");
                        text = text.replace(" Himself ", colIn + "Himself" + colOut + " ");
                        text = text.replace(" himself ", colIn + "himself" + colOut + " ");
                    }

                } else if (me.gender == "female") {



                    if (isSwapped == true) {	 // a female who was male (male tags to female)

                        var name = me.firstName;
                        var nameWithTags = colIn + name + colOut;
                        text = text.replace(name, nameWithTags);
                        text = text.replace(" he ", " " + colIn + "she" + colOut + " ");
                        text = text.replace(" He ", " " + colIn + "She" + colOut + " ");
                        text = text.replace(" his ", " " + colIn + "her" + colOut + " ");
                        text = text.replace(" His ", " " + colIn + "Her" + colOut + " ");
                        text = text.replace(" he's ", " " + colIn + "she's" + colOut + " ");
                        text = text.replace(" He's ", " " + colIn + "She's" + colOut + " ");
                        text = text.replace(" his ", " " + colIn + "her" + colOut + " ");
                        text = text.replace(" His ", " " + colIn + "Her" + colOut + " ");
                        text = text.replace(" him ", " " + colIn + "her" + colOut + " ");
                        text = text.replace(" Him ", " " + colIn + "Her" + colOut + " ");
                        text = text.replace(" himself ", " " + colIn + "herself" + colOut + " ");
                        text = text.replace(" Himself ", " " + colIn + "Herself" + colOut + " ");


                    } else {	// a female who is still female
                        var name = me.firstName;
                        var nameWithTags = colIn + name + colOut;
                        text = text.replace(name, nameWithTags);
                        text = text.replace(" she ", " " + colIn + "she" + colOut + " ");
                        text = text.replace(" She ", " " + colIn + "She" + colOut + " ");
                        text = text.replace(" her ", " " + colIn + "her" + colOut + " ");
                        text = text.replace(" Her ", " " + colIn + "Her" + colOut + " ");
                        text = text.replace(" she's ", " " + colIn + "she's" + colOut + " ");
                        text = text.replace(" She's ", " " + colIn + "She's" + colOut + " ");
                        text = text.replace(" her ", " " + colIn + "her" + colOut + " ");
                        text = text.replace(" Her ", " " + colIn + "Her" + colOut + " ");
                        text = text.replace(" Herself ", " " + colIn + "Herself" + colOut + " ");
                        text = text.replace(" herself ", " " + colIn + "herself" + colOut + " ");

                    }
                }

            } else if (me.useableTypes == "human") {

                if (me.gender == "male") {

                    if (!isSwapped) { // a male who is still male

                        text = text.replace("Your ", " " + colIn + "Your" + colOut + " ");
                        text = text.replace(" You ", " " + colIn + "You" + colOut + " ");
                        text = text.replace("You ", " " + colIn + "You" + colOut + " ");
                        text = text.replace(" you ", " " + colIn + "you" + colOut + " ");
                        text = text.replace(" Your ", " " + colIn + "Your" + colOut + " ");
                        text = text.replace(" your ", " " + colIn + "your" + colOut + " ");
                        text = text.replace(" You're ", " " + colIn + "You're" + colOut + " ");
                        text = text.replace(" you're ", " " + colIn + "you're" + colOut + " ");

                    } else if (isSwapped) { // a male who was female

                        text = text.replace("Your ", " " + colIn + "Your" + " " + colOut + " ");
                        text = text.replace(" You ", " " + colIn + "You" + " " + colOut + " ");
                        text = text.replace("You ", " " + colIn + "You" + " " + colOut + " ");
                        text = text.replace(" you ", " " + colIn + "you" + " " + colOut + " ");
                        text = text.replace(" Your ", " " + colIn + "Your" + " " + colOut + " ");
                        text = text.replace(" your ", " " + colIn + "your" + " " + colOut + " ");
                        text = text.replace(" You're ", " " + colIn + "You're" + " " + colOut + " ");
                        text = text.replace(" you're ", " " + colIn + "you're" + " " + colOut + " ");

                    }


                } else if ((me.gender == "female") && (!isSwapped)) { // a female who is still female

                    text = text.replace("Your ", " " + colIn + "Your</pink> ");
                    text = text.replace(" You ", " " + colIn + " " + "You" + colOut + " ");
                    text = text.replace("You ", " " + colIn + " " + "You" + colOut + " ");
                    text = text.replace(" you ", " " + colIn + " " + "you" + colOut + " ");
                    text = text.replace(" Your ", " " + colIn + " " + "Your" + colOut + " ");
                    text = text.replace(" your ", " " + colIn + " " + "your" + colOut + " ");
                    text = text.replace(" You're ", " " + colIn + " " + "You're" + colOut + " ");
                    text = text.replace(" you're ", " " + colIn + " " + "you're" + colOut + " ");
                } else if ((me.gender == "female") && (isSwapped)) { // a female who was male

                    text = text.replace(" Your ", " " + colIn + "Your" + colOut + " ");
                    text = text.replace(" You ", " " + colIn + "You" + colOut + " ");
                    text = text.replace("You ", " " + colIn + " " + "You" + colOut + " ");
                    text = text.replace(" you ", " " + colIn + " " + "you" + colOut + " ");
                    text = text.replace(" Your ", " " + colIn + " " + "Your" + colOut + " ");
                    text = text.replace(" your ", " " + colIn + " " + "your" + colOut + " ");
                    text = text.replace(" You're ", " " + colIn + " " + "You're" + colOut + " ");
                    text = text.replace(" you're ", " " + colIn + " " + "you're" + colOut + " ");
                }
            }// end huuman player if
        } // end for loop

        return text;

    } // end function

    // FUNCTION:  looks for variables (text enclosed in $ signs) and inserts the right 'short' text
    this.parseForVariables = parseForVariables;
    function parseForVariables(me, text) {

        for (var z = 0; z < 3; z++) {

            // I an NOT using else if because there may be several variables in one line
            if (text.indexOf("$BREASTS$") != -1) {
                //var sliceIn = me.writeCurrentShort(this.parts[breasts]);
                var sliceIn = me.writeCurrentShort("breasts");
                text = text.replace("$BREASTS$", sliceIn);
            }
            if (text.indexOf("$HAIR$") != -1) {
                var sliceIn = me.writeCurrentShort("hair");
                text = text.replace("$HAIR$", sliceIn);
            }
            if (text.indexOf("$FACE$") != -1) {
                var sliceIn = me.writeCurrentShort("face");
                text = text.replace("$FACE$", sliceIn);
            }
            if (text.indexOf("$VOICE$") != -1) {
                var sliceIn = me.writeCurrentShort("voice");
                text = text.replace("$VOICE$", sliceIn);
            }
            if (text.indexOf("$BODY$") != -1) {
                var sliceIn = me.writeCurrentShort("body");
                text = text.replace("$BODY$", sliceIn);
            }
            if (text.indexOf("$SHOULDERS$") != -1) {
                var sliceIn = me.writeCurrentShort("shoulders");
                text = text.replace("$SHOULDERS$", sliceIn);
            }
            if (text.indexOf("$WAIST$") != -1) {
                var sliceIn = me.writeCurrentShort("waist");
                text = text.replace("$WAIST$", sliceIn);
            }
            if (text.indexOf("$BUTT$") != -1) {
                var sliceIn = me.writeCurrentShort("butt");
                text = text.replace("$BUTT$", sliceIn);
            }
            if (text.indexOf("$LEGS$") != -1) {
                var sliceIn = me.writeCurrentShort("legs");
                text = text.replace("$LEGS$", sliceIn);
            }
            if (text.indexOf("$FEET$") != -1) {
                var sliceIn = me.writeCurrentShort("feet");
                text = text.replace("$FEET$", sliceIn);
            }
            if (text.indexOf("$HANDS$") != -1) {
                var sliceIn = me.writeCurrentShort("hands");
                text = text.replace("$HANDS$", sliceIn);
            }
            if (text.indexOf("$ARMS$") != -1) {
                var sliceIn = me.writeCurrentShort("arms");
                text = text.replace("$ARMS$", sliceIn);
            }
            if (text.indexOf("$GROIN$") != -1) {
                var sliceIn = me.writeCurrentShort("groin");
                text = text.replace("$GROIN$", sliceIn);
            }
            if (text.indexOf("$MIND$") != -1) {
                var sliceIn = me.writeCurrentShort("mind");
                text = text.replace("$MIND$", sliceIn);
            }
            if (text.indexOf("$SHIRT$") != -1) {
                var sliceIn = me.writeCurrentShort("shirt");
                text = text.replace("$SHIRT$", sliceIn);
            }
            if (text.indexOf("$PANTS$") != -1) {
                var sliceIn = me.writeCurrentShort("pants");
                text = text.replace("$PANTS$", sliceIn);
            }
            if (text.indexOf("$SHOES$") != -1) {
                var sliceIn = me.writeCurrentShort("shoes");
                text = text.replace("$SHOES$", sliceIn);
            }
            if (text.indexOf("$ACCESSORY$") != -1) {
                var sliceIn = me.writeCurrentShort("accessory");
                text = text.replace("$ACCESSORY$", sliceIn);
            }
            if (text.indexOf("$CUSTOM1$") != -1) {
                var sliceIn = me.writeCurrentShort("custom1");
                text = text.replace("$CUSTOM1$", sliceIn);
            }
            if (text.indexOf("$CUSTOM2$") != -1) {
                var sliceIn = me.writeCurrentShort("custom2");
                text = text.replace("$CUSTOM2$", sliceIn);
            }
            if (text.indexOf("$CUSTOM3$") != -1) {
                var sliceIn = me.writeCurrentShort("custom3");
                text = text.replace("$CUSTOM3$", sliceIn);
            }
            if (text.indexOf("$CUSTOM4$") != -1) {
                var sliceIn = me.writeCurrentShort("custom4");
                text = text.replace("$CUSTOM4$", sliceIn);
            }
            if (text.indexOf("$CUSTOM5$") != -1) {
                var sliceIn = me.writeCurrentShort("custom5");
                text = text.replace("$CUSTOM5$", sliceIn);
            }

        }


        return text;



    }

    this.incrementStage = incrementStage;
    function incrementStage(type) {

        //first we check to see if there is a waitfor to consider
        var nextStage = this.parts[type].currentStage + 1;
        if (this.parts[type].waitfor[nextStage] != null) {

            // consult the given part...
            var components = this.parts[type].waitfor[nextStage].split(" ");

            var max = components.length;
            for (var i = 0; i < max; i += 2) {


                var typeToWaitFor = components[i];
                var stageToWaitFor = components[i + 1];

                var skip = false;

                // stop when we start seeing null values
                if (typeToWaitFor != null && stageToWaitFor != null) {


                    if (this.parts[typeToWaitFor].currentStage < stageToWaitFor) {
                        return -1; // code to say that part failed.
                    } else {

                    }

                }

            }

        }

        if (this.parts[type].currentStage < this.parts[type].maxStage) {
            this.parts[type].currentStage++;

        }

        var newval = 0;
        newval = this.currentTFPoints * 1 - 1;
        this.currentTFPoints = newval;
        return 1;


    }

    this.partIsDone = partIsDone;
    function partIsDone(part) {

        var tf = false;

        if (this.parts[part] == null) {
            return true;
        }

        //try {
        if (this.parts[part].currentStage >= this.parts[part].maxStage) {
            tf = true;
        }


        return tf;

    }

    this.charIsFullyTransformed = charIsFullyTransformed;
    function charIsFullyTransformed() {

        tf = true;

        for (var s in this.parts) {
            var cur = this.parts[s];
            if (cur.currentStage < (cur.maxStage)) {
                return false;
            }
        }


        return tf;

    }

    // FUNCTION:  returns true if gender has been swapped
    this.genderIsSwapped = genderIsSwapped;
    function genderIsSwapped(char) {
        var part = "groin";

        if (char.parts[part].currentStage > char.parts[part].tgPoint) {
            if (char.gender != char.startGender) {

                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }

    }

    this.getTransformableParts = function getTransformableParts() {

        outputParts = new Array();


        for (var s in this.parts) {
            var cur = this.parts[s];
            if (cur.currentStage < cur.maxStage) {
                //////////////////////////////////////////

                //first we check to see if there is a waitfor to consider
                var nextStage = cur.currentStage + 1;


                if (cur.waitfor[nextStage] != null) {

                    // consult the given part...
                    var components = cur.waitfor[nextStage].split(" ");

                    var omit = false;
                    var max = components.length;
                    for (var i = 0; i < max; i += 2) {

                        var typeToWaitFor = components[i];
                        var stageToWaitFor = components[i + 1];

                        var skip = false;

                        // stop when we start seeing null values
                        if (typeToWaitFor != null && stageToWaitFor != null) {

                            if (this.parts[typeToWaitFor].currentStage < stageToWaitFor) {
                                omit = true;


                            } else {


                            }

                        }

                    }

                    if (omit == false) {

                        outputParts.push(cur.name);
                    }

                } else {

                    outputParts.push(cur.name);

                }


            } else {

            }
        }

        // debug thing
        for (var i in outputParts) {
            //alert(outputParts[i]);
        }

        return outputParts;

    }



}

function Part() {
    this.name = "";
    this.currentStage = 0;
    this.maxStage = 0;
    this.tgPoint = 99999;
    this.tgShade = new Array(7);

    this.waitfor = new Array();
    this.stages = preloadStages(200);


}

function WaitingArray() {

}


function Stages() {
    this.tf;
    this.status;
    this.short;
    //this.waitfor;
}

function debugPart(part) {

    var output = "<debug>";

    output += "<b>Current Stage:  </b>" + part.currentStage + "<br>";;
    output += "<b>max Stage:  </b>" + part.maxStage + "<br>";;
    output += "<b>tgStage:  </b>" + part.tgStage + "<br>";;

    output += "<b>--tf--</b><br>";

    //for each tf
    for (var i = 0; i <= part.maxStage; i++) {
        output += "<b>" + i + ":  </b>" + part.tf[i] + "<br>";
    }

    output += "<b>--status--</b><br>";
    //for each status
    for (var i = 0; i <= part.maxStage; i++) {
        output += "<b>" + i + ":  </b>" + part.status[i] + "<br>";
    }

    output += "<b>--short--</b><br>";
    //for each short
    for (var i = 0; i <= part.maxStage; i++) {
        output += "<b>" + i + ":  </b>" + part.short[i] + "<br>";
    }

    output += "</debug>";
    return output;

}

// returns the gender of a specific part, taking into consideration local feminization
function getPartGender(char, part) {

    output = "";

    if (part.currentStage >= part.tgPoint) {

        if (char.startGender == "male") {
            output = "female";
        } else {
            output = "male";
        }

        return output;

    } else {
        return char.startGender;
    }

}

function checkAndSwapGender(char) {


    if (char.parts["groin"].currentStage > char.parts["groin"].tgPoint) {



        if (char.gender == char.startGender) {

            if (char.gender == "male") {
                char.gender = "female";

            } else if (char.gender == "female") {
                char.gender = "male";
            }

        }
    }
}



function preloadStages(length) {
    var array = new Array(length);

    for (var x = 0; x < array.length; x++) {
        array[x] = new Stages();
    }
    return array;
}



