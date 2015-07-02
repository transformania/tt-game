var onlineChatters = [];

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

    for (var i = 0; i < onlineChatters.length; i++) {
        if (Date.now() - onlineChatters[i].time < 30000) {
            highActivity.push(onlineChatters[i]);
        } else {
            lowActivity.push(onlineChatters[i]);
        }
    }



    for (var i = 0; i < highActivity.length; i++) {
        onlinePanel.append("<span class='good'>" + highActivity[i].name + "</span></br>");
    }

    for (var i = 0; i < lowActivity.length; i++) {
        onlinePanel.append("<span class='bad'>" + lowActivity[i].name + "</span></br>");
    }



}

