var onlineChatters = [];
var lowActivityThreshold = 30000; // time before name turns from green to red without activity.  In milliseconds.

function addUserPing(name) {
    var refresh = false;
    for (var i = 0; i < onlineChatters.length; i++) {
        if (onlineChatters[i].name == name) {
            onlineChatters[i].time = Date.now();
            refresh = true;
        }
    }

    if (refresh == false) {

        var newUser = {
            name: name,
            time: Date.now(),
        };

        if (name != "") {
            onlineChatters.push(newUser);
        }
    }

    renderUserList();
}

setInterval(function () { renderUserList(); }, 3000);

function renderUserList() {

    var lowActivity = [];
    var highActivity = [];

    var onlinePanel = $(".userlist-chat");

    onlinePanel.html("");

    // filter into recent talkers and nonrecent talkers
    for (var i = 0; i < onlineChatters.length; i++) {
        if (Date.now() - onlineChatters[i].time < lowActivityThreshold) {
            highActivity.push(onlineChatters[i]);
        } else {
            lowActivity.push(onlineChatters[i]);
        }
    }

    // render to userlist panel
    for (var i = 0; i < highActivity.length; i++) {
        onlinePanel.append("<span class='good userlistRow'>" + highActivity[i].name + "</span></br>");
    }
    for (var i = 0; i < lowActivity.length; i++) {
        onlinePanel.append("<span class='bad userlistRow'>" + lowActivity[i].name + "</span></br>");
    }

    // assign double click to print out the @ reference
    $('.userlistRow').each(function () {
        $(this).dblclick(function () {
            $('#message').val("@" + $(this).html() + "  " + $('#message').val());
            $('#message').focus();
        });
    });

}

