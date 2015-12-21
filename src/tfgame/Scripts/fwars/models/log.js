// --------------------- LOG --------------------

var Log = function (message, outcome) {

    if (typeof outcome === undefined) {
        outcome = "";
    }

    this.id = logsIdNext;
    logsIdNext++;
    this.message = message;
    this.outcome = outcome;
}