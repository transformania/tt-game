// --------------------- ITEM GROUP --------------------

var ItemGroup = function (type) {
    this.items = [];
}

ItemGroup.prototype.addItem = function (newitem) {
    console.log(newitem);
    this.items.push(newitem);
};

ItemGroup.prototype.removeItem = function (itemToRemove) {
    var removalIndex = this.items.indexOf(itemToRemove);
    this.items.splice(removalIndex, 1);
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