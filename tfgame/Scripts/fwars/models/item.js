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

var Item = function (type, formerName) {
    this.type = type;
    this.id = itemIdNext;

    if (typeof formerName === undefined) {
        this.oldName = '';
    } else {
        this.oldName = formerName;
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



