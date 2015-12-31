var charactersToLoad = 0;
var charactersLoaded = 0;

function CharacterLoader() {
	this.rawText;
	this.characterCount = 0;
	//this.characters = [new Character(),new Character(),new Character(),new Character(),new Character()];
	this.characters = preloadCharacters(2);
	this.currChar = 0;
	
	this.loadData = loadData;
	function loadData(inputText) {
	
		//addNewCharactersToArray(50,this.characters);
		
		
	
		this.rawText = inputText;
		crudeLines = new Array();
		
		crudeLines = parseLines(this.rawText);
		processedLines = processLines(crudeLines);
		
		//try {
		
		loadLinesIntoCharacters(processedLines,this.characters);
		
		//} catch (err){
			//alert("out of index! " + this.characters[0].firstName + " " + this.currChar + " " + err.message);
			
			
			
			
		//}
		
	}	
	
	this.parseLines = parseLines;
	function parseLines(text) {
	
		var cursorPos = 0;
		var lineArray = new Array();
		var lineNo = 0;
		var linePos = 0;
		
		while (cursorPos<text.length) {
			
			var line = "";
	
			while (text[cursorPos]!=';') {
				
				line = line + text.charAt(cursorPos);
		
				linePos++;
				cursorPos++;
				
				if (cursorPos>text.length) {
					break;
				}
			}
			
			if (text[cursorPos]==';') {
				lineArray[lineNo] = line;
				cursorPos++;
				lineNo++;
				linePos = 0;
				line = "";
			}
			
			if (cursorPos>text.length) {
				break;
			}
				
		}
		
		return lineArray;
		
	}
	
	this.processLines = processLines;
	function processLines(crudeLines) {
	
		processedLines = new Array();
	
		// remove every tab character
		for (var i = 0; i < crudeLines.length; i++) {
			crudeLines[i] = crudeLines[i].replace("\t",""); 
		}
		
		// remove leading whitespaces
		for (var i = 0; i < crudeLines.length; i++) {
			crudeLines[i] = crudeLines[i].replace("\t",""); 
		}
		
		return crudeLines;

	}
	
	this.loadLinesIntoCharacters = loadLinesIntoCharacters;
	function loadLinesIntoCharacters(lines, chars) {
		
		try {
		
	
		var tag = 0;
		var value = 1;
		var currChar=-1;
		var stage = 0;
		var partType = "";
		
		
		for (var i = 0; i < lines.length; i++) {
			
			
			lines[i] =  removeLeadingWhiteSpaces(lines[i]);
			
			newtags = splitTag(lines[i]);
			
			newtags[value] = removeLeadingWhiteSpaces(newtags[value]);
			newtags[tag] = removeLeadingWhiteSpaces(newtags[tag]);
			
			lineValue = newtags[value];
			
			if (lines[i].lastIndexOf("#") == 0) {
				
			
			// load up general character attributes
			} else if (lines[i].lastIndexOf("[CHARACTER_NUM]") != -1) {
				
				currChar = newtags[value];
				
				expandCharacterArray(currChar, chars);
				charactersToLoad++;
				
				try {
				
				// find next free slot
				while (chars[currChar].firstName!="MISSING") {
					
					currChar++;
					expandCharacterArray(currChar, chars);	
						
				}
				
				} catch (err) {
					alert(currChar);
				}
				
				
				
					
			} else if (lines[i].lastIndexOf("[FirstName]") != -1) {
				chars[currChar].firstName = newtags[value];
			} else if (lines[i].lastIndexOf("[LastName]") != -1) {
				chars[currChar].lastName = newtags[value];
			} else if (lines[i].lastIndexOf("[FirstNameNew]") != -1) {
				chars[currChar].firstNameNew = newtags[value];
			} else if (lines[i].lastIndexOf("[LastNameNew]") != -1) {
				chars[currChar].lastNameNew = newtags[value];
			} else if (lines[i].lastIndexOf("[CharType]") != -1) {
				chars[currChar].useableTypes = newtags[value];	
			} else if (lines[i].lastIndexOf("[Gender]") != -1) {
				chars[currChar].startGender = newtags[value];
				chars[currChar].gender = newtags[value];
			} else if (lines[i].lastIndexOf("[Desc]") != -1) {
				chars[currChar].description = newtags[value];
			} else if (lines[i].lastIndexOf("[LoseDesc]") != -1) {
				chars[currChar].losedescription = newtags[value];	
			} else if (lines[i].lastIndexOf("[Intel]") != -1) {
				chars[currChar].intelligence = parseInt(newtags[value]);
			} else if (lines[i].lastIndexOf("[Reflex]") != -1) {
				chars[currChar].reflex = parseInt(newtags[value]);
			} else if (lines[i].lastIndexOf("[Prediction]") != -1) {
				chars[currChar].prediction = parseInt(newtags[value]);	
				
			} else if (lines[i].lastIndexOf("[Desire]") != -1) {
				chars[currChar].desire = parseInt(newtags[value]);
				
			} else if (lines[i].lastIndexOf("[Author]") != -1) {
				chars[currChar].author = newtags[value];
			} else if (lines[i].lastIndexOf("[AuthorHome]") != -1) {
				chars[currChar].authorHome = newtags[value];
			} else if (lines[i].lastIndexOf("[Tags]") != -1) {
				chars[currChar].tags = newtags[value];
			// select the level of transformation	
			} else if (lines[i].lastIndexOf("[STAGE]") != -1) {
				stage = parseInt(newtags[value]);
				
			// select which body part to focus on next	
			} else if (lines[i].lastIndexOf("[HAIR]") != -1) {
				partType = "hair";
			} else if (lines[i].lastIndexOf("[FACE]") != -1) {
				partType = "face";
			} else if (lines[i].lastIndexOf("[VOICE]") != -1) {
				partType = "voice";
			} else if (lines[i].lastIndexOf("[BREASTS]") != -1) {
				partType = "breasts";
			} else if (lines[i].lastIndexOf("[BREASTS]") != -1) {
				partType = "chest";
			} else if (lines[i].lastIndexOf("[BODY]") != -1) {
				partType = "body";
			} else if (lines[i].lastIndexOf("[SHOULDERS]") != -1) {
				partType = "shoulders";
			} else if (lines[i].lastIndexOf("[WAIST]") != -1) {
				partType = "waist";
			} else if (lines[i].lastIndexOf("[BUTT]") != -1) {
				partType = "butt";
			} else if (lines[i].lastIndexOf("[LEGS]") != -1) {
				partType = "legs";
			} else if (lines[i].lastIndexOf("[FEET]") != -1) {
				partType = "feet";
			} else if (lines[i].lastIndexOf("[HANDS]") != -1) {
				partType = "hands";
			} else if (lines[i].lastIndexOf("[ARMS]") != -1) {
				partType = "arms";
			} else if (lines[i].lastIndexOf("[GROIN]") != -1) {
				partType = "groin";
			} else if (lines[i].lastIndexOf("[MIND]") != -1) {
				partType = "mind";
			} else if (lines[i].lastIndexOf("[SHIRT]") != -1) {
				partType = "shirt";
			} else if (lines[i].lastIndexOf("[PANTS]") != -1) {
				partType = "pants";
			} else if (lines[i].lastIndexOf("[SHOES]") != -1) {
				partType = "shoes";
			} else if (lines[i].lastIndexOf("[ACCESSORY]") != -1) {
				partType = "accessory";
			} else if (lines[i].lastIndexOf("[CUSTOM1]") != -1) {
				partType = "custom1";
			} else if (lines[i].lastIndexOf("[CUSTOM2]") != -1) {
				partType = "custom2";
			} else if (lines[i].lastIndexOf("[CUSTOM3]") != -1) {
				partType = "custom3";
			} else if (lines[i].lastIndexOf("[CUSTOM4]") != -1) {
				partType = "custom4";
			} else if (lines[i].lastIndexOf("[CUSTOM5]") != -1) {
				partType = "custom5";
			}
			
			else if (lines[i].lastIndexOf("[WaitFor]") != -1) {
				
				var parts = newtags[value].split(" ");
				
				var bpart = parts[0]
				var lvl = parts[1];
				
				//alert(bpart + "," + lvl);
				
				//var i = chars[currChar].parts[partType].waitfor[stage].length;
				//var i = chars[currChar].parts[partType].waitfor.length;
				//alert(i + lines[i]);
				
				if (chars[currChar].parts[partType].waitfor[stage]==null) {
				chars[currChar].parts[partType].waitfor[stage] = bpart + " " + lvl + " ";
				} else {
					chars[currChar].parts[partType].waitfor[stage] +=  bpart + " " + lvl + " ";
				}
				
				
			// lets user choose where feminization occurs for given part
			} else if (lines[i].lastIndexOf("[tgPoint]") != -1) {
				
				var splitMe = lineValue.split(" ");
				var part = splitMe[0];
				var shade = splitMe[1];
				
				chars[currChar].parts[part].tgPoint = stage;

				
			} else if (lines[i].lastIndexOf("[tgShade]") != -1){
				var splitMe = lineValue.split(" ");
				var part = splitMe[0];
				var shade = splitMe[1];
				
				//alert(part + "," + shade);
				chars[currChar].parts[part].tgShade[stage] = shade;
				
			} else if (lines[i].lastIndexOf("[Alias]") != -1){
				
				try {
				
				var splitMe = lineValue.split(" ");
				
				var index = splitMe[0];
				var key = splitMe[1];
				
				chars[currChar].aliases[index] = key;
				
				} catch (e) {
					alert(e);
				}
	
			}
			
			// and finally we need to put the right value in for the part
			else if (lines[i].lastIndexOf("[tf]") != -1) {
				
				if (newtags[value]!="") {
					
					chars[currChar].parts[partType].name = partType;
				
					chars[currChar].parts[partType].stages[stage].tf = lineValue;
					if (stage > chars[currChar].parts[partType].maxStage ) {
						chars[currChar].parts[partType].maxStage = stage;
						
					}
				}
			
				

			} else if (lines[i].lastIndexOf("[status]") != -1) {
				
				try {
				
				if (newtags[value]!="") {
					
					chars[currChar].parts[partType].name = partType;
					
					chars[currChar].parts[partType].name = partType;
					//alert(chars[currChar].parts[partType].name);
					
					chars[currChar].parts[partType].stages[stage].status = lineValue;
					if (stage > chars[currChar].parts[partType].maxStage ) {
						chars[currChar].parts[partType].maxStage = stage;
						
					}
				}
				
				} catch (err) {
					alert(lines[i]);
				}
				
				

			} else if (lines[i].lastIndexOf("[short]") != -1) {
				
				if (newtags[value]!="") {
					
					chars[currChar].parts[partType].name = partType;
					
					chars[currChar].parts[partType].stages[stage].short = lineValue;
					if (stage > chars[currChar].parts[partType].maxStage ) {
						chars[currChar].parts[partType].maxStage = stage;
						
					}
				}
				
				

			}
			
			// we don't recognize this line, so throw an error
			else {
				if ((lines[i].lastIndexOf("#")==-1) && (lines[i].length>0)) {
					alert("Error parsing line:  *" + lines[i] + "*");
				}
			}
			
			
			
			
			

		} // ends big-ass for loop
		
		} catch (err){
			alert("Unpecified error.  Last line:  '" + lines[i] + "'.  Error: " + err);
		}
		
	}
	
	this.calculateMaxTFPoints = calculateMaxTFPoints;
	function calculateMaxTFPoints(i) {
		
		
		var output= new Number(0);
	
		for (var p in this.characters[i].parts)
		{
			var output = output*1 + parseInt(this.characters[i].parts[p].maxStage);	
		}
		
		this.characters[i].currentTFPoints = output;
		this.characters[i].maxTFPoints = output;
		
		return output;
	
	}
	
	this.splitTag = splitTag;
	function splitTag(line) {
	
		output = new Array();
	
		if (line.indexOf("[")!=-1) {
			var output = line.split("]");
			var first = output[0];
			var second = output[1];
		
			first = first + "]"; // add back in, got removed in split

			output[0] = first;
			output[1] = second;
		
		} else {
			output[0]="PARSE ERROR";
			output[1]="PARSE ERROR";
		}
	
		return output;
	
	}
	
	this.addNewCharactersToArray = addNewCharactersToArray;
	function addNewCharactersToArray(num, chars) {
		for (var i = 0; i < num;i++) {
			chars[0] = new Character();
		}
	}
	
	this.expandCharacterArray = expandCharacterArray;
 	function expandCharacterArray(index, chars) {
		

		var addMore = chars[index] instanceof Character;
		//alert(addMore + " " + index);
		
		if (addMore==false) {
			//alert("adding... " + index);
			chars[index] = new Character();
			
		} else if (chars[index] instanceof Character) {
			//alert("is instance!  " + index);
		}
	}
	
	this.nextEmptyCharSlot = nextEmptyCharSlot;
	function nextEmptyCharSlot(chars) {
		for (var i = 0; i < chars.characters.length; i++) {
			if (chars.characters[i] instanceof Character) {
				//alert("xxx");
				if (chars.characters[i].firstName=="MISSING") {
					return i;
				}
				
			}
		}
		
		return -1;
	}
	
}

function removeLeadingWhiteSpaces(text) {

	var pointer = 0;
	
	for (var i = 0; i < text.length; i++) {
		if ((text.charAt(i)==" ") || (text.charAt(i)=="	")) {
			pointer++;
		} else {
			break;
		}
	}
	
	text = text.substr(pointer,text.length);
	
	pointer = text.length-1;
	for (var i = text.length - 1; i >= 0; i--) {
		if ((text.charAt(i)==" ") || (text.charAt(i)=="	")) {
			pointer--;
		} else {
			break;
		}
	}
	
	text = text.substr(0,pointer+1);
	return text;
}

function preloadCharacters(length) {
	
	 var array = new Array(length);

        for (var x=0; x<array.length; x++) {
		
	    //if ( !array[x] instanceof Character) {
		array[x]=new Character();
	    //}
        }

        return array;
	
}
