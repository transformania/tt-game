// --------------------- ITEM GROUP --------------------

var ItemGroup = function (type) {
    this.items = [];
}

ItemGroup.prototype.addItem = function (newitem) {
    this.items.push(newitem);
};

ItemGroup.prototype.removeItem = function (itemToRemove) {
    var removalIndex = this.items.indexOf(itemToRemove);
    this.items.splice(removalIndex, 1);
};

ItemGroup.prototype.mergeIntoGroup = function (otherGroup) {
    for (var i = 0; i < otherGroup.items.length; i++) {
        this.addItem(otherGroup.items[i]);
    }

    otherGroup.items = [];

};

// --------------------- ITEM --------------------

/*
    type -- item type / name, ie 'Cotton Panties.'  Must match up with something in item statics
    oldName -- name of human who got turned into this.  Empty string if it is store bought or other
    modifier -- modifier type, either Cute or Sexy for female and Rough / Classy for male.
    modifierTier -- level of modifier
*/

var Item = function (type, formerName) {
    this.type = type;
    this.id = itemIdNext;

    if (typeof formerName === undefined) {
        this.oldName = '';
    } else {
        this.oldName = formerName;
    }

    this.modifier = "";
    this.modifierTier = 0;

    // make a roll to assign this item a modifier and tier
    var targetGender = itemStatsMap[this.type].targetGender;
    var roll = Math.random();

    var WORSE = .04;
    var BETTER = .14 + WORSE;

    if (targetGender == "female") {
        if (roll < WORSE) {
            this.modifier = "sexy";
        } else if (roll < BETTER) {
            this.modifier = "cute";
        }
    } else {
        if (roll < WORSE) {
            this.modifier = "rough";
        } else if (roll < BETTER) {
            this.modifier = "classy";
        }
    }

    //if (roll < .18) {
    //    var roll2 = Math.random();
    //    if (roll2 < .01) {
    //        this.modifierTier = 0;
    //    } else if (roll2 < .01) {
    //        this.modifierTier = 0;
    //    }
    //}
    
    itemIdNext++;
}

Item.prototype.getVictimName = function () {
    if (this.oldName != '') {
        return '( ' + this.oldName + ' )';
    } else {
        return '';
    }
};

Item.prototype.getModifier = function () {
    if (this.modifier == '') {
        return "";
    } else {

        // var output = "T" + this.modifierTier.toString() + " ";
        var output = "";

        if (this.modifier == "cute") {
            output += "Cute";
        } else if (this.modifier == "sexy") {
            output += "Sexy";
        } else if (this.modifier == "rough") {
            output += "Rough";
        } else if (this.modifier == "classy") {
            output += "Classy";
        }

        return output;
    }
};

Item.prototype.getBasePrice = function () {
    return itemStatsMap[this.type].value;
}

Item.prototype.getSellPrice = function () {
    return Math.floor(itemStatsMap[this.type].value * .75);
}

Item.prototype.isSpell = function () {
    return Math.floor(itemStatsMap[this.type].isSpell);
}

Item.prototype.getGraphic = function () {
    return itemStatsMap[this.type].graphic;
}



