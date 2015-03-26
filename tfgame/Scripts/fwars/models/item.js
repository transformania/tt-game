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
    quality -- power modifier.  "default", ""
*/

var Item = function (type, formerName, quality) {
    this.type = type;
    this.id = itemIdNext;

    if (typeof formerName === undefined) {
        this.oldName = '';
    } else {
        this.oldName = formerName;
    }

    if (typeof quality === undefined) {
        this.quality = 'default';
    } else {
        this.quality = quality;
    }

    itemIdNext++;
}

Item.prototype.getVictimName = function () {
    if (this.oldName != '') {
        return '( ' + this.oldName + ' )';
    } else {
        return '';
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



